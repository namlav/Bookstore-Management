using QL_Sach_Nhom.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QL_Sach_Nhom.Controllers
{
    public class HomeController : Controller
    {
        private QLNHASACHEntities db = new QLNHASACHEntities();

        // GET: Home
        public ActionResult Index()
        {
            var danhSachSach = db.Saches
                .Include(s => s.TacGia)
                .Include(s => s.TheLoai)
                .Include(s => s.NhaXuatBan)
                .ToList();
            return View(danhSachSach);
        }

        // GET: Home/Details/5
        public ActionResult Details(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Sach sach = db.Saches.Find(id);
            if (sach == null) return HttpNotFound();
            return View(sach);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG");
            ViewBag.MaTL = new SelectList(db.TheLoais, "MaTL", "TenTL");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB");

            var newId = db.Database.SqlQuery<string>("SELECT dbo.fn_TuDongTangMaSach()").FirstOrDefault();

            Sach sach = new Sach();
            sach.MaSach = newId;

            return View(sach);
        }

        // POST: Home/Create
        [HttpPost]
        public ActionResult Create(Sach sach)
        {
            if(ModelState.IsValid)
            {
                db.Saches.Add(sach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTL = new SelectList(db.TheLoais, "MaTL", "TenTL", sach.MaTL);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        // GET: Home/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Sach sach = db.Saches.Find(id);
            if (sach == null) return HttpNotFound();

            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTL = new SelectList(db.TheLoais, "MaTL", "TenTL", sach.MaTL);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        // POST: Home/Edit/5
        [HttpPost]
        public ActionResult Edit(Sach sach)
        {
            if(ModelState.IsValid)
            {
                db.Entry(sach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", sach.MaTG);
            ViewBag.MaTL = new SelectList(db.TheLoais, "MaTL", "TenTL", sach.MaTL);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach);
        }

        // GET: Home/Delete/5
        public ActionResult Delete(string id)
        {
            if(id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Sach sach = db.Saches.Find(id);
            if(sach == null) return HttpNotFound();


            return View(sach);
        }

        // POST: Home/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, Sach sach)
        {
            sach = db.Saches.Find(id);
            try
            {
                db.Saches.Remove(sach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể xóa sách này vì sách đã có trong Hóa đơn hoặc Phiếu nhập. Vui lòng xóa dữ liệu liên quan trước.");
                return View(sach);

            }
        }

        // GET: Home/TimKiemNangCao
        public ActionResult TimKiemNangCao(string TuKhoa, string MaTG, string MaTL, string MaNXB)
        {

            ViewBag.MaTG = new SelectList(db.TacGias, "MaTG", "TenTG", MaTG);
            ViewBag.MaTL = new SelectList(db.TheLoais, "MaTL", "TenTL", MaTL);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans, "MaNXB", "TenNXB", MaNXB);

            var ketQua = db.sp_TimKiemSachNangCao(TuKhoa, MaTG, MaTL, MaNXB).ToList();

            return View(ketQua);
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
