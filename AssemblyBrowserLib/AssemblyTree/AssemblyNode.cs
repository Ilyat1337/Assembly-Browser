using System.Collections.Generic;

namespace AssemblyBrowserLib.AssemblyTree
{
    public class AssemblyNode
    {
        private NodeType nodeType;
        private AccessModifire accessModifire;
        private string textRepresentation;
        private List<AssemblyNode> nodes;

        public AssemblyNode(NodeType nodeType)
        {
            NodeType = nodeType;
        }

        public NodeType NodeType
        {
            get { return nodeType; }
            internal set { nodeType = value; }
        }

        public AccessModifire AccessModifire
        {
            get { return accessModifire; }
            internal set { accessModifire = value; }
        }

        public string TextRepresentation
        {
            get { return textRepresentation; }
            internal set { textRepresentation = value; }
        }

        public List<AssemblyNode> GetNodes()
        {
            return nodes;
        }

        internal AssemblyNode AddNode(AssemblyNode assemblyNode)
        {
            if (nodes == null)
                nodes = new List<AssemblyNode>();
            nodes.Add(assemblyNode);
            return assemblyNode;
        }

        internal void AddAll(List<AssemblyNode> assemblyNodes)
        {
            if (nodes == null)
                nodes = new List<AssemblyNode>();
            nodes.AddRange(assemblyNodes);
        }
    }
}
