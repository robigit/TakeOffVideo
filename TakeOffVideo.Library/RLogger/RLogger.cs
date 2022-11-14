using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeOffVideo.Library.RLogger
{

    public interface IRLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);

        IEnumerable<RLog> GetMessages(int fromid);

        void Enable(bool value);

        bool IsEnabled();


    }

    public enum TipoLog { Info, Warn, Error };

    public class RLog
    {
        public int _id = 0;
        static int progressivo = 0;

        public int ID => _id;

        public RLog() { _id = progressivo++; }

        public string? Message { get; set; }

        public TipoLog Log { get; set; }

        

    }

    public class RLogger : IRLogger
    {
        private readonly List<RLog> _log = new ();

        public void Error(string message)
        {   
            if (!_enabled)
            {
                return;
            }
            lock (_log)
            {
                _log.Add(new RLog { Message = message, Log = TipoLog.Error });
            }
        }

        public void Info(string message)
        {
            if(!_enabled) { return;  }
            lock (_log)
            {
                _log.Add(new RLog { Message = message, Log = TipoLog.Info });
            }
        }

        public void Warn(string message)
        {
            if (!_enabled) { return; }
            lock (_log)
            {
                _log.Add(new RLog { Message = message, Log = TipoLog.Warn });
            }
        }


        public IEnumerable<RLog> GetMessages(int fromid)
        {
            return _log.Where(m => m._id>=fromid);
        }

        private bool _enabled = false;

        public bool IsEnabled() => _enabled;

        public void Enable(bool value)
        {
            _enabled= value;
        }
    }
}
