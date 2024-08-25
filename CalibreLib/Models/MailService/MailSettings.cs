namespace CalibreLib.Models.MailService
{
    public class MailSettings
    {
        public int Id { get; set; }
        public required string SMTP_HostName { get; set; }
        public int SMTP_Port { get; set; }
        public MailKit.Security.SecureSocketOptions SMTP_Encryption { get; set; }
        public required string SMTP_UserName { get; set; }
        public required string SMTP_Password { get; set; }
        public string FromEmail { get; set; } = "automailer <automailer@example.com>";
        public int? SMTP_AttachmentSize { get; set; } = 25;
    }
}
