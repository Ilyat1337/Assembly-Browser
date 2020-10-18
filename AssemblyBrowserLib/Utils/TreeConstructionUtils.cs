using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Collections.Generic;

namespace AssemblyBrowserLib.Utils
{
    class TreeConstructionUtils
    {
        private static readonly NodeComparer nodeComparer;

        static TreeConstructionUtils()
        {
            nodeComparer = new NodeComparer();
        }

        public static AssemblyNode ConstructAssemblyTree(Dictionary<string, List<AssemblyNode>> namespaceToTypesMap)
        {
            AssemblyNode rootNode = new AssemblyNode(NodeType.Namespace);
            foreach (KeyValuePair<string, List<AssemblyNode>> namespaceToTypes in namespaceToTypesMap)
            {
                AddFoldersStructure(rootNode, namespaceToTypes.Key, namespaceToTypes.Value);
            }
            SortNodes(rootNode);
            return rootNode;
        }

        private static void AddFoldersStructure(AssemblyNode rootNode, string namespaceName, List<AssemblyNode> nodes)
        {
            string[] namespaceParts = namespaceName.Split('.');
            AssemblyNode currNode = CreateOrGetNamespaceNode(rootNode, namespaceParts[0]);
            for (int i = 1; i < namespaceParts.Length; i++)
            {
                currNode = CreateOrGetFolderNode(currNode, namespaceParts[i]);
            }
            currNode.AddAll(nodes);
        }

        private static AssemblyNode CreateOrGetNamespaceNode(AssemblyNode rootNode, string rootNamespaceName)
        {
            return CreateOrGetNode(rootNode, NodeType.Namespace, rootNamespaceName);
        }

        private static AssemblyNode CreateOrGetFolderNode(AssemblyNode rootNode, string rootNamespaceName)
        {
            return CreateOrGetNode(rootNode, NodeType.Folder, rootNamespaceName);
        }

        private static AssemblyNode CreateOrGetNode(AssemblyNode rootNode, NodeType nodeType, string textRepresentation)
        {
            if (rootNode.GetNodes() == null)
            {
                return rootNode.AddNode(CreateEmptyNode(nodeType, textRepresentation));
            }

            AssemblyNode foundNode = rootNode.GetNodes().Find(node => node.NodeType == nodeType && node.TextRepresentation.Equals(textRepresentation));
            if (foundNode == null)
                foundNode = rootNode.AddNode(CreateEmptyNode(nodeType, textRepresentation));
            return foundNode;
        }

        private static AssemblyNode CreateEmptyNode(NodeType nodeType, string textRepresentation)
        {
            AssemblyNode node = new AssemblyNode(nodeType);
            node.TextRepresentation = textRepresentation;
            return node;
        }

        private static void SortNodes(AssemblyNode currNode)
        {
            if (currNode.GetNodes() == null || !IsStorageNode(currNode))
                return;
            currNode.GetNodes().Sort(nodeComparer);
            foreach (AssemblyNode node in currNode.GetNodes())
                SortNodes(node);
        }

        internal static bool IsStorageNode(AssemblyNode node)
        {
            return node.NodeType == NodeType.Namespace || node.NodeType == NodeType.Folder;
        }
    }

    class NodeComparer : IComparer<AssemblyNode>
    {
        public int Compare(AssemblyNode x, AssemblyNode y)
        {
            if (TreeConstructionUtils.IsStorageNode(x) && !TreeConstructionUtils.IsStorageNode(y))
                return -1;
            if (!TreeConstructionUtils.IsStorageNode(x) && TreeConstructionUtils.IsStorageNode(y))
                return 1;
            return x.TextRepresentation.CompareTo(y.TextRepresentation);
        }

    }
}
