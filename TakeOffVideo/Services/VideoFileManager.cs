using Microsoft.JSInterop;

namespace TakeOffVideo.Services
{


    public interface IVideoFileManager
    {
        Task AggiungiNuovo(string url, int turno, string pettorale);

        IEnumerable<VideoFile> GetElenco();

        void RegistraOnNuovo(Func<VideoFile, Task> action);
        VideoFile? GetById(int id);
    }

    public class VideoFile
    {
        public int ID { get; set; }
        public DateTime OraRegistrazione { get; set; }

        public string Url { get; set; }
        public int Turno { get; set; }
        public string Pettorale { get; set; }

        public string NomeFile => $"{OraRegistrazione:yyyyMMdd}_{OraRegistrazione:HH-mm-ss}_{Turno}_{Pettorale}.webm";

        public override string ToString()
        {
            return $"{OraRegistrazione:HH:mm:ss} turno {Turno} #{Pettorale}"; 
        }

        
    }

    public class VideoFileManager : IVideoFileManager
    {

        private IJSObjectReference? _JSRecorder = null;

        List<Func<VideoFile, Task>> _actions = new ();

        List<VideoFile> _urls = new();

        IJSRuntime _JS;
        public VideoFileManager(IJSRuntime jS)
        {
            _JS = jS;
        }

        private async Task<IJSObjectReference> GetRef()
        {
            _JSRecorder ??= await _JS.InvokeAsync<IJSObjectReference>("import", "./script/videofilemanager.js");

            return _JSRecorder;
        }


        public void RegistraOnNuovo(Func<VideoFile, Task> action)
        {
            _actions.Add(action);
        }

        private int _maxid = 1;

        public async Task AggiungiNuovo(string url, int turno, string pettorale)
        {
            if (!_urls.Any(v=> v.Url == url))
            {
                var v = new VideoFile
                {
                    ID = _maxid++,
                    Url = url,
                    Turno = turno,
                    Pettorale = pettorale,
                    OraRegistrazione = DateTime.Now
                };

                _urls.Add(v);

                // salva su file

                //var nome = $"{DateTime.Today:yyyyMMdd}_{DateTime.Now:HH-mm-ss}_{turno}_{pettorale}.webm";

                var r = await GetRef();
                await r.InvokeVoidAsync("downloadBlob", url, v.NomeFile);

                foreach (var action in _actions)
                    await action(v);


                // ripulire i vecchi
            }


        }

        public IEnumerable<VideoFile> GetElenco()
        {
            return _urls.OrderByDescending(v => v.OraRegistrazione);
        }

        public VideoFile? GetById(int id)
        {
            return _urls.FirstOrDefault(v => v.ID == id);
        }


    }
}
