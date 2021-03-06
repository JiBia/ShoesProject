﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShoesProject.Areas.Admin.Models;
using ShoesProjectModels.Model;

namespace ShoesProject.Areas.Admin.Controllers
{
    [AuthorizeBusiness]
    public class ColorsController : Controller
    {
        private Shoes db = new Shoes();

        // GET: Admin/Colors
        public ActionResult Index()
        {
            var enabledColors = db.Colors.Where(c => c.ColorStatus ?? false);
            return View(enabledColors.ToList());
        }

        // GET: Admin/Colors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }

        // GET: Admin/Colors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Colors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ColorId,ColorValue,ColorCode")] Color color)
        {
            color.ColorStatus = true;
            if (ModelState.IsValid)
            {
                db.Colors.Add(color);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(color);
        }

        // GET: Admin/Colors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }

        // POST: Admin/Colors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ColorId,ColorValue,ColorCode,ColorStatus")] Color color)
        {
            if (ModelState.IsValid)
            {
                db.Entry(color).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(color);
        }

        // GET: Admin/Colors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Colors.Find(id);
            if (color == null)
            {
                return HttpNotFound();
            }
            return View(color);
        }

        // POST: Admin/Colors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Color color = db.Colors.Find(id);
            //db.Colors.Remove(color);
            color.ColorStatus = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult getProductColors(int ProductId)
        {           
            var productColors = (from p in db.Products
                                from c in p.Colors
                                where p.ProductId == ProductId && c.ColorStatus == true
                                select new ColorViewModel { ColorId = c.ColorId, ColorCode = c.ColorCode, ColorValue = c.ColorValue, isPresent = true }).ToList();
            var allColors = from c in db.Colors select new ColorViewModel { ColorId = c.ColorId, ColorCode = c.ColorCode, ColorValue = c.ColorValue, isPresent = false };
            var productColorsId = productColors.Select(x => x.ColorId);
            foreach (var item in allColors)
            {
                if (!productColorsId.Contains(item.ColorId)) productColors.Add(item);
            }
            return Json(productColors, JsonRequestBehavior.AllowGet);
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
