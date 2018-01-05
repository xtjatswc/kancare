using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FastQues.Models;

namespace FastQues.Controllers
{
    public class QuestionRecordDetailsController : Controller
    {
        private FastQuesContext db = new FastQuesContext();

        // GET: QuestionRecordDetails
        public ActionResult Index()
        {
            return View(db.QuestionRecordDetail.ToList());
        }

        // GET: QuestionRecordDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecordDetail questionRecordDetail = db.QuestionRecordDetail.Find(id);
            if (questionRecordDetail == null)
            {
                return HttpNotFound();
            }
            return View(questionRecordDetail);
        }

        // GET: QuestionRecordDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuestionRecordDetails/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,QuestionRecordID,QuestionnaireID,QuestionID,QuestionOptionID,OptionValue")] QuestionRecordDetail questionRecordDetail)
        {
            if (ModelState.IsValid)
            {
                db.QuestionRecordDetail.Add(questionRecordDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionRecordDetail);
        }

        // GET: QuestionRecordDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecordDetail questionRecordDetail = db.QuestionRecordDetail.Find(id);
            if (questionRecordDetail == null)
            {
                return HttpNotFound();
            }
            return View(questionRecordDetail);
        }

        // POST: QuestionRecordDetails/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,QuestionRecordID,QuestionnaireID,QuestionID,QuestionOptionID,OptionValue")] QuestionRecordDetail questionRecordDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionRecordDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionRecordDetail);
        }

        // GET: QuestionRecordDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionRecordDetail questionRecordDetail = db.QuestionRecordDetail.Find(id);
            if (questionRecordDetail == null)
            {
                return HttpNotFound();
            }
            return View(questionRecordDetail);
        }

        // POST: QuestionRecordDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionRecordDetail questionRecordDetail = db.QuestionRecordDetail.Find(id);
            db.QuestionRecordDetail.Remove(questionRecordDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
