using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
//using System.Windows.Forms;

#region Autodesk
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using AcadAppl = Autodesk.AutoCAD.ApplicationServices.Application;
using acadEnt = Autodesk.AutoCAD.DatabaseServices.Entity;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices.Styles;
using C3D = Autodesk.Civil.DatabaseServices;
#endregion
//[assembly: CommandClass(typeof(PipeNetwork.Commands))]


namespace PipeNetwork
{
    public class Commands
    {
        [CommandMethod("xPipeDataField")]
        public static void CmdSelectPipe()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect network part: ");
            peo.SetRejectMessage("\nOnly network parts");
            peo.AddAllowedClass(typeof(Part), false);
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK) return;
            ObjectId partId = per.ObjectId;

            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                Part p = trans.GetObject(partId, OpenMode.ForRead) as Part;
                ed.WriteMessage("\nDXF.Name= " + partId.ObjectClass.DxfName);
                ed.WriteMessage("\nNetwork.Name= " + p.NetworkName);

                //PartDataField[] fields = p.PartData.GetAllDataFields();
                //foreach (PartDataField field in fields)
                //{
                //    ed.WriteMessage("\n{0}: {1}", field.Description, field.Value);
                //}
            }
        }

        // Save Pipes from Network to a csv.file
        //[CommandMethod("xPipedatas")]
        public void CmdGetAllPipeEntities()
        {

            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            Database acadDb = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;

            List<string> m_List = new List<string>();
            SelectionSet m_SSet = null;

            PromptSelectionResult psr = acadDoc.Editor.SelectAll();


            if (psr.Status == PromptStatus.OK)
            {
                m_SSet = psr.Value;
            }

            using (Transaction tr = acadDb.TransactionManager.StartTransaction())
            {
                if (m_SSet != null)
                {
                    for (int i = 0; i < m_SSet.Count; i++)
                    {
                        try
                        {
                            acadEnt m_Ent = tr.GetObject(m_SSet[i].ObjectId, OpenMode.ForRead) as acadEnt;
                            #region Pipe elements
                            if (m_Ent.GetType().FullName == "Autodesk.Civil.DatabaseServices.Pipe")
                            {
                                Part p = tr.GetObject(m_Ent.ObjectId, OpenMode.ForRead) as Part;
                                //ed.WriteMessage("\nDXF.Name= " + m_Ent.ObjectId.ObjectClass.DxfName);
                                //ed.WriteMessage("\nNetwork.Name= " + p.NetworkName);


                                Pipe pipe = p.ObjectId.GetObject(OpenMode.ForRead) as Pipe;
                                //PartsList parts = (PartsList)civdoc.Styles.PartsListSet[0].GetObject(OpenMode.ForRead);
                                //PartFamily fam = (PartFamily)parts[1].GetObject(OpenMode.ForRead);
                                //ObjectId structId = fam[0];
                                ObjectId newStructId = ObjectId.Null;
                                Network ntwrk = (Network)pipe.NetworkId.GetObject(OpenMode.ForRead);
                                string ntwrkName = ntwrk.PartsListName;
                                //ntwrk.AddStructure(fam.ObjectId, structId, pipe.StartPoint, 0, ref newStructId, false);
                                //pipe.UpgradeOpen();
                                //pipe.ConnectToStructure(ConnectorPositionType.Start, newStructId, true);
                                //pipe.DowngradeOpen();
                                //tr.Commit();
                                Point3d startPt = pipe.StartPoint;
                                Point3d endPt = pipe.EndPoint;
                                double length = pipe.Length2D;
                                double slope = pipe.Slope;
                                string dimension = pipe.Description;
                                m_List.Add(string.Format(p.NetworkName +
                                    ", Length, " + Convert.ToString(Math.Round(length, 1)) +
                                    ", Dimension, " + dimension +
                                    ", Slope, " + Convert.ToString(Math.Round(slope, 3)) +
                                    ", StartH, " + Math.Round(startPt.Z, 3) +
                                    ", EndH, " + Math.Round(endPt.Z, 3)));
                            }
                            #endregion

                            #region Structure elements
                            if (m_Ent.GetType().FullName == "Autodesk.Civil.DatabaseServices.Structure")
                            {
                                Part p = tr.GetObject(m_Ent.ObjectId, OpenMode.ForRead) as Part;

                                //ed.WriteMessage("\n " + i + " " + m_Ent.GetType().FullName);
                                Structure structure = p.ObjectId.GetObject(OpenMode.ForRead) as Structure;

                                m_List.Add(string.Format(p.NetworkName +
                                    ", Nr," + structure.Name +
                                    ", Dimension, " + structure.Description +
                                    ", Lock," + Math.Round(structure.RimElevation, 3) +
                                    ", VG," + Math.Round(structure.SumpElevation, 3)
                                    ));
                            }
                            #endregion
                        }

                        catch (System.Exception ex)
                        {
                            ed.WriteMessage(ex.ToString());
                        }
                    }
                }
                m_List.Sort();
                
                tr.Commit();
            }
            try
            {
                string path = @"c:\temp\";
                System.IO.Directory.CreateDirectory(path);
                //string fileName = System.IO.Path.GetRandomFileName();
                path = path + "1.csv";
                File.WriteAllLines(path, m_List);
                var ExcelApp = new Excel.Application();
                var openCSVFile = System.Windows.Forms.MessageBox.Show
                    ("Export Complete. CSV file saved as: " + path + ". \n\n Open File Now?", "CSV Exported", 
                    System.Windows.Forms.MessageBoxButtons.YesNo, 
                    System.Windows.Forms.MessageBoxIcon.Information);
                if (openCSVFile == System.Windows.Forms.DialogResult.Yes)
                {
                    // NEED TO OPEN THE CSV FILE IN EXCEL....?
                    ExcelApp.Workbooks.OpenText(path, Comma: true);
                    ExcelApp.Visible = true;
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage("\nCan´t create file:");
            }
          
        }

        [CommandMethod("xCreatePipe")]
        public static void CmdCreatePipe()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityOptions peo = new PromptEntityOptions("\nSelect network part: ");
            peo.SetRejectMessage("\nOnly network parts");
            peo.AddAllowedClass(typeof(Part), false);
            PromptEntityResult per = ed.GetEntity(peo);
            if (per.Status != PromptStatus.OK) return;
            ObjectId partId = per.ObjectId;

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            CivilDocument civDoc = CivilApplication.ActiveDocument;
            Database aDb = acDoc.Database;
            
            PromptPointResult pPtRes;
            PromptPointOptions pPtOpts = new PromptPointOptions("");

            // Prompt for the start point
            pPtOpts.Message = "\nEnter the start point of the line: ";
            pPtRes = acDoc.Editor.GetPoint(pPtOpts);
            Point3d ptStart = pPtRes.Value;

            // Exit if the user presses ESC or cancels the command
            if (pPtRes.Status == PromptStatus.Cancel) return;

            // Prompt for the end point
            pPtOpts.Message = "\nEnter the end point of the line: ";
            pPtOpts.UseBasePoint = true;
            pPtOpts.BasePoint = ptStart;
            pPtRes = acDoc.Editor.GetPoint(pPtOpts);
            Point3d ptEnd = pPtRes.Value;

            if (pPtRes.Status == PromptStatus.Cancel) return;


            using (Transaction ts = aDb.TransactionManager.StartTransaction())
            {
                //Part p = trans.GetObject(partId, OpenMode.ForRead) as Part;
                ObjectIdCollection oIdCollection = civDoc.GetPipeNetworkIds();

                ObjectId objId = oIdCollection[0];
                Network oNetwork = ts.GetObject(objId, OpenMode.ForWrite) as Network;
                ed.WriteMessage("Pipe Network: {0}\n", oNetwork.Name);
                // Go through the list of part types and select the first pipe found
                ObjectId pid = oNetwork.PartsListId;
                PartsList pl = ts.GetObject(pid, OpenMode.ForWrite) as PartsList;
                ObjectId oid = pl[0];
                PartFamily pfa = ts.GetObject(oid, OpenMode.ForWrite) as PartFamily;
                ObjectId psize = pfa[0];
                LineSegment3d line = new LineSegment3d(ptStart, ptEnd);
                ObjectIdCollection col = oNetwork.GetPipeIds();
                ObjectId oidNewPipe = ObjectId.Null;

                try
                {
                    oNetwork.AddLinePipe(oid, psize, line, ref oidNewPipe, false);
                }
                catch(System.Exception ex)
                {

                }
                
                
                Pipe oNewPipe = ts.GetObject(oidNewPipe, OpenMode.ForRead) as Pipe;
                ed.WriteMessage("Pipe created: {0}\n", oNewPipe.DisplayName);
                ts.Commit();

                //PartDataField[] fields = p.PartData.GetAllDataFields();
                //foreach (PartDataField field in fields)
                //{
                //    ed.WriteMessage("\n{0}: {1}", field.Description, field.Value);
                //}
            }
        }


    }
}
