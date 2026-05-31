using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Ivi.Visa;
using NationalInstruments.Visa;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modbus.Device;
using System.IO.Ports;
using System.ComponentModel;
using System.Data;

namespace Service
{
    public class Services
    {
        private readonly I_Queues<TPS> _Qtp;

        // Lista global de TPs
        List<TPS> TP = new List<TPS>();
        private int _lastTP_ID = 0;
        // Esto es un setter injection // alias Inyeccion por propiedad
        private I_Source _Source;
        private I_Multimeter _Multimeter;
        private SerialPort _port;
        private ModbusSerialMaster _master;
        private I_ProductRepository _productRepository;

        private DataTable _resultadosTable;
        //private BindingList<TP_R> _resultados = new BindingList<TP_R>();

        //--- No lo inyecto-----
        private I_Dictionary<Productos> _dicti;

        //---------------Inyeccion por constructor-----------------
        public Services(I_Queues<TPS> qtp, I_Multimeter multi, I_Source sorce, SerialPort port, I_ProductRepository pr, I_Dictionary<Productos> diccionario)
        {
            _port = port;
            _Qtp = qtp;
            _Multimeter = multi;
            _Source = sorce;
            _productRepository = pr;
            _dicti = diccionario;


            _resultadosTable = new DataTable();
            _resultadosTable.Columns.Add("Test Result Id", typeof(int));
            _resultadosTable.Columns.Add("Series Number", typeof(string));
            _resultadosTable.Columns.Add("Test Plan Id", typeof(int));
            _resultadosTable.Columns.Add("Test Result Value", typeof(decimal));
            _resultadosTable.Columns.Add("Status Code", typeof(string));
            _resultadosTable.Columns.Add("Test Date Time", typeof(DateTime));


        }
        // --------------Coms Database-----------------------------

        public void AddTP()
        {
            List<TPS> tps = GetTPs();
            foreach (var item in tps)
            {
                _Qtp.EnQueue(item);
            }
            _dicti = _productRepository.ProductHistory(); // Agregar todos los NS y modelos
        }
        public List<TPS> GetTPs()
        {
            int newId = 1; // starting ID for new TPS
            TP.Clear();

            List<string> AllNS = _productRepository.GetNS();

            foreach (string ns in AllNS)
            {

                List<TPS> tp = _productRepository.Get_TP(ns);
                TP.AddRange(tp);


            }
            foreach (var item in TP)
            {
                _lastTP_ID++;
                item.TP_ID = _lastTP_ID;


            }


            return TP;
        }


        //----------------------Queue-------------------------------
        /*
            public void RegisterData(TPS tp, TPS tp2, TPS tp3, Modelo mod, Productos prod)
            {

                foreach (var item in _Qtp.ToList())
                {
                    if (item.TP_ID == tp.TP_ID)
                    {
                        throw new InvalidOperationException($"El ID {tp.TP_ID} ya existe en la cola de calibración.");
                    }
                }
                _Qtp.EnQueue(tp);
                _Qtp.EnQueue(tp2);
                _Qtp.EnQueue(tp3);
                //_productRepository.insertTp(tp);
                //_productRepository.insertTp(tp2);
                //_productRepository.insertTp(tp3);
                //_productRepository.insert_Mod_Prod(mod, prod);

            
                // Add diccionario y tablas a la base de datos

        }
        */

        public List<string[]> GetTP()
        {
            List<string[]> lista = new List<string[]>();
            foreach (var item in _Qtp.ToList())
            {
                string[] datos = new string[]
                {
                        item.TP_ID.ToString(),
                        item.T_N.ToString(),
                        item.Version.ToString(),
                        item.Nombre,
                        item.LI.ToString(),
                        item.LS.ToString(),
                        item.Unit
                };
                lista.Add(datos);
            }
            return lista;



        }
        public TPS SEE_TPS()
        {
            return _Qtp.Peek();

        }

        public List<string> PDict(int key)
        {
            List<string> Product; // esta lista contendra el registro buscado
                                  //_dicti = _productRepository.ProductHistory(); 
                                  //Esto lo movi a la linea 65
            _dicti.TryGetRegistro(key, out Product);
            return Product;



        }
        public int SeeTest()
        {
            TPS test = _Qtp.Peek();
            int testcode = 0;
            if (test.Nombre == "Voltage Test")
            {
                testcode = 2;
            }
            else if (test.Nombre == "Motor Vel Test")
            {
                testcode = 3;
            }
            else if (test.Nombre == "Continuity Test")
            {
                testcode = 1;
            }

            else if (test.Nombre == "CCW Test")
            {
                testcode = 4;
            }
            else { throw new InvalidOperationException("No se reconoce la prueba a realizar."); }

            return testcode;
        }
        public string GetSN()
        {
            return _Qtp.Peek().Nombre;
        }
        public TPS Eliminate() // Eliminar de la cola
        {
            return _Qtp.DeQueue();
        }
        public double Test()
        {
            TPS test = _Qtp.Peek(); // Antes tenia DeQueue
            double result = 0.0;
            
            if (test.Nombre == "Voltage Test")
            {
                CCW();
                _Source.Output_ON();

                Thread.Sleep(20); // Esperar a que la fuente se estabilice
                string measurement = _Multimeter.MeasureVdc(15, 0.0001);
                result = ProcesarMediciones(measurement);
                result = Math.Round(result, 4);
                
            }
            
            else if (test.Nombre == "Continuity Test")
            {
                CW();
                _Source.Output_OFF();
                Thread.Sleep(20); // Esperar a que la fuente se estabilice
                string measurement = _Multimeter.MeasureContinuityRaw();
                result = ProcesarMediciones(measurement);
                result = Math.Round(result, 4);
            }

            else if (test.Nombre == "CCW Test")
            {
                CW();
                _Source.Output_ON();

                Thread.Sleep(20); // Esperar a que la fuente se estabilice
                string measurement = _Multimeter.MeasureVdc(15, 0.0001);

                result = ProcesarMediciones(measurement);
                result = Math.Round(result, 4);
            }
            else if (test.Nombre == "Motor Vel Test")
            {
                VelControl();
                _Source.Output_ON()
                ; Thread.Sleep(20); // Esperar a que la fuente se estabilice
                string measurement = _Multimeter.MeasureVdc(15, 0.0001);

                result = ProcesarMediciones(measurement);
                result = Math.Round(result, 4);
            }



            else { throw new InvalidOperationException("No se reconoce la prueba a realizar."); }


            // initiate test with instruments

            return result;

        }
        //case


        //------------------Instrumentos Connection--------------------

        public void ConfigureInstruments(string sourceAddress, string multimeterAddress)
        {
            _Source.Connect_Instrument(sourceAddress);
            _Multimeter.Connect_Instrument(multimeterAddress);
            Thread.Sleep(500); // Esperar a que los instrumentos se conecten
            string message = _Source.Identify();
            string message2 = _Multimeter.Identify();

            Thread.Sleep(100);
            
                if (!message2.Contains("34401A")
                    || !message.Contains("66312A"))
                {
                    _Source.Close_Instrument();
                    _Multimeter.Close_Instrument();
                    throw new ArgumentException("Error connecting to instruments.");
                }
            
            

        }

        public void CloseInstruments()
        {
            _Source.Close_Instrument();
            _Multimeter.Close_Instrument();
        }

        public List<string> ListResources()
        {
            List<string> resources = _Multimeter.ListResources().ToList();
            return resources;
        }
        //result
        public double ProcesarMediciones(string datos)
        {
            //tpr

            List<double> lista = new List<double>();

            // Split por coma, salto de línea y espacios
            string[] tokens = datos.Split(
                new[] { ',', '\n', '\r', ' ' },
                StringSplitOptions.RemoveEmptyEntries
            );

            foreach (var t in tokens)
            {
                if (double.TryParse(t,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double valor))
                {
                    valor = Math.Round(valor, 4);
                    lista.Add(valor);

                }
            }

            //valoresValidos = lista.ToArray();



            return lista.Average(); //Regresa el promedio de la lista
        }

        public void TurnOn(double volt, double curr)
        {
            _Source.Set_Voltage(volt);
            //_Source.Set_Current(curr);
            //Thread.Sleep(20);
            _Source.Output_ON();

        }

        public void TurnOff()
        {
            _Source.Set_Voltage(0);
            //_Source.Set_Current(0);
            //Thread.Sleep(20);
            _Source.Output_OFF();
        }
        // -------------------------------------------------------------
        // -------------------------------------------------------------
        //  -------------------------------------------------------------
        // Modbus comunication section

        public string[] GetPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            return ports;

        }

        public void Connect_SL(string com, int baud)
        {

            _port = new SerialPort(com, baud, Parity.None, 8, StopBits.One);

            _port.Open();

            _master = ModbusSerialMaster.CreateRtu(_port);
            _master.Transport.ReadTimeout = 100;
            _master.Transport.WriteTimeout = 100;

        }
        public void Close_SL()
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }
        }

        private void ActivarRele(int numero, byte slaveId)
        {
            int real = numero - 1;
            _master.WriteSingleCoil(slaveId, (ushort)real, true);
            Thread.Sleep(50);
        }

        private void DesactivarRele(int numero, byte slaveId)
        {
            int real = numero - 1;
            _master.WriteSingleCoil(slaveId, (ushort)real, false);
            Thread.Sleep(50);
        }

        public void deactivateRel()
        {
            DesactivarRele(2, 247);
            DesactivarRele(3, 247);
            Thread.Sleep(20);
            DesactivarRele(1, 247);
            DesactivarRele(5, 247);
            DesactivarRele(8, 247);
        }
        public void CW()
        {
            DesactivarRele(3, 247);
            Thread.Sleep(20);
            DesactivarRele(1, 247);
            DesactivarRele(5, 247);
            DesactivarRele(8, 247);
            Thread.Sleep(20);
            ActivarRele(2, 247);


            //Thread.Sleep(100);
        }
        public void CCW()
        {
            DesactivarRele(2, 247);
            Thread.Sleep(20);


            DesactivarRele(1, 247);
            DesactivarRele(5, 247);
            DesactivarRele(8, 247);
            Thread.Sleep(20);
            ActivarRele(3, 247);



        }
        public void VelControl()
        {
            DesactivarRele(2, 247);
            Thread.Sleep(20);

            DesactivarRele(3, 247);
            Thread.Sleep(20);
            ActivarRele(5, 247);
            ActivarRele(8, 247);
            ActivarRele(1, 247);

        }

        // -------------------------------------------------------------
        // Data Table

        public void Register_TP_R(TP_R r)
        {

            //_resultados.Add(r);
            DataRow row = _resultadosTable.NewRow();
            row["Test Result Id"] = r.TR_ID;
            row["Series Number"] = r.SN;
            row["Test Plan Id"] = r.TP_ID;
            row["Test Result Value"] = r.TR_Value;
            row["Status Code"] = r.StatusCode;
            row["Test Date Time"] = r.Test_Date;
            _resultadosTable.Rows.Add(row);
            //_productRepository.insertTp
            decimal val = Convert.ToDecimal(r.TR_Value);
            _productRepository.insert(r.SN, r.TP_ID, val, r.StatusCode, r.Test_Date);

        }
        public DataTable GetDataTable()
        {
            return _resultadosTable;
        }


        public DataTable SearchResults(int opsN, string nSerie, DateTime startDate,
        DateTime endDate, string status, string model)
        {
            return _productRepository.GetFilteredResults(opsN +1, nSerie,  startDate,
             endDate,  status,  model);
        }

        public int QueueSize()
        {
            return _Qtp.Size();
        }
    }




}
