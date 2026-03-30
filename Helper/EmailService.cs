using Boulevard.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using Boulevard.Models;
using System.Threading.Tasks;

namespace Boulevard.Helper
{
    public class EmailService
    {
        public async Task  ForgetPasswordEmail(Member model)
        {
            try
            {
                var httpContext = new BaseController();

                String body = FakeController.RenderViewToString("Email", "ForgetPassword", model);
                MailMessage mailCompany = new MailMessage();

                mailCompany.To.Add(model.Email);
              
                mailCompany.From = new MailAddress("partners@boulevardsuperapp.com", "Boulevard");


                mailCompany.Subject = "Member Forget Password";
                mailCompany.Body = body;
                mailCompany.IsBodyHtml = true;
                mailCompany.Priority = MailPriority.Normal;
                mailCompany.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                


                SmtpClient clientComapny = new SmtpClient();
                clientComapny.Host = "giowm1114.siteground.biz";
               

                clientComapny.Credentials = new NetworkCredential("partners@boulevardsuperapp.com", "partners@123");
                clientComapny.EnableSsl = true;
                clientComapny.Port = 587;
                clientComapny.Send(mailCompany);
            }
            catch (Exception ex)
            {


            }

        }



        public async Task SendRegistrationResponse(Member model)
        {
            try
            {
                var httpContext = new FakeController();
                String body = FakeController.RenderViewToString("Email", "RegistrationResponse", model);
                MailMessage mailCompany = new MailMessage();

                mailCompany.To.Add(model.Email);

                mailCompany.From = new MailAddress("partners@boulevardsuperapp.com", "Boulevard");


                mailCompany.Subject = "Customer Registration - boulvard";
                mailCompany.Body = body;
                mailCompany.IsBodyHtml = true;
                mailCompany.Priority = MailPriority.Normal;
                mailCompany.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                SmtpClient clientComapny = new SmtpClient();
                clientComapny.Host = "giowm1114.siteground.biz";
                clientComapny.Credentials = new NetworkCredential("partners@boulevardsuperapp.com", "partners@123");
                clientComapny.EnableSsl = true;
                clientComapny.Port = 587;
                clientComapny.Send(mailCompany);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task SendEnquiryResponse(CustomerEnquery model)
        {
            try
            {
                var httpContext = new FakeController();
                String body = FakeController.RenderViewToString("Email", "CustomerEnquiry", model);
                MailMessage mailCompany = new MailMessage();

                mailCompany.To.Add("partners@boulevardsuperapp.com");

                mailCompany.From = new MailAddress("partners@boulevardsuperapp.com", "Boulevard");


                mailCompany.Subject = "Customer Enquiry - boulvard";
                mailCompany.Body = body;
                mailCompany.IsBodyHtml = true;
                mailCompany.Priority = MailPriority.Normal;
                mailCompany.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

                SmtpClient clientComapny = new SmtpClient();
                clientComapny.Host = "giowm1114.siteground.biz";
                clientComapny.Credentials = new NetworkCredential("partners@boulevardsuperapp.com", "partners@123");
                clientComapny.EnableSsl = true;
                clientComapny.Port = 587;
                clientComapny.Send(mailCompany);
            }
            catch (Exception)
            {

                
            }
        }


        public async Task Sendemail()
        {
            try
            {
                var fromAddress = new MailAddress("partners@boulevardsuperapp.com", "Boulevard Partner");
                var toAddress = new MailAddress("nafiz@royex.net", "Recipient Name");
                const string fromPassword = "partners@123";
                const string subject = "Test Email";
                const string body = "Hello, this is a test email sent via SMTP in C#.";

                var smtp = new SmtpClient
                {
                    Host = "giowm1114.siteground.biz", // SiteGround SMTP server
                    Port = 587, // Use 465 for SSL, 587 for TLS
                    EnableSsl = true, // SSL required
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        
    }
    }
}