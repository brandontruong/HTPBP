using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }
    }
}
