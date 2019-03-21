using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iS3.ImportTools.D2.Shp
{
    public class ShpEditor
    {
        private string _outputFileName;
        public ShpEditor(string outputFileName)
        {
            _outputFileName = outputFileName;
            //点
            OSGeo.OGR.Geometry point = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
            //线
            OSGeo.OGR.Geometry linepoint = new OSGeo.OGR.Geometry(wkbGeometryType.wkbMultiLineString);
            //圆
            OSGeo.OGR.Geometry circlepoint = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLinearRing);
            //椭圆
            OSGeo.OGR.Geometry arcpoint = new OSGeo.OGR.Geometry(wkbGeometryType.wkbMultiPoint);
            //圆弧
            // 为了支持中文路径，请添加下面这句代码
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            // 注册所有的驱动
            Ogr.RegisterAll();
        }
        public void CreateShp(string layerName,List<Geometry> GeomList)
        {
            string strDriverName = "ESRI Shapefile";
            using (Driver oDriver = Ogr.GetDriverByName(strDriverName))
            {
                //用此Driver创建Shape文件
                OSGeo.OGR.DataSource poDS;
                poDS = oDriver.CreateDataSource(_outputFileName, null);
                if (poDS == null)
                    MessageBox.Show("DataSource Creation Error");
                using (DataSource oDS = oDriver.CreateDataSource(_outputFileName, null))
                {
                    //步骤2、创建空间坐标系
                    OSGeo.OSR.SpatialReference oSRS = new OSGeo.OSR.SpatialReference("");
                    oSRS.SetWellKnownGeogCS("WGS84");
                    //步骤3、创建图层，并添加坐标系，创建一个多边形图层(wkbGeometryType.wkbUnknown,存放任意几何特征)
                    Layer oLayer = oDS.CreateLayer(layerName, oSRS, wkbGeometryType.wkbUnknown, null);
                    // 步骤4、下面创建属性表
                    FieldDefn oFieldPlotArea = new FieldDefn("Name", FieldType.OFTString);          // 先创建一个叫PlotArea的属性
                    oFieldPlotArea.SetWidth(100);
                    // 步骤5、将创建的属性表添加到图层中
                    oLayer.CreateField(oFieldPlotArea, 1);
                    foreach (var geom in GeomList)
                    {
                        //步骤6、定义一个特征要素oFeature(特征要素包含两个方面1.属性特征2.几何特征)
                        FeatureDefn oDefn = oLayer.GetLayerDefn();
                        Feature oFeature = new Feature(oDefn);    //建立了一个特征要素并将指向图层oLayer的属性表        
                        oFeature.SetField(0, -1);               //步骤7、设置属性特征的值          

                        oFeature.SetGeometry(geom);     //步骤8、设置几何特征       
                        oLayer.CreateFeature(oFeature); //步骤9、将特征要素添加到图层中
                    }

                }
            }
            MessageBox.Show("成功导出文件到" + _outputFileName);

        }
    }
}


//foreach (iS3Mark mark in iS3Marks)
//                        {
//                            iS3Geom _geom = null;
//                            foreach (iS3Geom geom in iS3Geoms)
//                            {
//                                if (Math.Sqrt((mark.CenterX-geom.CenterX)* (mark.CenterX - geom.CenterX)+
//                                    (mark.CenterY - geom.CenterY) * (mark.CenterY - geom.CenterY))<0.1)
//                                {
//                                    _geom = geom;
//                                }
//                            }
//                            if (_geom != null)
//                            {
//                                _geom.name = mark.name;
//                            }
//                        }

//public class iS3Geom
//{
//    public OSGeo.OGR.Geometry geom { get; set; }
//    public string name { get; set; }
//    public double CenterX { get; set; }
//    public double CenterY { get; set; }
//}



//public class iS3Mark
//{
//    public string name { get; set; }
//    public double CenterX { get; set; }
//    public double CenterY { get; set; }
//}