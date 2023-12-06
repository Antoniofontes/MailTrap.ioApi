namespace SendingEmailWithASPNETCore.Services
{
    public interface IAPIMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData,string tokenTemplate);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
    }
}

