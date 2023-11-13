using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SendingEmailWithASPNETCore.Services
{
    public class APIMailService : IAPIMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly HttpClient _httpClient;

        public APIMailService(IOptions<MailSettings> mailSettingsOptions, IHttpClientFactory httpClientFactory)
        {
            _mailSettings = mailSettingsOptions.Value;
            _httpClient = httpClientFactory.CreateClient("MailTrapApiClient");
        }

        public async Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData)
        {
            string filePath = Directory.GetCurrentDirectory() + "\\Templates\\welcome.html";
            string emailTemplateText = File.ReadAllText(filePath);

            var htmlBody = emailTemplateText;

            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = htmlMailData.EmailToId, Name = htmlMailData.EmailToName } },
                Subject = "Hello",
                Html = htmlBody
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendMailAsync(MailData mailData)
        {
            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = mailData.EmailToId, Name = mailData.EmailToName } },
                Subject = mailData.EmailSubject,
                Text = mailData.EmailBody
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SendMailWithAttachmentsAsync(MailDataWithAttachment mailDataWithAttachment)
        {
            var attachments = new List<object>();
            if (mailDataWithAttachment.EmailAttachments != null)
            {
                foreach (var attachmentFile in mailDataWithAttachment.EmailAttachments)
                {
                    if (attachmentFile.Length == 0)
                    {
                        continue;
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await attachmentFile.CopyToAsync(memoryStream);
                        attachments.Add(new
                        {
                            FileName = attachmentFile.FileName,
                            Content = Convert.ToBase64String(memoryStream.ToArray()),
                            Type = attachmentFile.ContentType,
                            Disposition = "attachment" // or inline
                        });
                    }
                }
            }

            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = mailDataWithAttachment.EmailToId, Name = mailDataWithAttachment.EmailToName } },
                Subject = mailDataWithAttachment.EmailSubject,
                Text = mailDataWithAttachment.EmailBody,
                Attachments = attachments.ToArray()
            };

            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }
    }
}
