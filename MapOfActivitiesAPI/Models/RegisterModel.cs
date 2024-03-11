﻿using System.ComponentModel.DataAnnotations;

namespace MapOfActivitiesAPI.Models
{
    public class RegisterModel
    {


        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
    }
}