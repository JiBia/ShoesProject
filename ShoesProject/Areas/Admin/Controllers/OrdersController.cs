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
    public class OrdersController : Controller
    {
        private Shoes db = new Shoes();

        // GET: Admin/Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.User).Include(o => o.Voucher);
            var enabledOrders = orders.Where(order => order.OrderStatus ?? false);
            return View(enabledOrders.ToList());
        }

        public ActionResult OrdersDetail(int? id)
        {
            return RedirectToAction("Index", "OrdersDetails", new { area = "Admin", id = id });
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            ViewBag.VoucherId = new SelectList(db.Vouchers, "VoucherId", "VoucherName");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderId,UserId,VoucherId,Address,PhoneNumber,ProgressStatus,OrderStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            ViewBag.VoucherId = new SelectList(db.Vouchers, "VoucherId", "VoucherName", order.VoucherId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            ViewBag.VoucherId = new SelectList(db.Vouchers, "VoucherId", "VoucherName", order.VoucherId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,UserId,VoucherId,Address,PhoneNumber,ProgressStatus,OrderStatus")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", order.UserId);
            ViewBag.VoucherId = new SelectList(db.Vouchers, "VoucherId", "VoucherName", order.VoucherId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            //db.Orders.Remove(order);
            order.OrderStatus = false;
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
