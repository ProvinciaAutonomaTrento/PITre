// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DatiCert;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.AggiornaStatusMask;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Daticert
{
    internal class DaticertHandler : IRequestHandler<DatiCertRequest, DaticertResult>
    {
        #region Public members
        public DaticertHandler(ILogger<DaticertHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }
        public async Task<DaticertResult> Handle(DatiCertRequest request, CancellationToken cancellationToken)
        {

            var output = new ProcessorOutput();
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var email = request.email;
            DocsPaVO.DatiCert.Daticert datiCert = new();
            string err = string.Empty;
            bool success = true;
            int? processedAttachments = 0;
            try
            {
                var datiCertAtt = email.Attachments.Where(a => "daticert.xml".Equals(a.FileName)).FirstOrDefault();
                XmlDocument Xmlfile = new XmlDocument();
                InteropResolver my = new InteropResolver();
                XmlTextReader xtr = new XmlTextReader(new MemoryStream(datiCertAtt?.Content));
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                xvr.ValidationType = System.Xml.ValidationType.DTD;
                xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                xvr.XmlResolver = my;

                try
                {

                    Xmlfile.Load(xvr);
                }
                catch (System.Xml.Schema.XmlSchemaException e)
                {
                    await InsDpaMailElab(request.message.Id,"D");
                    this._logger.LogError(ErrorDescription.XmlDatiCertInvalid + e.Message);
                    err = ErrorDescription.XmlDatiCertInvalid;
                }

                XmlElement nodeRoot = Xmlfile.DocumentElement;
                try
                {
                    datiCert.tipoRicevutaIntestazione = nodeRoot.GetAttributeNode("tipo").Value;
                    datiCert.erroreRicevuta = nodeRoot.GetAttributeNode("errore").Value;
                }
                catch (Exception ex)
                {
                    xvr.Close();
                    xtr.Close();
                    this._logger.LogError(ErrorDescription.XmlDatiCertReading + ex.Message);
                    err = ErrorDescription.XmlDatiCertReading;
                }


                try
                {
                    XmlNodeList xmlIntestazione = nodeRoot.SelectNodes("intestazione");
                    int indice = 0;
                    int massimo = xmlIntestazione.Item(0).ChildNodes.Count - 1;
                    datiCert.mittente = xmlIntestazione.Item(0).ChildNodes[indice].InnerText;
                    indice++;
                    bool ok = false;
                    List<string> destinatario = new List<string>();
                    List<string> tipoDestinatario = new List<string>();
                    while (!ok)
                    {
                        if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("destinatari".ToLower()))
                        {
                            tipoDestinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value);
                            destinatario.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                            indice++;
                        }
                        else
                            break;
                    }

                    datiCert.destinatarioLst = destinatario.ToArray();
                    datiCert.tipoDestinatarioLst = tipoDestinatario.ToArray();

                    List<string> risposte = new List<string>();
                    while (indice < massimo)
                    {
                        if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("risposte".ToLower()))
                        {
                            risposte.Add(xmlIntestazione.Item(0).ChildNodes[indice].InnerText);
                            indice++;
                        }
                    }
                    datiCert.risposteLst = risposte.ToArray();


                    if (xmlIntestazione.Item(0).ChildNodes[indice].Name.ToLower().Equals("oggetto".ToLower()))
                    {
                        datiCert.oggetto = xmlIntestazione.Item(0).ChildNodes[indice].InnerText.Replace("'", "'''");
                    }

                    XmlNodeList xmlDati = nodeRoot.SelectNodes("dati");
                    indice = 0;
                    massimo = xmlDati.Item(0).ChildNodes.Count;

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("gestore-emittente".ToLower()))
                        {
                            datiCert.gestioneEmittente = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("data".ToLower()))
                        {
                            datiCert.zona = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("zona").Value;
                            datiCert.giorno = xmlDati.Item(0).ChildNodes[indice].SelectNodes("giorno").Item(0).InnerText;
                            datiCert.ora = xmlDati.Item(0).ChildNodes[indice].SelectNodes("ora").Item(0).InnerText;
                            indice++;
                        }
                    }

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("identificativo".ToLower()))
                        {
                            datiCert.identificativo = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }
                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("msgid".ToLower()))
                        {
                            datiCert.msgid = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricevuta".ToLower()))
                        {
                            datiCert.ricevuta = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            datiCert.tipoRicevuta = xmlDati.Item(0).ChildNodes[indice].Attributes.GetNamedItem("tipo").Value;
                            indice++;
                        }

                    }


                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("consegna".ToLower()))
                        {
                            datiCert.consegna = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("ricezione".ToLower()))
                        {
                            datiCert.ricezione = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }

                    if (indice < massimo)
                    {
                        if (xmlDati.Item(0).ChildNodes[indice].Name.ToLower().Equals("errore-esteso".ToLower()))
                        {
                            datiCert.errore_esteso = xmlDati.Item(0).ChildNodes[indice].InnerText;
                            indice++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ErrorDescription.XmlDatiCertReading + ex.Message);
                    err = ErrorDescription.XmlDatiCertReading;
                }
                finally
                {
                    xvr.Close();
                    xtr.Close();
                }

                if (!string.IsNullOrEmpty(err))
                {
                    output.Success = false;
                    output.ErrorMessage = err;
                    return new(output);
                }

                try
                {
                    var subjSeg = datiCert.oggetto.Split('#');

                    if (subjSeg.Length == 2)
                    {
                        datiCert.oggetto = subjSeg[0];
                        datiCert.docnumber = subjSeg[1];
                    }
                    else if (subjSeg.Length > 2)
                    {
                        datiCert.oggetto = subjSeg[0];
                        datiCert.docnumber = subjSeg[subjSeg.Length - 2];
                    }
                    datiCert.docnumber = CleanDocNumber(datiCert.docnumber);
                }
                catch(Exception ex)
                {
                    this._logger.LogError(ErrorDescription.XmlCantFindDocNumber + ex.Message);
                }

                if(string.IsNullOrEmpty(datiCert.docnumber))
                {
                    output.Success = false;
                    output.ErrorMessage = ErrorDescription.RiferimentoAlDocumentoNonTrovato;
                    if (datiCert.tipoRicevutaIntestazione.Equals("posta-certificata"))
                        output.ErrorMessage = ErrorDescription.MailInoltrata;
                    return new(output);
                }

                var tipoNotifica = await this.RicercaTipoNotificaByCodice(datiCert.tipoRicevutaIntestazione);
                string idAllegato = string.Empty;
                string systemIdTipoNotifica = string.Empty;


                if (tipoNotifica != null &&
                    !string.IsNullOrEmpty(tipoNotifica.idTipoNotifica))
                    systemIdTipoNotifica = tipoNotifica.idTipoNotifica;

                if (!string.IsNullOrEmpty(datiCert.docnumber))
                {
                    string destinatario = (datiCert.destinatarioLst != null && datiCert.destinatarioLst.Count() > 0) ? datiCert.destinatarioLst[0] : string.Empty;

                    if (!await this.VerificaPresenzaNotifica(datiCert, systemIdTipoNotifica))
                    {
                        string nomeAllegato = string.Empty;
                        DocsPaVO.documento.Allegato all = null;

                        try
                        {
                            nomeAllegato = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml"; 

                            all = new DocsPaVO.documento.Allegato();
                            all.docNumber = datiCert.docnumber;
                            all.fileName = nomeAllegato;
                            all.version = "0";
                            all.numeroPagine = 1;
                            all.TypeAttachment = 2;
                            all.descrizione = MessageDescription.RicevutaRitorno + datiCert.tipoRicevutaIntestazione + " - " + destinatario;
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(exception: ex, message: string.Format(ErrorDescription.AttCreationError,ex.Message));
                            throw ex;
                        }

                        try
                        {
                            await this._mediator.Send(new Application.Requests.DocumentoAggiungiAllegato(new(),all));
                            idAllegato = all.versionId;
                            await this.SetFlagAllegatiPecIsExt(all.versionId,all.docNumber,"P");
                        }
                        catch(Exception ex)
                        {
                            err = string.Format(ErrorDescription.CantAddAttachment, email.Subject.Value);
                            this._logger.LogError(exception:ex, message: ex.Message);
                            xvr.Close();
                            xtr.Close();
                            throw ex;
                        }

                        DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)all;

                        if (!await EsistiExt(nomeAllegato))
                            await this.InsertExtNonGen(nomeAllegato, "application/octet-stream");

                        DocsPaVO.documento.FileDocumento fdAll = new();

                        try
                        {
                            fdAll = new DocsPaVO.documento.FileDocumento();
                            fdAll.content = email.BinaryContent;
                            fdAll.length = fdAll.content.Length;
                            fdAll.name = nomeAllegato;
                        }
                        catch(Exception ex)
                        {
                            this._logger.LogError(exception: ex, message: ex.Message);
                            throw ex;
                        }

                        try
                        {
                            await this._mediator.Send(new Application.Requests.DocumentoPutFile(fRAll, fdAll, new()));
                        }
                        catch (Exception ex)
                        {
                            err = string.Format(ErrorDescription.CantPullAttachmentFile,nomeAllegato);
                            this._logger.LogError(exception:ex,message: ex.Message);
                            await this._mediator.Send(new Application.Requests.DocumentoRimuoviAllegato(all, new(), new DocsPaVO.documento.SchedaDocumento() { docNumber = datiCert.docnumber }));
                        }
                        finally
                        {
                            xvr.Close();
                            xtr.Close();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(err))
                {
                    output.Success = false;
                    output.ErrorMessage = err;
                    return new(output);
                }

                if (string.IsNullOrEmpty(systemIdTipoNotifica))
                {
                    await this.InserimentoTipoNotifica(datiCert.tipoRicevutaIntestazione);
                    tipoNotifica = await this.RicercaTipoNotificaByCodice(datiCert.tipoRicevutaIntestazione);
                    if (tipoNotifica != null && !string.IsNullOrEmpty(tipoNotifica.idTipoNotifica))
                    {
                        systemIdTipoNotifica = tipoNotifica.idTipoNotifica;
                    }
                    else
                    {
                        err = ErrorDescription.CantSaveNot;
                        output.Success = false;
                        output.ErrorMessage = err;
                        return new(output);
                    }
                }

                for (int i = 0; i < datiCert.destinatarioLst.Length; i++)
                {
                    if (!await this.InserimentoNotifica(datiCert, systemIdTipoNotifica, i, idAllegato))
                    {
                        await this.DeleteNotifica(datiCert.docnumber);

                        err = ErrorDescription.ErrDatiCert;
                        output.Success = false;
                        output.ErrorMessage = err;
                        return new(output);
                    }
                    else
                    {
                        if (tipoNotifica.codiceNotifica == "errore" || tipoNotifica.codiceNotifica == "DNS" || tipoNotifica.codiceNotifica == "non-accettazione" || tipoNotifica.codiceNotifica == "errore-consegna" || tipoNotifica.codiceNotifica == "preavviso-errore-consegna")
                        {
                            DocsPaVO.DatiCert.Notifica not1 = new DocsPaVO.DatiCert.Notifica();
                            not1 = (DocsPaVO.DatiCert.Notifica)datiCert;

                            await this._webMethodLoggerService.LogOK("NO_DELIVERY_SEND_PEC", not1.docnumber,
                                    string.Format(MessageDescription.NoDeliverySendPec, tipoNotifica.descrizioneNotifica, not1.data_ora, not1.destinatario,
                                        not1.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : not1.tipoDestinatario, not1.identificativo));
                        }

                        var notificaArr = await this.RicercaNotifiche(datiCert.docnumber);
                        for (int j = 0; j < notificaArr.Length; j++)
                        {
                            await this._mediator.Send(new AggiornaStatusMaskRequest(notificaArr[j]));
                        }
                    }
                }

                if (datiCert.risposteLst.Length > 1)
                {
                    var notificaArr = await this.RicercaNotifiche(datiCert.docnumber);

                    for (int i = 0; i < datiCert.risposteLst.Length; i++)
                    {
                        for (int j = 0; j < notificaArr.Length; j++)
                        {
                            if (!await this.InserimentoNotifica(notificaArr[i], idAllegato))
                            {
                                err = ErrorDescription.ErrInsNot;
                            }
                            else
                            {
                                await this._mediator.Send(new AggiornaStatusMaskRequest(notificaArr[j]));

                                if (tipoNotifica.codiceNotifica == "errore" || tipoNotifica.codiceNotifica == "DNS" || tipoNotifica.codiceNotifica == "non-accettazione" || tipoNotifica.codiceNotifica == "errore-consegna" || tipoNotifica.codiceNotifica == "preavviso-errore-consegna")
                                {
                                    DocsPaVO.DatiCert.Notifica not1 = new DocsPaVO.DatiCert.Notifica();
                                    not1 = (DocsPaVO.DatiCert.Notifica)datiCert;

                                    await this._webMethodLoggerService.LogOK("NO_DELIVERY_SEND_PEC", not1.docnumber,
                                            string.Format(MessageDescription.NoDeliverySendPec, tipoNotifica.descrizioneNotifica, not1.data_ora, not1.destinatario,
                                                not1.tipoDestinatario.ToUpper().Equals("ESTERNO") ? "MAIL NON CERTIFICATA" : not1.tipoDestinatario, not1.identificativo));
                                }
                            }
                        }
                    }
                }
                output.Success = success;
                if (datiCert != null)
                {
                    output.DocNumber = datiCert.docnumber != null ? datiCert.docnumber.AsLong() : default;
                    output.Success = true;
                    output.ProcessedAttachments = ++processedAttachments;
                }
                output.ErrorMessage = err;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
                err = ex.Message;
            }

            output.ProcessedAttachments = processedAttachments;
            output.ErrorMessage = err;
            output.Success = string.IsNullOrEmpty(err);

            return new DaticertResult(output);
        }

        #endregion

        #region Private members
        protected readonly ILogger<DaticertHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        private async Task<bool> InserimentoNotifica(Notifica notifica, string idAllegato)
        {

            NotificaEntity? notificaEntity = null;
            bool output = false;

            notificaEntity = new()
            {
                ID_TIPO_NOTIFICA = notifica.idTipoNotifica != null ? notifica.idTipoNotifica.AsLong() : default,
                DOCNUMBER = notifica.docnumber.AsLong(),
                VAR_MITTENTE = notifica.mittente.Replace("'", "''"),
                VAR_TIPO_DESTINATARIO = notifica.tipoDestinatario,
                VAR_DESTINATARIO = notifica.destinatario.Replace("'", "''"),
                VAR_RISPOSTE = notifica.risposte.Replace("'", "''"),
                VAR_OGGETTO = notifica.oggetto.Replace("'", "''"),
                VAR_GESTIONE_EMITTENTE = notifica.gestioneEmittente,
                VAR_ZONA = notifica.zona,
                VAR_GIORNO_ORA = notifica.data_ora.AsDateTime(),
                VAR_IDENTIFICATIVO = notifica.identificativo,
                VAR_MSGID = notifica.msgid,
                VAR_TIPO_RICEVUTA = notifica.tipoRicevuta,
                VAR_CONSEGNA = notifica.consegna,
                VAR_RICEZIONE = notifica.ricezione,
            };

            if (!string.IsNullOrEmpty(notifica.errore_esteso))
            {
                notificaEntity.VAR_ERRORE_ESTESO = notifica.errore_esteso.Replace("'", "''");
            }
            else
            {
                notificaEntity.VAR_ERRORE_ESTESO = string.Empty;
            }

            if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
            {
                notificaEntity.VAR_ERRORE_RICEVUTA = notifica.erroreRicevuta.Replace("'", "''");
            }
            else
            {
                notificaEntity.VAR_ERRORE_RICEVUTA = string.Empty;
            }

            if (!string.IsNullOrEmpty(idAllegato))
                notificaEntity.VERSION_ID = idAllegato.AsLong();
            else
            {
                notificaEntity.VERSION_ID = null;
            }
            this._dbContext.NotificaEntities.Add(notificaEntity);

            try
            {
                var nC = await ((DbContext)this._dbContext).SaveChangesAsync();
                output = nC > 0;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }


            return output;
        }


        private async Task<DocsPaVO.DatiCert.Notifica[]> RicercaNotifiche(string docnumber)
        {
            List<DocsPaVO.DatiCert.Notifica> notifiche = new();

            if (!string.IsNullOrEmpty(docnumber))
            {
                var notEntities = await this._dbContext.NotificaEntities.AsNoTracking().Where(n => n.DOCNUMBER == docnumber.AsLong()).ToListAsync();

                notEntities.ForEach(n =>
                {
                    notifiche.Add(new()
                    {
                        idNotifica = n.SYSTEM_ID.ToString(),
                        mittente = n.VAR_MITTENTE,
                        tipoDestinatario = n.VAR_TIPO_DESTINATARIO,
                        destinatario = n.VAR_DESTINATARIO,
                        risposte = n.VAR_RISPOSTE,
                        oggetto = n.VAR_OGGETTO,
                        gestioneEmittente = n.VAR_GESTIONE_EMITTENTE,
                        zona = n.VAR_ZONA,
                        data_ora = n.VAR_GIORNO_ORA != null ? n.VAR_GIORNO_ORA.AsDateTimeFormat() : null,
                        identificativo = n.VAR_IDENTIFICATIVO,
                        msgid = n.VAR_MSGID,
                        tipoRicevuta = n.VAR_TIPO_RICEVUTA,
                        consegna = n.VAR_CONSEGNA,
                        ricezione = n.VAR_RICEZIONE,
                        errore_esteso = n.VAR_ERRORE_ESTESO,
                        docnumber = n.DOCNUMBER != null ? n.DOCNUMBER.ToString() : null,
                        idTipoNotifica = n.ID_TIPO_NOTIFICA != null ? n.ID_TIPO_NOTIFICA.ToString() : null,
                        erroreRicevuta = n.VAR_ERRORE_RICEVUTA
                    });

                });
            }
            return notifiche.ToArray();
        }


        private async Task DeleteNotifica(string docnumber)
        {
            NotificaEntity? notToRemove = null;
            try
            {
                if (!string.IsNullOrEmpty(docnumber))
                    await this._dbContext.NotificaEntities.FirstOrDefaultAsync(n => n.DOCNUMBER == docnumber.AsLong());
                if (notToRemove != null)
                {
                    this._dbContext.NotificaEntities.Remove(notToRemove);
                    var nC = await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }

        }

        private async Task<bool> InserimentoNotifica(DocsPaVO.DatiCert.Daticert daticert, string systemIdTipoNotifica, int indiceDestinatari, string idAllegato)
        {
            bool output = false;
            if (daticert == null)
                return output;

            Notifica notifica = new Notifica();
            notifica = (Notifica)daticert;



            if (!string.IsNullOrEmpty(systemIdTipoNotifica))
                notifica.idTipoNotifica = systemIdTipoNotifica;
            else
                return output;


            if (daticert.destinatarioLst.Length > 0 &&
                daticert.tipoDestinatarioLst.Length > 0)
            {
                notifica.destinatario = daticert.destinatarioLst[indiceDestinatari];
                notifica.tipoDestinatario = daticert.tipoDestinatarioLst[indiceDestinatari];
            }
            else
                return output;

            if (!string.IsNullOrEmpty(daticert.giorno) &&
                !string.IsNullOrEmpty(daticert.ora))
                notifica.data_ora = daticert.giorno + " " + daticert.ora;
            else
                return output;

            if (daticert.risposteLst != null &&
               daticert.risposteLst.Length > 0)
                notifica.risposte = daticert.risposteLst[0];
            else
                return output;



            NotificaEntity notToInsert = new()
            {
                ID_TIPO_NOTIFICA = notifica.idTipoNotifica != null ? notifica.idTipoNotifica.AsLong() : default,
                DOCNUMBER = notifica.docnumber.AsLong(),
                VAR_MITTENTE = notifica.mittente.Replace("'","''"),
                VAR_TIPO_DESTINATARIO = notifica.tipoDestinatario,
                VAR_DESTINATARIO = notifica.destinatario.Replace("'","''"),
                VAR_RISPOSTE = notifica.risposte.Replace("'", "''"),
                VAR_OGGETTO = notifica.oggetto.Replace("'", "''"),
                VAR_GESTIONE_EMITTENTE = notifica.gestioneEmittente,
                VAR_ZONA = notifica.zona,
                VAR_GIORNO_ORA = notifica.data_ora.AsDateTime(),
                VAR_IDENTIFICATIVO = notifica.identificativo,
                VAR_MSGID = notifica.msgid,
                VAR_TIPO_RICEVUTA = notifica.tipoRicevuta,
                VAR_CONSEGNA = notifica.consegna,
                VAR_RICEZIONE = notifica.ricezione,

            };

            if (!string.IsNullOrEmpty(notifica.errore_esteso))
            {
                notToInsert.VAR_ERRORE_ESTESO = notifica.errore_esteso.Replace("'", "''");
            }
            else
            {
                notToInsert.VAR_ERRORE_ESTESO = string.Empty;
            }

            if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
            {
                notToInsert.VAR_ERRORE_RICEVUTA = notifica.erroreRicevuta.Replace("'", "''");
            }
            else
            {
                notToInsert.VAR_ERRORE_RICEVUTA = string.Empty;
            }

            if (!string.IsNullOrEmpty(idAllegato))
                notToInsert.VERSION_ID = idAllegato.AsLong();
            else
            {
                notToInsert.VERSION_ID = null;
            }
            this._dbContext.NotificaEntities.Add(notToInsert);


            try
            {
                var nC = await ((DbContext)this._dbContext).SaveChangesAsync();
                output = nC > 0;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
            }

            return output;
        }



        


        private async Task<bool> InserimentoTipoNotifica(string codiceNotifica)
        {
            bool output = false;

            TipoNotificaEntity tipoNotifica = new()
            {
                VAR_CODICE_NOTIFICA = codiceNotifica,
                VAR_DESCRIZIONE = MessageDescription.DescNotifica + codiceNotifica
            };

            this._dbContext.TipoNotificaEntities.Add(tipoNotifica);
            var mCount = await ((DbContext)this._dbContext).SaveChangesAsync();
            output = mCount > 0;

            return output;
        }

        private async Task<bool> SetFlagAllegatiPecIsExt(string versionId, string docNumber, string flagChar)
        {
            bool output = false;

            var ver = await this._dbContext.VersionEntities.Where(v => v.VERSION_ID == versionId.AsLong() && v.DOCNUMBER == docNumber.AsLong()).ToListAsync();


            ver.ForEach( vi => vi.CHA_ALLEGATI_ESTERNO = flagChar);

            var mCount = await ((DbContext)this._dbContext).SaveChangesAsync();

            if (mCount > 0)
                output = true;

            return output;

        }


        private async Task<bool> EsistiExt(string filename)
        {
            bool output = false;

            if (!string.IsNullOrEmpty(filename))
            {
                string ext = Path.GetExtension(filename);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.Remove(0,1);

                    var app = await this._dbContext.AppEntities.AsNoTracking().Where(app => app.DEFAULT_EXTENSION == ext).OrderByDescending(ap => ap.SYSTEM_ID).AnyAsync();

                    output = app;
                }
            }


            return output;
        }


        private async Task<bool> InsertExtNonGen(string filename, string mimeType)
        {
            bool output = false;
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    string ext = Path.GetExtension(filename);
                    if (!string.IsNullOrEmpty(ext))
                    {
                        ext = ext.Remove(0, 1);

                        if (!string.IsNullOrEmpty(ext) && !await _dbContext.AppEntities.AnyAsync(a => a.APPLICATION.ToUpper() == ext.ToUpper()))
                        {
                            AppEntity appToInsert = new()
                            {
                                APPLICATION = ext,
                                DESCRIPTION = ext,
                                FILING_SCHEME = 2,
                                DEFAULT_EXTENSION = ext,
                                MIME_TYPE = mimeType
                            };

                            await this._dbContext.AppEntities.AddAsync(appToInsert);
                            var rC = await ((DbContext)this._dbContext).SaveChangesAsync();

                            output = rC > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);

                var pendingAppEntity = ((DbContext)_dbContext).ChangeTracker.Entries<AppEntity>()
                    .Where(a => a.State == EntityState.Added)
                    .FirstOrDefault();

                if (pendingAppEntity != null)
                    pendingAppEntity.State = EntityState.Detached;
            }

            return output;
        }


        private string GetFileName(string fileName)
        {
            string res = null;
            if (fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
            {
                res = fileName.Substring(0, fileName.LastIndexOf("."));
            }
            else
            {
                res = fileName;
            }
            return res;
        }
        private string CleanDocNumber(string docnumber)
        {
            if(string.IsNullOrEmpty(docnumber))
                return null;

            long convertedDocNumber = 0;
            bool isNum = long.TryParse(docnumber.Trim(), out convertedDocNumber);
            if (isNum)
                return convertedDocNumber.ToString();

            return null;
        }

        private async Task InsDpaMailElab(string idMessage, string ragione)
        {
            MailElaborataEntity mailEl = new()
            {
                VAR_MESSAGE = idMessage.Replace("'", "''"),
                CHA_RAGIONE_ELAB = ragione,
                DTA_ELAB = await this._dbContext.GetSystemDateTime(),
                ID_REGISTRO = null,
                ID_PROFILE = null,
                VAR_EMAIL = null,
            };

            this._dbContext.MailElaborataEntities.Add(mailEl);

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }

        private async Task<bool> VerificaPresenzaNotifica(DocsPaVO.DatiCert.Daticert dartiCert, string systemIdTipoNotifica)
        {
            bool output = false;
            if (dartiCert == null)
                return output;

            Notifica notifica = new Notifica();
            notifica = (Notifica)dartiCert;

            if(!string.IsNullOrEmpty(systemIdTipoNotifica))
                notifica.idTipoNotifica = systemIdTipoNotifica;
            else
                return output;

            if (!string.IsNullOrEmpty(dartiCert.giorno) && !string.IsNullOrEmpty(dartiCert.ora))
                notifica.data_ora = dartiCert.giorno + " " + dartiCert.ora;
            else
                return output;

            if (dartiCert.risposteLst != null && dartiCert.risposteLst.Length > 0)
                notifica.risposte = dartiCert.risposteLst[0];
            else
                return output;


            var pred = PredicateBuilder.New<NotificaEntity>();
            bool applyPred = false;


            var notQuery = (from n in this._dbContext.NotificaEntities.AsNoTracking()
             where
                (n.ID_TIPO_NOTIFICA == notifica.idTipoNotifica.AsLong()) &&
                (n.DOCNUMBER == notifica.docnumber.AsLong()) &&
                (n.VAR_MITTENTE == notifica.mittente.Replace("'", "''")) &&
                (n.VAR_RISPOSTE == notifica.risposte.Replace("'", "''")) &&
                (n.VAR_OGGETTO == notifica.oggetto.Replace("'", "''")) &&
                (n.VAR_GESTIONE_EMITTENTE == notifica.gestioneEmittente) &&
                (n.VAR_ZONA == notifica.zona) &&
                n.VAR_GIORNO_ORA == notifica.data_ora.AsDateTime() &&
                (n.VAR_IDENTIFICATIVO == notifica.identificativo) &&
                (n.VAR_MSGID == notifica.msgid) &&
                (n.VAR_TIPO_RICEVUTA == notifica.tipoRicevuta) &&
                (n.VAR_CONSEGNA == notifica.consegna) &&
                (n.VAR_RICEZIONE == notifica.ricezione)

             select n);
            
            if (!string.IsNullOrEmpty(notifica.errore_esteso))
            {
                var er = notifica.errore_esteso.Replace("'", "''");
                applyPred = true;
                pred = pred.And(n => EF.Functions.Like(n.VAR_ERRORE_ESTESO, $"{er}"));
            }
            if (!string.IsNullOrEmpty(notifica.erroreRicevuta))
            {
                applyPred = true;
                pred = pred.And(n => n.VAR_ERRORE_RICEVUTA != null && n.VAR_ERRORE_RICEVUTA.Equals(notifica.erroreRicevuta.Replace("'", "''")));
            }

            NotificaEntity? not = null;
            if(applyPred)
                not = await notQuery.Where(pred).FirstOrDefaultAsync();
            else
                not = await notQuery.FirstOrDefaultAsync();


            output = not != null;

            return output;
        }

        private async Task<TipoNotifica> RicercaTipoNotificaByCodice(string codiceNotifica)
        {
            var tipoNotifica = new TipoNotifica();

            try
            {
                var not = await (from n in this._dbContext.TipoNotificaEntities.AsNoTracking()
                                 where (n.VAR_CODICE_NOTIFICA != null && n.VAR_CODICE_NOTIFICA.Equals(codiceNotifica)) || (codiceNotifica == n.VAR_CODICE_NOTIFICA)
                                 select n).ToListAsync();

                not.ForEach(e => tipoNotifica = new()
                {
                    idTipoNotifica = e.SYSTEM_ID.ToString(),
                    codiceNotifica = e.VAR_CODICE_NOTIFICA,
                    descrizioneNotifica = e.VAR_DESCRIZIONE
                });
            }
            catch (Exception ex)
            {
                tipoNotifica = null;
                this._logger.LogError(exception:ex,message:ex.Message);
            }
            

            return tipoNotifica;
        }

        #endregion
    }
}
