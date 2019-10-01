using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class InputMarca
    {
        /// <summary>
        /// codice applicazione chiamante
        /// </summary>
        public string applicazione;
        /// <summary>
        /// rappresentazione in stringa esadecimale del file sul quale apporre la marca temporale
        /// </summary>
        public string file_p7m;
        /// <summary>
        /// nome utente che ha inviato il documento sul quale apporre la marca temporale
        /// </summary>
        public string riferimento;
    }
}
