using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using Autodesk.Revit.DB;
namespace Utility
{
    public static class AnnotationUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        
        public static void CreateDimensionCrossSection(this Model.Entity.ActiveView ettActiveView)
        {
            var setting = ModelData.Instance.Setting;
            var rvLink = setting.RevitLink;
            ReferenceArray framingLRRefArray = new ReferenceArray();
            ReferenceArray framingTBRefArray1 = new ReferenceArray();
            ReferenceArray framingTBRefArray2 = new ReferenceArray();
            Model.Entity.Element ettFramingInstanceInview = null;
            List<Model.Entity.Element> ettFloorInstancesInView = new List<Model.Entity.Element>();
            Model.Entity.Element ettColumnInstanceInView = null;
            var ettElems = ettActiveView.EntityElements;
            foreach (var item in ettElems)
            {
                
                switch(item.ElementType)
                {
                    
                    case Model.Entity.ElementType.Framing:
                        ettFramingInstanceInview = item;
                        break;
                    case Model.Entity.ElementType.Floor:
                        ettFloorInstancesInView.Add(item);
                        break;
                    case Model.Entity.ElementType.Column:
                        ettColumnInstanceInView = item;
                        break;
                }
            }
            
            var framingInViewGeometry = ettFramingInstanceInview?.Geometry;
            var framingInViewOrigin = framingInViewGeometry?.OriginRevit;
            var facesFraming = ettFramingInstanceInview?.Faces;
            var facesColumn = ettColumnInstanceInView?.Faces;
            if (rvLink != null)
            {
                framingLRRefArray.Append((facesFraming?.LeftFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                framingLRRefArray.Append((facesFraming?.RightFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                framingTBRefArray1.Append((facesFraming?.TopFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                framingTBRefArray1.Append((facesFraming?.BotFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                framingTBRefArray2.Append((facesFraming.TopFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                framingTBRefArray2.Append((facesFraming.BotFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());

                foreach (var item in ettFloorInstancesInView)
                {
                    if (!item.Faces.TopFace.Origin.Z.IsEqual(facesFraming.TopFace.Origin.Z)
                        && !item.Faces.BotFace.Origin.Z.IsEqual(facesFraming.BotFace.Origin.Z))
                    {
                        framingTBRefArray1.Append((item.Faces.TopFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                        framingTBRefArray1.Append((item.Faces.BotFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                    }
                    else if (item.Faces.TopFace.Origin.Z.IsEqual(facesFraming.TopFace.Origin.Z))
                    {
                        framingTBRefArray1.Append((item.Faces.BotFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                    }
                    else if (item.Faces.BotFace.Origin.Z.IsEqual(facesFraming.BotFace.Origin.Z))
                    {
                        framingTBRefArray1.Append((item.Faces.TopFace.Reference.CreateLinkReference(rvLink) as Reference).MakeLinkedRef4Dim());
                    }
                    
                }
            }
            else
            {
                framingLRRefArray.Append(facesFraming?.LeftFace.Reference);
                framingLRRefArray.Append(facesFraming?.RightFace.Reference);
                framingTBRefArray1.Append(facesFraming?.TopFace.Reference);
                framingTBRefArray1.Append(facesFraming?.BotFace.Reference);
                framingTBRefArray2.Append(facesFraming.TopFace.Reference);
                framingTBRefArray2.Append(facesFraming.BotFace.Reference);

                foreach (var item in ettFloorInstancesInView)
                {
                    if (!item.Faces.TopFace.Origin.Z.IsEqual(facesFraming.TopFace.Origin.Z)
                        && !item.Faces.BotFace.Origin.Z.IsEqual(facesFraming.BotFace.Origin.Z))
                    {
                        framingTBRefArray1.Append(item.Faces.TopFace.Reference);
                        framingTBRefArray1.Append(item.Faces.BotFace.Reference);
                    }
                    else if (item.Faces.TopFace.Origin.Z.IsEqual(facesFraming.TopFace.Origin.Z))
                    {
                        framingTBRefArray1.Append(item.Faces.BotFace.Reference);
                    }
                    else if (item.Faces.BotFace.Origin.Z.IsEqual(facesFraming.BotFace.Origin.Z))
                    {
                        framingTBRefArray1.Append(item.Faces.TopFace.Reference);
                    }

                }
            }


            
            
            var dimensionOriginLR = framingInViewOrigin + (framingInViewGeometry.LengthY/2 + 200.0.Milimet2Feet()) * ettActiveView.RevitActiveView.UpDirection;
            var dimensionOriginTB1 = framingInViewOrigin + (framingInViewGeometry.LengthX +  200.0.Milimet2Feet()) * -ettActiveView.RevitActiveView.RightDirection;
            var dimensionOriginTB2 = framingInViewOrigin + (framingInViewGeometry.LengthX + 400.0.Milimet2Feet()) * -ettActiveView.RevitActiveView.RightDirection;
            var dimensionLineLR = Line.CreateBound(dimensionOriginLR, dimensionOriginLR + ettActiveView.RevitActiveView.RightDirection);
            var dimensionLineTB1 = Line.CreateBound(dimensionOriginTB1, dimensionOriginTB1 + ettActiveView.RevitActiveView.UpDirection);
            var dimensionLineTB2 = Line.CreateBound(dimensionOriginTB2, dimensionOriginTB2 + ettActiveView.RevitActiveView.UpDirection);
            var dimType = setting.DimensionType;

            


            if (ettFloorInstancesInView.Count != 0)
            {
                var dimLR = revitData.Document.Create.NewDimension(ettActiveView.RevitActiveView, dimensionLineLR, framingLRRefArray, dimType);
                var dimTB1 = revitData.Document.Create.NewDimension(ettActiveView.RevitActiveView, dimensionLineTB1, framingTBRefArray1, dimType);
                var dimTB2 = revitData.Document.Create.NewDimension(ettActiveView.RevitActiveView, dimensionLineTB2, framingTBRefArray2, dimType);

            }
            else if (ettFloorInstancesInView.Count == 0)
            {
                revitData.Document.Create.NewDimension(ettActiveView.RevitActiveView, dimensionLineLR, framingLRRefArray, dimType);
                revitData.Document.Create.NewDimension(ettActiveView.RevitActiveView, dimensionLineTB1, framingTBRefArray1, dimType);
                //revitData.Document.Create.NewDetailCurve(ettActiveView.RevitActiveView, Autodesk.Revit.DB.Line.CreateBound(dimensionOriginTB2, dimensionOriginTB2 + 500.0.milimeter2Feet() * ettActiveView.RevitActiveView.UpDirection));
            }
            
        }
        public static void CreateMultiRebarTagFromList (this List<List<Model.Entity.Rebar>> multiRebar)
        {
            var ettActiveView = ModelData.Instance.ActiveView;
            List<Model.Entity.Rebar> ettRebarsToTag = new List<Model.Entity.Rebar>();
            List<ElementId> rebarIdsToTag = new List<ElementId>();
            var framing = ettActiveView.EntityElements.Where(x => x.ElementType == Model.Entity.ElementType.Framing).SingleOrDefault();            
            var framingGeometry = framing.Geometry;
            var multiRefAnnotationType = ModelData.Instance.Setting.MultiRefAnnotationType;
            if(multiRefAnnotationType != null)
            {
                MultiReferenceAnnotationOptions multiRebarTagOpts = new MultiReferenceAnnotationOptions(multiRefAnnotationType);
                multiRebarTagOpts.TagHeadPosition = new XYZ(0, 1, 0);
                multiRebarTagOpts.DimensionLineDirection = -revitData.ActiveView.RightDirection;
                multiRebarTagOpts.DimensionPlaneNormal = revitData.ActiveView.ViewDirection;
                double a = 100;
                double yOffsetDimOrigin = 0;
                double yOffsetTagHead = 0;
                for (int i = 0; i < multiRebar.Count; i++)
                {

                    if (multiRebar[i][0]?.RebarLayer == Model.Entity.RebarLayer.Top)
                    {
                        if (multiRebar[i][0].RebarFunction == Model.Entity.RebarFunction.Straight)
                        {
                            yOffsetDimOrigin = framingGeometry.LengthY / 2 + 40.0.milimeter2Feet();
                        }
                        else if (multiRebar[i][0].RebarFunction == Model.Entity.RebarFunction.Add_Reinforce)
                        {
                            yOffsetDimOrigin = framingGeometry.LengthY / 2 - a.milimeter2Feet();
                            a += a / 1.5;
                        }

                    }
                    else if (multiRebar[i][0]?.RebarLayer == Model.Entity.RebarLayer.Bottom)
                    {
                        if (multiRebar[i][0].RebarFunction == Model.Entity.RebarFunction.Straight)
                        {
                            yOffsetDimOrigin = -(framingGeometry.LengthY / 2 + 40.0.milimeter2Feet());
                        }
                        else if (multiRebar[i][0].RebarFunction == Model.Entity.RebarFunction.Add_Reinforce)
                        {
                            yOffsetDimOrigin = -(framingGeometry.LengthY / 2 - a.milimeter2Feet());
                            a += a / 1.7;
                        }
                    }
                    else if(multiRebar[i][0]?.RebarLayer == Model.Entity.RebarLayer.Middle)
                    {
                        yOffsetDimOrigin = 40.0.milimeter2Feet();
                    }

                    for (int j = 0; j < multiRebar[i].Count; j++)
                    {
                        ettRebarsToTag.Add(multiRebar[i][j]);
                    }
                    ettRebarsToTag.ToList().ForEach(x => rebarIdsToTag.Add(x.SingleRebar.Id));
                    multiRebarTagOpts.SetElementsToDimension(rebarIdsToTag);
                    multiRebarTagOpts.DimensionLineOrigin = framingGeometry.OriginRevit + (yOffsetDimOrigin) * framingGeometry.BasisY;
                    yOffsetTagHead = yOffsetDimOrigin - 0.00559;
                    var mra = MultiReferenceAnnotation.Create(revitData.Document, revitData.ActiveView.Id, multiRebarTagOpts);
                    var mraTag = revitData.Document.GetElement(mra.TagId) as IndependentTag;
                    mraTag.TagHeadPosition = framingGeometry.OriginRevit + ((yOffsetTagHead) * revitData.ActiveView.UpDirection) + (150.0.milimeter2Feet()+framingGeometry.LengthX/2) * revitData.ActiveView.RightDirection;
                    ettRebarsToTag.Clear();
                    rebarIdsToTag.Clear();

                }


            }
        }
        
        public static void CreateRebarTag (this Model.Entity.ActiveView ettActiveView)
        {
            
            var rebars = ettActiveView.EntityRebars;
           

            var topRebars = rebars.Where(x => x.RebarLayer == Model.Entity.RebarLayer.Top).GroupBy(x => x.RebarNumber).ToList();
            var botRebars = rebars.Where(x => x.RebarLayer == Model.Entity.RebarLayer.Bottom).GroupBy(x => x.RebarNumber).ToList();
            var midRebars = rebars.Where(x => x.RebarLayer == Model.Entity.RebarLayer.Middle).ToList();
            var stirrupRebars = rebars.Where(x => x.RebarFunction == Model.Entity.RebarFunction.Stirrup).ToList();
            List<Model.Entity.Rebar> singleTopRebar = new List<Model.Entity.Rebar>();
            List<List<Model.Entity.Rebar>> multiTopRebar = new List<List<Model.Entity.Rebar>>();
            List<Model.Entity.Rebar> singleBotRebar = new List<Model.Entity.Rebar>();
            List<List<Model.Entity.Rebar>> multiBotRebar = new List<List<Model.Entity.Rebar>>();
            List<List<Model.Entity.Rebar>> midRebar = new List<List<Model.Entity.Rebar>>();
            if(midRebars.Count>0)
            {
                midRebar.Add(midRebars);
            }



            foreach (var rebar in topRebars)
            {
                if(rebar.Select(x=>x).ToList().Count == 1 && rebar.Select(x=>x).ToList().First().AsValue("Quantity").ValueNumber == 1)
                {
                    singleTopRebar.Add(rebar.Select(x=>x).ToList().FirstOrDefault());
                }
                else
                {
                    multiTopRebar.Add(rebar.Select(x => x).ToList());
                }
            }
            foreach (var rebar in botRebars)
            {
                if (rebar.Select(x => x).ToList().Count == 1 && rebar.Select(x => x).ToList().First().AsValue("Quantity").ValueNumber == 1)
                {
                    singleBotRebar.Add(rebar.Select(x => x).ToList().FirstOrDefault());
                }
                else
                {
                    multiBotRebar.Add(rebar.Select(x => x).ToList());
                }
            }
            multiTopRebar?.CreateMultiRebarTagFromList();
            multiBotRebar?.CreateMultiRebarTagFromList();
            singleTopRebar?.ForEach(x => x.CreateSingleRebarTag());
            singleBotRebar?.ForEach(x => x.CreateSingleRebarTag());
            midRebar?.CreateMultiRebarTagFromList();
            stirrupRebars.ForEach(x => x.CreateSingleRebarTag());

        }
        public static void CreateSingleRebarTag (this Model.Entity.Rebar ettRebar)
        {
            var ettActiveView = ModelData.Instance.ActiveView; 
            
            var framing = ettActiveView.EntityElements.Where(x => x.ElementType == Model.Entity.ElementType.Framing).SingleOrDefault();
            var framingGeometry = framing.Geometry;
            var setting = ModelData.Instance.Setting;
            var midRebar = ettActiveView.EntityRebars.Where(x => x.RebarLayer == Model.Entity.RebarLayer.Middle).FirstOrDefault();
            ElementId tagTypeId = null;
            if (ettRebar.RebarFunction == Model.Entity.RebarFunction.Stirrup)
            {
                tagTypeId = setting.StirrupRebarTag?.Id;

                
            }
            else
            {
                tagTypeId = setting.StructuralRebarTag?.Id;
   
               
            }
            
            if(tagTypeId  != null)
            {
                XYZ pnt2PlaceTag = new XYZ();
                XYZ pnt4LeaderEnd = ettRebar.MaxOccurOriginZCenterLineCurve;
                double yOffsetTagHead = 0;
                switch (ettRebar.RebarLayer)
                {
                    case Model.Entity.RebarLayer.Top:
                        pnt2PlaceTag = framingGeometry.OriginRevit + (framingGeometry.LengthY / 2 + 110.0.milimeter2Feet()) * framingGeometry.BasisY;

                        break;
                    case Model.Entity.RebarLayer.Bottom:
                        pnt2PlaceTag = framingGeometry.OriginRevit - (framingGeometry.LengthY / 2 + 110.0.milimeter2Feet()) * framingGeometry.BasisY;
                        break;
                    case Model.Entity.RebarLayer.None:
                        if(midRebar == null)
                        {
                            pnt2PlaceTag = framingGeometry.OriginRevit;
                            pnt4LeaderEnd = pnt2PlaceTag - (pnt2PlaceTag.X - pnt4LeaderEnd.X) * revitData.ActiveView.RightDirection;
                            break;
                        }
                        else
                        {
                            pnt2PlaceTag = framingGeometry.OriginRevit - 30.0.milimeter2Feet() * revitData.ActiveView.UpDirection;
                            pnt4LeaderEnd = pnt2PlaceTag - (pnt2PlaceTag.X - pnt4LeaderEnd.X) * revitData.ActiveView.RightDirection;
                            break;
                        }
                        
                }
                
                IndependentTag tagRebar = IndependentTag.Create(revitData.Document, tagTypeId, ettActiveView.RevitActiveView.Id, ettRebar.Reference, true, TagOrientation.Horizontal, pnt2PlaceTag);
                tagRebar.SetValue("Leader Type", 1);
                
                tagRebar.LeaderEnd = pnt4LeaderEnd;         
                tagRebar.TagHeadPosition = pnt2PlaceTag - (0.00559 * revitData.ActiveView.UpDirection) + (150.0.milimeter2Feet() + framingGeometry.LengthX/2) * revitData.ActiveView.RightDirection;
                tagRebar.LeaderElbow = pnt2PlaceTag - (pnt2PlaceTag.X - tagRebar.LeaderEnd.X)*revitData.ActiveView.RightDirection ;
            }
            
        }
        public static void InsertBreakLineFamily(this Model.Entity.ActiveView ettActive)
        {
            var ettElems = ettActive.EntityElements;
            var ettFramingInView = ettElems.Where(x => x.ElementType == Model.Entity.ElementType.Framing).SingleOrDefault();
            var ettFloorsInView = ettElems.Where(x => x.ElementType == Model.Entity.ElementType.Floor).ToList();
            var leftFaceFraming = ettFramingInView.Faces.LeftFace;
            var rightFaceFraming = ettFramingInView.Faces.RightFace;
        }
    }

}
