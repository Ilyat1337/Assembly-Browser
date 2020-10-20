using System;

namespace TestAssembly
{
    public enum ABCEnum
    {
        A, B, C
    }

    interface ITestInterface
    {
        void DisplayABCEnum(ABCEnum aBCEnum);
    }

    class TestClass
    {
        public delegate string DemoDelegate(object o);
        internal event DemoDelegate demoDelegate;

        private String str;

        protected internal class InnerClass
        {
            private int foo;

            struct InnerStruct
            {
                int A;
                int B;
            }
        }

        public string Str
        {
            get { return str; }
            private set { }
        }

        internal string AddTwoStrings(string str1, string str2)
        {
            return str1 + str2;
        }
    }
}
