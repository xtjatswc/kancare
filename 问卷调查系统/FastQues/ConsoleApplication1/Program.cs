using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string sql = @"--更新省、市
  update QuestionRecord
  set City = b.OptionValue
  from
  QuestionRecord e 
  inner join
  [dbo].[QuestionRecordDetail] b on e.id = b.QuestionRecordID 
inner join [Question] d  on d.ID = b.QuestionID
inner join [dbo].[QuestionOption] c on b.QuestionOptionID = c.id
where d.QuestionnaireID = 1 and d.QuestionName = '市' and City is null


  update QuestionRecord
  set Province = b.OptionValue
  from
  QuestionRecord e 
  inner join
  [dbo].[QuestionRecordDetail] b on e.id = b.QuestionRecordID 
inner join [Question] d  on d.ID = b.QuestionID
inner join [dbo].[QuestionOption] c on b.QuestionOptionID = c.id
where d.QuestionnaireID = 1 and d.QuestionName = '省'  and Province is null

update QuestionRecord set [Province] = b.[ProvinceReal]
from QuestionRecord a inner join [dbo].[Province] b
on a.[Province] = b.[ProvinceAlias]
where a.[Province] != b.[ProvinceReal]

update QuestionRecord set [City] = b.[CityReal]
from QuestionRecord a inner join [dbo].[City] b
on a.[City] = b.[CityAlias]
where a.[City] != b.[CityReal]

update QuestionRecord set [HospitalLevel] = b.OptionName
from [dbo].[QuestionRecord] a
inner join
(
select e.id, e.RecordTime,d.QuestionNo, d.QuestionName,c.OptionName,b.OptionValue from [dbo].[QuestionRecordDetail] b
inner join [Question] d  on d.ID = b.QuestionID
inner join [dbo].[QuestionRecord] e on e.id = b.QuestionRecordID 
inner join [dbo].[QuestionOption] c on b.QuestionOptionID = c.id
where d.QuestionnaireID = 1 and d.QuestionName = '贵院等级'
) b on a.id = b.id
where a.[HospitalLevel] is null

";


            try
            {
                using (SqlConnection sqlConn = new SqlConnection(@"data source=.\sqlexpress;initial catalog=FastQuesDB;user id=sa;password=sa;multipleactiveresultsets=True;application name=EntityFramework"))
                {
                    SqlCommand sqlCmd = new SqlCommand(sql, sqlConn);
                    sqlConn.Open();
                    int ret = sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    Console.WriteLine("执行成功：" + ret);
                    Thread.Sleep(3 * 1000);

                }

            }
            catch (Exception ex)
            {
                Thread.Sleep(10 * 1000);
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }

        }
    }
}
