namespace SendingEmailWithASPNETCore.Services
{
    public interface IAPIMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
    }
}

