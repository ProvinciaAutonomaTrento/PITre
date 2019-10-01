using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using NttDataWA.DocsPaWR;
//using NttDataWA.DocsPaFatturazioneWR;
using NttDataWA.Utils;
using log4net;

namespace NttDataWA.UIManager
{
    public class ImportInvoiceManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(ImportInvoiceManager));
        //private static DocsPaFatturazioneWR.DocsPaFatturazioneWS ws = new DocsPaFatturazioneWS();
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();
        private static string InvoiceNamespace = System.Configuration.ConfigurationManager.AppSettings["NAMESPACE_FATTURAPA"];


        // METODI TEMPORANEI PER TEST
        public static DocsPaWR.FileDocumento getPDF()
        {
            DocsPaWR.FileDocumento fileDoc = new DocsPaWR.FileDocumento();
            Byte[] bites = System.IO.File.ReadAllBytes("C:\\test_invoice.pdf");
            fileDoc.cartaceo = false;
            fileDoc.content = bites;
            fileDoc.estensioneFile = "pdf";
            fileDoc.fullName = "esempio_fattura.pdf";
            fileDoc.length = bites.Length;
            fileDoc.name = "esempio_fattura.pdf";
            fileDoc.nomeOriginale = "esempio_fattura.pdf";
            fileDoc.path = "";

            return fileDoc;

        }

        public static string getFattura(string idFattura)
        {
            
            ws.Timeout = System.Threading.Timeout.Infinite;
            try
            {
                //string invoice = ws.GetFatturaXML(ImportInvoiceManager.getInfoUtente(), idFattura);
                string invoice = ws.GetFatturaXML(UserManager.GetInfoUser(), idFattura);
                if (string.IsNullOrEmpty(invoice) || invoice.Equals("NotFound"))
                {
                    // fattura non trovata
                    return "NotFound";
                }
                else if (invoice.Equals("KO"))
                {
                    // errore nella ricerca
                    return "KO";
                }
                else
                {
                    XmlDocument XmlDoc = new XmlDocument();
                    
                    XmlDoc.LoadXml(invoice);
                    SetSessionValue("invoiceXML", XmlDoc);

                    XmlNamespaceManager xmlnsMan = new XmlNamespaceManager(XmlDoc.NameTable);
                    //xmlnsMan.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                    //xmlnsMan.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                    xmlnsMan.AddNamespace("p", InvoiceNamespace);

                    XmlNode node = XmlDoc.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaHeader/DatiTrasmissione/CodiceDestinatario", xmlnsMan);
                    if (string.IsNullOrEmpty(node.InnerText.Trim()))
                    {
                        return "NO_IPA";
                    }

                    // Produco il file XML da utilizzare per l'anteprima
                    // contiene il riferimento al foglio di stile XSL


                    //XmlDocument previewInvoice = new XmlDocument();
                    //string decl = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                    //string pi = "<?xml-stylesheet type=\"text/xsl\" href=\"http://localhost/nttdatawa/importDati/fatturapa_v1.0.xsl\"?>";
                    //string previewXml = invoice.Replace(decl, decl + "\n" + pi);
                    //previewInvoice.LoadXml(previewXml);

                    //previewInvoice.Save("C:\\Users\\utente\\temp\\test_invoice.xml");



                    //Byte[] bytes = Encoding.UTF8.GetBytes(previewInvoice.OuterXml);
                    //DocsPaWR.FileDocumento fileDoc = new DocsPaWR.FileDocumento();

                    //fileDoc.cartaceo = false;
                    //fileDoc.contentType = "text/xml";
                    //fileDoc.content = bytes;
                    //fileDoc.length = bytes.Length;

                    // nascondere i campi vuoti nella preview


                    // Nascondo i campi vuoti in un file copia, conservo l'originale
                    // cosi non altero la struttura del xml

                    //rimuovo i nodi vuoti (meglio nasconderli)
                    //XmlDocument XmlDocPreview = new XmlDocument();
                    //XmlDocPreview.LoadXml(invoice);
                    //XmlNodeList emptyElements = XmlDocPreview.SelectNodes(@"//*[not(node())]");
                    //do
                    //{
                    //    foreach (XmlNode nodeTemp in emptyElements)
                    //        nodeTemp.ParentNode.RemoveChild(nodeTemp);
                    //    emptyElements = XmlDocPreview.SelectNodes(@"//*[not(node())]");
                    //} while (emptyElements.Count > 0);

                    // fine rimozione nodi vuoti


                    //DocsPaWR.FileDocumento fileDoc = getInvoicePreview(invoice);
                    DocsPaWR.FileDocumento fileDoc = getInvoicePreview(XmlDoc.OuterXml);
                    //.FileDocumento fileDoc = getInvoicePreview(XmlDocPreview.OuterXml);
                    SetSessionValue("invoicePreview", fileDoc);

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return "KO";
            }

        }

        public static DocsPaWR.FileDocumento getFileDocFattura()
        {
            try
            {
                DocsPaWR.FileDocumento retVal = new DocsPaWR.FileDocumento();
                XmlDocument XmlDoc = (XmlDocument)GetSessionValue("invoiceXML");
                if (XmlDoc != null)
                {
                    // recupero il numero fattura
                    XmlNamespaceManager xmlnsMan = new XmlNamespaceManager(XmlDoc.NameTable);
                    //xmlnsMan.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.0");
                    //xmlnsMan.AddNamespace("p", "http://www.fatturapa.gov.it/sdi/fatturapa/v1.1");
                    xmlnsMan.AddNamespace("p", InvoiceNamespace);

                    XmlNode node = XmlDoc.SelectSingleNode("p:FatturaElettronica/FatturaElettronicaBody/DatiGenerali/DatiGeneraliDocumento/Numero", xmlnsMan);
                    string idFattura = node.InnerText;

                    // content
                    Byte[] bytes = Encoding.UTF8.GetBytes(XmlDoc.OuterXml);

                    // parametri fileDocumento
                    retVal.cartaceo = false;
                    retVal.content = bytes;
                    retVal.contentType = "text/xml";
                    retVal.estensioneFile = "xml";
                    retVal.fullName = "Invoice_No" + idFattura + ".xml";
                    retVal.length = bytes.Length;
                    retVal.name = "Invoice_No" + idFattura + ".xml";
                    retVal.nomeOriginale = "Invoice_No" + idFattura + ".xml";
                    retVal.path = "";
                }
                else
                {
                    retVal = null;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }


        }
    
        public static bool uploadFattura()
        {

            bool retVal = false;

            try
            {
                XmlDocument docXML = (XmlDocument)GetSessionValue("invoiceXML");
                string invoice = string.Empty;
                if (docXML != null)
                {
                    invoice = docXML.OuterXml;
                    retVal = ws.SendFattura(invoice, UserManager.GetInfoUser(), RoleManager.GetRoleInSession().idGruppo);
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                UIManager.AdministrationManager.DiagnosticError(ex);
            }

            return retVal;

        }

        public static bool updateParams(string rifAmm, string strIdDoc, string codGic, string posFin, string strDesc, string strQuant, string strPrezUni, string strPrezTot, string strAliquot, string optional1, string optional2, string optional3, string optional4, string optional5, string optional6, string optional7, string optional8, string optional9, string optional10)
        {
            XmlDocument fatturaXML = (XmlDocument)GetSessionValue("invoiceXML");
            if (fatturaXML == null)
                return false;
            
            NttDatalLibrary.FatturaElettronicaType fattura = FatturaPARepository.GenerateFatturaFromXML(fatturaXML);
            if (fattura == null)
                return false;
            try
            {
                // Controlla nella rappresentazione tabellare dei nodi quali sono quelli obbligatori (già creati) e quali non (da creare nel caso non esistano)
                // Aggiorna Campi oggetto.

                #region Header 
                // 1.2.6
                if (!string.IsNullOrWhiteSpace(rifAmm))
                {
                    fattura.FatturaElettronicaHeader.CedentePrestatore.RiferimentoAmministrazione = rifAmm.Trim();
                }
                #endregion

                #region Body
                var body = fattura.FatturaElettronicaBody.FirstOrDefault();
                if (body != null)
                {
                    NttDatalLibrary.DatiDocumentiCorrelatiType _datiContratto = null;
                    NttDatalLibrary.DatiDocumentiCorrelatiType _datiOrdineAcquisto = null;
                    NttDatalLibrary.DettaglioLineeType _dettaglioLinea = null;
                    if (body.DatiGenerali.DatiOrdineAcquisto != null)
                        _datiOrdineAcquisto = body.DatiGenerali.DatiOrdineAcquisto.FirstOrDefault();
                    _dettaglioLinea = body.DatiBeniServizi.DettaglioLinee.FirstOrDefault();

                    //2.1.2.4  
                    if (!string.IsNullOrEmpty(optional1))
                    {
                        //   [2.1.2.2] idDocumento è un requisito

                        if (_datiOrdineAcquisto != null)
                            _datiOrdineAcquisto.NumItem = optional1.Trim();
                    }
                    //2.1.2.5
                    if (!string.IsNullOrEmpty(optional2))
                    {
                        if (_datiOrdineAcquisto != null)
                            _datiOrdineAcquisto.CodiceCommessaConvenzione = optional2.Trim();
                    }

                    // 2.1.3.2
                    if (!string.IsNullOrWhiteSpace(strIdDoc))
                    {
                        if (body.DatiGenerali.DatiContratto == null)
                            body.DatiGenerali.DatiContratto = new NttDatalLibrary.DatiDocumentiCorrelatiType[] { new NttDatalLibrary.DatiDocumentiCorrelatiType() };
                        _datiContratto = body.DatiGenerali.DatiContratto.FirstOrDefault();  // supposto che sia già stato creato in precedenza nel backend
                        _datiContratto.IdDocumento = strIdDoc.Trim();
                    }
                    //2.1.3.5
                    if (!string.IsNullOrEmpty(optional5))
                    {
                        if (_datiContratto != null)
                            _datiContratto.CodiceCommessaConvenzione = optional5.Trim();
                    }
                    // 2.1.3.7
                    if (!string.IsNullOrEmpty(codGic))
                    {
                        if (_datiContratto != null)
                            _datiContratto.CodiceCIG = codGic.Trim();
                    }

                    // [2.1.4.2]
                    if (!string.IsNullOrEmpty(optional10))
                    {
                        if (body.DatiGenerali.DatiConvenzione == null)
                            body.DatiGenerali.DatiConvenzione = new NttDatalLibrary.DatiDocumentiCorrelatiType[] { new NttDatalLibrary.DatiDocumentiCorrelatiType() };
                        var _datiConvenzione = body.DatiGenerali.DatiConvenzione.FirstOrDefault();
                        _datiConvenzione.IdDocumento = optional10;
                    }

                    // [2.1.5.1]
                    if (!string.IsNullOrEmpty(optional8))
                    {
                        if (body.DatiGenerali.DatiRicezione == null)
                            body.DatiGenerali.DatiRicezione = new NttDatalLibrary.DatiDocumentiCorrelatiType[] { new NttDatalLibrary.DatiDocumentiCorrelatiType() };
                        var _datiRicezione = body.DatiGenerali.DatiRicezione.FirstOrDefault();
                        //2.1.5.1
                        if (!string.IsNullOrEmpty(optional7))
                            //_datiRicezione.RiferimentoNumeroLinea = new string[] { optional7.Trim() };
                            _datiRicezione.RiferimentoNumeroLinea = optional7.Trim();
                        //2.1.5.2
                        _datiRicezione.IdDocumento = optional8.Trim();
                        //2.1.5.4
                        if (!string.IsNullOrEmpty(optional9))
                            _datiRicezione.NumItem = optional9.Trim();
                    }


                    
                    //2.2.1.7
                    //if (!string.IsNullOrEmpty(optional3) && _dettaglioLinea != null)
                    //{
                    //    DateTime _dataInizio;
                    //    DateTime.TryParse(optional3, out _dataInizio);
                    //    _dettaglioLinea.DataInizioPeriodo = _dataInizio;
                    //}
                    //2.2.1.8 
                    //if (!string.IsNullOrEmpty(optional4) && _dettaglioLinea != null)
                    //{
                    //    DateTime _dataFine;
                    //    DateTime.TryParse(optional4, out _dataFine);
                    //    _dettaglioLinea.DataInizioPeriodo = _dataFine;
                    //}


                    //2.2.1.15
                    if (!string.IsNullOrEmpty(optional6) && _dettaglioLinea != null)
                    {
                        _dettaglioLinea.RiferimentoAmministrazione = optional6.Trim();
                    }

                    // [2.2.1.16.2]
                    if (!string.IsNullOrEmpty(posFin) && _dettaglioLinea != null)
                    {
                        NttDatalLibrary.AltriDatiGestionaliType _altriDati;
                        if (_dettaglioLinea.AltriDatiGestionali != null)
                            _altriDati = _dettaglioLinea.AltriDatiGestionali.FirstOrDefault();
                        else
                        {
                            _dettaglioLinea.AltriDatiGestionali = new NttDatalLibrary.AltriDatiGestionaliType[] { new NttDatalLibrary.AltriDatiGestionaliType() };
                            _altriDati = _dettaglioLinea.AltriDatiGestionali.FirstOrDefault();
                        }
                        DateTime _dataFattura = body.DatiGenerali.DatiGeneraliDocumento.Data;
                        // [2.2.1.16.1]
                        _altriDati.TipoDato = "POS_FIN";
                        // [2.2.1.16.2]
                        _altriDati.RiferimentoTesto = posFin.Trim();
                        // [2.2.1.16.3]
                        _altriDati.RiferimentoNumero = "0.00";
                        _altriDati.RiferimentoNumeroSpecified = true;
                        // [2.2.1.16.4]
                        _altriDati.RiferimentoData = _dataFattura;
                        _altriDati.RiferimentoDataSpecified = true;

                        // altri dati gestionali 2

                        var _altriDati2 = new NttDatalLibrary.AltriDatiGestionaliType();
                        _dettaglioLinea.AltriDatiGestionali = new NttDatalLibrary.AltriDatiGestionaliType[] { _altriDati, _altriDati2 };

                        _altriDati2.TipoDato = "ANNO_FIN";
                        _altriDati2.RiferimentoTesto = _dataFattura.Year.ToString();
                        _altriDati2.RiferimentoNumero = "0.00";
                        _altriDati2.RiferimentoNumeroSpecified = true;
                        _altriDati2.RiferimentoData = _dataFattura;
                        _altriDati2.RiferimentoDataSpecified = true;
                    }

                    if (!string.IsNullOrWhiteSpace(strDesc) &&
                        !string.IsNullOrWhiteSpace(strQuant) &&
                        !string.IsNullOrWhiteSpace(strPrezUni) &&
                        !string.IsNullOrWhiteSpace(strPrezTot) &&
                        !string.IsNullOrWhiteSpace(strAliquot))
                    {
                        strPrezUni = strPrezUni.Replace("-", "").Replace("+", "");
                        strPrezTot = strPrezTot.Replace("-", "").Replace("+", "");

                        if (!strQuant.Contains('.') && !strQuant.Contains(','))
                            strQuant = strQuant + ".00";
                        if (!strAliquot.Contains('.') && !strAliquot.Contains(','))
                            strAliquot = strAliquot + ".00";

                        Decimal _prezzoUnitario = 0;
                        bool _conversionePrezzoUnitario = Decimal.TryParse(strPrezUni, out _prezzoUnitario);
                        Decimal _prezzoTotale = 0;
                        bool _conversionePrezzoTotale = Decimal.TryParse(strPrezTot, out _prezzoTotale);

                        List<NttDatalLibrary.DettaglioLineeType> _listaLinee = body.DatiBeniServizi.DettaglioLinee.ToList();
                        NttDatalLibrary.DettaglioLineeType _lineaLast = _listaLinee.Last();
                        

                        //2.2.1.8 
                        //if (_dettaglioLinea != null)
                        //{
                        //    _dettaglioLinea.PrezzoUnitario += _prezzoTotale;  // ? 
                        //    _dettaglioLinea.PrezzoTotale += _prezzoTotale;
                        //}
                        if(_lineaLast != null)
                        {
                            _lineaLast.PrezzoUnitario += _prezzoUnitario;
                            _lineaLast.PrezzoTotale += _prezzoTotale;
                        }

                        // nuova linea 
                        NttDatalLibrary.DettaglioLineeType _dettaglioLinea2 = new NttDatalLibrary.DettaglioLineeType();
                        //_dettaglioLinea2.NumeroLinea = "2";
                        _dettaglioLinea2.NumeroLinea = (_listaLinee.Count + 1).ToString();
                        _dettaglioLinea2.Descrizione = strDesc.Trim();
                        Decimal _quantitaLinea2;
                        //_dettaglioLinea2.QuantitaSpecified = Decimal.TryParse(strQuant.Replace(",", "."), out _quantitaLinea2);
                        _dettaglioLinea2.QuantitaSpecified = Decimal.TryParse(strQuant.Replace(".", ","), out _quantitaLinea2);
                        _dettaglioLinea2.Quantita = _quantitaLinea2;
                        _dettaglioLinea2.PrezzoUnitario = _prezzoUnitario > 0 ? -1 * _prezzoUnitario : _prezzoUnitario;
                        _dettaglioLinea2.PrezzoTotale = _prezzoTotale > 0 ? -1 * _prezzoTotale : _prezzoTotale;
                        Decimal _aliquotaLinea2 = 0;
                        //Decimal.TryParse(strAliquot.Replace(",", "."), out _aliquotaLinea2);
                        Decimal.TryParse(strAliquot.Replace(".", ","), out _aliquotaLinea2);
                        _dettaglioLinea2.AliquotaIVA = _aliquotaLinea2;

                        _listaLinee.Add(_dettaglioLinea2);
                        body.DatiBeniServizi.DettaglioLinee = _listaLinee.ToArray();

                        //body.DatiBeniServizi.DettaglioLinee = new NttDatalLibrary.DettaglioLineeType[] { _dettaglioLinea, _dettaglioLinea2 };
                    }


                }
                #endregion

                #region Creazione XML
                
                XmlDocument xmlDoc = FatturaPARepository.GenerateXMLFromFattura(fattura); new XmlDocument();
                if (xmlDoc == null)
                    return false;

                #endregion

                SetSessionValue("invoiceXML", xmlDoc);

                DocsPaWR.FileDocumento preview = getInvoicePreview(xmlDoc.InnerXml);
                //DocsPaWR.FileDocumento preview = getInvoicePreview(XmlDocPreview.OuterXml);
                SetSessionValue("invoicePreview", preview);
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
            
            return true;

        }

        private static DocsPaWR.FileDocumento getInvoicePreview(string invoice)
        {
            try
            {
                string urlXSL = System.Configuration.ConfigurationManager.AppSettings["FATTURAPA_XSL_URL"];

                XmlDocument previewInvoice = new XmlDocument();
                //string decl = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"; 
                string decl = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                string pi = "<?xml-stylesheet type=\"text/xsl\" href=\"" + urlXSL + "\"?>";
                string previewXml = invoice.Replace(decl, decl + "\n" + pi);
                previewInvoice.LoadXml(previewXml);

                Byte[] bytes = Encoding.UTF8.GetBytes(previewInvoice.OuterXml);
                DocsPaWR.FileDocumento fileDoc = new DocsPaWR.FileDocumento();

                fileDoc.cartaceo = false;
                fileDoc.contentType = "text/xml";
                fileDoc.content = bytes;
                fileDoc.length = bytes.Length;

                return fileDoc;
            }
            catch (Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DocsPaWR.FileDocumento getInvoicePreviewPdf(byte[] content)
        {
            try
            {
                return ws.FatturazioneGetPreviewPdf(content);
            }
            catch(Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
            
        }

        #region Session functions
        /// <summary>
        /// Reperimento valore da sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        private static System.Object GetSessionValue(string sessionKey)
        {
            try
            {
                return System.Web.HttpContext.Current.Session[sessionKey];
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Impostazione valore in sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="sessionValue"></param>
        private static void SetSessionValue(string sessionKey, object sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session[sessionKey] = sessionValue;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Rimozione chiave di sessione
        /// </summary>
        /// <param name="sessionKey"></param>
        private static void RemoveSessionValue(string sessionKey)
        {
            try
            {
                System.Web.HttpContext.Current.Session.Remove(sessionKey);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        //private static DocsPaFatturazioneWR.InfoUtente getInfoUtente()
        //{
        //    try
        //    {
        //        Utente user = HttpContext.Current.Session["userData"] as Utente;
        //        DocsPaWR.Ruolo role = HttpContext.Current.Session["userRole"] as DocsPaWR.Ruolo;

        //        DocsPaFatturazioneWR.InfoUtente infoUtente = new DocsPaFatturazioneWR.InfoUtente();
        //        if (user != null && role != null)
        //        {
        //            infoUtente.idPeople = user.idPeople;
        //            infoUtente.dst = user.dst;
        //            infoUtente.idAmministrazione = user.idAmministrazione;
        //            infoUtente.userId = user.userId;
        //            infoUtente.sede = user.sede;
        //            if (user.urlWA != null)
        //                infoUtente.urlWA = user.urlWA;
        //            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["userDelegato"] != null)
        //                infoUtente.delegato = HttpContext.Current.Session["userDelegato"] as DocsPaFatturazioneWR.InfoUtente;

        //            infoUtente.idCorrGlobali = role.systemId;
        //            infoUtente.idGruppo = role.idGruppo;

        //            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"]))
        //                infoUtente.codWorkingApplication = System.Configuration.ConfigurationManager.AppSettings["CODICE_APPLICAZIONE"].ToString();
        //        }

        //        return infoUtente;

        //    }
        //    catch (System.Exception ex)
        //    {
        //        UIManager.AdministrationManager.DiagnosticError(ex);
        //        return null;
        //    }
        //}

        #endregion

    }


}