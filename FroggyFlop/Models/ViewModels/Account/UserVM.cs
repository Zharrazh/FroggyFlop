using FroggyFlop.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FroggyFlop.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM() { }

        public UserVM(UserDTO dto)
        {
            Id = dto.Id;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Email = dto.Email;
            Username = dto.Username;
            Password = dto.Password;

        }
        public int Id { get; set; }
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}