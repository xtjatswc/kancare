using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Web;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Windows.Forms;

//将引用的DLL：Microsoft.Office.Interop.Excel;的嵌入互操作类型改为false,就可以了
namespace Utility
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
        /// DataTable直接导出Excel,此方法会把DataTable的数据用Excel打开,再自己手动去保存到确切的位置
        /// </summary>
        /// <param name="dt">要导出Excel的DataTable</param>
        /// <returns></returns>
        public bool DoExport(System.Data.DataTable dt)
        {
            Microsoft.Office.Interop.Excel.Application app = new ApplicationClass();
            if (app == null)
            {
                throw new Exception("Excel无法启动");
            }
            app.Visible = true;
            Workbooks wbs = app.Workbooks;
            Workbook wb = wbs.Add(Missing.Value);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            int cnt = dt.Rows.Count;
            int columncnt = dt.Columns.Count;

            // *****************获取数据********************
            object[,] objData = new Object[cnt + 1, columncnt];  // 创建缓存数据
            // 获取列标题
            for (int i = 0; i < columncnt; i++)
            {
                objData[0, i] = dt.Columns[i].ColumnName;
            }
            // 获取具体数据
            for (int i = 0; i < cnt; i++)
            {
                System.Data.DataRow dr = dt.Rows[i];
                for (int j = 0; j < columncnt; j++)
                {
                    objData[i + 1, j] = dr[j];
                }
            }

            //********************* 写入Excel******************
            Range r = ws.get_Range(app.Cells[1, 1], app.Cells[cnt + 1, columncnt]);
            r.NumberFormat = "@";
            //r = r.get_Resize(cnt+1, columncnt);
            r.Value2 = objData;
            r.EntireColumn.AutoFit();

            app = null;
            return true;
        }

        /// <summary>  
        /// DataGridView控件选中数据保存到Excel  
        /// </summary>  
        /// <param name="ExportDgv"></param>  
        /// <param name="DgvTitle"></param>  
        /// <returns></returns>  
        public bool CopyToExcel(DataGridView ExportDgv, string DgvTitle)
        {
            try
            {
                if (ExportDgv == null)
                {
                    return false;
                }

                if (ExportDgv.Columns.Count == 0 || ExportDgv.Rows.Count == 0)
                {
                    return false;
                }

                //Excel2003最大行是65535 ,最大列是255  
                //Excel2007最大行是1048576,最大列是16384  
                //if (ExportDgv.RowCount > 65536 || ExportDgv.ColumnCount > 256)  
                //{  
                //    return false;  
                //}  

                ExportDgv.Focus();

                //复制数据到Clipboard  
                int I = ExportDgv.GetCellCount(DataGridViewElementStates.Selected);
                if (I > 0)
                {
                    Clipboard.SetDataObject(ExportDgv.GetClipboardContent());
                }

                //创建Excel对象  
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    return false;
                }
                //创建Excel工作薄  
                Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
                //创建Excel工作表  
                Microsoft.Office.Interop.Excel.Worksheet xlSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets[1];//第1个工作表  
                //粘贴数据  
                xlSheet.get_Range("A1", System.Type.Missing).PasteSpecial(XlPasteType.xlPasteAll,
                    XlPasteSpecialOperation.xlPasteSpecialOperationNone,
                    System.Type.Missing, System.Type.Missing);

                //显示工作薄区间  
                xlApp.Visible = true;
                xlApp.Caption = DgvTitle;
                //设置文本表格的属性  
                xlApp.Cells.EntireColumn.AutoFit();//自动列宽  
                xlApp.Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                xlApp.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
                xlApp.ErrorCheckingOptions.BackgroundChecking = false;
                return true;

            }
            catch
            {
                return false;
            }
        }

    }
}