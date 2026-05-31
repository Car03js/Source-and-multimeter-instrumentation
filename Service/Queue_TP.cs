using Service.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Queue_TP<T> : I_Queues<T>
    {
        private readonly List<T> _cola = new List<T>();


        public void EnQueue(T instrumento)
        {
            _cola.Add(instrumento);
        }
        //
        //      
        //  
        public T DeQueue()
        {
            if (!IsEmpty())
            {
                // Como los datos se van a pilando, cuando nosotros eliminemos
                // El primer elemento que entro, sera el primero en salir
                T primero = _cola[0];
                _cola.RemoveAt(0);
                return primero;

            }
            else
            {
                throw new InvalidOperationException("La cola está vacía.");
            }
        }
        public T Peek()
        {
            if (!IsEmpty())
            {

                return _cola[0];
            }
            else
            {
                throw new InvalidOperationException("La cola está vacía.");
            }
        }

        public bool IsEmpty()
        {
            return _cola.Count == 0; // Esto nos permitira utilizarlo en otras partes del codigo
                                     // Especificamente en Peek y DeQueue para evitar errores
        }

        public int Size()
        {
            return _cola.Count;
        }


        public IEnumerator<T> GetEnumerator() => _cola.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    }
}
