using ShipmentInformation;

namespace SendingEmailWithASPNETCore.Services
{
    public interface IAPIMailService
    {
        Task<bool> SendMailAsync(MailData mailData);
        Task<bool> SendHTMLMailAsync(ClientGuideData Data,string tokenTemplate);
        Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment);
    }
}

