using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;


namespace Utility
{
    public static class RebarUtil
    {
        private static RevitData revitData = RevitData.Instance;

        public static IEnumerable<Autodesk.Revit.DB.Curve> GetCenterlineCurves(this Model.Entity.Rebar rebar)
        {
            if(rebar.SingleRebar != null)
            {
                return rebar.SingleRebar.GetCenterlineCurves(false,false,false,Autodesk.Revit.DB.Structure.MultiplanarOption.IncludeOnlyPlanarCurves,0);
            }
            throw new Model.Exception.CaseNotCheckException();
        }
        public static Autodesk.Revit.DB.Line GetDistributionPath(this Model.Entity.Rebar rebar)
        {
            if(rebar.SingleRebar != null)
            {
                var rsda = rebar.SingleRebar.GetShapeDrivenAccessor();
                return rsda.GetDistributionPath();
            }
            throw new Model.Exception.CaseNotCheckException();
        }
        public static IEnumerable<Autodesk.Revit.DB.Curve> GetDrivingCurves (this Model.Entity.Rebar rebar)
        {
            if(rebar.SingleRebar != null)
            {
                var rsda = rebar.SingleRebar.GetShapeDrivenAccessor();
                return rsda.ComputeDrivingCurves();
            }
            throw new Model.Exception.RebarInSysHasNotDrivenCurves();
        }
        public static Autodesk.Revit.DB.XYZ MaxOccurOriginZCenterLineCurve (this Model.Entity.Rebar rebar)
        {
            
                var centerLineCurves = rebar.GetDrivingCurves();
                var originZ = new List<Autodesk.Revit.DB.XYZ>();
                foreach (var item in centerLineCurves)
                {
                    originZ.Add(item.GetEndPoint(0));
                    originZ.Add(item.GetEndPoint(1));
                }
                return originZ.MaxRepeatedItem();
           
        }
        public static void SetValue(this Model.Entity.Rebar rebar, string name, object obj)
        {
            if(rebar.SingleRebar != null)
            {
                
                rebar.SingleRebar.SetValue(name,obj);
               
            }
            //if (rebar.RebarInsystem != null)
            //{
            //    rebar.RebarInsystem.SetValue(name,obj);
            //}
        }
        public static Model.ParameterValue AsValue(this Model.Entity.Rebar rebar, string paramname)
        {
            if(rebar.SingleRebar != null)
            {
               return rebar.SingleRebar.AsValue(paramname);
            }
            throw new Model.Exception.CaseNotCheckException(); 
        }
        public static Autodesk.Revit.DB.ElementId GetHostId(this Model.Entity.Rebar rebar)
        {
            if(rebar.SingleRebar != null)
            {
                return rebar.SingleRebar.GetHostId();
            }
            throw new Model.Exception.CaseNotCheckException(); 
        }
        public static void CreateMultiRebarTag (this List<Autodesk.Revit.DB.ElementId> rebarIds
            , Autodesk.Revit.DB.MultiReferenceAnnotationType multiRebarTagType,double y)
        {

            Autodesk.Revit.DB.MultiReferenceAnnotationOptions multiRebarTagOpts = new Autodesk.Revit.DB.MultiReferenceAnnotationOptions(multiRebarTagType);
            multiRebarTagOpts.TagHeadPosition = new Autodesk.Revit.DB.XYZ(0, 1, 0);
            multiRebarTagOpts.DimensionLineOrigin = revitData.FramingInstancesInView.FirstOrDefault().GetTotalTransform().Origin + y * revitData.ActiveView.UpDirection;
           

            multiRebarTagOpts.DimensionLineDirection = revitData.ActiveView.RightDirection;
            multiRebarTagOpts.DimensionPlaneNormal = revitData.ActiveView.ViewDirection;
            multiRebarTagOpts.SetElementsToDimension(rebarIds);
            var mra = Autodesk.Revit.DB.MultiReferenceAnnotation.Create(revitData.Document, revitData.ActiveView.Id, multiRebarTagOpts);
            var rebarTag = revitData.Document.GetElement(mra.TagId) as Autodesk.Revit.DB.IndependentTag;
            rebarTag.TagHeadPosition = revitData.FramingInstancesInView.FirstOrDefault().GetTotalTransform().Origin + (y-0.00559) * (revitData.ActiveView.UpDirection) + 1 * (revitData.ActiveView.RightDirection);
        }
        public static void SetDirectionXYRebar(this List<Model.Entity.Rebar> rebars )
        {
            var rebarCate = new List<Autodesk.Revit.DB.BuiltInCategory>
            {
                Autodesk.Revit.DB.BuiltInCategory.OST_Rebar
            };
            ParameterUtil.AddParameter("Rebar Direction (XY)", Model.AddParameterType.Instance, Model.DefinitionGroupType.KetCau
                , Autodesk.Revit.DB.ParameterType.Text, Autodesk.Revit.DB.BuiltInParameterGroup.PG_CONSTRUCTION, rebarCate);

            foreach (var item in rebars)
            {
                if(item.AsValue("Host Category").ValueNumber == (double)Autodesk.Revit.DB.Structure.RebarHostCategory.StructuralFraming)
                {
                    var hostIdElem = item?.GetHostId().GetRevitElement();
                    var curveframing = (hostIdElem.Location as Autodesk.Revit.DB.LocationCurve).Curve as Autodesk.Revit.DB.Line;
                    var directionframing = curveframing.Direction;
                    if (directionframing.IsXOrY())

                    {
                        item.SetValue("Rebar Direction (XY)", "X");
                    }
                    else
                    {
                        item.SetValue("Rebar Direction (XY)", "Y");
                    }
                }
                else if(item.AsValue("Host Category").ValueNumber == (double)Autodesk.Revit.DB.Structure.RebarHostCategory.Floor)
                {
                    var shapedrivenDirRebar = item.DistributionDirection;

                    if (shapedrivenDirRebar.IsXOrY())
                    {
                        item.SetValue("Rebar Direction (XY)", "Y");
                    }
                    else
                    {
                        item.SetValue("Rebar Direction (XY)", "X");
                    }
                }
            }
        }
        public static Model.Entity.Element GetEntityHost (this Model.Entity.Rebar ettRebar)
        {
            var hostRevit = ettRebar.SingleRebar.GetHostId().GetRevitElement();
            var ettHost = new Model.Entity.Element { RevitElement = hostRevit };
            return ettHost;
        }
        public static string GetRebarNumber (this Model.Entity.Rebar ettRebar)
        {
            var revitRebar = ettRebar.SingleRebar;
            var rebarNumber = revitRebar.AsValue("Rebar Number").ValueText;
            return rebarNumber;
        }
        public static Model.Entity.RebarLayer GetRebarLayer (this Model.Entity.Rebar ettRebar)
        {

            var host = ettRebar.SingleRebar.GetHostId().GetRevitElement();
            var ettHostGeometry = ettRebar.EntityHost.Geometry;
            var lengthZDirHost = ettHostGeometry.BasisZ;
            var originZHost = ettHostGeometry.OriginRevit.Z;
            var originZRebar = ettRebar.MaxOccurOriginZCenterLineCurve.Z;
            var distributionPathDir = ettRebar.DistributionDirection;
            if (!distributionPathDir.IsParallelDirection(lengthZDirHost))
            {
                if (originZRebar > originZHost + 50.0.Milimet2Feet() && !distributionPathDir.IsSameDirection(lengthZDirHost))
                {
                    return Model.Entity.RebarLayer.Top;
                }
                else if (originZRebar < originZHost - 50.0.Milimet2Feet() && !distributionPathDir.IsSameDirection(lengthZDirHost))
                {
                    return Model.Entity.RebarLayer.Bottom;
                }
                else
                {
                    return Model.Entity.RebarLayer.Middle;
                }
            }
            else
            {
                return Model.Entity.RebarLayer.None;
            }
            ;
                 
        } 
        public static Model.Entity.RebarFunction GetRebarFunction (this Model.Entity.Rebar ettRebar)
        {
            var host = ettRebar.SingleRebar.GetHostId().GetRevitElement() as Autodesk.Revit.DB.FamilyInstance;
            var ettHostGeo = ettRebar.EntityHost.Geometry;
            var lengthDirHost = ettHostGeo.BasisZ;
            var distributionDirRebar = ettRebar.DistributionDirection;
            var lengthHost = (host).AsValue("Cut Length").ValueNumber.milimeter2Feet();
            var listCurveRebar = ettRebar.DrivingCurves.ToList();
            double lengthRebar = 0;
            if(!lengthDirHost.IsParallelDirection(distributionDirRebar))
            {
                foreach (var item in listCurveRebar)
                {
                    lengthRebar += item.Length;
                }
                if (lengthRebar > lengthHost - 100.0.milimeter2Feet())
                {
                    return Model.Entity.RebarFunction.Straight;
                }
                else
                {
                    return Model.Entity.RebarFunction.Add_Reinforce;
                }
            }
            else
            {
                return Model.Entity.RebarFunction.Stirrup;
            }

        }
    }
}
