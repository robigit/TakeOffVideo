using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.TOVFileManagerNS;

public class VideoFile
{
    //public int ID { get; set; }
    public DateTime OraRegistrazione { get; set; }

    public string? Url { get; set; }

    public string? Tipo { get; set; }
    public int Turno { get; set; }
    public string? Pettorale { get; set; }

    public bool Pinned { get; set; } = false;

    public TimeSpan Durata { get; set; }

    public string NomeFile => $"TOV_{OraRegistrazione:yyyyMMdd}_{OraRegistrazione:HH-mm-ss}_{Turno}_{Pettorale}.{Tipo}";


    public override string ToString()
    {
        return $"{OraRegistrazione:HH:mm:ss} turno {Turno} #{Pettorale}";
    }

}