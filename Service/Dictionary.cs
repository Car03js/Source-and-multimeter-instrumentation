using Service.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SN<T> : I_Dictionary<T>
    {
        // Se crea el diccionario al cual realizaremos las operaciones de historial
        private readonly Dictionary<int, List<string>> _Diccionario =
            new Dictionary<int, List<string>>();
        /// <summary>
        /// Aqui lo cambie para que en el string podamos guardar todos los detalles 
        /// de cuando se calibro, cuando se registro y si es posible, para volver a 
        /// registrar y calibrar para futuras situaciones como cada año
        /// </summary>


        //Dictionary<string, QueueInstrumento<I_Instrumentos>> _Diccionario = new Dictionary<string, QueueInstrumento<I_Instrumentos>>();
        public void AgregarHistorial(int id, string detalle)
        {

            if (!_Diccionario.ContainsKey(id))
            {
                _Diccionario[id] = new List<string>();
            }
            _Diccionario[id].Add(detalle);
        }




        //public Dictionary<string, object> ConsultarHistorial(string id)
        //{
        // return 
        //}
        public string[][] Mostrar()
        {
            //return _Diccionario.Keys.ToArray();
            int count = _Diccionario.Count;
            var _DiccCopy = _Diccionario;
            int x = 0;
            int y = 0;
            string[][] ar = new string[count][];
            foreach (var sign in _DiccCopy)
            {

                ar[x] = new string[(sign.Value.Count) + 1];
                x++;
            }


            x = 0;


            foreach (var item in _Diccionario)
            {


                int c = 1;
                //ar[x] = new string[4];
                foreach (var strObj in item.Value)
                {
                    //ar[x][0] = ($"ID: {item.Key}");
                    //ar[x] = new string[strObj.Length]; // Defino el tamaño del segundo arreglo
                    // Esto nos permite la flexibilidad de que en algun futuro se pueda volver 
                    // a registrar o calibrar para dar mantenimiento
                    ar[x][0] = ($"ID: {item.Key}"); //Donde este dato siempre se mantendra fijo
                    ar[x][c] = ($"Detalle: {strObj}"); // Pero este sera los detalles si se registro o ya se calibro
                    c++;
                    //Lo que sucedera con este arreglo es que
                    // El primer corchete son las id (son la clave / key
                    // El segundo estara reservado para saber si se registro
                    // El tercero es para reconocer si se calibro

                    //Ar[x][0] es la ID
                    //Ar[x][1] es Calibracion registrada a esta fecha
                    //Ar[x][2] es calibracion realizada a esta fecha
                    // Es decir se vera de la siguiente manera dentro de la DGV
                    ///ID            Detalle
                    ///1234          Se registro a esta fecha
                    ///1234          Se Calibro a esta fecha
                    ///2222          Se registro a esta fecha   
                    ///2222          Digamos que para este caso aun no se ha calibrado, por lo tanto 
                    ///strObj.length no lo considerara para la posicion [2]
                }
                x++;
            }

            return ar;
        }

        // Obtiene un valor por ID y clave (O(1))
        /*public bool GetValorPorId(string id, string clave, out object? valor)
        {
            valor = null;
            if (!_Diccionario.TryGetValue(id, out var reg)) return false;
            return reg.TryGetValue(clave, out valor);
        } */

        // (Opcional) Obtener el registro completo por ID
        public bool TryGetRegistro(int id, out List<string> registro)
            => _Diccionario.TryGetValue(id, out registro);

        public int cuenta()
        { return _Diccionario.Count(); }




        //-------------------------------------------------------------------------
        // Enumarator para permitir la iteración sobre el diccionario

        public IEnumerator<KeyValuePair<int, List<string>>> GetEnumerator()
        {
            return _Diccionario.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //?
    }

}

