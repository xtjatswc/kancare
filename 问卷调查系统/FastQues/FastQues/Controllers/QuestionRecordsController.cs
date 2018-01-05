using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FastQues.Models;
using System.Transactions;
using FastQues.Common;

namespace FastQues.Controllers
{
    public class QuestionRecordsController : Controller
    {
        private FastQuesContext db = new FastQuesContext();

        // GET: QuestionRecords
        public ActionResult Index(int? id = 1)
        {
            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            return View(db.QuestionRecord.Where(o => o.QuestionnaireID == id).ToList());
        }

        // GET: QuestionRecords
        public ActionResult Index2(int? id = 1, string Province = null, string City = null)
        {
            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
  //          DataTable result = db.Database.SqlQueryForDataTatable(@"SELECT [ID]
  //    ,[QuestionnaireID]
  //    ,[RecordTime]
  //    ,[StatusFlags]
  //    ,[Province]
  //    ,[City]
  //FROM [FastQuesDB].[dbo].[QuestionRecord] where Province = '" + Province + "' and City = '" + City + "'");// db.("select * from QuestionRecord", null);
            return View(db.QuestionRecord.Where(o => o.QuestionnaireID == id && o.Province == Province && o.City == City).ToList());
        }

        // GET: QuestionRecords
        public ActionResult Province(int? id = 1)
        {
            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            DataTable result = db.Database.SqlQueryForDataTatable(@"select a.*,b.Code from 
(
select [Province], COUNT(*) [Cc] from QuestionRecord  where [QuestionnaireID] = " + id + @" group by [Province] 
) a left join [dbo].[ProvinceCode] b on a.[Province] = b.[Province]
where a.Province is not null
order by [Province]");
            return View(result.AsEnumerable());
        }

        // GET: QuestionRecords
        public ActionResult Home(int? id = 1, String ProvinceCode = null)
        {
            string where = "";
            if (ProvinceCode != null)
            {
                where = " and b.code = '" + ProvinceCode + "' ";
            }

            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            DataTable result = db.Database.SqlQueryForDataTatable(@"select a.* from
(
select [Province], [City], COUNT(*) [Cc] from QuestionRecord  where [QuestionnaireID] = " + id + @" group by [Province], [City] 
) a inner join [dbo].[ProvinceCode] b on a.[Province] = b.[Province]
where 1=1 " + where + @"
order by [Province], [City]");

            //按市、医院等级
            ViewBag.result2 = db.Database.SqlQueryForDataTatable(@"select a.* from
(
select [Province], [City],[HospitalLevel], COUNT(*) [Cc] from QuestionRecord  where [QuestionnaireID] = " + id + @" group by [Province], [City] ,[HospitalLevel]
) a inner join [dbo].[ProvinceCode] b on a.[Province] = b.[Province]
where 1=1 " + where + @"
order by [Province], [City],[HospitalLevel]");

            //按医院等级
            ViewBag.result3 = db.Database.SqlQueryForDataTatable(@"select a.* from
(
select [Province], [HospitalLevel], COUNT(*) [Cc] from QuestionRecord  where [QuestionnaireID] = " + id + @" group by [Province], [HospitalLevel] 
) a inner join [dbo].[ProvinceCode] b on a.[Province] = b.[Province]
where 1=1 " + where + @"
order by [Province],[HospitalLevel]");

            return View(result.AsEnumerable());
        }

        public ActionResult Excel(int? id = 1, string Province = null, string City = null)
        {
            string p = Province == null ? " e.[Province] is null " : " e.[Province] = '" + Province + "'" ;
            string c = City == null ? " e.[City] is null " : " e.[City] = '" + City + "'";

            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            DataTable result = db.Database.SqlQueryForDataTatable(@"select e.id, e.RecordTime,d.QuestionNo, d.QuestionName,c.OptionName,b.OptionValue from [dbo].[QuestionRecordDetail] b
inner join [Question] d  on d.ID = b.QuestionID
inner join [dbo].[QuestionRecord] e on e.id = b.QuestionRecordID 
inner join [dbo].[QuestionOption] c on b.QuestionOptionID = c.id
where d.QuestionnaireID = 1 and " + p + @" and " + c + @" 
order by e.id,d.SortNo,c.SortNo");
            string filename = "问卷导出";
            var ms = result.Export2Excel(filename);
            return File(ms, "application/vnd.ms-excel", filename + ".xls");
        }

        public ActionResult Excel2(int? id = 1, string ProvinceCode = null)
        {
            string p = ProvinceCode == null ? " " : " and f.[Code] = '" + ProvinceCode + "' ";

            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            DataTable result = db.Database.SqlQueryForDataTatable(@"select e.id, e.RecordTime,d.QuestionNo, d.QuestionName,c.OptionName,b.OptionValue from [dbo].[QuestionRecordDetail] b
inner join [Question] d  on d.ID = b.QuestionID
inner join [dbo].[QuestionRecord] e on e.id = b.QuestionRecordID 
inner join [dbo].[QuestionOption] c on b.QuestionOptionID = c.id
inner join [dbo].[ProvinceCode] f on f.Province = e.Province
where d.QuestionnaireID = 1 " + p + @"
order by e.id,d.SortNo,c.SortNo");
            string filename = "问卷导出";
            var ms = result.Export2Excel(filename);
            return File(ms, "application/vnd.ms-excel", filename + ".xls");
        }



        // GET: Questions
        public ActionResult Answer(int? id = 1)
        {
            ViewBag.StatusFlags = Guid.NewGuid();
            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == id).First();
            ViewBag.optionDict = db.QuestionOption.Where(o => o.QuestionnaireID == id).OrderBy(o => o.QuestionID).OrderBy(o => o.SortNo).GroupBy(o => o.QuestionID).ToDictionary(k => k.Key, v => v.ToList());

            return View(db.Question.Where(o => o.QuestionnaireID == id).ToList().OrderBy(o => o.SortNo));
        }

        [HttpPost]
        public ActionResult SaveQuestionRecord(QuestionRecord questionRecord, List<QuestionRecordDetail> lstQuestionRecordDetail)
        {

            using (TransactionScope scope = new TransactionScope())
            {
                var qr = db.QuestionRecord.SingleOrDefault(o => o.StatusFlags == questionRecord.StatusFlags);
                if (qr == null)
                {
                    //新增
                    questionRecord.RecordTime = DateTime.Now;
                    questionRecord = db.QuestionRecord.Add(questionRecord);
                    db.SaveChanges();

                }
                else
                {
                    //修改
                    qr.RecordTime = DateTime.Now;
                    db.SaveChanges();
                    questionRecord = qr;
                    //删除明细
                    var del = db.QuestionRecordDetail.Where(o => o.QuestionRecordID == questionRecord.ID);
                    db.QuestionRecordDetail.RemoveRange(del);
                    db.SaveChanges();
                }

                //保存明细
                if (lstQuestionRecordDetail != null)
                {
                    foreach (var detail in lstQuestionRecordDetail)
                    {
                        detail.QuestionRecordID = questionRecord.ID;
                    }
                    db.QuestionRecordDetail.AddRange(lstQuestionRecordDetail);
                    db.SaveChanges();
                }

                scope.Complete();
            }

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(questionRecord);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: QuestionRecords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecord questionRecord = db.QuestionRecord.Find(id);
            if (questionRecord == null)
            {
                return HttpNotFound();
            }
            ViewBag.questionnaire = db.Questionnaire.Where(o => o.ID == questionRecord.QuestionnaireID).First();

            ViewBag.optionDict = db.QuestionOption.Where(o => o.QuestionnaireID == questionRecord.QuestionnaireID).OrderBy(o => o.SortNo).GroupBy(o => o.ID).ToDictionary(k => k.Key, v => v.ToList());

            ViewBag.detailDict = db.QuestionRecordDetail.Where(o => o.QuestionRecordID == questionRecord.ID).GroupBy(o => o.QuestionID).ToDictionary(k => k.Key, v => v.ToList());

            ViewBag.questionList = db.Question.Where(o => o.QuestionnaireID == questionRecord.QuestionnaireID).OrderBy(o => o.SortNo).ToList();

            return View(questionRecord);
        }

        // GET: QuestionRecords/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuestionRecords/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuestionnaireID,RecordTime")] QuestionRecord questionRecord)
        {
            if (ModelState.IsValid)
            {
                db.QuestionRecord.Add(questionRecord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionRecord);
        }

        // GET: QuestionRecords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecord questionRecord = db.QuestionRecord.Find(id);
            if (questionRecord == null)
            {
                return HttpNotFound();
            }
            return View(questionRecord);
        }

        // POST: QuestionRecords/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuestionnaireID,RecordTime")] QuestionRecord questionRecord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionRecord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionRecord);
        }

        // GET: QuestionRecords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecord questionRecord = db.QuestionRecord.Find(id);
            if (questionRecord == null)
            {
                return HttpNotFound();
            }
            return View(questionRecord);
        }

        // POST: QuestionRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionRecord questionRecord = null;
            using (TransactionScope scope = new TransactionScope())
            {
                questionRecord = db.QuestionRecord.Find(id);
                db.QuestionRecord.Remove(questionRecord);
                db.SaveChanges();

                var options = db.QuestionRecordDetail.Where(o => o.QuestionRecordID == id);
                db.QuestionRecordDetail.RemoveRange(options);
                db.SaveChanges();

                scope.Complete();
            }

            return RedirectToAction("Index", new { id= questionRecord .QuestionnaireID});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
