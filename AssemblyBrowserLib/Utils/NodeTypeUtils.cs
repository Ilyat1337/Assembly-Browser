using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Reflection;

namespace AssemblyBrowserLib.Utils
{
    class NodeTypeUtils
    {
        public static NodeType GetNodeTypeByMemberInfo(MemberInfo memberInfo)
        {
            if (memberInfo is Type)
            {
                Type type = memberInfo as Type;
                if (type.IsSubclassOf(typeof(Delegate)) || type.IsSubclassOf(typeof(MulticastDelegate)))
                    return NodeType.Delegate;
                if (type.IsClass)
                    return NodeType.Class;
                if (type.IsInterface)
                    return NodeType.Interface;
                if (type.IsEnum)
                    return NodeType.Enum;
                if (type.IsValueType)
                    return NodeType.Struct;
                return (NodeType)(-1);
            }
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Constructor:
                case MemberTypes.Method:
                    return NodeType.Method;
                case MemberTypes.Field:
                    return NodeType.Field;
                case MemberTypes.Property:
                    return NodeType.Property;
                case MemberTypes.Event:
                    return NodeType.Event;
            }
            return (NodeType)(-1);
        }
    }
}
