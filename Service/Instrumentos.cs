using Service.Interfaces;
using System;
using Ivi.Visa;
using NationalInstruments.Visa;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Service
{
    public abstract class Instrumentos : I_Instruments, IDisposable
    {
        private string _resourceName;
        private ResourceManager rm;
        protected MessageBasedSession mbSession;

        public void Close_Instrument()
        {
            if (mbSession != null)
            {
                mbSession.Dispose();
                mbSession = null;
            }

            if (rm != null)
            {
                rm.Dispose();
                rm = null;
            }
        }
        public void Dispose()
        {
            Close_Instrument();
        }

        public void Connect_Instrument(string resourceName)
        {
            try
            {
                rm = new ResourceManager();
                mbSession = (MessageBasedSession)rm.Open(resourceName);
                //Console.WriteLine($"Connected to {resourceName}");
                mbSession.ReadStatusByte();

                mbSession.TimeoutMilliseconds = 5000;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to instrument: " + ex.Message);
            }
        }

        public void Write(string command)
        {
            if (mbSession != null)
            {
                mbSession.RawIO.Write(command);
            }
            else
            {
                throw new InvalidOperationException("Instrument is not connected.");
            }
        }

        public string Read()
        {
            if (mbSession != null)
            {
                return mbSession.RawIO.ReadString(50000);
            }
            else
            {
                throw new InvalidOperationException("Instrument is not connected.");
            }
        }

        public string Query(string command)
        {
            if (mbSession != null)
            {

                Write(command);
                return Read();
            }
            else
            {
                throw new InvalidOperationException("Instrument is not connected.");
            }
        }

        public string[] ListResources()
        {
            using (var resourceManager = new ResourceManager())
            {
                var resources = resourceManager.Find("?*INSTR");
                return resources.ToArray();
            }
        }

        public string Identify()
        {
            return Query("*IDN?");
        }




    }
    public class Source : Instrumentos, I_Source
    {
        
        public void Output_OFF()
        {
            Write("OUTP OFF");
        }
        public void Output_ON()
        {
            Write("OUTP ON");
        }
        public void Set_Current(double current)
        {
            Write($"SOUR:CURR {current}");
        }
        public void Set_Voltage(double voltage)
        {
            Write($"SOUR:VOLT {voltage}");
        }
    }

    public class Multimeter : Instrumentos, I_Multimeter
    {
        public string MeasureIdc(double range, double resolution)
        {
            Write($"CONF:CURR:DC {range},{resolution}");
            Write("SAMPLE:COUNT 30");
            return Query("READ?");
        }
        public string MeasureRes2W(double range, double resolution)
        {
            Write($"CONF:RES {range},{resolution}");
            Write("SAMPLE:COUNT 30");
            return Query("READ?");
        }
        public string MeasureVdc(double range, double resolution)
        {
            Write($"CONF:VOLT:DC {range},{resolution}");
            Write("SAMPLE:COUNT 30");
            return Query("READ?");
        }
        public string MeasureContinuityRaw()
        {
            Write("CONF:CONT");
            Write("SAMPLE:COUNT 30");
            return Query("READ?");
        }
    }
}
