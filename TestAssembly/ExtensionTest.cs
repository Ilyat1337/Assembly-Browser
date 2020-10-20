
using TestAssembly;
using System;

namespace AssemblyBrowserDemo
{
    static class ExtensionTest
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static TestClass GetObject(this TestClass testClass)
        {
            return testClass;
        }

        public static void SayHello()
        {

        }
    }
}
