using System;
using TestWebApplication.Content;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestWebApplication.Models;
using System.Web.Security;
using System.Net.Mail;
using System.Net;

namespace TestWebApplication.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult LiveInterview()
        {
            return View();
        }
        public ActionResult AccountSuccess()
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
            var obj1 = context.UserLogins.Where(x => x.Username.Equals(user.Username)).FirstOrDefault();
            var obj2 = context.UserLogins.Where(x => x.Email.Equals(user.Email)).FirstOrDefault();
            if (obj1 != null)
            {
                if (string.Compare(user.Username, obj1.Username) == 0)
                {
                    ViewBag.message = "Username already exists. Please try again.";
                    return RedirectToAction("CreateUser");
                }
                else
                {
                    ViewBag.message = "Unknown error has occured.";
                    return RedirectToAction("Index");
                }
            }
            if (obj2 != null) {
                if (string.Compare(user.Email, obj2.Email) == 0)
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
                context.Insert_User(user.Username, passwordHash, user.Email, activationCode, (int)user.UserGroup, (int)user.UserType);


                SendVerificationCodeEmail(user.Email, activationCode.ToString());

                return RedirectToAction("AccountSuccess");
            }
            
        }

        [NonAction]
        public void SendVerificationCodeEmail(string email, string code)
        {
            
            var fromEmail = new MailAddress("prepinservice@outlook.com", "PrepIN Support");
            var toEmail = new MailAddress(email);
           string fromEmailPassword = "prepin2020";


            
                string verifyUrl = "/Home/VerifyAccount?activationCode=" + code;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                
                string subject = "Account has been created for you!";

                string body = "</br> </br> Your new account has been created. Click <a href='" + link + "'>here</a> to verify your account";


                var smtp = new SmtpClient
                {
                    Host = "smtp.office365.com",
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
        [NonAction]
        public void SendResetCodeEmail(string email, string code)
        {
           
            var fromEmail = new MailAddress("prepinservice@outlook.com", "PrepIN Support");
            var toEmail = new MailAddress(email);
            string fromEmailPassword = "prepin2020";

            string verifyUrl = "/Home/ResetPasswordCodeValidation?resetCode=" + code;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            string subject = "Reset Password";

            string body = "</br> </br> Here is the password reset email you requested. Click <a href='" + link + "'>here</a> to hange your password.";


            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
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
        public void SendConfirmAptEmail(string email, string code, UserModel user)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();
            string username = context.UserLogins.Where(x => x.UserID == user.UserID).FirstOrDefault().Username;
            var fromEmail = new MailAddress("prepinservice@outlook.com", "PrepIN Support");
            var toEmail = new MailAddress(email);
            string fromEmailPassword = "prepin2020";

            string acceptUrl = "/Home/ConfirmApt?confirmCode=" + code;
            string denyUrl = "/Home/DenyApt?denyCode=" + code;
            var acceptLink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, acceptUrl);
            var denyLink = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, denyUrl);
  
            string subject = "Confirm Appointment";

            string body = "</br> </br> You have a new appointment set up with "+ username + ". Click <a href='" + acceptLink + "'>here</a> to accept this appointment. Click <a href='" + denyLink + "'>here</a> to deny this appointment.";


            var smtp = new SmtpClient
            {
                Host = "smtp.office365.com",
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
            var pass = Crypto.Hash(user.Password);

            if (obj != null && obj.IsEmailConfirmed == true)
            {
                if (string.Compare(pass, obj.Password) == 0) {
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
                else
                {
                    ViewBag.Message = "Username/Password combo incorrect. Please try again";
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
            TestDatabaseEntities context = new TestDatabaseEntities();
            UserLogin user = context.UserLogins.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            
            if (User.Identity.IsAuthenticated)
            {

                UserModel model = new UserModel();
                model.UserID = user.UserID;
                model.UserGroupID = user.UserGroupID;
                model.UserTypeID = user.UserTypeID;
                model.HasAppointment = user.HasAppointment;
                
                if(user.UserGroupID != 1)
                {

                    model.AvailList = context.Availabilities.Where(x => x.InstructorUserID == user.UserID).ToList();
                }
                if (user.HasAppointment == true)
                {

                    model.Appointments = context.Appointments.Where(x => x.StudentUserID == user.UserID).ToList();
                    
                    if( model.UserGroupID == 1)
                    {
                        int oppositeId = model.Appointments.FirstOrDefault().InstructorUserID;
                        model.AppointmentTime = context.GetStudentAppointmentTime(user.UserID).FirstOrDefault();
                        model.WhoIsMyAptWith = context.GetUserNameFromID(oppositeId).ToString();
                    }

                    if (model.UserGroupID != 1)
                    {
                        int oppositeId = model.Appointments.FirstOrDefault().StudentUserID;
                        model.AppointmentTime = context.GetInstructorAppointmentTime(user.UserID).FirstOrDefault();
                        model.WhoIsMyAptWith = context.GetUserNameFromID(oppositeId).ToString();
                    }
                }
                if (user.HasAppointment == false && user.UserGroupID == 1)
                {
                    model.MatchedAvails = context.Availabilities.Where(x => x.UserTypeID == user.UserTypeID).ToList();
                }
                return View(model);
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
       
        public ActionResult Availability()
        {
            if (User.Identity.IsAuthenticated)
            {
                TestDatabaseEntities context = new TestDatabaseEntities();
                UserLogin user = context.UserLogins.Where(x => x.Username == User.Identity.Name).FirstOrDefault();

                AvailabilityModel model = new AvailabilityModel();
                model.InstructorUserID = user.UserID;
                model.UserTypeID = user.UserTypeID;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetAvailability(AvailabilityModel user)
        {

            TestDatabaseEntities context = new TestDatabaseEntities();
            context.InsertAvailablity(user.InstructorUserID, user.UserTypeID, user.DateTime);
            ViewBag.message = "You have set your availabilty successfully. You can now add more times if you wish. " +
                "Please return to your profile page to view availability times.";
           
            return RedirectToAction("Availability");
        }

        [HttpGet]
        public ActionResult RemoveAvail(int id)
        {

            TestDatabaseEntities context = new TestDatabaseEntities();
            context.DeleteAvail(id);
            ViewBag.Message = "You have successfully deleted availability time.";

            return RedirectToAction("ProfilePage");
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
                SendResetCodeEmail(email, resetCode);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PickAppointment(UserModel user)
        {

            TestDatabaseEntities context = new TestDatabaseEntities();
            if (user != null)
            {
                if (context.Appointments.Where(x => x.StudentUserID == user.UserID).FirstOrDefault() == null)
                {
                    var selection = context.Availabilities.Where(x => x.AvailabilityID == user.SelectedAvailId).FirstOrDefault();
                    var instructor = context.UserLogins.Where(x => x.UserID == selection.InstructorUserID).FirstOrDefault();
                    var confirmCode = Guid.NewGuid();
                    context.InsertAppointment(user.UserID, selection.InstructorUserID, selection.DateTime, confirmCode, selection.AvailabilityID);

                    SendConfirmAptEmail(instructor.Email, confirmCode.ToString(), user);
                    ViewBag.Message = "You have successfully asked for an meeting with " + instructor.Username.ToString() + ". We will notify you when they have confirmed the appointment.";
                    RedirectToAction("ProfilePage");
                }
                else
                {
                    ViewBag.Message = "You have already signed up for an appointment.";
                    RedirectToAction("ProfilePage");
                }
            }


            return RedirectToAction("ProfilePage");
        }
        [HttpGet]
        public ActionResult ConfirmApt(string confirmCode)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();
            var v = context.Appointments.Where(x => x.confirmCode == new Guid(confirmCode)).FirstOrDefault();
            var aptUser = context.UserLogins.Where(x => x.UserID == v.StudentUserID).FirstOrDefault();
            var instructor = context.UserLogins.Where(x => x.UserID == v.InstructorUserID).FirstOrDefault();
            var availId = v.OriginalAvailID;
            if (v != null)
            {
                context.ConfirmApt(v.AppointmentID, aptUser.UserID, instructor.UserID);

                
                var fromEmail = new MailAddress("prepinseniorproject@gmail.com", "PrepIN Support");
                var toEmail = new MailAddress(aptUser.Email);
                var fromEmailPassword = "seniorproject20";
                string subject = "Confirmation of Appointment";

                string body = "</br> </br> Your appointment with " + instructor.Username.ToString() + " for " + v.Time.ToLongDateString() + " at " +v.Time.ToShortTimeString() + " has been confirmed!";


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
            


            ViewBag.Message = "You have successfully confirmed your appointment with " + aptUser.Username + ".";
                context.DeleteAvail(availId);
                return RedirectToAction("ProfilePage");
            }
            else
            {
                ViewBag.Message = "An error has occured. Redirecting you to profile page or login page.";
                return RedirectToAction("ProfilePage");
            }
            
        }
        [HttpGet]
        public ActionResult DenyApt(string denyCode)
        {
            TestDatabaseEntities context = new TestDatabaseEntities();
            var v = context.Appointments.Where(x => x.confirmCode == new Guid(denyCode)).FirstOrDefault();
            var aptUser = context.UserLogins.Where(x => x.UserID == v.StudentUserID).FirstOrDefault();
            var instructor = context.UserLogins.Where(x => x.UserID == v.InstructorUserID).FirstOrDefault();
            if (v != null)
            {
                context.DeleteApt(v.AppointmentID, aptUser.UserID, instructor.UserID);


                var fromEmail = new MailAddress("prepinservice@outlook.com", "PrepIN Support");
                var toEmail = new MailAddress(aptUser.Email);
                var fromEmailPassword = "prepin2020";
                string subject = "Denial of Appointment";

                string body = "</br> </br> Your appointment with " + instructor.Username.ToString() + " for " + v.Time.ToLongDateString() + " at " + v.Time.ToShortTimeString() + " was denyed. Please choose another time.";


                var smtp = new SmtpClient
                {
                    Host = "smtp.outlook.com",
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



                ViewBag.Message = "You have successfully denied the appointment with " + aptUser.Username + ".";
                return RedirectToAction("ProfilePage");
            }
            else
            {
                ViewBag.Message = "An error has occured. Redirecting you to profile page or login page.";
                return RedirectToAction("ProfilePage");
            }

        }
        [HttpGet]
        public ActionResult CancelApt(int id, int studentUserId, int interviewerId)
        {

            TestDatabaseEntities context = new TestDatabaseEntities();
            
            context.DeleteApt(id, studentUserId, interviewerId);

            ViewBag.Message = "You have successfully canceled your appointment.";

            return RedirectToAction("ProfilePage");
        }

    }
}