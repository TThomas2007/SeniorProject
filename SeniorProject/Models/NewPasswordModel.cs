using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApplication.Models
{
    public class NewPasswordModel
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string ResetCode { get; set; }

    }
}