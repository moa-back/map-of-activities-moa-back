﻿using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Net.Mail;

namespace MapOfActivitiesAPI.Services
{
    public class EmailService
    {
        public async Task<string> ReadHtmlFileAsync(string filePath, string text, string message)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string fileContent = await reader.ReadToEndAsync();

                    string modifiedContent = fileContent.Replace("YourUrlHere", message);

                    return modifiedContent.Replace("YourTextHere", text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}"); 
                return null; }
        }

        public async Task SendEmailAsync(string email, string subject, string text, string message)
        {
            string emailBody = await ReadHtmlFileAsync("Views\\EmailView.html", text, message);
            if (emailBody != null)
            {

                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("Map of Activities", "robotpro@ukr.net"));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = emailBody
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.CheckCertificateRevocation = false;
                    await client.ConnectAsync("smtp.ukr.net", 465, true);
                    await client.AuthenticateAsync("robotpro@ukr.net", "4a10xqygwrsXPBvd");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}