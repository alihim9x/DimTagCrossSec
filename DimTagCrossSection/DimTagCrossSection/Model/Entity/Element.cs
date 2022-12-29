using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using Autodesk.Revit.DB.Structure;

namespace Model.Entity
{
    public class Element : NotifyClass
    {
        public List<Model.Entity.Rebar> EttRebars { get; set; } = new List<Model.Entity.Rebar>();
        public Autodesk.Revit.DB.Element RevitElement { get; set; }
        private Model.Entity.Face face;
        public Model.Entity.Face Faces
        {
            get
            {
                if (face == null)
                {
                    face = this.GetFaces();
                }
                return face;
            }
        }
        
        private ElementType? elementType;
        public ElementType? ElementType
        {
            get
            {
                if (elementType == null)
                {
                    elementType = this.GetElementType();
                }
                return elementType;
            }
        }
        private Geometry geometry;
        public Geometry Geometry
        {
            get
            {
                if(geometry == null)
                {
                    geometry = this.GetGeometry();
                }
                return geometry;
            }
        }
    }

       
}
