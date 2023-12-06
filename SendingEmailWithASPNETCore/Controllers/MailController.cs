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
        public async Task<bool> SendHTMLMailAsync(List<HTMLMailData> htmlMailDataList, [FromHeader] string tokenTemplate)
        {
            foreach (var htmlMailData in htmlMailDataList) {                
                
               var result = await _apiMailService.SendHTMLMailAsync(htmlMailData,tokenTemplate);

                if(!result)
                {
                     return false;
                }
            }
            return true;
        }

        [HttpPost]
        [Route("SendMailWithAttachmentAsync")]
        public async Task<bool> SendMailWithAttachmentAsync([FromForm] MailDataWithAttachment mailDataWithAttachment)
        {
            return await _apiMailService.SendMailWithAttachmentsAsync(mailDataWithAttachment);
        }

    }
}
