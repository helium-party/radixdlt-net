using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Dahomey.Cbor.Attributes;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonObjectMappingConvention : IObjectMappingConvention
    {
        private readonly INamingConvention _dsonNamingConvention = new CamelCaseNamingConvention();
        private readonly OutputMode _outputMode;
        
        public DsonObjectMappingConvention(OutputMode outputMode)
        {
            _outputMode = outputMode;
        }

        public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
        {
            var type = objectMapping.ObjectType;
            var memberMappings = new List<MemberMapping<T>>();

            var discriminatorAttribute = type.GetCustomAttribute<CborDiscriminatorAttribute>();
            if (discriminatorAttribute != null)
            {
                objectMapping.SetDiscriminator(discriminatorAttribute.Discriminator);
                objectMapping.SetDiscriminatorPolicy(discriminatorAttribute.Policy);
            }

            var namingConventionType = type.GetCustomAttribute<CborNamingConventionAttribute>()?.NamingConventionType;
            if (namingConventionType != null)
            {
                objectMapping.SetNamingConvention((INamingConvention)Activator.CreateInstance(namingConventionType));
            }

            var lengthModeAttribute = type.GetCustomAttribute<CborLengthModeAttribute>();
            if (lengthModeAttribute != null)
            {
                objectMapping.SetLengthMode(lengthModeAttribute.LengthMode);
            }

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var p in props)
            {
                var serializationAttributes = p.GetCustomAttributes().OfType<SerializationOutputAttribute>();
                if (!serializationAttributes.Any())
                {
                    if (_outputMode == OutputMode.None)
                        continue;
                }
                else
                {
                    // Property should be excluded from serialization
                    if (serializationAttributes.Any(a => a.ValidOn.Contains(OutputMode.None)))
                        continue;

                    // For 'OutputMode.All', every property (except the ones with 'OutputMode.None' will be serialized)
                    if (_outputMode != OutputMode.All)
                    {
                        // Check for matching output mode or OutputMode.All
                        if (!serializationAttributes.Any(a => a.ValidOn.Contains(_outputMode) || a.ValidOn.Contains(OutputMode.All)))
                            continue;
                    }
                }

                var memberMapping = new MemberMapping<T>(registry.ConverterRegistry, objectMapping, p, p.PropertyType);
                ProcessDefaultValue(p, memberMapping);
                ProcessShouldSerializeMethod(memberMapping);
                ProcessLengthMode(p, memberMapping);
                ProcessRequired(p, memberMapping);
                memberMappings.Add(memberMapping);
            }

            objectMapping.AddMemberMappings(memberMappings);

            var constructorInfos = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var constructorInfo = constructorInfos.FirstOrDefault(c => c.IsDefined(typeof(CborConstructorAttribute)));

            if (constructorInfo != null)
            {
                CborConstructorAttribute constructorAttribute = constructorInfo.GetCustomAttribute<CborConstructorAttribute>();
                CreatorMapping creatorMapping = objectMapping.MapCreator(constructorInfo);
                if (constructorAttribute.MemberNames != null)
                {
                    creatorMapping.SetMemberNames(constructorAttribute.MemberNames);
                }
            }
            // if no default constructor, pick up first one
            else if (constructorInfos.Length > 0 && !constructorInfos.Any(c => c.GetParameters().Length == 0))
            {
                constructorInfo = constructorInfos[0];
                objectMapping.MapCreator(constructorInfo);
            }

            var methodInfo = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(OnDeserializingAttribute)));
            if (methodInfo != null)
            {
                objectMapping.SetOnDeserializingMethod(GenerateCallbackDelegate<T>(methodInfo));
            }
            else if (type.GetInterfaces().Any(i => i == typeof(ISupportInitialize)))
            {
                objectMapping.SetOnDeserializingMethod(t => ((ISupportInitialize)t).BeginInit());
            }

            methodInfo = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(OnDeserializedAttribute)));
            if (methodInfo != null)
            {
                objectMapping.SetOnDeserializedMethod(GenerateCallbackDelegate<T>(methodInfo));
            }
            else if (type.GetInterfaces().Any(i => i == typeof(ISupportInitialize)))
            {
                objectMapping.SetOnDeserializedMethod(t => ((ISupportInitialize)t).EndInit());
            }

            methodInfo = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(OnSerializingAttribute)));
            if (methodInfo != null)
            {
                objectMapping.SetOnSerializingMethod(GenerateCallbackDelegate<T>(methodInfo));
            }

            methodInfo = type.GetMethods().FirstOrDefault(m => m.IsDefined(typeof(OnSerializedAttribute)));
            if (methodInfo != null)
            {
                objectMapping.SetOnSerializedMethod(GenerateCallbackDelegate<T>(methodInfo));
            }

            objectMapping.SetNamingConvention(_dsonNamingConvention);
        }

        private void ProcessDefaultValue<T>(MemberInfo memberInfo, MemberMapping<T> memberMapping) where T : class
        {
            var defaultValueAttribute = memberInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultValueAttribute != null)
            {
                memberMapping.SetDefaultValue(defaultValueAttribute.Value);
            }

            if (memberInfo.IsDefined(typeof(CborIgnoreIfDefaultAttribute)))
            {
                memberMapping.SetIngoreIfDefault(true);
            }
        }

        private void ProcessShouldSerializeMethod<T>(MemberMapping<T> memberMapping) where T : class
        {
            string shouldSerializeMethodName = "ShouldSerialize" + memberMapping.MemberInfo.Name;
            Type objectType = memberMapping.MemberInfo.DeclaringType;

            MethodInfo shouldSerializeMethodInfo = objectType.GetMethod(shouldSerializeMethodName, new Type[] { });
            if (shouldSerializeMethodInfo != null &&
                shouldSerializeMethodInfo.IsPublic &&
                shouldSerializeMethodInfo.ReturnType == typeof(bool))
            {
                // obj => ((TClass) obj).ShouldSerializeXyz()
                ParameterExpression objParameter = Expression.Parameter(typeof(object), "obj");
                Expression<Func<object, bool>> lambdaExpression = Expression.Lambda<Func<object, bool>>(
                    Expression.Call(
                        Expression.Convert(objParameter, objectType),
                        shouldSerializeMethodInfo),
                    objParameter);

                memberMapping.SetShouldSerializeMethod(lambdaExpression.Compile());
            }
        }

        private void ProcessLengthMode<T>(MemberInfo memberInfo, MemberMapping<T> memberMapping) where T : class
        {
            var lengthModeAttribute = memberInfo.GetCustomAttribute<CborLengthModeAttribute>();
            if (lengthModeAttribute != null)
            {
                memberMapping.SetLengthMode(lengthModeAttribute.LengthMode);
            }
        }

        private void ProcessRequired<T>(MemberInfo memberInfo, MemberMapping<T> memberMapping) where T : class
        {
            var jsonRequiredAttribute = memberInfo.GetCustomAttribute<CborRequiredAttribute>();
            if (jsonRequiredAttribute != null)
            {
                memberMapping.SetRequired(jsonRequiredAttribute.Policy);
            }
        }

        private Action<T> GenerateCallbackDelegate<T>(MethodInfo methodInfo)
        {
            // obj => obj.Callback()
            ParameterExpression objParameter = Expression.Parameter(typeof(T), "obj");
            Expression<Action<T>> lambdaExpression = Expression.Lambda<Action<T>>(
                Expression.Call(
                    Expression.Convert(objParameter, typeof(T)),
                    methodInfo),
                objParameter);

            return lambdaExpression.Compile();
        }
    }

    public class DsonObjectMappingConventionProvider : IObjectMappingConventionProvider
    {
        private readonly IObjectMappingConvention _objectMappingConvention;

        public DsonObjectMappingConventionProvider(OutputMode mode)
        {
            _objectMappingConvention = new DsonObjectMappingConvention(mode);
        }

        public IObjectMappingConvention GetConvention(Type type)
        {
            return _objectMappingConvention;
        }
    }
}
