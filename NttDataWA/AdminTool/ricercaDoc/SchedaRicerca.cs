using System;
using System.Collections;
using System.Web.UI.WebControls;
using DocsPaUtils.LogsManagement;
using System.Text.RegularExpressions;
using log4net;
using SAAdminTool.DocsPaWR;
using SAAdminTool.utils;

namespace SAAdminTool.ricercaDoc
{
    /// <summary>
    /// Summary description for SchedaRicerca.
    /// </summary>
    public class SchedaRicerca
    {
        public const string SESSION_KEY = "SchedaRicerca";
        private ILog logger = LogManager.GetLogger(typeof(SchedaRicerca));
        private string _searchKey = string.Empty;
        DocsPaWR.Utente utente = null;
        DocsPaWR.Ruolo ruolo = null;
        DocsPaWR.FiltroRicerca[][] filters = null;
        System.Web.UI.Page currentPg = null;
        NuovaRicerca nuovaRic = null;
        string tipo = null;

        public SchedaRicerca()
        {

        }

        public SchedaRicerca(string searchKey)
        {
            this._searchKey = searchKey;
        }

        public SchedaRicerca(string searchKey, SAAdminTool.DocsPaWR.Utente usr, DocsPaWR.Ruolo grp)
            : this(searchKey)
        {
            utente = usr;
            ruolo = grp;
        }

        public SchedaRicerca(string searchKey, SAAdminTool.DocsPaWR.Utente usr, DocsPaWR.Ruolo grp, System.Web.UI.Page pg)
            : this(searchKey)
        {
            utente = usr;
            ruolo = grp;
            currentPg = pg;
        }

        public SAAdminTool.DocsPaWR.Utente Utente
        {
            get { return utente; }
            set { utente = value; }
        }

        public SAAdminTool.DocsPaWR.Ruolo Ruolo
        {
            get { return ruolo; }
            set { ruolo = value; }
        }

        public SAAdminTool.DocsPaWR.FiltroRicerca[][] FiltriRicerca
        {
            get { return filters; }
            set { filters = value; }
        }

        public System.Web.UI.Page Pagina
        {
            get { return currentPg; }
            set { currentPg = value; }
        }

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public NuovaRicerca ProprietaNuovaRicerca
        {
            get { return nuovaRic; }
            set { nuovaRic = value; }
        }

        public string getSearchKey()
        {
            return _searchKey;
        }

        public string gridId
        {
            get;
            set;
        }

        public SAAdminTool.DocsPaWR.FiltroRicerca GetFiltro(string nomeFiltro)
        {
            DocsPaWR.FiltroRicerca filtro = null;

            if (filters == null || filters[0] == null)
                return null;

            ArrayList al = new ArrayList(filters[0]);
            bool found = false;
            int idx = -1;
            for (int i = 0; !found && i < al.Count; i++)
            {
                DocsPaWR.FiltroRicerca fr = (SAAdminTool.DocsPaWR.FiltroRicerca)al[i];
                if (fr != null && fr.argomento == nomeFiltro)
                {
                    idx = i;
                    found = true;
                }
            }
            if (idx != -1)
                filtro = (SAAdminTool.DocsPaWR.FiltroRicerca)al[idx];

            return filtro;
        }

        public void SetFiltro(string nomeFiltro, SAAdminTool.DocsPaWR.FiltroRicerca filtro)
        {
            if (filtro == null)
                return;

            if (filters == null)
                filters = new SAAdminTool.DocsPaWR.FiltroRicerca[1][];

            ArrayList al = null;
            if (filters[0] != null)
                al = new ArrayList(filters[0]);
            else
                al = new ArrayList();

            bool found = false;
            int idx = -1;
            for (int i = 0; !found && i < al.Count; i++)
            {
                DocsPaWR.FiltroRicerca fr = (SAAdminTool.DocsPaWR.FiltroRicerca)al[i];
                if (fr != null && fr.argomento == nomeFiltro)
                {
                    idx = i;
                    found = true;
                }
            }
            if (idx != -1)
                al[idx] = filtro;
            else
                al.Add(filtro);

            filters[0] = new SAAdminTool.DocsPaWR.FiltroRicerca[al.Count];
            al.CopyTo(filters[0]);
        }

        public void RimuoviFiltro(string nomeFiltro)
        {
            if (filters == null || filters[0] == null)
                return;

            ArrayList al = new ArrayList(filters[0]);
            bool found = false;
            int idx = -1;
            for (int i = 0; !found && i < al.Count; i++)
            {
                DocsPaWR.FiltroRicerca fr = (SAAdminTool.DocsPaWR.FiltroRicerca)al[i];
                if (fr != null && fr.argomento == nomeFiltro)
                {
                    idx = i;
                    found = true;
                }
            }
            if (idx != -1)
                al.RemoveAt(idx);

            filters[0] = new SAAdminTool.DocsPaWR.FiltroRicerca[al.Count];
            al.CopyTo(filters[0]);
        }

        public bool verificaNome(string nomeRicerca, SAAdminTool.DocsPaWR.InfoUtente infoUtente, string pagina, string adl)
        {
            bool retValue = false;
            try
            {
                //Permette di non fare il controllo sul nome della ricerca salvata
                if (utils.InitConfigurationKeys.GetValue("0", "FE_VERIFICA_NOME") != null && utils.InitConfigurationKeys.GetValue("0", "FE_VERIFICA_NOME").Equals("1"))
                {
                    retValue = false;
                }
                else
                {
                    DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                    retValue = docspaws.verificaNomeRicerca(nomeRicerca, infoUtente, pagina, adl);
                }
            }
            catch (Exception e)
            {
                logger.Debug("SchedaRicerca.verificaNomeRicerca - Errore nella verifica univocità del nome della ricerca: " + e.Message);
                throw e;
            }
            return retValue;
        }

        public void Salva(string type, bool custumGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType)
        {
            logger.Debug("SchedaRicerca.Salva");
            if (nuovaRic == null)
                return;

            try
            {
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                DocsPaWR.SearchItem item = new SAAdminTool.DocsPaWR.SearchItem();
                item.system_id = 0;
                item.descrizione = nuovaRic.Titolo;
                item.tipo = this.tipo;

                switch (nuovaRic.Condivisione)
                {
                    case NuovaRicerca.ModoCondivisione.Utente:
                        item.owner_idPeople = Int32.Parse(this.utente.idPeople);
                        break;
                    case NuovaRicerca.ModoCondivisione.Ruolo:
                        item.owner_idGruppo = Int32.Parse(this.ruolo.idGruppo);
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(this._searchKey))
                    item.pagina = this.currentPg.ToString();
                else
                    item.pagina = this._searchKey;

                logger.Debug("SchedaRicerca.Salva - Serializzazione dei filtri");
                item.filtri = SchedaRicerca.FiltersToString(this.filters);
                logger.Debug("SchedaRicerca.Salva - Filtri serializzati");

                logger.Debug("SchedaRicerca.Salva - Sto per salvare la ricerca: " + item.ToString());

                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                if ((type != null) && (type == "ricADL"))
                    item = docspaws.SalvaRicerca(item, infoUtente, true, custumGrid, gridId, tempGrid, gridType);
                else
                    item = docspaws.SalvaRicerca(item, infoUtente, false, custumGrid, gridId, tempGrid, gridType);

                nuovaRic.Id = item.system_id;
                logger.Debug("SchedaRicerca.Salva - Ricerca salvata");

                if (custumGrid)
                {
                    if (!string.IsNullOrEmpty(item.gridId))
                    {
                        GridManager.SelectedGrid = GridManager.GetGridFromSearchId(item.gridId, gridType);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.Salva - Errore nel salvataggio della ricerca: " + ex.Message);
                throw ex;
            }
        }

        public void Cancella(int id)
        {
            logger.Debug("SchedaRicerca.Cancella");
            try
            {
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                logger.Debug("SchedaRicerca.Cancella - Sto per cancellare la ricerca: " + id);
                bool ok = docspaws.CancellaRicerca(id);
                logger.Debug("SchedaRicerca.Cancella - Ricerca cancellata");
            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.Cancella - Errore nella cancellazione della ricerca: " + ex.Message);
                throw ex;
            }
        }

        public void Seleziona(int id, out string gridTempId)
        {
            logger.Debug("SchedaRicerca.Seleziona");
            DocsPaWR.SearchItem item = null;
            try
            {
                gridTempId = string.Empty;
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                logger.Debug("SchedaRicerca.Seleziona - Sto per cercare la ricerca: " + id);
                item = docspaws.RecuperaRicerca(id);
                if (item == null)
                {
                    logger.Debug("SchedaRicerca.Seleziona - Ricerca non trovata");
                    throw new Exception("Ricerca non trovata");
                }
                else
                {
                    logger.Debug("SchedaRicerca.Seleziona - Ricerca trovata");
                    if (!string.IsNullOrEmpty(item.gridId))
                    {
                        gridTempId = item.gridId;
                    }
                    logger.Debug("SchedaRicerca.Seleziona - Deserializzazione dei filtri");
                    filters = SchedaRicerca.StringToFilters(item.filtri);
                    logger.Debug("SchedaRicerca.Seleziona - Filtri deserializzati");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.Seleziona - Errore nella selezione della ricerca: " + ex.Message);
                throw ex;
            }
        }

        public ListItem[] ElencoRicerche(string tipo)
        {
            return ElencoRicerche(tipo, false, null);
        }


        public ListItem[] ElencoRicerche(string tipo, System.Web.UI.WebControls.DropDownList ddl)
        {
            return ElencoRicerche(tipo, false, ddl, null);
        }



        public ListItem[] ElencoRicerche(string tipo, System.Web.UI.WebControls.DropDownList ddl, string selVal)
        {
            return ElencoRicerche(tipo, false, ddl, selVal);
        }

        public ListItem[] ElencoRicerche(string tipo, bool allPages)
        {
            return ElencoRicerche(tipo, allPages, null, null);
        }

        public ListItem[] ElencoRicerche(string tipo, bool allPages, System.Web.UI.WebControls.DropDownList ddl)
        {
            return ElencoRicerche(tipo, allPages, ddl, null);
        }

        public ListItem[] ElencoRicercheADL(string tipo, bool allPages, System.Web.UI.WebControls.DropDownList ddl, string selVal)
        {
            string selected = selVal;
            if (ddl != null && selected == null)
                selected = (ddl.SelectedIndex > 0) ? ddl.SelectedValue : null;

            ListItem[] outcome = null;
            ArrayList alOutcome = new ArrayList();
            //logger.Debug("SchedaRicerca.ElencoRicerche");

            try
            {
                //1.Carico dal database la lista delle ricerche per l'utente e ruolo corrente
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                //logger.Debug("SchedaRicerca.ElencoRicerche - Sto per cercare la lista delle ricerche");
                DocsPaWR.SearchItemList itemList = docspaws.ListaRicercheSalvate(Int32.Parse(this.utente.idPeople),
                    Int32.Parse(this.ruolo.idGruppo),
                    (!allPages) ? this._searchKey.ToString() : null, true, tipo);

                if (itemList != null)
                {
                    //logger.Debug("SchedaRicerca.ElencoRicerche - " + itemList.lista.Length + " ricerche trovate");
                    foreach (SAAdminTool.DocsPaWR.SearchItem item in itemList.lista)
                    {
                        ListItem li = new ListItem(item.descrizione, item.system_id.ToString());
                        alOutcome.Add(li);
                    }
                    outcome = new ListItem[alOutcome.Count];
                    alOutcome.CopyTo(outcome);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.ElencoRicerche - Errore nel recupero della lista delle ricerche: " + ex.Message);
                throw ex;
            }

            if (ddl != null)
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("", "0"));
                ddl.Items.AddRange(outcome);
                if (selected != null)
                {
                    bool found = false;
                    for (int i = 1; !found && i < ddl.Items.Count; i++)
                    {
                        ListItem li_ = ddl.Items[i];
                        if (li_.Value == selected)
                        {
                            ddl.SelectedIndex = i;
                            found = true;
                        }
                    }
                }
            }

            return outcome;

        }
        public ListItem[] ElencoRicerche(string tipo, bool allPages, System.Web.UI.WebControls.DropDownList ddl, string selVal)
        {
            string selected = selVal;
            if (ddl != null && selected == null)
                selected = (ddl.SelectedIndex > 0) ? ddl.SelectedValue : null;

            ListItem[] outcome = null;
            ArrayList alOutcome = new ArrayList();
            //logger.Debug("SchedaRicerca.ElencoRicerche");

            try
            {
                //1.Carico dal database la lista delle ricerche per l'utente e ruolo corrente
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                //logger.Debug("SchedaRicerca.ElencoRicerche - Sto per cercare la lista delle ricerche");
                DocsPaWR.SearchItemList itemList = docspaws.ListaRicercheSalvate(Int32.Parse(this.utente.idPeople),
                    Int32.Parse(this.ruolo.idGruppo),
                    (!allPages) ? this._searchKey.ToString() : null, false, tipo);

                if (itemList != null)
                {
                    //logger.Debug("SchedaRicerca.ElencoRicerche - "+itemList.lista.Length+" ricerche trovate");
                    foreach (SAAdminTool.DocsPaWR.SearchItem item in itemList.lista)
                    {
                        ListItem li = new ListItem(item.descrizione, item.system_id.ToString());
                        alOutcome.Add(li);
                    }
                    outcome = new ListItem[alOutcome.Count];
                    alOutcome.CopyTo(outcome);
                }
            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.ElencoRicerche - Errore nel recupero della lista delle ricerche: " + ex.Message);
                throw ex;
            }

            if (ddl != null)
            {
                ddl.Items.Clear();
                ddl.Items.Add(new ListItem("", "0"));
                ddl.Items.AddRange(outcome);
                if (selected != null)
                {
                    bool found = false;
                    for (int i = 1; !found && i < ddl.Items.Count; i++)
                    {
                        ListItem li_ = ddl.Items[i];
                        if (li_.Value == selected)
                        {
                            ddl.SelectedIndex = i;
                            found = true;
                        }
                    }
                }
            }

            return outcome;
        }

        public static string FiltersToString(SAAdminTool.DocsPaWR.FiltroRicerca[][] filtriRicerca)
        {
            try
            {
                //Serializzazione del filtro di ricerca
                System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(filtriRicerca.GetType());
                System.IO.MemoryStream _mStream = new System.IO.MemoryStream();
                System.Xml.XmlTextWriter _writer = new System.Xml.XmlTextWriter(_mStream, System.Text.Encoding.ASCII);
                _serializer.Serialize(_writer, filtriRicerca);

                //A questo punto il mio oggetto serializzato è nello stream
                //estraiamo il buffer dello stream e lo convertiamo in stringa
                string outcome = System.Text.Encoding.ASCII.GetString(_mStream.GetBuffer());

                //Ripuliamo l'oggetto serializzato
                int tmp = outcome.IndexOf("\0");
                outcome = outcome.Remove(tmp, outcome.Length - tmp);

                return outcome;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fallita serializzazione dei criteri di ricerca");
            }
            return null;
        }

        public static SAAdminTool.DocsPaWR.FiltroRicerca[][] StringToFilters(string filtriRicerca)
        {
            DocsPaWR.FiltroRicerca[][] outcome = null;
            try
            {
                Type t = typeof(SAAdminTool.DocsPaWR.FiltroRicerca[][]);
                System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(t);

                System.IO.StringReader sr = new System.IO.StringReader(filtriRicerca);

                System.Xml.XmlTextReader _reader = new System.Xml.XmlTextReader(sr);
                outcome = ((SAAdminTool.DocsPaWR.FiltroRicerca[][])(_serializer.Deserialize(_reader)));
            }
            catch
            {
                Console.WriteLine("Fallita deserializzazione dei criteri di ricerca");
            }
            return outcome;
        }

        public bool verificaNomeModifica(string nomeRicerca, SAAdminTool.DocsPaWR.InfoUtente infoUtente, string pagina, string idRicerca)
        {
            bool retValue = false;
            try
            {
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                retValue = docspaws.verificaNomeRicercaModifica(nomeRicerca, infoUtente, pagina, idRicerca);
            }
            catch (Exception e)
            {
                logger.Debug("SchedaRicerca.verificaNomeRicerca - Errore nella verifica univocità del nome della ricerca: " + e.Message);
                throw e;
            }
            return retValue;
        }

        public void Modifica(string type, bool custumGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType)
        {
            logger.Debug("SchedaRicerca.Modifica");

            try
            {
                DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
                DocsPaWR.SearchItem item = new SAAdminTool.DocsPaWR.SearchItem();
                item.system_id = nuovaRic.Id;
                item.descrizione = nuovaRic.Titolo;
                item.tipo = this.tipo;

                switch (nuovaRic.Condivisione)
                {
                    case NuovaRicerca.ModoCondivisione.Utente:
                        item.owner_idPeople = Int32.Parse(this.utente.idPeople);
                        break;
                    case NuovaRicerca.ModoCondivisione.Ruolo:
                        item.owner_idGruppo = Int32.Parse(this.ruolo.idGruppo);
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(this._searchKey))
                    item.pagina = this.currentPg.ToString();
                else
                    item.pagina = this._searchKey;

                logger.Debug("SchedaRicerca.Modifica - Serializzazione dei filtri");
                item.filtri = SchedaRicerca.FiltersToString(this.filters);
                logger.Debug("SchedaRicerca.Modifica - Filtri serializzati");

                logger.Debug("SchedaRicerca.Modifica - Sto per modificare la ricerca: " + item.ToString());

                DocsPaWR.InfoUtente infoUtente = UserManager.getInfoUtente();

                item = docspaws.ModificaRicerca(item, infoUtente, custumGrid, gridId, tempGrid, gridType);

                if (custumGrid)
                {
                    if (!string.IsNullOrEmpty(item.gridId))
                    {
                        GridManager.SelectedGrid = GridManager.GetGridFromSearchId(item.gridId, gridType);
                    }
                }

                logger.Debug("SchedaRicerca.Modifica - Ricerca salvata");
            }
            catch (Exception ex)
            {
                logger.Debug("SchedaRicerca.Salva - Errore nel salvataggio della ricerca: " + ex.Message);
                throw ex;
            }
        }




        public class NuovaRicerca
        {
            public enum ModoCondivisione { Utente, Ruolo }
            int id = 0;
            string title = "";
            ModoCondivisione share = ModoCondivisione.Utente;

            public NuovaRicerca()
            { }

            public int Id
            {
                get { return id; }
                set { id = value; }
            }

            public string Titolo
            {
                get { return title; }
                set { title = (value != null) ? value : ""; }
            }

            public ModoCondivisione Condivisione
            {
                get { return share; }
                set { share = value; }
            }
        }


    }
}
