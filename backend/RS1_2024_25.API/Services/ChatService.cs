using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RS1_2024_25.API.Data;
using RS1_2024_25.API.Data.Models.TenantSpecificTables.Modul2_Basic;

namespace RS1_2024_25.API.Services
{
    public class ChatService
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<ChatService> _logger;
        private readonly ApplicationDbContext _db;

        private readonly string _model = "gpt-3.5-turbo";

        public ChatService(IConfiguration config, ILogger<ChatService> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;

            var apiKey = config["OpenAI:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Missing OpenAI API key in configuration.");
            }

            var openAIClient = new OpenAIClient(apiKey);
            _chatClient = openAIClient.GetChatClient(_model);
        }

        // ---------------------------------------------------
        // 1) PRODUCT CHECK IN DATABASE (TENANT SAFE)
        // ---------------------------------------------------

        private async Task<string?> TryFindProductAsync(string userMessage)
        {
            var msg = userMessage.ToLower().Trim();

            // Search all products for the current tenant
            var products = await _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .AsNoTracking()
                .ToListAsync();

            // Simple fuzzy logic: if the product name exists inside the message
            var found = products.FirstOrDefault(p =>
                msg.Contains(p.Name.ToLower())
            );

            if (found == null)
                return null;

            return
                $"✔ The product **{found.Name}** *is available*.\n" +
                $"💰 Price: **{found.Price} KM**\n" +
                $"🏷️ Brand: **{found.Brand?.Name ?? "N/A"}**\n" +
                $"🎨 Color: **{found.Color?.Name ?? "N/A"}**";
        }

        // ---------------------------------------------------
        // 2) MAIN CHAT METHOD
        // ---------------------------------------------------

        public async Task<string> GetChatResponseAsync(string userMessage)
        {
            try
            {
                // 1) First check the database
                var dbCheck = await TryFindProductAsync(userMessage);

                if (dbCheck != null)
                    return dbCheck;

                // 2) If not found — send to OpenAI model
                var messages = new List<ChatMessage>
                {
                    ChatMessage.CreateSystemMessage(
                        "You are an HRKE Shop assistant. If asked about product availability, DO NOT invent products. " +
                        "If the product check returns null, simply say it is not available."
                    ),
                    ChatMessage.CreateUserMessage(userMessage)
                };

                var response = await _chatClient.CompleteChatAsync(messages);
                var chatCompletion = response.Value;

                var contentParts = chatCompletion?.Content;

                if (contentParts != null && contentParts.Any())
                    return string.Join(" ", contentParts.Select(p => p.Text));

                return "⚠️ Unable to generate a response at this moment.";
            }
            catch (System.ClientModel.ClientResultException ex)
            {
                _logger.LogError($"OpenAI API Error: {ex.Message}");

                if (ex.Message.Contains("insufficient_quota"))
                    return "⚠️ Chatbot temporarily unavailable — quota exceeded.";

                if (ex.Message.Contains("rate_limit"))
                    return "⚙️ Too many requests. Please try again in a few seconds.";

                return "⚠️ Connection problem with AI service.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return "❌ Unexpected error while processing the request.";
            }
        }
    }
}
