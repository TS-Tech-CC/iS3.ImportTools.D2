using iS3.ImportTools.D2.CAD;
using iS3.ImportTools.D2.Shp;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
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
using Teigha.DatabaseServices;

namespace iS3.ImportTools.D2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            //注册GDAL
            GdalConfiguration.ConfigureGdal();

            GdalConfiguration.ConfigureOgr();
            Gdal.AllRegister();
        }
        string selectFileName;
        string selectLayerName;
        List<OSGeo.OGR.Geometry> resultGeo;
        //选择要打开的CAD文件
        private void InputFileBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dwg";
            dlg.Filter = "dwg documents (.dwg)|*.dwg";

            // Get the selected file name and display in a TextBox 
            if (dlg.ShowDialog() == true)
            {
                InputFileNameTB.Text = dlg.FileName;
                selectFileName = dlg.FileName;
                List<string> layerList = DWGReader.ReadLayerFromFile(dlg.FileName);
                layerLB.ItemsSource = layerList;
               // new DWGReader().ReadFile(dlg.FileName, @"E://shp/11.shp");
            }
        }

       
        //点击查看图层内属性
        private void layerLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectLayerName = layerLB.SelectedItem as string;
            var result = DWGReader.ReadPropertyFromFile(selectFileName, selectLayerName);
            List<string> resultList= result.Item1;
            resultGeo = result.Item2;
            PropertyDG.ItemsSource = resultList;
        }

        //导出
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            string exportFile = ExportFileNameTB.Text;
            ShpEditor editor = new ShpEditor(exportFile);
            editor.CreateShp(selectLayerName, resultGeo);
        }
    }
}
