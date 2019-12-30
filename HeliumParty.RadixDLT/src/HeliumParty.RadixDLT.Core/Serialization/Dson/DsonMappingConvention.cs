using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HeliumParty.RadixDLT.Serialization.Dson
{
    public class DsonObjectMappingConvention : IObjectMappingConvention
    {
        private readonly IObjectMappingConvention _defaultObjectMappingConvention = new DefaultObjectMappingConvention();
        private readonly INamingConvention _dsonNamingConvention = new CamelCaseNamingConvention();
        private readonly OutputMode _outputMode;
        
        public DsonObjectMappingConvention(OutputMode outputMode)
        {
            _outputMode = outputMode;
        }

        public void Apply<T>(SerializationRegistry registry, ObjectMapping<T> objectMapping) where T : class
        {
            // TODO add fields?
            var memberMappings = new List<MemberMapping>();

            _defaultObjectMappingConvention.Apply<T>(registry, objectMapping);

            objectMapping.ClearMemberMappings();

            var props = typeof(T)
                    .GetProperties()
                    .Concat(typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance));

            bool shouldSerialize;
            foreach (var p in props)
            {
                // By default, we always want to serialize the property (e.g. if it doesn't have an OutputAttribute)
                shouldSerialize = true;

                var serializationAttributes = p.GetCustomAttributes().OfType<SerializationOutputAttribute>();
                if (serializationAttributes.Count() == 0)
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
                            shouldSerialize = false;
                    }
                }

                if (shouldSerialize)
                {
                    var memberMapping = new MemberMapping(registry.ConverterRegistry, objectMapping, p, p.PropertyType);
                    ProcessShouldSerializeMethod(memberMapping);
                    memberMappings.Add(memberMapping);
                }
            }

            //objectMapping.SetMemberMappings(objectMapping.MemberMappings.OrderBy(m => m.MemberInfo.Name).ToList());
            objectMapping.SetMemberMappings(memberMappings);
            objectMapping.SetNamingConvention(_dsonNamingConvention);
        }

        private void ProcessShouldSerializeMethod(MemberMapping memberMapping)
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
