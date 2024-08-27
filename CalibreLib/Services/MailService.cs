using MimeKit;
using MailKit.Net.Smtp;
using CalibreLib.Models.MailService;
using MailKit;
using CalibreLib.Data;

namespace CalibreLib.Services
{
    public class MailService : Models.MailService.IMailService
    {
        private readonly MailSettings _mailsettings;

        public MailService(CalibreLibContext context)
        {
            _mailsettings = context.MailSettings.FirstOrDefault();
        }

        public async Task SendMailAsync(MailData mailData)
        {
            if (_mailsettings == null)
                throw new Exception("Mail Settings null");

            MimeMessage message = new MimeMessage();
            MailboxAddress fromAddress = new MailboxAddress(_mailsettings.FromEmail, _mailsettings.SMTP_UserName);
            MailboxAddress toAddress = new MailboxAddress(mailData.EmailAddress, mailData.EmailAddress);
            message.From.Add(fromAddress);
            message.To.Add(toAddress);
            message.Subject = mailData.Subject;
            BodyBuilder bodyBuilder = new BodyBuilder();
            if (mailData.IsBodyHtml)
                bodyBuilder.HtmlBody = mailData.Body;
            else
                bodyBuilder.TextBody = mailData.Body;

            if (mailData.Attachment != null)
            {
                var attachment = new MimePart()
                {
                    Content = new MimeContent(mailData.Attachment, ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = mailData.AttachmentName
                };

                bodyBuilder.Attachments.Add(attachment);
            }
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient(new ProtocolLogger("./Logs/smtp.log")))
            {
                await client.ConnectAsync(_mailsettings.SMTP_HostName, _mailsettings.SMTP_Port, _mailsettings.SMTP_Encryption);
                await client.AuthenticateAsync(_mailsettings.SMTP_UserName, _mailsettings.SMTP_Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }
    }
}
