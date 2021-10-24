using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIT5032_Ass.Models
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Please enter the receiver")]
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}