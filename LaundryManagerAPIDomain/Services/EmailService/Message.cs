using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaundryManagerAPIDomain.Services.EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }
        public Message(IEnumerable<string> receivers, string subject, string content,IFormFileCollection attachment=null )
        {
            To = new List<MailboxAddress>();
            To.AddRange(receivers.Select(receiver => new MailboxAddress(receiver)));
            Subject = subject;
            Content = content;
            Attachments = attachment;
        }
    }
}
