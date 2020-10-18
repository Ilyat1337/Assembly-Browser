using AssemblyBrowserLib.AssemblyTree;
using AssemblyBrowserLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyBrowserLib
{
    public class  AssemblyBrowser
    {
        private Assembly assembly;
        private Dictionary<string, List<AssemblyNode>> namespaceToTypesMap;

        public AssemblyBrowser()
        {            
            namespaceToTypesMap = new Dictionary<string, List<AssemblyNode>>();
        }

        public void LoadAssemblyFromFile(string assemblyFilePath)
        {
            assembly = Assembly.LoadFile(assemblyFilePath);
        }

        public AssemblyNode GetAssemblyTree()
        {
            Type[] assemblyTypes = GetAssemblyTypes(assembly);
            FillNamespaceToTypeMapForTypes(assemblyTypes);
            return TreeConstructionUtils.ConstructAssemblyTree(namespaceToTypesMap);
        }

        private Type[] GetAssemblyTypes(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            
            return types.Where(type => !type.IsNested
                && type.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
        }

        private void FillNamespaceToTypeMapForTypes(Type[] assemblyTypes)
        {
            namespaceToTypesMap.Clear();
            foreach (Type type in assemblyTypes)
            {
                AssemblyNode assemblyNode = TryMemberInfoToAssemblyNode(type);
                if (assemblyNode != null)
                {
                    if (!namespaceToTypesMap.ContainsKey(type.Namespace))
                        namespaceToTypesMap.Add(type.Namespace, new List<AssemblyNode>());
                    namespaceToTypesMap[type.Namespace].Add(assemblyNode);
                }
            }
        }

        private AssemblyNode TryMemberInfoToAssemblyNode(MemberInfo memberInfo)
        {
            NodeType nodeType = NodeTypeUtils.GetNodeTypeByMemberInfo(memberInfo);
            if (nodeType == 0)
                return null;
            AssemblyNode assemblyNode = new AssemblyNode(nodeType);
            assemblyNode.AccessModifire = AccessModifireUtils.GetAccessModifireFor(memberInfo);
            assemblyNode.TextRepresentation = TextRepresentationUtils.GetTextRepresentationFor(memberInfo, assemblyNode.NodeType);

            if (CanHaveNestedMembers(assemblyNode))
            {
                FillNestedMembers(memberInfo, assemblyNode);
            }
            return assemblyNode;
        }

        private static bool CanHaveNestedMembers(AssemblyNode assemblyNode)
        {
            return assemblyNode.NodeType >= NodeType.Class && assemblyNode.NodeType <= NodeType.Enum;
        }

        private void FillNestedMembers(MemberInfo memberInfo, AssemblyNode assemblyNode)
        {
            MemberInfo[] membersInfo = GetMemberInfos(memberInfo as Type);
            foreach (MemberInfo member in membersInfo)
            {
                AssemblyNode innerNode = TryMemberInfoToAssemblyNode(member);
                if (innerNode != null)
                    assemblyNode.AddNode(innerNode);
            }
        }

        private MemberInfo[] GetMemberInfos(Type type)
        {
            return type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | 
                                    BindingFlags.Public | BindingFlags.NonPublic)
                .Where(member => ((member is MethodBase) ? !(member as MethodBase).IsSpecialName : true) 
                && member.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
        }
    }

}
