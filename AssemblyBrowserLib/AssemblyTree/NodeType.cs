namespace AssemblyBrowserLib.AssemblyTree
{
    public enum NodeType
    {
        Namespace = 0,
        Folder,

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
