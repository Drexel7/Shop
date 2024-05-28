using SendGrid;
using SendGrid.Helpers.Mail;

// functia care trebuia sa trimita email. Nu am reusit sa o fac sa functioneze
// <!-- Maftei Gutui Robert Mihaita-->
namespace Shop.Helpers
{
    public class EmailSender
    {
        public static async Task SendEmail(string toEmail,string username,string subject,string message)
        {
            string apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("numaimerge", "Shopppal");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
    

        }
    }
}
