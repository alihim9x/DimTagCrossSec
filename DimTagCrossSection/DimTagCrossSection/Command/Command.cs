using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using SingleData;
using Autodesk.Revit.UI.Selection;
using Utility;
using Autodesk.Revit.DB.Structure;
using System.Diagnostics;

namespace DimTagCrossSection
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #region Initial
            var singleTon = Singleton.Instance = new Singleton();
            var revitData = singleTon.RevitData;
            revitData.UIApplication = commandData.Application;
            var sel = revitData.Selection;
            var doc = revitData.Document;
            var activeView = revitData.ActiveView;
            var tx = revitData.Transaction;
            var uidoc = revitData.UIDocument;
            var app = revitData.Application;
            tx.Start();
            #endregion


            var form = FormData.Instance.InputForm;
            form.ShowDialog();



            //AnnotationUtil.CreateMultiRebarTag(ettActiveView);

            //var sectionView = doc.GetElement(new ElementId(346209));
            //var bbActiveView = sectionView.get_BoundingBox(null);
            //var solidActivew = bbActiveView.CreateSolidFromBBox();

            //Action<Document> differenceAction8 = ((x) =>
            //{
            //    var translateSolid8 = solidActivew.MoveToOrigin();
            //    FreeFormElement.Create(x, translateSolid8);

            //});

            //var columnFamily8 = FamilyUtil.Create($"Solid_{Guid.NewGuid()}", differenceAction8);
            //columnFamily8.Insert(sel.PickPoint());





            tx.Commit();


            return Result.Succeeded;       
        }
    }
}
