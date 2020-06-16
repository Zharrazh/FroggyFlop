using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FroggyFlop.Models.ViewModels.Account
{
    public class LoginUserVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [DisplayName("Remember me")]
        public bool IsRemember { get; set; }
    }
}