using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;
using System.Web.UI;
using System.IO;
using System.Collections;

namespace NttDataWA.UIManager
{
    public class SearchManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public const string SESSION_KEY = "SchedaRicerca";
        private string _searchKey = string.Empty;
        DocsPaWR.Utente utente = null;
        DocsPaWR.Ruolo ruolo = null;
        DocsPaWR.FiltroRicerca[][] filters = null;
        System.Web.UI.Page currentPg = null;
        NuovaRicerca nuovaRic = null;
        string tipo = null;

        public SearchManager()
        {

        }

        public SearchManager(string searchKey)
        {
            this._searchKey = searchKey;
        }

        public SearchManager(string searchKey, DocsPaWR.Utente usr, DocsPaWR.Ruolo grp)
            : this(searchKey)
        {
            utente = usr;
            ruolo = grp;
        }

        public SearchManager(string searchKey, DocsPaWR.Utente usr, DocsPaWR.Ruolo grp, System.Web.UI.Page pg)
            : this(searchKey)
        {
            utente = usr;
            ruolo = grp;
            currentPg = pg;
        }

        public DocsPaWR.Utente Utente
        {
            get { return utente; }
            set { utente = value; }
        }

        public DocsPaWR.Ruolo Ruolo
        {
            get { return ruolo; }
            set { ruolo = value; }
        }

        public DocsPaWR.FiltroRicerca[][] FiltriRicerca
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

        public DocsPaWR.FiltroRicerca GetFiltro(string nomeFiltro)
        {
            DocsPaWR.FiltroRicerca filtro = null;

            if (filters == null || filters[0] == null)
                return null;

            ArrayList al = new ArrayList(filters[0]);
            bool found = false;
            int idx = -1;
            for (int i = 0; !found && i < al.Count; i++)
            {
                DocsPaWR.FiltroRicerca fr = (DocsPaWR.FiltroRicerca)al[i];
                if (fr != null && fr.argomento == nomeFiltro)
                {
                    idx = i;
                    found = true;
                }
            }
            if (idx != -1)
                filtro = (DocsPaWR.FiltroRicerca)al[idx];

            return filtro;
        }

        public static DocsPaWR.SearchItem GetItemSearch(int id) {
            return docsPaWS.RecuperaRicerca(id);
        }

        public void SetFiltro(string nomeFiltro, DocsPaWR.FiltroRicerca filtro)
        {
            if (filtro == null)
                return;

            if (filters == null)
                filters = new DocsPaWR.FiltroRicerca[1][];

            ArrayList al = null;
            if (filters[0] != null)
                al = new ArrayList(filters[0]);
            else
                al = new ArrayList();

            bool found = false;
            int idx = -1;
            for (int i = 0; !found && i < al.Count; i++)
            {
                DocsPaWR.FiltroRicerca fr = (DocsPaWR.FiltroRicerca)al[i];
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

            filters[0] = new DocsPaWR.FiltroRicerca[al.Count];
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
                DocsPaWR.FiltroRicerca fr = (DocsPaWR.FiltroRicerca)al[i];
                if (fr != null && fr.argomento == nomeFiltro)
                {
                    idx = i;
                    found = true;
                }
            }
            if (idx != -1)
                al.RemoveAt(idx);

            filters[0] = new DocsPaWR.FiltroRicerca[al.Count];
            al.CopyTo(filters[0]);
        }

        public bool verificaNome(string nomeRicerca, DocsPaWR.InfoUtente infoUtente, string pagina, string adl)
        {
            bool retValue = false;
            try
            {
                //Permette di non fare il controllo sul nome della ricerca salvata
                if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_VERIFICA_NOME.ToString())) && Utils.InitConfigurationKeys.GetValue("0", DBKeys.FE_VERIFICA_NOME.ToString()).Equals("1"))
                {
                    retValue = false;
                }
                else
                {
                    retValue = docsPaWS.verificaNomeRicerca(nomeRicerca, infoUtente, pagina, adl);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return retValue;
        }

        public void Salva(string type, bool custumGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType)
        {
            if (nuovaRic == null)
                return;

            try
            {
                DocsPaWR.SearchItem item = new DocsPaWR.SearchItem();
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

                item.filtri = SearchManager.FiltersToString(this.filters);

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                if ((type != null) && (type == "ricADL"))
                    item = docsPaWS.SalvaRicerca(item, infoUtente, true, custumGrid, gridId, tempGrid, gridType);
                else
                    item = docsPaWS.SalvaRicerca(item, infoUtente, false, custumGrid, gridId, tempGrid, gridType);

                nuovaRic.Id = item.system_id;

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
                throw ex;
            }
        }

        public void Cancella(int id)
        {
            try
            {
                bool ok = docsPaWS.CancellaRicerca(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Seleziona(int id, out string gridTempId)
        {
            DocsPaWR.SearchItem item = null;
            try
            {
                gridTempId = string.Empty;
                item = docsPaWS.RecuperaRicerca(id);
                if (item == null)
                {
                    throw new Exception("Ricerca non trovata");
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.gridId))
                    {
                        gridTempId = item.gridId;
                    }
                    filters = SearchManager.StringToFilters(item.filtri);
                }
            }
            catch (Exception ex)
            {
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

            try
            {
                //1.Carico dal database la lista delle ricerche per l'utente e ruolo corrente
                DocsPaWR.SearchItemList itemList = docsPaWS.ListaRicercheSalvate(Int32.Parse(this.utente.idPeople),
                    Int32.Parse(this.ruolo.idGruppo),
                    (!allPages) ? this._searchKey.ToString() : null, true, tipo);

                if (itemList != null)
                {
                    foreach (DocsPaWR.SearchItem item in itemList.lista)
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

            try
            {
                //1.Carico dal database la lista delle ricerche per l'utente e ruolo corrente
                DocsPaWR.SearchItemList itemList = docsPaWS.ListaRicercheSalvate(Int32.Parse(this.utente.idPeople),
                    Int32.Parse(this.ruolo.idGruppo),
                    (!allPages) ? this._searchKey.ToString() : null, false, tipo);

                if (itemList != null)
                {
                    foreach (DocsPaWR.SearchItem item in itemList.lista)
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

        public static string FiltersToString(DocsPaWR.FiltroRicerca[][] filtriRicerca)
        {
            try
            {
                //EMANUELA 18/12/2014 : modificata per i caratteri accentati

                //Serializzazione del filtro di ricerca
                System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(filtriRicerca.GetType());
                System.IO.MemoryStream _mStream = new System.IO.MemoryStream();
                System.Xml.XmlTextWriter _writer = new System.Xml.XmlTextWriter(_mStream, System.Text.Encoding.UTF8);
                _serializer.Serialize(_writer, filtriRicerca);
                _mStream = (MemoryStream)_writer.BaseStream;
                _mStream.Position = 0;
                //A questo punto il mio oggetto serializzato è nello stream
                //estraiamo il buffer dello stream e lo convertiamo in stringa
                //string outcome = System.Text.Encoding.ASCII.GetString(_mStream.GetBuffer());
                StreamReader sr = new StreamReader(_mStream);
                string outcome  = sr.ReadToEnd();
                //Ripuliamo l'oggetto serializzato
                //int tmp = outcome.IndexOf("\0");
                //outcome = outcome.Remove(tmp, outcome.Length - tmp);

                return outcome;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fallita serializzazione dei criteri di ricerca");
            }
            return null;
        }

        public static DocsPaWR.FiltroRicerca[][] StringToFilters(string filtriRicerca)
        {
            DocsPaWR.FiltroRicerca[][] outcome = null;
            try
            {
                Type t = typeof(DocsPaWR.FiltroRicerca[][]);
                System.Xml.Serialization.XmlSerializer _serializer = new System.Xml.Serialization.XmlSerializer(t);

                System.IO.StringReader sr = new System.IO.StringReader(filtriRicerca);

                System.Xml.XmlTextReader _reader = new System.Xml.XmlTextReader(sr);
                outcome = ((DocsPaWR.FiltroRicerca[][])(_serializer.Deserialize(_reader)));
            }
            catch
            {
                Console.WriteLine("Fallita deserializzazione dei criteri di ricerca");
            }
            return outcome;
        }

        public bool verificaNomeModifica(string nomeRicerca, DocsPaWR.InfoUtente infoUtente, string pagina, string idRicerca)
        {
            bool retValue = false;
            try
            {
                retValue = docsPaWS.verificaNomeRicercaModifica(nomeRicerca, infoUtente, pagina, idRicerca);
            }
            catch (Exception e)
            {
                throw e;
            }
            return retValue;
        }

        public void Modifica(string type, bool custumGrid, string gridId, Grid tempGrid, GridTypeEnumeration gridType)
        {
            try
            {
                DocsPaWR.SearchItem item = new DocsPaWR.SearchItem();
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

                item.filtri = SearchManager.FiltersToString(this.filters);

                DocsPaWR.InfoUtente infoUtente = UserManager.GetInfoUser();

                item = docsPaWS.ModificaRicerca(item, infoUtente, custumGrid, gridId, tempGrid, gridType);

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