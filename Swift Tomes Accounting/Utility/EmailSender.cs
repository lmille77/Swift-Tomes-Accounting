using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;

namespace Swift_Tomes_Accounting.Utility
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailjetClient client = new MailjetClient("46d42e0937efeee5057258014a29729a", "14ec6ee4e3f0e599748cbdb666d3d40f")
            {
                
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.FromEmail, "swifttomes@gmail.com")
            .Property(Send.FromName, "Swift Tomes")
            .Property(Send.Subject, subject)            
            .Property(Send.HtmlPart, htmlMessage)
            .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email", email}
                 }
                });
            

               
            MailjetResponse response = await client.PostAsync(request);
        }
        
        public async Task SendEmailAsync(string email, string subject, string htmlMessage, List<string> attachments)
        {
            MailjetClient client = new MailjetClient("46d42e0937efeee5057258014a29729a", "14ec6ee4e3f0e599748cbdb666d3d40f")
            {

            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
            .Property(Send.InlineAttachments, attachments)
            .Property(Send.FromEmail, "swifttomes@gmail.com")
            .Property(Send.FromName, "Swift Tomes")
            .Property(Send.Subject, subject)
            .Property(Send.HtmlPart, htmlMessage)
            .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email", email}
                 }
                });



            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
