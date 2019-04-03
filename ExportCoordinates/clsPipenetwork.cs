using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace ExportCoordinates
{
    class clsPipenetwork
    {
        private List<Details> _Network;
        private ObjectIdCollection _NetworkIds;

        public ObjectIdCollection NetworkIds
        {
            get { return _NetworkIds; }
            set { _NetworkIds = value; }
        }

        internal List<Details> Network
        {
            get { return _Network; }
            set { _Network = value; }
        }

        public void GetPipenetwork()
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = acadDoc.Editor;
            CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;

            if (CivilDoc.GetPipeNetworkIds() == null)
            {
                ed.WriteMessage("There are no pipe networks to export.  Open a document that contains at least one pipe network");
                //return null;
            }

            using (Transaction ts = acadDoc.Database.TransactionManager.StartTransaction())
            {
                int i;
                List<Details> _StructureDict = new List<Details>();
                _Network = new List<Details>();

                try
                {
                    i = CivilDoc.GetPipeNetworkIds().Count;
                    for (int x = 0; x <= i; x++)
                    {

                        ObjectId oNetworkId = CivilDoc.GetPipeNetworkIds()[x];
                        Network oNetwork = ts.GetObject(oNetworkId, OpenMode.ForRead) as Network;
                        _Network.Add(new Details
                            {
                                Name = oNetwork.Name,
                                Id = oNetworkId
                            });

                        // Get pipes:
                        ObjectIdCollection oPipeIds = oNetwork.GetPipeIds();
                        int pipeCount = oPipeIds.Count;

                        // Get structures:
                        ObjectIdCollection oStructureIds = oNetwork.GetStructureIds();
                        foreach (ObjectId oid in oStructureIds)
                        {
                            Structure _Structure = ts.GetObject(oid, OpenMode.ForRead) as Structure;
                            _StructureDict.Add(new Details
                            {
                                Name = _Structure.Name,
                                RimElevation = _Structure.RimElevation,
                                SumpElevation = _Structure.SumpElevation,
                                Position = new Point2d(_Structure.Position.Y, _Structure.Position.X)
                            });
                        }
                    }

                    if (_Network.Count > 0)
                        foreach(Details x in _Network)
                        {
                            _NetworkIds.Add(x.Id);
                        }

                    //return oStructureDict;
                }



                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("PipeSample: " + ex.Message);
                    //return null;
                }

            }

        }

        private static Microsoft.Office.Interop.Excel.Application xlApp = null;
        private static Workbook xlWb = null;
        private static Worksheet xlWsStructures = null;
        private static Worksheet xlWsPipes = null;

        public static void ExportToExcel(ObjectIdCollection oNetworkIds)
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = acadDoc.Editor;
            CivilDocument CivilDoc = Autodesk.Civil.ApplicationServices.CivilApplication.ActiveDocument;
            // Document AcadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            // Check that there's a pipe network to parse
            //if (CivilDoc.GetPipeNetworkIds() == null)
            //{
            //    ed.WriteMessage("There are no pipe networks to export.  Open a document that contains at least one pipe network");
            //    return;
            //}

            // Interop code is adapted from the MSDN site:
            // http://msdn.microsoft.com/en-us/library/ms173186(VS.80).aspx
            xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                Console.WriteLine(@"EXCEL could not be started. Check that your office installation 
                and project references are correct.");
                return;
            }
            xlApp.Visible = true;
            xlWb = xlApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            xlWsStructures = (Worksheet)xlWb.Worksheets[1];
            xlWsPipes = (Worksheet)xlWb.Worksheets.Add(xlWsStructures, System.Type.Missing, 1, System.Type.Missing);
            //xlWsPipes.Name = "Pipes";
            xlWsStructures.Name = "Structures";

            if (xlWsPipes == null)
            {
                Console.WriteLine(@"Worksheet could not be created. Check that your office installation 
                and project references are correct.");
            }

            // Iterate through each pipe network
            using (Transaction ts = acadDoc.Database.TransactionManager.StartTransaction())
            {
                int i;
                int row = 1;
                char col = 'B';
                //Dictionary<string, char> dictPipe = new Dictionary<string, char>(); // track data parts column
                //// set up header
                Range aRange = xlWsPipes.get_Range("A1", System.Type.Missing);

                try
                {
                    //i = CivilDoc.GetPipeNetworkIds().Count;
                    i = oNetworkIds.Count;
                    for (int x = 0; x <= i; x++)
                    {

                        ObjectId oNetworkId = oNetworkIds[x];
                        Network oNetwork = ts.GetObject(oNetworkId, OpenMode.ForWrite) as Network;

                        // Get pipes:
                        //ObjectIdCollection oPipeIds = oNetwork.GetPipeIds();
                        //int pipeCount = oPipeIds.Count;


                        // Now export the structures
                        col = 'B';
                        Dictionary<string, char> dictStructures = new Dictionary<string, char>(); // track data parts column
                        // Set up header
                        aRange = xlWsStructures.get_Range("A1", System.Type.Missing);
                        aRange.Value2 = "Handle";
                        aRange = xlWsStructures.get_Range("B1", System.Type.Missing);
                        aRange.Value2 = "Structure Name";
                        aRange = xlWsStructures.get_Range("D1", System.Type.Missing);
                        aRange.Value2 = "X";
                        aRange = xlWsStructures.get_Range("C1", System.Type.Missing);
                        aRange.Value2 = "Y";
                        aRange = xlWsStructures.get_Range("E1", System.Type.Missing);
                        aRange.Value2 = "Lock";
                        aRange = xlWsStructures.get_Range("F1", System.Type.Missing);
                        aRange.Value2 = "VG";

                        // Get structures:
                        ObjectIdCollection oStructureIds = oNetwork.GetStructureIds();
                        foreach (ObjectId oid in oStructureIds)
                        {
                            Structure oStructure = ts.GetObject(oid, OpenMode.ForRead) as Structure;
                            col = 'A';
                            
                            //col = 'B';
                            row++;
                            aRange = xlWsStructures.get_Range("" + col + row, System.Type.Missing);
                            aRange.Value2 = oNetwork.Name;
                            aRange = xlWsStructures.get_Range("" + ++col + row, System.Type.Missing);
                            aRange.Value2 = oStructure.Name;
                            aRange = xlWsStructures.get_Range("" + ++col + row, System.Type.Missing);
                            aRange.Value2 = oStructure.Position.Y;
                            aRange = xlWsStructures.get_Range("" + ++col + row, System.Type.Missing);
                            aRange.Value2 = oStructure.Position.X;
                            aRange = xlWsStructures.get_Range("" + ++col + row, System.Type.Missing);
                            aRange.Value2 = oStructure.RimElevation;
                            aRange = xlWsStructures.get_Range("" + ++col + row, System.Type.Missing);
                            aRange.Value2 = oStructure.SumpElevation;

                        }
                    }
                }



                catch (Autodesk.AutoCAD.Runtime.Exception ex)
                {
                    ed.WriteMessage("PipeSample: " + ex.Message);
                    return;
                }

            }
        }
 
        #region IExtensionApplication Members

        public void Initialize() { }

        public void Terminate()
        {
            // Clean up all our Excel COM objects    
            // This will close Excel without saving

            if (xlWb != null)
            {
                try
                {
                    xlWb.Close(false, Type.Missing, Type.Missing);
                    xlApp.Quit();
                    Marshal.FinalReleaseComObject(xlWsStructures);
                    Marshal.FinalReleaseComObject(xlWsPipes);
                    Marshal.FinalReleaseComObject(xlWb);
                    Marshal.FinalReleaseComObject(xlApp);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        #endregion
 
    }
}
