using LjussAPI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Threading.Tasks;
using NLog;

namespace YourApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _apiKey;
        private readonly string _senderEmail;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["BrevoSettings:ApiKey"];
            _senderEmail = configuration["BrevoSettings:SenderEmail"];

            if (String.IsNullOrEmpty(_apiKey) || String.IsNullOrEmpty(_senderEmail))
            {
                throw new ArgumentException("Brevo API Key or SenderEmail is not configured properly.");
            }
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var client = new RestClient("https://api.brevo.com/v3/smtp/email");
                var request = new RestRequest("", Method.Post);

                request.AddHeader("accept", "application/json");
                request.AddHeader("api-key", _apiKey);
                request.AddHeader("content-type", "application/json");

                var emailData = new
                {
                    sender = new { name = "Furkan", email = _senderEmail },
                    to = new[] { new { email = to, name = "Alıcı" } },
                    subject = subject,
                    htmlContent = $"<p>{body}</p>"
                };

                request.AddJsonBody(emailData);
                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    _logger.Error($"E-posta gönderilemedi! Hata: {response.StatusCode} - {response.Content}");
                    return false;
                }

                _logger.Info($"E-posta başarıyla gönderildi: {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "E-posta gönderme sırasında hata oluştu.");
                return false;
            }
        }
    }
}
