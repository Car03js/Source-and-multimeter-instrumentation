using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controlador.ControladorService
{
    public class PCB
    {
        public PCB(string Serie, string Char) {
        
            NumeroSerie = Serie;
            Model_Name = Char;
        }

        public int ID_PCB { get; set; }
        public string NumeroSerie {  get; set; }
        public string Model_Name {  get; set; }
    }

    public class Modelo
    {
        public Modelo(string name) {

            Nombre = name;
        }
        public int M_ID { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public int TP_ID { get; set; }
    }

    public class T
    {
        public T(int ID, int mId, string nombre, decimal li, decimal ls, string unit, DateTime date)
        {
            TP_ID = ID;
            M_ID = mId;
            Nombre = nombre;
            LI = li;
            LS = ls;
            Unit = unit;
            Reg_Date = date;
        }
        internal int TP_ID { get; set; }

        private int M_ID { get; set; } // Modelo ID
        //public string NS { get; set; } = string.Empty;

        private string Nombre {  get; set; } = string.Empty;

        private decimal LI { get; set; }
        private decimal LS { get; set; }
        private string Unit { get; set; }
        private DateTime Reg_Date { get; set; }

        
        



    }

    public class TP_R
    {

        public TP_R(int TRID,string Ns, int mID, string name, decimal meas, decimal li,
            decimal ls, string direct, bool status, DateTime Reg, DateTime Test, string unit) 
        {
            TR_ID = TRID;
            NS = Ns;
            M_ID = mID;
            TP_Name = name;
            Unit = unit;
            Meass_R = meas;
            LI = li;
            LS = ls;
            Motor_Direction = direct;
            Status = status;
            Reg_Date = Reg;
            Test_Date = Test;

        }
        private int TR_ID { get; set; }
        private string NS { get; set; } = string.Empty;
        private int M_ID { get; set; }
        private string TP_Name { get; set; }
        private string Unit { get; set; }
        private decimal Meass_R { get; set; }
        private decimal LI { get; set; }
        private decimal LS { get; set; }
        private string  Motor_Direction { get; set; }
        private bool Status { get; set; }

        private DateTime Reg_Date { get; set; }

        private DateTime Test_Date { get; set; }

    }
}
