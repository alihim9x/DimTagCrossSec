using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entity
{
    public class Face
    {
        public Autodesk.Revit.DB.PlanarFace LeftFace { get; set; }
        public Autodesk.Revit.DB.PlanarFace RightFace { get; set; }
        public Autodesk.Revit.DB.PlanarFace TopFace { get; set; }
        public Autodesk.Revit.DB.PlanarFace BotFace { get; set; }
    }
}
