using System;
using System.Xml;
using System.Data;
using System.Globalization;
//using Debugger = DocsPaUtils.LogsManagement.Debugger;
using System.Configuration;
using BusinessLogic.interoperabilita;
using DocsPaVO.ProfilazioneDinamica;
using BusinessLogic.Utenti;
using System.Collections.Generic;
using DocsPaUtils.Configuration;
using System.Text.RegularExpressions;
using BusinessLogic.Documenti;
using DocsPaVO.documento;
using DocsPaVO.utente;
using System.IO;
using System.Text;
using System.Collections;


namespace BusinessLogic.interoperabilità
{
    public class InteroperabilitaSendXmlInAttach
    {

        public bool ManageAttachXML_Suap(ref DocsPaVO.documento.SchedaDocumento schedaDocOriginale,DocsPaVO.utente.InfoUtente infoUtenteInterop,string mailFrom)
        {
            if (schedaDocOriginale.template == null)
                return false;

            if (schedaDocOriginale.template.DESCRIZIONE.ToUpper() != "ENTESUAP")
                return false;

            XmlParsing.suap.SuapManager sm = new XmlParsing.suap.SuapManager("ENTESUAP");

            //refresh
            DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtenteInterop, schedaDocOriginale.docNumber, schedaDocOriginale.docNumber);

            string xmlResult = sm.ExportEnteSuapXML(infoUtenteInterop, schedaDoc, mailFrom);
            if (String.IsNullOrEmpty (xmlResult ))
                return false;



            Allegato allEntesuap = null;


            foreach (Allegato all in schedaDoc.allegati)
            {
                string originalName = BusinessLogic.Documenti.FileManager.getOriginalFileName(infoUtenteInterop, all);
                if ((originalName != null) && (originalName.ToLowerInvariant().Equals("entesuap.xml")))
                {
                    allEntesuap = all;
                    break;
                }
            }

            //Add del File xml come filedocumento:
            DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
            fdAllNew.content = Encoding.UTF8.GetBytes(xmlResult);
            fdAllNew.length = fdAllNew.content.Length;
            fdAllNew.name = "ENTESUAP.XML";
            fdAllNew.fullName = fdAllNew.name;
            fdAllNew.contentType = "text/xml";
            string err;
            if (allEntesuap != null)
            {
                FileDocumento fd = BusinessLogic.Documenti.FileManager.getFile(allEntesuap, infoUtenteInterop);
                string xmlAll = System.Text.ASCIIEncoding.ASCII.GetString(fd.content);
                //
                if (XmlParsing.suap.SuapManager.compareEnteSuapXml(xmlAll, xmlResult))
                    return true;
                else
                {
                    
                    FileRequest fileReq = new Documento();
                    fileReq.docNumber = allEntesuap.docNumber;
                    fileReq.descrizione = "Versione creata per modifiche apportate ai dati contenuti nel file ENTESUAP.xml";
                    DocsPaVO.documento.FileRequest fr = VersioniManager.addVersion(fileReq, infoUtenteInterop, false);


                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtenteInterop, out err))
                        throw new Exception(err);
                    else
                    {
                        SchedaDocumento sdNew = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtenteInterop, schedaDoc.docNumber, schedaDoc.docNumber);
                        schedaDocOriginale.allegati = sdNew.allegati;
                        return true;
                    }
                }
            }
            else
            {
                DocsPaVO.documento.Allegato  allegato = new DocsPaVO.documento.Allegato();
                allegato.descrizione = "ENTESUAP.XML";
                allegato.numeroPagine = 1;
                allegato.docNumber = schedaDoc.docNumber;
                allegato.version = "0";
                allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtenteInterop, allegato);

                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)allegato;
                if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtenteInterop, out err))
                    throw new Exception(err);
                else
                {
                    SchedaDocumento sdNew = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtenteInterop, schedaDoc.docNumber, schedaDoc.docNumber);
                    schedaDocOriginale.allegati = sdNew.allegati;
                    return true;
                }
            }


            //presume insuccesso (mai na gioa)..
            //return false;
        }

        public bool ManageAttachXML(DocsPaVO.documento.SchedaDocumento schedaDocOriginale, DocsPaVO.documento.SchedaDocumento schedaDocCopiato, string IdRuoloMittenteFisico, DocsPaVO.utente.InfoUtente infoUtenteInterop)
        {
            //****************************************************************************************************************//
            // Modifica Evolutiva di Giordano Iacozzilli Data: 27/04/2012
            // 
            // La presidenza del Consiflio dei Ministri richiede la possibilità di inviare via Interoperabilità interna,
            // al Registro dell'Ufficio UBRRAC un file Xml contenente un insieme di dati integrativi associati ad alcune tipologie 
            // di documenti prodotti dai centri di spesa per una successiva elaboraizone dal sistema OpenDGov.
            //
            //Workflow:
            // 1) Controllo presenza su db del codice FE_ATTACH_XML nella Config Globali
            // 2) Se il controllo è ok, verifico che il tipo doc sia compreso nella lista dei tipi ammessi(GET DEI TIPI DOC COL PIPE DAL DB).
            // 3) Verifico che il mio Ruolo abbia la visibilità su tutti i campi, altrimenti nada.
            // 4) Creo L'xml e lo allego alla scheda Doc
            //
            //****************************************************************************************************************//

            //Doppio controllo, interop Semplificata e Xml Attach.
            if (System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"] != null &&
           System.Configuration.ConfigurationManager.AppSettings["INTEROP_INT_NO_MAIL"].ToString() != "0")
            {
                //1) Controllo presenza su db del codice FE_ATTACH_XML nella Config Globali
                if (GET_XML_ATTACH())
                {
                    //Get Template.
                    DocsPaVO.ProfilazioneDinamica.Templates template = (schedaDocOriginale.template);
                    string err = string.Empty;
                    //verifico che il tipo doc sia compreso nella lista dei tipi ammessi
                    if (template != null && GET_TIPI_ATTI_CUSTOM().Contains(ClearString(GetDescrTipoAtto(template.ID_TIPO_ATTO, infoUtenteInterop.idAmministrazione))))
                    {
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli _ass = new AssDocFascRuoli();
                        //Verifico che il mio Ruolo abbia la visibilità su tutti i campi, altrimenti nada.
                        int _totCampi = template.ELENCO_OGGETTI.Count;
                        int _ContCampi = 0;

                        for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                        {
                            OggettoCustom oggettoCustom = (OggettoCustom)template.ELENCO_OGGETTI[i];
                            // visibilità.
                            _ass = ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampoTipologiaDoc(IdRuoloMittenteFisico, template.SYSTEM_ID.ToString(), oggettoCustom.SYSTEM_ID.ToString());
                            if (_ass.VIS_OGG_CUSTOM == "1")
                                _ContCampi = _ContCampi + 1;
                        }

                        //Verifico che il mio Ruolo abbia la visibilità su tutti i campi
                        if (_ContCampi == _totCampi)
                        {
                            if (schedaDocCopiato.documenti != null && schedaDocCopiato.documenti[0] != null)
                            {
                                try
                                {
                                    Dictionary<string, string> _dict = new Dictionary<string, string>();
                                    foreach (OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                    {
                                        _dict.Add(oggettoCustom.DESCRIZIONE, oggettoCustom.VALORE_DATABASE);
                                    }
                                    //Creo L'xml e lo allego alla scheda Doc
                                    XmlDocument xDocDaAllegare = new XmlDocument();
                                    xDocDaAllegare = CreateXmlToAttach(_dict, schedaDocCopiato, GetDescrTipoAtto(template.ID_TIPO_ATTO, infoUtenteInterop.idAmministrazione));

                                    DocsPaVO.documento.Allegato allegato = null;

                                    //Add Allegato:
                                    allegato = new DocsPaVO.documento.Allegato();
                                    allegato.descrizione = GetDescrTipoAtto(template.ID_TIPO_ATTO, infoUtenteInterop.idAmministrazione) + "_XML";
                                    allegato.numeroPagine = 1;
                                    allegato.docNumber = schedaDocCopiato.docNumber;
                                    allegato.version = "0";
                                    allegato.position = (schedaDocOriginale.allegati.Count + 1);

                                    allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtenteInterop, allegato);

                                    //Add del File xml come filedocumento:
                                    DocsPaVO.documento.FileDocumento fdAllNew = new DocsPaVO.documento.FileDocumento();
                                    fdAllNew.content = Encoding.UTF8.GetBytes(xDocDaAllegare.OuterXml);
                                    fdAllNew.length = Encoding.UTF8.GetBytes(xDocDaAllegare.OuterXml).Length;
                                    fdAllNew.name = GetDescrTipoAtto(template.ID_TIPO_ATTO, infoUtenteInterop.idAmministrazione) + "_XML" + ".xml";
                                    fdAllNew.fullName = fdAllNew.name;
                                    fdAllNew.contentType = "text/xml";
                                    DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)allegato;
                                    if (!BusinessLogic.Documenti.FileManager.putFile(ref fr, fdAllNew, infoUtenteInterop, out err))
                                        throw new Exception(err);

                                }
                                catch (Exception ex)
                                {
                                    err = ex.Message;
                                    throw ex;
                                }
                            }
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;

                return true;
            }
            else
                return false;
        }


        private string GetDescrTipoAtto(string idTipoAtto, string idamm)
        {
            ArrayList _arr = BusinessLogic.Documenti.DocManager.getTipologiaAtto(idamm);
            foreach (DocsPaVO.documento.TipologiaAtto _ta in _arr)
            {
                if (_ta.systemId == idTipoAtto)
                {
                    return _ta.descrizione;
                }
            }
            return "";
        }


        private XmlDocument CreateXmlToAttach(Dictionary<string, string> _dict, DocsPaVO.documento.SchedaDocumento schedaDocPartenza, string root)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();

                XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
                xDoc.AppendChild(xDecl);

                //add root node
                XmlElement xRoot = xDoc.CreateElement("", ClearString(root), "");
                xDoc.AppendChild(xRoot);
              

                foreach (KeyValuePair<string, string> kvp in _dict)
                {
                    UTF8Encoding utf8 = new UTF8Encoding();
                    XmlElement xElem = xDoc.CreateElement(ClearString(kvp.Key.ToString().ToLower()));
                    try
                    {
                        DateTime _dt = new DateTime();
                        _dt = Convert.ToDateTime(kvp.Value.ToString());
                        xElem.InnerText = _dt.ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        String unicodeString = kvp.Value;
                        Byte[] encodedBytes = utf8.GetBytes(unicodeString);
                        String decodedString = utf8.GetString(encodedBytes);
                        xElem.InnerText = decodedString;
                    }
                    if (!string.IsNullOrEmpty(kvp.Value))
                        xRoot.AppendChild(xElem);
                }
                return xDoc;
            }
            catch
            {
                return null;
            }

        }

        private string PurifyStringEncUTF8(string str2purify)
        {
            try
            {
                System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;

                // Convert a string to utf-8 bytes.
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str2purify);

                // Convert utf-8 bytes to a string.
                string s_unicode2 = System.Text.Encoding.UTF8.GetString(utf8Bytes);
                return s_unicode2;
            }
            catch (EncoderFallbackException e)
            {
                return str2purify;
            }

        }

        public static string ClearString(string source)
        {
            //Debug: Giordano: modificare poi con le REX come dio comanda.
            string trim = Regex.Replace(source, @" ", "").Replace(@"'", "").Replace(".", "");
            return trim;
        }

        private bool GET_XML_ATTACH()
        {
            if (InitConfigurationKeys.GetValue("0", "BE_ATTACH_XML") != null && InitConfigurationKeys.GetValue("0", "BE_ATTACH_XML").Equals("1"))
            {
                return true;
            }
            return false;
        }

        private List<string> GET_TIPI_ATTI_CUSTOM()
        {
            if (InitConfigurationKeys.GetValue("0", "BE_CUSTOM_TIPI_ATTO") != null)
            {
                return new List<string>(InitConfigurationKeys.GetValue("0", "BE_CUSTOM_TIPI_ATTO").Split('|'));
            }
            return null;
        }

    }


}


