using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.LibroFirma
{
    [XmlInclude(typeof(DocsPaVO.filtri.LibroFirma.listaArgomentiProcessoFirma))]
    [XmlInclude(typeof(DocsPaVO.filtri.LibroFirma.OrderBy))]
    public class FiltroProcessoFirma
    {
        #region private fields

        private string _argomento;
        private string _valore;

        #endregion

        #region Public property

        public DocsPaVO.filtri.LibroFirma.listaArgomentiProcessoFirma listaFiltriProcessoFirma;
        public DocsPaVO.filtri.LibroFirma.OrderBy ordinamentoProcessoFirma;

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
