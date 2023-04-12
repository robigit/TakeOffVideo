using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeOffVideo.Library.DBFReader;

namespace TakeOffVideo.Library.Global
{
    public class GlobalObjects
    {
        public IEnumerable<AtletaWise>? Atleti { get; set; } = null;

        public string? GaraSel { get; set; }

        public string? NomeFileDbf { get; set; }


    }
}
