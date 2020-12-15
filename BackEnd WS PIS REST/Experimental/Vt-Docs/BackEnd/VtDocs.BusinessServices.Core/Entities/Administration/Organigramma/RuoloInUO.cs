using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Administration.Organigramma
{
    /// <summary>
    /// Dati di un ruolo nell'organigramma
    /// </summary>
    [Serializable()]
    public class RuoloInUO
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Codice
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CodiceTipoRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DescrizioneTipoRuolo
        {
            get;
            set;
        }

        /// <summary>
        /// Livello del tipo ruolo
        /// </summary>
        public int Livello
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Responsabile
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Segretario
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int Gerarchia
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DiRiferimento
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CodiceUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DescrizioneUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ClassificaUO
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IdParentUO
        {
            get;
            set;
        }
    }
}
