using Org.BouncyCastle.Asn1.Pkcs;

namespace SendingEmailWithASPNETCore.Services
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
    }
}
