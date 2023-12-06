using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;

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

        public async Task<bool> SendHTMLMailAsync(HTMLMailData htmlMailData, string tokenTemplate)
        {
            var htmlTemplateText = await GetTemplateContentFromMailtrap(htmlMailData.EmailToId,tokenTemplate, htmlMailData.EmailToName);
            var htmlBody = htmlTemplateText;

            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = htmlMailData.EmailToId, Name = htmlMailData.EmailToName } },
                Subject = htmlBody,
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

        private async Task<string> GetTemplateContentFromMailtrap(string email,string tokenTemplate, string name)
        {
            string htmlTemplateText = await RetrieveTemplateContentUsingMailtrapAPI( email,tokenTemplate,name);
            return htmlTemplateText;
        }
        private async Task<string> RetrieveTemplateContentUsingMailtrapAPI(string email,string tokenTemplate,string name)
        {
            string _mailtrapApiKey = "9f875d92a1ac089025aa184ba649ee4e";
            try
            {
                var client = new RestClient("https://send.api.mailtrap.io/api/send");

                var request = new RestRequest("",Method.Post);
                request.AddHeader("Authorization", $"Bearer {_mailtrapApiKey}");
                request.AddHeader("Content-Type", "application/json");

                // Puedes modificar los datos del mensaje según tus necesidades
                var jsonBody = JsonConvert.SerializeObject(new
                {
                    from = new { email = "mailtrap@netboxworld.com", name = "Mailtrap Test" },
                    to = new[] { new { email = email } },
                    template_uuid = tokenTemplate,
                    template_variables = new
                    {
                        user_email = name,
                        next_step_link= "Test_Next_step_link",
                        get_started_link="Test_Get_started_link",
                        onboarding_video_link="Test_Onboarding_video_link"
                    }
                });

                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = response.Content;
                    return responseString;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.StatusDescription}");
                    Console.WriteLine($"Response Content: {response.Content}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            throw new Exception("Failed to retrieve template content from Mailtrap.io");
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
