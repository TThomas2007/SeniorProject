using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestWebApplication.Models
{
    public class AvailabilityModel
    {
        public int AvailabilityID { get; set; }
        public int InstructorUserID { get; set; }
        public int UserTypeID { get; set; }
        [Display(Name = "Availability Date and Time")]
        [DisplayFormat(DataFormatString = "{0:MMM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime DateTime { get; set; }
    }
}