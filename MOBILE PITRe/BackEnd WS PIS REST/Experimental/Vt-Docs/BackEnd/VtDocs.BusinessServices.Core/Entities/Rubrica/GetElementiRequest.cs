using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Rubrica
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetElementiRequest : Request
    {
        /// <summary>
        /// 
        /// </summary>
        public GetElementiRequest()
        {
            this.TipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
            this.IdRegistri = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> IdRegistri
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DocsPaVO.addressbook.TipoUtente TipoUtente
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FiltroPerCodice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FiltroPerDescrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FiltroPerCitta
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FiltroPerLocalita
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraUtenti
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraRuoli
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraRF
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraListe
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FiltraRubricaComune
        {
            get;
            set;
        }
    }
}
