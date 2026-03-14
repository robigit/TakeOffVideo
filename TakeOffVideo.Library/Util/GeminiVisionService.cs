using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.Util
{
    public class GeminiVisionService
    {
        private readonly HttpClient _httpClient;

        public GeminiVisionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeminiNulloResult> AnalyzeFoulAsync(string base64Image, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API Key mancante");

            // Rimuove il prefisso "data:image/png;base64," se presente
            var base64Data = base64Image;
            var parts = base64Image.Split(',');
            if (parts.Length == 2)
            {
                base64Data = parts[1];
            }

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";

            var systemPrompt = @"Sei un giudice internazionale di atletica leggera.
Osserva la linea gialla verticale (tavola di battuta) e il piede dell'atleta. L'atleta arriva da destra e salta verso sinistra.
Se il piede o la scarpa si estendono visibilmente a sinistra della linea gialla (toccando il suolo o la plastilina integrata oltre la linea), è un fallo (NULLO). Altrimenti è VALIDO.
Rispondi con una singola riga esatta in questo formato:
VERDETTO | Motivazione.
Dove VERDETTO può essere solo NULLO oppure VALIDO.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = systemPrompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = "image/png",
                                    data = base64Data
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.1, // Bassa temperatura per risposte deterministiche
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Errore API Gemini ({response.StatusCode}): {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            
            var aiText = result?.Candidates?[0]?.Content?.Parts?[0]?.Text ?? "";
            aiText = aiText.Trim();

            // Parsing: "NULLO | Il piede supera la linea..."
            bool isNullo = aiText.StartsWith("NULLO", StringComparison.OrdinalIgnoreCase);
            string messaggio = aiText;
            
            var split = aiText.Split('|');
            if (split.Length >= 2)
            {
                messaggio = split[1].Trim();
            }

            return new GeminiNulloResult(isNullo, messaggio);
        }
    }

    public record GeminiNulloResult(bool IsNullo, string Messaggio);

    // DTOs for Gemini API response
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[] Candidates { get; set; } = Array.Empty<Candidate>();
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; } = new Content();
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[] Parts { get; set; } = Array.Empty<Part>();
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = "";
    }
}
