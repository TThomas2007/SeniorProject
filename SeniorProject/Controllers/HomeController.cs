using System;
using TestWebApplication.Content;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWebApplication.Models;
using System.Web.Security;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;
using System.Net;

namespace TestWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AccountSuccess()
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
            
            var activationCode = Guid.NewGuid();
            var passwordHash = Crypto.Hash(user.UserLogin.Password);
            
            var userId = context.Insert_User(user.UserLogin.Username, passwordHash, user.UserLogin.Email, activationCode).FirstOrDefault();
            if (userId == -1)
            {

                ViewBag.Message = "Username is already taken, please enter a different username";
                return RedirectToAction("Login");
            }
            if (userId == -2)
            {

                ViewBag.Message = "Email is already taken, please enter a different email";
                return RedirectToAction("Login");
            }
            else
            {
                SendVerificationLinkEmail(user.UserLogin.Email, activationCode.ToString());
                
                return RedirectToAction("AccountSuccess");
            }
        }
        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode)
        {
            var verifyUrl = "/Home/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("prepinseniorproject@gmail.com", "PrepIN Support");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "seniorproject20";
            string subject = "Account has been created for you!";

            string body = "</br> </br> Your new account has been created. Click <a href='" + link + "'>here</a> to verify your account";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();
            var v = context.UserLogins.Where(x => x.ActivationCode == new Guid(id)).FirstOrDefault();
            if(v != null)
            {
                var userId = v.UserID;
                context.ConfirmEmail(userId);
            }
            return View();
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
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(UserLogin user)
        {
            var userID = user.UserID;
           
            TestDatabaseEntities context = new TestDatabaseEntities();
            var email = context.GetEmailFromUserID(userID).ToString();
            if(email != null)
            {
                string resetCode = Guid.NewGuid().ToString();
            }

            return View();
        }
    }
}