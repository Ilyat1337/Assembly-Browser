using AssemblyBrowserLib;
using AssemblyBrowserLib.AssemblyTree;
using AssemblyBrowserLib.exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace AssemblyBrowserWPF
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private const string ASSEMBLY_LOAD_ERROR_CAPTION = "Assembly load error";
        private const string ASSEMBLY_NOT_LOADED_ERROR_CAPTION = "Assembly not loaded error";
        private const string OPEN_FILE_FILTER = "Dll library|*.dll";

        private const string ICONST_FOLDER_NAME = "icons";
        private readonly AssemblyBrowser assemblyBrowser;
        private readonly IconsManager iconsManager;
        private readonly FileDialogService fileDialogService;

        public ApplicationViewModel()
        {
            iconsManager = new IconsManager(ICONST_FOLDER_NAME);
            fileDialogService = new FileDialogService(OPEN_FILE_FILTER);
            assemblyBrowser = new AssemblyBrowser();

            //assemblyBrowser.LoadAssemblyFromFile("D:/!Университет/5 семестр/СПП/AssemblyBrowser/TestAssembly/bin/Debug/netstandard2.0/TestAssembly.dll");
           
            //AssemblyNode rootNode = assemblyBrowser.GetAssemblyTree();
            //Nodes = RecursiveNodeToViewNode(rootNode).Nodes;
        }

        private List<AssemblyNodeView> nodes;
        public List<AssemblyNodeView> Nodes
        {
            get => nodes;
            set
            {
                nodes = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private ControlCommand openCommand;
        public ControlCommand OpenCommand
        {
            get
            {
                return openCommand ??
                (openCommand = new ControlCommand(obj =>
                {
                    if (fileDialogService.OpenFileDialog())
                    {
                        try
                        {
                            assemblyBrowser.LoadAssemblyFromFile(fileDialogService.FilePath);
                            AssemblyNode rootNode = assemblyBrowser.GetAssemblyTree();
                            Nodes = RecursiveNodeToViewNode(rootNode).Nodes;
                        }
                        catch (AssemblyLoadException e)
                        {
                            fileDialogService.ShowErrorMessage(e.Message, ASSEMBLY_LOAD_ERROR_CAPTION);
                        }
                        catch (AssemblyNotLoadedException e)
                        {
                            fileDialogService.ShowErrorMessage(e.Message, ASSEMBLY_NOT_LOADED_ERROR_CAPTION);
                        }
                    }
                }
                ));
            }
        }

        private ControlCommand closeWindowCommand;
        public ControlCommand CloseWindowCommand
        {
            get
            {
                return closeWindowCommand ??
                (openCommand = new ControlCommand(obj =>
                {
                    (obj as Window).Close();
                }
                ));
            }
        }

        private AssemblyNodeView RecursiveNodeToViewNode(AssemblyNode node)
        {
            AssemblyNodeView assemblyNodeView = new AssemblyNodeView();
            assemblyNodeView.TextRepresentation = node.TextRepresentation;
            assemblyNodeView.ImageSource = iconsManager.GetIconPathForNode(node.NodeType, node.AccessModifire);
            if (node.GetNodes() != null)
            {
                assemblyNodeView.Nodes = new List<AssemblyNodeView>();
                foreach (AssemblyNode nestedNode in node.GetNodes())
                    assemblyNodeView.Nodes.Add(RecursiveNodeToViewNode(nestedNode));
            }
            return assemblyNodeView;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

