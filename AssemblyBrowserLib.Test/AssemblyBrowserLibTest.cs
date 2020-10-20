using AssemblyBrowserLib.AssemblyTree;
using AssemblyBrowserLib.exceptions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AssemblyBrowserLib.Test
{
    public class AssemblyBrowserLibTest
    {
        private const string TEST_ASSEMBLY_PATH = "D:/!Университет/5 семестр/СПП/AssemblyBrowser/TestAssembly/bin/Debug/netstandard2.0/TestAssembly.dll";

        [Fact]
        public void ShouldReturnCorrectTreeStructure()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(TEST_ASSEMBLY_PATH);

            AssemblyNode rootNode = assemblyBrowser.GetAssemblyTree();

            //Namespaces test
            Assert.NotNull(rootNode.GetNodes());
            Assert.Equal(2, rootNode.GetNodes().Count);
            IEnumerable<string> namespaces = rootNode.GetNodes().Select(node => node.TextRepresentation);
            Assert.Contains("AssemblyBrowserDemo", namespaces);
            Assert.Contains("TestAssembly", namespaces);

            AssemblyNode testAssemblyNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("TestAssembly"));

            //TestAssembly namspace types test
            IEnumerable<NodeType> namespaceTypes = testAssemblyNode.GetNodes().Select(node => node.NodeType);

            Assert.Contains(NodeType.Enum, namespaceTypes);
            Assert.Contains(NodeType.Interface, namespaceTypes);
            Assert.Contains(NodeType.Class, namespaceTypes);

            AssemblyNode testClassNode = testAssemblyNode.GetNodes().Find(node => node.TextRepresentation.Equals("TestClass"));

            //TestClass types test
            IEnumerable<NodeType> testClassTypes = testClassNode.GetNodes().Select(node => node.NodeType);
            Assert.Contains(NodeType.Delegate, testClassTypes);
            Assert.Contains(NodeType.Event, testClassTypes);
            Assert.Contains(NodeType.Field, testClassTypes);
            Assert.Contains(NodeType.Class, testClassTypes);
            Assert.Contains(NodeType.Property, testClassTypes);
            Assert.Contains(NodeType.Method, testClassTypes);

            //Inner types test
            IEnumerable<NodeType> innerClassTypes = testClassNode.GetNodes()
                .Find(node => node.NodeType == NodeType.Class).GetNodes().Select(node => node.NodeType);
            Assert.Contains(NodeType.Field, innerClassTypes);
            Assert.Contains(NodeType.Struct, innerClassTypes);
        }

        [Fact]
        public void ShouldFindExtensionMethods()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();
            assemblyBrowser.LoadAssemblyFromFile(TEST_ASSEMBLY_PATH);

            AssemblyNode rootNode = assemblyBrowser.GetAssemblyTree();

            AssemblyNode extTestClassNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("AssemblyBrowserDemo")).GetNodes()[0];
            IEnumerable<NodeType> extTestExtMethods = extTestClassNode.GetNodes().Select(node => node.NodeType)
                .Where(nodeType => nodeType == NodeType.ExtensionMethod);
            Assert.Equal(1, extTestExtMethods.Count());

            AssemblyNode testClassNode = rootNode.GetNodes().Find(node => node.TextRepresentation.Equals("AssemblyBrowserDemo")).GetNodes()
                .Find(node => node.NodeType == NodeType.Class);
            IEnumerable<NodeType> testClassExtMethods = extTestClassNode.GetNodes().Select(node => node.NodeType)
                .Where(nodeType => nodeType == NodeType.ExtensionMethod);
            Assert.Equal(1, testClassExtMethods.Count());
        }

        [Fact]
        public void ShouldThrowExceptionWhenAssemblyNotLoaded()
        {
            AssemblyBrowser assemblyBrowser = new AssemblyBrowser();

            Assert.Throws<AssemblyNotLoadedException>(() => assemblyBrowser.GetAssemblyTree());
        }
    }
}
