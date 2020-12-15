using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPaDocumentale.Interfaces
{
    /// <summary>
    /// Interfaccia per la gestione della ricerca fulltext
    /// </summary>
    public interface IFullTextSearchManager
    {
        /// <summary>
        /// Ricerca full text
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="idReg"></param>
        /// <param name="infoUtente"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <returns></returns>
        System.Collections.ArrayList FullTextSearch(string testo,
                                                    string idReg,
                                                    int numPage,
                                                    out int numTotPage,
                                                    out int nRec);
        /// <summary>
        /// Ricerca fulltext
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        System.Collections.ArrayList FullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context);

        /// <summary>
        /// Necessario per impostare il documento come indicizzato
        /// nel documentale ai fini della ricerca fulltext
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        bool SetDocumentAsIndexed(string docnumber);

        /// <summary>
        /// Reperimento numero di record massimo che è possibile 
        /// far restituire dalla ricerca fulltext
        /// </summary>
        /// <returns></returns>
        int GetMaxRowCount();

        ///<summary>
        ///Ricerca semplice che ritorna una lista di identificativi dei documenti trovati
        ///in prima battuta gli identificativi sono i version_id
        ///</summary>
        System.Collections.ArrayList simpleFullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context);
    }
}