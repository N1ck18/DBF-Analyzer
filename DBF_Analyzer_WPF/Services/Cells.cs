using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBF_Analyzer_WPF.Services
{   
    internal class Cell
    {
        public string CellName { get; set; }
        public List<Cell_Controls> Cell_Controls { get; set; }
    }
    internal class Cell_Controls
    {
        public string[] Values { get; set; }
        public string TargetCell { get; set; }
        public string[] TargetCellValues { get; set; }
        public string Extended { get; set; }
    }

}
