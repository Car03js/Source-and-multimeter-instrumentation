using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physical_Instrument
{
    public interface I_Instrumentos
    {
        void Connection_Instrument(string resourceName);
        void Close_Instrument();
        void Write(string command);
        string Read();
        string Query(string command);
        string[] ListResources();

    }

    
}
