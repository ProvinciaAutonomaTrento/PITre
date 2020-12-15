using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using DocsPaUtils;
using DocsPaVO.filtri;
using DocsPaVO.filtri.trasmissione;
using DocsPaVO.Report;
using DocsPaVO.utente;
using DocsPaVO.Spedizione;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe è responsabile della generazione dei dati per l'export dei risultati della ricerca
    /// modelli di trasmissione.
    /// </summary>
    [ReportDataExtractorClass()]
    public class ElencoSpedizioni
    {
        /// <summary>
        /// Metodo per la generazione del report relativo ai modelli di trasmissione
        /// </summary>
        /// <param name="userInfo">Informazioni sull'utente</param>
        /// <param name="searchFilter">Lista dei filtri da utilizzare per ricercare modelli di trasmissione che soddisfano determinati criteri</param>
        /// <returns>Dataset con i risultati restituiti dalla ricerca</returns>
        [ReportDataExtractorMethod(ContextName = "ReportSpedizioni")]
        public DataSet GetModelliTrasmissioneReport(InfoUtente userInfo, List<FiltroRicerca> searchFilter)
        {
            List<FiltriReportSpedizioni> filtriSpedizioni = new List<FiltriReportSpedizioni>();
            Interoperabilita interop = new Interoperabilita();
            if (string.IsNullOrEmpty(searchFilter[0].listaFiltriSpedizioni.IdDocumento))
                return MappingListSpedizioni(interop.GetReportSpedizioni(searchFilter[0].listaFiltriSpedizioni, userInfo.idPeople, userInfo.idGruppo));
            else
            {
                return MappingListSpedizioni(interop.GetReportSpedizioniDocumento(searchFilter[0].listaFiltriSpedizioni));
            }
        }



#region Private Method


        private DataSet MappingListSpedizioni( List<InfoDocumentoSpedito>  listInfoDocSpedito)
        {
            if (listInfoDocSpedito.Count == 0) return null;

            DataSet resultDataSet = new DataSet("DataSet_ListaSpedizioni");
            DataTable dtSpedizioni = new DataTable("DataTable_ListaSpedizioni");
            //aggiunta colonne 
            dtSpedizioni.Columns.Add(new DataColumn("PROTOCOLLO"));
            dtSpedizioni.Columns.Add(new DataColumn("OGGETTO"));
            dtSpedizioni.Columns.Add(new DataColumn("NOMINATIVO_DESTINATARIO"));
            dtSpedizioni.Columns.Add(new DataColumn("TIPO_DESTINATARIO"));
            dtSpedizioni.Columns.Add(new DataColumn("MEZZO_SPEDIZIONE"));
            dtSpedizioni.Columns.Add(new DataColumn("MAIL_MITTENTE"));
            dtSpedizioni.Columns.Add(new DataColumn("MAIL_DESTINATARIO"));
            dtSpedizioni.Columns.Add(new DataColumn("DATA_SPEDIZIONE"));
            dtSpedizioni.Columns.Add(new DataColumn("ACCETTAZIONE"));
            dtSpedizioni.Columns.Add(new DataColumn("CONSEGNA"));
            dtSpedizioni.Columns.Add(new DataColumn("CONFERMA"));
            dtSpedizioni.Columns.Add(new DataColumn("ANNULLAMENTO"));
            dtSpedizioni.Columns.Add(new DataColumn("ECCEZIONE"));
            dtSpedizioni.Columns.Add(new DataColumn("AZIONE_INFO"));
            //aggiunta righe
            foreach (InfoDocumentoSpedito infoDoc in listInfoDocSpedito)
                if (infoDoc.Spedizioni != null)
                    foreach (InfoSpedizione infoSpedizione in infoDoc.Spedizioni)
                    {
                        DataRow drSpedizione = dtSpedizioni.NewRow();
                        drSpedizione[0] = infoDoc.Protocollo;            //PROTOCOLLO
                        drSpedizione[1] = infoDoc.DescrizioneDocumento;  //OGGETTO
                        drSpedizione[2] = infoSpedizione.NominativoDestinatario; //NOMINATIVO DESTINATARIO
                        drSpedizione[3] = infoSpedizione.TipoDestinatario; //TIPO DESTINATARIO
                        drSpedizione[4] = infoSpedizione.MezzoSpedizione ; //MEZZO SPEDIZIONE
                        drSpedizione[5] = infoSpedizione.EMailMittente; //E-MAIL MITTENTE
                        drSpedizione[6] = infoSpedizione.EMailDestinatario ; //E-MAIL DESTINATARIO
                        drSpedizione[7] = infoSpedizione.DataSpedizione ; //DATA SPEDIZIONE
                        //drSpedizione[7] = infoSpedizione.TipoRicevuta_Accettazione.ToString() ; //TIPO RICEVUTA - ACCETTAZIONE
                        //drSpedizione[8] = infoSpedizione.TipoRicevuta_Annullamento.ToString(); //TIPO RICEVUTA - ANNULLAMENTO
                        //drSpedizione[9] = infoSpedizione.TipoRicevuta_Conferma.ToString(); //TIPO RICEVUTA - CONFERMA
                        //drSpedizione[10] = infoSpedizione.TipoRicevuta_Consegna; //TIPO RICEVUTA - CONSEGNA
                        //drSpedizione[11] = infoSpedizione.TipoRicevuta_Eccezione; //TIPO RICEVUTA - ECCEZIONE
                        drSpedizione[8] = (infoSpedizione.TipoRicevuta_Accettazione.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Accettazione.ToString()); 
                        drSpedizione[9] = (infoSpedizione.TipoRicevuta_Consegna.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Consegna.ToString());
                        drSpedizione[10] = (infoSpedizione.TipoRicevuta_Conferma.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Conferma.ToString());
                        drSpedizione[11] = (infoSpedizione.TipoRicevuta_Annullamento.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Annullamento.ToString());
                        drSpedizione[12] = (infoSpedizione.TipoRicevuta_Eccezione.ToString().Equals("AttendereCausaMezzo") ? "--" : infoSpedizione.TipoRicevuta_Eccezione.ToString());
                        drSpedizione[13] = (infoSpedizione.Azione_Info.ToString().Equals("Rispedire") ? "Verificare e Rispedire" : infoSpedizione.Azione_Info.ToString());
                        dtSpedizioni.Rows.Add(drSpedizione);
                    }
            resultDataSet.Tables.Add(dtSpedizioni);
            resultDataSet.AcceptChanges();
            return resultDataSet;
        }

#endregion 

    }
}
