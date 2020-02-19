using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;

namespace ContosoUniversity.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Student
        public ActionResult Index(string sortOrder,string currentFilter, string searchString,int?page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";

           

            //查询功能
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s =>
                    s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
            }
            //排序功能
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.FirstMidName);
                    break;
            }

            var pageSize = 3;
            var pageNumber = (page ?? 1);
            

            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        //框架自动生成的模板
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }*/

        // POST: Student/Create
        /// <summary>
        /// 自动创建的表单，进行修改。
        /// 1.去掉了ID，因为这是自动生成 的。
        /// 2.出于防止攻击，设备TOKEN
        /// 3.Bind特性用于防止“过多发布”攻击。举例来说，假设Student实体中包含一个Secert字段，你不想让此属性由Web页面来进行更新，所以你没有在页面上放置Secert的相应输入框。但黑客可以通过工具强行附加Secert字段即相应值到表单中并发送给服务器端。在没有使用Bind的默认情况下，模型绑定器会自动遍历提交过来的所有表单值并尝试更新到实体中，所以Secert也会得到更新——使用黑客强行附加的值。安全的做法是使用Bind特性的Include参数，可以让你指定那些字段是由模型绑定器来进行更新的，也可以相反地使用Exclude来排除你不想让模型绑定器来进行更新的属性。我们推荐使用Include的理由是，如果对实体添加了新的属性，Exclude是不会自动更新的，新属性会默认被模型绑定器进行更新。
        /// 4.try-catch块是除了Bind特性外您对脚手架代码所做的唯一更改。如果在保存时有一个源于DataException的异常被引发，一个通用的错误消息被显示出来。由于DataException错误有时会由外部的应用程序引发，而不是程序编写的错误，所以建议用户进行再次尝试。此外，虽然该实例中没有实现，在生产环境下，所有的应用程序错误都应该被记录下来。
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")]
            Student student)
        {
            try
            {
                if (ModelState.IsValid)//服务器端验证
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(student);
        }


        // GET: Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: Student/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
                new string[] {"LastName", "FirstMidName", "EnrollmentDate"}))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(studentToUpdate);
        }


        // GET: Student/Delete/5
        /// <summary>
        /// 此代码接受一个可选择参数saveChangesError，指示该方法是否是由保存更改后出现了故障的的方法调用的。在HttpGet Delete方法不是由之前出现了错误的方法被调用的，该参数为false。当HttpPost Delete出现了错误，参数为true并且错误信息被传递给视图。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveChangesError"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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