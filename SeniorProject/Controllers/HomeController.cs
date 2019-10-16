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
            var passwordHash = Crypto.Hash(user.Password);
            var obj = context.UserLogins.Where(x => x.Username.Equals(user.Username)).FirstOrDefault();
            if (obj != null)
            {
                if (string.Compare(user.Username, obj.Username) == 0)
                {
                    ViewBag.message = "Username already exists. Please try again.";
                    return RedirectToAction("CreateUser");
                }
                if (string.Compare(user.Email, obj.Email) == 0)
                {
                    ViewBag.message = "Email already exists. Please try again.";
                    return RedirectToAction("CreateUser");
                }
                else
                {
                    ViewBag.message = "Unknown error has occured.";
                        return RedirectToAction("Index");
                }
            }
            else
            {
                context.Insert_User(user.Username, passwordHash, user.Email, activationCode, (int)user.UserGroup);


                SendCodeEmail(user.Email, activationCode.ToString(), "verify");

                return RedirectToAction("AccountSuccess");
            }
            
        }

        [NonAction]
        public void SendCodeEmail(string email, string code, string action)
        {


            if (action == "verify")
            {
                var verifyUrl = "/Home/VerifyAccount?activationCode=" + code;
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
            else if (action == "reset")
            {
                var verifyUrl = "/Home/ResetPasswordCodeValidation?resetCode=" + code;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var fromEmail = new MailAddress("prepinseniorproject@gmail.com", "PrepIN Support");
                var toEmail = new MailAddress(email);
                var fromEmailPassword = "seniorproject20";
                string subject = "Reset Password";

                string body = "</br> </br> Here is the password reset email you requested. Click <a href='" + link + "'>here</a> to hange your password.";


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
        }
        [HttpGet]
        public ActionResult VerifyAccount(string activationCode)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();
            var v = context.UserLogins.Where(x => x.ActivationCode == new Guid(activationCode)).FirstOrDefault();
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
        public ActionResult Login(UserModel user, string ReturnUrl="")
        {
            TestDatabaseEntities context = new TestDatabaseEntities();


            var obj = context.UserLogins.Where(x => x.Username.Equals(user.Username)).FirstOrDefault();
            if (obj != null && obj.IsEmailConfirmed == true)
            {
                if (string.Compare(Crypto.Hash(user.Password), obj.Password) == 0) {
                    int timeout = user.RememberMe ? 525600 : 20;
                    var ticket = new FormsAuthenticationTicket(user.Username, user.RememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                       
                        return RedirectToAction("ProfilePage");
                    }
                }
            }
            else if(obj.IsEmailConfirmed == false)
            {
                ViewBag.Message = "You have not confirmed your email yet. Please check the email you signed up with and try again.";
            }
            else
            {
                ViewBag.Message = "Username/Password combo incorrect. Please try again";
            }

            return View(user);


        }

       [Authorize]
        public ActionResult ProfilePage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string email)
        {
            
            TestDatabaseEntities context = new TestDatabaseEntities();
            
            
            if(email != null)
            {
                string resetCode = Guid.NewGuid().ToString();
                context.AddPasswordResetCode(email, resetCode);
                SendCodeEmail(email, resetCode, "reset");
                ViewBag.Message = "Password reset email sent to " + email;
            }

            return View();
        }
        [HttpGet]
        public ActionResult ResetPasswordCodeValidation(string resetCode)
        {

            TestDatabaseEntities context = new TestDatabaseEntities();
            var match = context.UserLogins.Where(x => x.ResetPasswordCode == resetCode);
            if(match != null)
            {
                NewPasswordModel model = new NewPasswordModel();
                model.ResetCode = resetCode;
                return View(model);
            }
            else
            {
                ViewBag.message = "You have reached this page in error. Now being returned to login page";
                return RedirectToAction("Login");
            }
            
        }
        [HttpPost]
        public ActionResult ResetPasswordCodeValidation(NewPasswordModel model)
        {
            var message = "";
            TestDatabaseEntities context = new TestDatabaseEntities();
            if (model.ResetCode != null)
            {
                string newPassword = Crypto.Hash(model.NewPassword);
                context.ChangePassword(newPassword, model.ResetCode);
                message = "Password successfully changed";
                ViewBag.Message = message;
                return RedirectToAction("ProfilePage");
            }
            else
            {
                message = "Something went wrong. Redirecting to login page. Please contact support.";
                ViewBag.Message = message;
                return RedirectToAction("Login");
            }

        }
    }
}