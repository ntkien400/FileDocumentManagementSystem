﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FileDocument.Models.Entities
{
    public class User : IdentityUser
    {
        [Required]
        public string FristName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public bool Gender { get; set; }
        public bool IsTokenValid { get; set; } = true;
        public Address Address { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
    }
}
