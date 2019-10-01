using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;
using DocsPaWS.VtDocsWS;
using System.ServiceModel;
using VtDocsWS.Services;
using System.Xml;

//temp per debug sistemi esterni - da cancellare
using log4net;

namespace VtDocsWS.WebServices
{
    public class Utils
    {
        // temp per debug sistemi esterni - da cancellare
        private static ILog logger = LogManager.GetLogger(typeof(Utils));

        public static string Encrypt(string toEncrypt)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                //Di default disabilito l'hashing
                bool useHashing = false;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Decrypt(string cipherString)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                //Di default disabilito l'hashing
                bool useHashing = false;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DocsPaVO.utente.InfoUtente getInfoUtenteFromToken(string tokenDiAutenticazione)
        {
            try
            {
                string clearToken = Utils.Decrypt(tokenDiAutenticazione);
                string[] arrayInfoUtente = clearToken.Split('|');

                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                infoUtente.idCorrGlobali = arrayInfoUtente[0];
                infoUtente.idPeople = arrayInfoUtente[1];
                infoUtente.idGruppo = arrayInfoUtente[2];
                infoUtente.dst = arrayInfoUtente[3];
                infoUtente.idAmministrazione = arrayInfoUtente[4];
                infoUtente.userId = arrayInfoUtente[5];
                infoUtente.sede = arrayInfoUtente[6];
                infoUtente.urlWA = arrayInfoUtente[7];

                return infoUtente;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DocsPaVO.utente.Ruolo getRuoloFromInfoUtente(DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtente.idPeople);

                foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                {
                    if (ruolo.systemId == infoUtente.idCorrGlobali)
                        return ruolo;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Reperimento dell'id dell'amministrazione di appartenenza dell'utente
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetIdAmministrazione(string userId)
        {
            string idAmministrazione = BusinessLogic.Utenti.UserManager.getIdAmmUtente(userId);

            if (string.IsNullOrEmpty(idAmministrazione))
            {
                throw new PisException("ADMINISTRATION_NOT_FOUND");
            }

            return idAmministrazione;
        }

        /// <summary>
        /// Reperimento dell'id amministrazione dal codice Amministrazione.
        /// Questo permette di gestire gli utenti multiamministrazione.
        /// </summary>
        /// <param name="codeAdm"></param>
        /// <returns></returns>
        public static string GetIdAdmByCodeAdm(string codeAdm)
        {
            string idAmministrazione = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(codeAdm);
            if (string.IsNullOrEmpty(idAmministrazione))
            {
                throw new PisException("ADMINISTRATION_NOT_FOUND");
            }

            return idAmministrazione;
        }

        /// <summary>
        /// Reperimento del primo ruolo di appartenenza dell'utente
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo GetRuoloPreferito(string idPeople)
        {
            DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));

            if (ruoli != null && ruoli.Length > 0)
            {
                return ruoli[0];
            }
            else
            {
                return null;
            }
        }

        //<summary>

        //</summary>
        //<param name="userName"></param>
        //<returns></returns>
        public static DocsPaVO.utente.InfoUtente Impersonate(string userName)
        {
            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(userName, Utils.GetIdAmministrazione(userName));

            if (utente == null)
                throw new ApplicationException(string.Format("Utente {0} non trovato", userName));

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, Utils.GetRuoloPreferito(utente.idPeople));

            //   Reperimento token superutente
            infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

            return infoUtente;
        }

        public static Domain.Correspondent GetCorrespondent(DocsPaVO.utente.Corrispondente corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Domain.Correspondent result = new Domain.Correspondent();

            if (corr != null)
            {

                result.Address = corr.indirizzo;
                result.AdmCode = corr.codiceAmm;
                result.AOOCode = corr.codiceAOO;
                result.Cap = corr.cap;
                result.City = corr.citta;
                result.Code = corr.codiceRubrica;
                if (!string.IsNullOrEmpty(corr.idRegistro) && !corr.idRegistro.Equals("NULL"))
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(corr.idRegistro);
                    if (reg == null)
                    {
                        //Registro non trovato
                        throw new PisException("REGISTER_NOT_FOUND");
                    }
                    else
                    {
                        result.CodeRegisterOrRF = reg.codRegistro;
                    }
                }
                if (!string.IsNullOrEmpty(corr.tipoCorrispondente))
                {
                    result.CorrespondentType = corr.tipoCorrispondente;
                    if (corr.tipoCorrispondente == "P")
                    {
                        result.Name = (string.IsNullOrEmpty(corr.nome) ? "" : corr.nome);
                        result.Surname = (string.IsNullOrEmpty(corr.cognome) ? "" : corr.cognome);
                    }
                }
                result.Description = corr.descrizione;
                result.Email = corr.email;
                result.Fax = corr.fax;
                result.Id = corr.systemId;
                result.Location = corr.localita;
                result.Nation = corr.nazionalita;
                result.NationalIdentificationNumber = corr.codfisc;
                result.Note = corr.note;
                result.PhoneNumber = corr.telefono1;
                result.IsCommonAddress = corr.inRubricaComune;
                if (corr.canalePref != null && !string.IsNullOrEmpty(corr.canalePref.descrizione))
                {
                    result.PreferredChannel = corr.canalePref.descrizione;
                }
                result.Province = corr.prov;
                if (corr.tipoIE != null)
                {
                    result.Type = corr.tipoIE;
                }

                if (corr.systemId != null)
                {
                    DocsPaVO.utente.Corrispondente dettaglio = BusinessLogic.Utenti.addressBookManager.GetDettaglioCorrispondente(corr.systemId);
                    if (dettaglio != null)
                    {
                        result.Address = dettaglio.indirizzo;
                        result.Cap = dettaglio.cap;
                        result.Province = dettaglio.prov;
                        result.Nation = dettaglio.nazionalita;
                        result.NationalIdentificationNumber = dettaglio.codfisc;
                        result.PhoneNumber = dettaglio.telefono1;
                        result.Fax = dettaglio.fax;
                        result.Note = dettaglio.note;
                        result.City = dettaglio.citta;
                        result.VatNumber = dettaglio.partitaiva;
                        result.Location = dettaglio.localita;
                        //result.IpaCode = dettaglio.codiceIpa;
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Cerca il corrispondente prima tramite systemID, se non lo trova, e la ricerca nella rubrica comune è abilitata, lo cerca poi tramite codice nella rubrica comune.
        /// Restituisce null se il corrispondente non viene risolto
        /// </summary>
        /// <param name="system_id"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente RisolviCorrispondente(string searchKey, InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente corrispondente = null;

            corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(searchKey);
            if (corrispondente == null || string.IsNullOrEmpty(corrispondente.systemId))
            {
                bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                if (rubricaComuneAbilitata)
                {
                    corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(searchKey, infoUtente);
                    if (corrispondente != null && string.IsNullOrEmpty(corrispondente.systemId))
                        corrispondente = null;
                }
            }

            return corrispondente;
        }

        public static Domain.Template GetDetailsTemplateDoc(DocsPaVO.ProfilazioneDinamica.Templates template, string docNumber)
        {
            Domain.Template result = new Domain.Template();

            if (template != null)
            {
                result.Name = template.DESCRIZIONE;
                result.Id = template.SYSTEM_ID.ToString();

                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                if (oggettiCustom != null && oggettiCustom.Length > 0)
                {
                    result.Fields = new Domain.Field[oggettiCustom.Length];
                    int numField = 0;

                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                    {

                        Domain.Field fieldTemp = new Domain.Field();

                        fieldTemp.Id = oggettoCustom.SYSTEM_ID.ToString();
                        fieldTemp.Name = oggettoCustom.DESCRIZIONE;
                        if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                        {
                            fieldTemp.Required = true;
                        }
                        else
                        {
                            fieldTemp.Required = false;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                        {
                            fieldTemp.Type = "MultipleChoise";
                            if (oggettoCustom.VALORI_SELEZIONATI != null && oggettoCustom.VALORI_SELEZIONATI.Count > 0)
                            {
                                fieldTemp.MultipleChoice = new string[oggettoCustom.VALORI_SELEZIONATI.Count];
                                for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Count; i++)
                                {
                                    fieldTemp.MultipleChoice[i] = oggettoCustom.VALORI_SELEZIONATI[i].ToString();
                                }
                            }
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Corrispondente"))
                        {
                            fieldTemp.Type = "Correspondent";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            // LULUCIANI: 
                            //la logica di PITRE prevede che nel template della tipologia nel campo corrispondente sia memorizzato
                            //il dpa-corr-globali.system_id e non la corr.codiceRubrica + " - " + corr.descrizione..
                            //deve essere mantenuta questa logica, perchè l'oggetto template è utilizzato anche per INSERT e se si non inserisce
                            // system_id , ma la corr.codiceRubrica + " - " + corr.descrizione poi si hanno degli errori..
                            //if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                            //{
                            //    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                            //    if (corr != null)
                            //    {
                            //        fieldTemp.Value = corr.codiceRubrica + " - " + corr.descrizione;
                            //    }
                            //}
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CampoDiTesto"))
                        {
                            fieldTemp.Type = "TextField";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("SelezioneEsclusiva"))
                        {
                            fieldTemp.Type = "ExclusiveSelection";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("MenuATendina"))
                        {
                            fieldTemp.Type = "DropDown";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore"))
                        {
                            fieldTemp.Type = "Counter";
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                }
                            }
                            fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Data"))
                        {
                            fieldTemp.Type = "Date";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                        {
                            fieldTemp.Type = "SubCounter";
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                            }
                            fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE;
                        }
                        result.Fields[numField] = fieldTemp;
                        numField++;
                    }
                }

                int idDiagramma = 0;
                DocsPaVO.DiagrammaStato.Stato stato = null;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(template.SYSTEM_ID.ToString());
                if (idDiagramma != 0)
                {
                    diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                    if (diagramma != null)
                    {
                        result.StateDiagram = new Domain.StateDiagram();
                        result.StateDiagram.Description = diagramma.DESCRIZIONE;
                        result.StateDiagram.Id = diagramma.SYSTEM_ID.ToString();
                        stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(docNumber);
                        if (stato != null)
                        {
                            Domain.StateOfDiagram actualState = new Domain.StateOfDiagram();
                            result.StateDiagram.StateOfDiagram = new Domain.StateOfDiagram[1];
                            result.StateDiagram.StateOfDiagram[0] = actualState;
                            result.StateDiagram.StateOfDiagram[0].Description = stato.DESCRIZIONE;
                            result.StateDiagram.StateOfDiagram[0].DiagramId = diagramma.SYSTEM_ID.ToString();
                            result.StateDiagram.StateOfDiagram[0].FinaleState = stato.STATO_FINALE;
                            result.StateDiagram.StateOfDiagram[0].Id = stato.SYSTEM_ID.ToString();
                            result.StateDiagram.StateOfDiagram[0].InitialState = stato.STATO_INIZIALE;
                        }
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static Domain.Register GetRegister(DocsPaVO.utente.Registro registro)
        {
            Domain.Register result = new Domain.Register();
            if (registro != null)
            {
                result.Code = registro.codRegistro;
                result.Description = registro.descrizione;
                result.Id = registro.systemId;
                if (!string.IsNullOrEmpty(registro.chaRF) && registro.chaRF.Equals("1"))
                {
                    result.IsRF = true;
                }
                if (!string.IsNullOrEmpty(registro.stato))
                {
                    if (registro.stato.Equals("A"))
                    {
                        result.State = "Open";
                    }
                    else
                    {
                        result.State = "Closed";
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static Domain.File GetFile(DocsPaVO.documento.FileRequest fileRequest, bool getFile, DocsPaVO.utente.InfoUtente infoUtente, bool segnatura, bool timbro, string path, DocsPaVO.documento.SchedaDocumento schedaDoc, bool FileConFirma, labelPdf position = null)
        {
            Domain.File result = new Domain.File();

            if (fileRequest != null)
            {
                result.Description = fileRequest.descrizione;
                result.Id = fileRequest.docNumber;
                result.VersionId = fileRequest.version;
                if (getFile)
                {
                    DocsPaVO.documento.FileDocumento fileDocumento = null;
                    try
                    {
                        if (FileConFirma)
                        {
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
                        }
                        else if (!segnatura && !timbro)
                        {
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
                        }
                        else
                        {
                            if (position == null)
                                position = new labelPdf();
                            if (segnatura)
                            {
                                if (schedaDoc.documentoPrincipale != null)
                                {
                                    position.orientamento = "orizzontale";
                                    position.position = "15-30";
                                    position.sel_color = "1";
                                    position.sel_font = "1";
                                    position.tipoLabel = true;
                                }
                                fileDocumento = BusinessLogic.Documenti.FileManager.getFileConSegnatura(fileRequest, schedaDoc, infoUtente, path, position, false);
                            }
                            else
                            {
                                if (timbro)
                                {
                                    position.orientamento = "orizzontale";
                                    position.position = "15-30";
                                    position.sel_color = "1";
                                    position.sel_font = "1";
                                    position.tipoLabel = true;
                                    fileDocumento = BusinessLogic.Documenti.FileManager.getFileConSegnatura(fileRequest, schedaDoc, infoUtente, path, position, false);
                                }
                            }
                            // Forzatura per Integrazione.
                            if (fileDocumento == null || string.IsNullOrEmpty(fileDocumento.nomeOriginale))
                                fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);

                        }
                        result.Name = fileDocumento.nomeOriginale;
                        result.Content = fileDocumento.content;
                        result.MimeType = fileDocumento.contentType;
                    }
                    catch
                    {
                        fileDocumento = null;
                    }
                }
                else
                {
                    DocsPaVO.documento.FileDocumento fileInfo = new FileDocumento();
                    fileInfo = BusinessLogic.Documenti.FileManager.getInfoFile(fileRequest, infoUtente);
                    if (fileInfo != null)
                    {
                        result.Name = fileInfo.nomeOriginale;
                        result.MimeType = fileInfo.contentType;
                    }
                }
            }

            return result;
        }

        public static Domain.StateOfDiagram GetStateOfDiagram(DocsPaVO.DiagrammaStato.Stato stato, string idDiagramma)
        {
            Domain.StateOfDiagram result = new Domain.StateOfDiagram();
            if (stato != null)
            {
                result.Description = stato.DESCRIZIONE;
                result.DiagramId = idDiagramma;
                result.FinaleState = stato.STATO_FINALE;
                result.Id = stato.SYSTEM_ID.ToString();
                result.InitialState = stato.STATO_INIZIALE;
            }
            else
            {
                result = null;
            }
            return result;
        }

        public static Domain.Document GetDocumentFromSearchObject(DocsPaVO.Grids.SearchObject obj, bool estrazioneTemplate, DocsPaVO.Grid.Field[] visibilityFields, string codiceAmm)
        {
            Domain.Document result = new Domain.Document();
            string value = string.Empty;
            if (obj != null)
            {
                result.Id = obj.SearchObjectID;
                result.DocNumber = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D1")).FirstOrDefault().SearchObjectFieldValue;
                value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D11")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(value))
                {
                    result.Annulled = true;
                }
                result.ArrivalDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D21")).FirstOrDefault().SearchObjectFieldValue;
                result.Attachments = null;

                result.CreationDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D9")).FirstOrDefault().SearchObjectFieldValue;
                result.DocumentType = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D3")).FirstOrDefault().SearchObjectFieldValue;
                result.IdParent = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_DOCUMENTO_PRINCIPALE")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(result.DocumentType) && result.DocumentType.Equals("G") && string.IsNullOrEmpty(result.IdParent))
                {
                    result.IsAttachments = true;
                }

                result.Object = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D4")).FirstOrDefault().SearchObjectFieldValue;
                result.Signature = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D8")).FirstOrDefault().SearchObjectFieldValue;
                if (string.IsNullOrEmpty(result.Signature) && !result.DocumentType.Equals("G"))
                {
                    result.Predisposed = true;
                }

                value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D16")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                {
                    result.PrivateDocument = true;
                }

                string idRegistro = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_REGISTRO")).FirstOrDefault().SearchObjectFieldValue;
                string codRegistro = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("D2")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(idRegistro) && !string.IsNullOrEmpty(codRegistro))
                {
                    result.Register = new Domain.Register();
                    result.Register.Id = idRegistro;
                    result.Register.Code = codRegistro;
                }

                string idTipoAtto = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_ATTO")).FirstOrDefault().SearchObjectFieldValue;
                string descTipoAtto = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;

                if (!string.IsNullOrEmpty(idTipoAtto) && !string.IsNullOrEmpty(descTipoAtto))
                {
                    result.Template = new Domain.Template();
                    result.Template.Id = idTipoAtto;
                    result.Template.Name = descTipoAtto;
                    if (estrazioneTemplate)
                    {
                        List<Domain.Field> campiPIS = new List<Domain.Field>();
                        Domain.Field campoPIS = null;
                        foreach (DocsPaVO.Grid.Field campoProf in visibilityFields)
                        {
                            campoPIS = new Domain.Field();
                            campoPIS.Id = campoProf.CustomObjectId.ToString();
                            campoPIS.Name = campoProf.Label;
                            campoPIS.Value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(campoProf.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            if (campoPIS.Value.ToUpper().Contains("CONTATORE_DI_REPERTORIO"))
                            {
                                string tempX1 = string.Empty;
                                campoPIS.Value = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(result.Id, codiceAmm, false, out tempX1);
                            }
                            campiPIS.Add(campoPIS);
                        }
                        if (campiPIS.Count > 0)
                        {
                            result.Template.Fields = campiPIS.ToArray();
                        }
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static DocsPaVO.filtri.FiltroRicerca[] addToArrayFiltroRicerca(DocsPaVO.filtri.FiltroRicerca[] array, DocsPaVO.filtri.FiltroRicerca nuovoElemento)
        {
            DocsPaVO.filtri.FiltroRicerca[] nuovaLista;
            if (array != null)
            {
                int len = array.Length;
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[len + 1];
                array.CopyTo(nuovaLista, 0);
                nuovaLista[len] = nuovoElemento;
                return nuovaLista;
            }
            else
            {
                nuovaLista = new DocsPaVO.filtri.FiltroRicerca[1];
                nuovaLista[0] = nuovoElemento;
                return nuovaLista;
            }
        }

        public static Domain.Document GetDetailsDocument(DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.InfoUtente infoUtente, bool GetFile)
        {
            Domain.Document result = new Domain.Document();
            if (documento != null)
            {
                //Dati in comune per tutti
                result.Id = documento.systemId;
                result.Object = documento.oggetto.descrizione;
                if (!string.IsNullOrEmpty(documento.privato) && documento.privato.Equals("1"))
                {
                    result.PrivateDocument = true;
                }
                else
                {
                    result.PrivateDocument = false;
                }
                result.CreationDate = documento.dataCreazione;
                result.DocNumber = documento.docNumber;
                result.DocumentType = documento.tipoProto;

                if (BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled())
                {
                    if (documento.ConsolidationState != null && documento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                    {
                        string message = string.Empty;
                        // Diabilitazione controlli su documento consolidato
                        if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                        {
                            result.ConsolidationState = "ConsolidationState1";
                        }
                        else if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                        {
                            result.ConsolidationState = "ConsolidationState2";
                        }
                    }
                }
                else
                {
                    result.ConsolidationState = string.Empty;
                }

                if (documento.noteDocumento != null && documento.noteDocumento.Count > 0)
                {
                    result.Note = new Domain.Note[documento.noteDocumento.Count];
                    int y = 0;
                    foreach (DocsPaVO.Note.InfoNota tempNot in documento.noteDocumento)
                    {
                        Domain.Note nota = new Domain.Note();
                        nota.Description = tempNot.Testo;
                        nota.Id = tempNot.Id;
                        nota.User = new Domain.User();
                        nota.User.Id = tempNot.UtenteCreatore.IdUtente;
                        nota.User.Description = tempNot.UtenteCreatore.DescrizioneUtente;
                        result.Note[y] = nota;
                        y++;
                    }
                }

                //Se il documento è in cestino
                if (!string.IsNullOrEmpty(documento.inCestino) && documento.inCestino.Equals("1"))
                {
                    result.InBasket = true;
                }
                else
                {
                    result.InBasket = false;
                }

                if (documento.template != null)
                {
                    result.Template = Utils.GetDetailsTemplateDoc(documento.template, documento.docNumber);
                }

                //Documento principale
                if (documento.documenti != null && documento.documenti.Count > 0)
                {
                    DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                    result.MainDocument = Utils.GetFile(versioneCorrente, GetFile, infoUtente, false, false, string.Empty, null, false);
                }

                //Prendi allegati
                if (documento.allegati != null && documento.allegati.Count > 0)
                {
                    int y = 0;
                    result.Attachments = new Domain.File[documento.allegati.Count];
                    foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                    {
                        result.Attachments[y] = Utils.GetFile(tempAll, GetFile, infoUtente, false, false, string.Empty, null, false);
                        y++;
                    }
                }

                if (documento.protocollo != null)
                {

                    if (documento.protocollo.protocolloAnnullato != null)
                    {
                        result.Annulled = true;
                    }
                    else
                    {
                        result.Annulled = false;
                    }

                    if (!string.IsNullOrEmpty(documento.protocollo.segnatura))
                    {
                        result.Signature = documento.protocollo.segnatura;
                    }
                    else
                    {
                        result.Predisposed = true;
                    }

                    if (!string.IsNullOrEmpty(documento.protocollo.dataProtocollazione))
                    {
                        result.ProtocolDate = documento.protocollo.dataProtocollazione;
                    }

                    if (documento.registro != null)
                    {
                        result.Register = Utils.GetRegister(documento.registro);
                    }


                    //CASO PROTOCOLLO IN ARRIVO
                    if (documento.tipoProto.Equals("A"))
                    {
                        if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittente != null)
                        {
                            result.Sender = Utils.GetCorrespondent(((ProtocolloEntrata)documento.protocollo).mittente, infoUtente);
                        }

                        if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittenti != null && ((ProtocolloEntrata)documento.protocollo).mittenti.Count > 0)
                        {
                            result.MultipleSenders = new Domain.Correspondent[((ProtocolloEntrata)documento.protocollo).mittenti.Count];
                            int y = 0;
                            foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloEntrata)documento.protocollo).mittenti)
                            {
                                result.MultipleSenders[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                y++;
                            }
                        }

                        if (((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente != null)
                        {
                            result.ProtocolSender = ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente;
                        }

                        if (((ProtocolloEntrata)documento.protocollo) != null && !string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                        {
                            result.DataProtocolSender = ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente;
                        }

                        if (((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione != null)
                        {
                            result.MeansOfSending = ((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione;
                        }

                        if (((ProtocolloEntrata)documento.protocollo) != null && string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                        {
                            result.ProtocolDate = ((ProtocolloEntrata)documento.protocollo).dataProtocollazione;
                        }

                        if (documento.documenti != null && documento.documenti != null && documento.documenti.Count > 0)
                        {
                            result.ArrivalDate = ((DocsPaVO.documento.Documento)documento.documenti[0]).dataArrivo;
                        }
                    }

                    //CASO PROTOCOLLO IN PARTENZA
                    if (documento.tipoProto.Equals("P"))
                    {
                        if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).mittente != null)
                        {
                            result.Sender = Utils.GetCorrespondent(((ProtocolloUscita)documento.protocollo).mittente, infoUtente);
                        }

                        if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatari != null && ((ProtocolloUscita)documento.protocollo).destinatari.Count > 0)
                        {
                            result.Recipients = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatari.Count];
                            int y = 0;
                            foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatari)
                            {
                                result.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                y++;
                            }
                        }

                        if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count > 0)
                        {
                            result.RecipientsCC = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count];
                            int y = 0;
                            foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatariConoscenza)
                            {
                                result.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                y++;
                            }
                        }
                    }

                    //CASO PROTOCOLLO INTERNO
                    if (documento.tipoProto.Equals("I"))
                    {
                        if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).mittente != null)
                        {
                            result.Sender = Utils.GetCorrespondent(((ProtocolloInterno)documento.protocollo).mittente, infoUtente);
                        }

                        if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatari != null && ((ProtocolloInterno)documento.protocollo).destinatari.Count > 0)
                        {
                            result.Recipients = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatari.Count];
                            int y = 0;
                            foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatari)
                            {
                                result.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                y++;
                            }
                        }

                        if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count > 0)
                        {
                            result.RecipientsCC = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count];
                            int y = 0;
                            foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatariConoscenza)
                            {
                                result.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                y++;
                            }
                        }
                    }
                }
            }
            else
            {
                documento = null;
            }

            return result;
        }

        public static DocsPaVO.utente.Corrispondente GetCorrespondentFromPis(Domain.Correspondent corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();

            DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();

            if (corr != null)
            {
                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("U"))
                {
                    ///
                    result = new DocsPaVO.utente.UnitaOrganizzativa();
                    result.tipoCorrispondente = "U";
                    result.info = new DocsPaVO.addressbook.DettagliCorrispondente();
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;
                    result.localita = corr.Location;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(corr.CodeRegisterOrRF);
                        if (reg == null)
                        {
                            //Registro non trovato
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }


                    dettagli.Corrispondente.AddCorrispondenteRow(
                        corr.Address,
                        corr.City,
                        corr.Cap,
                        corr.Province,
                        corr.Nation,
                        corr.PhoneNumber,
                        string.Empty,
                        corr.Fax,
                        corr.NationalIdentificationNumber,
                        corr.Note,
                        corr.Location,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        corr.VatNumber
                        //, string.Empty
                        );

                    result.email = corr.Email;
                    result.info = dettagli;
                    result.dettagli = true;
                    if (corr.PreferredChannel != null)
                    {

                        ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                        foreach (MezzoSpedizione m_spediz in mezziSpedizione)
                        {
                            if (m_spediz.Descrizione.ToUpper().Equals(corr.PreferredChannel.ToUpper()))
                            {
                                result.canalePref = new DocsPaVO.utente.Canale();
                                result.canalePref.descrizione = m_spediz.Descrizione;
                                result.canalePref.systemId = m_spediz.IDSystem;
                                result.canalePref.tipoCanale = m_spediz.chaTipoCanale;
                                break;
                            }
                        }

                    }
                }

                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("P"))
                {
                    result = new DocsPaVO.utente.Utente();
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.cognome = corr.Surname;
                    result.nome = corr.Name;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(corr.CodeRegisterOrRF);
                        if (reg == null)
                        {
                            //Registro non trovato
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }

                    result.tipoCorrispondente = "P";
                    result.email = corr.Email;

                    dettagli.Corrispondente.AddCorrispondenteRow(
                        corr.Address,
                        corr.City,
                        corr.Cap,
                        corr.Province,
                        corr.Nation,
                        corr.PhoneNumber,
                        string.Empty,
                        corr.Fax,
                        corr.NationalIdentificationNumber,
                        corr.Note,
                        corr.Location,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        corr.VatNumber
                        //,string.Empty
                        );

                    result.info = dettagli;
                    result.dettagli = true;
                }

                //Occasionale
                if (corr.Type == null || corr.Type.Equals("O"))
                {
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                    result.cognome = corr.Surname;
                    result.nome = corr.Name;
                    result.codiceAmm = corr.AdmCode;
                    result.codiceAOO = corr.AOOCode;
                    result.descrizione = corr.Description;
                    result.idAmministrazione = infoUtente.idAmministrazione;

                    if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(corr.CodeRegisterOrRF);
                        if (reg == null)
                        {
                            //Registro non trovato
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            result.idRegistro = reg.systemId;
                        }
                    }

                    result.tipoCorrispondente = "O";
                    result.email = corr.Email;
                }
                if (corr != null && !string.IsNullOrEmpty(corr.Type))
                {
                    result.tipoIE = corr.Type;
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static DocsPaVO.filtri.FiltroRicerca[][] GetFiltersDocumentsFromPis(Domain.Filter[] filters, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.filtri.FiltroRicerca[][] qV = new DocsPaVO.filtri.FiltroRicerca[1][];
            qV[0] = new DocsPaVO.filtri.FiltroRicerca[1];
            DocsPaVO.filtri.FiltroRicerca[] fVList = new DocsPaVO.filtri.FiltroRicerca[0];
            DocsPaVO.filtri.FiltroRicerca fV1 = null;
            bool filterFound = false;
            bool findType = false;

            foreach (VtDocsWS.Domain.Filter fil in filters)
            {
                if (fil != null && !string.IsNullOrEmpty(fil.Value))
                {
                    if (fil.Name.ToUpper().Equals("YEAR"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ANNO_PROTOCOLLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (!findType)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "TIPO";
                            fV1.valore = "tipo";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            findType = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("IN_PROTOCOL"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROT_ARRIVO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (!findType)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "TIPO";
                            fV1.valore = "tipo";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            findType = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("OUT_PROTOCOL"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROT_PARTENZA";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (!findType)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "TIPO";
                            fV1.valore = "tipo";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            findType = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("INTERNAL_PROTOCOL"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROT_INTERNO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (!findType)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "TIPO";
                            fV1.valore = "tipo";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            findType = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("NOT_PROTOCOL"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "GRIGIO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        if (!findType)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "TIPO";
                            fV1.valore = "tipo";
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            findType = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("PREDISPOSED"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PREDISPOSTO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("ATTACHMENTS") && !fil.Value.ToUpper().Equals("FALSE"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ALLEGATO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("PRINTS"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "STAMPA_REG";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("NUM_PROTOCOL_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "NUM_PROTOCOLLO_DAL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("NUM_PROTOCOL_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "NUM_PROTOCOLLO_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("TEMPLATE"))
                    {
                        filterFound = true;
                        fV1 = new FiltroRicerca();
                        fV1.argomento = "TIPO_ATTO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROFILAZIONE_DINAMICA";
                        fV1.valore = "Profilazione Dinamica";
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (fil.Template != null)
                        {
                            if (!string.IsNullOrEmpty(fil.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(fil.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(fil.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(fil.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(fil.Value))
                            {
                                int templateNum = 0;

                                if (Int32.TryParse(fil.Value, out templateNum))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(templateNum.ToString());

                                }
                                else
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(fil.Value, infoUtente.idAmministrazione);

                                }
                            }
                        }

                        if (template != null)
                        {
                            if (fil.Template != null)
                            {
                                fV1.template = Utils.GetTemplateFromPis(fil.Template, template, true);
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                        else
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_FROM") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DATA_CREAZIONE_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DATA_CREAZIONE_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("PROTOCOL_DATE_FROM") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DATA_PROT_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("PROTOCOL_DATE_TO") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DATA_PROT_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("SENDER_RECIPIENT") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new FiltroRicerca();
                        fV1.argomento = "ID_MITT_DEST";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("DOCNUMBER_FROM") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DOCNUMBER_DAL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("DOCNUMBER_TO") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "DOCNUMBER_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("REGISTER") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(fil.Value);
                        if (reg != null)
                        {
                            fV1 = new DocsPaVO.filtri.FiltroRicerca();
                            fV1.argomento = "REGISTRO";
                            fV1.valore = reg.systemId;
                            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        }
                        else
                        {
                            //Registro mancante
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                    }

                    if (fil.Name.ToUpper().Equals("OBJECT") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "OGGETTO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("FULL_TEXT_SEARCH") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "RICERCA_FULL_TEXT";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (fil.Name.ToUpper().Equals("TEMPLATE_EXTRACTION") && !string.IsNullOrEmpty(fil.Value))
                    {
                        filterFound = true;
                        // è un filtro che serve per la visualizzazione, non per la ricerca
                    }

                    if (!filterFound)
                        throw new PisException("FILTER_NOT_FOUND");
                }
            }

            // Impostazione valori di default per i filtri obbligatori
            //
            int count = 0;

            // IN_PROTOCOL
            count = filters.Count(f => f != null && f.Name == "IN_PROTOCOL");
            if (count == 0)
            {
                fV1 = new DocsPaVO.filtri.FiltroRicerca();
                fV1.argomento = "PROT_ARRIVO";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            // OUT_PROTOCOL
            count = filters.Count(f => f != null && f.Name == "OUT_PROTOCOL");
            if (count == 0)
            {
                fV1 = new DocsPaVO.filtri.FiltroRicerca();
                fV1.argomento = "PROT_PARTENZA";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            // INTERNAL_PROTOCOL
            count = filters.Count(f => f != null && f.Name == "INTERNAL_PROTOCOL");
            if (count == 0)
            {
                fV1 = new DocsPaVO.filtri.FiltroRicerca();
                fV1.argomento = "PROT_INTERNO";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            // NOT_PROTOCOL
            count = filters.Count(f => f != null && f.Name == "NOT_PROTOCOL");
            if (count == 0)
            {
                fV1 = new DocsPaVO.filtri.FiltroRicerca();
                fV1.argomento = "GRIGIO";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }


            // PREDISPOSED
            count = filters.Count(f => f != null && f.Name == "PREDISPOSED");
            if (count == 0)
            {
                fV1 = new DocsPaVO.filtri.FiltroRicerca();
                fV1.argomento = "PREDISPOSTO";
                fV1.valore = "false";
                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            }

            //// ATTACHMENTS
            //count = filters.Count(f => f != null && f.Name == "ATTACHMENTS");
            //if (count == 0)
            //{
            //    fV1 = new DocsPaVO.filtri.FiltroRicerca();
            //    fV1.argomento = "ALLEGATO";
            //    fV1.valore = "false";
            //    fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
            //}

            fV1 = new DocsPaVO.filtri.FiltroRicerca();
            fV1.nomeCampo = "CREATION_DATE";
            fV1.argomento = "ORDER_DIRECTION";
            fV1.valore = "DESC";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            qV[0] = fVList;

            return qV;
        }

        public static Domain.Document GetDocument(DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.InfoUtente infoUtente, bool infoOnFile, bool getFile)
        {
            Domain.Document responseDocument = new Domain.Document();


            //Dati in comune per tutti
            responseDocument.Id = documento.systemId;
            responseDocument.Object = documento.oggetto.descrizione;
            if (!string.IsNullOrEmpty(documento.privato) && documento.privato.Equals("1"))
            {
                responseDocument.PrivateDocument = true;
            }
            else
            {
                responseDocument.PrivateDocument = false;
            }
            responseDocument.CreationDate = documento.dataCreazione;
            responseDocument.DocNumber = documento.docNumber;
            responseDocument.DocumentType = documento.tipoProto;

            if (BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled())
            {
                if (documento.ConsolidationState != null && documento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                {
                    string message = string.Empty;
                    // Diabilitazione controlli su documento consolidato
                    if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                    {
                        responseDocument.ConsolidationState = "ConsolidationState1";
                    }
                    else if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                    {
                        responseDocument.ConsolidationState = "ConsolidationState2";
                    }
                }
            }
            else
            {
                responseDocument.ConsolidationState = string.Empty;
            }

            if (documento.noteDocumento != null && documento.noteDocumento.Count > 0)
            {
                responseDocument.Note = new Domain.Note[documento.noteDocumento.Count];
                int y = 0;
                foreach (DocsPaVO.Note.InfoNota tempNot in documento.noteDocumento)
                {
                    Domain.Note nota = new Domain.Note();
                    nota.Description = tempNot.Testo;
                    nota.Id = tempNot.Id;
                    nota.User = new Domain.User();
                    nota.User.Id = tempNot.UtenteCreatore.IdUtente;
                    nota.User.Description = tempNot.UtenteCreatore.DescrizioneUtente;
                    responseDocument.Note[y] = nota;
                    y++;
                }
            }

            //Se il documento è in cestino
            if (!string.IsNullOrEmpty(documento.inCestino) && documento.inCestino.Equals("1"))
            {
                responseDocument.InBasket = true;
            }
            else
            {
                responseDocument.InBasket = false;
            }

            if (documento.template != null)
            {
                responseDocument.Template = Utils.GetDetailsTemplateDoc(documento.template, documento.docNumber);
            }

            //Documento principale
            if (documento.documenti != null && documento.documenti.Count > 0)
            {
                DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                responseDocument.MainDocument = Utils.GetFile(versioneCorrente, getFile, infoUtente, false, false, string.Empty, null, false);
                if (infoOnFile)
                    responseDocument.MainDocument.Description += "§" + versioneCorrente.fileSize;

            }

            //Prendi allegati
            if (documento.allegati != null && documento.allegati.Count > 0)
            {
                Domain.File tempAllPIS = null;
                ArrayList allegatiAlbo = new ArrayList();
                foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                {
                    if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoEsterno(tempAll.versionId) != "1" &&
                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(tempAll.versionId) != "1" &&
                        BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(tempAll.versionId) != "1")
                    {
                        tempAllPIS = Utils.GetFile(tempAll, getFile, infoUtente, false, false, string.Empty, null, false);
                        if (infoOnFile)
                            tempAllPIS.Description += "§" + tempAll.fileSize;
                        allegatiAlbo.Add(tempAllPIS);

                    }
                }
                if (allegatiAlbo.Count > 0)
                    responseDocument.Attachments = (Domain.File[])allegatiAlbo.ToArray(typeof(Domain.File));
                else
                    responseDocument.Attachments = null;

            }

            if (documento.protocollo != null)
            {

                if (documento.protocollo.protocolloAnnullato != null)
                {
                    responseDocument.Annulled = true;
                    //responseDocument.AnnulmentDate = documento.protocollo.protocolloAnnullato.dataAnnullamento;
                }
                else
                {
                    responseDocument.Annulled = false;
                }

                if (!string.IsNullOrEmpty(documento.protocollo.segnatura))
                {
                    responseDocument.Signature = documento.protocollo.segnatura;
                }
                else
                {
                    responseDocument.Predisposed = true;
                }

                if (!string.IsNullOrEmpty(documento.protocollo.dataProtocollazione))
                {
                    responseDocument.ProtocolDate = documento.protocollo.dataProtocollazione;
                }

                if (documento.registro != null)
                {
                    responseDocument.Register = Utils.GetRegister(documento.registro);
                }


                //CASO PROTOCOLLO IN ARRIVO
                if (documento.tipoProto.Equals("A"))
                {
                    if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittente != null)
                    {
                        responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloEntrata)documento.protocollo).mittente, infoUtente);
                    }

                    if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittenti != null && ((ProtocolloEntrata)documento.protocollo).mittenti.Count > 0)
                    {
                        responseDocument.MultipleSenders = new Domain.Correspondent[((ProtocolloEntrata)documento.protocollo).mittenti.Count];
                        int y = 0;
                        foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloEntrata)documento.protocollo).mittenti)
                        {
                            responseDocument.MultipleSenders[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                            y++;
                        }
                    }

                    if (((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente != null)
                    {
                        responseDocument.ProtocolSender = ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente;
                    }

                    if (((ProtocolloEntrata)documento.protocollo) != null && !string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                    {
                        responseDocument.DataProtocolSender = ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente;
                    }

                    if (((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione != null)
                    {
                        responseDocument.MeansOfSending = ((ProtocolloEntrata)documento.protocollo).descMezzoSpedizione;
                    }

                    if (((ProtocolloEntrata)documento.protocollo) != null && string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                    {
                        responseDocument.ProtocolDate = ((ProtocolloEntrata)documento.protocollo).dataProtocollazione;
                    }

                    if (documento.documenti != null && documento.documenti != null && documento.documenti.Count > 0)
                    {
                        responseDocument.ArrivalDate = ((DocsPaVO.documento.Documento)documento.documenti[0]).dataArrivo;
                    }
                }

                //CASO PROTOCOLLO IN PARTENZA
                if (documento.tipoProto.Equals("P"))
                {
                    if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).mittente != null)
                    {
                        responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloUscita)documento.protocollo).mittente, infoUtente);
                    }

                    if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatari != null && ((ProtocolloUscita)documento.protocollo).destinatari.Count > 0)
                    {
                        responseDocument.Recipients = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatari.Count];
                        int y = 0;
                        foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatari)
                        {
                            responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                            y++;
                        }
                    }

                    if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count > 0)
                    {
                        responseDocument.RecipientsCC = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count];
                        int y = 0;
                        foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatariConoscenza)
                        {
                            responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                            y++;
                        }
                    }
                }

                //CASO PROTOCOLLO INTERNO
                if (documento.tipoProto.Equals("I"))
                {
                    if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).mittente != null)
                    {
                        responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloInterno)documento.protocollo).mittente, infoUtente);
                    }

                    if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatari != null && ((ProtocolloInterno)documento.protocollo).destinatari.Count > 0)
                    {
                        responseDocument.Recipients = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatari.Count];
                        int y = 0;
                        foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatari)
                        {
                            responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                            y++;
                        }
                    }

                    if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count > 0)
                    {
                        responseDocument.RecipientsCC = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count];
                        int y = 0;
                        foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatariConoscenza)
                        {
                            responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                            y++;
                        }
                    }
                }
            }
            return responseDocument;
        }

        public static Domain.Project GetProject(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Domain.Project result = new Domain.Project();
            if (fascicolo == null)
            {
                result = null;
            }
            else
            {
                result.ClassificationScheme = new Domain.ClassificationScheme();
                result.ClassificationScheme.Id = fascicolo.idTitolario;
                result.ClosureDate = fascicolo.chiusura;
                result.Code = fascicolo.codice;
                result.CollocationDate = fascicolo.dtaLF;
                result.CreationDate = fascicolo.dataCreazione;
                result.Description = fascicolo.descrizione;
                result.Id = fascicolo.systemID;
                result.IdParent = fascicolo.idClassificazione;
                result.Number = fascicolo.codUltimo;
                //result.OpeningDate = fascicolo.apertura;

                if (fascicolo.noteFascicolo != null && fascicolo.noteFascicolo.Length > 0)
                {
                    result.Note = new Domain.Note[fascicolo.noteFascicolo.Length];
                    int y = 0;
                    foreach (DocsPaVO.Note.InfoNota tempNot in fascicolo.noteFascicolo)
                    {
                        Domain.Note nota = new Domain.Note();
                        nota.Description = tempNot.Testo;
                        nota.Id = tempNot.Id;
                        nota.User = new Domain.User();
                        nota.User.Id = tempNot.UtenteCreatore.IdUtente;
                        nota.User.Description = tempNot.UtenteCreatore.DescrizioneUtente;
                        result.Note[y] = nota;
                        y++;
                    }
                }

                if (!string.IsNullOrEmpty(fascicolo.stato) && fascicolo.stato.Equals("C"))
                {
                    result.Open = false;
                }
                else
                {
                    result.Open = true;
                }

                result.Paper = fascicolo.cartaceo;

                if (!string.IsNullOrEmpty(fascicolo.idUoLF))
                {
                    Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(fascicolo.idUoLF);
                    result.PhysicsCollocation = corr.descrizione;
                }

                if (!string.IsNullOrEmpty(fascicolo.privato) && fascicolo.privato.Equals("1"))
                {
                    result.Private = true;
                }

                result.Type = fascicolo.tipo;

                fascicolo.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(fascicolo.systemID);
                if (fascicolo.template != null)
                {
                    result.Template = GetDetailsTemplatePrj(fascicolo.template, fascicolo);
                }

                if (!string.IsNullOrEmpty(fascicolo.controllato) && fascicolo.controllato.Equals("1"))
                {
                    result.Controlled = true;
                }

            }
            return result;
        }

        public static Domain.Template GetDetailsTemplatePrj(DocsPaVO.ProfilazioneDinamica.Templates template, DocsPaVO.fascicolazione.Fascicolo fasc)
        {
            Domain.Template result = new Domain.Template();

            if (template != null)
            {
                result.Name = template.DESCRIZIONE;
                result.Id = template.SYSTEM_ID.ToString();

                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                if (oggettiCustom != null && oggettiCustom.Length > 0)
                {
                    result.Fields = new Domain.Field[oggettiCustom.Length];
                    int numField = 0;

                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                    {

                        Domain.Field fieldTemp = new Domain.Field();

                        fieldTemp.Id = oggettoCustom.SYSTEM_ID.ToString();
                        fieldTemp.Name = oggettoCustom.DESCRIZIONE;
                        if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                        {
                            fieldTemp.Required = true;
                        }
                        else
                        {
                            fieldTemp.Required = false;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                        {
                            fieldTemp.Type = "MultipleChoise";
                            if (oggettoCustom.VALORI_SELEZIONATI != null && oggettoCustom.VALORI_SELEZIONATI.Count > 0)
                            {
                                fieldTemp.MultipleChoice = new string[oggettoCustom.VALORI_SELEZIONATI.Count];
                                for (int i = 0; i < oggettoCustom.VALORI_SELEZIONATI.Count; i++)
                                {
                                    fieldTemp.MultipleChoice[i] = oggettoCustom.VALORI_SELEZIONATI[i].ToString();
                                }
                            }
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Corrispondente"))
                        {
                            fieldTemp.Type = "Correspondent";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            //if (!string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE))
                            //{
                            //    DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemIDDisabled(oggettoCustom.VALORE_DATABASE);
                            //    if (corr != null)
                            //    {
                            //        fieldTemp.Value = corr.codiceRubrica + " - " + corr.descrizione;
                            //    }
                            //}
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CampoDiTesto"))
                        {
                            fieldTemp.Type = "TextField";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("SelezioneEsclusiva"))
                        {
                            fieldTemp.Type = "ExclusiveSelection";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("MenuATendina"))
                        {
                            fieldTemp.Type = "DropDown";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore"))
                        {
                            fieldTemp.Type = "Counter";
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null)
                                {
                                    fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                }
                            }
                            fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Data"))
                        {
                            fieldTemp.Type = "Date";
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                        }

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                        {
                            fieldTemp.Type = "SubCounter";
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                            }
                            fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                            fieldTemp.Value = oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE;
                        }
                        result.Fields[numField] = fieldTemp;
                        numField++;
                    }
                }

                int idDiagramma = 0;
                DocsPaVO.DiagrammaStato.Stato stato = null;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociatoFasc(template.SYSTEM_ID.ToString());
                if (idDiagramma != 0)
                {
                    diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                    if (diagramma != null)
                    {
                        result.StateDiagram = new Domain.StateDiagram();
                        result.StateDiagram.Description = diagramma.DESCRIZIONE;
                        result.StateDiagram.Id = diagramma.SYSTEM_ID.ToString();
                        stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoFasc(fasc.systemID);
                        if (stato != null)
                        {
                            Domain.StateOfDiagram actualState = new Domain.StateOfDiagram();
                            result.StateDiagram.StateOfDiagram = new Domain.StateOfDiagram[1];
                            result.StateDiagram.StateOfDiagram[0] = actualState;
                            result.StateDiagram.StateOfDiagram[0].Description = stato.DESCRIZIONE;
                            result.StateDiagram.StateOfDiagram[0].DiagramId = diagramma.SYSTEM_ID.ToString();
                            result.StateDiagram.StateOfDiagram[0].FinaleState = stato.STATO_FINALE;
                            result.StateDiagram.StateOfDiagram[0].Id = stato.SYSTEM_ID.ToString();
                            result.StateDiagram.StateOfDiagram[0].InitialState = stato.STATO_INIZIALE;
                        }
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        public static DocsPaVO.filtri.FiltroRicerca[][] GetFiltersProjectsFromPis(Domain.Filter[] filters, DocsPaVO.utente.InfoUtente infoUtente, out DocsPaVO.utente.Registro registro, out DocsPaVO.fascicolazione.Classificazione objClassificazione)
        {
            registro = null;
            DocsPaVO.filtri.FiltroRicerca[][] qV = new DocsPaVO.filtri.FiltroRicerca[1][];
            qV[0] = new DocsPaVO.filtri.FiltroRicerca[1];
            DocsPaVO.filtri.FiltroRicerca[] fVList = new DocsPaVO.filtri.FiltroRicerca[0];
            DocsPaVO.filtri.FiltroRicerca fV1 = null;
            bool classificazione = false;
            string codiceClassificazione = string.Empty;
            string idTitolario = string.Empty;
            objClassificazione = null;
            bool filterFound = false;

            foreach (VtDocsWS.Domain.Filter fil in filters)
            {
                if (fil != null && !string.IsNullOrEmpty(fil.Value))
                {
                    if (fil.Name.ToUpper().Equals("YEAR"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ANNO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CREAZIONE_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CREATION_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CREAZIONE_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLOSING_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CHIUSURA_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLOSING_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CHIUSURA_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("OPENING_DATE_FROM"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "APERTURA_SUCCESSIVA_AL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("OPENING_DATE_TO"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "APERTURA_PRECEDENTE_IL";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("STATE"))
                    {
                        filterFound = true;
                        if (fil.Value.ToUpper().Equals("O"))
                        {
                            fil.Value = "A";
                        }
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "STATO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("TYPE_PROJECT"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TIPO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (fil.Name.ToUpper().Equals("PROJECT_CODE"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CODICE_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }
                    if (fil.Name.ToUpper().Equals("PROJECT_NUMBER"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "NUMERO_FASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("PROJECT_DESCRIPTION"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TITOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("CLASSIFICATION_CODE"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "CODICE_CLASSIFICA";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        classificazione = true;
                        codiceClassificazione = fil.Value;
                    }

                    if (fil.Name.ToUpper().Equals("CLASSIFICATION_SCHEME"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "ID_TITOLARIO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                        idTitolario = fil.Value;
                    }

                    if (fil.Name.ToUpper().Equals("REGISTER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            registro = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(fil.Value);
                        }
                    }

                    if (fil.Name.ToUpper().Equals("SUBPROJECT"))
                    {
                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "SOTTOFASCICOLO";
                        fV1.valore = fil.Value;
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                    }

                    if (fil.Name.ToUpper().Equals("TEMPLATE"))
                    {
                        // viene cercato solo l'id del template con TIPOLOGIA_FASCICOLO
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;

                        int tempIdTemplate = 0;
                        if (!int.TryParse(fil.Value, out tempIdTemplate))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(fil.Value);
                            if (template != null)
                                tempIdTemplate = template.SYSTEM_ID;
                        }

                        filterFound = true;
                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "TIPOLOGIA_FASCICOLO";
                        //fV1.valore = fil.Value;
                        fV1.valore = tempIdTemplate.ToString();
                        fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

                        fV1 = new DocsPaVO.filtri.FiltroRicerca();
                        fV1.argomento = "PROFILAZIONE_DINAMICA";
                        fV1.valore = "Profilazione Dinamica";

                        if (fil.Template != null)
                        {
                            if (!string.IsNullOrEmpty(fil.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(fil.Template.Id);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(fil.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(fil.Template.Name);
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(fil.Value))
                            {
                                int templateNum = 0;

                                if (Int32.TryParse(fil.Value, out templateNum))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(templateNum.ToString());

                                }
                                else
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateByDescrizione(fil.Value);

                                }
                            }
                        }



                        if (template != null)
                        {
                            if (fil.Template != null)
                            {
                                fV1.template = Utils.GetTemplateFromPis(fil.Template, template, true);
                                fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);
                            }
                        }
                        else
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }

                    if (fil.Name.ToUpper().Equals("TEMPLATE_EXTRACTION"))
                    {
                        filterFound = true;
                        // filtro non coinvolto nella ricerca ma nella stampa dei risultati.
                    }

                    if (!filterFound)
                        throw new PisException("FILTER_NOT_FOUND");
                }
            }

            //Caso particolare classificazione devo fare alcuni calcoli per creare l'oggetto classificazione
            if (classificazione)
            {
                if (registro != null && !string.IsNullOrEmpty(codiceClassificazione) && !string.IsNullOrEmpty(idTitolario))
                {
                    ArrayList listaClassifiche = BusinessLogic.Fascicoli.TitolarioManager.getTitolario2(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, codiceClassificazione, false, idTitolario);
                    if (listaClassifiche != null && listaClassifiche.Count > 0)
                    {
                        objClassificazione = (DocsPaVO.fascicolazione.Classificazione)listaClassifiche[0];
                    }
                }
                else
                {
                    //Parametri mancanti per la classificazione
                    throw new PisException("MISSING_PATAMETERS_CLASSIFICATION");
                }
            }

            fV1 = new DocsPaVO.filtri.FiltroRicerca();
            fV1.nomeCampo = "CREATION_DATE";
            fV1.argomento = "ORDER_DIRECTION";
            fV1.valore = "DESC";
            fVList = Utils.addToArrayFiltroRicerca(fVList, fV1);

            qV[0] = fVList;

            return qV;
        }

        public static Domain.Project GetProjectFromSearchObject(DocsPaVO.Grids.SearchObject obj, bool estrazioneTemplates, DocsPaVO.Grid.Field[] visibilityFields)
        {
            Domain.Project result = new Domain.Project();
            string value = string.Empty;
            if (obj != null)
            {
                result.Id = obj.SearchObjectID;
                result.Number = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P14")).FirstOrDefault().SearchObjectFieldValue;
                result.ClassificationScheme = new Domain.ClassificationScheme();
                result.ClassificationScheme.Description = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P10")).FirstOrDefault().SearchObjectFieldValue;
                result.Code = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P3")).FirstOrDefault().SearchObjectFieldValue;
                result.Description = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P4")).FirstOrDefault().SearchObjectFieldValue;
                result.CreationDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P5")).FirstOrDefault().SearchObjectFieldValue;
                result.ClosureDate = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P6")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue) && obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue.Equals("1"))
                {
                    result.Private = true;
                }
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P11")).FirstOrDefault().SearchObjectFieldValue) && obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P9")).FirstOrDefault().SearchObjectFieldValue.Equals("1"))
                {
                    result.Paper = true;
                }
                result.Type = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P1")).FirstOrDefault().SearchObjectFieldValue;
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue))
                {
                    if (obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue.Equals("A"))
                    {
                        result.Open = true;
                    }
                }
                // errore nell'estrazione del template. P16 non riguarda il template ma lo stato. Modifico l'if
                //if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("P16")).FirstOrDefault().SearchObjectFieldValue))
                if (!string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_FASC")).FirstOrDefault().SearchObjectFieldValue) &&
                    !string.IsNullOrEmpty(obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue))
                {
                    result.Template = new Domain.Template();
                    result.Template.Id = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("ID_TIPO_FASC")).FirstOrDefault().SearchObjectFieldValue;
                    result.Template.Name = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals("U1")).FirstOrDefault().SearchObjectFieldValue;
                    if (estrazioneTemplates)
                    {
                        List<Domain.Field> campiPIS = new List<Domain.Field>();
                        Domain.Field campoPIS = null;
                        foreach (DocsPaVO.Grid.Field campoProf in visibilityFields)
                        {
                            campoPIS = new Domain.Field();
                            campoPIS.Id = campoProf.CustomObjectId.ToString();
                            campoPIS.Name = campoProf.Label;
                            campoPIS.Value = obj.SearchObjectField.Where(e => e.SearchObjectFieldID.Equals(campoProf.FieldId)).FirstOrDefault().SearchObjectFieldValue;
                            campiPIS.Add(campoPIS);
                        }
                        if (campiPIS.Count > 0)
                        {
                            result.Template.Fields = campiPIS.ToArray();
                        }
                    }
                }

            }
            else
            {
                result = null;
            }

            return result;
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateFromPis(Domain.Template templatePis, DocsPaVO.ProfilazioneDinamica.Templates template, bool search)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = new DocsPaVO.ProfilazioneDinamica.Templates();

            if (templatePis == null || template == null)
            {
                result = null;
            }
            else
            {
                result.DESCRIZIONE = templatePis.Name;
                result.SYSTEM_ID = Int32.Parse(templatePis.Id);

                DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                       template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));


                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                {
                    Domain.Field campo = templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                    // Impostazione del valore
                    if (campo != null && (string.IsNullOrEmpty(campo.Value) || ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione") && (campo.MultipleChoice == null || campo.MultipleChoice.Length == 0))))
                    {

                        if (campo.Required && !search)
                        {
                            //Campo obbligatorio
                            throw new PisException("FIELD_REQUIRED");
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }

                        //Nel caso di contatorei che non sono di registor o rf posso far scattare il contatore senza valore
                        if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")) && (!oggettoCustom.TIPO_CONTATORE.Equals("A") && !oggettoCustom.TIPO_CONTATORE.Equals("R")))
                        {
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }
                    }
                    else
                    {
                        if (campo != null)
                        {
                            oggettoCustom.VALORE_DATABASE = campo.Value;
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                        //Passo come valore il codice del Registro rf del contatore

                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                        {
                            //Aggiunta per contatore
                            if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                            {
                                if (campo != null && !string.IsNullOrEmpty(campo.CodeRegisterOrRF))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(campo.CodeRegisterOrRF);

                                    if (reg != null)
                                    {
                                        oggettoCustom.ID_AOO_RF = reg.systemId;
                                    }
                                    else
                                    {
                                        //Registro|RF mancante
                                        throw new PisException("REGISTER_NOT_FOUND");
                                    }
                                }
                                else
                                {
                                    if (!search)
                                        //Id registro o rf mancante
                                        throw new PisException("REQUIRED_ID_REGISTER");
                                }

                            }

                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                        }


                        if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                        {
                            for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                            {
                                if (campo != null)
                                {
                                    foreach (string word in campo.MultipleChoice)
                                    {
                                        if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                        {
                                            oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result = template;
            }

            return template;
        }

        /// <summary>
        /// Modifica Lembo 11-01-2013
        /// Nuovo metodo per prelevare il template dal database che tiene in conto della visibilità dei campi per un ruolo.
        /// </summary>
        /// <param name="templatePis">Il template inserito tramite WS</param>
        /// <param name="template">Il template prelevato dal DB</param>
        /// <param name="search"></param>
        /// <param name="idRuolo">L'id del ruolo dell'utente che si autentica tramite WS</param>
        /// <param name="DoP">D se i template da considerare sono di tipo Documento, P se di tipo fascicolo </param>
        /// <param name="codeApplication">Alcune applicazioni possono ignorare la visibilità dei campi</param>
        /// <param name="docPrincipale">Per l'elaborazione XML</param>
        /// <returns></returns>
        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplateFromPisVisibility(Domain.Template templatePis, DocsPaVO.ProfilazioneDinamica.Templates template, bool search, string idRuolo, string DoP, string codeApplication, DocsPaVO.utente.InfoUtente infoUtente = null, Domain.File docPrincipale = null, string codeRegister = null, string codeRF = null, bool editDocument = false, string idSdi = null)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = new DocsPaVO.ProfilazioneDinamica.Templates();
            byte[] fDocContent = null;
            // Modifica Elaborazione XML da PIS req.2
            if (string.IsNullOrEmpty(template.PATH_XSD_ASSOCIATO) || editDocument)
            {
                if (templatePis == null || template == null)
                {
                    result = null;
                }
                else
                {
                    if (template != null)
                    {
                        result.DESCRIZIONE = template.DESCRIZIONE;
                        result.SYSTEM_ID = template.SYSTEM_ID;
                    }
                    else
                    {
                        result.DESCRIZIONE = templatePis.Name;
                        result.SYSTEM_ID = Int32.Parse(templatePis.Id);
                    }
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                           template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));
                    DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti = null;
                    if (DoP == "D") //per template documenti
                        dirittiOggetti = (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(idRuolo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));
                    else if (DoP == "P") // per template fascicoli
                        dirittiOggetti = (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(idRuolo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));

                    foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                    {
                        Domain.Field campo = null;
                        // Alcune applicazioni ignorano i diritti sui campi.
                        if (!ApplicationIgnoresRights(codeApplication) && !(oggettoCustom.TIPO.DESCRIZIONE_TIPO).ToUpper().Equals("SEPARATORE"))
                        {
                            //Controlla che l'utente abbia diritto di modifica sull'oggetto custom
                            if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()) != null && dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM != "0")
                                campo = templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());
                            else
                            {
                                // Genera eccezione se un utente cerca di inserire o modificare un campo su cui il suo ruolo non ha diritto
                                if (templatePis.Fields != null && templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()) != null && !string.IsNullOrEmpty(templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()).Value))
                                    throw new PisException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                            }
                        }
                        else
                        {
                            campo = templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant());

                        }
                        // Impostazione del valore
                        if (campo != null && string.IsNullOrEmpty(campo.Value) || (campo != null && (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione") && (campo.MultipleChoice == null || campo.MultipleChoice.Length == 0)))
                        {

                            if (campo.Required && !search)
                            {
                                //Campo obbligatorio
                                throw new PisException("FIELD_REQUIRED");
                            }
                            else
                            {
                                //oggettoCustom.VALORE_DATABASE = string.Empty;
                            }

                            //Nel caso di contatorei che non sono di registor o rf posso far scattare il contatore senza valore
                            if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")) && (!oggettoCustom.TIPO_CONTATORE.Equals("A") && !oggettoCustom.TIPO_CONTATORE.Equals("R")))
                            {
                                oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                            }
                        }
                        else
                        {
                            if (campo != null)
                            {
                                oggettoCustom.VALORE_DATABASE = campo.Value;
                            }
                            else
                            {
                                //oggettoCustom.VALORE_DATABASE = string.Empty;

                            }
                            //Passo come valore il codice del Registro rf del contatore

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                            {
                                //Aggiunta per contatore
                                //Modifica Lembo 11-01-2013: I contatori di tipo A o R scattano solo se popolati nel CodeRegister e nel Value
                                if (campo != null) //inutile cercare il registro di RF se non è presente il campo.
                                {
                                    if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                    {
                                        if (campo != null && !string.IsNullOrEmpty(campo.CodeRegisterOrRF))
                                        {
                                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(campo.CodeRegisterOrRF);

                                            if (reg != null)
                                            {
                                                oggettoCustom.ID_AOO_RF = reg.systemId;
                                            }
                                            else
                                            {
                                                //Registro|RF mancante
                                                throw new PisException("REGISTER_NOT_FOUND");
                                            }
                                        }
                                        else
                                        {
                                            if (!search)
                                                //Id registro o rf mancante
                                                throw new PisException("REQUIRED_ID_REGISTER");
                                        }

                                    }
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                                else
                                {
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                                }

                            }


                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                            {
                                for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                {
                                    if (campo != null)
                                    {
                                        foreach (string word in campo.MultipleChoice)
                                        {
                                            if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                                            {
                                                oggettoCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    result = template;
                }
            }
            else
            {

                if (template.PATH_XSD_ASSOCIATO.ToUpper() == "ASSOC_NO_VALIDAZIONE")
                {
                    // Template da acquisire da XML senza effettuare la validazione
                    if (docPrincipale == null)
                    {
                        throw new PisException("ELAB_XML_FILE_NOT_FOUND");
                    }
                    else if (!docPrincipale.Name.ToUpper().EndsWith("XML") && !docPrincipale.Name.ToUpper().EndsWith("P7M"))
                    {
                        throw new PisException("ELAB_XML_FILE_NOT_VALID");
                    }

                    string stringaXml = null;
                    if (docPrincipale.Name.ToUpper().EndsWith("P7M"))
                    {
                        //sbustare
                        FileDocumento fdoc = new FileDocumento();
                        fdoc.fullName = docPrincipale.Name;
                        fdoc.name = docPrincipale.Name;
                        fdoc.contentType = docPrincipale.MimeType;
                        fdoc.content = docPrincipale.Content;
                        fdoc.nomeOriginale = docPrincipale.Name;
                        fdoc.length = docPrincipale.Content.Length;

                        try
                        {
                            bool ctrlFirmaX = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, null);
                            // Problema con il controllo della firma: 
                            // Sembra sia cambiato qualcosa nel DocsPaVO.documento.SignerInfo.CertificateInfo.RevocationStatus
                            //if (!ctrlFirmaX) throw new Exception("Errore nel controllo firma");
                        }
                        catch (Exception exXml)
                        {
                            // Vedo se c'è un errata codifica Base64
                            logger.Error("ERRORE nel file P7M: provo ad eliminare una codifica base64");
                            fdoc.content = Convert.FromBase64String(Encoding.UTF8.GetString(docPrincipale.Content));
                            try
                            {
                                bool ctrlFirmaX2 = BusinessLogic.Documenti.FileManager.VerifyFileSignature(fdoc, null);
                                if (!ctrlFirmaX2) throw new Exception("Errore nel controllo firma");
                            }
                            catch (Exception exXml2)
                            {

                                throw new Exception("Errore nel file XML.P7M, elaborazione non possibile. Controllare la codifica del file.");
                            }
                        }
                        if (!fdoc.name.ToUpper().EndsWith("XML"))
                        {
                            throw new PisException("ELAB_XML_FILE_NOT_VALID");
                        }
                        else
                        {
                            stringaXml = Encoding.UTF8.GetString(fdoc.content);
                        }
                        //docPrincipale.Content = fdoc.content;
                        //docPrincipale.Name = fdoc.name;
                        fDocContent = fdoc.content;

                    }
                    result.DESCRIZIONE = template.DESCRIZIONE;
                    result.SYSTEM_ID = template.SYSTEM_ID;
                    if (string.IsNullOrEmpty(stringaXml))
                        stringaXml = Encoding.UTF8.GetString(docPrincipale.Content);

                    stringaXml = RemoveUtf8ByteOrderMark(stringaXml);
                    try
                    {
                        AssociazioneXMLperTemplate(ref template, templatePis, stringaXml, DoP, codeApplication, idRuolo, search, infoUtente, codeRegister, codeRF, idSdi);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Eccezione in GetTemplateFromPisVisibility: " + ex.Message);
                        //potrebbe capitare che la codifica dell'xml è utf-16
                        stringaXml = System.Text.Encoding.Unicode.GetString(fDocContent);
                        stringaXml = stringaXml.Trim();
                        AssociazioneXMLperTemplate(ref template, templatePis, stringaXml, DoP, codeApplication, idRuolo, search, infoUtente, codeRegister, codeRF, idSdi);
                    }
                    result = template;
                }


                else
                {
                    // Template da acquisire da XML con validazione
                    if (docPrincipale == null)
                    {
                        throw new PisException("ELAB_XML_FILE_NOT_FOUND");
                    }
                    else if (!docPrincipale.Name.ToUpper().EndsWith("XML"))
                    {
                        throw new PisException("ELAB_XML_FILE_NOT_VALID");
                    }
                    result.DESCRIZIONE = template.DESCRIZIONE;
                    result.SYSTEM_ID = template.SYSTEM_ID;
                    string stringaXml = Encoding.UTF8.GetString(docPrincipale.Content);
                    stringaXml = RemoveUtf8ByteOrderMark(stringaXml);
                    bool validato = ValidateXmlWithXsd(stringaXml, template.PATH_XSD_ASSOCIATO);
                    AssociazioneXMLperTemplate(ref template, templatePis, stringaXml, DoP, codeApplication, idRuolo, search, infoUtente, codeRegister, codeRF, idSdi);
                    result = template;
                }
            }
            return template;
        }
        /// <summary>
        /// Ad alcune applicazioni è concesso ignorare i diritti sui campi dei template.
        /// </summary>
        /// <param name="codeApplication">il codice applicazione</param>
        /// <returns>vero se l'applicazione può ignorare i diritti.</returns>
        public static bool ApplicationIgnoresRights(string codeApplication)
        {
            bool retval = false;
            if (!String.IsNullOrEmpty(codeApplication) && codeApplication == "ALBO_TELEMATICO")
                retval = true;
            return retval;
        }

        // Modifica Elaborazione XML da PIS req.2

        private static void AssociazioneXMLperTemplate(ref DocsPaVO.ProfilazioneDinamica.Templates template, Domain.Template templatePis, string stringaXml, string DoP, string codeApplication, string idRuolo, bool search, DocsPaVO.utente.InfoUtente infoUtente, string codeRegister, string codeRF, string idSdI)
        {
            logger.Debug("START");
            XmlDocument xmlDoc = new XmlDocument();
            if (stringaXml.Contains("xml version=\"1.1\""))
            {
                logger.Debug("Versione XML 1.1. Provo conversione");
                stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
            }

            try
            {
                xmlDoc.LoadXml(stringaXml);
            }
            catch (Exception exXml)
            {
                try
                {
                    string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    if (stringaXml.StartsWith(byteOrderMarkUtf8))
                    {
                        stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                    }
                    xmlDoc.LoadXml(stringaXml);
                }
                catch (Exception bomUTF8)
                {
                    throw new Exception("Errore nel file XML, elaborazione non possibile. Controllare la codifica del file.");
                }

            }
            DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                                   template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));
            DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti = null;
            string notePerElaborazioneXML = "";
            if (DoP == "D") //per template documenti
                dirittiOggetti = (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(idRuolo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));
            // L'elaborazione xml è prevista solo per documenti.
            else if (DoP == "P") // per template fascicoli
                dirittiOggetti = (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getDirittiCampiTipologiaFasc(idRuolo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
            {

                bool campoDaIns = false;
                // Alcune applicazioni ignorano i diritti sui campi.
                if (!ApplicationIgnoresRights(codeApplication) && !(oggettoCustom.TIPO.DESCRIZIONE_TIPO).ToUpper().Equals("SEPARATORE"))
                {
                    //Controlla che l'utente abbia diritto di modifica sull'oggetto custom
                    if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM != "0")
                        campoDaIns = true;
                    else
                    {
                        // Genera eccezione se un utente cerca di inserire o modificare un campo su cui il suo ruolo non ha diritto
                        if (templatePis.Fields != null && templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()) != null && !string.IsNullOrEmpty(templatePis.Fields.FirstOrDefault(e => e.Name.ToUpperInvariant() == oggettoCustom.DESCRIZIONE.ToUpperInvariant()).Value))
                            throw new PisException("TEMPLATE_FIELD_NOT_ROLE_EDITABLE");
                    }
                }
                else
                {
                    campoDaIns = true;
                }
                // Impostazione del valore
                // Inserire qui il mapping XML dei singoli campi.
                //Nel caso di contatorei che non sono di registor o rf posso far scattare il contatore senza valore

                // Il contatore scatta se l'utente ha visibilità sul campo.
                if (((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore") || (oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore")))
                {
                    //if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).VIS_OGG_CUSTOM != "0")
                    if (campoDaIns)
                    {
                        if (oggettoCustom.TIPO_CONTATORE.Equals("A"))
                        {
                            if (!string.IsNullOrEmpty(codeRegister))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(codeRegister);

                                if (reg != null)
                                {
                                    oggettoCustom.ID_AOO_RF = reg.systemId;
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                                else
                                {
                                    //Registro|RF mancante
                                    throw new PisException("REGISTER_NOT_FOUND");
                                }
                            }
                            else
                            {
                                throw new PisException("REQUIRED_CODEREGISTER");
                            }
                        }
                        else if (oggettoCustom.TIPO_CONTATORE.Equals("R"))
                        {
                            if (!string.IsNullOrEmpty(codeRF))
                            {
                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(codeRF);

                                if (reg != null)
                                {
                                    oggettoCustom.ID_AOO_RF = reg.systemId;
                                    oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                                }
                                else
                                {
                                    //Registro|RF mancante
                                    throw new PisException("REGISTER_NOT_FOUND");
                                }
                            }
                            else
                            {
                                throw new PisException("REQUIRED_CODERF");
                            }
                        }
                        else
                            oggettoCustom.CONTATORE_DA_FAR_SCATTARE = true;
                    }
                    else
                    {
                        oggettoCustom.CONTATORE_DA_FAR_SCATTARE = false;
                    }
                }



                if (campoDaIns)
                {

                    if (string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                    {

                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && !search)
                        {
                            //Campo obbligatorio
                            //throw new PisException("FIELD_REQUIRED");
                            throw new Exception("Elaborazione XML: manca il mapping per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                        }
                        else
                        {
                            oggettoCustom.VALORE_DATABASE = string.Empty;
                        }
                    }
                    else
                    {
                        // metodo estrazione Valore xml campo.
                        try
                        {
                            bool associaSecondo = false;
                            string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                            string[] mappingXml = mappings[0].Split('>');
                            string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                            for (int i = 1; i < mappingXml.Length; i++)
                            {
                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                            }
                            string valore = "";
                            try
                            {
                                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                valore = node.InnerXml; // valore dell'xml estratto
                            }
                            catch (Exception nodo)
                            {
                                if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                    associaSecondo = true;
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                {
                                    valore = "";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                {
                                    valore = "";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                {
                                    valore = "999";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                {
                                    valore = "Verifica firma digitale effettuata da SDI, secondo le Specifiche tecniche operative delle Regole tecniche di cui all’allegato B del D.M. n. 55 del 3 aprile 2013 e ss.mm.ii.";
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                {
                                    valore = idSdI;
                                }
                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                {
                                    XmlElement root = xmlDoc.DocumentElement;
                                    valore = root.Attributes["versione"].Value;
                                }
                                else
                                {
                                    notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                    if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                    {
                                        throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                            {
                                if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                    valore = "";
                            }
                            //if (string.IsNullOrEmpty(valore) && mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                            //    associaSecondo = true;
                            //if (associaSecondo)
                            //{
                            //    mappingXml = mappings[1].Split('>');
                            //    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                            //    for (int i = 1; i < mappingXml.Length; i++)
                            //    {
                            //        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                            //    }
                            //    valore = "";
                            //    try
                            //    {
                            //        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                            //        valore = node.InnerXml; // valore dell'xml estratto
                            //    }
                            //    catch (Exception nodo)
                            //    {                                    
                            //            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                            //            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                            //            {
                            //                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                            //            }                                    
                            //    }
                            //}
                            if (associaSecondo)
                            {
                                int associaSecondoI = 1;
                                while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                {
                                    mappingXml = mappings[associaSecondoI].Split('>');
                                    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                    for (int i = 1; i < mappingXml.Length; i++)
                                    {
                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                    }
                                    valore = "";
                                    try
                                    {
                                        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                        valore = node.InnerXml; // valore dell'xml estratto
                                    }
                                    catch (Exception nodo)
                                    {

                                    }
                                    associaSecondoI++;
                                }
                                if (string.IsNullOrEmpty(valore))
                                {
                                    notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                    if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                    {
                                        throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                    }

                                }
                            }

                            // Estrazione dei campi CDATA
                            if (valore.Contains("<![CDATA["))
                            {
                                valore = valore.Replace("<![CDATA[", "");
                                valore = valore.Replace("]]>", "");
                            }

                            oggettoCustom.VALORE_DATABASE = valore;

                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                            {
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("MULTINODE"))
                                {
                                    string separatore = oggettoCustom.OPZIONI_XML_ASSOC.Split('>')[1];

                                    XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                    if (multinode.Count > 1)
                                    {
                                        valore = "";
                                        foreach (XmlNode nodoX in multinode)
                                        {
                                            if (!valore.Contains(nodoX.InnerXml))
                                            {
                                                valore += nodoX.InnerXml + separatore;
                                            }
                                        }
                                    }
                                    if (valore.Length > 180) valore = valore.Substring(0, 180);

                                }
                                #region Fornitore fattura elettronica
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                {
                                    if (string.IsNullOrEmpty(valore))
                                    {
                                        string mappingNome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Nome";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        valore = "";
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }

                                        string mappingCognome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Cognome";
                                        mappingXml = mappingCognome.Split('>');
                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore += (" " + node.InnerXml); // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Cliente Fattura elettronica attiva
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CLIENTE_1"))
                                {
                                    if (string.IsNullOrEmpty(valore))
                                    {
                                        string mappingNome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Nome";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        valore = "";
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }

                                        string mappingCognome = "CessionarioCommittente>DatiAnagrafici>Anagrafica>Cognome";
                                        mappingXml = mappingCognome.Split('>');
                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore += (" " + node.InnerXml); // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            {
                                                throw new Exception("Elaborazione XML: Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE);
                                            }
                                        }
                                    }
                                }
                                #endregion
                                #region Partita IVA CessionarioCommittente
                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_PIVA_CLIENTE_1"))
                                {
                                    // CessionarioCommittente>DatiAnagrafici>CodiceFiscale
                                    if( string.IsNullOrEmpty(valore) || valore =="999")
                                    {
                                        string mappingNome = "CessionarioCommittente>DatiAnagrafici>CodiceFiscale";
                                        mappingXml = mappingNome.Split('>');
                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                        for (int i = 1; i < mappingXml.Length; i++)
                                        {
                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                        }
                                        try
                                        {
                                            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                            valore = node.InnerXml; // valore dell'xml estratto
                                        }
                                        catch (Exception nodo)
                                        {
                                            notePerElaborazioneXML += "Nodo XML mancante per il campo " + oggettoCustom.DESCRIZIONE + ". ";
                                            
                                        }
                                    }
                                }
                                #endregion
                                oggettoCustom.VALORE_DATABASE = valore;
                            }
                            if (!string.IsNullOrEmpty(valore))
                            {
                                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                                {
                                    string[] valoriAssociati1 = oggettoCustom.OPZIONI_XML_ASSOC.Split('>');
                                    bool trovato = false;
                                    for (int i = 0; i < valoriAssociati1.Length; i++)
                                    {
                                        string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                                        if (valoriAssociati2[1] == valore)
                                        {
                                            oggettoCustom.VALORE_DATABASE = valoriAssociati2[0];
                                            trovato = true;
                                        }
                                    }
                                    if (!trovato)
                                    {
                                        notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Valore " + valore + " non valido. ";
                                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                        {
                                            throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non ha un campo associato configurato");
                                        }
                                    }

                                }
                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                                {
                                    string conversione = oggettoCustom.OPZIONI_XML_ASSOC;
                                    try
                                    {

                                        //DateTime dtp = DateTime.ParseExact(valore, conversione, System.Globalization.CultureInfo.InvariantCulture);
                                        DateTime dtp = DateTime.ParseExact(valore.Substring(0, conversione.Length), conversione, System.Globalization.CultureInfo.InvariantCulture);

                                        oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy");
                                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                                        {
                                            oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy " + oggettoCustom.FORMATO_ORA);
                                        }
                                    }
                                    catch (Exception exData)
                                    {
                                        oggettoCustom.VALORE_DATABASE = "";
                                        notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione + ". ";
                                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                        {
                                            throw new Exception("Elaborazione XML: Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore " + valore + " non è compatibile con la stringa di conversione " + conversione);
                                        }
                                    }

                                }
                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                {
                                    DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                    //filtriRic.doUo = true;
                                    //filtriRic.doRuoli = true;
                                    filtriRic.doRubricaComune = true;
                                    //filtriRic.doUtenti = true;
                                    //filtriRic.doRF = true;
                                    filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                    filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                    filtriRic.caller.IdRuolo = infoUtente.idGruppo;
                                    filtriRic.caller.IdUtente = infoUtente.idPeople;
                                    filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                    string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                                    //if (tipoRicerca == "CODE")
                                    //    filtriRic.codice = valore;
                                    switch (tipoRicerca)
                                    {
                                        case "CODE":
                                            filtriRic.codice = valore;
                                            break;
                                        case "DESCRIZIONE":
                                            filtriRic.descrizione = valore;
                                            break;
                                        case "PIVA":
                                            filtriRic.partitaIva = valore;
                                            break;
                                        case "CF":
                                            filtriRic.codiceFiscale = valore;
                                            break;
                                        case "MAIL":
                                            filtriRic.email = valore;
                                            break;
                                    }
                                    BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);

                                    // DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                                    DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                                    ArrayList objElementiRubrica = corrSearcher.Search(filtriRic, smistamentoRubrica);
                                    if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                                    {
                                        string sysId = "";

                                        if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                        {
                                            sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                        }
                                        else
                                        {
                                            DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice);

                                            if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                                            {
                                                bool rubricaComuneAbilitata = BusinessLogic.RubricaComune.Configurazioni.GetConfigurazioni(infoUtente).GestioneAbilitata;
                                                if (rubricaComuneAbilitata)
                                                {
                                                    corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubricaRubricaComune(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice, infoUtente);
                                                }

                                                if (corr != null)
                                                    sysId = corr.systemId;
                                            }
                                        }
                                        oggettoCustom.VALORE_DATABASE = sysId;
                                    }
                                    else
                                    {
                                        oggettoCustom.VALORE_DATABASE = string.Empty;
                                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                                            throw new Exception("Elaborazione XML: Errore nell'associazione del campo " + oggettoCustom.DESCRIZIONE + ", Corrispondente non trovato con i dati presenti nello XML.");
                                        else
                                            notePerElaborazioneXML += "Errore di associazione del campo " + oggettoCustom.DESCRIZIONE + ". Il valore è presente nel file XML ma non è stato possibile associarlo automaticamente ad un corrispondente.  L’eventuale integrazione avviene manualmente. ";
                                    }
                                }
                                else if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                {

                                }
                            }
                            else
                            {
                                notePerElaborazioneXML += "Valore per il campo " + oggettoCustom.DESCRIZIONE + " non presente nel file XML. ";
                            }

                        }
                        catch (Exception e)
                        {
                            //if (e.Message.Equals("Corrispondente non trovato con i dati presenti nello XML."))
                            //{
                            //    throw new Exception("Corrispondente non trovato con i dati presenti nello XML.");
                            //}
                            if (e.Message.Contains("Elaborazione XML:"))
                            {
                                throw new Exception(e.Message);
                            }
                            else
                            {
                                oggettoCustom.VALORE_DATABASE = string.Empty;
                                oggettoCustom.VALORI_SELEZIONATI = null;
                            }
                        }

                        if (oggettoCustom.CAMPO_OBBLIGATORIO == "SI" && (string.IsNullOrEmpty(oggettoCustom.VALORE_DATABASE) || ((oggettoCustom.VALORI_SELEZIONATI == null || oggettoCustom.VALORI_SELEZIONATI.Count < 1) && oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")))
                        {
                            //throw new PisException("FIELD_REQUIRED");
                            throw new Exception("Elaborazione XML: Valore assente per il campo obbligatorio " + oggettoCustom.DESCRIZIONE);
                        }
                        // QUA DEVE FINIRE

                    }

                }
            }

            // scrittura note elaborazione xml

            //var noteElab2 = from DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCust in oggettiCustom 
            //           where oggCust.DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML"
            //           select oggCust;
            //DocsPaVO.ProfilazioneDinamica.OggettoCustom noteElab = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)noteElab2;

            for (int i = 0; i < oggettiCustom.Length; i++)
            {
                if (oggettiCustom[i].DESCRIZIONE.ToUpper() == "NOTE RELATIVE ALL'ELABORAZIONE XML")
                {
                    if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                    {
                        if (notePerElaborazioneXML.Length > 220)
                        {
                            notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                        }
                        oggettiCustom[i].VALORE_DATABASE = notePerElaborazioneXML;
                    }
                    else
                        oggettiCustom[i].VALORE_DATABASE = "Elaborazione avvenuta con successo";
                }
            }
        }

        private static void GetFieldValueFromXML(ref DocsPaVO.ProfilazioneDinamica.OggettoCustom oggCustom, XmlDocument xmlDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                string[] mappings = oggCustom.CAMPO_XML_ASSOC.Split('§');
                string[] mappingXml = mappings[0].Split('>');
                string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                for (int i = 1; i < mappingXml.Length; i++)
                {
                    mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                }
                XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                string valore = node.InnerXml; // valore dell'xml estratto
                oggCustom.VALORE_DATABASE = valore;
                //superfluo
                //if (oggCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                //{
                //    oggCustom.VALORE_DATABASE = valore;
                //}

                if (oggCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                {
                    string[] valoriAssociati1 = oggCustom.OPZIONI_XML_ASSOC.Split('>');
                    for (int i = 0; i < valoriAssociati1.Length; i++)
                    {
                        string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                        if (valoriAssociati2[1] == valore)
                        {
                            oggCustom.VALORE_DATABASE = valoriAssociati2[0];
                        }
                    }

                }
                else if (oggCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                {

                    string conversione = oggCustom.OPZIONI_XML_ASSOC;
                    DateTime dtp = DateTime.ParseExact(valore, conversione, System.Globalization.CultureInfo.InvariantCulture);
                    oggCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy HH:mm:ss");

                }
                else if (oggCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                {
                    DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                    filtriRic.doUo = true;
                    filtriRic.doRuoli = true;
                    filtriRic.doRubricaComune = true;
                    filtriRic.doUtenti = true;
                    filtriRic.doRF = true;
                    filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                    string tipoRicerca = oggCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                    //if (tipoRicerca == "CODE")
                    //    filtriRic.codice = valore;
                    switch (tipoRicerca)
                    {
                        case "CODE":
                            filtriRic.codice = valore;
                            break;
                        case "DESCRIZIONE":
                            filtriRic.descrizione = valore;
                            break;
                        case "PIVA":
                            filtriRic.partitaIva = valore;
                            break;
                        case "CF":
                            filtriRic.codiceFiscale = valore;
                            break;
                        case "MAIL":
                            filtriRic.email = valore;
                            break;
                    }
                    BusinessLogic.Rubrica.DPA3_RubricaSearchAgent corrSearcher = new BusinessLogic.Rubrica.DPA3_RubricaSearchAgent(infoUtente);

                    // DocsPaDB.Query_DocsPAWS.Rubrica query = new DocsPaDB.Query_DocsPAWS.Rubrica(infoUtente);
                    DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                    ArrayList objElementiRubrica = corrSearcher.Search(filtriRic, smistamentoRubrica);
                    if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                    {
                        oggCustom.VALORE_DATABASE = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                    }
                }

                if ((oggCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                {

                    // Lo studio sulla casella di selezione verrà fatto in seguito. Bisogna capire cosa fare.

                    //for (int i = 0; i < oggCustom.ELENCO_VALORI.Count; i++)
                    //{

                    //            if (((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggCustom.ELENCO_VALORI[i]).VALORE.Equals(word))
                    //            {
                    //                oggCustom.VALORI_SELEZIONATI[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggCustom.ELENCO_VALORI[i]).VALORE;
                    //                break;
                    //            }

                    //}
                }




            }
            catch (Exception e)
            {
                oggCustom.VALORE_DATABASE = string.Empty;
                oggCustom.VALORI_SELEZIONATI = null;
            }

            if (oggCustom.CAMPO_OBBLIGATORIO == "SI" && (string.IsNullOrEmpty(oggCustom.VALORE_DATABASE) || ((oggCustom.VALORI_SELEZIONATI == null || oggCustom.VALORI_SELEZIONATI.Count < 1) && oggCustom.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")))
            {
                throw new PisException("FIELD_REQUIRED");
            }

        }

        // Modifica Elaborazione XML da PIS req.3
        public static bool ValidateXmlWithXsd(string stringaXml, string pathXsd)
        {
            bool retval = true;
            //string stringaXsd= System.IO.File.ReadAllText(pathXsd);
            System.Xml.Schema.XmlSchemaSet schemas = new System.Xml.Schema.XmlSchemaSet();
            string[] coppie = pathXsd.Split('>');
            for (int i = 0; i < coppie.Length; i++)
            {
                string[] coppiaXsdTrgtNS = coppie[i].Split('<');
                if (coppiaXsdTrgtNS.Length > 0)
                {
                    schemas.Add(coppiaXsdTrgtNS[1], coppiaXsdTrgtNS[0]);
                }
            }

            //schemas.Add("", XmlReader.Create(new System.IO.StringReader(stringaXsd)));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(stringaXml);
            //XmlReaderSettings xmlrs = new XmlReaderSettings();
            //xmlrs.Schemas.Add(schemas);
            //xmlrs.ValidationType = ValidationType.Schema;
            xmlDoc.Schemas.Add(schemas);
            //System.Xml.Schema.ValidationEventHandler eventHandler = new System.Xml.Schema.ValidationEventHandler(ValidationEventHandler);
            try
            {
                xmlDoc.Validate(null);
            }
            catch (Exception ex)
            {
                throw new PisException("ELAB_XML_XSD_VALIDATION_ERROR");
            }
            return retval;
        }

        /// <summary>
        /// Alcuni xml possono avere come primo carattere un Byte Order Mark. Sdi in particolare.
        /// Da problemi nel xml.load, quindi facciamo in modo che sia escluso se presente.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string RemoveUtf8ByteOrderMark(string xml)
        {
            // Prova alternativa


            //string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            //if (xml.StartsWith(byteOrderMarkUtf8) && !xml.StartsWith("<"))
            //{
            //    xml = xml.Remove(0, byteOrderMarkUtf8.Length);
            //}
            return xml.Trim();
        }

        //static void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        //{
        //    switch (e.Severity)
        //    {
        //        case System.Xml.Schema.XmlSeverityType.Error:
        //            throw new PisException("ELAB_XML_XSD_VALIDATION_ERROR");
        //            break;
        //    }
        //}

        /// <summary>
        /// Reperimento oggetto classificazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceClassifica"></param>
        /// <returns></returns>
        public static DocsPaVO.fascicolazione.Classificazione GetClassificazione(DocsPaVO.utente.InfoUtente infoUtente, string codiceClassifica)
        {
            System.Collections.ArrayList listClassificazioni = BusinessLogic.Fascicoli.TitolarioManager.getTitolario(
            infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, null, codiceClassifica, false);

            if (listClassificazioni != null && listClassificazioni.Count > 0)
                return (DocsPaVO.fascicolazione.Classificazione)listClassificazioni[0];
            else
            {
                return null;
            }
        }

        public static DocsPaVO.fascicolazione.Fascicolo GetProjectFromPis(Domain.Project project, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.fascicolazione.Fascicolo result = new DocsPaVO.fascicolazione.Fascicolo();

            if (project == null)
            {
                result = null;
            }
            else
            {
                result.idTitolario = project.ClassificationScheme.Id;
                result.dtaLF = project.CollocationDate;
                result.descrizione = project.Description;
                result.numFascicolo = project.Number;

                result.noteFascicolo = null;
                if (project.Note != null && project.Note.Length > 0)
                {
                    result.noteFascicolo = new DocsPaVO.Note.InfoNota[project.Note.Length];
                    int y = 0;
                    foreach (Domain.Note nota in project.Note)
                    {
                        DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                        tempNot.DaInserire = true;
                        tempNot.Testo = nota.Description;
                        tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                        tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                        tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                        tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                        result.noteFascicolo[y] = tempNot;
                        y++;
                    }
                }

                result.cartaceo = project.Paper;

                if (project.Controlled)
                {
                    result.controllato = "1";
                }
                else
                {
                    result.controllato = "0";
                }

                result.idUoLF = project.PhysicsCollocation;

                result.dtaLF = project.CollocationDate;

                if (project.Private)
                {
                    result.privato = "1";
                }
                else
                {
                    result.privato = "0";
                }

                if (project.Register != null)
                    result.idRegistro = project.Register.Id;
                result.apertura = DateTime.Now.ToString("dd/MM/yyyy");
                result.codiceGerarchia = project.CodeNodeClassification;

            }

            return result;
        }

        public static DocsPaVO.utente.DatiModificaCorr GetModificaCorrespondentFromPis(Domain.Correspondent pis, DocsPaVO.utente.Corrispondente corr)
        {
            DocsPaVO.utente.DatiModificaCorr result = new DatiModificaCorr();

            if (pis == null)
            {
                throw new PisException("MISSING_PARAMETER");
            }
            else
            {
                if (!string.IsNullOrEmpty(pis.Cap))
                {
                    result.cap = pis.Cap;
                }
                else
                {
                    result.cap = corr.cap;
                }
                if (!string.IsNullOrEmpty(pis.City))
                {
                    result.citta = pis.City;
                }
                else
                {
                    result.citta = corr.citta;
                }
                if (!string.IsNullOrEmpty(pis.NationalIdentificationNumber))
                {
                    result.codFiscale = pis.NationalIdentificationNumber;
                }
                else
                {
                    result.codFiscale = corr.codfisc;
                }
                if (!string.IsNullOrEmpty(pis.Code))
                {
                    result.codice = pis.Code;
                }
                else
                {
                    result.codice = corr.codiceRubrica;
                }
                if (!string.IsNullOrEmpty(pis.AdmCode))
                {
                    result.codiceAmm = pis.AdmCode;
                }
                else
                {
                    result.codiceAmm = corr.codiceAmm;
                }
                if (!string.IsNullOrEmpty(pis.AOOCode))
                {
                    result.codiceAoo = pis.AOOCode;
                }
                else
                {
                    result.codiceAoo = corr.codiceAOO;
                }
                if (!string.IsNullOrEmpty(pis.Code))
                {
                    result.codRubrica = pis.Code;
                }
                else
                {
                    result.codRubrica = corr.codiceRubrica;
                }
                if (!string.IsNullOrEmpty(pis.Surname))
                {
                    result.cognome = pis.Surname;
                }
                else
                {
                    result.cognome = corr.cognome;
                }
                if (!string.IsNullOrEmpty(pis.Description))
                {
                    result.descCorr = pis.Description;
                }
                else
                {
                    result.descCorr = corr.descrizione;
                }
                if (!string.IsNullOrEmpty(pis.Email))
                {
                    result.email = pis.Email;
                }
                else
                {
                    result.email = corr.email;
                }
                if (!string.IsNullOrEmpty(pis.Fax))
                {
                    result.fax = pis.Fax;
                }
                else
                {
                    result.fax = corr.fax;
                }

                result.idCorrGlobali = corr.systemId;

                if (!string.IsNullOrEmpty(pis.Address))
                {
                    result.indirizzo = pis.Address;
                }
                else
                {
                    result.indirizzo = corr.indirizzo;
                }
                if (!string.IsNullOrEmpty(pis.Location))
                {
                    result.localita = pis.Location;
                }
                else
                {
                    result.localita = corr.localita;
                }
                if (!string.IsNullOrEmpty(pis.Nation))
                {
                    result.nazione = pis.Nation;
                }
                else
                {
                    result.nazione = corr.nazionalita;
                }
                if (!string.IsNullOrEmpty(pis.Name))
                {
                    result.nome = pis.Name;
                }
                else
                {
                    result.nome = corr.nome;
                }
                if (!string.IsNullOrEmpty(pis.Note))
                {
                    result.note = pis.Note;
                }
                else
                {
                    result.note = corr.note;
                }
                if (!string.IsNullOrEmpty(pis.Province))
                {
                    result.provincia = pis.Province;
                }
                else
                {
                    result.provincia = corr.prov;
                }
                if (!string.IsNullOrEmpty(pis.PhoneNumber))
                {
                    result.telefono = pis.PhoneNumber;
                }
                else
                {
                    result.telefono = corr.telefono1;
                }
                if (!string.IsNullOrEmpty(pis.PreferredChannel))
                {
                    int idCanalePref = 0;
                    if (Int32.TryParse(pis.PreferredChannel, out idCanalePref))
                    {
                        result.idCanalePref = pis.PreferredChannel;
                    }
                    else
                    {
                        result.idCanalePref = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc(pis.PreferredChannel);
                    }
                    //result.descrizioneCanalePreferenziale = pis.PreferredChannel;
                }
                else
                {
                    if (corr.canalePref != null)
                        result.descrizioneCanalePreferenziale = corr.canalePref.descrizione;
                }
            }

            return result;
        }

        public static DocsPaVO.utente.InfoUtente CheckAuthentication(Services.Request request, string pisMethodName)
        {
            DocsPaVO.utente.InfoUtente result = null;

            if (request == null || string.IsNullOrEmpty(request.UserName))
            {
                throw new PisException("MISSING_PARAMETER");
            }

            if (string.IsNullOrEmpty(request.CodeAdm) && string.IsNullOrEmpty(request.AuthenticationToken))
            {
                throw new PisException("MISSING_PARAMETER");
            }
            bool isSysExt = false;
            bool isUtenteAutomatico = false;
            //string idAmm = "";
            //if (!string.IsNullOrEmpty(request.CodeAdm))
            //{
            //    try
            //    {
            //        idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(request.CodeAdm);
            //        if (string.IsNullOrEmpty(idAmm))
            //            throw new PisException("ADMINISTRATION_NOT_FOUND");
            //    }
            //    catch (Exception exxx)
            //    {
            //        throw new PisException("ADMINISTRATION_NOT_FOUND");
            //    }
            //}
            logger.Debug("BEGIN");
            // Autenticazione Sistemi Esterni R.2 
            //if (System.Configuration.ConfigurationManager.AppSettings["AUTH_SYS_EXT_TRANSITION"]!=null && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["AUTH_SYS_EXT_TRANSITION"].ToString()) && System.Configuration.ConfigurationManager.AppSettings["AUTH_SYS_EXT_TRANSITION"].ToString() == "1")
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_AUTH_SYS_EXT_TRANSITION")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_AUTH_SYS_EXT_TRANSITION") == "1")
            {
                logger.Debug("Chiave sysext attiva");
                if (!string.IsNullOrEmpty(request.AuthenticationToken))
                {
                    if (!DocsPaUtils.Security.SSOAuthTokenHelper.IsAuthToken(request.AuthenticationToken))
                    {
                        throw new PisException("INVALID_TOKEN");
                    }
                }
            }
            else
            {
                logger.Debug("Chiave sysExt disattiva");
                if (string.IsNullOrEmpty(request.AuthenticationToken))
                {
                    throw new PisException("MISSING_PARAMETER");
                }
                else
                {
                    if (!string.IsNullOrEmpty(pisMethodName))
                    {
                        logger.DebugFormat("PIS: {0}", pisMethodName);
                        // il controllo sul pisMethodName vuoto è a causa dei log DB...
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(request.CodeAdm);
                        logger.DebugFormat("AMM Code: {0}, id {1}", request.CodeAdm, idAmm);
                        DocsPaVO.amministrazione.SistemaEsterno sysExt = BusinessLogic.Amministrazione.SistemiEsterni.getSistemaEsternoByUserid(idAmm, request.UserName);
                        string tknDuration = "20";
                        if (sysExt == null)
                        {
                            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_PIS_TOKEN_DURATION")))
                            {
                                tknDuration = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "BE_PIS_TOKEN_DURATION");

                            }
                        }
                        else
                        {
                            isSysExt = true;
                            tknDuration = sysExt.TokenPeriod.ToString();
                        }
                        logger.DebugFormat("Periodo Token: {0}", tknDuration);
                        string controllo = DocsPaUtils.Security.SSOAuthTokenHelper.CtrlAuthToken(request.AuthenticationToken, request.UserName, tknDuration);
                        logger.DebugFormat("Controllo: {0}", controllo);
                        if (controllo == "WRONG_USER")
                        {
                            throw new Exception("Token non formalmente corretto");
                        }
                        else if (controllo == "TKN_EXPIRED")
                        {
                            throw new Exception("Token scaduto");
                        }
                        else if (controllo == "OK" && sysExt != null)
                        {
                            logger.DebugFormat("Diritti: {0}", sysExt.Diritti);
                            if (!sysExt.Diritti.ToUpper().Contains(pisMethodName.ToUpper() + ";"))
                            {
                                throw new Exception("Utente non autorizzato all'utilizzo del metodo.");
                            }

                        }
                    }
                }
            }


            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = null;
            string dst = string.Empty;

            //Caso in cui utilizzo il token
            if (!string.IsNullOrEmpty(request.AuthenticationToken))
            {
                // La procedura deve essere cambiata. Potrebbe dare problemi con gli utenti multiamministrazione.
                //utente = BusinessLogic.Utenti.UserManager.getUtente(request.UserName, Utils.GetIdAmministrazione(request.UserName));

                // Procedura Nuova
                utente = BusinessLogic.Utenti.UserManager.getUtente(request.UserName, Utils.GetIdAdmByCodeAdm(request.CodeAdm));
                isUtenteAutomatico = IsUtenteAutomatico(utente);
            }
            else
            {
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN")) && DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_SSOLOGIN").Equals("1"))
                {
                    utente = BusinessLogic.Utenti.UserManager.getUtenteByCodice(request.UserName, request.CodeAdm);
                    if (utente == null)
                    {
                        //Utente non trovato
                        throw new PisException("USER_NO_EXIST");
                    }
                    else
                    {
                        string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(request.UserName);

                        DocsPaVO.utente.UserLogin login = new UserLogin();
                        login.Password = token;
                        login.UserName = request.UserName;
                        login.SSOLogin = true;
                        string idWebSession = string.Empty;
                        string ipAddress = string.Empty;
                        DocsPaVO.utente.UserLogin.LoginResult loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                        try
                        {
                            utente = BusinessLogic.Utenti.Login.loginMethod(login, out loginResult, true, idWebSession, out ipAddress);
                            dst = utente.dst;
                        }
                        catch (PisException pisEx)
                        {
                            //Utente non trovato
                            throw new PisException("USER_NO_EXIST");
                        }
                    }
                }
            }

            if (utente == null)
            {
                //Utente non trovato
                throw new PisException("USER_NO_EXIST");
            }

            DocsPaVO.utente.Ruolo ruolo = null;
            ArrayList arrayRuoli = new ArrayList();
            bool ruoloOK = false;
            //Se fornisco un determinato ruolo
            if (!string.IsNullOrEmpty(request.CodeRoleLogin))
            {
                if (!isSysExt)
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByCodice(request.CodeRoleLogin);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                    else
                    {
                        //arrayRuoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople);
                        //if (arrayRuoli != null && arrayRuoli.Count > 0)
                        //{
                        //    foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        //    {
                        //        if (rol.codiceRubrica.ToUpper() == ruolo.codice.ToUpper())
                        //            ruoloOK = true;
                        //    }
                        //    if (!ruoloOK) throw new PisException("ROLE_NO_EXIST");
                        //}
                        //Se l'utente è automatico può operare con qualsiasi ruolo
                        if (!isUtenteAutomatico)
                        {
                            List<UserMinimalInfo> utentiinruolo = BusinessLogic.Utenti.UserManager.GetUsersInRoleMinimalInfo(ruolo.idGruppo);
                            if (utentiinruolo != null && utentiinruolo.Count > 0)
                            {
                                foreach (UserMinimalInfo umi in utentiinruolo)
                                {
                                    if (umi.SystemId == utente.idPeople) ruoloOK = true;
                                }
                            }
                            if (!ruoloOK) throw new PisException("ROLE_NO_EXIST");
                        }
                    }
                }
                else
                {
                    throw new Exception("Property CodeRoleLogin not valid for external systems");
                }
            }
            else
            {
                ruolo = Utils.GetRuoloPreferito(utente.idPeople);
                if (ruolo == null)
                {
                    //L'utente non ha ruoli
                    throw new PisException("USER_NO_ROLE");
                }
            }
            if (isSysExt)
            {
                ruolo.RuoloDiSistema = "1";
            }
            // Impostazione del codice dell'applicazione esterna
            //
            utente.codWorkingApplication = request.CodeApplication;

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

            //   Reperimento token superutente
            // infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
            //Reperimento token utente
            if (string.IsNullOrEmpty(dst))
            {
                //Reperimento token superutente
                infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();
            }
            else
            {
                infoUtente.dst = dst;
            }
            if (isSysExt)
            {
                infoUtente.diSistema = "1";
            }
            result = infoUtente;

            return result;
        }

        private static bool IsUtenteAutomatico(Utente utente)
        {
            bool isAutomatico = false;

            Utente utenteAutomatico = BusinessLogic.Utenti.UserManager.GetUtenteAutomatico(utente.idAmministrazione);
            if (utenteAutomatico != null)
                isAutomatico = utenteAutomatico.idPeople.Equals(utente.idPeople);

            return isAutomatico;
        }

        public static DocsPaVO.rubrica.ParametriRicercaRubrica GetParametriRicercaRubricaFromPis(Domain.Filter[] filters, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.rubrica.ParametriRicercaRubrica result = new DocsPaVO.rubrica.ParametriRicercaRubrica();

            result.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
            result.parent = "";
            result.caller.IdRuolo = infoUtente.idGruppo;
            result.caller.filtroRegistroPerRicerca = string.Empty;
            bool filterFound = false;

            foreach (Domain.Filter fil in filters)
            {
                if (fil != null && !string.IsNullOrEmpty(fil.Value))
                {
                    if (fil.Name.ToUpper().Equals("OFFICES"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doUo = true;
                        }
                        else
                        {
                            result.doUo = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("USERS"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doUtenti = true;
                        }
                        else
                        {
                            result.doUtenti = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("ROLES"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRuoli = true;
                        }
                        else
                        {
                            result.doRuoli = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("TYPE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            if (fil.Value.ToUpper().Equals("EXTERNAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                            }
                            if (fil.Value.ToUpper().Equals("INTERNAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
                            }
                            if (fil.Value.ToUpper().Equals("GLOBAL"))
                            {
                                result.tipoIE = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                            }
                        }
                    }

                    if (fil.Name.ToUpper().Equals("COMMON_ADDRESSBOOK"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRubricaComune = true;
                        }
                        else
                        {
                            result.doRubricaComune = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("RF"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            result.doRF = true;
                        }
                        else
                        {
                            result.doRF = false;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("CODE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codice = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("EXACT_CODE"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codice = fil.Value;
                            result.queryCodiceEsatta = true;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("DESCRIPTION"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.descrizione = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("CITY"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.citta = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("LOCALITY"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.localita = fil.Value;
                        }
                    }

                    if (fil.Name.ToUpper().Equals("MAIL"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.email = fil.Value;
                        }
                    }
                    // Modifica 23-01-2013: In seguito alla distinzione dei valori di codice fiscale e partita iva, la ricerca va fatta sui due campi separatamente.
                    // Quindi devono essere creati dei filtri distinti.
                    //if (fil.Name.ToUpper().Equals("NAT"))
                    //{
                    //    filterFound = true;
                    //    if (!string.IsNullOrEmpty(fil.Value))
                    //    {
                    //        result.cf_piva = fil.Value;
                    //    }
                    //}
                    if (fil.Name.ToUpper().Equals("NATIONAL_IDENTIFICATION_NUMBER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.codiceFiscale = fil.Value;
                        }
                    }
                    if (fil.Name.ToUpper().Equals("VAT_NUMBER"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            result.partitaIva = fil.Value;
                        }
                    }
                    if (fil.Name.ToUpper().Equals("REGISTRY_OR_RF"))
                    {
                        filterFound = true;
                        if (!string.IsNullOrEmpty(fil.Value))
                        {
                            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(fil.Value);
                            if (reg == null)
                            {
                                //Registro non trovato
                                throw new PisException("REGISTER_NOT_FOUND");
                            }
                            else
                            {
                                result.caller.IdRegistro = reg.systemId;
                                result.caller.filtroRegistroPerRicerca = reg.systemId;
                            }
                        }
                    }
                    if (fil.Name.ToUpper().Equals("EXTRACT_ID_COMMONADDRESSBOOK"))
                    {
                        filterFound = true;
                    }
                    if (fil.Name.ToUpper().Equals("EXTENDED_SEARCH_NO_REG"))
                    {
                        if (!string.IsNullOrEmpty(fil.Value) && fil.Value.ToUpper().Equals("TRUE"))
                        {
                            filterFound = true;
                            //result.calltype = DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO;
                            result.caller.filtroRegistroPerRicerca = "";
                            ArrayList registriLista = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(infoUtente.idCorrGlobali, string.Empty, string.Empty);
                            foreach (DocsPaVO.utente.Registro reg in registriLista)
                            {
                                if (!string.IsNullOrEmpty(result.caller.filtroRegistroPerRicerca))
                                    result.caller.filtroRegistroPerRicerca += ("," + reg.systemId);
                                else result.caller.filtroRegistroPerRicerca += reg.systemId;
                            }
                        }
                    }
                    if (!filterFound)
                        throw new PisException("FILTER_NOT_FOUND");

                }
            }

            return result;
        }

        public static Domain.TransmissionModel GetModel(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model)
        {
            Domain.TransmissionModel result = new Domain.TransmissionModel();
            if (model == null)
            {
                result = null;
                //modello di trasmissione non trovato
                throw new PisException("TRANSMISSION_MODEL_NOT_FOUND");
            }
            else
            {
                result.Code = model.CODICE;
                result.Description = model.NOME;
                result.Id = model.SYSTEM_ID.ToString();
                result.Type = model.CHA_TIPO_OGGETTO;
            }
            return result;
        }

        public static DocsPaVO.Spedizione.SpedizioneDocumento GetInfoSpedizione(DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.Spedizione.SpedizioneDocumento result = new DocsPaVO.Spedizione.SpedizioneDocumento();
            if (documento.tipoProto.Equals("P") || (documento.tipoProto.Equals("I")))
            {
                if (documento.tipoProto.Equals("P"))
                {
                    if (((ProtocolloUscita)documento.protocollo).destinatariConoscenza == null)
                    {
                        ((ProtocolloUscita)documento.protocollo).destinatariConoscenza = new ArrayList();
                    }
                }
                if (documento.tipoProto.Equals("I"))
                {
                    if (((ProtocolloInterno)documento.protocollo).destinatariConoscenza == null)
                    {
                        ((ProtocolloInterno)documento.protocollo).destinatariConoscenza = new ArrayList();
                    }
                }
            }
            result = BusinessLogic.Spedizione.SpedizioneManager.GetSpedizioneDocumento(infoUtente, documento);

            return result;
        }

        public static DocsPaVO.utente.Corrispondente GetCorrespondentFromPisNewInsert(Domain.Correspondent corr, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.utente.Corrispondente result = new DocsPaVO.utente.Corrispondente();

            if (corr != null)
            {
                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("U"))
                {
                    result = new DocsPaVO.utente.UnitaOrganizzativa();
                }

                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("P"))
                {
                    result = new DocsPaVO.utente.Utente();
                    ((DocsPaVO.utente.Utente)result).cognome = corr.Surname;
                    ((DocsPaVO.utente.Utente)result).nome = corr.Name;
                }

                if (!string.IsNullOrEmpty(corr.Code))
                {
                    result.codiceCorrispondente = corr.Code;
                    result.codiceRubrica = corr.Code;
                }
                else
                {
                    throw new Exception("Codice del corrispondente mancante.");
                }
                result.descrizione = corr.Description;
                result.tipoCorrispondente = corr.Type;
                result.email = corr.Email;
                result.idAmministrazione = infoUtente.idAmministrazione;
                if (!string.IsNullOrEmpty(corr.CodeRegisterOrRF))
                {
                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(corr.CodeRegisterOrRF);
                    if (reg == null)
                    {
                        //Registro non trovato
                        throw new PisException("REGISTER_NOT_FOUND");
                    }
                    else
                    {
                        result.idRegistro = reg.systemId;
                    }
                }

                ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                DocsPaVO.utente.Canale canaleLettera = null;
                foreach (MezzoSpedizione m_spediz in mezziSpedizione)
                {
                    if (corr.PreferredChannel != null)
                    {

                        if (m_spediz.Descrizione.ToUpper().Equals(corr.PreferredChannel.ToUpper()))
                        {
                            result.canalePref = new DocsPaVO.utente.Canale();
                            result.canalePref.descrizione = m_spediz.Descrizione;
                            result.canalePref.systemId = m_spediz.IDSystem;
                            result.canalePref.tipoCanale = m_spediz.chaTipoCanale;
                            break;
                        }
                    }

                    if (m_spediz.Descrizione.ToUpper().Equals("LETTERA"))
                    {
                        canaleLettera = new DocsPaVO.utente.Canale();
                        canaleLettera.descrizione = m_spediz.Descrizione;
                        canaleLettera.systemId = m_spediz.IDSystem;
                        canaleLettera.tipoCanale = m_spediz.chaTipoCanale;

                    }

                }

                if (result.canalePref == null)
                {
                    result.canalePref = canaleLettera;
                }


                string indirizzo = "";
                string citta = "";
                string cap = "";
                string provincia = "";
                string nazione = "";
                string telefono = "";
                string fax = "";
                string cf = "";
                string note = "";
                string locazione = "";
                string partitaIva = "";
                string codiceIpa = "";

                if (!string.IsNullOrEmpty(corr.Address))
                {
                    indirizzo = corr.Address.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.City))
                {
                    citta = corr.City.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Cap))
                {
                    cap = corr.Cap.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Province))
                {
                    provincia = corr.Province.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Nation))
                {
                    nazione = corr.Nation.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.PhoneNumber))
                {
                    telefono = corr.PhoneNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Fax))
                {
                    fax = corr.Fax.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.NationalIdentificationNumber))
                {
                    cf = corr.NationalIdentificationNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Note))
                {
                    note = corr.Note.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.Location))
                {
                    locazione = corr.Location.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                if (!string.IsNullOrEmpty(corr.VatNumber))
                {
                    partitaIva = corr.VatNumber.TrimStart(" ".ToCharArray()).TrimEnd(" ".ToCharArray());
                }
                //Dettagli corrispondente
                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                dettagli.Corrispondente.AddCorrispondenteRow(
                      indirizzo,
                       citta,
                        cap,
                        provincia,
                        nazione,
                        telefono,
                        "",
                        fax,
                        cf,
                        note,
                        locazione,
                        "",
                        "",
                        "",
                        partitaIva
                    //,codiceIpa
                        );

                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("U"))
                {
                    ((DocsPaVO.utente.UnitaOrganizzativa)result).info = dettagli;
                }
                if (corr.CorrespondentType != null && corr.CorrespondentType.Equals("P"))
                {
                    ((DocsPaVO.utente.Utente)result).info = dettagli;
                }
                result.dettagli = false;

            }
            else
            {
                result = null;
            }

            return result;
        }

        public static Domain.Stamp GetStampAndSignature(DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Domain.Stamp result = new Domain.Stamp();
            if (documento == null)
            {
                result = null;
            }
            else
            {
                Dictionary<string, string> dati = BusinessLogic.Documenti.DocManager.GetDatiSegnaturaTimbro(documento.systemId);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("segnatura"))
                    {
                        result.SignatureValue = dati["segnatura"];
                    }
                    if (dati.ContainsKey("resgistro"))
                    {
                        result.CodeRegister = dati["resgistro"];
                    }
                    if (dati.ContainsKey("uo"))
                    {
                        result.CodeUO = dati["uo"];
                    }
                    if (dati.ContainsKey("amministrazione"))
                    {
                        result.CodeAdministration = dati["amministrazione"];
                    }
                    if (dati.ContainsKey("anno"))
                    {
                        result.Year = dati["anno"];
                    }
                    if (dati.ContainsKey("data"))
                    {
                        result.DataProtocol = dati["data"];
                    }
                    if (dati.ContainsKey("ora"))
                    {
                        result.TimeProtocol = dati["ora"];
                    }
                    if (dati.ContainsKey("tipo"))
                    {
                        result.TypeProtocol = dati["tipo"];
                    }
                    if (dati.ContainsKey("numero"))
                    {
                        result.NumberProtocol = dati["numero"];
                    }
                    if (dati.ContainsKey("allegati"))
                    {
                        result.NumberAtthachements = dati["allegati"];
                    }
                    if (dati.ContainsKey("fascicoli"))
                    {
                        result.Classifications = dati["fascicoli"];
                    }
                    if (dati.ContainsKey("docnumber"))
                    {
                        result.DocNumber = dati["docnumber"];
                    }
                    if (dati.ContainsKey("rf"))
                    {
                        result.CodeRf = dati["rf"];
                    }
                }
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                string TimbroIniziale = infoAmm.Timbro_pdf;
                string timbro = GetDatiTimbro(TimbroIniziale, TimbroIniziale, infoAmm, result);
                result.StampValue = timbro;

            }

            return result;
        }

        #region UtilsTimbro
        /// <summary>
        /// Questo metodo rimuove i separatori o la descrizione dei campi non valorizzati del timbro
        /// </summary>
        /// <returns></returns>
        private static string RemoveDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int count = 0;
            int start = 0;
            int inizio = 0;
            string specialChar = "#%*@";
            //Uso un ciclo while nel caso ci sia per errore più di un'occorrenza del codice da rimuovere!
            while (currTimbro.Contains(currVal))
            {
                //devo ricalcolare l'indice dei codici precedenti
                int[] ordine = codicePrec(timbro_iniziale);

                if (currVal.Equals("COD_AMM"))
                {
                    if (ordine[0] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                        count = currTimbro.IndexOf("COD_AMM") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_AMM");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_REG"))
                {
                    if (ordine[1] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                        count = currTimbro.IndexOf("COD_REG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_REG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_PROTO"))
                {
                    if (ordine[2] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                        count = currTimbro.IndexOf("NUM_PROTO") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_PROTO");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("DATA_COMP"))
                {
                    if (ordine[3] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                        count = currTimbro.IndexOf("DATA_COMP") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("DATA_COMP");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("ORA"))
                {
                    if (ordine[4] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                        count = currTimbro.IndexOf("ORA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("ORA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("NUM_ALLEG"))
                {
                    if (ordine[5] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                        count = currTimbro.IndexOf("NUM_ALLEG") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("NUM_ALLEG");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("CLASSIFICA"))
                {
                    if (ordine[6] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                        count = currTimbro.IndexOf("CLASSIFICA") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("CLASSIFICA");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("IN_OUT"))
                {
                    if (ordine[7] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                        count = currTimbro.IndexOf("IN_OUT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("IN_OUT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (currVal.Equals("COD_UO_PROT"))
                {
                    if (ordine[8] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[8]]) + dati[ordine[8]].Length;
                        count = currTimbro.IndexOf("COD_UO_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_UO_VIS"))
                {
                    if (ordine[9] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[9]]) + dati[ordine[9]].Length;
                        count = currTimbro.IndexOf("COD_UO_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_UO_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_PROT"))
                {
                    if (ordine[10] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[10]]) + dati[ordine[10]].Length;
                        count = currTimbro.IndexOf("COD_RF_PROT") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_PROT");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                if (currVal.Equals("COD_RF_VIS"))
                {
                    if (ordine[11] >= 0)
                    {
                        start = currTimbro.IndexOf(dati[ordine[11]]) + dati[ordine[11]].Length;
                        count = currTimbro.IndexOf("COD_RF_VIS") - start;
                        currTimbro = currTimbro.Remove(start, count);
                    }
                    else
                    {
                        count = currTimbro.IndexOf("COD_RF_VIS");
                        currTimbro = currTimbro.Remove(0, count);
                    }
                    inizio = currTimbro.IndexOf(currVal);
                    currTimbro = currTimbro.Remove(inizio, currVal.Length);
                    currTimbro = currTimbro.Insert(inizio, specialChar);
                    timbro_iniziale = timbro_iniziale.Remove(timbro_iniziale.IndexOf(currVal), currVal.Length);
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            }
            currTimbro = currTimbro.Replace(specialChar, currVal);
            return currTimbro;
        }

        /// <summary>
        /// Restituisce il vettore degli indici dei codici precedenti
        /// </summary>
        /// <param name="timbro_iniziale"></param>
        /// <returns></returns>
        private static int[] codicePrec(string timbro_iniziale)
        {
            int[] ordine = new int[12];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }
            return ordine;
        }

        /// <summary>
        /// Questo metodo restituisce i separatori o la descrizione dei campi richiesti
        /// </summary>
        /// <returns></returns>
        private static string ReturnDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int[] ordine = new int[8];
            int i = -1;
            while (timbro_iniziale != string.Empty)
            {
                string appo = timbro_iniziale;
                if (timbro_iniziale.StartsWith("COD_AMM"))
                {
                    ordine[0] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_AMM", "");
                    i = 0;
                }
                if (timbro_iniziale.StartsWith("COD_REG"))
                {
                    ordine[1] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_REG", "");
                    i = 1;
                }
                if (timbro_iniziale.StartsWith("NUM_PROTO"))
                {
                    ordine[2] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_PROTO", "");
                    i = 2;
                }
                if (timbro_iniziale.StartsWith("DATA_COMP"))
                {
                    ordine[3] = i;
                    timbro_iniziale = timbro_iniziale.Replace("DATA_COMP", "");
                    i = 3;
                }
                if (timbro_iniziale.StartsWith("ORA"))
                {
                    ordine[4] = i;
                    timbro_iniziale = timbro_iniziale.Replace("ORA", "");
                    i = 4;
                }
                if (timbro_iniziale.StartsWith("NUM_ALLEG"))
                {
                    ordine[5] = i;
                    timbro_iniziale = timbro_iniziale.Replace("NUM_ALLEG", "");
                    i = 5;
                }
                if (timbro_iniziale.StartsWith("CLASSIFICA"))
                {
                    ordine[6] = i;
                    timbro_iniziale = timbro_iniziale.Replace("CLASSIFICA", "");
                    i = 6;
                }
                if (timbro_iniziale.StartsWith("IN_OUT"))
                {
                    ordine[7] = i;
                    timbro_iniziale = timbro_iniziale.Replace("IN_OUT", "");
                    i = 7;
                }

                // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
                if (timbro_iniziale.StartsWith("COD_UO_PROT"))
                {
                    ordine[8] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_PROT", "");
                    i = 8;
                }
                if (timbro_iniziale.StartsWith("COD_UO_VIS"))
                {
                    ordine[9] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_UO_VIS", "");
                    i = 9;
                }
                if (timbro_iniziale.StartsWith("COD_RF_PROT"))
                {
                    ordine[10] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_PROT", "");
                    i = 10;
                }
                if (timbro_iniziale.StartsWith("COD_RF_VIS"))
                {
                    ordine[11] = i;
                    timbro_iniziale = timbro_iniziale.Replace("COD_RF_VIS", "");
                    i = 11;
                }
                // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                //se il timbro iniziale è rimasto invariato rimuovo un carattere ed inizio di nuovo la ricerca
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }

            int count = 0;
            int start = 0;
            if (currVal.Equals("COD_AMM"))
            {
                if (ordine[0] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[0]]) + dati[ordine[0]].Length;
                    count = currTimbro.IndexOf("COD_AMM") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_AMM");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("COD_REG"))
            {
                if (ordine[1] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[1]]) + dati[ordine[1]].Length;
                    count = currTimbro.IndexOf("COD_REG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("COD_REG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_PROTO"))
            {
                if (ordine[2] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[2]]) + dati[ordine[2]].Length;
                    count = currTimbro.IndexOf("NUM_PROTO") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_PROTO");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("DATA_COMP"))
            {
                if (ordine[3] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[3]]) + dati[ordine[3]].Length;
                    count = currTimbro.IndexOf("DATA_COMP") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("DATA_COMP");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("ORA"))
            {
                if (ordine[4] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[4]]) + dati[ordine[4]].Length;
                    count = currTimbro.IndexOf("ORA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("ORA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("NUM_ALLEG"))
            {
                if (ordine[5] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[5]]) + dati[ordine[5]].Length;
                    count = currTimbro.IndexOf("NUM_ALLEG") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("NUM_ALLEG");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("CLASSIFICA"))
            {
                if (ordine[6] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[6]]) + dati[ordine[6]].Length;
                    count = currTimbro.IndexOf("CLASSIFICA") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("CLASSIFICA");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            if (currVal.Equals("IN_OUT"))
            {
                if (ordine[7] >= 0)
                {
                    start = currTimbro.IndexOf(dati[ordine[7]]) + dati[ordine[7]].Length;
                    count = currTimbro.IndexOf("IN_OUT") - start;
                    currTimbro = currTimbro.Substring(start, count);
                }
                else
                {
                    count = currTimbro.IndexOf("IN_OUT");
                    currTimbro = currTimbro.Substring(0, count);
                }
            }
            return currTimbro;
        }
        #endregion

        public static string GetDatiTimbro(string TimbroIniziale, string datiTimbro, DocsPaVO.amministrazione.InfoAmministrazione currAmm, Domain.Stamp stamp)
        {
            string retValue = string.Empty;
            //separatore fra un valore e l'altro del timbro
            string separatore = " ";
            //valore di fine riga per determinare se andare a capo oppure no
            string escape = String.Empty;
            //valore che verrà restituito come output alla richiesta di dati da stampare su pdf!!!
            string Timbro = String.Empty;

            string sep = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getSeparator();
            //è l'ultimo valore sostituito nella fase di creazione del timbro
            string[] lastVal = { "COD_AMM", "COD_REG", "NUM_PROTO", "DATA_COMP", "ORA", "NUM_ALLEG", "CLASSIFICA", "IN_OUT", "COD_UO_PROT", "COD_UO_VIS", "COD_RF_PROT", "COD_RF_VIS" };
            if (datiTimbro.Contains("COD_AMM"))
            {
                //string codAmm = DocsPaDB.Utils.Personalization.getInstance(sch.registro.idAmministrazione).getCodiceAmministrazione();
                string codAmm = DocsPaDB.Utils.Personalization.getInstance(currAmm.IDAmm).getCodiceAmministrazione();
                if (codAmm != String.Empty)
                {
                    datiTimbro = datiTimbro.Replace("COD_AMM", (codAmm + escape));
                    lastVal[0] = codAmm + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_AMM", lastVal);
                    //Metodo alternativo al RemoveDesc che lascia le etichette ma gestisce la nuova linea
                    //datiTimbro = datiTimbro.Remove((datiTimbro.IndexOf("COD_AMM") - 1), 1);
                    //datiTimbro = datiTimbro.Replace("COD_AMM", escape);
                    datiTimbro = datiTimbro.Replace("COD_AMM", "");
                }
            }

            //questo è il codice AOO ed è valorizzato solo sui documenti protocollati
            if (datiTimbro.Contains("COD_REG"))
            {
                lastVal[1] = stamp.CodeRegister + escape;
                datiTimbro = datiTimbro.Replace("COD_REG", stamp.CodeRegister);
            }

            //numero protocollo
            if (datiTimbro.Contains("NUM_PROTO"))
            {
                //Normalizzo il numero di protocollo secondo lo standard a 7 cifre
                int MAX_LENGTH = 7;
                string zeroes = "";
                string numProto = "";

                if (stamp.TypeProtocol.Equals("G"))
                {
                    numProto = stamp.DocNumber;
                    for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                    {
                        zeroes = zeroes + "0";
                    }
                    numProto = zeroes + numProto;

                    datiTimbro = datiTimbro.Replace("NUM_PROTO", ("ID: " + numProto + escape));
                    lastVal[2] = numProto + escape;
                }
                else
                {
                    numProto = stamp.NumberProtocol;
                    for (int ind = 1; ind <= MAX_LENGTH - numProto.Length; ind++)
                    {
                        zeroes = zeroes + "0";
                    }
                    numProto = zeroes + numProto;

                    datiTimbro = datiTimbro.Replace("NUM_PROTO", (numProto + escape));
                    lastVal[2] = numProto + escape;
                }
            }

            //data protocollazione
            if (datiTimbro.Contains("DATA_COMP"))
            {
                datiTimbro = datiTimbro.Replace("DATA_COMP", (stamp.DataProtocol + escape));
                lastVal[3] = stamp.DataProtocol + escape;
            }

            //ora di protocollazione
            if (datiTimbro.Contains("ORA"))
            {
                //aggiunta dell'ora di protocollazione nel timbro
                string ora = stamp.TimeProtocol;
                if ((ora != null) && (ora != ""))
                {
                    //se l'ora è nel formato comprensivo dei secondi devo rimuovere i secondi prima di inserire l'ora!
                    if (ora.Length > 5)
                    {
                        ora = ora.Remove((ora.Length - 3), 3);
                    }
                    datiTimbro = datiTimbro.Replace("ORA", (ora + escape));
                    lastVal[4] = ora + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "ORA", lastVal);
                    datiTimbro = datiTimbro.Replace("ORA", "");
                }
            }

            //numero allegati
            if (datiTimbro.Contains("NUM_ALLEG"))
            {
                datiTimbro = datiTimbro.Replace("NUM_ALLEG", (stamp.NumberAtthachements + escape));
                lastVal[5] = System.Convert.ToString(stamp.NumberAtthachements) + escape;
            }

            //classificazione o classificazioni del corrente documento protocollato
            if (datiTimbro.Contains("CLASSIFICA"))
            {
                Timbro = Timbro + stamp.Classifications + escape;
                if (Timbro == string.Empty)
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                    datiTimbro = datiTimbro.Replace("CLASSIFICA", "");
                }
                else
                {
                    datiTimbro = datiTimbro.Replace("CLASSIFICA", Timbro);
                    lastVal[6] = Timbro;
                }

            }

            //Tipo di protocollo
            if (datiTimbro.Contains("IN_OUT"))
            {
                if (string.IsNullOrEmpty(stamp.TypeProtocol))
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "IN_OUT", lastVal);
                    datiTimbro = datiTimbro.Replace("IN_OUT", "");
                }
                else
                {
                    datiTimbro = datiTimbro.Replace("IN_OUT", stamp.TypeProtocol + escape);
                    lastVal[7] = stamp.TypeProtocol + escape;
                }
            }


            // GESTIONE UNITA' ORGANIZZATIVE ED RF ***********************************************
            if (datiTimbro.Contains("COD_UO_PROT"))
            {
                if (!string.IsNullOrEmpty(stamp.CodeUO))
                {
                    datiTimbro = datiTimbro.Replace("COD_UO_PROT", (stamp.CodeUO + escape));
                    lastVal[8] = stamp.CodeUO + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_PROT", lastVal);
                    datiTimbro = datiTimbro.Replace("COD_UO_PROT", "");
                }
            }

            if (datiTimbro.Contains("COD_UO_VIS"))
            {
                if (!string.IsNullOrEmpty(stamp.CodeUO))
                {
                    datiTimbro = datiTimbro.Replace("COD_UO_PROT", (stamp.CodeUO + escape));
                    lastVal[9] = stamp.CodeUO + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_UO_VIS", lastVal);
                    datiTimbro = datiTimbro.Replace("COD_UO_VIS", "");
                }
            }

            if (datiTimbro.Contains("COD_RF_PROT"))
            {
                string rf = stamp.CodeRf;
                if (!string.IsNullOrEmpty(rf))
                {
                    datiTimbro = datiTimbro.Replace("COD_RF_PROT", (rf + escape));
                    lastVal[10] = rf + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_PROT", lastVal);
                    datiTimbro = datiTimbro.Replace("COD_RF_PROT", "");
                }
            }

            if (datiTimbro.Contains("COD_RF_VIS"))
            {
                string rf = stamp.CodeRf;
                if (!string.IsNullOrEmpty(rf))
                {
                    datiTimbro = datiTimbro.Replace("COD_RF_VIS", (rf + escape));
                    lastVal[11] = rf + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_RF_VIS", lastVal);
                    datiTimbro = datiTimbro.Replace("COD_RF_VIS", "");
                }

            }

            Timbro = datiTimbro;

            retValue = Timbro;

            return retValue;
        }

        public static void CheckFilterTypes(Domain.Filter[] filter)
        {
            if (filter != null)
            {
                foreach (Domain.Filter f in filter)
                {
                    if (f != null)
                    {
                        switch (f.Type)
                        {
                            case Domain.FilterTypeEnum.Number:
                                try
                                {
                                    Convert.ToInt32(f.Value);
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Formato Number non corretto");
                                }
                                break;

                            case Domain.FilterTypeEnum.String:
                                break;

                            case Domain.FilterTypeEnum.Bool:
                                try
                                {
                                    Convert.ToBoolean(f.Value);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Formato Bool non corretto");
                                }

                            case Domain.FilterTypeEnum.Date:
                                try
                                {
                                    /*
                                     *  Date Format Example: 01/03/2012
                                     *  dd = day (01)
                                     *  MM = month (03)
                                     *  yyyy = year (2012)
                                     *  / = date separator
                                     */

                                    string pattern = "dd/MM/yyyy";
                                    DateTime dateVal;
                                    if (!DateTime.TryParseExact(f.Value, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                    {
                                        throw new Exception("Formato Date non corretto");
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Formato Date non corretto");
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifica Response.success: se false rilancia una FaultExecption con codice e descrizione presi dall'oggetto Error della Response
        /// </summary>
        /// <param name="response"></param>
        public static void CheckFaultException(Response response)
        {
            if (!response.Success)
            {
                PISFaultException exception = new PISFaultException(response.Error.Code, response.Error.Description);
                throw new FaultException<PISFaultException>(exception, exception.FaultDescription);
            }
        }

        /// <summary>
        /// Metodo utilizzato per il reperimento di un FileDoc contenente le info di un File con le informazioni di firma
        /// </summary>
        /// <param name="response"></param>
        public static Domain.FileDoc.FileDoc GetFileDoc(DocsPaVO.documento.FileRequest fileRequest, bool getFile, DocsPaVO.utente.InfoUtente infoUtente, bool segnatura, bool timbro, string path, DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            Domain.FileDoc.FileDoc result = new Domain.FileDoc.FileDoc();

            if (fileRequest != null)
            {
                result.Description = fileRequest.descrizione;
                result.Id = fileRequest.docNumber;
                result.VersionId = fileRequest.version;
                if (getFile)
                {
                    DocsPaVO.documento.FileDocumento fileDocumento = null;
                    try
                    {
                        if (!segnatura && !timbro)
                        {
                            fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);
                        }
                        else
                        {
                            DocsPaVO.documento.labelPdf position = new labelPdf();
                            if (segnatura)
                            {
                                if (schedaDoc.documentoPrincipale != null)
                                {
                                    position.orientamento = "orizzontale";
                                    position.position = "15-30";
                                    position.sel_color = "1";
                                    position.sel_font = "1";
                                    position.tipoLabel = true;
                                }
                                fileDocumento = BusinessLogic.Documenti.FileManager.getFileConSegnatura(fileRequest, schedaDoc, infoUtente, path, position, false);
                            }
                            else
                            {
                                if (timbro)
                                {
                                    position.orientamento = "orizzontale";
                                    position.position = "15-30";
                                    position.sel_color = "1";
                                    position.sel_font = "1";
                                    position.tipoLabel = true;
                                    fileDocumento = BusinessLogic.Documenti.FileManager.getFileConSegnatura(fileRequest, schedaDoc, infoUtente, path, position, false);
                                }
                            }

                        }
                        result.Name = fileDocumento.name;
                        result.Content = fileDocumento.content;
                        result.MimeType = fileDocumento.contentType;

                        //Region relativa al recupero delle informazioni di verifica firma
                        #region VERIFICAFIRMA

                        //datarif da definire - TO DO:
                        DateTime dataRif = BusinessLogic.Documenti.FileManager.dataRiferimentoValitaDocumento(fileRequest, infoUtente);
                        //end  datarif

                        BusinessLogic.Documenti.FileManager.VerifyFileSignature(fileDocumento, dataRif);

                        result.verifySignatureResult = convertVerifySignatureResult(fileDocumento);
                        result.timestampResult = convertTimestampResult(fileDocumento);

                        #endregion

                    }
                    catch
                    {
                        fileDocumento = null;
                    }
                }
            }

            return result;
        }

        private static Domain.FileDoc.VerifySignatureResult convertVerifySignatureResult(FileDocumento fileDoc)
        {
            Domain.FileDoc.VerifySignatureResult ret = new Domain.FileDoc.VerifySignatureResult();

            if (fileDoc != null)
            {
                //Cambiare gli altri toString con Convert...
                //StatusCode
                ret.StatusCode = Convert.ToString(fileDoc.signatureResult.StatusCode);

                //StatusDescription
                ret.StatusDescription = fileDoc.signatureResult.StatusDescription;

                //FinalDocumentName
                ret.FinalDocumentName = fileDoc.signatureResult.FinalDocumentName;

                //CRLOnlineCheck
                ret.CRLOnlineCheck = fileDoc.signatureResult.CRLOnlineCheck.ToString();

                //PKCS7Document
                int doc_i = 0;
                int sigInf_i = 0;

                //Istanziare l'array PKCS7Document.
                ret.PKCS7Documents = new Domain.FileDoc.PKCS7Document[fileDoc.signatureResult.PKCS7Documents.Length];

                //Sostituire la logica del foreach con for
                //foreach (PKCS7Document doc in fileDoc.signatureResult.PKCS7Documents)
                for (int i = 0; i < fileDoc.signatureResult.PKCS7Documents.Length - 1; i++)
                {
                    //if (doc != null)
                    if (fileDoc.signatureResult.PKCS7Documents[i] != null)
                    {
                        //Level
                        //ret.PKCS7Documents[doc_i].Level = doc.Level.ToString();
                        ret.PKCS7Documents[i].Level = fileDoc.signatureResult.PKCS7Documents[i].Level.ToString();

                        //DocumentFileName
                        //ret.PKCS7Documents[doc_i].DocumentFileName = doc.DocumentFileName;
                        ret.PKCS7Documents[i].DocumentFileName = fileDoc.signatureResult.PKCS7Documents[i].DocumentFileName;

                        //SignAlgorithm
                        //ret.PKCS7Documents[doc_i].SignAlgorithm = doc.SignAlgorithm;
                        ret.PKCS7Documents[i].SignAlgorithm = fileDoc.signatureResult.PKCS7Documents[i].SignAlgorithm;

                        //SignerInfo
                        //Istanziare l'array SignerInfo
                        ret.PKCS7Documents[i].SignersInfo = new Domain.FileDoc.SignerInfo[fileDoc.signatureResult.PKCS7Documents[i].SignersInfo.Length];
                        //foreach (SignerInfo sigInf in doc.SignersInfo)
                        for (int j = 0; j < fileDoc.signatureResult.PKCS7Documents[i].SignersInfo.Length - 1; j++)
                        {
                            //ParentSignerCertSN
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].ParentSignerCertSN = sigInf.ParentSignerCertSN;
                            ret.PKCS7Documents[i].SignersInfo[j].ParentSignerCertSN = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].ParentSignerCertSN;

                            //CertificateInfo
                            #region CertificateInfo
                            //RevocationStatus
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.RevocationStatus = sigInf.CertificateInfo.RevocationStatus.ToString();
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationStatus = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationStatus.ToString();

                            //RevocationStatusDescription
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.RevocationStatusDescription = sigInf.CertificateInfo.RevocationStatusDescription;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationStatusDescription = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationStatusDescription;

                            //SerialNumber
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.SerialNumber = sigInf.CertificateInfo.SerialNumber;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SerialNumber = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SerialNumber;

                            //SignatureAlgorithm
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.SignatureAlgorithm = sigInf.CertificateInfo.SignatureAlgorithm;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SignatureAlgorithm = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SignatureAlgorithm;

                            //ValidFromDate
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.ValidFromDate = sigInf.CertificateInfo.ValidFromDate.ToString();
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ValidFromDate = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ValidFromDate.ToString();

                            //ValidToDate
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.ValidToDate = sigInf.CertificateInfo.ValidToDate.ToString();
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ValidToDate = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ValidToDate.ToString();

                            //RevocationDate
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.RevocationDate = sigInf.CertificateInfo.RevocationDate.ToString();
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationDate = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.RevocationDate.ToString();

                            //SubjectName
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.SubjectName = sigInf.CertificateInfo.SubjectName;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SubjectName = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.SubjectName;

                            //IssuerName
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.IssuerName = sigInf.CertificateInfo.IssuerName;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.IssuerName = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.IssuerName;

                            //ThumbPrint
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].CertificateInfo.ThumbPrint = sigInf.CertificateInfo.ThumbPrint;
                            ret.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ThumbPrint = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].CertificateInfo.ThumbPrint;
                            #endregion

                            //SubjectInfo
                            #region SubjectInfo
                            //CommonName
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.CommonName = sigInf.SubjectInfo.CommonName;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CommonName = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CommonName;

                            //CertId
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.CertId = sigInf.SubjectInfo.CertId;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CertId = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CertId;

                            //CodiceFiscale
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.CodiceFiscale = sigInf.SubjectInfo.CodiceFiscale;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CodiceFiscale = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.CodiceFiscale;

                            //DataDiNascita
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.DataDiNascita = sigInf.SubjectInfo.DataDiNascita;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.DataDiNascita = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.DataDiNascita;

                            //Ruolo
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.Ruolo = sigInf.SubjectInfo.Ruolo;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Ruolo = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Ruolo;

                            //Cognome
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.Cognome = sigInf.SubjectInfo.Cognome;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Cognome = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Cognome;

                            //Nome
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.Nome = sigInf.SubjectInfo.Nome;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Nome = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Nome;

                            //Country
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.Country = sigInf.SubjectInfo.Country;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Country = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.Country;

                            //SerialNumber
                            //ret.PKCS7Documents[doc_i].SignersInfo[sigInf_i].SubjectInfo.SerialNumber = sigInf.SubjectInfo.SerialNumber;
                            ret.PKCS7Documents[i].SignersInfo[j].SubjectInfo.SerialNumber = fileDoc.signatureResult.PKCS7Documents[i].SignersInfo[j].SubjectInfo.SerialNumber;
                            #endregion

                            //sigInf_i++;
                        }

                        //doc_i++;
                    }
                }
            }

            return ret;

        }

        private static Domain.FileDoc.OutputResponseMarca convertTimestampResult(FileDocumento fileDoc)
        {
            Domain.FileDoc.OutputResponseMarca ret = new Domain.FileDoc.OutputResponseMarca();

            if (fileDoc != null)
            {
                //DescrizioneErrore
                ret.descrizioneErrore = fileDoc.timestampResult.descrizioneErrore;

                //docm - stringa che rappresenta il timestamp
                ret.docm = fileDoc.timestampResult.docm;

                //docm_date - timestamp in formato data
                ret.docm_date = fileDoc.timestampResult.docm_date;

                //dsm - data scadenza marca
                ret.dsm = fileDoc.timestampResult.dsm;

                //fromDate - data a partire dalla quale la marca è valida
                ret.fromDate = fileDoc.timestampResult.fromDate;

                //esito
                ret.esito = fileDoc.timestampResult.esito;

                //fhash - stringa esadecimale rappresentante l'hash del file al quale
                ret.fhash = fileDoc.timestampResult.fhash;

                //marca - stringa in base64 che rappresenta la marca temporale restituita dalla TSA
                ret.marca = fileDoc.timestampResult.marca;

                //sernum - numero seriale della marca temporale
                ret.sernum = fileDoc.timestampResult.sernum;

                //TSA
                #region TSA
                //TSARFC2253Name - Nome TSA in formato RFC2253
                ret.TSA.TSARFC2253Name = fileDoc.timestampResult.TSA.TSARFC2253Name;
                //CN - Nome Comune
                ret.TSA.CN = fileDoc.timestampResult.TSA.CN;
                //UO - Nome dell'unità organizzativa
                ret.TSA.OU = fileDoc.timestampResult.TSA.OU;
                //O - Nome dell'organizzazione
                ret.TSA.O = fileDoc.timestampResult.TSA.O;
                //C - Sigla della Nazione
                ret.TSA.C = fileDoc.timestampResult.TSA.C;
                #endregion

                //DecryptedTSR
                #region DecryptedTSR
                //Content - conetnuto binario della marca
                ret.DecryptedTSR.content = fileDoc.timestampResult.DecryptedTSR.content;
                //length - dimensione dell'array di byte contenente la marca
                ret.DecryptedTSR.length = fileDoc.timestampResult.DecryptedTSR.length.ToString();
                //contentType - MIMEtype del contenuto della marca
                ret.DecryptedTSR.contentType = fileDoc.timestampResult.DecryptedTSR.contentType;
                #endregion

                //TimestampedDoc - numero del documento marcato temporalmente
                ret.timestampedDoc = fileDoc.timestampResult.timestampedDoc;

                //snCertificato - numero seriale del certificato di firma
                ret.snCertificato = fileDoc.timestampResult.snCertificato;

                //algCertificato - algoritmo di firma del certificato della TSA
                ret.algCertificato = fileDoc.timestampResult.algCertificato;

                //algHash - algoritmo di hash
                ret.algHash = fileDoc.timestampResult.algHash;
            }

            return ret;

        }

        public static bool CleanRightsExtSys(string idObject, string idUtSysExt, string idSysExt)
        {
            // Cancellazione dei diritti dei ruoli supremi se il documento o il fascicolo sono creati da SysExt
            bool retval = false;
            retval = BusinessLogic.Amministrazione.SistemiEsterni.CleanRightsExtSysAfterCreation(idObject, idUtSysExt, idSysExt);

            return retval;
        }

        public static bool isRuoloAuthorized(InfoUtente infoUt, string tipologia)
        {
            bool trovato = false;
            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUt.idGruppo);
            if (ruolo == null || ruolo.funzioni == null)
                return false;
            /*-*/

            foreach (DocsPaVO.utente.Funzione f in ruolo.funzioni)
            {
                if (f.codice.Equals(tipologia))
                {
                    trovato = true;
                    break;
                }
            }

            //for (int i = 0; i < ruolo.funzioni.Count; i++)
            //{
            //    if (ruolo.funzioni[i].Equals(tipologia))
            //    {
            //        trovato = true;
            //        break;
            //    }
            //}

            return trovato;
        }

        public static int maxFileSizePermitted()
        {
            int result = -1;
            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PIS_MAXFILESIZE_MB")))
            {
                if (Int32.TryParse(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PIS_MAXFILESIZE_MB"), out result))
                {
                    result = result * 1048576;
                }

            }
            return result;
        }

        public static string CreateAuthToken(Utente utente, Ruolo ruolo)
        {
            string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(utente.userId);

            string ss_token = GetToken(utente, ruolo);
            return ss_token;
        }

        public static string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
            {
                if (rl.idGruppo == ruolo.idGruppo)
                    okRuolo = true;
            }

            if (okRuolo)
            {
                string tokenDiAutenticazione = null;
                try
                {
                    string clearToken = string.Empty;
                    clearToken += ruolo.systemId + "|";
                    clearToken += utente.idPeople + "|";
                    clearToken += ruolo.idGruppo + "|";
                    clearToken += utente.dst + "|";
                    clearToken += utente.idAmministrazione + "|";
                    clearToken += utente.userId + "|";
                    clearToken += utente.sede + "|";
                    clearToken += utente.urlWA;

                    tokenDiAutenticazione = Utils.Encrypt(clearToken);
                }
                catch (Exception e)
                {
                    //  logger.Debug("Errore durante il GetInfoUtente.", e);
                }

                tokenDiAutenticazione = "SSO=" + tokenDiAutenticazione;
                return tokenDiAutenticazione;
            }
            else
            {
                //logger.Debug("L'utente : " + utente.descrizione + " non appartiene al ruolo : " + ruolo.descrizione);
                return null;
            }
        }

        public static ArrayList estraiCorrispondenti(string oppData, DocsPaVO.utente.InfoUtente infoUtente, out string templatestring, out string corrNotFound, out string corrRagTrasm, string splitchar)
        {
            ArrayList result = new ArrayList();
            templatestring = "";
            corrNotFound = "";
            corrRagTrasm = "";
            string[] tempcorrs;

            logger.Debug("Estrazione da stringa " + oppData);
            if (!string.IsNullOrEmpty(oppData))
            {
                tempcorrs = oppData.Split('§');
                DocsPaVO.utente.Corrispondente tempCorrObj;
                string truncUndScore = "";
                foreach (string corrX in tempcorrs)
                {
                    if (corrX.Contains(splitchar))
                        truncUndScore = corrX.Substring(0, corrX.LastIndexOf(splitchar));
                    else truncUndScore = corrX;

                    if (truncUndScore.Contains("&amp;")) truncUndScore = truncUndScore.Replace("&amp;", "&");

                    tempCorrObj = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(truncUndScore, infoUtente);
                    if (tempCorrObj != null)
                    {
                        result.Add(tempCorrObj);
                        if (!string.IsNullOrEmpty(templatestring)) templatestring += Environment.NewLine + truncUndScore;
                        else templatestring = truncUndScore;

                        if (!string.IsNullOrEmpty(corrRagTrasm)) corrRagTrasm += "§" + corrX.Replace("&amp;", "&");
                        else corrRagTrasm = corrX.Replace("&amp;", "&");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(corrNotFound)) corrNotFound += "§" + corrX.Replace("&amp;", "&");
                        else corrNotFound = corrX.Replace("&amp;", "&");
                    }
                }
                if (result.Count < 1) result = null;
            }
            else { result = null; }
            return result;
        }

        public static void salvaTrasmissioni(ArrayList corrs, string codeRagTrasm, DocsPaVO.utente.InfoUtente infoUt, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            if (corrs != null && corrs.Count > 0)
            {
                int indice = 0;
                string[] ragtrasm = codeRagTrasm.Split('§');
                foreach (DocsPaVO.utente.Corrispondente corrX2 in corrs)
                {
                    //result = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(trasmissione);
                    DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
                    DocsPaVO.trasmissione.Trasmissione resultTrasm = null;
                    DocsPaVO.trasmissione.RagioneTrasmissione ragione = null;
                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
                    transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
                    // Utente mittente della trasmissione
                    transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUt.idPeople);
                    // Ruolo mittente
                    // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
                    transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUt.idCorrGlobali);

                    string generaNotifica = "1";
                    if (false)//request.Notify != null && !request.Notify)
                    {
                        transmission.NO_NOTIFY = "1";
                        generaNotifica = "0";
                    }
                    else
                    {
                        transmission.NO_NOTIFY = "0";
                        generaNotifica = "1";
                    }
                    //DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica("codiceruolodest", infoUtente);
                    ragione = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUt.idAmministrazione,
                        ragtrasm[indice]);
                    if (ragtrasm.Length > 1) indice++;
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corrX2, ragione, "", "S");
                    //Provo ad aggiungere la data di scadenza 

                    //foreach(DocsPaVO.trasmissione.TrasmissioneSingola ts in transmission.trasmissioniSingole)
                    //{
                    //    ts.dataScadenza = "DATA DI SCADENZA";
                    //}

                    resultTrasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(transmission);
                }
            }
        }

        public static void salvaTrasmUnica(ArrayList auths, ArrayList props, ArrayList reviews, DocsPaVO.utente.InfoUtente infoUt, string codeRagTrasm, DocsPaVO.fascicolazione.Fascicolo fascicolo, string notaRolesNotFound, string notaRevsNotFound)
        {
            int indice = 0;
            string[] ragtrasm = codeRagTrasm.Split('§');
            bool corrTrovati = false;
            DocsPaVO.trasmissione.Trasmissione transmission = new DocsPaVO.trasmissione.Trasmissione();
            DocsPaVO.trasmissione.Trasmissione resultTrasm = null;
            DocsPaVO.trasmissione.RagioneTrasmissione ragioneAuth = null, ragioneProp = null, ragioneRev = null;
            transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
            // Utente mittente della trasmissione
            transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUt.idPeople);
            // Ruolo mittente
            // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
            transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUt.idCorrGlobali);
            transmission.NO_NOTIFY = "0";
            transmission.noteGenerali = "APPROVAL<br/>";
            //if (!string.IsNullOrEmpty(notaRolesNotFound))
            //    transmission.noteGenerali += "The following roles were not found:<br/> " + notaRolesNotFound;

            if (auths != null && auths.Count > 0)
            {
                ragioneAuth = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUt.idAmministrazione,
                    "AUTHORIZER");
                foreach (DocsPaVO.utente.Corrispondente corr in auths)
                {
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corr, ragioneAuth, "", "S");
                }
                corrTrovati = true;
            }

            if (props != null && props.Count > 0)
            {
                ragioneProp = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUt.idAmministrazione,
                    "PROPOSER");
                foreach (DocsPaVO.utente.Corrispondente corr in props)
                {
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corr, ragioneProp, "", "S");
                }
                corrTrovati = true;
            }
            if (corrTrovati)
                resultTrasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(transmission);
            transmission = new DocsPaVO.trasmissione.Trasmissione();
            transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.FASCICOLO;
            transmission.infoFascicolo = new DocsPaVO.fascicolazione.InfoFascicolo(fascicolo);
            // Utente mittente della trasmissione
            transmission.utente = BusinessLogic.Utenti.UserManager.getUtente(infoUt.idPeople);
            // Ruolo mittente
            // Se inserito in ingresso il CodeRole utilizzo il coderole (WIP)
            transmission.ruolo = BusinessLogic.Utenti.UserManager.getRuolo(infoUt.idCorrGlobali);
            transmission.NO_NOTIFY = "0";
            transmission.noteGenerali = "REVIEW</br>";
            //if (!string.IsNullOrEmpty(notaRevsNotFound))
            //    transmission.noteGenerali = "The following roles were not found:<br/> " + notaRevsNotFound;
            corrTrovati = false;

            if (reviews != null && reviews.Count > 0)
            {
                foreach (DocsPaVO.utente.Corrispondente corr in reviews)
                {
                    ragioneRev = ragioneProp = BusinessLogic.Trasmissioni.RagioniManager.getRagioneByCodice(infoUt.idAmministrazione,
                    ragtrasm[indice]);
                    if (ragtrasm.Length > 1) indice++;
                    transmission = BusinessLogic.Trasmissioni.TrasmManager.addTrasmissioneSingola(
                        transmission, corr, ragioneRev, "", "S");
                }
                corrTrovati = true;
            }
            if (corrTrovati)
                resultTrasm = BusinessLogic.Trasmissioni.TrasmManager.saveTrasmMethod(transmission);
        }

        public static string CreateDocProceeding(InfoUtente user, string regCode, string subject, DocsPaVO.ProfilazioneDinamica.Templates template, Corrispondente corr, Ruolo role, string addressBookCode, byte[] content, Domain.File[] attachments)
        {
            logger.Debug("START");

            string docnumber = string.Empty;

            DocsPaVO.documento.SchedaDocumento doc = new DocsPaVO.documento.SchedaDocumento();
            DocsPaVO.documento.SchedaDocumento docResult = new DocsPaVO.documento.SchedaDocumento();
            doc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(user, false);

            logger.Debug("Registro");
            DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(regCode);

            logger.Debug("Popolamento SchedaDocumento");
            doc.oggetto.descrizione = subject;
            doc.oggetto.daAggiornare = true;
            doc.tipoProto = "A";
            doc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();
            string arrivalDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ((DocsPaVO.documento.ProtocolloEntrata)doc.protocollo).mittente = corr;
            ((DocsPaVO.documento.Documento)doc.documenti[0]).dataArrivo = arrivalDate;
            if (template != null)
            {
                doc.template = template;
                doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto() { systemId = template.SYSTEM_ID.ToString(), descrizione = template.DESCRIZIONE };
            }

            // -----------------------------------------------------
            logger.Debug("Salvataggio scheda");
            docResult = BusinessLogic.Documenti.DocSave.addDocGrigia(doc, user, role);

            if (docResult != null)
            {
                bool dummy;
                // fix per demo 29-08 (si spera di risolvere prima o poi)
                ((DocsPaVO.documento.Documento)docResult.documenti[0]).dataArrivo = arrivalDate;
                logger.Debug("Arrival date: " + arrivalDate);

                if (reg != null)
                    docResult.registro = reg;
                docResult.tipoProto = "A";
                docResult.mezzoSpedizione = BusinessLogic.Documenti.InfoDocManager.getIdMezzoSpedizioneByDesc("PORTALE");
                docResult.descMezzoSpedizione = "PORTALE";
                docResult.predisponiProtocollazione = true;
                docResult = BusinessLogic.Documenti.DocSave.save(user, docResult, false, out dummy, role);
            }
            else
            {
                throw new PisException("APPLICATION_ERROR");
            }

            // -----------------------------------------------------
            logger.Debug("Associazione mezzo spedizione PORTALE");
            BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(user, docResult.mezzoSpedizione, docResult.docNumber);

            // -----------------------------------------------------
            logger.Debug("Upload del file");
            DocsPaVO.documento.FileRequest fileReq = (DocsPaVO.documento.FileRequest)docResult.documenti[0];
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            //fileDoc.name = request.Proceeding.DocumentObject + ".pdf";
            //fileDoc.fullName = request.Proceeding.DocumentObject + ".pdf";
            fileDoc.name = addressBookCode + ".pdf";
            fileDoc.fullName = addressBookCode + ".pdf";
            fileDoc.content = content;
            fileDoc.contentType = "application/pdf";
            fileDoc.length = content.Length;
            fileDoc.bypassFileContentValidation = true;

            logger.Debug("Aggiunta allegati");
            if (attachments != null && attachments.Count() > 0)
            {
                logger.DebugFormat("{0} allegati", attachments.Count());
                foreach (Domain.File reqAttachment in attachments)
                {
                    DocsPaVO.documento.Allegato attachment = new DocsPaVO.documento.Allegato();
                    attachment.docNumber = docResult.docNumber;
                    attachment.descrizione = reqAttachment.Description;
                    attachment.fileName = reqAttachment.Name;
                    attachment.version = "0";
                    attachment.numeroPagine = 1;

                    DocsPaVO.documento.FileDocumento fdAttachment = new DocsPaVO.documento.FileDocumento();
                    fdAttachment.name = reqAttachment.Name;
                    fdAttachment.fullName = reqAttachment.Name;
                    fdAttachment.nomeOriginale = reqAttachment.Name;
                    fdAttachment.content = reqAttachment.Content;
                    fdAttachment.contentType = string.IsNullOrEmpty(reqAttachment.MimeType) ? "application/pdf" : reqAttachment.MimeType;
                    fdAttachment.estensioneFile = reqAttachment.Name.Split('.').Last();

                    DocsPaVO.documento.Allegato attResult = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(user, attachment);
                    if (attResult != null)
                    {
                        BusinessLogic.Documenti.FileManager.putFile(attResult, fdAttachment, user);
                    }
                }
            }

            string errorMsg = string.Empty;
            if (!BusinessLogic.Documenti.FileManager.putFile(ref fileReq, fileDoc, user, out errorMsg))
            {
                logger.DebugFormat("Errore nel salvataggio del documento! {0}", errorMsg);
                throw new PisException("FILE_CREATION_ERROR");
            }

            docnumber = docResult.docNumber;
            logger.DebugFormat("END - ID={0}", docnumber);

            return docnumber;
        }

        public static Domain.DocInProceeding GetDocInProceeding(Domain.Document doc)
        {
            Domain.DocInProceeding docInProc = new Domain.DocInProceeding();

            docInProc.Id = doc.Id;
            docInProc.DocNumber = doc.DocNumber;
            docInProc.Object = doc.Object;
            docInProc.CreationDate = doc.CreationDate;
            docInProc.DocumentType = doc.DocumentType;
            docInProc.PrivateDocument = doc.PrivateDocument;
            docInProc.PersonalDocument = doc.PersonalDocument;
            docInProc.IsAttachments = doc.IsAttachments;
            docInProc.Predisposed = doc.Predisposed;
            docInProc.Signature = doc.Signature;
            docInProc.Annulled = doc.Annulled;
            docInProc.MeansOfSending = doc.MeansOfSending;
            docInProc.InBasket = doc.InBasket;
            docInProc.ProtocolDate = doc.ProtocolDate;
            docInProc.ConsolidationState = doc.ConsolidationState;
            docInProc.ProtocolSender = doc.ProtocolSender;
            docInProc.DataProtocolSender = doc.DataProtocolSender;
            docInProc.ArrivalDate = doc.ArrivalDate;

            if (doc.Sender != null)
            {
                docInProc.Sender = doc.Sender;
            }
            if (doc.Recipients != null)
            {
                docInProc.Recipients = doc.Recipients;
            }
            if (doc.RecipientsCC != null)
            {
                docInProc.RecipientsCC = doc.RecipientsCC;
            }
            if (doc.MultipleSenders != null)
            {
                docInProc.MultipleSenders = doc.MultipleSenders;
            }
            if (doc.Template != null)
            {
                docInProc.Template = doc.Template;
            }
            if (doc.Note != null)
            {
                docInProc.Note = doc.Note;
            }
            if (doc.MainDocument != null)
            {
                docInProc.MainDocument = doc.MainDocument;
            }
            if (doc.Attachments != null)
            {
                docInProc.Attachments = doc.Attachments;
            }
            if (doc.Register != null)
            {
                docInProc.Register = doc.Register;
            }
            docInProc.IdParent = doc.IdParent;


            return docInProc;
        }

        public static Domain.FileFTPUploadInfo getDomFileFTPInfo(DocsPaVO.ExternalServices.FileFtpUpInfo input)
        {
            Domain.FileFTPUploadInfo retInfo = new Domain.FileFTPUploadInfo();
            retInfo.IdQueue = input.IdQueue;
            retInfo.IdDocument = input.IdDocument;
            retInfo.CodeAdm = input.CodeAdm;
            retInfo.Description = input.Description;
            retInfo.ErrorMessage = input.ErrorMessage;
            retInfo.Filename = input.FileName;
            retInfo.FileSize = input.FileSize;
            retInfo.FSPath = input.FSPath;
            retInfo.FTPPath = input.FTPPath;
            retInfo.HashFile = input.HashFile;
            retInfo.Status = input.Status;
            retInfo.Uploader = input.Uploader;
            retInfo.UploaderRole = input.UploaderRole;
            retInfo.Version = input.Version;
            retInfo.VersionId = input.VersionId;
            return retInfo;
        }

        public static Domain.User getUser(DocsPaVO.utente.Utente utente)
        {
            Domain.User retval = new Domain.User();
            if (utente != null)
            {
                retval.Id = utente.idPeople;
                retval.Description = utente.descrizione;
                retval.Name = utente.nome;
                retval.Surname = utente.cognome;
                retval.UserId = utente.userId;
                retval.NationalIdentificationNumber = utente.codfisc;
            }
            else retval = null;
            return retval;
        }

        public static Domain.Role getRole(DocsPaVO.utente.Ruolo ruolo)
        {
            Domain.Role retval = new Domain.Role();
            if (ruolo != null)
            {
                retval.Code = ruolo.codice;
                retval.Description = ruolo.descrizione;
                retval.Id = ruolo.idGruppo;
                if (ruolo.registri != null && ruolo.registri.Count > 0)
                {
                    Domain.Register regD = null;
                    retval.Registers = new Domain.Register[ruolo.registri.Count];
                    int i = 0;
                    foreach (DocsPaVO.utente.Registro reg in ruolo.registri)
                    {
                        regD = Utils.GetRegister(reg);
                        retval.Registers[i] = regD;
                        i++;
                    }
                }
            }
            return retval;
        }

        public static DocsPaVO.LibroFirma.PassoFirma getPassoDiFirmaFromDomain(Domain.SignBook.SignatureStep step)
        {
            DocsPaVO.LibroFirma.PassoFirma retVal = new DocsPaVO.LibroFirma.PassoFirma();
            retVal.DaAggiornare = step.ToUpdate;
            retVal.dataScadenza = step.ExpirationDate;
            retVal.Evento = getEventoLibroFirmaFromDomain(step.Event);
            retVal.idEventiDaNotificare = step.EventsToNotifyIds;
            retVal.idPasso = step.IdStep;
            retVal.idProcesso = step.IdProcess;
            retVal.Invalidated = step.Invalidated;
            retVal.IsModello = step.IsModel;
            retVal.note = step.Note;
            retVal.numeroSequenza = step.SequenceNumber;
            if (step.InvolvedRole != null) { retVal.ruoloCoinvolto = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(step.InvolvedRole.Id); }
            if (step.InvolvedUser != null) { retVal.utenteCoinvolto = BusinessLogic.Utenti.UserManager.getUtenteById(step.InvolvedUser.Id); }
            if (!string.IsNullOrEmpty(step.InvolvedRoleTypeCode) && !string.IsNullOrEmpty(step.InvolvedRoleTypeIdAmm)) { retVal.TpoRuoloCoinvolto = BusinessLogic.Amministrazione.SistemiEsterni.getTipoRuoloByCode(step.InvolvedRoleTypeIdAmm, step.InvolvedRoleTypeCode); }

            return retVal;
        }

        public static DocsPaVO.LibroFirma.Evento getEventoLibroFirmaFromDomain(Domain.SignBook.Event ev)
        {
            DocsPaVO.LibroFirma.Evento retVal = new DocsPaVO.LibroFirma.Evento();
            retVal.CodiceAzione = ev.CodeAction;
            retVal.Descrizione = ev.Description;
            retVal.Gruppo = ev.Group;
            retVal.IdEvento = ev.IdEvent;
            retVal.TipoEvento = ev.EventType;


            return retVal;
        }

        public static DocsPaVO.LibroFirma.ProcessoFirma GetProcessoFirmaFromDomain(Domain.SignBook.SignatureProcess process)
        {
            DocsPaVO.LibroFirma.ProcessoFirma retVal = new DocsPaVO.LibroFirma.ProcessoFirma();
            retVal.idPeopleAutore = process.AuthorUserId;
            retVal.idProcesso = process.IdProcess;
            retVal.idRuoloAutore = process.AuthorRoleId;
            retVal.isInvalidated = process.isInvalidated;
            retVal.IsProcessModel = process.IsProcessModel;
            retVal.nome = process.Name;
            retVal.passi = new List<DocsPaVO.LibroFirma.PassoFirma>();

            foreach (Domain.SignBook.SignatureStep step in process.Steps)
            {
                retVal.passi.Add(getPassoDiFirmaFromDomain(step));
            }

            return retVal;
        }

        public static void ExecAutoTrasmByIdStatus(InfoUtente infoUt, DocsPaVO.documento.SchedaDocumento doc, DocsPaVO.DiagrammaStato.Stato stato, string idTemplate)
        {
            try
            {
                ArrayList idModelli = BusinessLogic.DiagrammiStato.DiagrammiStato.isStatoTrasmAuto(infoUt.idAmministrazione, stato.SYSTEM_ID.ToString(), idTemplate);
                if (idModelli != null && idModelli.Count > 0)
                {

                    DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUt.idGruppo);
                    foreach ( DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model in idModelli)
                    {
                        string pathFE = "";
                        if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                            pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                        //DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = BusinessLogic.Trasmissioni.ModelliTrasmissioni.getModelloByID(infoUt.idAmministrazione, idModello);
                        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelOut = null;
                        BusinessLogic.Trasmissioni.TrasmManager.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica(infoUt, pathFE, doc, model.CODICE, ruolo, out modelOut);

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

    }
}
