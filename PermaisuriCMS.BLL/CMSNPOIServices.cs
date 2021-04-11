using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PermaisuriCMS.DAL;
using PermaisuriCMS.Model;
using System.Configuration;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace PermaisuriCMS.BLL
{
// ReSharper disable InconsistentNaming
    public class CMSNPOIServices
// ReSharper restore InconsistentNaming
    {

        public static int LoadImage(string path, IWorkbook wb)
        {
            var file = new FileStream(path, FileMode.Open, FileAccess.Read);
            var buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);
            return wb.AddPicture(buffer, PictureType.JPEG);
        }

        public static int LoadImage(string path, ISheet sheet, int row, int col, string name)
        {
            var w = (int)(sheet.GetColumnWidth(col) / 36.56) - 1;//36.56
            var h = (int)(sheet.GetRow(row).Height / 15) - 1;
            var fileExtension = Path.GetExtension(path + name);
            var pPath = string.Format("{0}{1:yyyyMMddHHmmssfff}{2}", path, DateTime.Now, fileExtension);
            // bool b = Picture.GetPicThumbnail_Filling(path + name, pPath, h, w, 90);
            var id = 0;
            using (var file = new FileStream(pPath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[file.Length];
                file.Read(buffer, 0, (int)file.Length);
                id = sheet.Workbook.AddPicture(buffer, PictureType.JPEG);
                var patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
                //create the anchor
                var anchor = new HSSFClientAnchor(10, 4, 1023, 255, col, row, col, row);
                anchor.AnchorType = (AnchorType) 1;
                //load the picture and get the picture index in the workbook
                var picture = (HSSFPicture)patriarch.CreatePicture(anchor, id);
                //Reset the image to the original size.
                //picture.Resize();   //Note: Resize will reset client anchor you set.
                //picture.LineStyle = LineStyle.DashDotGel;
            }
            //File.Delete(pPath);
            return id;
        }
        //public string ExportSKU(IEnumerable<CMS_SKU> SKUList, User_Profile_Model userInfo)

        public string ExportSKU(IEnumerable<Export2Excel> skuList, User_Profile_Model userInfo)
        {
            var exportPath = System.Web.HttpContext.Current.Server.MapPath("~/MediaLib/");
            IWorkbook workbook = null;
            using (var file = new FileStream(exportPath + @"/SKUData_Tpl.xlsx", FileMode.Open, FileAccess.Read))
            {
                workbook = WorkbookFactory.Create(file);
            }
            var sheet1 = workbook.GetSheet("Sheet1");
            var i = 0;
            foreach (Export2Excel SKU in skuList)
            {
                i++;//第一行是标题列 忽略
                var curRow = sheet1.CreateRow(i);
                curRow.HeightInPoints = 151;//设置单元格的高度，用于支持图像的显示
                //主显图像
                var firstPic = "NoPicture";
                var j = 0;
                curRow.CreateCell(j++).SetCellValue(i);//显然 这时候的J还是0...

                if (!string.IsNullOrEmpty(SKU.ImgName))
                {
                    firstPic = ConfigurationManager.AppSettings["CMSImgUrl"] + SKU.HMNUM + "/" + SKU.ImgName + SKU.FileFormat;

                    var physicalPath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ImageStoragePath"]);
                    var firstPicSamll = physicalPath + SKU.HMNUM + "/" + SKU.ImgName + "_th" + SKU.FileFormat;
                    if (File.Exists(firstPicSamll))
                    {
                        //IDrawing patriarch = sheet1.CreateDrawingPatriarch();
                        ////create the anchor
                        //XSSFClientAnchor anchor = new XSSFClientAnchor(0, 0, 0, 0, 1, i, 1, i);
                        ////anchor.AnchorType = 2;
                        ////load the picture and get the picture index in the workbook
                        //XSSFPicture picture = (XSSFPicture)patriarch.CreatePicture(anchor, LoadImage(firstPic_Samll, workbook));
                        ////Reset the image to the original size.
                        ////picture.Resize();   //Note: Resize will reset client anchor you set.
                        //picture.LineStyle = LineStyle.DashDotGel;

                        var bytes = File.ReadAllBytes(firstPicSamll);
                        var pictureIdx = workbook.AddPicture(bytes, PictureType.JPEG);
                        // Create the drawing patriarch.  This is the top level container for all shapes. 
                        var patriarch = sheet1.CreateDrawingPatriarch();
                        var anchor = new XSSFClientAnchor(0, 0, 0, 0, 1, i, 1, i);//coloum固定是1
                        var pict = patriarch.CreatePicture(anchor, pictureIdx);
                        pict.Resize();
                    }
                }
                curRow.CreateCell(j++).SetCellValue(firstPic);
                curRow.CreateCell(j++).SetCellValue(SKU.SKU);
                curRow.CreateCell(j++).SetCellValue(SKU.ChannelName);
                curRow.CreateCell(j++).SetCellValue(SKU.ProductName);
                curRow.CreateCell(j++).SetCellValue(SKU.BrandName);
                curRow.CreateCell(j++).SetCellValue(SKU.UPC);

                var landedCost = SKU.FirstCost.ConvertToNotNull()
                           + SKU.OceanFreight.ConvertToNotNull()
                           + SKU.USAHandlingCharge.ConvertToNotNull()
                           + SKU.Drayage.ConvertToNotNull();

                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(landedCost));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.FirstCost.ConvertToNotNull()));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.OceanFreight.ConvertToNotNull()));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.Drayage.ConvertToNotNull()));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.USAHandlingCharge.ConvertToNotNull()));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.EstimateFreight));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.SalePrice));
                curRow.CreateCell(j++).SetCellValue(Convert.ToDouble(SKU.RetailPrice));

                //curRow.CreateCell(j++).SetCellValue(SKU.MaterialName == null ? "NONE" : SKU.MaterialName);
                //curRow.CreateCell(j++).SetCellValue(SKU.ColourName == null ? "NONE" : SKU.ColourName);
                curRow.CreateCell(j++).SetCellValue(SKU.MaterialName ?? "NONE");
                curRow.CreateCell(j++).SetCellValue(SKU.ColourName ?? "NONE");
                curRow.CreateCell(j++).SetCellValue(SKU.HMNUM);
                curRow.CreateCell(j++).SetCellValue(SKU.Pieces);
                curRow.CreateCell(j++).SetCellValue(SKU.MasterPack);
                var boxNum = SKU.Pieces / SKU.MasterPack;
                curRow.CreateCell(j++).SetCellValue(boxNum);
                curRow.CreateCell(j++).SetCellValue(SKU.ProductName);
                curRow.CreateCell(j++).SetCellValue(SKU.StockKey);
                curRow.CreateCell(j++).SetCellValue(SKU.StockkeyQTY.ConvertToNotNull());

                var strWeight = "";
                var strCtn = "";
                foreach (var cCtn in SKU.CMS_HMNUM.CMS_ProductCTN)//view里面如果存在HMNUM，那么CMS_HMNUM也100%存在这个HMNUM，所以不需要做NULL判断 2014年5月2日
                {
                    strWeight = cCtn.CTNTitle + ": " + (cCtn.CTNWeight.ConvertToNotNull() * boxNum).ToString() + "\n";
                    strCtn += cCtn.CTNTitle + ": " + cCtn.CTNLength + "X" + cCtn.CTNWidth + "X" + cCtn.CTNHeight + " \n";
                };
                curRow.CreateCell(j++).SetCellValue(strWeight);
                curRow.CreateCell(j++).SetCellValue(strCtn);

                //var strDim = "";
                //foreach (var cDim in SKU.CMS_HMNUM.CMS_ProductDimension)
                //{

                //    strDim += cDim.DimTitle + ": " + cDim.DimLength + "X" + cDim.DimWidth + "X" + cDim.DimHeight + "\n";
                //};


                var strDim = SKU.CMS_HMNUM.CMS_ProductDimension
                    .Aggregate("", (current, cDim) => current + (cDim.DimTitle + ": " + cDim.DimLength + "X" + cDim.DimWidth + "X" + cDim.DimHeight + "\n"));
                ;

                curRow.CreateCell(j++).SetCellValue(strDim);
                curRow.CreateCell(j++).SetCellValue(SKU.ParentCategoryName ?? "NONE");
                curRow.CreateCell(j++).SetCellValue(SKU.SubCategoryName ?? "NONE");
                curRow.CreateCell(j++).SetCellValue(SKU.StatusName);
                curRow.CreateCell(j++).SetCellValue(SKU.ProductDesc);
                curRow.CreateCell(j++).SetCellValue(SKU.Specifications);
                curRow.CreateCell(j++).SetCellValue(SKU.Keywords);
                string strVis;
                switch (SKU.Visibility.ConvertToNotNull())
                {
                    case 0:
                        strVis = "Online";
                        break;
                    case 1:
                        strVis = "Offline";
                        break;
                    case 2:
                        strVis = "Online";
                        break;
                    default:
                        strVis = "Discontinue";
                        break;
                }
                curRow.CreateCell(j++).SetCellValue(strVis);
                curRow.CreateCell(j++).SetCellValue(SKU.SKU_QTY);
                curRow.CreateCell(j++).SetCellValue(SKU.URL);

                //Force excel to recalculate all the formula while open
                sheet1.ForceFormulaRecalculation = true;
            }
           
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var excelName = userInfo.User_Account +timeStamp+ ".xlsx";
            var curUserExcle = exportPath + excelName;
            //if (File.Exists(curUserExcle))
            //{
            //    //有存在则先清掉，避免第一次导N行数据，第二次导出M行数据，当M<N的时候，该文件里面依然保留有N行旧有的数据（那些未被覆盖的行数）2014年5月5日
            //    File.Delete(curUserExcle);
            //}
            using (var file = new FileStream(curUserExcle, FileMode.OpenOrCreate))
            {
                workbook.Write(file);
            }
            return excelName;
        }

    }
}
