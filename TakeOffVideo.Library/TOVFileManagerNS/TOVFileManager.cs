using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TakeOffVideo.Library.Util;

namespace TakeOffVideo.Library.TOVFileManagerNS;

public partial class TOVFileManager : JSModule
{

    public event Func<VideoFile, Task>? OnNuovo;

    public enum TipoScelta{ Ok, Cancel, NotSupported }

    private async Task NotifyOnNuovo(VideoFile v)
    {
        if (OnNuovo != null)
            await OnNuovo.Invoke(v);
    }

    List<VideoFile> _urls = new();
    public TOVFileManager(IJSRuntime js)
        : base(js, "./_content/TakeOffVideo.Library/TOVFileManager.js")
    {
    }

    public record JSDirectory(string Name, IJSObjectReference Instance, bool Supported) : IAsyncDisposable
    {
        // When .NET is done with this JSDirectory, also release the underlying JS object
        public ValueTask DisposeAsync() => Instance.DisposeAsync();
    }

    JSDirectory? _directoryvideo;

    JSDirectory? _directoryimg;

    public async ValueTask<TipoScelta> ShowDirectoryPicker(bool video)
    {
        
        var dir = await InvokeAsync<JSDirectory>("showDirectoryPicker");
        
        if(dir.Supported)
        {
            if(video)
                _directoryvideo = dir;
            else
                _directoryimg = dir;
            return TipoScelta.Ok;
        }
        
        return dir.Name.Contains("Abort") ? TipoScelta.Cancel : TipoScelta.NotSupported;

    }

    public string? GetNomeDir(bool video)
    {
        return  video? _directoryvideo?.Name : _directoryimg?.Name ;
    }


    public async ValueTask<bool> SalvaFileSuCartella(string nome, string url )
    {
        if (_directoryvideo== null) 
        { 
            await InvokeVoidAsync("downloadBlob", url, nome);
        }
        else
            await InvokeVoidAsync("salvaFile", _directoryvideo.Instance, nome, url);
        return true;
    }

    public async ValueTask<bool> SalvaFileSuCartellaImg(string nome, string url)
    {
        if (_directoryimg != null)
        {
            await InvokeVoidAsync("salvaFile", _directoryimg.Instance, nome, url);
            return true;
        }
        return false;
    }

    public IEnumerable<VideoFile?> GetElenco()
    {
        return _urls.OrderByDescending(v => v.OraRegistrazione);
    }

    public async Task AggiungiNuovo(string url, string tipo, int turno, string? pettorale, TimeSpan durata)
    {
        if (!_urls.Any(v => v.Url == url))
        {
            var v = new VideoFile
            {
                //ID = _maxid++,
                Url = url,
                Turno = turno,
                Pettorale = pettorale,
                OraRegistrazione = DateTime.Now,
                Tipo = tipo,
                Durata = durata
            };

            _urls.Add(v);

            await RimuoviVideoVecchi();

            // salva su file

            await SalvaFileSuCartella(v.NomeFile, url);

            await NotifyOnNuovo(v);

        }

    }
    private async Task RimuoviVideoVecchi()
    {

        int MAX_TO_KEEP = 3;

        int ndaelim = _urls.Where(v => !v.Pinned).Count();

        if (ndaelim <= MAX_TO_KEEP)
            return;

        var listaelim1 = _urls.Where(v => !v.Pinned).OrderByDescending(v => v.OraRegistrazione);


        var listaelim = listaelim1.TakeLast(ndaelim - MAX_TO_KEEP);

        
        foreach (var v in listaelim)
        {
            await InvokeVoidAsync("rimuoviblob", v.Url);
        }

        _urls.RemoveAll(v => listaelim.Contains(v));

    }

    public async Task<bool> AggiungiDaFile(string url, string nomefile)
    {
        bool ret = false;

        var m = RegexNomeFile().Match(nomefile);

        try
        {
            if (m.Success)
            {

                var date = new DateTime(int.Parse(m.Groups["anno"].Value), int.Parse(m.Groups["mese"].Value), int.Parse(m.Groups["giorno"].Value)

                           , int.Parse(m.Groups["ora"].Value), int.Parse(m.Groups["min"].Value), int.Parse(m.Groups["sec"].Value)
                    );

                if (true || date.Date == DateTime.Today.Date)
                {

                    ret = true;

                    int.TryParse(m.Groups["turno"].Value, out int turno);
                    var pett = m.Groups["pett"].Value;

                    var tipo = m.Groups["tipo"].Value;

                    var v = new VideoFile
                    {
                        //ID = _maxid++,
                        Url = url,
                        Turno = turno,
                        Pettorale = pett,
                        OraRegistrazione = date,
                        Pinned = true,
                        Tipo = tipo
                    };

                    _urls.Add(v);

                    await RimuoviVideoVecchi();


                    await NotifyOnNuovo(v);

                }
            }
        }
        catch
        {

        }

        if (!ret)
        {
            await InvokeVoidAsync("rimuoviblob", url);
        }

        return ret;
    }

    [GeneratedRegex("^TOV_(?<anno>\\d{4})(?<mese>\\d{2})(?<giorno>\\d{2})_(?<ora>\\d{2})-(?<min>\\d{2})-(?<sec>\\d{2})_(?<turno>\\d+)_(?<pett>\\w*).(?<tipo>\\w*)")]
    private static partial Regex RegexNomeFile();
}
