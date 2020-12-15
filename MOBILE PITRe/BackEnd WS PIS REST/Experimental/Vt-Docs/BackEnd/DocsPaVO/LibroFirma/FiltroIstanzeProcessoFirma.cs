using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.LibroFirma
{
    [Serializable()]
    [XmlInclude(typeof(DocsPaVO.filtri.ricerca.listaArgomenti))]
    [XmlInclude(typeof(DocsPaVO.filtri.LibroFirma.listaArgomenti))]

    public class FiltroIstanzeProcessoFirma
    {
        #region private fields

        private string _argomento;
        private string _valore;

        #endregion

        #region Public property

        public DocsPaVO.filtri.LibroFirma.listaArgomenti listaIstanzeProcessoFirma;

        public string Argomento
        {
            get
            {
                return _argomento;
            }
            set
            {
                _argomento = value;
            }
        }

        public string Valore
        {
            get
            {
                return _valore;
            }
            set
            {
                _valore = value;
            }
        }
        #endregion
    }
}
