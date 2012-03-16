using System;

namespace BP.Domain.Entities
{
    public class UserProfile
    {
        public Guid UserProfileId { get; set; } 
        public Guid UserId { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Phone { get; set; }
        public Guid OrganizationId { get; set; }
        public string Company { get; set; }
    }
}