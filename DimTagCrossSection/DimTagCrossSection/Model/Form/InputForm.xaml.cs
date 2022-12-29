using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SingleData;
using Utility;

namespace Model.Form
{
    /// <summary>
    /// Interaction logic for InputForm.xaml
    /// </summary>
    public partial class InputForm : Window
    {
        public InputForm()
        {
            InitializeComponent();
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            var activeView = ModelData.Instance.ActiveView;
            InputFormUtil.Run(activeView);
            //Autodesk.Revit.UI.TaskDialog.Show("Revit", "Done!");
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            var form = FormData.Instance.InputForm;
            form.Close();
            //sw.Stop();
            //Autodesk.Revit.UI.TaskDialog.Show("Revit", $"{sw.Elapsed}");
        }

        private void SelectRevitLink_Click(object sender, RoutedEventArgs e)
        {
            InputFormUtil.SelectRevitLink();
        }
    }
}
