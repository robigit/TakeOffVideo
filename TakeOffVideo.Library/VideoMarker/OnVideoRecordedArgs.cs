using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.VideoMarker
{
    public record OnVideoRecordedArgs(string Url, string Tipo, TimeSpan Durata);
}
