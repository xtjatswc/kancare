using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Dapper;

namespace DataLinker.Excel
{
    public class ExcelDemo
    {
        public void Test()
        {


            StringBuilder sbr = new StringBuilder();
            string filePath = Global.AppDir + @"excel\test\user.xlsx";
            using (FileStream fs = File.OpenRead(filePath))   //打开myxls.xls文件
            {
                IWorkbook wk;
                if (Path.GetExtension(filePath) == ".xls")
                    wk = new HSSFWorkbook(fs);   //把xls文件中的数据写入wk中
                else
                    wk = new XSSFWorkbook(fs);

                for (int i = 0; i < wk.NumberOfSheets; i++)  //NumberOfSheets是myxls.xls中总共的表数
                {
                    ISheet sheet = wk.GetSheetAt(i);   //读取当前表数据
                    for (int j = 0; j <= sheet.LastRowNum; j++)  //LastRowNum 是当前表的总行数
                    {
                        IRow row = sheet.GetRow(j);  //读取当前行数据
                        if (row != null)
                        {
                            sbr.Append("-------------------------------------\r\n"); //读取行与行之间的提示界限
                            for (int k = 0; k <= row.LastCellNum; k++)  //LastCellNum 是当前行的总列数
                            {
                                ICell cell = row.GetCell(k);  //当前表格
                                if (cell != null)
                                {
                                    sbr.Append(cell.ToString());   //获取表格中的数据并转换为字符串类型
                                }
                            }
                        }
                    }
                }
            }
            sbr.ToString();
            using (StreamWriter wr = new StreamWriter(new FileStream(@"c:/myText.txt", FileMode.Append)))  //把读取xls文件的数据写入myText.txt文件中
            {
                wr.Write(sbr.ToString());
                wr.Flush();
            }
        }
    }
}
