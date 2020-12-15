using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Classe DataMapper per il mapping dei fascicoli del sistema documentale 
    /// in corrispondenti oggetti validi per la pubblicazione
    /// </summary>
    public class ProjectDataMapper : IDataMapper
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
            string section = string.Empty;
            try
            {
                Subscriber.Proxy.PublishedObject retValue = null;

                this._infoUtente = Security.ImpersonateUser(logInfo);
                section = "Impersonate";
                // Reperimento dati del fascicolo
                DocsPaVO.fascicolazione.Fascicolo project = this.GetProject(logInfo);
                section = "GetProject";
                if (project != null)
                {
                    // Mapping dei dati del fascicolo
                    List<Subscriber.Proxy.Property> list = new List<Subscriber.Proxy.Property>();

                    if (project.apertura != null)
                    {
                        section = "AperturaFascicolo";
                        list.Add(
                             new Subscriber.Proxy.Property
                             {
                                 Name = "AperturaFascicolo",
                                 Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                 Value = project.apertura,
                                 Hidden = true
                             }
                         );
                    }

                    if (project.chiusura != null)
                    {
                        section = "ChiusuraFascicolo";
                        list.Add(
                             new Subscriber.Proxy.Property
                             {
                                 Name = "ChiusuraFascicolo",
                                 Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                 Value = project.chiusura,
                                 Hidden = true
                             }
                         );
                    }

                    if (project.noteFascicolo != null)
                    {
                        section = "NoteFascicolo";
                        if (project.noteFascicolo.Count() > 0)
                        {
                            if (!String.IsNullOrEmpty(project.noteFascicolo.FirstOrDefault().Testo))
                            {
                                list.Add(
                                 new Subscriber.Proxy.Property
                                 {
                                     Name = "NoteFascicolo",
                                     Type = Subscriber.Proxy.PropertyTypesEnum.String,
                                     Value = project.noteFascicolo.FirstOrDefault().Testo,
                                     Hidden = true
                                 }

                                );
                            }
                        }
                    }

                    section = "CodiceFascicolo";
                    list.Add(
                        new Subscriber.Proxy.Property
                        {
                            Name = "CodiceFascicolo",
                            Type = Subscriber.Proxy.PropertyTypesEnum.String,
                            Value = project.codice,
                            Hidden = true
                        }
                    );
                    section = "DataCreazione";
                    list.Add(
                        new Subscriber.Proxy.Property
                        {
                            Name = "DataCreazione",
                            Type = Subscriber.Proxy.PropertyTypesEnum.String,
                            Value = project.dataCreazione
                        }
                    );

                    if (project.template != null)
                    {
                        section = "Template";
                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom field in project.template.ELENCO_OGGETTI)
                        {
                            this.AddProperty(field, list);
                        }
                    }

                    // Reperimento dello stato del fascicolo
                    DocsPaVO.DiagrammaStato.Stato objectState = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoFasc(project.systemID);

                    if (objectState != null)
                    {
                        section = "IDStato";
                        list.Add(
                            new Subscriber.Proxy.Property
                            {
                                Name = "IDStato",
                                Type = Subscriber.Proxy.PropertyTypesEnum.Numeric,
                                Value = objectState.SYSTEM_ID,
                                Hidden = true
                            }
                        );
                        section = "Stato";
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

                    retValue = new Subscriber.Proxy.PublishedObject
                    {
                        IdObject = project.systemID,
                        Description = string.Format("{0} - {1}", project.codice, project.descrizione),
                        ObjectType = "Fascicolo",
                        TemplateName = project.template.DESCRIZIONE,
                        Properties = list.ToArray()
                    };
                }
                section = "Finito";
                return retValue;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message + " Section: " + section);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected virtual void AddProperty(DocsPaVO.ProfilazioneDinamica.OggettoCustom field, List<Subscriber.Proxy.Property> properties)
        {
            if (field.TIPO.DESCRIZIONE_TIPO.ToUpperInvariant() == "CORRISPONDENTE")
            {
                // Id del corrispondente come campo nascosto
                properties.Add(new Subscriber.Proxy.Property
                {
                    Name = string.Format("ID:{0}", field.DESCRIZIONE),
                    Type = this.GetType(field),
                    IsTemplateProperty = true,
                    Hidden = true,
                    Value = field.VALORE_DATABASE
                });
                                
                // Reperimento dei metadati del corrispondente
                DocsPaVO.utente.Corrispondente corrispondente = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(field.VALORE_DATABASE);

                if (corrispondente != null)
                {
                    // Descrizione del corrispondente
                    properties.Add(new Subscriber.Proxy.Property
                    {
                        Name = field.DESCRIZIONE,
                        Type = this.GetType(field),
                        IsTemplateProperty = true,
                        Value = string.Format("{0} - {1}", corrispondente.codiceRubrica, corrispondente.descrizione)
                    });

                    // Mail del corrispondente
                    properties.Add(new Subscriber.Proxy.Property
                    {
                        Name = string.Format("MAIL:{0}", field.DESCRIZIONE),
                        Type = this.GetType(field),
                        IsTemplateProperty = true,
                        Hidden = true,
                        Value = corrispondente.email
                    });

                }
            }
            else
            {
                string value = string.Empty;

                Subscriber.Proxy.Property p = new Subscriber.Proxy.Property
                {
                    Name = field.DESCRIZIONE,
                    Type = this.GetType(field),
                    IsTemplateProperty = true
                };

                if (field.TIPO.DESCRIZIONE_TIPO == "CasellaDiSelezione")
                {
                    if (field.VALORI_SELEZIONATI != null && field.VALORI_SELEZIONATI.Count > 0)
                    {
                        foreach (string val in field.VALORI_SELEZIONATI)
                        {
                            if (!string.IsNullOrEmpty(value))
                                value += "|";

                            value += val;
                        }
                    }
                }
                else if (field.TIPO.DESCRIZIONE_TIPO == "Contatore")
                {
                    value = field.FORMATO_CONTATORE;

                    if (!string.IsNullOrEmpty(field.VALORE_DATABASE))
                    {
                        value = value.Replace("ANNO", field.ANNO);
                        value = value.Replace("CONTATORE", field.VALORE_DATABASE);

                        value = value.Replace("COD_AMM", BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(this._infoUtente.idAmministrazione).Codice);
                        value = value.Replace("COD_UO", field.CODICE_DB);

                        if (!string.IsNullOrEmpty(field.DATA_INSERIMENTO))
                        {
                            int fine = field.DATA_INSERIMENTO.LastIndexOf(".");

                            if (fine > -1)
                                value = value.Replace("gg/mm/aaaa hh:mm", field.DATA_INSERIMENTO.Substring(0, fine));

                            value = value.Replace("gg/mm/aaaa", field.DATA_INSERIMENTO.Substring(0, 10));
                        }

                        if (!string.IsNullOrEmpty(field.ID_AOO_RF) && field.ID_AOO_RF != "0")
                        {
                            DocsPaVO.utente.Registro registro = BusinessLogic.Utenti.RegistriManager.getRegistro(field.ID_AOO_RF);

                            if (registro != null)
                            {
                                value = value.Replace("RF", registro.codRegistro);
                                value = value.Replace("AOO", registro.codRegistro);
                            }
                        }
                    }
                }
                else
                {
                    value = field.VALORE_DATABASE;
                }

                p.Value = value;

                properties.Add(p);
            }
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
        /// Dictionary per effettuare il caching degli oggetti fascicolo reperiti
        /// </summary>
        //private Dictionary<int, DocsPaVO.fascicolazione.Fascicolo> _state = null;

        /// <summary>
        /// Reperimento fascicolo
        /// </summary>
        /// <returns></returns>
        protected virtual DocsPaVO.fascicolazione.Fascicolo GetProject(LogInfo logInfo)
        {
            //if (this._state == null)
            //    this._state = new Dictionary<int, DocsPaVO.fascicolazione.Fascicolo>();

            //if (this._state.ContainsKey(logInfo.ObjectId))
            //{
            //    return (DocsPaVO.fascicolazione.Fascicolo)this._state[logInfo.ObjectId];
            //}
            //else
            //{
                DocsPaVO.fascicolazione.Fascicolo project = ProjectDataAdapter.GetProject(logInfo);

                //lock (_state)
                //{
                //    this._state[logInfo.ObjectId] = project;
                //}

                return project;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        protected class ProjectDataAdapter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static DocsPaVO.fascicolazione.Fascicolo GetProject(LogInfo logInfo)
            {
                DocsPaVO.utente.InfoUtente userInfo = Security.ImpersonateUser(logInfo);

                DocsPaVO.fascicolazione.Fascicolo project = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(logInfo.ObjectId.ToString(), userInfo);

                if (project != null && project.template == null)
                {
                    project.template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascDettagli(project.systemID);
                }

                return project;
            }
        }

        #endregion
    }
}