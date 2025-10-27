// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Amministrazione
{
    public class DocumentoStatoFinaleManager
    {
        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string id_Aoo_RF, string annoDa, string annoA, string numeroDa, string numeroA, bool sbloccati, string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idOggetto, id_Aoo_RF, annoDa, annoA, numeroDa, numeroA, sbloccati, IdAmministrazione);
        }

        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idTemplates, string idOggetto, string id_Aoo_RF, string anno, string numero, bool sbloccati, string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idTemplates, idOggetto, id_Aoo_RF, anno, numero, IdAmministrazione, sbloccati);
        }

        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idTemplate, string anno, bool sbloccati, string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idTemplate, anno, sbloccati, IdAmministrazione);
        }

        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string idDocumento, string anno, string idRegistro, bool sbloccati, string IdTipologia, bool Protocollati, string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idDocumento, anno, idRegistro, sbloccati, IdTipologia, Protocollati, IdAmministrazione);
        }

        public static bool ModificaDocumentoStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale[] infoModifica)
        {
            bool retValue = false;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {

                foreach (DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale mod in infoModifica)
                {
                    using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                        retValue = amm.ModificaDocumentoStatoFinale(infoUtente, mod);

                }



                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
        }

    }
}
