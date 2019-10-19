using Dahomey.Cbor.Serialization;
using Dahomey.Cbor.Serialization.Conventions;
using Dahomey.Cbor.Serialization.Converters.Mappings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var memberMappings = new List<MemberMapping>();

            _defaultObjectMappingConvention.Apply<T>(registry, objectMapping);

            objectMapping.ClearMemberMappings();
            var props = typeof(T).GetProperties();
            foreach (var p in props)
            {
                var shouldSerialize = true;

                if (p.GetCustomAttributes().Where(x => x.GetType() == typeof(SerializationOutputAttribute)).ToArray().Length == 0)
                {
                    // field or property has no attribute
                    switch (_outputMode)
                    {
                        case OutputMode.None:
                            shouldSerialize = false; // to archive zero output when OutputMode == None
                            break;
                        case OutputMode.Hash:
                            break;
                        case OutputMode.Api:
                            break;
                        case OutputMode.Wire:
                            break;
                        case OutputMode.Persist:
                            break;
                        case OutputMode.All:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // Attribute is present
                    foreach (var attr in p.GetCustomAttributes())
                    {
                        if (attr.GetType() != typeof(SerializationOutputAttribute)) continue;

                        var oa = (SerializationOutputAttribute)attr;
                        switch (_outputMode)
                        {
                            case OutputMode.None:
                            {
                                shouldSerialize = false;
                                break;
                            }
                            case OutputMode.Hash:
                            {
                                if (oa.ValidOn.Contains(OutputMode.Hash) || oa.ValidOn.Contains(OutputMode.All))
                                    shouldSerialize = true;
                                else shouldSerialize = false;

                                break;
                            }
                            case OutputMode.Api:
                            {
                                if (oa.ValidOn.Contains(OutputMode.Api) || oa.ValidOn.Contains(OutputMode.All))
                                        shouldSerialize = true;
                                else shouldSerialize = false;

                                break;
                            }
                            case OutputMode.Wire:
                            {
                                if (oa.ValidOn.Contains(OutputMode.Wire) || oa.ValidOn.Contains(OutputMode.All))
                                        shouldSerialize = true;
                                else shouldSerialize = false;

                                break;
                            }
                            case OutputMode.Persist:
                            {
                                if (oa.ValidOn.Contains(OutputMode.Persist) || oa.ValidOn.Contains(OutputMode.All))
                                        shouldSerialize = true;
                                else shouldSerialize = false;

                                break;
                            }
                            case OutputMode.All:
                            {
                                shouldSerialize = true;
                                if (oa.ValidOn.Contains(OutputMode.None))
                                        shouldSerialize = false;

                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                if (shouldSerialize)
                {
                    var memberMapping = new MemberMapping(registry.ConverterRegistry, objectMapping, p, p.PropertyType);
                    ProcessShouldSerializeMethod(memberMapping);
                    memberMappings.Add(memberMapping);
                }
            }

            objectMapping.SetMemberMappings(objectMapping.MemberMappings.OrderBy(m => m.MemberInfo.Name).ToList());
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
