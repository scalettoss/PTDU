using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
using MyClass.DAO;
using PTDU.Library;
using System.IO;

namespace PTDU.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductsDao productsDao = new ProductsDao();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDao suppliersDao = new SuppliersDao();

        // GET: Admin/Product
        public ActionResult Index()
        {
            return View(productsDao.getList("Index"));
        }

        // GET: Admin/Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }

            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListSupID = new SelectList(suppliersDao.getList("Index"), "Id", "Name");
            //dung de su dung droplist
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {
                products.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                products.UpdateAt = DateTime.Now;
                products.CreateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: UpdateAt
                products.UpdateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: Slug
                products.Slug = XString.Str_Slug(products.Name);
                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img != null && img.ContentLength > 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathDir = "/Public/img/product/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh 
                productsDao.Insert(products);
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListSupID = new SelectList(suppliersDao.getList("Index"), "Id", "Name");
            return View(products);
        }

        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListSupID = new SelectList(suppliersDao.getList("Index"), "Id", "Name");
            return View(products);
        }

        // POST: Admin/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {

                products.UpdateAt = DateTime.Now;
                //Xu ly tu dong: Order
                //Xu ly tu dong: Slug
                products.Slug = XString.Str_Slug(products.Name);
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/product";
                if (img.ContentLength != 0)
                {
                    //Xu ly cho muc xoa hinh anh
                    if (products.Img != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                        System.IO.File.Delete(DelPath);
                    }
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh
                productsDao.Update(products);
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // GET: Admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = productsDao.getRow(id);
            productsDao.Delete(products);
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }


        /// <summary>
        /// /////////////////////
        /// </summary>
        /// <param</param>
        /// <returns></returns>




        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                //chuyen doi trang thai cua Satus tu 1<->2
                products.Status = (products.Status == 1) ? 2 : 1;

                //cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                productsDao.Update(products);

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
            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                // khong hien len trang index
                products.Status = 0;

                //cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                productsDao.Update(products);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Xóa sản phẩm vào thùng rác thành công");
                return RedirectToAction("Index");
            }

        }

        public ActionResult Trash()
        {
            return View(productsDao.getList("Trash"));
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDao.getRow(id);
            if (products == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                products.Status = 2;

                //cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                productsDao.Update(products);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Phục hồi sản phẩm thành công");
                return RedirectToAction("Index");
            }
        }
    }
}
