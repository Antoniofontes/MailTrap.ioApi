using Microsoft.AspNetCore.Mvc;
using SendingEmailWithASPNETCore.Services;

namespace SendingEmailWithASPNETCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailAPIController : ControllerBase
    {
        private readonly IAPIMailService _apiMailService;

        public MailAPIController(IAPIMailService apiMailService)
        {
            _apiMailService = apiMailService;
        }

        [HttpPost]
        [Route("SendMailAsync")]
        public async Task<bool> SendMailAsync(MailData mailData)
        {
            return await _apiMailService.SendMailAsync(mailData);
        }

        [HttpPost]
        [Route("SendHTMLMailAsync")]
        public async Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData)
        {
            return await _apiMailService.SendHTMLMailAsync(htmlMailData);
        }

        [HttpPost]
        [Route("SendMailWithAttachmentAsync")]
        public async Task<bool> SendMailWithAttachmentAsync([FromForm] MailDataWithAttachment mailDataWithAttachment)
        {
            return await _apiMailService.SendMailWithAttachmentsAsync(mailDataWithAttachment);
        }

    }
}
