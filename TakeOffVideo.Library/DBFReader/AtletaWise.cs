using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.DBFReader
{
    public class AtletaWise
    {
        public string? NOMINATIVO { get; set; }

        /// <summary>
        /// Societa
        /// </summary>
        public string? DENOM { get; set; }
        public string? PETTO { get; set; }
        public string? CATEG { get; set; }


        public string? CODGARA { get; set; }

        /// <summary>
        /// Intestazione Gara
        /// </summary>
        public string? INTESTAZIO { get; set; }


        /// <summary>
        /// S o N
        /// </summary>
        public string? CONFERMA { get; set; }

        public string? NUM_TES { get; set; }
        public string? DAT_NAS { get; set; }

        public override string ToString()
        {
            return $"{PETTO} - {NOMINATIVO}";
        }

    }
}
