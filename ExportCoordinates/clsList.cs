using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ExportCoordinates
{
    class Details
    {
        public string Name { get; set; }
        public  ObjectId Id { get; set; }
        public double RimElevation { get; set; }
        public double SumpElevation { get; set; }
        public Point2d Position { get; set; }
        

        public static Comparison<Details> SortByName = delegate(Details p1, Details p2)
        {
            return p1.Name.CompareTo(p2.Name);
        };
    }
}
