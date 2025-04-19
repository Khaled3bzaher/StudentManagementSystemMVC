using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.Models
{
    //[Table("Users")]
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(14)]
        public string NationalId { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateRegistered { get; set; }= DateTime.Now;

        public bool IsActive { get; set; } = true;
        public string? ProfilePictureURL { get; set; }
    }
}
