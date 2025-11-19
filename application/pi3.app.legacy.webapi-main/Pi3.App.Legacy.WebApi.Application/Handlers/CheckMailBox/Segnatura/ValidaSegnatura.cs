// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.FlussoAutomatico;
using DocsPaVO.Interoperabilita.Segnatura;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Pi3.Core.Services.Configuration;
using System.Net.Mail;
using DocsPaVO.documento;
using System.Security.Cryptography;
using DocumentFormat.OpenXml.Drawing;
using Pi3.Core.Services.File.FirmaDigitale2;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class ValidaSegnatura
    {
        public bool documentOk = false;
        public string eccezioneXml = null;
        //Andrea De Marco - Variabile booleana aggiunta per gestire i controlli Bloccanti/non bloccanti
        public bool controlloBloccante = true;
        public bool searchAttachByDescription = false;
        string dettagli_eccezione = string.Empty;
        //End Andrea De Marco
        public string numRegMitt = string.Empty;
        protected readonly ILogger<ValidaSegnatura> _logger;

        public ValidaSegnatura(string xmlSegnatura, AnalyzedMessage messaggio, Email email, IPi3DbContext dbContext,
            IFirmaDigitale2Service firmaDigitale2Service, string eccezioneBloccante, string proseguiSigilloMancante, string checkMailNotReadBase64, out string validaSegnErrorMessage,  out bool searchAttachSegnaturabyDescription)
        {
            validaSegnErrorMessage = string.Empty;
            searchAttachSegnaturabyDescription = false;
            CheckSegnatura(xmlSegnatura, messaggio, email, eccezioneBloccante, proseguiSigilloMancante, checkMailNotReadBase64, dbContext, firmaDigitale2Service, out validaSegnErrorMessage, out searchAttachSegnaturabyDescription);            
        }

        private void CheckSegnatura(string xmlSegnatura, AnalyzedMessage messaggio, Email email, string eccezioneBloccante, string proseguiSigilloMancante, string checkMailNotReadBase64, IPi3DbContext dbContext, IFirmaDigitale2Service firmaDigitale2Service, out string validaSegnErrorMessage, out bool searchAttachSegnaturabyDescription)
        {
            validaSegnErrorMessage = string.Empty;
            searchAttachSegnaturabyDescription = false;
            documentOk = false;
            numRegMitt = messaggio.Subject;
            string anomalia = string.Empty;

            //Verifico se l'xml è valido rispetto all'xsd
            if (!IsDtdValid(xmlSegnatura, messaggio))
            {
                //Eccezione
                controlloBloccante = false;
                if (!string.IsNullOrEmpty(eccezioneBloccante) && eccezioneBloccante.Equals("1"))
                    controlloBloccante = true;
                anomalia = EccezioniSegnatura.EccezioneSegnaturaNonValida; // "000_Irricevibile. Il file segnatura.xml non risulta valido";
                eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject);
                if (eccezioneXml != null && eccezioneXml.Length > 0)
                    validaSegnErrorMessage = controlloBloccante ? string.Format(Resources.MsgControlloBloccante, anomalia) : Resources.MsgControlloNonBloccante;
                return;
            }
            //Verifico se è possibile elaborare la segnatura.xml
            if (!CheckErrorInLoadXml(email))
            {
                //Eccezione
                controlloBloccante = true;
                anomalia = EccezioniSegnatura.EccezioneSegnaturaNonElaborabile; // "000_Irricevibile. Impossibile elaborare il file segnatura.xml";
                eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject);
                if (eccezioneXml != null && eccezioneXml.Length > 0)
                    validaSegnErrorMessage = string.Format(Resources.MsgControlloBloccante, anomalia); 
                return;
            }
            SegnaturaInformaticaType sexml = DeserializeObject<SegnaturaInformaticaType>(xmlSegnatura);
            if (sexml == null)
            {
                xmlSegnatura = xmlSegnatura.Replace("xmlns=\"http://www.agid.gov.it/protocollo/pec/\"", "xmlns=\"http://www.agid.gov.it/protocollo/\"");
                sexml = DeserializeObject<SegnaturaInformaticaType>(xmlSegnatura);
            }
            IdentificatoreType identificatore = sexml.Intestazione.Identificatore;
            //Verifico se la struttura della segnatura è corretta
            //0: SENZA ECCEZIONE  1:CON ECCEZIONE NON BLOCCANTE  2: CON ECCEZIONE BLOCCANTE
            string livelloAnomaliaSegnatura = GetLivelloAnomaliaSegnatura(email.Sender.Address, "STRUTTURA_VALIDA", dbContext).Result;
            if (!IsStrutturaValida(email, sexml))
            {
                if (livelloAnomaliaSegnatura.Equals("0"))
                {
                    //this._logger.LogDebug("Struttura non valido: cerco per descrizione file");
                    searchAttachByDescription = true;
                }
                else
                {
                    //Eccezione
                    controlloBloccante = livelloAnomaliaSegnatura.Equals("2");
                    anomalia = EccezioniSegnatura.EccezioneSegnaturaFileNonCorrispondenti; // "000_Irricevibile. Impossibile elaborare il file segnatura.xml. I file della segnatura non corrispondono a quelli inviati nella mail.";
                    eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject, identificatore);
                    if (eccezioneXml != null && eccezioneXml.Length > 0)
                        validaSegnErrorMessage = controlloBloccante ? string.Format(Resources.MsgControlloBloccante, anomalia) : Resources.MsgControlloNonBloccante;
                    return;
                }
            }

            //Verifico la firma del file di segnatura
            SignatureType signature = sexml.Signature;
            if (signature == null)
            {
                //Se a 0, invio eccezione e non elaboro la mail;
                //se ad 1, invia l''eccezione ed elaboro la mail;
                //se a 2, non invio eccezione ed elaboro la mail.
                if (!(string.IsNullOrEmpty(proseguiSigilloMancante) || proseguiSigilloMancante.Equals("0")))
                {
                    if (proseguiSigilloMancante.Equals("1"))
                    {
                        controlloBloccante = false;
                        anomalia = EccezioniSegnatura.EccezioneSegnaturaValidaFirma; //"001_ValidazioneFirma";
                        eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject, identificatore);
                        if (eccezioneXml != null && eccezioneXml.Length > 0)
                            validaSegnErrorMessage = Resources.MsgControlloNonBloccante;
                    }
                }
                else
                {
                    //Eccezione
                    controlloBloccante = true;
                    anomalia = EccezioniSegnatura.EccezioneSegnaturaValidaFirma; //"001_ValidazioneFirma";
                    eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject, identificatore);
                    if (eccezioneXml != null && eccezioneXml.Length > 0)
                        validaSegnErrorMessage = string.Format(Resources.MsgControlloBloccante, anomalia); ;
                    return;
                }
            }
            else
            {
                //0: SENZA ECCEZIONE  1:CON ECCEZIONE NON BLOCCANTE  2: CON ECCEZIONE BLOCCANTE
                string livelloAnomaliaSegnaturaFirmaNonValida = GetLivelloAnomaliaSegnatura(email.Sender.Address, "SEGNATURA_XML_FIRMA_NON_VALIDA", dbContext).Result;
                if (!CheckFirmaSegnaturaProtocollo(email, sexml, proseguiSigilloMancante, checkMailNotReadBase64, firmaDigitale2Service).Result 
                    && livelloAnomaliaSegnaturaFirmaNonValida != "0")
                {
                    //Eccezione
                    controlloBloccante = livelloAnomaliaSegnaturaFirmaNonValida == "2";
                    anomalia = EccezioniSegnatura.EccezioneSegnaturaValidaFirma; //"001_ValidazioneFirma";
                    eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject, identificatore);
                    if (eccezioneXml != null && eccezioneXml.Length > 0)
                        validaSegnErrorMessage = string.Format(Resources.MsgControlloBloccante, anomalia); 
                    return;
                }
            }

            //Verifica corrispondenza impronta file
            if (!CheckImprontaFile(email, sexml) && livelloAnomaliaSegnatura != "0")
            {
                //Eccezione
                controlloBloccante = livelloAnomaliaSegnatura == "2";
                anomalia = EccezioniSegnatura.EccezioneSegnaturaAnomaliaImpronta; // "002_AnomaliaImpronta";
                eccezioneXml = GeneraEccezioneXml(anomalia, messaggio.Subject, identificatore);
                if (eccezioneXml != null && eccezioneXml.Length > 0)
                    validaSegnErrorMessage = string.Format(Resources.MsgControlloBloccante, anomalia);
                return;
            }

            //if (eccezioneXml != null && eccezioneXml.Length > 0)
            //    validaSegnErrorMessage = controlloBloccante ? Resources.MsgControlloBloccante : Resources.MsgControlloNonBloccante;

            //if (eccezioneXml != null && eccezioneXml.Length > 0)
            //    this._logger.LogDebug($"Generata Eccezione {anomalia}");
        }

        private bool CheckImprontaFile(Email email, SegnaturaInformaticaType sexml)
        {
            //this._logger.LogDebug("INIZIO CheckImprontaFile");
            bool retval = true;
            Dictionary<string, byte[]> lstAllegatiSegnatura = new Dictionary<string, byte[]>();
            lstAllegatiSegnatura.Add(Resources.NomeFileSegnatura, null); //Esiste per forza.. se no non staremo qui.

            if (sexml.Descrizione.Allegato != null)
            {
                foreach (object obj in sexml.Descrizione.Allegato)
                {
                    DocumentoType documento = obj as DocumentoType;
                    if (documento != null)
                    {
                        if (searchAttachByDescription)
                            lstAllegatiSegnatura.Add(documento.Descrizione.ToLower(), documento.Impronta.Value);
                        else
                            lstAllegatiSegnatura.Add(documento.nomeFile.ToLower(), documento.Impronta.Value);

                    }
                }
            }

            if (sexml.Descrizione.DocumentoPrimario != null)
            {
                DocumentoType docPrincipaleSegnatura = sexml.Descrizione.DocumentoPrimario as DocumentoType;
                if (docPrincipaleSegnatura != null && docPrincipaleSegnatura.nomeFile != null)
                {
                    if (searchAttachByDescription)
                        lstAllegatiSegnatura.Add(docPrincipaleSegnatura.Descrizione.ToLower(), docPrincipaleSegnatura.Impronta.Value);
                    else
                        lstAllegatiSegnatura.Add(docPrincipaleSegnatura.nomeFile.ToLower(), docPrincipaleSegnatura.Impronta.Value);
                }
            }
            foreach (EmailContentAttachment att in email.Attachments)
            {
                //Verifico l'impronta
                if (!att.FileName.ToLower().Equals(Resources.NomeFileSegnatura))
                {
                    //this._logger.LogDebug("NOME ALLEGATO: " + att.FileName);
                    bool chekImpronta = lstAllegatiSegnatura[att.FileName.ToLower()] != null && ComputeHashAsSha256(att.Content).ToString().Equals(lstAllegatiSegnatura[att.FileName.ToLower()].ToString());
                    if (!chekImpronta)
                    {
                        return false;
                    }
                }
            }
            //this._logger.LogDebug("FINE CheckImprontaFile");
            return retval;
        }

        private async Task<bool> CheckFirmaSegnaturaProtocollo(Email email, SegnaturaInformaticaType sexml, string proseguiSigilloMancante, string checkMailNotReadBase64, IFirmaDigitale2Service firmaDigitale2Service)
        {
            bool retval = true;

            SignatureType signature = sexml.Signature;
            if (signature == null)
            {
                if (!string.IsNullOrEmpty(proseguiSigilloMancante) && proseguiSigilloMancante.Equals("1"))
                    return true;
                else
                    return false;
            }

            EmailContentAttachment segnatura = null;
            foreach (EmailContentAttachment att in email.Attachments)
            {
                if (att.FileName.ToLower().Equals(Resources.NomeFileSegnatura))
                {
                    segnatura = att;
                    break;
                }
            }

            FileDocumento fileDoc = new FileDocumento();
            fileDoc.name = Resources.NomeFileSegnatura;
            fileDoc.content = segnatura.Content;
            int oldLen = fileDoc.content.Length;
            try
            {
                if (string.IsNullOrEmpty(checkMailNotReadBase64) ||
                    !checkMailNotReadBase64.Equals("1"))
                {
                    byte[] deb64Content = ReadBase64(fileDoc.content);
                    if (deb64Content != null)
                    {
                        fileDoc.content = deb64Content;
                        fileDoc.length = fileDoc.content.Length;
                    }
                }
                retval = await this.VerifyFileSignatureXADES(fileDoc, firmaDigitale2Service);
            }
            catch (Exception e)
            {
                //this._logger.LogError($"Qualcosa è andato storto durante la VerifyFileSignature {e.Message}");
                //Qualcosa è andato storto...
                return false;
            }

            return retval;
        }

        private async Task<bool> VerifyFileSignatureXADES(FileDocumento fileDoc, IFirmaDigitale2Service firmaDigitale2Service)
        {
            var verificaFirmaResponse = new VerificaResponse();

            try
            {
                verificaFirmaResponse = await firmaDigitale2Service.Verifica(new VerificaRequest()
                {
                    FileFirmato = fileDoc.content,
                    VerificaCompleta = false
                });
            }
            catch (Exception ex)
            {
                return false;
            }

            if (verificaFirmaResponse.Warning != null)
                return false;

            return true;
        }

        private bool IsStrutturaValida(Email email, SegnaturaInformaticaType sexml)
        {
            bool retval = true;
            List<String> lstAllegatiSegnatura = new List<string>();
            lstAllegatiSegnatura.Add(Resources.NomeFileSegnatura); //Esiste per forza.. se no non staremo qui.

            if (sexml.Descrizione.Allegato != null)
            {
                foreach (object obj in sexml.Descrizione.Allegato)
                {
                    DocumentoType documento = obj as DocumentoType;
                    if (documento != null)
                        lstAllegatiSegnatura.Add(documento.nomeFile.ToLower());
                }
            }

            if (sexml.Descrizione.DocumentoPrimario != null)
            {
                DocumentoType docPrincipaleSegnatura = sexml.Descrizione.DocumentoPrimario as DocumentoType;
                if (docPrincipaleSegnatura != null && docPrincipaleSegnatura.nomeFile != null)
                    lstAllegatiSegnatura.Add(docPrincipaleSegnatura.nomeFile.ToLower());
            }

            if (lstAllegatiSegnatura.Count != email.Attachments.Count)
                return false;

            foreach (EmailContentAttachment att in email.Attachments)
                if (!lstAllegatiSegnatura.Contains(att.FileName.ToLower()))
                    return false;

            return retval;
        }

        private bool CheckErrorInLoadXml(Email email)
        {
            bool result = true;

            try
            {
                foreach (EmailContentAttachment a in email.Attachments)
                {
                    if (a.FileName.ToLower().Equals(Resources.NomeFileSegnatura))
                    {
                        // Controllo encoding
                        XmlDocument doc = new XmlDocument();
                        InteropResolver my = new InteropResolver();
                        System.IO.MemoryStream ms = new System.IO.MemoryStream(a.Content);
                        XmlTextReader xtr = new XmlTextReader(ms);
                        xtr.WhitespaceHandling = WhitespaceHandling.None;
                        XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                        xvr.ValidationType = System.Xml.ValidationType.Schema;
                        xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                        xvr.XmlResolver = my;
                        try
                        {
                            doc.Load(xvr);
                        }
                        catch (System.Xml.XmlException e)
                        {
                            //this._logger.LogError("Eccezione CheckErrorInLoadXml:" + e.Message);
                            //this._logger.LogDebug("Errore CheckErrorInLoadXml stackTrace : " + e.StackTrace);
                            result = false;
                        }
                        catch (Exception e)
                        {
                            //this._logger.LogError("Eccezione:" + e.Message);
                        }
                        finally
                        {
                            xvr.Close();
                            xtr.Close();
                        }
                    }

                }
            }
            catch (Exception e)
            {
                //this._logger.LogError("Errore in checkEncoding: " + e.Message);
            }

            return result;
        }

        bool IsDtdValid(string segnaturaXml, AnalyzedMessage messaggio)
        {
            try
            {

                Validator valid = new Validator(segnaturaXml);
                if (valid.ValidationErrorList.Count() == 0)
                {
                    return true;
                }

                if (valid.ValidationErrorList.Count() == 1 && valid.ValidationErrorList[0].Contains("xmldsig#:Signature"))
                    return true;

                //foreach (string ve in valid.ValidationErrorList)
                //    this._logger.LogDebug(String.Format("Errori durante la validazione di segnatura XML {0}", ve));

            }
            catch (Exception e)
            {
                //this._logger.LogError(string.Format("Errori durante la validazione di segnatura XML {e}", e));
                dettagli_eccezione = " " + e.Message.ToString();
            }

            return false;
        }

        public string GeneraEccezioneXml(string motivo, string descrizioneMessaggio, IdentificatoreType identificatore = null)
        {
            DocsPaVO.Interoperabilita.Segnatura.NotificaEccezioneType notificaEccezione = new NotificaEccezioneType();
            notificaEccezione.Motivo = motivo;

            if (identificatore == null)
            {
                DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3 item = DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3.DescrizioneMessaggio;
                notificaEccezione.MessaggioRicevuto = new DocsPaVO.Interoperabilita.Segnatura.MessaggioRicevutoType();
                notificaEccezione.MessaggioRicevuto = new MessaggioRicevutoType
                {
                    ItemsElementName = new DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3[] { item },
                    Items = new object[] { descrizioneMessaggio = descrizioneMessaggio }
                };
            }
            else
            {
                DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3 item = DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3.Identificatore;
                notificaEccezione.MessaggioRicevuto = new DocsPaVO.Interoperabilita.Segnatura.MessaggioRicevutoType();
                notificaEccezione.MessaggioRicevuto = new MessaggioRicevutoType
                {
                    ItemsElementName = new DocsPaVO.Interoperabilita.Segnatura.ItemsChoiceType3[] { item },
                    Items = new object[] { identificatore = identificatore }
                };
            }

            return StringWriterWithEncoding.ToXmlString(notificaEccezione);

        }

        public static async Task<string> GetLivelloAnomaliaSegnatura(string email, string anomalia, IPi3DbContext dbContext)
        {
            // 0: nessun'anomalia
            // 1: eccezione non bloccante
            // 2: eccezione bloccante
            List<string> emailList = new List<string>() { email.ToUpper() };
            string[] s = email.Split('@');
            if (s != null && s.Count() > 1)
                emailList.Add($"*{s[1].ToUpper()}");

            return await dbContext.LivelloAnomaliaSegnaturaEntities
                .Where(x => emailList.Contains(x.VAR_EMAIL.ToUpper()) && 
                    x.VAR_ANOMALIA.ToUpper().Equals(anomalia.ToUpper()))
                .Select(x => x.CHA_LIVELLO_ECCEZIONE)
                .FirstOrDefaultAsync() ?? "2";  //se non presente eccezione bloccante
        }

        static t DeserializeObject<t>(String pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(t));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            XmlTextReader reader = new XmlTextReader(new StringReader(pXmlizedString));
            try
            {
                return (t)xs.Deserialize(reader);
            }
            catch (Exception e) { System.Console.WriteLine(e); return default(t); }
        }
        static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        static byte[] ReadBase64(byte[] inBase64Bytes)
        {
            MemoryStream msIn = new MemoryStream(inBase64Bytes);
            MemoryStream msOut = new MemoryStream();
            StreamReader sr = new StreamReader(msIn);
            while (true)
            {
                string line = sr.ReadLine();
                if (String.IsNullOrEmpty(line))
                    break;

                byte[] temp = null;

                try
                {
                    temp = Convert.FromBase64String(line);
                }
                catch
                {
                    return null;
                }
                msOut.Write(temp, 0, temp.Length);
            }
            return msOut.ToArray();
        }

        private byte[] ComputeHashAsSha256(byte[] content)
        {
            using (var algorithm = HashAlgorithm.Create("SHA256"))
                return algorithm.ComputeHash(content);
        }
    }
}
