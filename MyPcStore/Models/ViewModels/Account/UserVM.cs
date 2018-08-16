using MyPcStore.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyPcStore.Models.ViewModels.Account
{
    public class UserVM
    {
        public UserVM()
        {
        }
        public UserVM(UserDTO currentrow)
        {
            Id = currentrow.Id;
            FirstName = currentrow.FirstName;
            LastName = currentrow.LastName;
            EmailAddress = currentrow.EmailAddress;
            Username = currentrow.Username;
            Password = currentrow.Password;
        }
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}