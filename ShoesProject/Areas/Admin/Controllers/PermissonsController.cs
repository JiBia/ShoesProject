﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoesProjectModels.Model;
using ShoesProject.Areas.Admin.Models;

namespace ShoesProject.Areas.Admin.Controllers
{
    [AuthorizeBusiness]
    public class PermissonsController : Controller
    {
        private Shoes db = new Shoes();

        // GET: Admin/Permissons
        public ActionResult Index(string id)
        {
            var permissons = db.Permissons.Where(p => p.BusinessId == id);
            return View(permissons.ToList());
        }

        // GET: Admin/Permissons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisson permisson = db.Permissons.Find(id);
            if (permisson == null)
            {
                return HttpNotFound();
            }
            return View(permisson);
        }

        // GET: Admin/Permissons/Create
        public ActionResult Create()
        {
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName");
            return View();
        }

        // POST: Admin/Permissons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PermissonId,PermissonName,PermissonDescription,BusinessId")] Permisson permisson)
        {
            if (ModelState.IsValid)
            {
                db.Permissons.Add(permisson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", permisson.BusinessId);
            return View(permisson);
        }

        // GET: Admin/Permissons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisson permisson = db.Permissons.Find(id);
            if (permisson == null)
            {
                return HttpNotFound();
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", permisson.BusinessId);
            return View(permisson);
        }

        // POST: Admin/Permissons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PermissonId,PermissonName,PermissonDescription,BusinessId")] Permisson permisson)
        {
            if (ModelState.IsValid)
            {
                db.Entry(permisson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BusinessId = new SelectList(db.Businesses, "BusinessId", "BusinessName", permisson.BusinessId);
            return View(permisson);
        }

        // GET: Admin/Permissons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Permisson permisson = db.Permissons.Find(id);
            if (permisson == null)
            {
                return HttpNotFound();
            }
            return View(permisson);
        }

        // POST: Admin/Permissons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Permisson permisson = db.Permissons.Find(id);
            db.Permissons.Remove(permisson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //[HttpPost]
        //public ActionResult getUserPermissons(int userId)
        //{

        //}
        public JsonResult getUserPermissons(string id, int userId)
        {
            var grantedPermissonsList = (from a in db.Admins
                                        from p in a.Permissons
                                        where a.AdminId == userId && p.BusinessId == id
                                        select new PermissonViewModel { PermissonId = p.PermissonId, PermissonName = p.PermissonName, Description = p.PermissonDescription, isGranted = true }).ToList();
            var permissonsList = from p in db.Permissons
                                 where p.BusinessId == id
                                 select new PermissonViewModel { PermissonId = p.PermissonId, PermissonName = p.PermissonName, Description = p.PermissonDescription, isGranted = false };

            var grantedPermissonsIdList = grantedPermissonsList.Select(p => p.PermissonId);
            foreach(var item in permissonsList)
            {
                if (!grantedPermissonsIdList.Contains(item.PermissonId)) grantedPermissonsList.Add(item);
            }
            return Json(grantedPermissonsList.OrderBy(x => x.Description), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public string UpdateUserPermissonState(int id, int userId)
        {
            string msg = "";
            var currentPermisson = db.Permissons.SingleOrDefault(x => x.PermissonId == id);
            var currentAdmin = db.Admins.SingleOrDefault(x => x.AdminId == userId);
            if(currentPermisson == null || currentAdmin == null)
            {
                return "<div class='alert alert-danger'>400 Bad Request</div>";
            }
            if(currentAdmin.Permissons.Any(x => x.PermissonId == currentPermisson.PermissonId))
            {
                currentAdmin.Permissons.Remove(currentPermisson);
                msg = "<div class='alert alert-danger'>Permisson Deactivated!</div>";
            }
            else
            {
                currentAdmin.Permissons.Add(currentPermisson);
                msg = "<div class='alert alert-success'>Permisson Activated!</div>";
            }
            db.SaveChanges();
            return msg;
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
