using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBF_Analyzer_WPF.Services
{
    class Checker
    {
        Cell_Controls control1 = new() { Values = new string[] { "d", "d" }, Extended = "", TargetCell = "MES", TargetCellValues = new string[] { "a", "b", "c" } };
        Cell_Controls control2 = new() { Values = new string[] { "d", "d" }, Extended = "", TargetCell = "MES", TargetCellValues = new string[] { "a", "b", "c" } };

        List<Cell_Controls> controls = new();        
        public void func()
        {
            controls.Add(control1);
            controls.Add(control2);
        }
        
        //Cell cell = new { CellName = "UKL", Values = new { 1, 2 }, Extended = "", TargetCell = "MES", TargetCellValues = { "a", "b", "c" }};
        //Cell cell = new() { CellName = "MES", List < T > controls };
        //cell.

    }
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
