using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.utente;
using DocsPaUtils;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Questa classe parziale, mette a disposizione una serie di metodi per la funzionalità di trasmissione
    /// parziale di un documento predisposto ricevuto per interoperabilità interna.
    /// </summary>
    public partial class Interoperabilita
    {
        /// <summary>
        /// Metodo per la selezione dei destinatari della trasmissione per interoperabilità interna selezionati
        /// in base al destinatario della spedizione.
        /// </summary>
        /// <param name="ds">Informazioni sui destinatari della trasmissioni</param>
        /// <param name="reg">AOO destinataria della spedizione</param>
        /// <param name="uoId">Id della UO destinataria della spedizione</param>
        /// <remarks>
        /// La differenza fra questo metodo ed il metodo <see cref="getRuoliDestTrasm" /> è che questo restituisce i
        /// ruoli sotto la UO destinataria mentre l'altro restituisce tutti quelli abilitati alla notifica all'interno della AOO
        /// </remarks>
        public void GetSelectiveRecipients(out DataSet ds, Registro reg, String uoId)
        {
            // Viene utilizzata la stessa query del metodo getRuoliDestTrasm
            Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RUOLI_VIS_CONSULTA_PEC"); ;
            q.setParam("idRegistro", reg.systemId);
            // Il parametro email viene valorizzato in modo che prenda in considerazione anche l'id della UO destinataria della
            // spedizione
            q.setParam("email", String.Format(" AND V.var_email_registro {0} and a.id_uo = {1}", this.dbType.ToUpper() == "ORACLE" ? "is null" : "=''", uoId));
           
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out ds, "RUOLI", queryString);
        }
    }
}
