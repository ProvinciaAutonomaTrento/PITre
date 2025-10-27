// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AmmGetInfoAmmCorrente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using System.Security.Cryptography.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStampAndSignature
{
    // Richiede libreria MediatR
    public class GetStampAndSignatureCommandHandler : IRequestHandler<GetStampAndSignatureCommand, GetStampAndSignatureCommandResponse>
    {
        #region Public Members

        public GetStampAndSignatureCommandHandler(ILogger<GetStampAndSignatureCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetStampAndSignatureCommandResponse> Handle(GetStampAndSignatureCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetStampAndSignature - START");

            GetStampAndSignatureCommandResponse response = new GetStampAndSignatureCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                string idDocument = request.idDocument;
                string signature = request.signature;
                if (string.IsNullOrEmpty(request.idDocument) && string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.idDocument) && !string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }
                #endregion

                #region implementazione

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                try
                {
                    if (!string.IsNullOrEmpty(idDocument))
                    {
                        documento = await this.GetDettaglio(infoUtente, idDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(signature))
                        {
                            documento = await this.RicercaProto(signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                try
                {

                    if (documento != null)
                    {
                        response.Stamp = await this.GetStampAndSignature(documento, infoUtente);

                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }


                #endregion

                response.Code = GetStampResponseCode.OK;

                _logger.LogInformation("end GetStampAndSignature");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetStampAndSignature: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetStampAndSignatureCommandResponse();
                response.Code = GetStampResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetStampAndSignature");
                response = new GetStampAndSignatureCommandResponse();
                response.Code = GetStampResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetStampAndSignatureCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        private async Task<SchedaDocumento> RicercaProto(string segnatura, InfoUtente infoUtente)
        {
            var profileEntity = await this._pi3DbContext.ProfileEntities.AsNoTracking().FirstOrDefaultAsync(p => p.VAR_SEGNATURA.ToUpper().Equals(segnatura.ToUpper()));
            string idProfile = string.Empty;
            if(profileEntity != null)
            {
                idProfile = profileEntity.SYSTEM_ID.ToString();
            }
            var output = await this.GetDettaglio(infoUtente, idProfile);

            return output;
        }

        private async Task<SchedaDocumento> GetDettaglio(InfoUtente infoUtente, string idProfile)
        {
            await this._pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);

            SchedaDocumento? output = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
            {
                Infoutente = infoUtente,
                DocNumber = string.Empty,
                IdProfile = idProfile
            })).Output;
            if(output != null)
            {
                output.accessRights = (await this._pi3DbContext.GetSecurity(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo)).ACCESSRIGHTS.ToString();
            }

            return output; 
        }

        private string GetDatiTimbro(string TimbroIniziale, string datiTimbro, DocsPaVO.amministrazione.InfoAmministrazione currAmm, Stamp stamp)
        {
            string retValue = string.Empty;
            string separatore = " ";
            string escape = string.Empty;
            string timbro = string.Empty;

            string sep = DBUtils.GetSeparatore(currAmm.IDAmm,this._pi3DbContext);

            string[] lastVal = { "COD_AMM", "COD_REG", "NUM_PROTO", "DATA_COMP", "ORA", "NUM_ALLEG", "CLASSIFICA", "IN_OUT", "COD_UO_PROT", "COD_UO_VIS", "COD_RF_PROT", "COD_RF_VIS" };

            if (datiTimbro.Contains("COD_AMM"))
            {
                string codAmm = currAmm.Codice;
                if (codAmm != string.Empty)
                {
                    datiTimbro = datiTimbro.Replace("COD_AMM", (codAmm + escape));
                    lastVal[0] = codAmm + escape;
                }
                else
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "COD_AMM", lastVal);
                    datiTimbro = datiTimbro.Replace("COD_AMM", "");
                }
            }

            if (datiTimbro.Contains("COD_REG"))
            {
                lastVal[1] = stamp.CodeRegister + escape;
                datiTimbro = datiTimbro.Replace("COD_REG", stamp.CodeRegister);
            }

            if (datiTimbro.Contains("NUM_PROTO"))
            {
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

            if (datiTimbro.Contains("DATA_COMP"))
            {
                datiTimbro = datiTimbro.Replace("DATA_COMP", (stamp.DataProtocol + escape));
                lastVal[3] = stamp.DataProtocol + escape;
            }

            if (datiTimbro.Contains("ORA"))
            {
                string ora = stamp.TimeProtocol;
                if ((ora != null) && (ora != ""))
                {
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

            if (datiTimbro.Contains("NUM_ALLEG"))
            {
                datiTimbro = datiTimbro.Replace("NUM_ALLEG", (stamp.NumberAtthachements + escape));
                lastVal[5] = System.Convert.ToString(stamp.NumberAtthachements) + escape;
            }

            if (datiTimbro.Contains("CLASSIFICA"))
            {
                timbro = timbro + stamp.Classifications + escape;
                if (timbro == string.Empty)
                {
                    datiTimbro = RemoveDesc(TimbroIniziale, datiTimbro, "CLASSIFICA", lastVal);
                    datiTimbro = datiTimbro.Replace("CLASSIFICA", "");
                }
                else
                {
                    datiTimbro = datiTimbro.Replace("CLASSIFICA", timbro);
                    lastVal[6] = timbro;
                }

            }

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

            timbro = datiTimbro;

            retValue = timbro;

            return retValue;
        }
        private async Task<Stamp> GetStampAndSignature(DocsPaVO.documento.SchedaDocumento documento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Stamp result = new Stamp();
            if (documento == null)
            {
                result = null;
            }
            else
            {
                Dictionary<string, string> dati = this.GetDatiSegnaturaTimbro(documento);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("segnatura"))
                    {
                        result.SignatureValue = dati["segnatura"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("resgistro"))
                    {
                        result.CodeRegister = dati["resgistro"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("uo"))
                    {
                        result.CodeUO = dati["uo"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("amministrazione"))
                    {
                        result.CodeAdministration = dati["amministrazione"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("anno"))
                    {
                        result.Year = dati["anno"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("data"))
                    {
                        result.DataProtocol = dati["data"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("ora"))
                    {
                        result.TimeProtocol = dati["ora"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("tipo"))
                    {
                        result.TypeProtocol = dati["tipo"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("numero"))
                    {
                        result.NumberProtocol = dati["numero"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("allegati"))
                    {
                        result.NumberAtthachements = dati["allegati"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("fascicoli"))
                    {
                        result.Classifications = dati["fascicoli"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("docnumber"))
                    {
                        result.DocNumber = dati["docnumber"] ?? string.Empty;
                    }
                    if (dati.ContainsKey("rf"))
                    {
                        result.CodeRf = dati["rf"] ?? string.Empty;
                    }
                }
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = ( await this._mediator.Send(new AmmGetInfoAmmCorrenteCommand()
                {
                    IdAmm = infoUtente.idAmministrazione
                }) ).Output;
                string TimbroIniziale = infoAmm.Timbro_pdf;
                string timbro = this.GetDatiTimbro(TimbroIniziale, TimbroIniziale, infoAmm, result);
                result.StampValue = timbro;

            }

            return result;
        }

        private Dictionary<string, string> GetDatiSegnaturaTimbro(DocsPaVO.documento.SchedaDocumento documento)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            var query = this._pi3DbContext.ProfileEntities.AsNoTracking().Where(p => p.SYSTEM_ID == documento.systemId.AsLong()).Join(
                this._pi3DbContext.AmministraEntities.AsNoTracking(),
                p => IPi3DbContextMappedFunctions.GetIdAmm(p.ID_PEOPLE_PROT.GetValueOrDefault()),
                d => d.SYSTEM_ID,
                (p, d) => new
                {
                    p.VAR_SEGNATURA,
                    COD_REG = IPi3DbContextMappedFunctions.GetCodReg(p.ID_REGISTRO.GetValueOrDefault()),
                    UO_CREATORE = IPi3DbContextMappedFunctions.GetCodUo(p.ID_UO_CREATORE.GetValueOrDefault()),
                    d.VAR_CODICE_AMM,
                    ANNO = p.NUM_ANNO_PROTO,
                    DTA_PROTO = p.DTA_PROTO != null ? p.DTA_PROTO.AsDateFormat() : null,
                    ORA_PROTO = p.DTA_PROTO != null ? p.DTA_PROTO.AsHoursMinutesSecondsFormat() : null,
                    p.CHA_TIPO_PROTO,
                    p.NUM_PROTO,
                    ALLEGATI = IPi3DbContextMappedFunctions.CountAllegatiByDocNumber(p.DOCNUMBER.GetValueOrDefault()),
                    COD_FASC = IPi3DbContextMappedFunctions.ClassCat(p.SYSTEM_ID),
                    p.DOCNUMBER,
                    COD_RF = IPi3DbContextMappedFunctions.GetCodiceRfByProfileId(p.SYSTEM_ID)
                });

            foreach (var row in query)
            {
                output.Add("segnatura", row.VAR_SEGNATURA ?? string.Empty);
                output.Add("resgistro", row.COD_REG ?? string.Empty);
                output.Add("uo", row.UO_CREATORE ?? string.Empty);
                output.Add("amministrazione", row.VAR_CODICE_AMM ?? string.Empty);
                output.Add("anno", row.ANNO != null ? row.ANNO.ToString() : string.Empty);
                output.Add("data", row.DTA_PROTO ?? string.Empty);
                output.Add("ora", row.ORA_PROTO ?? string.Empty);
                output.Add("tipo", row.CHA_TIPO_PROTO ?? string.Empty);
                output.Add("numero", row.NUM_PROTO != null ? row.NUM_PROTO.ToString() : string.Empty);
                output.Add("allegati", row.ALLEGATI != null ? row.ALLEGATI.ToString() : string.Empty);
                output.Add("fascicoli", row.COD_FASC ?? string.Empty);
                output.Add("docnumber", row.DOCNUMBER != null ? row.DOCNUMBER.ToString() : string.Empty);
                output.Add("rf", row.COD_RF);
            }

            return output;
        }


        private string RemoveDesc(string timbro_iniziale, string currTimbro, string currVal, string[] dati)
        {
            int count = 0;
            int start = 0;
            int inizio = 0;
            string specialChar = "#%*@";
            while (currTimbro.Contains(currVal))
            {
                int[] ordine = CodicePrec(timbro_iniziale);

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
            }
            currTimbro = currTimbro.Replace(specialChar, currVal);
            return currTimbro;
        }

        private static int[] CodicePrec(string timbro_iniziale)
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
                if (timbro_iniziale == appo)
                {
                    timbro_iniziale = timbro_iniziale.Remove(0, 1);
                }
            }
            return ordine;
        }

        #endregion
    }

}