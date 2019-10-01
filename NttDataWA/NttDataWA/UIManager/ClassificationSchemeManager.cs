using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.Utils;
using System.Xml;
using NttDatalLibrary;
using System.Web.UI.WebControls;

namespace NttDataWA.UIManager
{
    public class RisultatiRicercaTitolario : myTreeNode
    {
        public string IDTITOLARIO { get; set; }
        public string ISFASCICOLO { get; set; }
    }

    public class ClassificationSchemeManager
    {
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public static void SetTitolarioInSession(DocsPaWR.OrgTitolario titolario)
        {
            try
            {
                HttpContext.Current.Session["Titolario"] = titolario;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        public static DocsPaWR.OrgTitolario GetTitolarioInSession()
        {
            try
            {
                return HttpContext.Current.Session["Titolario"] as DocsPaWR.OrgTitolario;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// restituisce un nodo del titolario
        /// </summary>
        /// <param name="_idAmministrazione"></param>
        /// <param name="_idParent"></param>
        /// <param name="_idGruppo"></param>
        /// <param name="_idRegistro"></param>
        /// <param name="_idTitolario"></param>
        /// <returns></returns>
        public static string getNodeTitolario(string _idAmministrazione, string _idParent, string _idGruppo, string _idRegistro, string _idTitolario)
        {
            string xmlStream = string.Empty;
            try
            {
                xmlStream = docsPaWS.NodoTitolarioSecurity(_idAmministrazione, _idParent, _idGruppo, _idRegistro, _idTitolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
            return xmlStream;
        }

        /// <summary>
        /// restituisce il titolario attivo
        /// </summary>
        /// <param name="_idAmministrazione"></param>
        /// <returns></returns>
        public static DocsPaWR.OrgTitolario getTitolarioAttivo(string _idAmministrazione)
        {
            DocsPaWR.OrgTitolario titolario = null;
            try
            {
                DocsPaWR.OrgTitolario[] titolarioTemp = docsPaWS.getTitolariUtilizzabili(_idAmministrazione);
                if (titolarioTemp.Length > 0)
                    titolario = titolarioTemp[0];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return titolario;
        }

        /// <summary>
        /// restituisce il titolario
        /// </summary>
        /// <param name="_idTitolario"></param>
        /// <returns></returns>
        public static DocsPaWR.OrgTitolario getTitolario(string _idTitolario)
        {
            DocsPaWR.OrgTitolario titolario = null;
            try
            {
                titolario = docsPaWS.getTitolarioById(_idTitolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return titolario;
        }

        /// <summary>
        /// ricerac un nodo all'interno del titolario
        /// </summary>
        /// <param name="_codice"></param>
        /// <param name="_descrizione"></param>
        /// <param name="_note"></param>
        /// <param name="_indiceSistematico"></param>
        /// <param name="_idAmministrazione"></param>
        /// <param name="_idGruppo"></param>
        /// <param name="_idRegistro"></param>
        /// <param name="_idTitolario"></param>
        /// <returns></returns>
        public static List<RisultatiRicercaTitolario> getRicercaTitolario(string _codice, string _descrizione, string _note, string _indiceSistematico, string _idAmministrazione, string _idGruppo, string _idRegistro, string _idTitolario)
        {
            List<RisultatiRicercaTitolario> listaTitolario = new List<RisultatiRicercaTitolario>();
            try
            {
                string result = docsPaWS.filtroRicercaTitDocspa(_codice, _descrizione, _note, _indiceSistematico, _idAmministrazione, _idGruppo, _idRegistro, _idTitolario);
                if (!string.IsNullOrEmpty(result))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);

                    XmlNode lista = doc.SelectSingleNode("NewDataSet");
                    if (lista.ChildNodes.Count > 0)
                    {
                        foreach (XmlNode nodo in lista.ChildNodes)
                        {
                            RisultatiRicercaTitolario titolario = new RisultatiRicercaTitolario();
                            titolario.DESCRIZIONE = nodo.SelectSingleNode("DESCRIZIONE").InnerText.Replace("'", "&#39;").Replace("\"", "&quot;");
                            titolario.LIVELLO = nodo.SelectSingleNode("LIVELLO").InnerText;
                            titolario.CODICE = nodo.SelectSingleNode("CODICE").InnerText;
                            titolario.ID = nodo.SelectSingleNode("ID").InnerText;
                            titolario.PARENT = nodo.SelectSingleNode("IDPARENT").InnerText;
                            titolario.IDTITOLARIO = _idTitolario;
                            listaTitolario.Add(titolario);
                            //Request.QueryString["isFasc"]
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return listaTitolario;
        }

        /// <summary>
        /// funzione che consente il caricamento dei nodi figli della treeview del titolario
        /// </summary>
        /// <param name="indice"></param>
        public static void LoadTreeViewChild(TreeNode _Node, string _IdRegistro, string _IdAmministrazione, string _IdGruppo, string _IdTitolario)
        {
            try
            {
                myTreeNode TreeNodo = (myTreeNode)_Node;
                TreeNodo.Expanded = true;

                if (TreeNodo.ChildNodes.Count > 0 &&
                    int.Parse(TreeNodo.LIVELLO) > 0)
                    TreeNodo.ChildNodes.RemoveAt(0);

                string idParent = TreeNodo.IDRECORD;
                string xmlStream =
                    UIManager.ClassificationSchemeManager.getNodeTitolario(_IdAmministrazione, idParent, _IdGruppo, _IdRegistro, _IdTitolario);

                myTreeNode nodoT;
                myTreeNode nodoFiglio;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlStream);

                XmlNode lista = doc.SelectSingleNode("NewDataSet");
                if (lista.ChildNodes.Count > 0)
                {
                    foreach (XmlNode nodo in lista.ChildNodes)
                    {
                        nodoT = new myTreeNode();
                        nodoT.ID = nodo.SelectSingleNode("CODICE").InnerText;
                        nodoT.Text = nodo.SelectSingleNode("CODICE").InnerText + " - " + nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                        nodoT.IDRECORD = nodo.SelectSingleNode("IDRECORD").InnerText;
                        nodoT.PARENT = nodo.SelectSingleNode("IDPARENT").InnerText;
                        nodoT.CODLIV = nodo.SelectSingleNode("CODLIV").InnerText;
                        nodoT.CODICE = nodo.SelectSingleNode("CODICE").InnerText;
                        nodoT.DESCRIZIONE = nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                        nodoT.LIVELLO = nodo.SelectSingleNode("LIVELLO").InnerText;
                        nodoT.NUMMESICONSERVAZIONE = nodo.SelectSingleNode("NUMMESICONSERVAZIONE").InnerText;
                        nodoT.Expanded = false;

                        XmlNode nodoReg = nodo.SelectSingleNode("REGISTRO");
                        if (nodoReg != null)
                        {
                            nodoT.REGISTRO = nodo.SelectSingleNode("REGISTRO").InnerText;
                        }

                        TreeNodo.ChildNodes.Add(nodoT);

                        if (Convert.ToInt32(nodo.SelectSingleNode("FIGLIO").InnerText) > 0)
                        {
                            nodoFiglio = new myTreeNode();
                            nodoFiglio.Expanded = false;
                            nodoT.ChildNodes.Add(nodoFiglio);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// funzione che consente il caricamento iniziale del treeview del titolario
        /// </summary>
        /// <param name="nodoxml"></param>
        public static CustomTreeView loadTreeView(CustomTreeView TreeTitolario, string _IdAmministrazione, string _IdParent, string _IdGruppo, string _IdRegistro, string _IdTitolario)
        {

            try
            {
                string nodoxml = UIManager.ClassificationSchemeManager.getNodeTitolario(_IdAmministrazione, _IdParent, _IdGruppo, _IdRegistro, _IdTitolario);
                if (TreeTitolario.Nodes.Count == 0)

                    TreeTitolario.Nodes.Clear();

                myTreeNode nodoT;

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(nodoxml);

                XmlNode lista = doc.SelectSingleNode("NewDataSet");
                if (lista.ChildNodes.Count > 0)
                {
                    foreach (XmlNode nodo in lista.ChildNodes)
                    {
                        nodoT = new myTreeNode();

                        nodoT.ID = nodo.SelectSingleNode("CODICE").InnerText;

                        switch (nodo.SelectSingleNode("STATO").InnerText)
                        {
                            case "A":
                                string active = Utils.Languages.GetLabelFromCode("ClassificationSchemeManagerActive", UIManager.UserManager.GetUserLanguage());
                                nodoT.Text = nodo.SelectSingleNode("DESCRIZIONE").InnerText + " " + active;
                                break;

                            case "C":
                                DateTime dataAttivazione = Convert.ToDateTime(nodo.SelectSingleNode("DATA_ATTIVAZIONE").InnerText);
                                DateTime dataCessazione = Convert.ToDateTime(nodo.SelectSingleNode("DATA_CESSAZIONE").InnerText);
                                nodoT.Text = nodo.SelectSingleNode("DESCRIZIONE").InnerText + " in vigore dal " + dataAttivazione.ToString("dd/MM/yyyy") + " al " + dataCessazione.ToString("dd/MM/yyyy");
                                break;
                        }
                        nodoT.IDRECORD = nodo.SelectSingleNode("IDRECORD").InnerText;
                        nodoT.PARENT = nodo.SelectSingleNode("IDPARENT").InnerText;
                        nodoT.CODLIV = nodo.SelectSingleNode("CODLIV").InnerText;
                        nodoT.CODICE = nodo.SelectSingleNode("CODICE").InnerText;
                        nodoT.DESCRIZIONE = nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                        nodoT.LIVELLO = nodo.SelectSingleNode("LIVELLO").InnerText;
                        if (nodo.SelectSingleNode("NUMMESICONSERVAZIONE") != null)
                            nodoT.NUMMESICONSERVAZIONE = nodo.SelectSingleNode("NUMMESICONSERVAZIONE").InnerText;
                        else
                            nodoT.NUMMESICONSERVAZIONE = "0";

                        XmlNode nodoReg = nodo.SelectSingleNode("REGISTRO");
                        if (nodoReg != null)
                        {
                            nodoT.REGISTRO = nodo.SelectSingleNode("REGISTRO").InnerText;
                        }

                        TreeTitolario.Nodes.Add(nodoT);
                        if (Convert.ToInt32(nodo.SelectSingleNode("FIGLIO").InnerText) > 0)
                        {
                            nodoT.Expanded = true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return TreeTitolario;
        }

        /// <summary>
        /// restituisce l'indice sistematico
        /// </summary>
        /// <param name="_idTitolario"></param>
        /// <returns></returns>
        public static DocsPaWR.FileDocumento getIndiceSistematico(string _idTitolario)
        {
            try
            {
                DocsPaWR.OrgTitolario titolario = new DocsPaWR.OrgTitolario();
                titolario.ID = _idTitolario;
                docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                return docsPaWS.ExportIndiceSistematico(titolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Imposta la sessione dell'export
        /// </summary>
        public static void SetSessionExportFile(DocsPaWR.FileDocumento file)
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["EXPORT_FILE_SESSION"] == null)
                {
                    System.Web.HttpContext.Current.Session.Add("EXPORT_FILE_SESSION", file);
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Recupera l'export in sessione
        /// </summary>
        /// <returns></returns>
        public DocsPaWR.FileDocumento GetSessionExportFile()
        {
            try
            {
                DocsPaWR.FileDocumento filePdf = new DocsPaWR.FileDocumento();

                if (System.Web.HttpContext.Current.Session["EXPORT_FILE_SESSION"] != null)
                {
                    filePdf = (DocsPaWR.FileDocumento)System.Web.HttpContext.Current.Session["EXPORT_FILE_SESSION"];
                }
                return filePdf;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Rilascia la sessione dell'export
        /// </summary>
        public void ReleaseSessionExportFile()
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove("EXPORT_FILE_SESSION");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// carica il treeview dopo una ricerca
        /// </summary>
        /// <param name="idrecord"></param>
        /// <param name="idparent"></param>
        /// <param name="livello"></param>
        public static CustomTreeView LoadTreeViewRicerca(CustomTreeView _TreeView,
            string _IdAmministrazione, string _IdRecord, string _IdParent, int _Livello,
            string _IdGruppo, string _IdRegistro, string _IdTitolario, string _descrizione)
        {
            try
            {
                _Livello++;
                string xmlStream = docsPaWS.findNodoRoot(_IdRecord, _IdParent, _Livello);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlStream);

                myTreeNode TreeNodo = (myTreeNode)_TreeView.Nodes[0];

                XmlNode lista = doc.SelectSingleNode("NewDataSet");
                if (lista.ChildNodes.Count > 0)
                {
                    int indiceCiclo = 1;
                    for (int n = 2; n <= _Livello; n++)
                    {
                        XmlNode liv = doc.SelectSingleNode(".//livello[text()='" + n.ToString() + "']");
                        XmlNode root = liv.ParentNode;
                        string idparent = root.ChildNodes.Item(0).InnerText;
                        string id = root.ChildNodes.Item(2).InnerText;

                        if (findNode(idparent, TreeNodo.ChildNodes) != null)
                            TreeNodo = findNode(idparent, TreeNodo.ChildNodes);
                        //else
                        //    TreeNodo = selectNode(idparent, TreeNodo.ChildNodes);

                        TreeNodo.Expanded = true;
                        TreeNodo.ChildNodes.Clear();
                        indiceCiclo++;
                        string xmlStream1 = UIManager.ClassificationSchemeManager.getNodeTitolario(_IdAmministrazione, idparent, _IdGruppo, _IdRegistro, _IdTitolario);
                        myTreeNode nodoT;
                        myTreeNode nodoFiglio;

                        XmlDocument doc1 = new XmlDocument();
                        doc1.LoadXml(xmlStream1);

                        XmlNode lista1 = doc1.SelectSingleNode("NewDataSet");
                        if (lista1.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode nodo in lista1.ChildNodes)
                            {
                                nodoT = new myTreeNode();
                                nodoT.ID = nodo.SelectSingleNode("CODICE").InnerText;
                                nodoT.Text = nodo.SelectSingleNode("CODICE").InnerText + " - " + nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                                nodoT.IDRECORD = nodo.SelectSingleNode("IDRECORD").InnerText;
                                nodoT.PARENT = nodo.SelectSingleNode("IDPARENT").InnerText;
                                nodoT.CODLIV = nodo.SelectSingleNode("CODLIV").InnerText;
                                nodoT.CODICE = nodo.SelectSingleNode("CODICE").InnerText;
                                nodoT.DESCRIZIONE = nodo.SelectSingleNode("DESCRIZIONE").InnerText;
                                nodoT.LIVELLO = nodo.SelectSingleNode("LIVELLO").InnerText;
                                nodoT.NUMMESICONSERVAZIONE = nodo.SelectSingleNode("NUMMESICONSERVAZIONE").InnerText;
                                nodoT.Expanded = false;

                                XmlNode nodoReg = nodo.SelectSingleNode("REGISTRO");
                                if (nodoReg != null)
                                {
                                    nodoT.REGISTRO = nodo.SelectSingleNode("REGISTRO").InnerText;
                                }

                                TreeNodo.ChildNodes.Add(nodoT);

                                if (Convert.ToInt32(nodo.SelectSingleNode("FIGLIO").InnerText) > 0)
                                {
                                    nodoFiglio = new myTreeNode();
                                    nodoFiglio.Expanded = true;
                                    nodoT.ChildNodes.Add(nodoFiglio);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return _TreeView;
        }

        /// <summary>
        /// ricerca un nodo nel titolario
        /// </summary>
        /// <param name="idrecord"></param>
        /// <param name="nodecollection"></param>
        /// <returns></returns>
        private static myTreeNode findNode(string idrecord, TreeNodeCollection nodecollection)
        {
            try
            {
                myTreeNode mytreenode = null;
                foreach (TreeNode t in nodecollection)
                {
                    if (((myTreeNode)t).IDRECORD.Equals(idrecord))
                    {
                        mytreenode = (myTreeNode)t;
                        mytreenode.Selected = true;
                        break;
                    }
                }
                return mytreenode;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// seleziona un nodo
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="nodecollection"></param>
        /// <returns></returns>
        private static myTreeNode selectNode(string codice, TreeNodeCollection nodecollection)
        {
            try
            {
                myTreeNode mytreenode = new myTreeNode();
                foreach (TreeNode t in nodecollection)
                {
                    if (((myTreeNode)t).CODICE.Equals(codice))
                    {
                        mytreenode = (myTreeNode)t;
                        mytreenode.Selected = true;
                        break;
                    }
                }
                return mytreenode;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// verifica se l'indice sistematico è attivo
        /// </summary>
        /// <returns></returns>
        public static bool isEnableIndiceSistematico()
        {
            try
            {
                return docsPaWS.isEnableIndiceSistematico();
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static ArrayList getTitolariUtilizzabili()
        {
            return new ArrayList(docsPaWS.getTitolariUtilizzabili(UserManager.GetUserInSession().idAmministrazione));
        }

        //ADP 8 maggio 2013
        public static DocsPaWR.OrgNodoTitolario[] getNodiFromProtoTitolario(NttDataWA.DocsPaWR.Registro registro, string idAmministrazione, string numProtoPratica, string idTitolario)
        {

            DocsPaWR.OrgNodoTitolario[] arList = null;
            try
            {
                arList = docsPaWS.getNodiFromProtoTit(registro, idAmministrazione, numProtoPratica, idTitolario);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            return arList;

        }

    }
}