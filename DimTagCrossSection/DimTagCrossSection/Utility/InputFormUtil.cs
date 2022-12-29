using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;
using System.Diagnostics;

namespace Utility
{
    public static class InputFormUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        private static ModelData modelData
        {
            get
            {
                return ModelData.Instance;
            }
        }
        public static void Run(this Model.Entity.ActiveView ettActiveView)
        {
            //var b = revitData.InstanceElementsInViewRvL.ToList();
            //var a = revitData.FramingInstancesInViewRvL.ToList();
            //var c = revitData.FloorInstancesInViewRvL.ToList();
            var ettRebars = ettActiveView.EntityRebars;
            AnnotationUtil.CreateDimensionCrossSection(ettActiveView);
            if(ettRebars.Count != 0 )
            {
                AnnotationUtil.CreateRebarTag(ettActiveView);

            }


            //a.ForEach(x => Autodesk.Revit.UI.TaskDialog.Show("Revit", $"{x.Id}"));
            


        }
        public static void SelectRevitLink()
        {
            var form = FormData.Instance.InputForm;
            form.Hide();
            var sel = revitData.Selection;
            try
            {
                FormData.Instance.SettingView.Setting.RevitLink = sel.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element).GetRevitElement() as Autodesk.Revit.DB.RevitLinkInstance;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

            }
            form.ShowDialog();
        }
    }
}
