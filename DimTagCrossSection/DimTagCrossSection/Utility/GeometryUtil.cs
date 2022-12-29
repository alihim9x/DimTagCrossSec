using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class GeometryUtil
    {
        public static Model.Entity.Geometry GetGeometry(this Model.Entity.Element element)
        {
            var geometry = new Model.Entity.Geometry();
            var revitElem = element.RevitElement;
            var typeElem = element.ElementType;
            var revitElemFi = revitElem as Autodesk.Revit.DB.FamilyInstance;
            var tfRv = revitElemFi?.GetTotalTransform();

            switch(typeElem)
            {
                case Model.Entity.ElementType.Framing:
                    geometry.BasisX = tfRv.BasisY;
                    geometry.BasisY = tfRv.BasisZ;
                    geometry.BasisZ = tfRv.BasisX;
                    geometry.LengthX = revitElemFi.Symbol.AsValue("b").ValueNumber.milimeter2Feet();
                    geometry.LengthY = revitElemFi.Symbol.AsValue("h").ValueNumber.milimeter2Feet();
                    geometry.LengthZ = revitElemFi.AsValue("Cut Length").ValueNumber.milimeter2Feet();

                    geometry.Origin = tfRv.Origin - (geometry.BasisY * (geometry.LengthY / 2)) - (geometry.BasisX * (geometry.LengthX / 2))
                        - (geometry.BasisZ*(geometry.LengthZ/2));  // THÊM DI CHUYỂN ORIGIN VỀ ĐIỂM ĐẦU THEO Z
                    geometry.OriginRevit = tfRv.Origin;
                    break;
                case Model.Entity.ElementType.Column:
                    geometry.BasisX = tfRv.BasisY;
                    geometry.BasisY = tfRv.BasisZ;
                    geometry.BasisZ = tfRv.BasisX;
                    geometry.LengthX = revitElemFi.Symbol.AsValue("b").ValueNumber.milimeter2Feet();
                    geometry.LengthY = revitElemFi.Symbol.AsValue("h").ValueNumber.milimeter2Feet();
                    break;
                default:
                    throw new Model.Exception.CaseNotCheckException();
                  
            }
            return geometry;
        }
    }
}
