﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BP.ViewModels
{
   
    public class RegisterViewModel
    {
        [Display(Name = "Organization name")]
        public string OrganizationName { get; set; }

        [Required]
        [Display(Name = "Given name")]
        public string GivenName { get; set; }

        [Required]
        [Display(Name = "Family name")]
        public string FamilyName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Organization")]
        public Guid OrganizationId { get; set; }

        [HiddenInput]
        public Guid UserId { get; set; }

        [Display(Name = "Company")]
        public string Company { get; set; }
    }
}