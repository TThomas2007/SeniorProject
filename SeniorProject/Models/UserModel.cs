using System;
using System.Web;

namespace TestWebApplication.Models
{
    public class UserModel
    {
        public UserLogin UserLogin { get; set; }
        public string confirmPassword { get; set; }
        public bool RememberMe { get; set; }
    }
}
