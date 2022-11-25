using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeOffVideo.Library.Util;

namespace TakeOffVideo.Library.TOVFileManagerNS;

public class TOVFileManager : JSModule
{
    public TOVFileManager(IJSRuntime js)
        : base(js, "./_content/TakeOffVideo.Library/TOVFileManager.js")
    {
    }

    public record JSDirectory(string Name, IJSObjectReference Instance) : IAsyncDisposable
    {
        // When .NET is done with this JSDirectory, also release the underlying JS object
        public ValueTask DisposeAsync() => Instance.DisposeAsync();
    }

    JSDirectory? _directory;

    public async ValueTask<JSDirectory> ShowDirectoryPicker()
    {
        _directory = await InvokeAsync<JSDirectory>("showDirectoryPicker");
        return _directory;
    }

    public async ValueTask<bool> SalvaFileSuCartella(string nome, string url )
    {
        if(_directory== null) { return false;  }
        await InvokeVoidAsync("salvaFile", _directory.Instance, nome, url);
        return true;
    }


}
