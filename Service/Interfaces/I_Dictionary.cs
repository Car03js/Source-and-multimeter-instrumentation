using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface I_Dictionary<T> : IEnumerable<KeyValuePair<int, List<string>>>
    {
        void AgregarHistorial(int id, string detalle);
        int cuenta();
        string[][] Mostrar();
        //public bool GetValorPorId(string id, string clave, out object? valor);
        bool TryGetRegistro(int id, out List<string> registro);
    }
}
