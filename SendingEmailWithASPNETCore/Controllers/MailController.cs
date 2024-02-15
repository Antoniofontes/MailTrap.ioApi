using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using SendingEmailWithASPNETCore.Services;
using ShipmentInformation;
using System.Reflection.Emit;
using System.Xml.Linq;
using System;

namespace SendingEmailWithASPNETCore.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailAPIController : Controller
    {
        private readonly IAPIMailService _apiMailService;
        public MailAPIController(IAPIMailService apiMailService)
        {
            _apiMailService = apiMailService;
        }

        [HttpPost]
        [Route("SendMailAsync")]
        public async Task<IActionResult> SendMailAsync(MailData mailData)
        {
            bool result =await _apiMailService.SendMailAsync(mailData);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("SendHTMLMailAsync")]
        public async Task<IActionResult> SendHTMLMailAsync([FromBody] ClientGuideData clientGuideData, [FromHeader] string tokenTemplate)
        {
           var result = await _apiMailService.SendHTMLMailAsync(clientGuideData, tokenTemplate);
              if(result)
              {
                return Ok();
              }
            
           return BadRequest();
        }

        [HttpPost]
        [Route("SendMailWithAttachmentAsync")]
        public async Task<IActionResult> SendMailWithAttachmentAsync([FromForm] MailDataWithAttachment mailDataWithAttachment)
        {
            bool result = await _apiMailService.SendMailWithAttachmentsAsync(mailDataWithAttachment);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
