using LaundryManagerAPIDomain.Services.EmailService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LaundryManagerAPIDomain.Contracts
{
    public interface IEmailSender
    {
        void SendEmail(Message message, bool IsHTML=false);
        Task SendEmailAsync(Message message, bool IsHTML = false);
    }
}
