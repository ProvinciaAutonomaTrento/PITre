// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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

        public string Note;

        public string CodiceFiscale;

        public string PartitaIva;

        public List<RubricaEsterna> RubricaEsterna;

        /// <summary>
        /// Indica se ricercare solo nelle rubriche esterne
        /// </summary>
        public bool SoloRubricaEsterna = false;

    }
    public enum RubricaEsterna
    {
        IPA
    }
}
