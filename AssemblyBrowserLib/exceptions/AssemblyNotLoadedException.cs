using System;

namespace AssemblyBrowserLib.exceptions
{
    public class AssemblyNotLoadedException : Exception
    {
        private static readonly string EXCEPTION_MESSAGE = "Call to GetAssemblyTree() without loading assembly.";

        public AssemblyNotLoadedException() : base(EXCEPTION_MESSAGE) { }
    }
}
