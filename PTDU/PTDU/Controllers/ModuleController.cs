using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using MyClass.DAO;
using MyClass.Model;

namespace PTDU.Controllers
{
    public class ModuleController : Controller
    {
        // GET: Module
        MenusDAO menusDAO = new MenusDAO();
        public ActionResult MainMenu()
        {
            List<Menus> list = menusDAO.getListByParentId(0);
            return View("MainMenu",list);
        }
    }
}