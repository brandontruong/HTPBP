using System;
using System.Web.Helpers;
using BP.Domain.Abstract;

namespace BP.Domain.Concrete
{
    public class EmailService: IEmailService
    {
        public bool SendEmail(string recepientEmail, string emailTitle, string emailContent, out string error)
        {
            //Send confirmation email
            try
            {
                error = string.Empty;
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.EnableSsl = true;
                WebMail.UserName = "admin@simpleit.somee.com";
                WebMail.Password = "Eoo62oo8";
                WebMail.From = "admin@simpleit.somee.com";

                WebMail.Send(recepientEmail, emailTitle,emailContent);

            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
                //@:<b>Sorry - we couldn't send the email to confirm your RSVP.</b> 
            }

            return true;
        }
    }
}
