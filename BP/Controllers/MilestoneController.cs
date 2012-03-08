using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BP.Domain.Concrete;
using BP.Domain.Entities;
using BP.Models;

namespace BP.Controllers
{
    public class MilestoneController : Controller
    {
        //
        // GET: /Milestone/

        public ActionResult Index()
        {
            using (var db = new BPContext())
            {
                return View(db.Milestones.ToList());
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Milestone newMilestone)
        {
            try
            {
                using (var db = new BPContext())
                {
                    db.Milestones.Add(newMilestone);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}
