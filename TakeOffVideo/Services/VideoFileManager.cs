using Microsoft.JSInterop;
using System;

namespace TakeOffVideo.Services
{


    public interface IVideoFileManager
    {
        Task AggiungiNuovo(string url, int turno, string? pettorale);

        IEnumerable<VideoFile> GetElenco();

        void RegistraOnNuovo(Func<VideoFile, Task> action);
        VideoFile? GetById(int id);

        Task AggiungiDaFile(string url);

    }

    public class VideoFile
    {
        public int ID { get; set; }
        public DateTime OraRegistrazione { get; set; }

        public string? Url { get; set; }
        public int Turno { get; set; }
        public string? Pettorale { get; set; }

        public bool Pinned { get; set; } = false;

        public string NomeFile => $"TOV_{OraRegistrazione:yyyyMMdd}_{OraRegistrazione:HH-mm-ss}_{Turno}_{Pettorale}.webm";


        public override string ToString()
        {
            return $"{OraRegistrazione:HH:mm:ss} turno {Turno} #{Pettorale}";
        }

    }

    public class VideoFileManager : IVideoFileManager
    {

        private IJSObjectReference? _JScriptfile = null;

        List<Func<VideoFile, Task>> _actions = new ();

        List<VideoFile> _urls = new();

        IJSRuntime _JS;
        public VideoFileManager(IJSRuntime jS)
        {
            _JS = jS;
        }

        private async Task<IJSObjectReference> GetRef()
        {
            _JScriptfile ??= await _JS.InvokeAsync<IJSObjectReference>("import", "./script/videofilemanager.js");

            return _JScriptfile;
        }


        public void RegistraOnNuovo(Func<VideoFile, Task> action)
        {
            _actions.Add(action);
        }

        private int _maxid = 1;

        public async Task AggiungiNuovo(string url, int turno, string? pettorale)
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

                await PulisciOld();

                // salva su file

                //var nome = $"{DateTime.Today:yyyyMMdd}_{DateTime.Now:HH-mm-ss}_{turno}_{pettorale}.webm";

                var r = await GetRef();
                await r.InvokeVoidAsync("downloadBlob", url, v.NomeFile);

                foreach (var action in _actions)
                    await action(v);


                // ripulire i vecchi
            }

        }

        private async Task PulisciOld()
        {
           
            int MAX_TO_KEEP = 3;
            var listaelim1 = _urls.Where(v => !v.Pinned).OrderByDescending(v => v.OraRegistrazione);

            if (_urls.Count <= MAX_TO_KEEP)
                return;
            
            var listaelim = listaelim1.TakeLast(_urls.Count - MAX_TO_KEEP);

            var r = await GetRef();
 
            
            foreach(var v in listaelim)
            {
                await r.InvokeVoidAsync("rimuoviblob", v.Url);
            }

            _urls.RemoveAll(v => listaelim.Contains(v));

        }


        public IEnumerable<VideoFile> GetElenco()
        {
            return _urls.OrderByDescending(v => v.OraRegistrazione);
        }

        public VideoFile? GetById(int id)
        {
            return _urls.FirstOrDefault(v => v.ID == id);
        }


        public async Task AggiungiDaFile(string url)
        {
            if (!_urls.Any(v => v.Url == url))
            {
                var v = new VideoFile
                {
                    ID = _maxid++,
                    Url = url,
                    Turno = 0, //prendere dal nome??
                    Pettorale = "DAFILE",  //prendere dal nome??
                    OraRegistrazione = DateTime.Now, //prendere dal nome??
                    Pinned = true,
                };

                _urls.Add(v);

                await PulisciOld();

                foreach (var action in _actions)
                    await action(v);
            }
        }
    }
}
