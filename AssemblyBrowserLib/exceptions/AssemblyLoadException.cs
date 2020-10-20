using System;

namespace AssemblyBrowserLib.exceptions
{
    public class AssemblyLoadException : Exception
    {
        private static readonly string EXCEPTION_MESSAGE = "Error loading assembly from file.";

        public AssemblyLoadException() : base(EXCEPTION_MESSAGE) { }
    }
}
