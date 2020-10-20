namespace AssemblyBrowserLib.AssemblyTree
{
    public enum NodeType
    {
        Namespace = 0,
        Folder,
        ExtensionMethod,

        Class,
        Interface,
        Struct,
        Enum,
        Delegate,

        Event,
        Field,
        Method,
        Property
    }
}
