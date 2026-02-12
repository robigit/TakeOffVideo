using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.TOVFileManagerNS;

public class VideoFile
{
    //public int ID { get; set; }
    public DateTime OraRegistrazione { get; set; }

    public string OraRegistrazioneString
    {
        get 
        {
            var r =OraRegistrazione.ToString("HH:mm:ss");
            if (OraRegistrazione < DateTime.Today)
                r += "\n" + OraRegistrazione.ToString("d");

            return r ;
        }
    }


    public string? Url { get; set; }

    public string? Tipo { get; set; }
    public int Turno { get; set; }
    public string? Pettorale { get; set; }

    public bool Pinned { get; set; } = false;

    public TimeSpan Durata { get; set; }

    private static string SanitizeFilename(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "Unknown";
        
        // Remove invalid filename characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Concat(input.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        
        // Ensure we have something after sanitization
        return string.IsNullOrWhiteSpace(sanitized) ? "Unknown" : sanitized;
    }

    public string SoloNome => 
        $"TOV_{OraRegistrazione:yyyyMMdd}_{OraRegistrazione:HH-mm-ss}_{Turno}_{SanitizeFilename(Pettorale)}";

    public string NomeFile => $"{SoloNome}.{SanitizeFilename(Tipo)}";


    public override string ToString()
    {
        return $"{OraRegistrazione:HH:mm:ss} turno {Turno} #{Pettorale}";
    }

}