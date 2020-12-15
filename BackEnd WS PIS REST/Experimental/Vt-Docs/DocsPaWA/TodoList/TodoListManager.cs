using System;

namespace DocsPAWA.TodoList
{
    /// <summary>
    /// Summary description for TodoListManager.
    /// </summary>
    public class TodoListManager
    {
        private string _idAmm = string.Empty;
        private string _noticeDays = string.Empty;
        private string _countTxOverNoticeDays = string.Empty;
        private string _tipoObjTrasm = string.Empty;
        private string _datePost = string.Empty;
        private bool _noWF = false;
        private DocsPAWA.DocsPaWR.Ruolo _ruoloUtente;
        private DocsPAWA.DocsPaWR.Utente _infoUtente;

        public TodoListManager()
        {

        }

        public TodoListManager(DocsPAWA.DocsPaWR.Ruolo ruoloUtente, DocsPAWA.DocsPaWR.Utente infoUtente, string tipoObjTrasm, bool noWF)
        {
            this._ruoloUtente = ruoloUtente;
            this._infoUtente = infoUtente;
            this._idAmm = infoUtente.idAmministrazione;
            this._tipoObjTrasm = tipoObjTrasm;
            this._noWF = noWF;
        }

        public bool isNoticeActived(out string _noticeDays, out string _countTxOverNoticeDays, out string _datePost)
        {
            _noticeDays = string.Empty;
            _countTxOverNoticeDays = string.Empty;
            _datePost = string.Empty;
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();

            return ws.isNoticeActivedTDL(this._infoUtente, this._ruoloUtente, this._tipoObjTrasm, out _noticeDays, out _countTxOverNoticeDays, out _datePost);
        }

        public DocsPaWR.EsitoOperazione svuotaTDL(string dataImpostata, DocsPAWA.DocsPaWR.FiltroRicerca[] filtri)
        {
            DocsPaWR.EsitoOperazione esito = new DocsPAWA.DocsPaWR.EsitoOperazione();
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ws.Timeout = System.Threading.Timeout.Infinite;
            esito = ws.SvuotaTDL(this._infoUtente, this._ruoloUtente, dataImpostata, this._tipoObjTrasm, this._noWF, filtri);

            return esito;
        }

    }
}
