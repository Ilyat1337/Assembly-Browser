using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyBrowserWPF
{
    class AssemblyNodeView
    {
        public string TextRepresentation
        { get; set; }

        public string ImageSource
        { get; set; }

        public List<AssemblyNodeView> Nodes
        { get; set; }
    }
}
