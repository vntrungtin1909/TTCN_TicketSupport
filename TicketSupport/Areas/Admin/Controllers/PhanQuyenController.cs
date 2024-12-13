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
		public ActionResult Create(){
           
            ViewBag.QuyenList = new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen");
			ViewBag.ChucNangQuyen = GetChucNangQuyen();
			return View();
        }
		[AuthorizeRoles("PB-C")]
		[HttpPost]
        public ActionResult Create(tblphongban tblphongban, List<string> SelectedQuyen)
        {
			// Kiểm tra tính hợp lệ của Model
			if (ModelState.IsValid)
			{
				// Kiểm tra `ma_phong_ban` không được trống và không trùng
				if (db.tblphongbans.Any(p => p.ma_phong_ban == tblphongban.ma_phong_ban))
				{
					ViewBag.ChucNangQuyen = GetChucNangQuyen();
					ModelState.AddModelError("ma_phong_ban", "Mã phòng ban không được để bị trùng.");
				}

				// Kiểm tra `ten_phong_ban` không được trống và không trùng
				if (db.tblphongbans.Any(p => p.ten_phong_ban == tblphongban.ten_phong_ban))
				{
					ViewBag.ChucNangQuyen = GetChucNangQuyen();
					ModelState.AddModelError("ten_phong_ban", "Tên phòng ban không được để bị trùng.");
				}

				// Nếu không có lỗi, tiếp tục xử lý
				if (ModelState.IsValid)
				{
					// Gắn quyền nếu có
					if (SelectedQuyen != null && SelectedQuyen.Any())
					{
						tblphongban.tblquyens = db.tblquyens
							.Where(q => SelectedQuyen.Contains(q.ma_quyen))
							.ToList();
					}

					// Các giá trị mặc định
					tblphongban.trang_thai = true; // Mặc định kích hoạt
					tblphongban.ngay_tao = DateTime.Now;
					tblphongban.ngay_cap_nhat = DateTime.Now;

					// Lưu vào cơ sở dữ liệu
					db.tblphongbans.Add(tblphongban);
					db.SaveChanges();

					TempData["message"] = new XMessage("success", "Thêm mới phòng ban thành công");
					return RedirectToAction("Index");
				}
			}

			// Nếu không hợp lệ, trả về View với thông báo lỗi
			ViewBag.ChucNangQuyen = GetChucNangQuyen();
			ViewBag.SelectedQuyen = tblphongban.tblquyens.Select(q => q.ma_quyen).ToList();
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
            ViewBag.ChucNangQuyen = GetChucNangQuyen();
			ViewBag.SelectedQuyen = GetSelectedQuyen(tblphongban); 
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
		public List<ChucNangQuyenViewModel> GetChucNangQuyen()		
        {
			var chucnangs = db.tblchucnangs.ToList();
			var quyens = db.tblquyens.ToList();
			var chucNangQuyenList = chucnangs.Select(cn => new ChucNangQuyenViewModel
			{
				TenChucNang = cn.ten_chuc_nang,
				Quyens = quyens
				   .Where(q => q.ma_chuc_nang == cn.ma_chuc_nang)
				   .Select(q => new QuyenViewModel { MaQuyen = q.ma_quyen, TenQuyen = q.ten_quyen })
				   .ToList()
			}).ToList();
			return chucNangQuyenList;
		}
		public MultiSelectList GetQuyenList(List<string> selectedQuyen)
		{
			// Trả về MultiSelectList với quyền đã chọn (nếu có)
			return new MultiSelectList(db.tblquyens, "ma_quyen", "ten_quyen", selectedQuyen);
		}
		public List<string> GetSelectedQuyen(tblphongban tblphongban)
		{
			if (tblphongban?.tblquyens != null)
			{
				// Trả về danh sách các mã quyền đã được gán cho phòng ban
				return tblphongban.tblquyens.Select(q => q.ma_quyen).ToList();
			}
			return new List<string>(); // Trả về danh sách rỗng nếu không có quyền gán
		}


		[AuthorizeRoles("PB-E")]
		[HttpPost]
		public ActionResult Edit(tblphongban tblphongban, List<string> SelectedQuyen)
		{
			if (ModelState.IsValid)
			{
				// Lấy thông tin phòng ban từ cơ sở dữ liệu
				var phongban = db.tblphongbans.Include("tblquyens").FirstOrDefault(pb => pb.ma_phong_ban == tblphongban.ma_phong_ban);
				if (phongban == null)
				{
					return HttpNotFound();
				}
				// Kiểm tra xem tên phòng ban hoặc mã phòng ban có thay đổi không
				bool isMaPhongBanChanged = phongban.ma_phong_ban != tblphongban.ma_phong_ban;
				bool isTenPhongBanChanged = phongban.ten_phong_ban != tblphongban.ten_phong_ban;

				// Kiểm tra trùng tên phòng ban nếu có thay đổi
				if (isMaPhongBanChanged && db.tblphongbans.Any(pb => pb.ma_phong_ban == tblphongban.ma_phong_ban))
				{
					ViewBag.ChucNangQuyen = GetChucNangQuyen();
					ViewBag.QuyenList = GetQuyenList(SelectedQuyen); // Gọi hàm tạo MultiSelectList
					ViewBag.SelectedQuyen = SelectedQuyen ?? new List<string>(); // Giữ lại quyền đã chọn
					ModelState.AddModelError("ma_phong_ban", "Mã phòng ban đã tồn tại.");
				}

				// Kiểm tra trùng tên phòng ban nếu có thay đổi
				if (isTenPhongBanChanged && db.tblphongbans.Any(pb => pb.ten_phong_ban == tblphongban.ten_phong_ban))
				{
					ViewBag.ChucNangQuyen = GetChucNangQuyen();
					ViewBag.QuyenList = GetQuyenList(SelectedQuyen); // Gọi hàm tạo MultiSelectList
					ViewBag.SelectedQuyen = SelectedQuyen ?? new List<string>(); // Giữ lại quyền đã chọn
					ModelState.AddModelError("ten_phong_ban", "Tên phòng ban đã tồn tại.");
				}

				// Nếu có lỗi, trả về view cùng thông báo lỗi
				if (!ModelState.IsValid)
				{
					ViewBag.ChucNangQuyen = GetChucNangQuyen();
					ViewBag.QuyenList = GetQuyenList(SelectedQuyen); // Gọi hàm tạo MultiSelectList
					ViewBag.SelectedQuyen = SelectedQuyen ?? new List<string>(); // Giữ lại quyền đã chọn
					return View(tblphongban);
				}

				// Cập nhật thông tin phòng ban
				phongban.ten_phong_ban = tblphongban.ten_phong_ban;
				phongban.trang_thai = tblphongban.trang_thai;
				phongban.ngay_cap_nhat = DateTime.Now;

				// Cập nhật quyền
				phongban.tblquyens.Clear(); // Xóa tất cả quyền cũ
				if (SelectedQuyen != null && SelectedQuyen.Any())
				{
					phongban.tblquyens = db.tblquyens.Where(q => SelectedQuyen.Contains(q.ma_quyen)).ToList();
				}

				// Lưu thay đổi
				db.SaveChanges();
				TempData["message"] = new XMessage("success", "Cập nhật phòng ban thành công!");
				return RedirectToAction("Index");
			}

			// Nếu không hợp lệ, trả về View
			ViewBag.ChucNangQuyen = GetChucNangQuyen();
			ViewBag.QuyenList = GetQuyenList(SelectedQuyen); // Gọi hàm tạo MultiSelectList
			ViewBag.SelectedQuyen = SelectedQuyen ?? new List<string>(); // Giữ lại quyền đã chọn
			TempData["message"] = new XMessage("danger", "Cập nhật phòng ban thất bại!");
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
