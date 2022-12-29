using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
namespace Model.ViewModel
{
    public class SettingView : NotifyClass
    {
        public Model.Entity.Setting Setting { get; set; }
        public List<Autodesk.Revit.DB.MultiReferenceAnnotationType> MultiRefAnnotationTypes
        {
            get
            {
                return ModelData.Instance.MultiRefAnnotationTypes;
            }
        }
        public List<Autodesk.Revit.DB.DimensionType> DimensionTypes
        {
            get
            {
                return ModelData.Instance.DimensionTypes;
            }
        }
        public List<Autodesk.Revit.DB.FamilySymbol> StructuralRebarTags
        {
            get
            {
                return ModelData.Instance.StructuralRebarTags;
            }
        }
        public List<Autodesk.Revit.DB.RevitLinkInstance> RevitLinks
        {
            get
            {
                return ModelData.Instance.RevitLinks;
            }
        }
        
            
           
    }
}
