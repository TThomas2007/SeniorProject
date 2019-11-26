using System;
using System.Collections.Generic;
using System.Web;

namespace TestWebApplication.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LoginDate { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string ResetPasswordCode { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
        public int UserGroupID { get; set; }
        public int UserTypeID { get; set; }
        public UserGroup UserGroup { get; set; }
        public UserType UserType { get; set; }
        public string confirmPassword { get; set; }
        public bool RememberMe { get; set; }
        public List<Availability> AvailList { get; set; }
    }
}
