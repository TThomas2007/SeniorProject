using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestWebApplication.Models
{
    public enum UserGroup
    {
        [Display(Name = "Student")]
        Student = 1,
        [Display(Name = "Employer(Interviewer)")]
        Employer = 2,
        [Display(Name = "Teacher(Interviewer)")]
        Teacher = 3
    }

    public enum UserType
    {
        [Display(Name = "Software Development")]
        Programming = 1,
        [Display(Name = "Cyber Security")]
        CyberSecurity= 2,
        [Display(Name = "Networking")]
        Networking = 3
    }
}