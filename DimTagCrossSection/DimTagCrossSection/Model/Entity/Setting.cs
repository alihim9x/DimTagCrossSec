using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using SingleData;

namespace Model.Entity
{
    public class Setting : NotifyClass
    {
        private Autodesk.Revit.DB.MultiReferenceAnnotationType multiRefAnnotationType;
        public Autodesk.Revit.DB.MultiReferenceAnnotationType MultiRefAnnotationType
        {
            get
            {
                return multiRefAnnotationType;
            }
            set
            {
                if (multiRefAnnotationType == value) return;
                multiRefAnnotationType = value; OnPropertyChanged();
            }
        }
        private Autodesk.Revit.DB.DimensionType dimensionType;
        public Autodesk.Revit.DB.DimensionType DimensionType
        {
            get
            {
                return dimensionType;
            }
            set
            {
                if (dimensionType == value) return;
                dimensionType = value;
                OnPropertyChanged();

            }
        }
        private Autodesk.Revit.DB.IndependentTag independentTag;
        public virtual Autodesk.Revit.DB.IndependentTag IndependentTag
        {
            get
            {
                return independentTag;
            }
            set
            {
                if (independentTag == value) return;
                independentTag = value;
                OnPropertyChanged();
            }
        }
        private Autodesk.Revit.DB.FamilySymbol structuralRebarTag;
        public virtual Autodesk.Revit.DB.FamilySymbol StructuralRebarTag
        {
            get
            {
                return structuralRebarTag;
            }
            set
            {
                if (structuralRebarTag == value) return;
                structuralRebarTag = value;
                OnPropertyChanged();
            }
        }
        private Autodesk.Revit.DB.FamilySymbol stirrupRebarTag;
        public virtual Autodesk.Revit.DB.FamilySymbol StirrupRebarTag
        {
            get
            {
                return stirrupRebarTag;
            }
            set
            {
                if (stirrupRebarTag == value) return;
                stirrupRebarTag = value;
                OnPropertyChanged();
            }
        }
        private Autodesk.Revit.DB.RevitLinkInstance revitLink;
        public virtual Autodesk.Revit.DB.RevitLinkInstance RevitLink
        {
            get
            {
                return revitLink;
            }
            set
            {
                if (revitLink == value) return;
                revitLink = value;
                OnPropertyChanged();
            }
        }
    }
}
