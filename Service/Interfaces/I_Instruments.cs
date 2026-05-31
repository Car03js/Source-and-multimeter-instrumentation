using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface I_Instruments
    {
        void Connect_Instrument(string resourceName);
        void Close_Instrument();
        void Write(string command);
        string Read();
        string Query(string command);
        string[] ListResources();
        string Identify();
    }
    public interface I_Source : I_Instruments
    {
        void Output_ON();
        void Output_OFF();
        void Set_Voltage(double voltage);
        void Set_Current(double current);

    }

    public interface I_Multimeter : I_Instruments
    {
        string MeasureVdc(double range, double resolution);
        string MeasureIdc(double range, double resolution);
        string MeasureRes2W(double range, double resolution);
        string MeasureContinuityRaw();


    }
}
