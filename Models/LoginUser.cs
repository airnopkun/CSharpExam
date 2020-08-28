using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Exam.Models
{
    public class LoginUser
    {
        [EmailAddress]
        [Required]
        public string Email {get; set;}
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}