using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Runtime;

namespace iS3.ImportTools.D2.CAD
{
    public class DWGReader
    {
        public static Tuple<List<string>,List<Geometry>> ReadPropertyFromFile(string inputfilename,string layerName)
        {
            List<string> layerList = new List<string>();    //图层列表
            Database pDb;
            Transaction trans;
            List<string> result = new List<string>();
            List<Geometry> resultGeo = new List<Geometry>();
            using (Services ser = new Services())
            {
                using (pDb = new Database(false, false))//不加参数会出错                 
                {
                    pDb.ReadDwgFile(inputfilename, FileOpenMode.OpenForReadAndReadShare, false, "");
                    using (trans = pDb.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord btab = (BlockTableRecord)pDb.CurrentSpaceId.GetObject(OpenMode.ForRead);
                        List<string> bb = new List<string>();

                        foreach (ObjectId btr in btab)   //所有对象遍历
                        {
                            DBObject obj;
                            try
                            {
                                obj = trans.GetObject(btr, OpenMode.ForWrite);
                            }
                            catch
                            {
                                return null;
                            };

                            string a = obj.GetType().Name;
                            if (!bb.Contains(a))
                            {
                                bb.Add(a);
                            }
                            #region  switch
                            switch (obj.GetType().Name)
                            {
                                case "DBPoint":
                                    DBPoint dbpoint = (DBPoint)obj;

                                    OSGeo.OGR.Geometry point = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPoint);
                                    point.AddPoint(dbpoint.Position.X, dbpoint.Position.Y,0);
                                    resultGeo.Add(point);
                                    if (dbpoint.Layer != layerName) break;
                                    result.Add(string.Format("Point---X:{0},T:{1}", dbpoint.Position.X, dbpoint.Position.Y));
                                    break;
                                case "Line":
                                    Line line = (Line)obj;
                                    if (line.Layer != layerName) break;
                                    //result.Add(line);
                                    break;
                                case "Ellipse":
                                    Ellipse ellipse = (Ellipse)obj;
                                    if (ellipse.Layer != layerName) break;
                                    //result.Add(ellipse);
                                    break;
                                #region
                                case "Arc":
                                    Arc arc = (Arc)obj;
                                    if (arc.Layer != layerName) break;
                                   // result.Add(arc);
                                    break;
                                #endregion
                                case "Circle":
                                    Circle circle = (Circle)obj;


                                    if (circle.Layer != layerName) break;
                                    result.Add(string.Format("Circle---centerX:{0},centerY:{1},radius:{2}", circle.Center.X, circle.Center.Y, circle.Radius));
                                    //result.Add(circle);
                                    break;
                                case "Polyline":
                                    Teigha.DatabaseServices.Polyline polyline = (Teigha.DatabaseServices.Polyline)obj;
                                    if (polyline.Layer != layerName) break;
                                    //result.Add(polyline);
                                    break;
                                case "DBText":
                                    DBText dBText = (DBText)obj;
                                    if (dBText.Layer != layerName) break;
                                    result.Add(string.Format("DBText---TextString:{0}", dBText.TextString));
                                    // result.Add(dBText);
                                    break;
                                case "Region":
                                    Region region = (Region)obj;
                                    if (region.Layer != layerName) break;
                                    //result.Add(region);
                                    break;
                                default:
                                    break;
                            }
                            #endregion

                        }
                    }
                }
            }
            return new Tuple<List<string>, List<Geometry>>(result,resultGeo);
        }
        
        public static List<string> ReadLayerFromFile(string inputfilename)
        {
            List<string> layerList = new List<string>();    //图层列表
            Database pDb;
            Transaction trans;
            using (Services ser = new Services())
            {
                using (pDb = new Database(false, false))//不加参数会出错                 
                {
                    pDb.ReadDwgFile(inputfilename, FileOpenMode.OpenForReadAndReadShare, false, "");
                    using (trans = pDb.TransactionManager.StartTransaction())
                    {
                        #region   获取CAD内的所有图层

                        LayerTable lt = (LayerTable)trans.GetObject(pDb.LayerTableId, OpenMode.ForRead);
                        foreach (ObjectId layerId in lt)
                        {
                            LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(layerId, OpenMode.ForRead);
                            //ltr.Name;  图层名
                            layerList.Add(ltr.Name);
                        }
                        #endregion
                    }
                }
            }
            return layerList;
        }
    }
}
