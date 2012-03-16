using System;
using System.Web.Mvc;

namespace BP.Domain.Entities
{
    public class UserModel
    {
        [HiddenInput(DisplayValue = false)]
        public Guid UserId { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; }

        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public string Company { get; set; }

        public bool IsApproved { get; set; }
    }
}
