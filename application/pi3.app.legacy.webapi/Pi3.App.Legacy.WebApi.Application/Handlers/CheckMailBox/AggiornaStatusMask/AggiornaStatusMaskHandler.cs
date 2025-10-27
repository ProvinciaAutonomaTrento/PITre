// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DatiCert;
using DocsPaVO.documento;
using DocsPaVO.StatoInvio;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Daticert;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Eccezione;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.AggiornaStatusMask
{
    internal class AggiornaStatusMaskHandler : IRequestHandler<AggiornaStatusMaskRequest, AggiornaStatusMaskResult>
    {
        #region Public members
        public AggiornaStatusMaskHandler(ILogger<AggiornaStatusMaskHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService
        )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;

        }


        public async Task<AggiornaStatusMaskResult> Handle(AggiornaStatusMaskRequest request, CancellationToken cancellationToken)
        {
            bool retval = false;
            var notifica = request.notifica;
            _logger.LogDebug("Inserita la notifica, aggiorno la status-mask");
            try
            {
                // eliminareEccezione: quando l'esito è OK, elimino una possibile eccezione preesistente.
                bool eliminareEccezione = false;
                //DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert interop = new DocsPaDB.Query_DocsPAWS.InteroperabilitaDatiCert();
                TipoNotifica tipoNot = (await this._mediator.Send(new Application.Requests.getTipoNotifica(
                             request.notifica.idTipoNotifica
                            ))).output;


                DocsPaVO.StatoInvio.StatoInvio statoInvio = await this.getStatoInvioFromAddressAndProfile(notifica.destinatario, notifica.docnumber.AsLong());
                if (statoInvio != null)
                {
                    string statusmask = "";
                    if (!string.IsNullOrEmpty(statoInvio.statusMask))
                    {
                        statusmask = statoInvio.statusMask;
                        char[] status_c_mask = statusmask.ToCharArray();

                        if (tipoNot.codiceNotifica == "accettazione")
                        {
                            status_c_mask[1] = 'V';
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                if (notifica.tipoDestinatario == "certificato")
                                {
                                    if (status_c_mask[2] != 'V' || status_c_mask[2] == 'A' || status_c_mask[2] == 'N')
                                    {
                                        status_c_mask[2] = 'A';
                                    }
                                }
                                else
                                {
                                    status_c_mask[0] = 'V';
                                    eliminareEccezione = true;
                                }
                            }
                        }
                        else if (tipoNot.codiceNotifica == "non-accettazione")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[1] = 'X';
                            status_c_mask[2] = 'N';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'N';
                        }
                        else if (tipoNot.codiceNotifica == "DSN" || tipoNot.codiceNotifica == "errore")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[2] = 'X';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'V';
                        }
                        else if (tipoNot.codiceNotifica == "avvenuta-consegna")
                        {
                            status_c_mask[2] = 'V';
                            status_c_mask[6] = 'X';
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                status_c_mask[0] = 'V';
                                eliminareEccezione = true;
                            }
                        }
                        else if (tipoNot.codiceNotifica == "errore-consegna" || tipoNot.codiceNotifica == "preavviso-errore-consegna")
                        {
                            status_c_mask[0] = 'X';
                            status_c_mask[2] = 'X';
                            status_c_mask[3] = 'N';
                            status_c_mask[4] = 'N';
                            status_c_mask[5] = 'N';
                            status_c_mask[6] = 'N';
                            retval = await this.AggiornaDpa_StatoInvioConEccezione(notifica.destinatario, notifica.docnumber, "Errore di consegna verso la casella PEC.");
                        }
                        statusmask = new string(status_c_mask);
                    }
                    else
                    {
                        if (tipoNot.codiceNotifica == "accettazione")
                        {

                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                if (notifica.tipoDestinatario == "certificato")
                                {
                                    statusmask = "AVANNNN";
                                }
                                else
                                {
                                    statusmask = "VVNNNNN";
                                    eliminareEccezione = true;
                                }
                            }
                        }
                        else if (tipoNot.codiceNotifica == "non-accettazione")
                        {
                            statusmask = "XXNNNNN";
                        }
                        else if (tipoNot.codiceNotifica == "DSN" || tipoNot.codiceNotifica == "errore")
                        {
                            statusmask = "XVXNNNV";
                        }
                        else if (tipoNot.codiceNotifica == "avvenuta-consegna")
                        {
                            statusmask = "AVVAAAN";
                            if (statoInvio.tipoCanale == "MAIL")
                            {
                                statusmask = "VVVNNNN";
                                eliminareEccezione = true;
                            }
                        }
                        else if (tipoNot.codiceNotifica == "errore-consegna" || tipoNot.codiceNotifica == "preavviso-errore-consegna")
                        {
                            statusmask = "XVXNNNN";
                            retval = await this.AggiornaDpa_StatoInvioConEccezione(notifica.destinatario, notifica.docnumber, "Errore di consegna verso la casella PEC.");
                        }
                    }
                    retval = await this.AggiornaStatusMaskFromAddressAndProfile(notifica.destinatario, notifica.docnumber.AsLong(), statusmask, eliminareEccezione);


                }
                else
                {
                    _logger.LogDebug("Stato invio non trovato");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"Errore nell'aggiornamento della status mask: Messaggio {ex.Message} - StackTrace {ex.StackTrace}");
                retval = false;
            }

            return new AggiornaStatusMaskResult(retval);

        }



        #endregion









        #region Private members

        protected readonly ILogger<AggiornaStatusMaskHandler> _logger;

        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;


        private async Task<StatoInvio> getStatoInvioFromAddressAndProfile(string destinatario, long docnumber)
        {
            DocsPaVO.StatoInvio.StatoInvio retval = null;

            var statoInvioEntity = await this._dbContext.StatoInvioEntities.AsNoTracking()
                .Join(this._dbContext.DocumentTypesEntities, a => a.ID_DOCUMENTTYPE, b => b.SYSTEM_ID, (a, b) => new { a, b })
                .Where(x => x.a.VAR_INDIRIZZO.ToUpper() == destinatario.ToUpper() && x.a.ID_PROFILE == docnumber)
                .Select(x => new
                {

                    statusmask = x.a.STATUS_C_MASK,
                    tipo = x.b.TYPE_ID
                })
                .FirstOrDefaultAsync();

            if (statoInvioEntity != null)
            {
                retval = new DocsPaVO.StatoInvio.StatoInvio();
                retval.statusMask = statoInvioEntity.statusmask.ToString();
                retval.tipoCanale = statoInvioEntity.tipo.ToString();
            }
            return retval;
        }

        private async Task<bool> AggiornaDpa_StatoInvioConEccezione(string email, string docNumber, string motivo)
        {
            //Dato un docnumber cerco tutte le entry nella  stato invio
            List<long?> idCorrList = await this.GetIdCorrInStatoInvio(docNumber.AsLong());

            List<DocsPaVO.documento.ProtocolloDestinatario> pdList = new List<ProtocolloDestinatario>();

            foreach (long? corrId in idCorrList)
            {
                DocsPaVO.utente.Corrispondente corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(
                             corrId.ToString()
                            ))).output;

                corr.Emails = (await this._mediator.Send(new Application.Requests.GetMailCorrEsterno(
                             corrId.ToString()
                            ))).output;


                if ((corr.Emails == null) || (corr.Emails.Count == 0))
                {
                    List<MailCorrispondente> mcl = new List<MailCorrispondente>();
                    mcl.Add(new MailCorrispondente { Email = corr.email });
                    corr.Emails = mcl;
                }

                foreach (MailCorrispondente mcItem in corr.Emails)
                {
                    if (mcItem.Email == email)
                    {
                        var statoInvioAL = (await this._mediator.Send(new Application.Requests.InteroperabilitaAggiornamentoConferma(
                            docNumber.ToString(),
                             corr
                            ))).output;
                        foreach (DocsPaVO.documento.ProtocolloDestinatario p in statoInvioAL)
                            pdList.Add(p);
                    }
                }

            }

            if (pdList.Count == 1)
            {
                bool res_update = await updateStatoInvioAnnulla(pdList[0].systemId, motivo);
                if (!res_update)
                {
                    _logger.LogDebug("Errore: non e' stato eseguito l'update del profilo");
                    return false;
                }
            }
            else
            {
                _logger.LogDebug("In aggiornamento stato invio il docnumber {0} ha restituito numero {1} entry, questo è un problema", docNumber, pdList.Count());
                foreach (DocsPaVO.documento.ProtocolloDestinatario p in pdList)
                    _logger.LogDebug("protocollodestinatario {0}  pd {1} ", p.systemId, p.protocolloDestinatario);

                //Urca! piu di uno e mo? bho, torno false..
                return false;
            }
            return true;
        }

        private async Task<List<long?>> GetIdCorrInStatoInvio(long docnumber)
        {
            _logger.LogDebug("get IdCorr in DPA_STATO_INVIO");
            List<long?> listaIdCorr = await this._dbContext.StatoInvioEntities.AsNoTracking()
               .Where(x => x.ID_PROFILE == docnumber)
               .Select(x => x.ID_CORR_GLOBALE)
               .ToListAsync();

            return listaIdCorr;
        }


        private async Task<bool> updateStatoInvioAnnulla(string systemID, string motivoAnnulla)
        {
            bool result = false;

            try
            {
                result = await this.updStatoInvioEccezione(systemID, motivoAnnulla);
            }
            catch (Exception e)
            {
                _logger.LogError("Eccezione: " + e.Message);

                result = false;
            }

            return result;
        }

        private async Task<bool> updStatoInvioEccezione(string SystemID, string motivo_annulla)
        {
            bool res = false;
            string statusmask = await this.getStatusMask1("", "", "", SystemID);
            if (!string.IsNullOrEmpty(statusmask))
            {
                char[] sm = statusmask.ToCharArray();
                if (sm[5] == 'A' && sm[2] == 'V')
                {
                    sm[0] = 'X';
                    sm[3] = 'N';
                    sm[4] = 'N';
                    sm[5] = 'X';
                    if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                    {
                        sm[0] = 'A';
                        sm[3] = 'A';
                        sm[4] = 'A';
                    }
                    statusmask = new string(sm);
                }
            }


            var statoInvio = this._dbContext.StatoInvioEntities
                  .Where(x => x.SYSTEM_ID == SystemID.AsLong())
                  .ToList();

            if (!string.IsNullOrEmpty(statusmask))
            {
                statoInvio.ForEach(x => x.STATUS_C_MASK = statusmask);
            }
            else
            {
                if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                {
                    statoInvio.ForEach(x => x.STATUS_C_MASK = "AVVAAXN");
                }
                else
                {
                    statoInvio.ForEach(x => x.STATUS_C_MASK = "XVVNNXN");
                }
            }

            string condition = String.Format("SYSTEM_ID = {0}", SystemID);
            string values = string.Format("VAR_MOTIVO_ANNULLA = '{0}' ,CHA_ANNULLATO = '{1}'", motivo_annulla.Replace("'", "''"), "E");
            if (!string.IsNullOrEmpty(statusmask))
            {
                values += ", STATUS_C_MASK ='" + statusmask + "'";
            }
            else
            {
                if (motivo_annulla.Contains("Il documento è stato ricevuto dall’Amministrazione destinataria e pertanto non occorre effettuare rispedizioni."))
                {
                    values += ", STATUS_C_MASK ='AVVAAXN'";
                }
                else
                {
                    values += ", STATUS_C_MASK ='XVVNNXN'";
                }
            }

            res = ((await ((DbContext)this._dbContext).SaveChangesAsync()) > 0);
            return res;
        }

        private async Task<string> getStatusMask1(string idProf, string codiceAOO, string codiceAmministrazione, string systemidDPASI = "")
        {
            string retVal = string.Empty;

            var statusMask = _dbContext.StatoInvioEntities.AsNoTracking();
            if (!string.IsNullOrEmpty(systemidDPASI))
            {
                statusMask = statusMask.Where(si => si.SYSTEM_ID.ToString() == systemidDPASI);
            }
            else
            {
                if (!string.IsNullOrEmpty(idProf))
                {
                    statusMask = statusMask.Where(si => si.ID_PROFILE.ToString() == idProf);
                }
                if (!string.IsNullOrEmpty(codiceAOO))
                {
                    statusMask = statusMask.Where(si => si.VAR_CODICE_AOO.ToUpper() == codiceAOO.ToUpper());
                }
                if (!string.IsNullOrEmpty(codiceAmministrazione))
                {
                    statusMask = statusMask.Where(si => si.VAR_CODICE_AMM.ToUpper() == codiceAmministrazione.ToUpper());
                }
            }

            retVal = await statusMask.Select(x => x.STATUS_C_MASK).FirstOrDefaultAsync();


            return retVal;
        }


        private async Task<bool> AggiornaStatusMaskFromAddressAndProfile(string destinatario, long? docnumber, string statusmask, bool eliminareEccezione)
        {
            bool retval = false;
            try
            {

                var myEntity = this._dbContext.StatoInvioEntities
                    .Where(x => x.VAR_INDIRIZZO.ToUpper() == destinatario.ToUpper() && x.ID_PROFILE == docnumber)
                    .ToList();
                if (eliminareEccezione)
                    myEntity.ForEach(x => { x.STATUS_C_MASK = statusmask;
                        x.CHA_ANNULLATO = null;
                        x.VAR_MOTIVO_ANNULLA = null;
                        }
                    );
                else
                     myEntity.ForEach(x => x.STATUS_C_MASK = statusmask);

                



               
                _logger.LogDebug("Aggiornamento maschera status");

                retval = ((await ((DbContext)this._dbContext).SaveChangesAsync())>0); //bisogna vedere se questo funziona

            }
            catch (Exception ex)
            {
                _logger.LogError("Errore nell'aggiornamento della status mask. Messaggio {0}, Stacktrace {1}", ex.Message, ex.StackTrace);
            }
            return retval;
        }
        #endregion



    }
}



