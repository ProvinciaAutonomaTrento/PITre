using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Collections;

namespace VerificaSupporto
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VerificaSupportoProgress : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _interrompi = false;
        
        /// <summary>
        /// 
        /// </summary>
        private bool _finito = false;

        /// <summary>
        /// 
        /// </summary>
        public VerificaSupportoProgress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="idPeople"></param>
        /// <param name="idIstanza"></param>
        /// <param name="idDocumento"></param>
        /// <param name="pathSupporto"></param>
        /// <param name="percentualeVerifica"></param>
        public VerificaSupportoProgress(string serviceUrl, string idPeople, string idIstanza, string idDocumento, string pathSupporto, string percentualeVerifica)
            : this()
        {
            this.ServiceUrl = serviceUrl;
            this.IdPeople = idPeople;
            this.IdIstanza = idIstanza;
            this.IdDocumento = idDocumento;
            this.PathSupporto = pathSupporto;
            this.PercentualeVerifica = percentualeVerifica;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerificaSupportoProgress_Shown(object sender, EventArgs e)
        {
            try
            {
                this.Verifica();

                if (!this.Success)
                {
                    MessageBox.Show(this.ErrorMessage, "Verifica supporto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.IdDocumento))
                    {
                        // Verifica sul singolo documento
                        MessageBox.Show(string.Format("La verifica di integrità sul documento con Id '{0}' ha dato esito positivo", this.IdDocumento),
                                "Verifica supporto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Success = false;
                this.ErrorMessage = ex.Message;

                MessageBox.Show(ex.Message, "Verifica supporto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulla_Click(object sender, EventArgs e)
        {
            if (this.btnAnnulla.Text == "&Annulla")
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this._interrompi = true;

                this.Close();
            }

            if (this.btnAnnulla.Text == "&Chiudi")
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ServiceUrl
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        public string IdPeople
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PathSupporto
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string PercentualeVerifica
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdDocumento
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdIstanza
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private string PathFileChiusuraXML
        {
            get
            {
                return this.PathSupporto + @"\Chiusura\file_chiusura.xml";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string PathFileChiusuraP7M
        {
            get
            {
                return this.PathSupporto + @"\Chiusura\file_chiusura.xml.p7m";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string PathFileChiusuraTSR
        {
            get
            {
                return this.PathSupporto + @"\Chiusura\file_chiusura.tsr";
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private string PathFileDatiIstanza
        {
            get
            {
                return this.PathSupporto + @"\dati_istanza.xml";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Interrotta
        {
            get
            {
                return this._interrompi;
            }
        }

        #region Logica

        /// <summary>
        /// Verifica validità istanza di conservazione
        /// </summary>
        /// <param name="url"></param>
        /// <param name="idIstanza"></param>
        /// <param name="urlXml"></param>
        /// <param name="urlP7m"></param>
        /// <param name="urlTsr"></param>
        /// <returns></returns>
        private bool IsIstanzaConservazione()
        {
            return (File.Exists(this.PathFileChiusuraXML) &&
                    File.Exists(this.PathFileChiusuraP7M) &&
                    File.Exists(this.PathFileDatiIstanza));
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckIstanzaConservazione()
        {
            if (string.IsNullOrEmpty(this.IdIstanza))
                throw new ApplicationException("Parametro 'IdIstanza' mancante.");

            if (!this.IsIstanzaConservazione())
                throw new ApplicationException("La cartella non contiene un supporto valido.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string CalcolaImpronta256(byte[] stream)
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] impronta = sha.ComputeHash(stream);
            return BitConverter.ToString(impronta).Replace("-", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string CalcolaImpronta(byte[] stream)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] impronta = sha.ComputeHash(stream);
            Console.WriteLine(impronta.Length);

            return BitConverter.ToString(impronta).Replace("-", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="childList"></param>
        /// <param name="improntaCalcolata"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        public bool VerificaImprontaFile(XmlNodeList childList, out string improntaCalcolata , out string idDocumento)
        {
            bool result = false;

            XmlNode idNode = (XmlNode)childList[0];
            XmlNode pathNode = (XmlNode)childList[1];
            XmlNode hashNode = (XmlNode)childList[2];

            string docNumber = idNode.InnerText;
            string pathFile = pathNode.InnerText.Substring(1);
            string impronta = hashNode.InnerText;

            string path = Path.Combine(this.PathSupporto, pathFile);
            byte[] content = System.IO.File.ReadAllBytes(path);

            string improntaOriginale = string.Empty;

            improntaOriginale = this.CalcolaImpronta256 (content);

            if (impronta == improntaOriginale)
                result = true;
            else
            {
                improntaOriginale= this.CalcolaImpronta(content);
                if(impronta ==improntaOriginale)
                    result = true;
            }

            improntaCalcolata = improntaOriginale;
            idDocumento = docNumber;
            return result;
        }

        /// <summary>
        /// Reperimento identificativo istanza dal file di chiusura
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        private string GetIdIstanza(XmlDocument xmlFile)
        {
            List<XmlNode> list = new List<XmlNode>();

            XmlNodeList listaNodiHeader = xmlFile.GetElementsByTagName("sincro:SelfDescription");

            if (listaNodiHeader.Count == 0)
                throw new ApplicationException("Elemento 'sincro:SelfDescription' non presente. File di chiusura non valido.");

            XmlNode idNode = listaNodiHeader[0].ChildNodes[0];

            return idNode.InnerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private XmlNode[] GetNodiDocumento(XmlDocument xmlFile)
        {
            List<XmlNode> list = new List<XmlNode>();

            XmlNodeList listaNodiDocNumber = xmlFile.GetElementsByTagName("sincro:File");

            foreach (XmlNode n in listaNodiDocNumber)
            {
                if (!string.IsNullOrEmpty(this.IdDocumento))
                {
                    XmlNode idNode = n.ChildNodes[0];

                    if (string.Compare(idNode.InnerText, this.IdDocumento, true) == 0)
                    {
                        list.Add(n);
                        break;
                    }
                }
                else
                {
                    list.Add(n);
                }
            }

            if (!string.IsNullOrEmpty(this.IdDocumento) && list.Count == 0)
                throw new ApplicationException(string.Format("Documento con id {0} non trovato nel file di chiusura.", this.IdDocumento));
            
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void Verifica()
        {
            this.CheckIstanzaConservazione();
            
            // TODO: Verifica validità firma del file di chiusura

            // TODO: Verifica validità marca del file di chiusura

            Double percentuale;
            Double.TryParse(this.PercentualeVerifica, out percentuale);


            this.UpdateProgress(0, "Individuazione file di chiusura istanza in corso...");
            Application.DoEvents();
            
            XmlDocument xmlFile = new XmlDocument();
            xmlFile.Load(this.PathFileChiusuraXML);

            // Per la conservazione l'url del file xml da leggere deve essere quello in cui ha fatto download dell'istanza
            string idIstanzaFileChiusura = this.GetIdIstanza(xmlFile);

            if (idIstanzaFileChiusura != this.IdIstanza)
                throw new ApplicationException("Il file di chiusura non corrisponde all'istanza richiesta. Il supporto non è valido.");

            // Reperimento nodi relativi ai files
            XmlNode[] nodiDocumento = this.GetNodiDocumento(xmlFile);


            this.UpdateProgress(0, "Connessione al servizio per la verifica di integrità dei documenti in corso...");
            Application.DoEvents();
            
            using (Proxy.IntegritaServices services = new Proxy.IntegritaServices())
            {
                services.Url = this.ServiceUrl;

                // Numero totali di documenti che fanno parte dell'istanza
                int numeroDocumenti = nodiDocumento.Length;

                // Numero di documenti di cui voglio verificare l'impronta
                int n = 0;

                if (percentuale < 100)
                {
                    n = ((int)percentuale * numeroDocumenti / 100);

                    if (n == 0)
                        n = n + 1;
                }
                else
                {
                    n = numeroDocumenti;
                }

                // Genero una sequenza di numeri casuali senza ripetizione compresi tra 0 e numeroDocumenti-1
                int[] numeriCasuali = this.EseguiEstrazioneRandom(numeroDocumenti);

                // Verifico l'impronta di n documenti tra quelli presenti nel supporto e calcolo la percentuale di quelli che nn sono stati corrotti
                int contatore = 0;

                int countValidDocuments = 0;

                for (int i = 0; i < n; i++)
                {
                    this.UpdateProgress((contatore * 100 / numeroDocumenti), string.Format("Verifica integrità documento {0} di {1} in corso...", (i + 1), n));

                    if (this._interrompi)
                    {
                        if (MessageBox.Show("Interrompere la verifica del supporto?", this.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.OK)
                            throw new ApplicationException(string.Format("Verifica interrotta dall'utente. Analizzati {0} documenti su {1}.", i, n));
                        else
                            this._interrompi = false;
                    }

                    //prendo l'impronta del nodo con posizione tra quelli generati in maniera casuale
                    int indiceNodo = numeriCasuali[i];
                    XmlNode nodoDoc = ((XmlNode)nodiDocumento[indiceNodo]);
                    XmlNodeList childList = (XmlNodeList)nodoDoc.ChildNodes;

                    //verifico che l'impronta del documento e dei suoi allegati corrisponda a quella originale
                    string improntaCalcolata;
                    string idDocumento;

                    this.UpdateProgress((contatore * 100 / numeroDocumenti),
                            string.Format("Verifica impronta documento {0} di {1} su supporto fisico in corso...", (i + 1), n));

                    bool verifyImpronta = this.VerificaImprontaFile(childList, out improntaCalcolata, out idDocumento);

                    if (verifyImpronta)
                    {
                        Proxy.GetHashDocumentoRequest request = new Proxy.GetHashDocumentoRequest
                        {
                            IdDocumento = idDocumento,
                            IdPeople = this.IdPeople
                        };

                        this.UpdateProgress((contatore * 100 / numeroDocumenti),
                                string.Format("Verifica impronta documento {0} di {1} su repository in corso...", (i + 1), n));

                        Proxy.GetHashDocumentoResponse resp = services.GetHashDocumento(request);

                        if (resp.Success)
                        {
                            if (resp.HashDatabase == resp.HashRepository)
                            {
                                if (improntaCalcolata == resp.HashDatabase)
                                {
                                    countValidDocuments++;
                                }
                            }
                        }
                    }

                    contatore = contatore + 1;

                    this.UpdateProgress((contatore * 100 / numeroDocumenti),
                        string.Format("Verifica integrità documento {0} di {1} effettuata.", (i + 1), n));

                    Application.DoEvents();
                }

                if (countValidDocuments == n)
                {
                    this.Success = true;
                    this.ErrorMessage = string.Empty;
                }
                else
                {
                    this.Success = false;
                    
                    int invalidDocuments = (contatore - countValidDocuments);

                    string msg = string.Empty;
                    
                    if (invalidDocuments > 1)
                        msg = "Individuati {0} documenti corrotti. Il supporto risulta danneggiato.";
                    else
                        msg = "Individuato {0} documento corrotto. Il supporto risulta danneggiato.";

                    this.ErrorMessage = string.Format(msg, invalidDocuments);
                }

                if (!this._interrompi)
                {
                    this.btnAnnulla.Text = "&Chiudi";
                    this._finito = true;
                }
            }

            this._interrompi = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="description"></param>
        private void UpdateProgress(int percent, string description)
        {
            this.prgElaborazione.Value = percent;
            this.lblElaborazione.Text = description;
            this.lblElaborazione.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numeroElementi"></param>
        /// <returns></returns>
        private int[] EseguiEstrazioneRandom(int numeroElementi)
        {
            // inizializzazione dell'array di ritorno
            int[] iValues = new int[numeroElementi];

            SortedList sList = new SortedList();

            // popolo la SortedList con i valori
            for (int k = 0; k < numeroElementi; k++)
                sList.Add(k, k);

            // inizializzo il generatore di numeri random
            System.Random rnd = new System.Random(unchecked((int)DateTime.Now.Ticks));

            // estrazione 
            for (int k = 0; k < numeroElementi; k++)
            {
                // ad ogni ciclo il count della sortedlist diminuisce di uno

                int x = rnd.Next(0, sList.Count);

                // prendiamo il numero che troviamo alla posizione relativa

                iValues[k] = (int)sList.GetByIndex(x);

                // rimozione della posizione già utilizzata
                sList.RemoveAt(x);

            }
         
            return iValues;
        }
   
        #endregion
    }
}