using System;

namespace BP.Domain.Entities
{
    public class UserPermission
    {
        public Guid UserPermissionId { get; set; }
        public Guid UserId { get; set; }
        public Guid StepId { get; set; }

    }

}
