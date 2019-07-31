using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWebApplication.Models;


namespace TestWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            TestDatabaseEntities context = new TestDatabaseEntities();


            var model = new User();
            model.FirstName = context.Users.Where(x => x.UserID == 1).FirstOrDefault().FirstName;
            model.LastName = context.Users.Where(x => x.UserID == 1).FirstOrDefault().LastName;
            model.Email = context.Users.Where(x => x.UserID == 1).FirstOrDefault().Email;
            ViewBag.Message = "This is a test.";

            return View(model);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CreateUser()
        {
            ViewBag.Message = "Create New Account.";

            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(UserModel user)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();


            context.Insert_User(user.UserLogin.Username, user.UserLogin.Password, user.UserLogin.Email);
            
            

            return RedirectToAction("Index");
        }
    }
}