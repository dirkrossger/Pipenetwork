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


namespace ExportCoordinates
{
    class Layer 
    {
        public static List<string> ReadLayers()
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acadDb = acadDoc.Database;
            Editor ed = acadDoc.Editor;


            List<string> lstlay = new List<string>();
            string layName = "";

            LayerTableRecord layer;
            using (Transaction tr = acadDb.TransactionManager.StartOpenCloseTransaction())
            {
                LayerTable lt = tr.GetObject(acadDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId layerId in lt)
                {
                    layer = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                    layName = layer.Name;
                    if(!layName.Contains("|")) //ignore xref-layer
                    {
                        lstlay.Add(layer.Name);
                    }
                    if(lstlay.Contains("0"))
                    {
                        lstlay.RemoveAt(0);
                    }
                }
            }
            lstlay.Sort();
            return lstlay;
        }

        public bool RemoveLayer(string Layername)
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acadDb = acadDoc.Database;
            Editor ed = acadDoc.Editor;

            bool m_Fine = false;

            LayerTableRecord Laytblrec;
            using (Transaction tr = acadDb.TransactionManager.StartOpenCloseTransaction())
            {
                LayerTable lt = tr.GetObject(acadDb.LayerTableId, OpenMode.ForRead) as LayerTable;
                foreach (ObjectId layerId in lt)
                {
                    Laytblrec = tr.GetObject(layerId, OpenMode.ForWrite) as LayerTableRecord;
                    if(Laytblrec.Name == Layername)
                    {
                        try
                        {
                            //RemoveAllEntity(Layername);
                            Laytblrec.Erase();
                            m_Fine = true;
                        }
                        catch (System.Exception ex)
                        {
                            ed.WriteMessage("\nCan´t remove current Layer! " + Layername);
                            m_Fine = false;
                        }
                    }
                }
                tr.Commit();
                return m_Fine;
            }
        }

        public void RemoveAllEntity(string Layername)
        {
            Document acadDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acadDb = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            SelectionSet m_SSet = null;
            int m_Count = 0;

             TypedValue[] values = new TypedValue[] 
            {
              new TypedValue(8, Layername) 
            };
             
            PromptSelectionResult psr = acadDoc.Editor.SelectAll(new SelectionFilter(values));


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
                             acadEnt m_Ent = tr.GetObject(m_SSet[i].ObjectId, OpenMode.ForWrite) as acadEnt;
                             if (m_Ent != null)
                             {
                                 m_Ent.Erase();
                                 m_Count++;
                             }
                         }
                         catch(System.Exception ex)
                         {
                             ed.WriteMessage("\nCouldn´t remove Objects on Layer: " + Layername);
                         }
                         
                     }

                 }
                 tr.Commit();
                 ed.WriteMessage("\n " + m_Count + " Objects on Layer " + Layername + " removed.");
             }
        }
   }
}
