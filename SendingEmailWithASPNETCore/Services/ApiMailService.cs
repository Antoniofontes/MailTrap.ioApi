using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using ShipmentInformation;
using System.Net;

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

        public async Task<bool> SendHTMLMailAsync(ClientGuideData data, string tokenTemplate)
        {
            var htmlTemplateText = await GetTemplateContentFromMailtrap(data.Cli.CliEma,tokenTemplate, data.Cli.CliNom,data);
            var htmlBody = htmlTemplateText;


            var apiEmail = new
            {
                From = new { Email = _mailSettings.SenderEmail, Name = _mailSettings.SenderEmail },
                To = new[] { new { Email = data.Cli.CliEma, Name = data.Cli.CliNom } },
                Subject = "Shipment update",
                Html = htmlBody
            };
            
            if(apiEmail.Html== "true")
            {
                return true;
            }
            var httpResponse = await _httpClient.PostAsJsonAsync("send", apiEmail);

            var responseJson = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

            if (response != null && response.TryGetValue("success", out object? success) && success is bool boolSuccess && boolSuccess)
            {
                return true;
            }

            return false;
        }

        private async Task<string> GetTemplateContentFromMailtrap(string email,string tokenTemplate, string name, ClientGuideData data)
        {
            string htmlTemplateText = await RetrieveTemplateContentUsingMailtrapAPI( email,tokenTemplate,name, data);
            return htmlTemplateText;
        }
        private async Task<string> RetrieveTemplateContentUsingMailtrapAPI(string email, string tokenTemplate, string name, ClientGuideData data)
        {
            string _mailtrapApiKey = "9f875d92a1ac089025aa184ba649ee4e";

            try
            {
                var client = new RestClient("https://send.api.mailtrap.io/api/send");
                var request = new RestRequest("", Method.Post);
                request.AddHeader("Authorization", $"Bearer {_mailtrapApiKey}");
                request.AddHeader("Content-Type", "application/json");

                var guidesList = data.Guias.Select(guide => new
                {
                    awb= guide.Guia,
                    date = guide.Fecha.Date,
                    from_name = guide.Remitente,
                    to_name = guide.Destinatario,
                    contents = guide.Contenido,
                    weight = guide.Peso,
                    value = guide.Valor,
                    tracking_number = guide.Tracking,
                    tracking_url = guide.TrackingLink,
                    
                }).ToList();

                var jsonBody = JsonConvert.SerializeObject(new
                {
                    from = new { email = "mailtrap@netboxworld.com", name = "Mailtrap Test" },
                    to = new[] { new { email = email } },
                    template_uuid = tokenTemplate,
                    template_variables = new
                    {
                        client_name = name,
                        guides = guidesList, // Aquí pasas la lista de guías
                        tracking_portal_url = "https://www.google.com",
                        customer_service_contact = "[Customer Service Email or Phone Number]",
                        your_full_name = "[Your Full Name]",
                        your_position = "[Your Position]",
                        your_company = "[Your Company]",
                        your_contact_information = "[Your Contact Information]"
                    }
                });

                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseString = response.Content;
                    return "true";
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
