// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDocumentale.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale.Documentale
{
    public class FullTextSearchManager : IFullTextSearchManager
    {
        private static Type _type = null;

        protected IFullTextSearchManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IFullTextSearchManager _instance = null;

        public FullTextSearchManager(DocsPaVO.utente.InfoUtente infoUtente)
        {
            this._instance = (IFullTextSearchManager)Activator.CreateInstance(_type, infoUtente);
        }

        public ArrayList FullTextSearch(string testo, string idReg, int numPage, out int numTotPage, out int nRec)
        {
            return this.Instance.FullTextSearch(testo, idReg, numPage, out numTotPage, out nRec);
        }

        public ArrayList FullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            return this.Instance.FullTextSearch(ref context);
        }

        /// <summary>
        /// Necessario per impostare il documento come indicizzato
        /// nel documentale ai fini della ricerca fulltext
        /// </summary>
        /// <param name="docnumber"></param>
        /// <returns></returns>
        public bool SetDocumentAsIndexed(string docnumber)
        {
            return this.Instance.SetDocumentAsIndexed(docnumber);
        }

        public int GetMaxRowCount()
        {
            return this.Instance.GetMaxRowCount();
        }

        /// <summary>
        /// Restituisce la lista dei version_id dei documenti trovati nella ricerca fulltext
        /// Metodo primitivo da raffinare
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ArrayList simpleFullTextSearch(ref DocsPaVO.ricerche.FullTextSearchContext context)
        {
            return this.Instance.simpleFullTextSearch(ref context);
        }


    }
}
