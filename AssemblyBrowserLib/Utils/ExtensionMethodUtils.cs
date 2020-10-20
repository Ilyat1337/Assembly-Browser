using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace AssemblyBrowserLib.Utils
{
    class ExtensionMethodUtils
    {
        internal static bool IsextensionMethod(MemberInfo member)
        {
            return member is MethodBase && (member as MethodBase).GetCustomAttribute<ExtensionAttribute>() != null;
        }

        internal static void ChangeNodeTypeToExtensionMethod(AssemblyNode node)
        {
            node.NodeType = NodeType.ExtensionMethod;
        }

        internal static MemberInfo GetExtensionMethodClass(MemberInfo member)
        {
            return (member as MethodBase).GetParameters()[0].ParameterType;
        }
    }
}
