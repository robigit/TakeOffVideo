using Microsoft.JSInterop;
using System;
using System.Text.RegularExpressions;

namespace TakeOffVideo.Library.VideoFileManager
{


    public interface IVideoFileManager
    {
        Task AggiungiNuovo(string url, string tipo, int turno, string? pettorale);

        IEnumerable<VideoFile> GetElenco();

        //void RegistraOnNuovo(Func<VideoFile, Task> action);

        event Func<VideoFile, Task>? OnNuovo;

        VideoFile? GetById(int id);

        Task<bool> AggiungiDaFile(string url, string nome);

    }

    public class VideoFile
    {
        public int ID { get; set; }
        public DateTime OraRegistrazione { get; set; }

        public string? Url { get; set; }

        public string? Tipo { get; set; }
        public int Turno { get; set; }
        public string? Pettorale { get; set; }

        public bool Pinned { get; set; } = false;

        public string NomeFile => $"TOV_{OraRegistrazione:yyyyMMdd}_{OraRegistrazione:HH-mm-ss}_{Turno}_{Pettorale}.{Tipo}";


        public override string ToString()
        {
            return $"{OraRegistrazione:HH:mm:ss} turno {Turno} #{Pettorale}";
        }

    }

    public class VideoFileManager : IVideoFileManager
    {

        private IJSObjectReference? _JScriptfile = null;

        //List<Func<VideoFile, Task>> _actions = new ();

        public event Func<VideoFile, Task>? OnNuovo;

        private async Task NotifyOnNuovo(VideoFile v)
        { 
            if(OnNuovo!=null) 
                await OnNuovo.Invoke(v);
        }

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
            OnNuovo += action;
        }

        private int _maxid = 1;

        public async Task AggiungiNuovo(string url, string tipo, int turno, string? pettorale)
        {
            if (!_urls.Any(v=> v.Url == url))
            {
                var v = new VideoFile
                {
                    ID = _maxid++,
                    Url = url,
                    Turno = turno,
                    Pettorale = pettorale,
                    OraRegistrazione = DateTime.Now,
                    Tipo = tipo
                };

                _urls.Add(v);

                await PulisciOld();

                // salva su file

                var r = await GetRef();
                await r.InvokeVoidAsync("downloadBlob", url, v.NomeFile);

                //foreach (var action in _actions)
                //    await action(v);

                await NotifyOnNuovo(v);


                // ripulire i vecchi
            }

        }

        private async Task PulisciOld()
        {
           
            int MAX_TO_KEEP = 3;

            int ndaelim = _urls.Where(v => !v.Pinned).Count();

            if (ndaelim <= MAX_TO_KEEP)
                return;

            var listaelim1 = _urls.Where(v => !v.Pinned).OrderByDescending(v => v.OraRegistrazione);

            
            
            var listaelim = listaelim1.TakeLast(ndaelim - MAX_TO_KEEP);

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


        public async Task<bool> AggiungiDaFile(string url, string nomefile)
        {
            bool ret = false;

            var m = Regex.Match(nomefile,
                @"^TOV_(?<anno>\d{4})(?<mese>\d{2})(?<giorno>\d{2})_(?<ora>\d{2})-(?<min>\d{2})-(?<sec>\d{2})_(?<turno>\d+)_(?<pett>\w*).(?<tipo>\w*)");

            try
            {
                if (m.Success)
                {

                    var date = new DateTime(int.Parse(m.Groups["anno"].Value), int.Parse(m.Groups["mese"].Value), int.Parse(m.Groups["giorno"].Value)
    
                               ,int.Parse(m.Groups["ora"].Value), int.Parse(m.Groups["min"].Value), int.Parse(m.Groups["sec"].Value)
                        );

                    if (date.Date == DateTime.Today.Date)
                    {

                        ret = true;

                        int.TryParse(m.Groups["turno"].Value, out int turno);
                        var pett = m.Groups["pett"].Value;

                        var tipo = m.Groups["tipo"].Value;

                        var v = new VideoFile
                        {
                            ID = _maxid++,
                            Url = url,
                            Turno = turno, 
                            Pettorale = pett,  
                            OraRegistrazione = date, 
                            Pinned = true,
                            Tipo = tipo
                        };

                        _urls.Add(v);

                        await PulisciOld();


                        await NotifyOnNuovo(v);
                        //foreach (var action in _actions)
                        //    await action(v);
                    }
                }
            }
            catch
            {

            }

            if (!ret)
            {
                await (await GetRef()).InvokeVoidAsync("rimuoviblob", url);
            }

            return ret;
        }
    }
}
