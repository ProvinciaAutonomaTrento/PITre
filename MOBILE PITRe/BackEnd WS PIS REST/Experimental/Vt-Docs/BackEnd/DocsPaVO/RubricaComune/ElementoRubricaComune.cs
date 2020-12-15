using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaVO.RubricaComune
{
    /// <summary>
    /// Classe che mantiene le informazioni di un "ElementoRubrica" docspa in rubrica comune
    /// </summary>
    [Serializable()]
    public class InfoElementoRubricaComune
    {
        /// <summary>
        /// Identificativo univoco che rappresenta l'elemento in rubrica comune
        /// </summary>
        public int IdRubricaComune;
    }

    /// <summary>
    /// Classe per la gestione dei filtri impostati in rubrica docspa
    /// e da inviare a rubrica comune
    /// </summary>
    [Serializable()]
    public class FiltriRubricaComune
    {
        /// <summary>
        /// 
        /// </summary>
        public string Codice;

        /// <summary>
        /// 
        /// </summary>
        public string Descrizione;

        /// <summary>
        /// 
        /// </summary>
        public string Citta;

        /// <summary>
        /// True, indica di ricercare in rubrica la parola intera
        /// </summary>
        public bool RicercaParolaIntera;

        public string Mail;

        public string CodiceFiscale;

        public string PartitaIva;


    }
}
