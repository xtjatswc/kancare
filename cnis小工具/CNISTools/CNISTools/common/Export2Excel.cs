using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CNISTools
{
    public class ExcelExport
    {
        private ExcelExport()
        { }
        private static ExcelExport _instance = null;

        public static ExcelExport Instance
        {
            get
            {
                if (_instance == null) _instance = new ExcelExport();
                return _instance;
            }
        }

        /// <summary>
        /// 类版本
        /// </summary>
        public string version
        {
            get { return "0.1"; }
        }
        readonly int EXCEL03_MaxRow = 65535;

        /// <summary>
        /// 将DataTable转换为excel2003格式。
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public void DataTable2Excel(System.Data.DataTable dt, string sheetName, string path)
        {

            IWorkbook book = new HSSFWorkbook();
            if (dt.Rows.Count < EXCEL03_MaxRow)
                DataWrite2Sheet(dt, 0, dt.Rows.Count - 1, book, sheetName);
            else
            {
                int page = dt.Rows.Count / EXCEL03_MaxRow;
                for (int i = 0; i < page; i++)
                {
                    int start = i * EXCEL03_MaxRow;
                    int end = (i * EXCEL03_MaxRow) + EXCEL03_MaxRow - 1;
                    DataWrite2Sheet(dt, start, end, book, sheetName + i.ToString());
                }
                int lastPageItemCount = dt.Rows.Count % EXCEL03_MaxRow;
                DataWrite2Sheet(dt, dt.Rows.Count - lastPageItemCount, lastPageItemCount, book, sheetName + page.ToString());
            }
            MemoryStream ms = new MemoryStream();
            book.Write(ms);


            byte[] data = ms.ToArray();
            if (!File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.CreateNew);
                fs.Write(data, 0, data.Length);
                fs.Close();

            }
        }
        private void DataWrite2Sheet(System.Data.DataTable dt, int startRow, int endRow, IWorkbook book, string sheetName)
        {
            ISheet sheet = book.CreateSheet(sheetName);
            IRow header = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                ICell cell = header.CreateCell(i);
                string val = dt.Columns[i].Caption ?? dt.Columns[i].ColumnName;
                cell.SetCellValue(val);
            }
            int rowIndex = 1;
            for (int i = startRow; i <= endRow; i++)
            {
                DataRow dtRow = dt.Rows[i];
                IRow excelRow = sheet.CreateRow(rowIndex++);
                for (int j = 0; j < dtRow.ItemArray.Length; j++)
                {
                    excelRow.CreateCell(j).SetCellValue(dtRow[j].ToString());
                }
            }

        }

        /// <summary>
        /// 获取excel内容
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(string filePath)
        {
            DataTable dt = new DataTable();
            using (FileStream fsRead = System.IO.File.OpenRead(filePath))
            {
                IWorkbook wk = null;
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fsRead);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fsRead);
                    }

                    //获取第一个sheet
                    ISheet sheet = wk.GetSheetAt(0);
                    //获取第一行
                    IRow headrow = sheet.GetRow(0);
                    //创建列
                    for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                    {
                        DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                        //DataColumn datacolum = new DataColumn("F" + (i + 1));
                        dt.Columns.Add(datacolum);
                    }
                    //读取每行,从第二行起
                    for (int r = 1; r <= sheet.LastRowNum; r++)
                    {
                        bool result = false;
                        DataRow dr = dt.NewRow();
                        //获取当前行
                        IRow row = sheet.GetRow(r);
                        //读取每列
                        for (int j = 0; j < row.Cells.Count; j++)
                        {
                            ICell cell = row.GetCell(j); //一个单元格
                            dr[j] = GetCellValue(cell); //获取单元格的值
                                                        //全为空则不取
                            if (dr[j].ToString() != "")
                            {
                                result = true;
                            }
                        }
                        if (result == true)
                        {
                            dt.Rows.Add(dr); //把每行追加到DataTable
                        }
                    }
                }

            }
            return dt;
        }

        //对单元格进行判断取值
        private static string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    return cell.ToString();//
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
    }
}