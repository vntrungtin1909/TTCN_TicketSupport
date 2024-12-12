using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using TicketSupport.Areas.Admin.Authorization;
using TicketSupport.Filters;
using TicketSupport.Library;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    [AuthorizePermissions("PB")]
    [MyAuthenFilter]
    public class PhanQuyenController : Controller
    {
        Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();
		[AuthorizeRoles("PB-R")]
		public ActionResult Index()
        {
            return View(db.tblphongbans.ToList());
        }
		[AuthorizeRoles("PB-R")]
		// GET: Details
		public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").Include("tblchucnang").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }
            return View(tblphongban);
        }
		[AuthorizeRoles("PB-C")]
		// GET: Create
		public ActionResult Create()
        {
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen");
            return View();
        }
		[AuthorizeRoles("PB-C")]
		[HttpPost]
        public ActionResult Create(tblphongban tblphongban, List<string> SelectedQuyen)
        {
            if (ModelState.IsValid)
            {
                tblphongban.tblquyens = db.tblquyens.Where(q => SelectedQuyen.Contains(q.ma_quyen)).ToList();
                tblphongban.trang_thai = true; // Mặc định kích hoạt
                db.tblphongbans.Add(tblphongban);
                db.SaveChanges();
                TempData["message"] = new XMessage("success", "Thêm mới phòng ban thành công");
                return RedirectToAction("Index");
            }
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", SelectedQuyen);
            TempData["message"] = new XMessage("danger", "Thêm mới phòng ban thất bại");
            return View(tblphongban);
        }
		[AuthorizeRoles("PB-E")]
        // GET: Edit
        //public ActionResult Edit(string id)
        //      {
        //          if (string.IsNullOrEmpty(id))
        //          {
        //              return HttpNotFound();
        //          }

        //          var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
        //          if (tblphongban == null)
        //          {
        //              return HttpNotFound();
        //          }
        //          ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", tblphongban.tblquyens.Select(q => q.ma_quyen));
        //          return View(tblphongban);
        //      }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách chức năng và quyền
            var chucnangs = db.tblchucnangs.ToList();
            var quyens = db.tblquyens.ToList();

            // Tạo danh sách chức năng và quyền theo cấu trúc
            var chucNangQuyenList = chucnangs.Select(cn => new ChucNangQuyenViewModel
            {
                TenChucNang = cn.ten_chuc_nang,
                Quyens = quyens
                    .Where(q => q.ma_chuc_nang == cn.ma_chuc_nang)
                    .Select(q => new QuyenViewModel { MaQuyen = q.ma_quyen, TenQuyen = q.ten_quyen })
                    .ToList()
            }).ToList();

            ViewBag.ChucNangQuyen = chucNangQuyenList;
            ViewBag.SelectedQuyen = tblphongban.tblquyens.Select(q => q.ma_quyen).ToList();

            return View(tblphongban);
        }

        // ViewModel để truyền dữ liệu chính xác
        public class ChucNangQuyenViewModel
        {
            public string TenChucNang { get; set; }
            public List<QuyenViewModel> Quyens { get; set; }
        }

        public class QuyenViewModel
        {
            public string MaQuyen { get; set; }
            public string TenQuyen { get; set; }
        }


        [AuthorizeRoles("PB-E")]
		[HttpPost]
        public ActionResult Edit(tblphongban tblphongban, List<string> SelectedQuyen)
        {
            if (ModelState.IsValid)
            {
                var phongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == tblphongban.ma_phong_ban);
                if (phongban == null)
                {
                    return HttpNotFound();
                }

                phongban.ten_phong_ban = tblphongban.ten_phong_ban;
                phongban.trang_thai = tblphongban.trang_thai;
                phongban.tblquyens.Clear();

                if (SelectedQuyen != null)
                {
                    phongban.tblquyens = db.tblquyens.Where(q => SelectedQuyen.Contains(q.ma_quyen)).ToList();
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", SelectedQuyen);


            return View(tblphongban);
        }
		[AuthorizeRoles("PB-D")]

		// GET: Delete
		public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }
            return View(tblphongban);
        }
		[AuthorizeRoles("PB-D")]
		[HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var tblphongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == id);
            if (tblphongban == null)
            {
                return HttpNotFound();
            }

            db.tblphongbans.Remove(tblphongban);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
		[AuthorizeRoles("PB-C", "PB-E", "PB-D")]
		// GET: Status
		public ActionResult Status(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }

            var tblphongban = db.tblphongbans.Find(id);
            if (tblphongban == null)
            {
                TempData["message"] = new XMessage("danger", "cap nhat trang thai that bai");
                return RedirectToAction("Index");
            }

            tblphongban.trang_thai = !tblphongban.trang_thai; // Đổi trạng thái
            db.Entry(tblphongban).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            TempData["message"] = new XMessage("success", "cap nhat trang thai thanh cong");
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
