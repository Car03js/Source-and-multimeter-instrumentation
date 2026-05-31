using Controlador.ControladorService;
using Controlador.ControladorService.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controlador
{
    public class Servicio
    {
        private readonly I_QUEUE<TP> _Qtp;

        public Servicio (I_QUEUE<TP> qtp)
        {
            _Qtp = qtp;
        }
        public void RegisterData(TP tp)
        {




            foreach (var item in _Qtp.ToList())
            {
                if (item.TP_ID == tp.TP_ID)
                {
                    throw new InvalidOperationException($"El ID {tp.TP_ID} ya existe en la cola de calibración.");
                }
            }




        }



    }
}
