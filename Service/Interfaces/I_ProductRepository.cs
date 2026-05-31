   using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface I_ProductRepository
    {
        //List<Producto> BuscarPorNombre(string nombre);
        //DataTable LlenarDTConSP_Proveedor(string connectionString);
        List<string> GetNS();
        int GetNextID( int tabla);
        string difSN(string Name);
        void insertTp(TPS tp);
        void insert_Mod_Prod(Modelo mod, Productos prod);
        List<TPS> Get_TP(string ns);


        I_Dictionary<Productos> ProductHistory();

        DataTable GetFilteredResults(int operationNumber, string nSerie, DateTime startDate,
        DateTime endDate, string status, string model);
        void insert(string ns, int tp, decimal res, string stat, DateTime date);
        //List<Producto > BuscarPorFechaMovimiento(string fecha);
    }
}
