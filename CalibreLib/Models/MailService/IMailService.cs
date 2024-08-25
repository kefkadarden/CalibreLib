namespace CalibreLib.Models.MailService
{
    public interface IMailService
    {
        Task SendMailAsync(MailData mailData);
    }
}
