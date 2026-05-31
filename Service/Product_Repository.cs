using Ivi.Visa;
using Service.Interfaces;
using System;
using System.Collections.Generic;   
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ProductoRepository : I_ProductRepository
    {
        private readonly string _connectionString;
        private readonly I_Dictionary<Productos> _pr = new SN<Productos>();

        public ProductoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int GetNextID(int tabla)
        {

            int id = 0;
            SqlConnection sqlconn = new SqlConnection(_connectionString);
            string Query = "";
            switch (tabla)
            {
                case 1: // ID de test Plan
                    Query = "SP_NEXT_TP_ID";
                    break;

                case 2: // ID de model Id
                    Query = "NEXT_M_ID";
                    break;
                case 3: // Id de Test result
                    Query = "Get_NextTRID";
                    break;


            }
            using (SqlCommand cmd = new SqlCommand(Query, sqlconn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                sqlconn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);  // primera columna
                    }
                }

            }
            return id;


            //var value = reader.Read();

        }

        public I_Dictionary<Productos> ProductHistory()
        {

            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SP_GetAllNS", connection))
            {

                //cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                int i = 1;
                // Insert Model
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    while (reader.Read())
                    {
                        _pr.AgregarHistorial(i, reader["NSerie"].ToString());
                        _pr.AgregarHistorial(i, reader["Model_Id"].ToString());
                        i++;
                    }
                }


            }
            return _pr;
        }
        public string difSN(string Name)
        {
            string status = string.Empty;
            SqlConnection sqlconn = new SqlConnection(_connectionString);
            string Query = "SP_Is_NS_Repeated";
            using (SqlCommand cmd = new SqlCommand(Query, sqlconn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NSerie", Name);
                sqlconn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (reader.Read())
                    {
                        return reader.GetString(0);  // primera columna
                    }
                }

            }
            return status;
        }

        public void insertTp(TPS tp)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SP_InsertProducto", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TestNumber", tp.T_N);
                cmd.Parameters.AddWithValue("@Version", tp.Version);
                cmd.Parameters.AddWithValue("@TestName", tp.Nombre);
                cmd.Parameters.AddWithValue("@LowLimit", tp.LI);
                cmd.Parameters.AddWithValue("@HighLimit", tp.LS);
                cmd.Parameters.AddWithValue("@MeasureUnit", tp.Unit);


                connection.Open();
                cmd.ExecuteNonQuery(); // IMPORTANTE: Ejecuta el INSERT

            }
        }

        public void insert_Mod_Prod(Modelo mod, Productos prod)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SP_ADD_ModProd", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Insert Model
                cmd.Parameters.AddWithValue("@Nombre", mod.Nombre);
                cmd.Parameters.AddWithValue("@TestPlanId", mod.M_ID);

                // Insert Product
                cmd.Parameters.AddWithValue("@NSerie", prod.NumeroSerie);
                cmd.Parameters.AddWithValue("@NuevoStatus", prod.Test_Status);



                connection.Open();
                cmd.ExecuteNonQuery(); // IMPORTANTE: Ejecuta el INSERT

            }
        }

        public List<string> GetNS()
        {

            List<string> ns = new List<string>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SP_GetAllNS", connection))
            {
                //cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();

                // Insert Model
                using (var reader = cmd.ExecuteReader())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    while (reader.Read())
                    {
                        ns.Add(reader["NSerie"].ToString());
                    }
                }


            }

            return ns;
        }
        
        public List<TPS> Get_TP(string ns)
        {
            
            List<TPS> tps = new List<TPS>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("Get_TestPlans", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NSerie", ns);
                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        TPS tp = new TPS
                        {
                            TP_ID = reader.GetInt32(0),
                            T_N = reader.GetInt32(1),
                            Version = reader.GetInt32(2),
                            Nombre = reader.GetString(3),
                            LI = Convert.ToDouble(reader.GetDecimal(4)),
                            LS = Convert.ToDouble(reader.GetDecimal(5)),
                            Unit = reader.GetString(6)
                        };
                        tps.Add(tp);
                       
                   }
                   return tps;
                }
            }

        }


        public DataTable GetFilteredResults(
        int operationNumber,
        string nSerie,
        DateTime startDate,
        DateTime endDate,
        string status,
        string model)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_Filter", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@OperationNumber", operationNumber);
                cmd.Parameters.AddWithValue("@NS", nSerie ?? "");
                cmd.Parameters.AddWithValue("@S_Date", startDate.Date);
                cmd.Parameters.AddWithValue("@End_Date", endDate.Date);
                cmd.Parameters.AddWithValue("@Stat", status ?? "");
                cmd.Parameters.AddWithValue("@model", model ?? "");

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        public void insert(string ns, int tp, decimal res, string stat, DateTime date)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_InsRes", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                
                cmd.Parameters.AddWithValue("@NS", ns );
                cmd.Parameters.AddWithValue("@TP_ID", tp);
                cmd.Parameters.AddWithValue("@res", res);
                cmd.Parameters.AddWithValue("@status", stat);
                cmd.Parameters.AddWithValue("@DateTime", date);

                connection.Open();
                cmd.ExecuteNonQuery();  
            }

        }




    }
}
