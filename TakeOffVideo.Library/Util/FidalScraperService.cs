using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TakeOffVideo.Library.Global;

namespace TakeOffVideo.Library.Util
{
    public class FidalScraperService
    {
        private readonly HttpClient _httpClient;

        public FidalScraperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Atleta>> GetAtletiFromUrlAsync(string url)
        {
            // Use corsproxy.io to bypass browser CORS restrictions
            var proxyUrl = "https://corsproxy.io/?" + Uri.EscapeDataString(url);
            var html = await _httpClient.GetStringAsync(proxyUrl);
            return ParseHtml(html);
        }

        public async Task<List<Atleta>> GetAtletiFromStreamAsync(System.IO.Stream stream)
        {
            using var reader = new System.IO.StreamReader(stream);
            var html = await reader.ReadToEndAsync();
            return ParseHtml(html);
        }

        public List<Atleta> ParseHtml(string html)
        {
            var atleti = new List<Atleta>();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find the main results table
            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'table-striped')]");
            if (table == null) return atleti;

            // Get all rows in the tbody
            var rows = table.SelectNodes(".//tbody/tr");
            if (rows == null) return atleti;

            foreach (var row in rows)
            {
                // Skip rows that are panel-collapses (attempts details) or empty spacer rows
                if (row.HasClass("panel-collapse") || row.SelectNodes("td") == null || row.SelectNodes("td").Count < 5)
                {
                    continue;
                }

                var cells = row.SelectNodes("td");

                if (cells.Count >= 5)
                {
                    try
                    {
                        // Check if it's Iscrizioni (col 0 is Pettorale) or Risultati (col 1 is Pettorale)
                        // In Iscrizioni, col 1 has the <a> tag. In Risultati, col 2 has the <a> tag.
                        int pettoraleIndex = 1;
                        int atletaIndex = 2;
                        int societaIndex = 5;

                        // Se la colonna 1 ha il tag link (Iscrizioni), adatta gli indici
                        if (cells.Count > 1 && cells[1].SelectSingleNode(".//a") != null)
                        {
                            pettoraleIndex = 0;
                            atletaIndex = 1;
                            societaIndex = 4;
                        }

                        // Se invece non c'è il tag a in nessuna delle due tipiche posizioni, skippa
                        if (cells.Count > atletaIndex && cells[atletaIndex].SelectSingleNode(".//a") == null && 
                            cells[1].SelectSingleNode(".//a") == null) 
                        {
                            continue;
                        }

                        var pettoraleStr = cells[pettoraleIndex].InnerText.Trim();
                        var aTag = cells[atletaIndex].SelectSingleNode(".//a");
                        string fullNome = aTag != null ? aTag.InnerText.Trim() : cells[atletaIndex].InnerText.Trim();
                    
                        // The site puts LASTNAME Firstname
                        fullNome = fullNome.Replace("Campione Italiano", "").Trim();
                        var nameParts = fullNome.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        string cognome = nameParts.Length > 0 ? nameParts[0] : "";
                        string nome = nameParts.Length > 1 ? nameParts[1] : "";

                        // For FIDAL, the society string might have spans, so getting inner text usually cleans it
                        // Make sure we have enough columns for societa
                        string societaStr = cells.Count > societaIndex ? cells[societaIndex].InnerText.Trim() : "";
                        string societa = System.Net.WebUtility.HtmlDecode(societaStr).Trim();

                        if (!string.IsNullOrEmpty(fullNome) && fullNome != "DNS")
                        {
                            atleti.Add(new Atleta
                            {
                                Pettorale = pettoraleStr,
                                Cognome = cognome,
                                Nome = nome,
                                Societa = societa
                            });
                        }
                    }
                    catch
                    {
                        // Ignore malformed rows
                    }
                }
            }

            return atleti;
        }
        public Task<List<Atleta>> GetAtletiFromTextAsync(string text)
        {
            var atleti = new List<Atleta>();
            if (string.IsNullOrWhiteSpace(text)) return Task.FromResult(atleti);

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in lines)
            {
                // Copied tables usually have tab-separated values
                var parts = line.Split('\t', StringSplitOptions.TrimEntries);
                
                if (parts.Length >= 5)
                {
                    // Determiniamo il layout.
                    // Risultati: Col0: Classifica, Col1: Pettorale (numero), Col2: Nome, Col5: Societa
                    // Iscrizioni: Col0: Pettorale (numero), Col1: Nome (stringa), Col4: Societa
                    int pettoraleIndex = 1;
                    int atletaIndex = 2;
                    int societaIndex = 5;

                    // Se la colonna 1 NON è parsabile come intero, probabilmente è il Nome dell'atleta (Layout Iscrizioni)
                    if (!int.TryParse(parts[1], out _))
                    {
                        pettoraleIndex = 0;
                        atletaIndex = 1;
                        societaIndex = 4;
                    }

                    if (parts.Length > societaIndex)
                    {
                        var pettoraleStr = parts[pettoraleIndex];
                        var fullNome = parts[atletaIndex];
                        var societaStr = parts[societaIndex];

                        // Sometimes the name might contain " Campione Italiano" or similar tags pasted
                        fullNome = fullNome.Replace("Campione Italiano", "").Trim();
                        
                        var nameParts = fullNome.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        string cognome = nameParts.Length > 0 ? nameParts[0] : "";
                        string nome = nameParts.Length > 1 ? nameParts[1] : "";

                        // skip header rows
                        if (!string.IsNullOrEmpty(fullNome) && fullNome != "DNS" && fullNome.ToUpper() != "ATLETA" && fullNome.ToUpper() != "COGNOME E NOME") 
                        {
                            atleti.Add(new Atleta
                            {
                                Pettorale = pettoraleStr,
                                Cognome = cognome,
                                Nome = nome,
                                Societa = societaStr
                            });
                        }
                    }
                }
            }

            return Task.FromResult(atleti);
        }
    }
}
