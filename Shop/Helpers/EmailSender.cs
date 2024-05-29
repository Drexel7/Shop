/**************************************************************************
 *                                                                        *
 *  File:        EmailSender.cs                                           *
 *  Copyright:   (c) 2024, Maftei Gutui Robert, Branici Radu              *
 *                                                                        *
 *  E-mail:      robert-mihaita.maftei-gutui@student.tuiasi.ro,           *
 *               radu.branici@student.tuiasi.ro                           *
 *  Description: Book Store Online Web Application                        *  
 *               This class handles the functionality for sending emails  *
 *               using the SendGrid API.                                  *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/

using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Shop.Helpers
{
    public class EmailSender
    {
        /// <summary>
        /// Sends an email using the SendGrid API.
        /// </summary>
        public static async Task SendEmail(string toEmail, string username, string subject, string message)
        {
            string apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("numaimerge@example.com", "Shopppal");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
