namespace CalibreLib.Models.MailService
{
    public class MailData
    {
        public string? EmailName { get; set; }
        public required string EmailAddress { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public Stream Attachment { get; set; } = null!;
        public string AttachmentName { get; set; } = "attachment";
        public bool IsBodyHtml { get; set; } = false;
    }
}
