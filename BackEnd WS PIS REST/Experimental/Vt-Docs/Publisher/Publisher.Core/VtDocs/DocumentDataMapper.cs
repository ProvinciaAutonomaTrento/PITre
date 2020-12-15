using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Classe DataMapper per il mapping dei documenti del sistema documentale 
    /// in corrispondenti oggetti validi per la pubblicazione
    /// </summary>
    public class DocumentDataMapper : IDataMapper
    {
        #region Public Members

        /// <summary>
        /// Mapping dell'oggetto
        /// </summary>
        /// <param name="logInfo">
        /// Dati del log nel sistema documentale
        /// </param>
        /// <param name="ev">
        /// Dati dell'evento generato
        /// </param>
        /// <returns>
        /// Oggetto dal pubblicare
        /// </returns>
        public virtual Subscriber.Proxy.PublishedObject Map(VtDocs.LogInfo logInfo, EventInfo ev)
        {
            try
            {
                Subscriber.Proxy.PublishedObject retValue = null;

                // Reperimento dati del documento
                DocsPaVO.documento.SchedaDocumento document = this.GetDocument(logInfo);

                if (document != null)
                {
                    // Mapping dei dati del fascicolo
                    List<Subscriber.Proxy.Property> list = new List<Subscriber.Proxy.Property>();

                    if (document.template != null)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom field in document.template.ELENCO_OGGETTI)
                        {
                            Subscriber.Proxy.Property p = this.MapProperty(field);

                            if (p != null)
                                list.Add(p);
                        }
                    }

                    // Reperimento dello stato del fascicolo
                    DocsPaVO.DiagrammaStato.Stato objectState = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(document.systemId);

                    if (objectState != null)
                    {
                        list.Add(
                            new Subscriber.Proxy.Property
                            {
                                Name = "IDStato",
                                Type = Subscriber.Proxy.PropertyTypesEnum.Numeric,
                                Value = objectState.SYSTEM_ID,
                                Hidden = true
                            }
                        );

                        list.Add(
                            new Subscriber.Proxy.Property
                            {
                                Name = "Stato",
                                Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                Value = objectState.DESCRIZIONE,
                                Hidden = true
                            }
                        );
                    }

                    if (ev.LoadFileIfDocumentType)
                    {
                        // Caricamento ultima versione del documento
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest) document.documenti[0];
                        if (fr.subVersion != "!")
                        {
                            DocsPaVO.documento.FileDocumento file = BusinessLogic.Documenti.FileManager.getFile(fr, Security.ImpersonateUser(logInfo));

                            if (file != null)
                            {
                                list.Add(new Subscriber.Proxy.Property
                                {
                                    Name = "FileName",
                                    Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                    Value = file.name,
                                    Hidden = true
                                });

                                list.Add(new Subscriber.Proxy.Property
                                {
                                    Name = "File",
                                    Type = Subscriber.Proxy.PropertyTypesEnum.BinaryContent,
                                    BinaryValue = file.content,
                                    Hidden = true
                                });

                                list.Add(new Subscriber.Proxy.Property
                                {
                                    Name = "FilePrintThumb",
                                    Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                    Value = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(file.content),
                                    Hidden = true
                                });
                            }
                        }
                        //Gestione Allegati
                        if (document.allegati != null)
                        {
                            int attNumber = 0;
                            foreach (DocsPaVO.documento.Allegato allegato in document.allegati)
                            {
                                if (allegato.subVersion != "!")
                                {
                                    DocsPaVO.documento.FileDocumento file = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)allegato, Security.ImpersonateUser(logInfo));
                                    list.Add(new Subscriber.Proxy.Property
                                    {
                                        Name = String.Format("AttachmentName_{0}", attNumber),
                                        Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                        Value = file.name,
                                        Hidden = true
                                    });

                                    list.Add(new Subscriber.Proxy.Property
                                    {
                                        Name = String.Format("Attachment_{0}", attNumber),
                                        Type = Subscriber.Proxy.PropertyTypesEnum.BinaryContent,
                                        BinaryValue = file.content,
                                        Hidden = true
                                    });

                                    list.Add(new Subscriber.Proxy.Property
                                    {
                                        Name = String.Format("AttachmentPrintThumb_{0}", attNumber),
                                        Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                        Value = DocsPaUtils.Security.CryptographyManager.CalcolaImpronta(file.content),
                                        Hidden = true
                                    });
                                    attNumber++;
                                }
                            }
                        }
                    }

                    /* user information that generated the event */
                    list.Add(new Subscriber.Proxy.Property
                    {
                        Name = "UserName", //UserName utente che ha generato la pubblicazione
                        Type = Subscriber.Proxy.PropertyTypesEnum.String,
                        Value = logInfo.UserName,
                        Hidden = true
                    });
                    list.Add(new Subscriber.Proxy.Property
                    {
                        Name = "CodeAdm", // codice amministrazione
                        Type = Subscriber.Proxy.PropertyTypesEnum.String,
                        Value = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(logInfo.IdAdmin.ToString()).Codice,
                        Hidden = true
                    });
                    list.Add(new Subscriber.Proxy.Property
                    {
                        Name = "RoleCode", //codice ruolo
                        Type = Subscriber.Proxy.PropertyTypesEnum.String,
                        Value = logInfo.RoleCode,
                        Hidden = true
                    });

                    /* end user information*/

                    string docName = string.Empty;

                    if (document.tipoProto == "G")
                        docName = string.Format("ID: {0}", document.docNumber);
                    else if (document.protocollo != null)
                        docName = string.Format("Segnatura: {0}", document.protocollo.segnatura);

                    //document.protocollo.segnatura
                    retValue = new Subscriber.Proxy.PublishedObject
                    {
                        IdObject = document.systemId,
                        Description = docName,
                        ObjectType = "Documento",
                        TemplateName = document.template.DESCRIZIONE,
                        Properties = list.ToArray()
                    };
                }

                return retValue;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }
        }
               
        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected virtual Subscriber.Proxy.Property MapProperty(DocsPaVO.ProfilazioneDinamica.OggettoCustom field)
        {
            Subscriber.Proxy.Property p = null;

            if (field.TIPO.DESCRIZIONE_TIPO.ToUpperInvariant() == "CORRISPONDENTE")
            {
                // Id del corrispondente come campo nascosto
                p = new Subscriber.Proxy.Property
                {
                    Name = string.Format("ID:{0}", field.DESCRIZIONE),
                    Type = this.GetType(field),
                    IsTemplateProperty = true,
                    Hidden = true,
                    Value = field.VALORE_DATABASE
                };

                // Reperimento dei metadati del corrispondente
                DocsPaVO.utente.Corrispondente corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(field.VALORE_DATABASE);

                if (corrispondente != null)
                {
                    // Descrizione del corrispondente
                    p = new Subscriber.Proxy.Property
                    {
                        Name = field.DESCRIZIONE,
                        Type = this.GetType(field),
                        IsTemplateProperty = true,
                        Value = string.Format("{0} - {1}", corrispondente.codiceRubrica, corrispondente.descrizione)
                    };

                    // Mail del corrispondente
                    p = new Subscriber.Proxy.Property
                    {
                        Name = string.Format("MAIL:{0}", field.DESCRIZIONE),
                        Type = this.GetType(field),
                        IsTemplateProperty = true,
                        Hidden = true,
                        //Value = "stefano.frezza@valueteam.com"
                        Value = corrispondente.email
                    };

                }
            }
            else
            {
                p = new Subscriber.Proxy.Property
                {
                    Name = field.DESCRIZIONE,
                    Type = this.GetType(field),
                    IsTemplateProperty = true,
                    Value = field.VALORE_DATABASE
                };
            }

            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private Subscriber.Proxy.PropertyTypesEnum GetType(DocsPaVO.ProfilazioneDinamica.OggettoCustom field)
        {
            if (field.TIPO.DESCRIZIONE_TIPO.ToUpperInvariant() == "DATA")
                return Subscriber.Proxy.PropertyTypesEnum.Date;
            else if (field.TIPO.DESCRIZIONE_TIPO.ToUpperInvariant() == "CONTATORE")
                return Subscriber.Proxy.PropertyTypesEnum.Numeric;
            else
                return Subscriber.Proxy.PropertyTypesEnum.String;
        }

        /// <summary>
        /// Dictionary per effettuare il caching degli oggetti documenti reperiti
        /// </summary>
        //private Dictionary<int, DocsPaVO.documento.SchedaDocumento> _state = null;

        /// <summary>
        /// Reperimento documento
        /// </summary>
        /// <returns></returns>
        protected virtual DocsPaVO.documento.SchedaDocumento GetDocument(LogInfo logInfo)
        {
            //if (this._state == null)
            //    this._state = new Dictionary<int, DocsPaVO.documento.SchedaDocumento>();

            //if (this._state.ContainsKey(logInfo.ObjectId))
            //{
            //    return (DocsPaVO.documento.SchedaDocumento)this._state[logInfo.ObjectId];
            //}
            //else
            //{
                DocsPaVO.documento.SchedaDocumento document = DocumentDataAdapter.GetDocument(logInfo);

                //lock (_state)
                //{
                //    this._state[logInfo.ObjectId] = document;
                //}

                return document;
            //}
        }
        
        /// <summary>
        /// 
        /// </summary>
        private sealed class DocumentDataAdapter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static DocsPaVO.documento.SchedaDocumento GetDocument(LogInfo logInfo)
            {
                DocsPaVO.utente.InfoUtente userInfo = Security.ImpersonateUser(logInfo);

                DocsPaVO.documento.SchedaDocumento document = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, logInfo.ObjectId.ToString());

                if (document != null && document.template == null)
                {
                    document.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplate(document.systemId);
                }

                return document;
            }
        }

        #endregion
    }
}
