using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using acadEnt = Autodesk.AutoCAD.DatabaseServices.Entity;

[assembly: CommandClass(typeof(ExportCoordinates.Commands))]
namespace ExportCoordinates
{
    public class Commands
    {
        private Form1 m_Form1;

        [CommandMethod("xExportCoord")]
        public void ExportCoordinates()
        {
            m_Form1 = new Form1();
            m_Form1.Filldata();
            System.Windows.Forms.DialogResult m_result = m_Form1.ShowDialog();
            m_Form1.Close();
        }
    }
}
