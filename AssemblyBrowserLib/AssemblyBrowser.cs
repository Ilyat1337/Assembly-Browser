using AssemblyBrowserLib.AssemblyTree;
using AssemblyBrowserLib.exceptions;
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
        private class ExtMethodInfo
        {
            public MemberInfo ParentClass
            { get; set; }

            public AssemblyNode Node
            { get; set; }

            public AssemblyNode ParentNode
            { get; set; }
        }

        private Assembly assembly;
        private Dictionary<string, List<AssemblyNode>> namespaceToTypesMap;
        private List<ExtMethodInfo> extensionMethods;
        private Dictionary<MemberInfo, AssemblyNode> classesMap;

        public AssemblyBrowser()
        {            
            namespaceToTypesMap = new Dictionary<string, List<AssemblyNode>>();
            extensionMethods = new List<ExtMethodInfo>();
            classesMap = new Dictionary<MemberInfo, AssemblyNode>();
        }

        public void LoadAssemblyFromFile(string assemblyFilePath)
        {
            try
            {
                assembly = Assembly.LoadFrom(assemblyFilePath);
            } 
            catch (Exception)
            {
                throw new AssemblyLoadException();
            }
        }

        public AssemblyNode GetAssemblyTree()
        {
            if (assembly == null)
                throw new AssemblyNotLoadedException();
            Type[] assemblyTypes = GetAssemblyTypes(assembly);
            FillNamespaceToTypeMapForTypes(assemblyTypes);
            ResolveExtensionMethods();
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
            catch (Exception)
            {
                assembly = null;
                throw new AssemblyLoadException();
            }

            types = types.Where(type => type != null && !type.IsNested
                && type.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
            if (types.Length == 0)
                throw new AssemblyLoadException();
            return types;
        }

        private void FillNamespaceToTypeMapForTypes(Type[] assemblyTypes)
        {
            namespaceToTypesMap.Clear();
            extensionMethods.Clear();
            classesMap.Clear();

            AssemblyNode rootNode = new AssemblyNode(NodeType.Namespace);
            FillNestedMembers(assemblyTypes, rootNode);
            for (int i = 0; i < rootNode.GetNodes().Count; i++)
            { 
                if (!namespaceToTypesMap.ContainsKey(assemblyTypes[i].Namespace))
                    namespaceToTypesMap.Add(assemblyTypes[i].Namespace, new List<AssemblyNode>());
                namespaceToTypesMap[assemblyTypes[i].Namespace].Add(rootNode.GetNodes()[i]);
            }
        }

        private void FillNestedMembers(MemberInfo[] childMembers, AssemblyNode parentNode)
        {
            foreach (MemberInfo member in childMembers)
            {
                AssemblyNode innerNode = TryMemberInfoToAssemblyNode(member);
                if (innerNode != null)
                {
                    parentNode.AddNode(innerNode);
                    if (innerNode.NodeType == NodeType.Class)
                        classesMap.Add(member, innerNode);
                    else if (ExtensionMethodUtils.IsextensionMethod(member))
                    {
                        ExtensionMethodUtils.ChangeNodeTypeToExtensionMethod(innerNode);
                        extensionMethods.Add(new ExtMethodInfo()
                        {
                            ParentClass = ExtensionMethodUtils.GetExtensionMethodClass(member),
                            Node = innerNode,
                            ParentNode = parentNode
                        });
                    }
                }
            }
        }

        private AssemblyNode TryMemberInfoToAssemblyNode(MemberInfo memberInfo)
        {
            NodeType nodeType = NodeTypeUtils.GetNodeTypeByMemberInfo(memberInfo);
            if (nodeType == (NodeType)(-1))
                return null;
            AssemblyNode assemblyNode = new AssemblyNode(nodeType);
            assemblyNode.AccessModifire = AccessModifireUtils.GetAccessModifireFor(memberInfo);
            assemblyNode.TextRepresentation = TextRepresentationUtils.GetTextRepresentationFor(memberInfo, assemblyNode.NodeType);

            if (CanHaveNestedMembers(assemblyNode))
            {
                FillNestedMembers(GetMemberInfos(memberInfo as Type), assemblyNode);
            }
            return assemblyNode;
        }

        private static bool CanHaveNestedMembers(AssemblyNode assemblyNode)
        {
            return assemblyNode.NodeType >= NodeType.Class && assemblyNode.NodeType <= NodeType.Enum;
        }

        private void ResolveExtensionMethods()
        {
            foreach (ExtMethodInfo extMethodInfo in extensionMethods)
            {
                if (classesMap.ContainsKey(extMethodInfo.ParentClass))
                {
                    extMethodInfo.ParentNode.GetNodes().Remove(extMethodInfo.Node);
                    classesMap[extMethodInfo.ParentClass].GetNodes().Add(extMethodInfo.Node);
                }
            }
        }

        private MemberInfo[] GetMemberInfos(Type type)
        {
            return type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | 
                                    BindingFlags.Public | BindingFlags.NonPublic)
                .Where(member => ((member is MethodBase) ? !(member as MethodBase).IsSpecialName : true) 
                && ((member is FieldInfo) ? !(member as FieldInfo).IsSpecialName : true)
                && ((member is PropertyInfo) ? !(member as PropertyInfo).IsSpecialName : true)
                && member.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
        }
    }

}
