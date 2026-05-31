using Modbus.Device;
using NationalInstruments.Restricted;
using NationalInstruments.UI;
using Service;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace Proyecto_Final_Interfaces
{
    public partial class Form1 : Form
    {
        private I_Queues<TPS> Q_TP; // Cola de todos los testPlans

        private I_Dictionary<Productos> NS_Dicc; // Diccionario
        // Idealmente este contiene todos los elementos de los reportes al inicio
        private Services _service; //  Controlador o servicio
        private I_Source _source; //    Fuente
        private I_Multimeter _multimeter; // Multimetro
        private SerialPort _Port; // Puerto para modbus
        private I_ProductRepository _pr; // Repositorio de productos alias SQL

        private DataTable dt; // Datatalbes
        //BindingList<TP_R> bl;


        // IMPORTANTE AGREGAR ESTO
        /// <summary>
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>

        List<string> Prod = new List<string>();
        int NetTP_RId = 1;
        int TPS_ID = 1;
        int Key = 1;
        
        Thread meassureI;
        private bool clicked = false;
        int test = 0;

        string ns;
        double VMAX; double VMIN; double RMAX; double RMIN;



        string connectionString;

        //-----------
        
        
        public Form1()
        {
            InitializeComponent();

            //Initialize Queue
            lbl_hora.Text = DateTime.Now.ToLongTimeString();
            dt = new DataTable();
            dt.Columns.Add("TestResultId", typeof(int));
            dt.Columns.Add("Nserie", typeof(string));
            dt.Columns.Add("TestPlanId", typeof(int));
            dt.Columns.Add("TestResultValue", typeof(decimal));
            dt.Columns.Add("StatusCode", typeof(string));
            dt.Columns.Add("TestDateTime", typeof(DateTime));

            //CB_MODEL
            Q_TP = new Queue_TP<TPS>();
            _source = new Source();
            _multimeter = new Multimeter();
            
            //SW_State.Enabled = false;
            //SW_State.Value = false;
            CB_MODEL.SelectedIndex = 0;
            CB_Ver.SelectedIndex = 0;
            
            TB_VMAX.Enabled = false;
            TB_VMIN.Enabled = false;
            TB_RMAX.Enabled = false;
            TB_RMIN.Enabled = false;
            V_Vel.Value = 0;
            V_MOTOR.Value = 0;
            meter2.Value = 0;

            CB_Search.SelectedIndex = 0;
            CB_MOD.SelectedIndex = 0;
            CB_Status.SelectedIndex = 0;
            TB_Sns.Text = "SN9999A";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Establece la direccion de conexion
            string filePath = AppContext.BaseDirectory + "configuration.xml";
            XDocument doc = XDocument.Load(filePath);
            
            connectionString = doc.Root.Element("database")
                .Element("connectionString")
                .Value;

            DGV_Search.AutoGenerateColumns = true;

            _pr = new ProductoRepository(connectionString); // Creo el objeto
            
            // Inicializo el constructor
            _service = new Services(Q_TP, _multimeter, _source, _Port, _pr, NS_Dicc);
            
            
            //label8.Text = _service.NS("Andy");
            T_Time.Interval = 1000;
            T_Time.Start();
            try { Refresh_Loc();}
            catch (Exception ex)
            { MessageBox.Show("Error: " + ex); }

            // Change
            
        }

        private void button2_Click(object sender, EventArgs e) // BT_REG
        {
        }

        private void BT_REG_DATA_Click(object sender, EventArgs e)
        {
            try
            {
                _service.AddTP();
                Update_Queue_Display();
                MessageBox.Show("Succcesfully loaded TestPlans from database");
                LB_GLOBAL_MESSAGE.Text = "Succcesfully loaded TestPlans from database";



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Update_Queue_Display()
        {
            LV_Q.Items.Clear();
            var list = _service.GetTP();
            foreach (var item in list)
            {
                var columns = new ListViewItem(item[0]);
                columns.SubItems.Add(item[1]);
                columns.SubItems.Add(item[2]);
                columns.SubItems.Add(item[3]);
                columns.SubItems.Add(item[4]);
                columns.SubItems.Add(item[5]);
                columns.SubItems.Add(item[6]);
                //columns.SubItems.Add(item[7]);
                LV_Q.Items.Add(columns);

            }
            LB_QUEUE_No.Text = "Queue: " + _service.QueueSize().ToString();

        }
        //test
        private void T_Time_Tick(object sender, EventArgs e)
        {
            lbl_hora.Text = DateTime.Now.ToLongTimeString();
        }

       
        private void BT_T_Start_Click(object sender, EventArgs e)
        {
           try
            {

               
                //_service.Test();
                clicked = true;
                Update_Queue_Display();
                
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);
                LB_GLOBAL_MESSAGE.Text = ex.Message;
            }
            Update_Queue_Display();
        }

        private void lop()
        {
            while (true)
            {
                if (clicked == true)
                {

                    Prod.Clear();
                    Prod = _service.PDict(Key); // Obtiene el diccionario de numeros de serie
                    string ns = Prod.First();
                    string mod = Prod.Last();
                    int m_id = Convert.ToInt32(mod);
                    this.Invoke((MethodInvoker)(() =>
                    {

                        LB_V_MEAS.Text = "Voltage: 0 V";
                        LB_R_MEAS.Text = "Voltage: 0 V";

                    }));
                    for (int x = 1; x <= 4; x++)
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {

                            L_Failure.Value = false;
                            L_Succes.Value = false;
                            L_Con.Value = false;
                            L_Res.Value = false;
                            LED_V_MED.Value = false;
                            L_V_OutRange.Value = false;
                            L_R_OutRanged.Value = false;
                            V_Vel.Value = 0; // Prueva de cambio de velocidad
                            V_MOTOR.Value = 0; // Prueba de alimentacion V1
                            meter2.Value = 0; // Combio de direccion
                            

                        }));
                        string state = "Untested";
                        int TPcode = _service.SeeTest();
                        TPS testing = _service.SEE_TPS();
                        double result = 0;
                        //string ns = testing.Nombre;
                        VMIN = testing.LI;
                        VMAX = testing.LS;

                        int ManyNotTested = 0;
                        int tpID = x;
                        _service.deactivateRel();
                        


                       
                            switch (testing.Nombre)
                            {
                                case "Continuity Test":
                                this.Invoke((MethodInvoker)(() =>
                                {
                                    L_SS.Value = false;
                                    L_Con.Value = true;
                                }));
                                
                                    result = _service.Test();
                                    Thread.Sleep(1500);
                                    if (result < 14)
                                    { state = "PASS";
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        L_Succes.Value = true; L_Failure.Value = false;
                                    }));
                                     }
                                    else
                                    {
                                        state = "FAIL";
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        L_Succes.Value = false; L_Failure.Value = true;
                                    }));
                                    
                                        //MessageBox.Show("Error: Stoped testing.");
                                        ManyNotTested = 3;
                                    }
                                    break;
                                case "Voltage Test":
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        L_SS.Value = true;
                                        LED_V_MED.Value = true;
                                    }));
                                
                                    Thread.Sleep(100);
                                    if (m_id == 1)
                                    { _service.TurnOn(12, 1);}
                                    else { _service.TurnOn(6, 1); }
                                    
                                    result = _service.Test();
                                    _service.TurnOff();
                                    if (VMAX > result && result > VMIN)
                                    {
                                        state = "PASS";
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        V_Success.Value = true;
                                        V_MOTOR.Value = result;
                                        LB_V_MEAS.Text = "Voltage: " + result.ToString() + " V";
                                    }));
                                    Thread.Sleep(1000);
                                        
                                        
                                       
                                    }
                                    //state = "Failed"; L_V_OutRange.Value = true;

                                    else
                                    {
                                        state = "FAILED";
                                    this.Invoke((MethodInvoker)(() =>
                                    {
                                        L_V_OutRange.Value = true;
                                    }));
                                    
                                        if (result <= 15 && result >= 0) {
                                        this.Invoke((MethodInvoker)(() =>
                                        {
                                            V_MOTOR.Value = result; LB_V_MEAS.Text = "Voltage: " + result.ToString() + "V";
                                        }));
                                        }
                                        else {

                                        this.Invoke((MethodInvoker)(() =>
                                        {
                                            V_MOTOR.Value = 0;
                                            LB_V_MEAS.Text = "Voltage is negative";
                                        }));
                                        
                                        }
                                        Thread.Sleep(1000);
                                        ManyNotTested = 2;
                                    }
                                    break;

                                case "CCW Test":
                                    this.Invoke((MethodInvoker)(() => { L_SS.Value = true; L_Res.Value = true; }));
                                    
                                    Thread.Sleep(100);
                                    if (m_id == 1) { _service.TurnOn(12, 1.5); }
                                    else { _service.TurnOn(9, 1.5); }
                                        //_source.Set_Voltage(9);
                                        //_source.Set_Current(1.5);
                                        result = _service.Test();
                                    if ((VMIN) < result && result < (VMAX))
                                    {
                                        state = "PASS";
                                    this.Invoke((MethodInvoker)(() => 
                                    { R_Success.Value = true; meter2.Value = result;
                                        LB_R_MEAS.Text = "Voltage: " + result.ToString() + " V";
                                    }));
                                    
                                        Thread.Sleep(100);
                                    }
                                    else
                                    {
                                    state = "FAIL";
                                    
                                    this.Invoke((MethodInvoker)(() => { L_R_OutRanged.Value = true; }));
                                        if (result >= -15 && result <= 0) {
                                        this.Invoke((MethodInvoker)(() => { meter2.Value = result; LB_R_MEAS.Text = "Voltage: " + result.ToString() + " V"; }));
                                         } // Cambia rangos
                                        else {
                                        this.Invoke((MethodInvoker)(() => { meter2.Value = 0; LB_R_MEAS.Text = "Voltage is positive"; }));
                                        }
                                        ManyNotTested = 1;
                                        Thread.Sleep(100);

                                    }
                                    break;
                                case "Motor Vel Test":
                               
                        
                                    this.Invoke((MethodInvoker)(() => { L_V_VelocControl.Value = true; L_SS.Value = true; }));
                                    if (m_id == 1) { _service.TurnOn(6, 1.5); }
                                    else { _service.TurnOn(9, 1); }

                                    //_source.Set_Voltage(13);
                                    //_source.Set_Current(1.5);

                                    result = _service.Test();
                                    if ((VMAX) > result && result > (VMIN))
                                    {state = "PASS";
                                    this.Invoke((MethodInvoker)(() => { L_Veloc_S.Value = true; V_Vel.Value = result; }));
                                    }
                                    else
                                    {
                                         
                                        state = "FAIL";
                                        this.Invoke((MethodInvoker)(() => { L_Veloc_F.Value = true; }));
                                        if (result <= 15 && result >= 0) { V_Vel.Value = result; LB_V_MEAS2.Text = "Voltage: " + result.ToString(); } // Cambia rangos
                                        else { V_Vel.Value = 0; LB_V_MEAS2.Text = "Voltage is negative"; }
                                        Thread.Sleep(100);
                                        // No se elimina nada de las cola, porque es el ultimo test
                                    }
                                    break;

                            }

                        
                        if (m_id == 2)
                        { tpID += 4; }
                        testing = _service.Eliminate();
                        TP_R tpr = new TP_R(NetTP_RId, ns, tpID, result, state, DateTime.Now);
                        if (tpr.TR_Value > 15) { tpr.TR_Value = 99; }
                        _service.Register_TP_R(tpr);
                        switch (ManyNotTested)
                        {
                            case 3:
                                
                                for (int i = 0; i < 3; i++)
                                { reg(ns, i + 1, tpID + i + 1); }
                                NetTP_RId += 4;
                                x = 4;
                                break;


                            case 2:
                                for (int i = 0; i < 2; i++)
                                { reg(ns, i + 1, tpID + i + 1); }
                                NetTP_RId += 3;
                                x = 4;
                                break;


                            case 1:
                                reg(ns, 1, tpID + 1);
                                NetTP_RId += 2;
                                x = 4;
                                break;

                            case 0:
                                NetTP_RId++;
                                break;
                        }


                    }


                    this.Invoke((MethodInvoker)(() =>
                    {
                        Update_Queue_Display();
                        
                        dt = _service.GetDataTable();
                        Thread.Sleep(50);
                        dt = _service.GetDataTable();
                        DGV_History.DataSource = dt;
                        
                    }));
                    
                    Thread.Sleep(50);
                    //NetTP_RId += 4; // Esto incrementara la ID del test result
                    TPS_ID++; // Esto incementara la ID del test plan
                    Key++;
                    _service.TurnOff();
                    clicked = false;
                    _service.deactivateRel();

                    this.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show("Product test finalized");
                        LB_GLOBAL_MESSAGE.Text = "Product test finalized";
                    }));
                    
                }
                // _service.Eliminate();

            }
        
        }
        
        public void reg(string ns, int i, int tpID)
        {
            TPS testing = _service.Eliminate();
            TP_R tpr = new TP_R(NetTP_RId + i, ns, tpID, 0, "NT", DateTime.Now);
            _service.Register_TP_R(tpr);
            
        }

        private void BT_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                _service.ConfigureInstruments(CB_Source.SelectedItem.ToString(), CB_Multi.SelectedItem.ToString());
                _service.Connect_SL(CB_SL_Port.SelectedItem.ToString(), 9600);

                meassureI = new Thread(lop);
                meassureI.IsBackground = true;
                meassureI.Start();
                MessageBox.Show("Instruments Connected Successfully");
                LB_GLOBAL_MESSAGE.Text = "Instruments Connected Successfully";
                //_source.Output_OFF();
                _service.TurnOff(); // Start with source off
                _service.deactivateRel();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);
                LB_GLOBAL_MESSAGE.Text = ex.Message;
            }
        }
        private void BT_Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _service.TurnOff(); // Turn off source before disconnecting
                

                if (meassureI != null && meassureI.IsAlive)
                    meassureI.Interrupt();
                _service.CloseInstruments();
                _service.Close_SL();
                MessageBox.Show("Successfully disconected Instruments and SeaLevel");
                LB_GLOBAL_MESSAGE.Text = "Successfully disconected Instruments and SeaLevel";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);
                LB_GLOBAL_MESSAGE.Text = ex.Message;
            }
        }

        

        private void BT_RFSH_Click(object sender, EventArgs e)
        {
            try
            {
                Refresh_Loc();
                MessageBox.Show("Successfully refreshed port directions");
                LB_GLOBAL_MESSAGE.Text = "Successfully refreshed port directions";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);
                LB_GLOBAL_MESSAGE.Text = ex.Message;
            }
            
        }

        private void Refresh_Loc()
        {
            
            List<string> list = _service.ListResources();
            CB_Multi.Items.Clear();
            CB_Source.Items.Clear();
            foreach (var item in list)
            {
                if (item.Contains("GPIB"))
                {
                    CB_Multi.Items.Add(item);
                    CB_Source.Items.Add(item);
                }
            }
            
            string[] ports = _service.GetPorts();
            CB_SL_Port.Items.Clear();
            foreach (var port in ports)
            {
                CB_SL_Port.Items.Add(port);
            }

            CB_SL_Port.SelectedIndex = -1;
            CB_SL_Port.SelectedItem = null;
            CB_SL_Port.Text = string.Empty;

            CB_Source.SelectedIndex = -1;
            CB_Source.SelectedItem = null;
            CB_Source.Text = string.Empty;

            CB_Multi.SelectedIndex = -1;
            CB_Multi.SelectedItem = null;
            CB_Multi.Text = string.Empty;
        }

        // --------------------------------------------------------------
        // Modifica los valores de la prueba
        // Alias, cuando se selecciona una version diferente
        //---------------------------------------------------------------
        private void CB_Ver_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(CB_Ver.SelectedIndex)
            {
                case 0: // Version 1
                    UpdateTB(13.525, 11.475, 1000, 100); // limites de voltaje y resistencia
                    break;
                case 1: // Version 2
                    UpdateTB(13.125, 11.875, 2000, 200);
                    break;
                case 2: // Version 3
                    UpdateTB(3.925, 3.515, 4000, 400); 
                    break;
                case 3: // Version 4
                    UpdateTB(1.0025, 0.87, 3000, 300);
                    break;

            }
        }

        private void UpdateTB(double VM, double Vm, double RM, double Rm)
        {

            TB_VMAX.Text = VM.ToString();
            TB_VMIN.Text = Vm.ToString();
            TB_RMAX.Text = RM.ToString();
            TB_RMIN.Text = Rm.ToString();

        }

        private void meter2_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {
            
        }

        private void BT_GEN_HIST_Click(object sender, EventArgs e)
        {
            
            DGV_History.DataSource = dt;
        }

        private void V_MOTOR_AfterChangeValue(object sender, AfterChangeNumericValueEventArgs e)
        {

        }

        private void DTP_Start_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
            
        {
            
        }

        private void BT_Search_Click(object sender, EventArgs e)
        {
            
                string sn = TB_Sns.Text;
                string model = CB_MOD.Text;
                string status = CB_Status.Text;
                DateTime begin = DTP_Start.Value;
                DateTime end = DTP_End.Value;

                DGV_Search.DataSource = _service.SearchResults(CB_Search.SelectedIndex, sn, 
                begin, end, status, model);
                MessageBox.Show("Search Completed");
                LB_GLOBAL_MESSAGE.Text = "Search Completed";






        }
    }
}
