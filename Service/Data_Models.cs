using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Productos
    {
        public Productos(string Serie, string Char)
        {

            NumeroSerie = Serie;
            
        }
            
        
        public string NumeroSerie { get; set; }
        public int Model_ID { get; set; } = 0;
        public string Test_Status { get; set; } = string.Empty;
    }
    public class Test_Status
    {
        public Test_Status(string id, string status)
        {
            Status_Code = id;
            Status_Desc = status;
        }
        public string Status_Code { get; set; } = "NOT_TESTED";
        public string Status_Desc { get; set; } = string.Empty;
    }

    public class Modelo
    {
        public Modelo(string name, int tpID)
        {

            Nombre = name;
            TP_ID = tpID;
        }
        public int M_ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        /// //////////////////////public int TP_ID { get; set; }
        public int TP_ID { get; set; } = 0;
    }

    public class TPS //TP
    {
        public TPS(int ID, int tn, int ver, string nombre, double li, double ls, string unit)//DateTime date
        {
            TP_ID = ID;
            T_N = tn; // Test number es por ejemplo, para este test plan son
            // 1 voltaje, 2 resistencia, etc.
            Version = ver;
            Nombre = nombre;
            LI = li;
            LS = ls;
            Unit = unit;
            //Reg_Date = date;
        }
        public TPS() { }
        public int TP_ID { get; set; }= 1;

        public int T_N { get; set; } = 1; // Modelo ID
        //public string NS { get; set; } = string.Empty;
            
        public int Version { get; set; } = 0;

        public string Nombre { get; set; } = string.Empty; // Test Plan Name
        //internal string NS { get; set; } = string.Empty;
        public double LI { get; set; } = 0;
        public double LS { get; set; } = 0;
        public string Unit { get; set; } = string.Empty;
        //internal DateTime Reg_Date { get; set; } 

    }

    public class TP_R
    {

        public TP_R(int id, string ns, int TPid, double val, string statcod, DateTime test)
        { 
            TR_ID = id;
            SN =ns;
            TP_ID = TPid;
            TR_Value = val;
            StatusCode = statcod;
            Test_Date = test;

        }
        public int TR_ID { get; set; }
        public string SN { get; set; } = string.Empty;
        public int TP_ID { get; set; }
        public double TR_Value { get; set; }
        public string StatusCode { get; set; }
        public DateTime Test_Date { get; set; }

    }
}
