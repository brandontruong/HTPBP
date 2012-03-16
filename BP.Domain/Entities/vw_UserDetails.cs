using System;
using System.ComponentModel.DataAnnotations;

namespace BP.Domain.Entities
{
    public class vw_UserDetails
    {
        [Key]
        public Guid UserId { get; set; } 
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OrganizationName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid OrganizationId { get; set; }
        public string RoleName { get; set; }
        public string Company { get; set; }
        public bool IsApproved { get; set; }
    }
}