using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SingleData;

namespace Utility
{
    public static class ActiveViewUtil
    {
        private static RevitData revitData
        {
            get
            {
                return RevitData.Instance;
            }
        }
        public static List<Model.Entity.Element> GetEntityElements(this Model.Entity.ActiveView ettActiveView)
        {
            var setting = ModelData.Instance.Setting;
            List<Model.Entity.Element> ettElements = new List<Model.Entity.Element>();
            var rvLink = setting.RevitLink;
            
            var rvLFramingInStancesInView = revitData.FramingInstancesInViewRvL?.ToList().FirstOrDefault();
            var rvLFloorInstancesInView = revitData.FloorInstancesInViewRvL?.ToList();

            var framingInstanceInView = revitData.FramingInstancesInView?.ToList().FirstOrDefault();
            var floorInstanceInView = revitData.FloorInstancesInview?.ToList();
            var columnInstanceInView = revitData.ColumnInstancesInView?.ToList().FirstOrDefault();
            if(rvLink == null)
            {
                if (framingInstanceInView != null)
                {
                    ettElements.Add(new Model.Entity.Element { RevitElement = framingInstanceInView });
                }
                if (floorInstanceInView != null)
                {
                    foreach (var item in floorInstanceInView)
                    {
                        if (ElementUtil.IsIntersectWithActiveView(item))
                        {
                            ettElements.Add(new Model.Entity.Element { RevitElement = item });
                        }
                    }
                    //ettElements.Add(new Model.Entity.Element { RevitElement = floorInstanceInView });
                }
                if (columnInstanceInView != null)
                {
                    ettElements.Add(new Model.Entity.Element { RevitElement = columnInstanceInView });
                }
            }
            else
            {
                if (rvLFramingInStancesInView != null)
                {
                    ettElements.Add(new Model.Entity.Element { RevitElement = rvLFramingInStancesInView });
                }
                if (rvLFloorInstancesInView != null)
                {
                    foreach (var item in rvLFloorInstancesInView)
                    {
                        
                            ettElements.Add(new Model.Entity.Element { RevitElement = item });
                    }
                    //ettElements.Add(new Model.Entity.Element { RevitElement = floorInstanceInView });
                }
            }
            
            return ettElements;
        }
        public static List<Model.Entity.Rebar> GetEntityRebars (this Model.Entity.ActiveView ettActiveView)
        {
            var setting = ModelData.Instance.Setting;
            var rvLink = setting.RevitLink;
            List<Model.Entity.Rebar> ettRebars = new List<Model.Entity.Rebar>();
            var curDocRebarsInView = revitData.RebarsInView.ToList();
            var rvLinkRebarsInView = revitData.RebarInstancesInViewRvL.ToList();
            if(rvLink == null)
            {
                curDocRebarsInView.ForEach(x => ettRebars.Add(new Model.Entity.Rebar(x)));
            }
            else
            {
                rvLinkRebarsInView.ForEach(x => ettRebars.Add(new Model.Entity.Rebar(x)));
            }
            return ettRebars;
        }
        
    }
}
