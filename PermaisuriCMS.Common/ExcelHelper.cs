using System.Data;
using NPOI.HSSF.UserModel;
using System;
using System.IO;
using System.Text;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using System.Collections;
using NPOI.XSSF.UserModel;

namespace PermaisuriCMS.Common
{
    public class ExcelHelper
    {

        #region 从datatable中将数据导出到excel
        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        [Obsolete]
        static MemoryStream ExportToMemorySteram(DataTable dtSource, string strHeaderText)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet() as XSSFSheet;

            #region 右击文件 属性信息

            //{
            //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //    dsi.Company = "http://www.yongfa365.com/";
            //    workbook.DocumentSummaryInformation = dsi;

            //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //    si.Author = "Lee"; //填加xls文件作者信息
            //    si.ApplicationName = "NPOI测试程序"; //填加xls文件创建程序信息
            //    si.LastAuthor = "Charlotte"; //填加xls文件最后保存者信息
            //    si.Comments = "说明信息"; //填加xls文件作者信息
            //    si.Title = "NPOI测试"; //填加xls文件标题信息
            //    si.Subject = "NPOI测试Demo"; //填加文件主题信息
            //    si.CreateDateTime = DateTime.Now;
            //    workbook.SummaryInformation = si;
            //}

            #endregion

            var dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            var format = workbook.CreateDataFormat() as HSSFDataFormat;
            if (dateStyle != null)
            {
                if (format != null) dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

                //取得列宽
                var arrColWidth = new int[dtSource.Columns.Count];
                foreach (DataColumn item in dtSource.Columns)
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName).Length;
                }
                for (var i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (var j = 0; j < dtSource.Columns.Count; j++)
                    {
                        var intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth[j])
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
                var rowIndex = 0;

                foreach (DataRow row in dtSource.Rows)
                {
                    #region 新建表，填充表头，填充列头，样式

                    if (rowIndex == 65535 || rowIndex == 0)
                    {
                        if (rowIndex != 0)
                        {
                            sheet = workbook.CreateSheet() as XSSFSheet;
                        }

                        #region 表头及样式

                        {
                            var headerRow = sheet?.CreateRow(0) as XSSFRow;
                            if (headerRow != null)
                            {
                                headerRow.HeightInPoints = 25;
                                headerRow.CreateCell(0).SetCellValue(strHeaderText);

                                var headStyle = workbook.CreateCellStyle() as XSSFCellStyle;
                                if (headStyle != null)
                                {
                                    headStyle.Alignment = HorizontalAlignment.Center;
                                    var font = workbook.CreateFont() as XSSFFont;
                                    if (font != null)
                                    {
                                        font.FontHeightInPoints = 20;
                                        font.Boldweight = 700;
                                        headStyle.SetFont(font);
                                    }

                                    headerRow.GetCell(0).CellStyle = headStyle;
                                }
                            }

                            //sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                            sheet?.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                            //headerRow.Dispose();
                        }

                        #endregion


                        #region 列头及样式

                        {
                            if (sheet != null)
                            {
                                var headerRow = sheet.CreateRow(1) as XSSFRow;


                                var headStyle = workbook.CreateCellStyle() as XSSFCellStyle;
                                if (headStyle != null)
                                {
                                    headStyle.Alignment = HorizontalAlignment.Center;
                                    var font = workbook.CreateFont() as XSSFFont;
                                    if (font != null)
                                    {
                                        font.FontHeightInPoints = 10;
                                        font.Boldweight = 700;
                                        headStyle.SetFont(font);
                                    }


                                    foreach (DataColumn column in dtSource.Columns)
                                    {
                                        if (headerRow != null)
                                        {
                                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                                        }

                                        //设置列宽
                                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);

                                    }
                                }
                            }
                            //headerRow.Dispose();
                        }

                        #endregion

                        rowIndex = 2;
                    }

                    #endregion

                    #region 填充内容

                    if (sheet != null)
                    {
                        var dataRow = sheet.CreateRow(rowIndex);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            var newCell = dataRow.CreateCell(column.Ordinal);

                            var drValue = row[column].ToString();

                            switch (column.DataType.ToString())
                            {
                                case "System.String": //字符串类型
                                    newCell.SetCellValue(drValue);
                                    break;
                                case "System.DateTime": //日期类型
                                    DateTime dateV;
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);

                                    newCell.CellStyle = dateStyle; //格式化显示
                                    break;
                                case "System.Boolean": //布尔型
                                    bool boolV;
                                    bool.TryParse(drValue, out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16": //整型
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    int intV;
                                    int.TryParse(drValue, out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal": //浮点型
                                case "System.Double":
                                    double doubV;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull": //空值处理
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }

                        }
                    }

                    #endregion

                    rowIndex++;
                }
            }
            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        [Obsolete]
        public static void ExportDTtoExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (var ms = ExportToMemorySteram(dtSource, strHeaderText))
            {
                using (var fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    var data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        #endregion

        #region 从excel中将数据导出到datatable
        /// <summary>读取excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName)
        {
            IWorkbook hssfworkbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new XSSFWorkbook(file);
            }
            //var sheet = hssfworkbook.GetSheetAt(0) as XSSFSheet;
            var sheet = hssfworkbook.GetSheetAt(0);
            var dt = ImportDt(sheet, 0, true);
            return dt;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetName"></param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string sheetName, int headerRowIndex)
        {
            IWorkbook workbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            var sheet = workbook.GetSheet(sheetName) as XSSFSheet;
            var table = ImportDt(sheet, headerRowIndex, true);
            //ExcelFileStream.Close();
            //workbook = null;
            //sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetIndex"></param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int sheetIndex, int headerRowIndex)
        {
            IWorkbook workbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            var sheet = workbook.GetSheetAt(sheetIndex) as XSSFSheet;
            var table = ImportDt(sheet, headerRowIndex, true);
            //ExcelFileStream.Close();
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetName"></param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="needHeader"></param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string sheetName, int headerRowIndex, bool needHeader)
        {
            IWorkbook workbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            var sheet = workbook.GetSheet(sheetName) as XSSFSheet;
            var table = ImportDt(sheet, headerRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            //sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheetIndex"></param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="needHeader"></param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int sheetIndex, int headerRowIndex, bool needHeader)
        {
            IWorkbook workbook;
            using (var file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                workbook = new XSSFWorkbook(file);
            }
            var sheet = workbook.GetSheetAt(sheetIndex);
            DataTable table = ImportDt(sheet, headerRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            //sheet = null;
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="headerRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="needHeader"></param>
        /// <returns></returns>
        static DataTable ImportDt(ISheet sheet, int headerRowIndex, bool needHeader)
        {
            if (sheet == null) throw new ArgumentNullException("sheet");
            var table = new DataTable();
            try
            {
                IRow headerRow;
                var cellCount = 0;
                if (headerRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0) as XSSFRow;
                    if (headerRow != null)
                    {
                        cellCount = headerRow.LastCellNum;

                        for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                        {
                            var column = new DataColumn(Convert.ToString(i));
                            table.Columns.Add(column);
                        }
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(headerRowIndex) as XSSFRow;
                    if (headerRow != null)
                    {
                        cellCount = headerRow.LastCellNum;

                        for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                        {
                            if (i == 118)
                            {
                            }
                            if (headerRow.GetCell(i) == null)
                            {
                                if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                                {
                                    var column = new DataColumn(Convert.ToString("重复列名" + i));
                                    table.Columns.Add(column);
                                }
                                else
                                {
                                    var column = new DataColumn(Convert.ToString(i));
                                    table.Columns.Add(column);
                                }

                            }
                            else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                            {
                                var column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                var column = new DataColumn(headerRow.GetCell(i).ToString());
                                table.Columns.Add(column);
                            }
                        }
                    }
                }
                for (var i = (headerRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i) as XSSFRow;
                        }
                        else
                        {
                            row = sheet.GetRow(i) as XSSFRow;
                        }

                        var dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {
                            try
                            {
                                if (row.GetCell(j) == null) continue;
                                switch (row.GetCell(j).CellType)
                                {
                                    case CellType.String:
                                        var str = row.GetCell(j).StringCellValue;
                                        if (!string.IsNullOrEmpty(str))
                                        {
                                            dataRow[j] = str;
                                        }
                                        else
                                        {
                                            dataRow[j] = null;
                                        }
                                        break;
                                    case CellType.Numeric:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                        break;
                                    case CellType.Boolean:
                                        dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                        break;
                                    case CellType.Error:
                                        dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                        break;
                                    default:
                                        dataRow[j] = "";
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                NBCMSLoggerManager.Error(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        NBCMSLoggerManager.Error(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                NBCMSLoggerManager.Error(exception.ToString());
            }
            return table;
        }
        #endregion

        #region 更新excel中的数据
        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    NBCMSLoggerManager.Error(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, string[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        NBCMSLoggerManager.Error(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluid">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[] updateData, int coluid, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int i = 0; i < updateData.Length; i++)
            {
                try
                {
                    if (sheet1.GetRow(i + rowid) == null)
                    {
                        sheet1.CreateRow(i + rowid);
                    }
                    if (sheet1.GetRow(i + rowid).GetCell(coluid) == null)
                    {
                        sheet1.GetRow(i + rowid).CreateCell(coluid);
                    }

                    sheet1.GetRow(i + rowid).GetCell(coluid).SetCellValue(updateData[i]);
                }
                catch (Exception ex)
                {
                    NBCMSLoggerManager.Error(ex.ToString());
                    throw;
                }
            }
            try
            {
                readfile.Close();
                FileStream writefile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }

        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="updateData">需更新的数据</param>
        /// <param name="coluids">需更新的列号</param>
        /// <param name="rowid">需更新的开始行号</param>
        public static void UpdateExcel(string outputFile, string sheetname, double[][] updateData, int[] coluids, int rowid)
        {
            FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
            readfile.Close();
            ISheet sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < coluids.Length; j++)
            {
                for (int i = 0; i < updateData[j].Length; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i + rowid) == null)
                        {
                            sheet1.CreateRow(i + rowid);
                        }
                        if (sheet1.GetRow(i + rowid).GetCell(coluids[j]) == null)
                        {
                            sheet1.GetRow(i + rowid).CreateCell(coluids[j]);
                        }
                        sheet1.GetRow(i + rowid).GetCell(coluids[j]).SetCellValue(updateData[j][i]);
                    }
                    catch (Exception ex)
                    {
                        NBCMSLoggerManager.Error(ex.ToString());
                    }
                }
            }
            try
            {
                FileStream writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 更新Excel表格
        /// </summary>
        /// <param name="outputFile">需更新的excel表格路径</param>
        /// <param name="sheetname">sheet名</param>
        /// <param name="dt">需更新的数据</param>
        public static void ExportDTtoExcel(string outputFile, string sheetname, DataTable dt)
        {
            var readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

            var hssfworkbook = new XSSFWorkbook(readfile);
            readfile.Close();
            var sheet1 = hssfworkbook.GetSheet(sheetname);
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        if (sheet1.GetRow(i) == null)
                        {
                            sheet1.CreateRow(i);
                        }
                        if (sheet1.GetRow(i).GetCell(j) == null)
                        {
                            sheet1.GetRow(i).CreateCell(j);
                        }
                        sheet1.GetRow(i).GetCell(j).SetCellValue(dt.Rows[i][j].ToString());
                    }
                    catch (Exception ex)
                    {
                        NBCMSLoggerManager.Error(ex.ToString());
                    }
                }
            }
            try
            {
                var writefile = new FileStream(outputFile, FileMode.Create);
                hssfworkbook.Write(writefile);
                writefile.Close();
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }
        }

        #endregion

        public static int GetSheetNumber(string outputFile)
        {
            int number = 0;
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                number = hssfworkbook.NumberOfSheets;

            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }
            return number;
        }

        public static ArrayList GetSheetName(string outputFile)
        {
            ArrayList arrayList = new ArrayList();
            try
            {
                FileStream readfile = new FileStream(outputFile, FileMode.Open, FileAccess.Read);

                HSSFWorkbook hssfworkbook = new HSSFWorkbook(readfile);
                for (int i = 0; i < hssfworkbook.NumberOfSheets; i++)
                {
                    arrayList.Add(hssfworkbook.GetSheetName(i));
                }
            }
            catch (Exception ex)
            {
                NBCMSLoggerManager.Error(ex.ToString());
            }
            return arrayList;
        }
    }
}
