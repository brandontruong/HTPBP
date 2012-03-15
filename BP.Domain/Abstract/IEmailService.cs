using System;
using System.Collections.Generic;
using System.Web.Security;
using BP.Domain.Entities;

namespace BP.Domain.Abstract
{
    public interface IEmailService
    {
        bool SendEmail(string recepientEmail, string emailTitle, string emailContent, out string error);
        
    }
}
