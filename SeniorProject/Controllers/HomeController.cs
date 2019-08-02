using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWebApplication.Models;
using System.Web.Security;


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

            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }
        public ActionResult CreateUser()
        {
            

            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(UserModel user)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();


            context.Insert_User(user.UserLogin.Username, user.UserLogin.Password, user.UserLogin.Email);
            
            

            return RedirectToAction("Index");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel user)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();


            var obj = context.UserLogins.Where(x => x.Username.Equals(user.UserLogin.Username) && x.Password.Equals(user.UserLogin.Password)).FirstOrDefault();
            if(obj != null)
            {
                Session["UserID"] = obj.UserID.ToString();
                Session["UserName"] = obj.Username.ToString();
                return RedirectToAction("ProfilePage");
            }
            else
            {
                ViewBag.Message = "Username/Password combo incorrect. Please try again";
            }

            return View(user);


        }

        
        public ActionResult ProfilePage()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        
        public ActionResult Logout()
        {
            if (Session["UserName"] != null || Session["UserID"] != null)
            {
                //Session["UserName"] = null;
                //Session["UserID"] = null;
                Session.Abandon();

            }
            return RedirectToAction("Index");
        }
    }
}