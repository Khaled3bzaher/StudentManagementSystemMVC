using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementMVCProject.ExternalServices
{
    public class ConfirmationRegisterService : IEmailSender
    {
        private readonly IConfiguration configuration;

        public ConfirmationRegisterService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
                //var adminMail = configuration["EmailSettings:EmailSender"];
                //var adminPass = configuration["EmailSettings:EmailAppPassword"];
                //var SmtpServer = configuration["EmailSettings:SmtpServer"];
                //var SmtpPort = int.Parse(configuration["EmailSettings:SmtpPort"]);

                //var smtpClient = new SmtpClient(SmtpServer, SmtpPort)
                //{
                //    Credentials = new NetworkCredential(adminMail, adminPass),
                //    EnableSsl = true,
                //};

                //var MsgObj = new MailMessage()
                //{
                //    From = new MailAddress(adminMail),
                //    Subject = subject,
                //    Body = htmlMessage,
                //    IsBodyHtml = true,

                //};
                //MsgObj.To.Add(email);


                //smtpClient.Send(MsgObj);
                //Console.WriteLine($"Email Sent to {email}");
            
        }

        
    }
}
