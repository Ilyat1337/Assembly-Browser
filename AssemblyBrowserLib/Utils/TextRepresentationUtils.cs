using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyBrowserLib.Utils
{
    class TextRepresentationUtils
    {
        private const string DELEGATE_INVOKE_METHOD_NAME = "Invoke";

        public delegate string ToTextRepresentation(MemberInfo memberInfo);

        private static Dictionary<NodeType, ToTextRepresentation> textGenerators;

        static TextRepresentationUtils()
        {
            textGenerators = new Dictionary<NodeType, ToTextRepresentation>();
            textGenerators.Add(NodeType.Class, ClassToText);
            textGenerators.Add(NodeType.Interface, ClassToText);
            textGenerators.Add(NodeType.Struct, ClassToText);
            textGenerators.Add(NodeType.Enum, ClassToText);
            textGenerators.Add(NodeType.Property, PropertyToText);
            textGenerators.Add(NodeType.Field, FieldToText);
            textGenerators.Add(NodeType.Method, MethodToText);
            textGenerators.Add(NodeType.Delegate, DelegateToText);
            textGenerators.Add(NodeType.Event, EventToText);
        }

        public static string GetTextRepresentationFor(MemberInfo memberInfo, NodeType nodeType)
        {
            return textGenerators.ContainsKey(nodeType) ? textGenerators[nodeType].Invoke(memberInfo) : null;
        }

        private static string ClassToText(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }

        private static string PropertyToText(MemberInfo memberInfo)
        {
            PropertyInfo propertyInfo = memberInfo as PropertyInfo;
            return $"{propertyInfo.Name}: {propertyInfo.PropertyType.Name}";
        }

        private static string FieldToText(MemberInfo memberInfo)
        {
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            return $"{fieldInfo.Name}: {fieldInfo.FieldType.Name}";
        }

        private static string MethodToText(MemberInfo memberInfo)
        {
            MethodBase methodBase = memberInfo as MethodBase;
            string methodText = string.Format("{0}({1})", methodBase.Name, 
                string.Join(", ", methodBase.GetParameters().Select(o => o.ParameterType.Name).ToArray()));
            return methodBase is MethodInfo ? methodText + $": {(methodBase as MethodInfo).ReturnType.Name}" : methodText;
        }

        private static string DelegateToText(MemberInfo memberInfo)
        {
            Type delegateType = memberInfo as Type;
            MethodBase invokeMethod = delegateType.GetMethod(DELEGATE_INVOKE_METHOD_NAME);
            return string.Format("{0}({1}): {2}", delegateType.Name,
                string.Join(", ", invokeMethod.GetParameters().Select(o => o.ParameterType.Name).ToArray()),
                (invokeMethod as MethodInfo).ReturnType.Name);
        }

        private static string EventToText(MemberInfo memberInfo)
        {
            EventInfo eventInfo = (EventInfo)memberInfo;
            MethodInfo invokeMethodInfo = eventInfo.EventHandlerType.GetMethod(DELEGATE_INVOKE_METHOD_NAME);
            return string.Format("{0}: {1}({2})", eventInfo.Name, eventInfo.EventHandlerType.Name,
                string.Join(", ", invokeMethodInfo.GetParameters().Select(o => o.ParameterType.Name).ToArray()));
        }
    }
}