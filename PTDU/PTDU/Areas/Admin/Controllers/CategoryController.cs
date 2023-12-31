﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using PTDU.Library;
namespace PTDU.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(categoriesDAO.getList("Index"));
        }

        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //CREATE
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories categories)
        {
            if (ModelState.IsValid)
            {

                //Xu ly tu dong: CreateAt
                categories.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                categories.UpdateAt = DateTime.Now;
                //Xu ly tu dong: ParentId
                if (categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }
                //Xu ly tu dong: Order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Xu ly tu dong: Slug
                categories.Slug = XString.Str_Slug(categories.Name);

                //Chen them dong cho DB

                categoriesDAO.Insert(categories);
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categories categories)
        {
            if (ModelState.IsValid)
            {
                // xu li tu dong slug
                categories.Slug = XString.Str_Slug(categories.Name);

                // xu li tu dong parent id
                if (categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }   
                // xu li tu dong order
                if (categories.Order == null)
                {
                    categories.Order = 1;
                }    
                else
                {
                    categories.Order += 1;
                }    
                //xu li tu dong update at
                categories.UpdateAt = DateTime.Now;

                //cap nhat mau tin
                categoriesDAO.Update(categories);
                TempData["message"] = new XMessage("success", "Chỉnh sửa sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //////////////////////////////////////////////////////////////////////////////////////
        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = new XMessage("danger", "Xóa sản phẩm thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Xóa sản phẩm thất bại");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categories categories = categoriesDAO.getRow(id);
            categoriesDAO.Delete(categories);
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger","Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                //chuyen doi trang thai cua Satus tu 1<->2
                categories.Status = (categories.Status == 1) ? 2 : 1;

                //cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                categoriesDAO.Update(categories);

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
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                // khong hien len trang index
                categories.Status = 0;

                //cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                categoriesDAO.Update(categories);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
                return RedirectToAction("Index");
            }

        }

        public ActionResult Trash()
        {
            return View(categoriesDAO.getList("Trash"));
        }

        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                TempData["message"] = new XMessage("danger", "Không tìm thấy sản phẩm");
                return RedirectToAction("Index");
            }
            else
            {
                //truy van id

                categories.Status = 2;

                //cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //cap nhat lai DB
                categoriesDAO.Update(categories);

                //thong bao cap nhat trang thai thanh cong
                TempData["message"] = new XMessage("success", "Phục hồi sản phẩm thành công");
                return RedirectToAction("Index");
            }

        }


    }
}
