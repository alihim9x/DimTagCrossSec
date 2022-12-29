using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;
using Autodesk.Revit.DB;

namespace Utility
{
    public static class ElementUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }

        public static Autodesk.Revit.DB.Element GetRevitElement(this Autodesk.Revit.DB.ElementId elemId)
        {
            return RevitData.Instance.Document.GetElement(elemId);
        }
        public static Autodesk.Revit.DB.Element GetRevitElement(this Autodesk.Revit.DB.Reference reference)
        {
            return RevitData.Instance.Document.GetElement(reference);
        }
        public static Model.Entity.Element GetEntityElement(this Autodesk.Revit.DB.Element elem)
        {
            var ettElem = ModelData.Instance.EttElements.SingleOrDefault(x => x.RevitElement.UniqueId == elem.UniqueId);
            if (ettElem == null)
            {
                ettElem = new Model.Entity.Element()
                {
                    RevitElement = elem
                };
                ModelData.Instance.EttElements.Add(ettElem);
            }
            return ettElem;
        }
        //public static void CreateRebars(this Model.Entity.Element element)
        //{
        //    foreach (var rebar in element.Rebars)
        //    {
        //        rebar.CreateRebar();
        //    }
        //    foreach (var stirrup in element.Stirrups)
        //    {
        //        stirrup.CreateStirrup();
        //    }
        //}
        public static Model.Entity.ElementType GetElementType(this Model.Entity.Element ettElem)
        {
            var revitElem = ettElem.RevitElement;
            var cate = revitElem.Category;
            if (revitElem is Autodesk.Revit.DB.Floor)
                return Model.Entity.ElementType.Floor;
            if (revitElem is Autodesk.Revit.DB.Wall)
                return Model.Entity.ElementType.Wall;
            if (cate.Id.IntegerValue == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming)
                return Model.Entity.ElementType.Framing;
            if (cate.Id.IntegerValue == (int)Autodesk.Revit.DB.BuiltInCategory.OST_StructuralColumns)
                return Model.Entity.ElementType.Column;
            return Model.Entity.ElementType.Undefined;
        }
        public static Model.Entity.Face GetFaces ( this Model.Entity.Element ettElem)
        {
            var typeElem = ettElem.ElementType;
            var revitSolid = ettElem.RevitElement.GetSingleSolidValidRef();
            var faceElem = new Model.Entity.Face();
            
            var activeView = revitData.ActiveView;

            for (int i = 0; i < revitSolid.Faces.Size; i++)
            {

                if (revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsSameDirection(activeView.RightDirection))
                {
                    faceElem.RightFace = revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace;
                }
                else if (revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsOppositeDirection(activeView.RightDirection))
                {
                    faceElem.LeftFace = revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace;
                }
                else if (revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsSameDirection(activeView.UpDirection))

                {
                    faceElem.TopFace = revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace;

                   

                }
                else if (revitSolid.Faces.get_Item(i).ComputeNormal(Autodesk.Revit.DB.UV.Zero).IsOppositeDirection(activeView.UpDirection))
                {
                    faceElem.BotFace = revitSolid.Faces.get_Item(i) as Autodesk.Revit.DB.PlanarFace;
                }

            }
            
            
            return faceElem;
        }
        public static Autodesk.Revit.DB.XYZ MaxRepeatedItem(this List<Autodesk.Revit.DB.XYZ> listXYZ)
        {
            var maxRepeatedItems = listXYZ.GroupBy(x => x.Z).OrderByDescending(x => x.Count()).First().Select(x => x).First();
            return maxRepeatedItems;

        }
        public static List<Model.Entity.Rebar> GetModelEntityRebarInHost(this Autodesk.Revit.DB.Element element)
        {
            if ((element as Autodesk.Revit.DB.FamilyInstance) != null)
            {
                var rebarInHost = Autodesk.Revit.DB.Structure.RebarHostData.GetRebarHostData(element).GetRebarsInHost();
                List<Model.Entity.Rebar> rebarList = new List<Model.Entity.Rebar>();
                rebarInHost.ToList().ForEach(x => rebarList.Add(new Model.Entity.Rebar(x)));
                return rebarList;
            }
            //else if ((element as Autodesk.Revit.DB.Floor) != null)
            //{
            //    var elemId = element.Id;
            //    var floorRebarInstances = revitData.FloorRebars.Where(x => x.GetHostId() == elemId).ToList();
            //    return floorRebarInstances;
            //}
            throw new Model.Exception.CaseNotCheckException();

        }
        public static bool IsIntersectWithActiveView(this Autodesk.Revit.DB.Element elem)
        {
            
            var insElem = revitData.InstanceElements.Where(x =>
            {
                List<BuiltInCategory> bic = new List<BuiltInCategory>
                {
                    (BuiltInCategory)elem.Category.Id.IntegerValue
                };

                var cate = x.Category;
                if (cate == null) return false;
                return bic.Contains((Autodesk.Revit.DB.BuiltInCategory)cate.Id.IntegerValue);
            }).ToList();
            var yes = false;
            var activeView = revitData.ActiveView;
            var bbActiveView = activeView.get_BoundingBox(null);
            var solidOfView = bbActiveView.CreateSolidFromBBox().ScaleSolid(1.001);
            var outline = new Autodesk.Revit.DB.Outline(bbActiveView.Min, bbActiveView.Max);
            var bbIntersecFil = new Autodesk.Revit.DB.BoundingBoxIntersectsFilter(outline);
            /*var solid = elem.GetOriginalSolid().ScaleSolid(1.001);*/ // Để va chạm với các đối tượng chạm vô đối tượng đang xét chứ ko join vô
            var solidIntersecFil = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(solidOfView);
            //return new Autodesk.Revit.DB.FilteredElementCollector(revitData.Document
            //    , insElem.Select(x => x.Id).ToList()).WherePasses(bbIntersecFil).WherePasses(solidIntersecFil);
            IEnumerable<Autodesk.Revit.DB.Element> intersectElems = new Autodesk.Revit.DB.FilteredElementCollector(revitData.Document
                ,insElem.Select(x=>x.Id).ToList())/*.WherePasses(bbIntersecFil)*/.WherePasses(solidIntersecFil).ToList();
            foreach (var item in intersectElems)
            {
                if(elem.Id == item.Id)
                {
                    yes = true;
                    break;
                    
                }
                else
                {
                    yes = false;
                }
            }
            return yes;

        }
        public static Solid CreateSolidFromBBox (this BoundingBoxXYZ inputBb)
        {
            // corners in BBox coords
            XYZ pt0 = new XYZ(inputBb.Min.X, inputBb.Min.Y, inputBb.Min.Z);
            XYZ pt1 = new XYZ(inputBb.Max.X, inputBb.Min.Y, inputBb.Min.Z);
            XYZ pt2 = new XYZ(inputBb.Max.X, inputBb.Max.Y, inputBb.Min.Z);
            XYZ pt3 = new XYZ(inputBb.Min.X, inputBb.Max.Y, inputBb.Min.Z);
            //edges in BBox coords
            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);
            //create loop, still in BBox coords
            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            Double height = inputBb.Max.Z - inputBb.Min.Z;
            CurveLoop baseLoop = CurveLoop.Create(edges);
            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);
            Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);

            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox, inputBb.Transform);

            return transformBox;
        }

    }
}
