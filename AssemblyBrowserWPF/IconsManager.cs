using AssemblyBrowserLib.AssemblyTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserWPF
{
    class IconsManager
    {
        private static readonly List<string> baseNodeNames = new List<string> { "Namespace", "Folder", "ExtensionMethod", "Class", "Interface", "Structure", "Enum", "Delegate", "Event", "Field", "Method", "Property" };
        private static readonly List<string> accessModifireNames = new List<string> { "", "Protect", "Friend", "Private" };
        private static readonly string FILE_NAME_FORMAT = "{0}{1}_16x.png";

        private readonly List<List<string>> nodeToIconPath;

        public IconsManager(string iconsFolderName)
        {
            nodeToIconPath = new List<List<string>>();
            nodeToIconPath.Add(new List<string> { Path.Combine(iconsFolderName, string.Format(FILE_NAME_FORMAT, baseNodeNames[(int)NodeType.Namespace], "")) });
            nodeToIconPath.Add(new List<string> { Path.Combine(iconsFolderName, string.Format(FILE_NAME_FORMAT, baseNodeNames[(int)NodeType.Folder], "Closed")) });
            nodeToIconPath.Add(new List<string> { Path.Combine(iconsFolderName, string.Format(FILE_NAME_FORMAT, baseNodeNames[(int)NodeType.ExtensionMethod], "")) });
            for (int i = 3; i < baseNodeNames.Count; i++)
            {
                List<string> iconsPath = new List<string>();
                for (int j = 0; j < accessModifireNames.Count; j++)
                    iconsPath.Add(Path.Combine(iconsFolderName, string.Format(FILE_NAME_FORMAT, baseNodeNames[i], accessModifireNames[j])));
                nodeToIconPath.Add(iconsPath);
            }
        }

        public string GetIconPathForNode(NodeType nodeType, AccessModifire accessModifire)
        {
            switch (nodeType)
            {
                case NodeType.Namespace:
                    return nodeToIconPath[(int)NodeType.Namespace][0];
                case NodeType.Folder:
                    return nodeToIconPath[(int)NodeType.Folder][0];
                case NodeType.ExtensionMethod:
                    return nodeToIconPath[(int)NodeType.ExtensionMethod][0];
                default:
                    return nodeToIconPath[(int)nodeType][(int)accessModifire];
            }
        }

    }
}
