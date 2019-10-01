using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Amministrazione
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DocumentoStatoFinaleManager
    {
        /// <summary>
        /// 
        /// </summary>
        private DocumentoStatoFinaleManager()
        { }

        /// <summary>
        /// ritorna la lista dei documenti in stato finale
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idDocumento"></param>
        /// <param name="anno"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(
                        DocsPaVO.utente.InfoUtente infoUtente,
                        string idDocumento, string anno, string idRegistro, bool sbloccati, string IdTipologia, bool Protocollati,string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idDocumento, anno, idRegistro,sbloccati,IdTipologia,Protocollati,IdAmministrazione);
        }


        /// <summary>
        /// overloading per ricerca x tipologia
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idOggetto"></param>
        /// <param name="id_Aoo_RF"></param>
        /// <param name="annoDa"></param>
        /// <param name="annoA"></param>
        /// <param name="numeroDa"></param>
        /// <param name="numeroA"></param>
        /// <param name="sbloccati"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.DocumentoStatoFinale[]  GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string id_Aoo_RF, string annoDa, string annoA, string  numeroDa, string numeroA,bool sbloccati,string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idOggetto,id_Aoo_RF, annoDa, annoA, numeroDa, numeroA,sbloccati,IdAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idTemplate"></param>
        /// <param name="anno"></param>
        /// <param name="sbloccati"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente,string idTemplate ,string anno ,bool sbloccati,string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idTemplate, anno, sbloccati,IdAmministrazione);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idOggetto"></param>
        /// <param name="id_Aoo_RF"></param>
        /// <param name="anno"></param>
        /// <param name="numero"></param>
        /// <param name="sbloccati"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(DocsPaVO.utente.InfoUtente infoUtente,string idTemplates ,string idOggetto, string id_Aoo_RF, string anno, string numero, bool sbloccati,string IdAmministrazione)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetDocumentiStatoFinale(infoUtente, idTemplates,idOggetto, id_Aoo_RF, anno, numero,IdAmministrazione, sbloccati);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="infoUtente"></param>
        ///// <param name="idDocumento"></param>
        ///// <param name="anno"></param>
        ///// <param name="idRegistro"></param>
        ///// <returns></returns>
        //public static DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinaleSbloccati(
        //    DocsPaVO.utente.InfoUtente infoUtente,
        //                string idDocumento, string anno, string idRegistro)
        //{
        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="infoModifica"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="infoModifica"></param>
        //public static bool BloccaDocumentoStatoFinale(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale infoModifica)
        //{
        //    bool retValue = false;

        //    using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
        //    {

        //        if (retValue)
        //            transactionContext.Complete();
        //    }

        //    return retValue;
        //}
    }
}
