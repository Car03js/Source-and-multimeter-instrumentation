using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface I_Queues<T> : IEnumerable<T>
    {
        void EnQueue(T item);  // Agrega un elemento de tipo T
        T DeQueue();           // Elimina y devuelve el primer elemento
        T Peek();              // Devuelve el primer elemento sin eliminarlo
        bool IsEmpty();        // Indica si la cola está vacía
        int Size();            // Devuelve el tamaño de la cola
    }
}
