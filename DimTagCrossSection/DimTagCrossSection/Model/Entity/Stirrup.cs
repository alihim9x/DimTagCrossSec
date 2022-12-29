using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Utility;
using SingleData;

namespace Model.Entity
{
    public class Stirrup : NotifyClass
    {
        public Element Element { get; set; }

        private int sectionIndex;
        public int SectionIndex
        {
            get
            {
                return sectionIndex;
            }
            set
            {
                if (sectionIndex == value) return;
                sectionIndex = value; OnPropertyChanged();
            }
        }

        
        private double spacing;
        public double Spacing
        {
            get
            {
                return spacing;
            }
            set
            {
                spacing = value;
                OnPropertyChanged();
            }
        }

        

        

        
        
    }
}
