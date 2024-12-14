using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TicketSupport.Library;
using TicketSupport.Models;

namespace TicketSupport.Areas.Admin.Controllers
{
    public class YeuCauKTsController : Controller
    {
        private Tech_Support_TicketEntities db = new Tech_Support_TicketEntities();

        // GET: Admin/YeuCauKTs
        public ActionResult Index()
        {
            
            return View(db.tblyeucauhotrokythuats.ToList());
        }

        // GET: Admin/YeuCauKTs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                return HttpNotFound();
            }
            var tenPhongBan = db.tblphongbans.Where(p => p.ma_phong_ban == tblyeucauhotrokythuat.ma_phong_ban).Select(p => p.ten_phong_ban).FirstOrDefault();
            var tenKhachHang = db.tblkhachhangs.Where(p => p.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang).Select(p => p.ho_ten_khach_hang).FirstOrDefault();
            ViewBag.tenphong = tenPhongBan ?? "Không tồn tại";
            ViewBag.tenkhach = tenKhachHang ?? "Không tồn tại";
            return View(tblyeucauhotrokythuat);
        }

        // GET: Admin/YeuCauKTs/Create
        public ActionResult Create()
        {
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban");
            return View();
        }

        // POST: Admin/YeuCauKTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblyeucauhotrokythuat tblyeucauhotrokythuat)
        {
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.ma_khach_hang) || tblyeucauhotrokythuat.ma_khach_hang.Trim() == "")
            {
                ModelState.AddModelError("ma_khach_hang", "Mã khách hàng không được để trống");
            }
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.ten_yeu_cau) || tblyeucauhotrokythuat.ten_yeu_cau.Trim() == "")
            {
                ModelState.AddModelError("ten_yeu_ cau", "Tên yêu cầu không được để trống");
            }
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.noi_dung_yeu_cau) || tblyeucauhotrokythuat.noi_dung_yeu_cau.Trim() == "")
            {
                ModelState.AddModelError("noi_dung_yeu_ cau", "Nội dung yêu cầu không được để trống");
            }
            if (ModelState.IsValid)
            {
                tblyeucauhotrokythuat lastReq = db.tblyeucauhotrokythuats
                                    .OrderByDescending(kh => kh.ma_yeu_cau)
                                    .FirstOrDefault();
                if (lastReq == null)
                {
                    tblyeucauhotrokythuat.ma_yeu_cau = "YC00001";
                }
                else
                {
                    int num = int.Parse(lastReq.ma_yeu_cau.Substring(2));
                    tblyeucauhotrokythuat.ma_yeu_cau = "YC" + (num + 1).ToString("D5");
                }
                tblyeucauhotrokythuat.ngay_tiep_nhan = DateTime.Now;
                if (tblyeucauhotrokythuat.trang_thai_xu_ly == "Chưa xử lí")
                {
                    tblyeucauhotrokythuat.ngay_xu_ly = null;
                }
                else
                {
                    tblyeucauhotrokythuat.ngay_xu_ly = DateTime.Now;
                }
                
                    if (!db.tblkhachhangs.Any(u => u.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang))
                    {
                        TempData["message"] = new XMessage("danger", "Không tìm thấy mã khách hàng");
                        ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
                        return View(tblyeucauhotrokythuat);
                    }
                
                    db.tblyeucauhotrokythuats.Add(tblyeucauhotrokythuat);
                    db.SaveChanges();
                //CreateLSYC(tblyeucauhotrokythuat);
                    return RedirectToAction("Index");
            }
            
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }
 
        // GET: Admin/YeuCauKTs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                return HttpNotFound();
            }
           
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }

        // POST: Admin/YeuCauKTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( tblyeucauhotrokythuat tblyeucauhotrokythuat)
        {
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.ma_khach_hang) || tblyeucauhotrokythuat.ma_khach_hang.Trim() == "")
            {
                ModelState.AddModelError("ma_khach_hang", "Mã khách hàng không được để trống");
            }
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.ten_yeu_cau) || tblyeucauhotrokythuat.ten_yeu_cau.Trim() == "")
            {
                ModelState.AddModelError("ten_yeu_cau", "Tên yêu cầu không được để trống");
            }
            if (string.IsNullOrEmpty(tblyeucauhotrokythuat.noi_dung_yeu_cau) || tblyeucauhotrokythuat.noi_dung_yeu_cau.Trim() == "")
            {
                ModelState.AddModelError("noi_dung_yeu_cau", "Nội dung yêu cầu không được để trống");
            }
            if (ModelState.IsValid)
            {              
                tblyeucauhotrokythuat re = db.tblyeucauhotrokythuats.FirstOrDefault(d => d.ma_yeu_cau == tblyeucauhotrokythuat.ma_yeu_cau);
                if (re == null)
                {
                    TempData["message"] = new XMessage("danger", "Không tìm thấy mã yêu cầu");
                    ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
                    return View(tblyeucauhotrokythuat);
                }
                if (!db.tblkhachhangs.Any(u => u.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang))
                {
                    TempData["message"] = new XMessage("danger", "Không tìm thấy mã khách hàng");
                    ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
                    return View(tblyeucauhotrokythuat);
                }
                    tblyeucauhotrokythuat old = new tblyeucauhotrokythuat
                    {
                        ma_yeu_cau = re.ma_yeu_cau,
                        ten_yeu_cau = re.ten_yeu_cau,
                        ma_khach_hang = re.ma_khach_hang,
                        ma_phong_ban = re.ma_phong_ban,
                        trang_thai_xu_ly = re.trang_thai_xu_ly,
                        noi_dung_yeu_cau = re.noi_dung_yeu_cau,
                    };
                re.ten_yeu_cau = tblyeucauhotrokythuat.ten_yeu_cau;
                re.ma_khach_hang = tblyeucauhotrokythuat.ma_khach_hang;
                if (tblyeucauhotrokythuat.trang_thai_xu_ly != null)
                {
                    re.trang_thai_xu_ly = tblyeucauhotrokythuat.trang_thai_xu_ly;
                }
                if (re.ngay_xu_ly == null && tblyeucauhotrokythuat.trang_thai_xu_ly != "Chưa xử lí")
                {
                   re.ngay_xu_ly = DateTime.Now;      
                }

                re.ma_phong_ban = tblyeucauhotrokythuat.ma_phong_ban;
                re.noi_dung_yeu_cau = tblyeucauhotrokythuat.noi_dung_yeu_cau;
                

                db.Entry(re).State = EntityState.Modified;
                db.SaveChanges();
                CreateLSYC(old, re);
                    TempData["message"] = new XMessage("success", "Chỉnh sửa mẫu tin thành công");
                    return RedirectToAction("Index");

            }
           
            ViewBag.ma_phong_ban = new SelectList(db.tblphongbans, "ma_phong_ban", "ten_phong_ban", tblyeucauhotrokythuat.ma_phong_ban);
            return View(tblyeucauhotrokythuat);
        }
        public void CreateLSYC(tblyeucauhotrokythuat re, tblyeucauhotrokythuat moi)
        {
            tbllichsuthaydoiyeucau ls = new tbllichsuthaydoiyeucau();
            StringBuilder noiDungThayDoi = new StringBuilder();
            StringBuilder loaiThayDoi = new StringBuilder();
            if (moi.ten_yeu_cau != re.ten_yeu_cau)
            {
                noiDungThayDoi.AppendLine($"Thay đổi tên yêu cầu: {re.ten_yeu_cau} => {moi.ten_yeu_cau}");
                loaiThayDoi.Append("Thay đổi tên yêu cầu");
            }
            if (moi.ma_phong_ban != re.ma_phong_ban)
            {
                noiDungThayDoi.AppendLine($"Thay đổi mã phòng ban: {re.ma_phong_ban} => {moi.ma_phong_ban}");
                loaiThayDoi.Append("Thay đổi mã phòng ban");
            }
            if (moi.trang_thai_xu_ly != re.trang_thai_xu_ly)
            {
                noiDungThayDoi.AppendLine($"Thay đổi trạng thái xử lí: {re.trang_thai_xu_ly} => {moi.trang_thai_xu_ly}");
                loaiThayDoi.Append("Thay đổi trạng thái xử lí");
                ls.trang_thai_xu_ly = moi.trang_thai_xu_ly;
            }
            else
            {
                ls.trang_thai_xu_ly = re.trang_thai_xu_ly;
            }
            if (moi.noi_dung_yeu_cau != re.noi_dung_yeu_cau)
            {
                noiDungThayDoi.AppendLine($"Thay đổi nội dung: {re.noi_dung_yeu_cau} => {moi.noi_dung_yeu_cau}");
                loaiThayDoi.Append("Thay đổi nội dung yêu cầu");
            }

            if (noiDungThayDoi.Length == 0)
            {
                return; 
            }

            tbllichsuthaydoiyeucau lastHis = db.tbllichsuthaydoiyeucaus
                                            .OrderByDescending(kh => kh.ma_lich_su)
                                            .FirstOrDefault();

            if (lastHis == null || !lastHis.ma_lich_su.Contains(DateTime.Now.ToString("ddMMyyyy")))
            {
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-0001";
            }
            else
            {
                int num = int.Parse(lastHis.ma_lich_su.Substring(lastHis.ma_lich_su.LastIndexOf('-') + 1));
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-{(num + 1):D4}";
            }

            ls.ma_yeu_cau = re.ma_yeu_cau;
            ls.ngay_tao = DateTime.Now;
            ls.ma_nhan_vien = Session["UserId"].ToString();
            ls.noi_dung = noiDungThayDoi.ToString();
            ls.loai_thay_doi = loaiThayDoi.ToString();

            db.tbllichsuthaydoiyeucaus.Add(ls);
            db.SaveChanges();


        }
        // GET: Admin/YeuCauKTs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Mã không tồn tại");
                return RedirectToAction("Index");
            }
            tblyeucauhotrokythuat tblyeucauhotrokythuat = db.tblyeucauhotrokythuats.Find(id);
            if (tblyeucauhotrokythuat == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại yêu cầu");
                return RedirectToAction("Index");
            }
            var tenPhongBan = db.tblphongbans.Where(p => p.ma_phong_ban == tblyeucauhotrokythuat.ma_phong_ban).Select(p => p.ten_phong_ban).FirstOrDefault();
            var tenKhachHang = db.tblkhachhangs.Where(p => p.ma_khach_hang == tblyeucauhotrokythuat.ma_khach_hang).Select(p => p.ho_ten_khach_hang).FirstOrDefault();
            ViewBag.tenphong = tenPhongBan ?? "Không tồn tại";
            ViewBag.tenkhach = tenKhachHang ?? "Không tồn tại";
            return View(tblyeucauhotrokythuat);
        }

        // POST: Admin/YeuCauKTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            tblyeucauhotrokythuat re = db.tblyeucauhotrokythuats.Find(id);
            tbllichsuthaydoiyeucau lastHis = db.tbllichsuthaydoiyeucaus
                                            .OrderByDescending(kh => kh.ma_lich_su)
                                            .FirstOrDefault();
            tbllichsuthaydoiyeucau ls = new tbllichsuthaydoiyeucau();
            if (lastHis == null || !lastHis.ma_lich_su.Contains(DateTime.Now.ToString("ddMMyyyy")))
            {
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-0001";
            }
            else
            {
                int num = int.Parse(lastHis.ma_lich_su.Substring(lastHis.ma_lich_su.LastIndexOf('-') + 1));
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-{(num + 1):D4}";
            }

            ls.ma_yeu_cau = re.ma_yeu_cau;
            ls.ngay_tao = DateTime.Now;
            ls.ma_nhan_vien = Session["UserId"].ToString();
            ls.trang_thai_xu_ly = re.trang_thai_xu_ly;
            ls.loai_thay_doi = "Xóa yêu cầu hỗ trợ";
            ls.noi_dung = "Tên yêu cầu: " + re.ten_yeu_cau + " Mã phòng ban: " + re.ma_phong_ban + " Mã khách hàng: " + re.ma_khach_hang + " Nội dung: " + re.noi_dung_yeu_cau;

            db.tbllichsuthaydoiyeucaus.Add(ls);
            db.SaveChanges();

            db.tblyeucauhotrokythuats.Remove(re);
            db.SaveChanges();
            TempData["message"] = new XMessage("success", "xóa mẫu tin thành  công");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult trangThaiIndex(string id, string trangThaiXuLy)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(trangThaiXuLy))
            {
                TempData["message"] = new XMessage("danger", "Dữ liệu không hợp lệ");
                return RedirectToAction("Index");
            }

            var yeuCau = db.tblyeucauhotrokythuats.Find(id);
            if (yeuCau == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy yêu cầu");
                return RedirectToAction("Index");
            }
            
            if (yeuCau.trang_thai_xu_ly == null && trangThaiXuLy != "Chưa xử lí")
            {
                yeuCau.ngay_xu_ly = DateTime.Now;
            }
            tbllichsuthaydoiyeucau lastHis = db.tbllichsuthaydoiyeucaus
                                            .OrderByDescending(kh => kh.ma_lich_su)
                                            .FirstOrDefault();
            tbllichsuthaydoiyeucau ls = new tbllichsuthaydoiyeucau();
            if (lastHis == null || !lastHis.ma_lich_su.Contains(DateTime.Now.ToString("ddMMyyyy")))
            {
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-0001";
            }
            else
            {
                int num = int.Parse(lastHis.ma_lich_su.Substring(lastHis.ma_lich_su.LastIndexOf('-') + 1));
                ls.ma_lich_su = $"LS-{DateTime.Now:ddMMyyyy}-{(num + 1):D4}";
            }

            ls.ma_yeu_cau = yeuCau.ma_yeu_cau;
            ls.ngay_tao = DateTime.Now;
            ls.ma_nhan_vien = Session["UserId"].ToString();
            ls.trang_thai_xu_ly = trangThaiXuLy;
            ls.loai_thay_doi = "Thay đổi trạng thái xử lí";
            ls.noi_dung = "Trạng thái xử lí: " + yeuCau.trang_thai_xu_ly + " => " + trangThaiXuLy;
            yeuCau.trang_thai_xu_ly = trangThaiXuLy;
            db.tbllichsuthaydoiyeucaus.Add(ls);
            db.SaveChanges();
            db.Entry(yeuCau).State = EntityState.Modified;
            db.SaveChanges();

            TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");
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
