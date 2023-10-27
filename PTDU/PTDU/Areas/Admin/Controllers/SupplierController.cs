using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MyClass.DAO;
using MyClass.Model;
using PTDU.Library;

namespace PTDU.Areas.Admin.Controllers
{
    public class SupplierController : Controller
    {
        SuppliersDao suppliersDao = new SuppliersDao();

        // GET: Admin/Supplier
        public ActionResult Index()
        {
            return View(suppliersDao.getList("Index"));
        }

        // GET: Admin/Supplier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            return View(suppliers);
        }

        // GET: Admin/Supplier/Create
        public ActionResult Create()
        {
            ViewBag.ListOrder = new SelectList(suppliersDao.getList("Index"), "Order", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //xu li tu dong: Slug, CreateAt,, UpdateAt, Order
                suppliers.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                suppliers.CreateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: UpdateAt
                suppliers.UpdateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //Xu ly tu dong: Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img != null && img.ContentLength > 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + suppliers.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Img = imgName;
                        //upload hinh
                        string PathDir = "/Public/img/supplier/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh 

                suppliersDao.Insert(suppliers);
                TempData["message"] = new XMessage("success", "Tạo mới nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDao.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // GET: Admin/Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDao.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // POST: Admin/Supplier/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                //Xu ly tu dong: UpdateAt
                suppliers.UpdateAt = DateTime.Now;
                //Xu ly tu dong: Order
                if (suppliers.Order == null)
                {
                    suppliers.Order = 1;
                }
                else
                {
                    suppliers.Order += 1;
                }
                //Xu ly tu dong: Slug
                suppliers.Slug = XString.Str_Slug(suppliers.Name);
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "/Public/img/supplier/";
                if (img != null && img.ContentLength > 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = suppliers.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + suppliers.Id + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        suppliers.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                    if (suppliers.Img != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir), suppliers.Img);
                        System.IO.File.Delete(DelPath);
                    }
                }//ket thuc phan upload hinh anh     
                suppliersDao.Update(suppliers);
                TempData["message"] = new XMessage("success", "Cập nhật nhà cung cấp thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListOrder = new SelectList(suppliersDao.getList("Index"), "Order", "Name");
            return View(suppliers);
        }

        // GET: Admin/Supplier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại nhà cung cấp");
                return RedirectToAction("Index");
            }

            return View(suppliers);
        }

        // POST: Admin/Supplier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = suppliersDao.getRow(id);
            suppliersDao.Delete(suppliers);
            TempData["message"] = new XMessage("success", "Xóa nhà cung cấp thành công");
            return RedirectToAction("Index");
        }
        //mot so action moi: Status, Trash, DelTrash, Undo
        //copy 
        /// //////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////////////////
        /// //////////////////////////////////////////////////////////////
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                //chuyen doi trang thai cua Satus tu 1<->2
                suppliers.Status = (suppliers.Status == 1) ? 2 : 1;

                //cap nhat gia tri UpdateAt
                suppliers.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                suppliersDao.Update(suppliers);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");


                return RedirectToAction("Index");
            }

        }
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                // khong hien len trang index
                suppliers.Status = 0;

                //cap nhat gia tri UpdateAt
                suppliers.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                suppliersDao.Update(suppliers);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
                return RedirectToAction("Index");
            }

        }

        public ActionResult Trash()
        {
            return View(suppliersDao.getList("Trash"));
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Suppliers suppliers = suppliersDao.getRow(id);
            if (suppliers == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                suppliers.Status = 2;

                //cap nhat gia tri UpdateAt
                suppliers.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                suppliersDao.Update(suppliers);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Phục hồi sản phẩm thành công");
                return RedirectToAction("Index");
            }
        }
    }
}
