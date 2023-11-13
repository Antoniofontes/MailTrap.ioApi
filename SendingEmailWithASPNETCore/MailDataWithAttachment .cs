namespace SendingEmailWithASPNETCore
{
    public class MailDataWithAttachment:MailData
    {
        public IFormFileCollection EmailAttachments { get; set; }
    }
}
