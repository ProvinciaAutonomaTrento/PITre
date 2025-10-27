// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.InstanceAccess;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AsyncCreateDownloadRequest = Pi3.App.Legacy.WebApi.Application.Requests.AsyncCreateDownload;
using System.Xml;
using System.Globalization;
using DocsPaVO.utente;
using System.Net.Mail;
using DocsPaVO.documento;
using DocumentFormat.OpenXml;
using System.Collections;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Validation;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AsyncCreateDownload
{
    public class AsyncCreateDownloadHandler : IRequestHandler<AsyncCreateDownloadRequest, AsyncCreateDownloadResult>
    {

        protected readonly ILogger<AsyncCreateDownloadHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected Dictionary<string, string> infoDoc = new Dictionary<string, string>();

        private string ReplaceInvalidChar(string path)
        {
            string resultPath = path;
            char[] invalid = System.IO.Path.GetInvalidPathChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }
        private async Task UpdateInstanceStartDownload(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUser, string type)
        {
            string? idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            string rootPath = (await this._configurationService.GetValue<string>(idAmministrazione, "BE_INSTANCE_ACCESS_PATH"));

            if (!string.IsNullOrEmpty(rootPath))
            {
                InstanceAccessEntity? iaToUpdate = await this._dbContext.InstanceAccessEntities.FirstOrDefaultAsync(ia => ia.SYSTEM_ID == instanceAccess.ID_INSTANCE_ACCESS.AsLong());

                if (iaToUpdate != null)
                {
                    iaToUpdate.CHA_STATO_DOWNLOAD_INOLTRO = type;
                    await ((DbContext)this._dbContext).SaveChangesAsync();

                }
            }

        }
        private DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa ConvertiUO(DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa)
        {
            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML = new DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa();
            uoXML.CodiceUO = unitaOrganizzativa.codiceRubrica;
            uoXML.DescrizioneUO = unitaOrganizzativa.descrizione;
            uoXML.Livello = unitaOrganizzativa.livello;

            if (unitaOrganizzativa.parent == null)
                return uoXML;

            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML_Padre = ConvertiUO(unitaOrganizzativa.parent);
            List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa> uoXMLLst = new List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa>();
            uoXMLLst.Add(uoXML_Padre);
            uoXML.UnitaOrganizzativa1 = uoXMLLst.ToArray();

            return uoXML;
        }
        private async Task<DocsPaVO.InstanceAccess.Metadata.Amministrazione> GetInfoAmministrazione(string idAmm)
        {
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = (await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(idAmm))).output;
            return new DocsPaVO.InstanceAccess.Metadata.Amministrazione { CodiceAmministrazione = infoAmm.Codice, DescrizioneAmministrazione = infoAmm.Descrizione };
        }
        public async Task<DocsPaVO.utente.Utente> GetUtente(string idPeople)
        {
            var user = await (this._dbContext.PeopleEntities.AsNoTracking().
                Where(p => (p.DISABLED == null || !p.DISABLED.Equals("Y") && p.SYSTEM_ID == idPeople.AsLong()))).
                Select(p => new {

                    p.SYSTEM_ID,
                    p.USER_ID,
                    p.ID_AMM,
                    p.VAR_COGNOME,
                    p.VAR_NOME,
                    p.VAR_TELEFONO,
                    p.EMAIL_ADDRESS,
                    p.FROM_EMAIL_ADDRESS,
                    p.CHA_NOTIFICA,
                    p.CHA_AMMINISTRATORE,
                    p.CHA_NOTIFICA_CON_ALLEGATO,
                    p.VAR_SEDE,
                    p.MATRICOLA
                }).FirstOrDefaultAsync();

            DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();

            utente.idPeople = user.SYSTEM_ID.ToString();
            utente.userId = user.USER_ID;
            utente.descrizione = user.VAR_COGNOME + " " + user.VAR_NOME;
            utente.telefono = user.VAR_TELEFONO;
            utente.email = user.EMAIL_ADDRESS;
            utente.notifica = user.CHA_NOTIFICA;
            utente.amministratore = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.assegnante = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.assegnatario = FromCharToBool(user.CHA_AMMINISTRATORE);
            utente.idAmministrazione = user.ID_AMM.ToString();
            utente.notificaConAllegato = FromCharToBool(user.CHA_NOTIFICA_CON_ALLEGATO);
            utente.sede = user.VAR_SEDE;
            utente.matricola = (user.MATRICOLA != null ? user.MATRICOLA : null);
            utente.tipoCorrispondente = "P";
            utente.cognome = user.VAR_COGNOME;
            utente.nome = user.VAR_NOME;
            return utente;
        }
        private bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }
        private async Task<DocsPaVO.InstanceAccess.Metadata.Creatore> GetCreatore(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.InstanceAccess.Metadata.Creatore creatore = new DocsPaVO.InstanceAccess.Metadata.Creatore();
            creatore.CodiceRuolo = ruolo.codiceRubrica;
            creatore.DescrizioneRuolo = ruolo.descrizione;
            creatore.CodiceUtente = schDoc.userId;
            DocsPaVO.utente.Utente user = await this.GetUtente(schDoc.creatoreDocumento.idPeople);
            if (user != null)
                creatore.DescrizioneUtente = user.descrizione;
            return creatore;
        }

        private string ConvertiTipoPoto(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string retval = schDoc.tipoProto;
            switch (schDoc.tipoProto)
            {

                case "A":
                case "P":
                case "I":
                    {
                        retval = "Protocollato";
                        if (schDoc.protocollo != null)
                            if (string.IsNullOrEmpty(schDoc.protocollo.segnatura))
                                retval = "Predisposto";
                    }
                    break;

                case "G":
                    retval = "Grigio";
                    break;
            }
            return retval;
        }
        private string FormattaData(DateTime data)
        {
            return string.Format("{0}-{1}-{2}", data.Year, data.Month.ToString().PadLeft(2, '0'), data.Day.ToString().PadLeft(2, '0'));
        }
        private DateTime convertiData(string data)
        {
            DateTime dt = DateTime.Now;
            DateTime.TryParse(data, out dt);
            return dt;
        }
        public string FormattaOra(DateTime data)
        {
            return string.Format("{0}:{1}", data.Hour.ToString().PadLeft(2, '0'), data.Minute.ToString().PadLeft(2, '0'));
        }
        private async Task<DocsPaVO.InstanceAccess.Metadata.Registrazione> getRegistrazione(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo)
        {
            if (schDoc.protocollo != null)
            {
                DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione = new DocsPaVO.InstanceAccess.Metadata.Registrazione();
                registrazione.DataProtocollo = FormattaData(convertiData(schDoc.protocollo.dataProtocollazione));
                registrazione.OraProtocollo = FormattaOra(convertiData(schDoc.protocollo.dataProtocollazione));
                registrazione.NumeroProtocollo = schDoc.protocollo.numero;
                registrazione.SegnaturaProtocollo = schDoc.protocollo.segnatura;
                registrazione.TipoProtocollo = schDoc.tipoProto;

                registrazione.CodiceRF = null;
                registrazione.DescrizioneRF = null;

                if (schDoc.datiEmergenza != null)
                    registrazione.SegnaturaEmergenza = schDoc.datiEmergenza.protocolloEmergenza;

                if (schDoc.registro != null)
                {
                    registrazione.CodiceAOO = schDoc.registro.codRegistro;
                    registrazione.DescrizioneAOO = schDoc.registro.descrizione;
                }

                EstraiDatiProtoEntrata(schDoc, registrazione);
                EstraiDatiProtoUscita(schDoc, registrazione);
                EstraiDatiProtoInterno(schDoc, registrazione);

                if (schDoc.protocollatore != null)
                {
                    DocsPaVO.utente.Utente userProt = await this.GetUtente(schDoc.protocollatore.utente_idPeople);
                    DocsPaVO.utente.Ruolo ruoloProt = (await this._mediator.Send(new Application.Requests.GetRuoloEnabledAndDisabled(schDoc.creatoreDocumento.idCorrGlob_Ruolo))).output;
                    DocsPaVO.InstanceAccess.Metadata.Protocollista protocollista = new DocsPaVO.InstanceAccess.Metadata.Protocollista();

                    protocollista.DescrizioneUtente = userProt.descrizione;
                    protocollista.CodiceUtente = userProt.userId;
                    protocollista.DescrizioneRuolo = ruoloProt.descrizione;
                    protocollista.CodiceRuolo = ruoloProt.codiceRubrica;
                    protocollista.UOAppartenenza = ruolo.uo.codiceRubrica;
                    registrazione.Protocollista = protocollista;
                }
                return registrazione;
            }
            return null;
        }

        private void EstraiDatiProtoInterno(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloInterno protInt = schDoc.protocollo as DocsPaVO.documento.ProtocolloInterno;
            if (protInt != null)
            {
                List<DocsPaVO.InstanceAccess.Metadata.Destinatario> destList = new List<DocsPaVO.InstanceAccess.Metadata.Destinatario>();
                if (protInt.destinatari != null)
                {
                    foreach (object c in protInt.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protInt.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }
                if (protInt.destinatariConoscenza != null)
                {
                    foreach (object c in protInt.destinatariConoscenza)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protInt.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }

                if (protInt.mittente != null)
                {
                    List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protInt.mittente.codiceRubrica,
                        IndirizzoMail = protInt.mittente.email,
                        Descrizione = protInt.mittente.descrizione,
                        ProtocolloMittente = null,
                        DataProtocolloMittente = null
                    };
                    mittList.Add(m);
                    registrazione.Mittente = mittList.ToArray();
                }
                registrazione.Destinatario = destList.ToArray();
            }
        }
        private void EstraiDatiProtoUscita(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloUscita protUsc = schDoc.protocollo as DocsPaVO.documento.ProtocolloUscita;
            if (protUsc != null)
            {
                List<DocsPaVO.InstanceAccess.Metadata.Destinatario> destList = new List<DocsPaVO.InstanceAccess.Metadata.Destinatario>();
                if (protUsc.destinatari != null)
                {
                    foreach (object c in protUsc.destinatari)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protUsc.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }
                if (protUsc.destinatariConoscenza != null)
                {
                    foreach (object c in protUsc.destinatariConoscenza)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Destinatario d = new DocsPaVO.InstanceAccess.Metadata.Destinatario
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            MezzoSpedizione = protUsc.mezzoSpedizione.ToString()
                        };
                        destList.Add(d);
                    }
                }

                if (protUsc.mittente != null)
                {
                    List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protUsc.mittente.codiceRubrica,
                        IndirizzoMail = protUsc.mittente.email,
                        Descrizione = protUsc.mittente.descrizione,
                        ProtocolloMittente = null,
                        DataProtocolloMittente = null
                    };
                    mittList.Add(m);
                    registrazione.Mittente = mittList.ToArray();
                }
                registrazione.Destinatario = destList.ToArray();
            }
        }
        private void EstraiDatiProtoEntrata(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.InstanceAccess.Metadata.Registrazione registrazione)
        {
            DocsPaVO.documento.ProtocolloEntrata protEnt = schDoc.protocollo as DocsPaVO.documento.ProtocolloEntrata;
            if (protEnt != null)
            {
                registrazione.ProtocolloMittente = new DocsPaVO.InstanceAccess.Metadata.ProtocolloMittente
                {
                    Protocollo = protEnt.numero,
                    MezzoSpedizione = protEnt.mezzoSpedizione.ToString(),
                    Data = protEnt.dataProtocollazione
                };

                DocsPaVO.utente.Corrispondente corr = protEnt.mittente;
                List<DocsPaVO.InstanceAccess.Metadata.Mittente> mittList = new List<DocsPaVO.InstanceAccess.Metadata.Mittente>();

                if (protEnt.mittenti != null)
                {
                    foreach (object c in protEnt.mittenti)
                    {
                        DocsPaVO.utente.Corrispondente corrItem = c as DocsPaVO.utente.Corrispondente;
                        DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                        {
                            Codice = corrItem.codiceRubrica,
                            IndirizzoMail = corrItem.email,
                            Descrizione = corrItem.descrizione,
                            ProtocolloMittente = protEnt.numero,
                            DataProtocolloMittente = protEnt.dataProtocolloMittente
                        };
                        mittList.Add(m);
                    }
                }
                if (protEnt.mittenteIntermedio != null)
                {
                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = protEnt.mittenteIntermedio.codiceRubrica,
                        IndirizzoMail = protEnt.mittenteIntermedio.email,
                        Descrizione = protEnt.mittenteIntermedio.descrizione,
                        ProtocolloMittente = protEnt.numero,
                        DataProtocolloMittente = protEnt.dataProtocolloMittente
                    };
                    mittList.Add(m);
                }
                {
                    DocsPaVO.InstanceAccess.Metadata.Mittente m = new DocsPaVO.InstanceAccess.Metadata.Mittente
                    {
                        Codice = corr.codiceRubrica,
                        IndirizzoMail = corr.email,
                        Descrizione = corr.descrizione,
                        ProtocolloMittente = protEnt.numero,
                        DataProtocolloMittente = protEnt.dataProtocolloMittente
                    };
                    mittList.Add(m);
                }
                registrazione.Mittente = mittList.ToArray();

            }

        }


        private async Task<DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico> GetContestoArchivistico(DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.utente.Ruolo ruolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico retval = new DocsPaVO.InstanceAccess.Metadata.ContestoArchivistico();
            List<DocsPaVO.InstanceAccess.Metadata.Fascicolazione> fasList = new List<DocsPaVO.InstanceAccess.Metadata.Fascicolazione>();
            List<DocsPaVO.InstanceAccess.Metadata.Classificazione> titList = new List<DocsPaVO.InstanceAccess.Metadata.Classificazione>();
            object[] fasAList = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoliDaDocNoSecurity(infoUtente, schDoc.systemId))).output;
            foreach (object fo in fasAList)
            {
                DocsPaVO.fascicolazione.Fascicolo fas = fo as DocsPaVO.fascicolazione.Fascicolo;

                if (fas != null)
                {
                    if (fas.tipo == "P")
                    {
                        DocsPaVO.InstanceAccess.Metadata.Fascicolazione fascicolazione = new DocsPaVO.InstanceAccess.Metadata.Fascicolazione();

                        fascicolazione.DescrizioneFascicolo = fas.descrizione;
                        fascicolazione.CodiceFascicolo = fas.codice;

                        fascicolazione.CodiceSottofascicolo = null;
                        fascicolazione.DescrizioneSottofascicolo = null;


                        fasList.Add(fascicolazione);
                        if (fas.idTitolario != null)
                        {
                            DocsPaVO.amministrazione.OrgNodoTitolario nodo = (await this._mediator.Send(new Application.Requests.getNodoTitolarioById(fas.idTitolario))).output;
                            fascicolazione.TitolarioDiRierimento = nodo.Descrizione;
                        }

                        var fascicoli = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFoldersDocumentFascicolo(schDoc.systemId, fas.systemID))).output;
                        foreach (DocsPaVO.fascicolazione.Folder f in fascicoli)
                        {
                            DocsPaVO.InstanceAccess.Metadata.Fascicolazione fasFolder = new DocsPaVO.InstanceAccess.Metadata.Fascicolazione();
                            fasFolder.CodiceFascicolo = fas.descrizione;
                            fasFolder.DescrizioneFascicolo = fas.codice;
                            fasFolder.CodiceSottofascicolo = f.systemID;
                            fasFolder.DescrizioneSottofascicolo = f.descrizione;
                            fasFolder.TitolarioDiRierimento = fascicolazione.TitolarioDiRierimento;
                            fasList.Add(fasFolder);
                        }

                    }
                    else
                    {
                        DocsPaVO.amministrazione.OrgNodoTitolario nodo = (await this._mediator.Send(new Application.Requests.getNodoTitolarioById(fas.idTitolario))).output;
                        DocsPaVO.InstanceAccess.Metadata.Classificazione cl = new DocsPaVO.InstanceAccess.Metadata.Classificazione();
                        cl.TitolarioDiRiferimento = nodo.Descrizione;
                        cl.CodiceClassificazione = nodo.Codice;
                        titList.Add(cl);
                    }
                }
            }

            List<DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato> lstDocColl = new List<DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato>();
            if (schDoc.rispostaDocumento != null)
            {
                if ((schDoc.rispostaDocumento.docNumber != null) && (schDoc.rispostaDocumento.idProfile != null))
                {

                    DocsPaVO.documento.SchedaDocumento sc = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, schDoc.rispostaDocumento.idProfile, schDoc.rispostaDocumento.idProfile))).output;
                    if (sc == null)
                        this._logger.LogDebug("DOCUMENTO NON TROVATO");
                    DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato docColl = new DocsPaVO.InstanceAccess.Metadata.DocumentoCollegato
                    {
                        IDdocumento = schDoc.rispostaDocumento.idProfile,
                        DataCreazione = FormattaData(convertiData(sc.dataCreazione)),
                        Oggetto = sc.oggetto.descrizione,


                    };
                    if (sc.protocollo != null)
                    {
                        docColl.DataProtocollo = FormattaData(convertiData(sc.protocollo.dataProtocollazione));
                        docColl.NumeroProtocollo = sc.protocollo.numero;
                        docColl.SegnaturaProtocollo = sc.protocollo.segnatura;
                    }
                    lstDocColl.Add(docColl);
                }
            }



            retval.Fascicolazione = fasList.ToArray();
            retval.Classificazione = titList.ToArray();
            retval.DocumentoCollegato = lstDocColl.ToArray();


            return retval;
        }



        private async Task<string> XmlDoc(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.documento.FileRequest objFileRequest, InstanceAccessDocument instanceDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.Metadata.Document documento = new DocsPaVO.InstanceAccess.Metadata.Document();

            DocsPaVO.utente.Ruolo ruolo = (await this._mediator.Send(new Application.Requests.GetRuoloEnabledAndDisabled(schDoc.creatoreDocumento.idCorrGlob_Ruolo))).output;
            DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa = ruolo.uo;

            List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa> uoL = new List<DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa>();
            DocsPaVO.InstanceAccess.Metadata.UnitaOrganizzativa uoXML = ConvertiUO(unitaOrganizzativa);
            uoL.Add(uoXML);

            documento.SoggettoProduttore = new DocsPaVO.InstanceAccess.Metadata.SoggettoProduttore
            {
                Amministrazione = await this.GetInfoAmministrazione(ruolo.idAmministrazione),
                GerarchiaUO = new DocsPaVO.InstanceAccess.Metadata.GerarchiaUO { UnitaOrganizzativa = uoL.ToArray() },
                Creatore = await GetCreatore(schDoc, ruolo)
            };

            documento.IDdocumento = schDoc.systemId;
            documento.Oggetto = schDoc.oggetto.descrizione;
            documento.Tipo = ConvertiTipoPoto(schDoc);
            documento.DataCreazione = schDoc.dataCreazione;

            if (schDoc.privato != null && schDoc.privato.Equals("1"))
                documento.LivelloRiservatezza = "privato";
            else
                documento.LivelloRiservatezza = string.Empty;
            if (instanceDoc.ENABLE && fileDoc != null)
                documento.File = new(); // getFileDetail temporaneamente non richiamato - estrazione firma non implementata
            documento.Registrazione = await getRegistrazione(schDoc, ruolo);
            documento.ContestoArchivistico = await GetContestoArchivistico(schDoc, ruolo, infoUtente);

            if (schDoc.template != null)
            {
                DocsPaVO.InstanceAccess.Metadata.Tipologia t = new DocsPaVO.InstanceAccess.Metadata.Tipologia { NomeTipologia = schDoc.template.DESCRIZIONE, CampoTipologia = await GetCampiTipologia(schDoc.template) };
                documento.Tipologia = t;
            }
            documento.Allegati = await GetAllegati(schDoc, instanceDoc, infoUtente);
            documento.TipoRichiesta = instanceDoc.TYPE_REQUEST;

            return SerializeObject<DocsPaVO.InstanceAccess.Metadata.Document>(documento);
        }

        private async Task<DocsPaVO.InstanceAccess.Metadata.CampoTipologia[]> GetCampiTipologia(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            if (template == null)
                return null;

            List<DocsPaVO.InstanceAccess.Metadata.CampoTipologia> ctlist = new List<DocsPaVO.InstanceAccess.Metadata.CampoTipologia>();

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
            {
                DocsPaVO.InstanceAccess.Metadata.CampoTipologia ct = new DocsPaVO.InstanceAccess.Metadata.CampoTipologia
                {
                    NomeCampo = oggettoCustom.DESCRIZIONE,
                    ValoreCampo = await this.GetValoreOggettoCustom(oggettoCustom)
                };
                ctlist.Add(ct);
            }
            return ctlist.ToArray();
        }

        private async Task<string> GetValoreOggettoCustom(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            string riga = string.Empty;
            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
            {
                case "Corrispondente":
                    DocsPaVO.utente.Corrispondente corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(oggettoCustom.VALORE_DATABASE))).output;
                    if (corr != null && corr.descrizione != null)
                        riga += corr.descrizione;
                    break;

                case "Contatore":
                    string contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            contatore = contatore.Replace("ANNO", oggettoCustom.ANNO);
                            contatore = contatore.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = ( await this._mediator.Send( new Application.Requests.GetRegistroBySistemId(oggettoCustom.ID_AOO_RF))).output;
                                if (reg != null)
                                {
                                    contatore = contatore.Replace("RF", reg.codRegistro);
                                    contatore = contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            contatore = string.Empty;
                        }
                    }
                    else
                    {
                        contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += contatore;
                    break;

                case "CasellaDiSelezione":
                    string selezione = string.Empty;
                    foreach (string sel in oggettoCustom.VALORI_SELEZIONATI)
                    {
                        if (sel != null && sel != "")
                            selezione += sel + " - ";
                    }
                    if (selezione != null && selezione != string.Empty)
                        riga += selezione.Substring(0, selezione.Length - 2);
                    break;
                //controllare bene
                case "ContatoreSottocontatore":
                    string s_contatore = string.Empty;
                    if (oggettoCustom.FORMATO_CONTATORE != "")
                    {
                        s_contatore = oggettoCustom.FORMATO_CONTATORE;
                        if (oggettoCustom.VALORE_DATABASE != null && oggettoCustom.VALORE_DATABASE != "")
                        {
                            s_contatore = s_contatore.Replace("ANNO", oggettoCustom.ANNO);
                            s_contatore = s_contatore.Replace("CONTATORE", (oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE));
                            if (oggettoCustom.ID_AOO_RF != null && oggettoCustom.ID_AOO_RF != "0")
                            {
                                DocsPaVO.utente.Registro reg = (await this._mediator.Send(new Application.Requests.GetRegistroBySistemId(oggettoCustom.ID_AOO_RF))).output;
                                if (reg != null)
                                {
                                    s_contatore = s_contatore.Replace("RF", reg.codRegistro);
                                    s_contatore = s_contatore.Replace("AOO", reg.codRegistro);
                                }
                            }
                        }
                        else
                        {
                            s_contatore = string.Empty;
                        }
                    }
                    else
                    {
                        s_contatore = oggettoCustom.VALORE_DATABASE;
                    }

                    riga += s_contatore;
                    break;


                default:
                    riga += oggettoCustom.VALORE_DATABASE;
                    break;
            }
            return riga;
        }


        private async Task<DocsPaVO.InstanceAccess.Metadata.Allegato[]> GetAllegati(DocsPaVO.documento.SchedaDocumento schDoc, InstanceAccessDocument doc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            if (schDoc.allegati == null)
                return null;
            if (schDoc.allegati.Count() == 0)
                return null;

            List<DocsPaVO.InstanceAccess.Metadata.Allegato> lstAll = new List<DocsPaVO.InstanceAccess.Metadata.Allegato>();
            foreach (object a in schDoc.allegati)
            {
                DocsPaVO.InstanceAccess.Metadata.Allegato allegato = new DocsPaVO.InstanceAccess.Metadata.Allegato();
                DocsPaVO.documento.Allegato all = a as DocsPaVO.documento.Allegato;
                if (all != null && (from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(all.docNumber) select att.ENABLE).FirstOrDefault())
                {
                    allegato.Descrizione = all.descrizione;
                    allegato.ID = all.docNumber;
                    //allegato.Tipo = "manuale"; //Cablato , per ora.. poi si vedrà
                    string tipoAllegato = "";
                    switch (all.TypeAttachment)
                    {
                        case 1:
                            tipoAllegato = "Allegato Utente";
                            break;
                        case 2:
                            tipoAllegato = "Allegato PEC";
                            break;
                        case 3:
                            tipoAllegato = "Allegato IS";
                            break;
                        case 4:
                            tipoAllegato = "Allegato Esterno";
                            break;
                        default:
                            tipoAllegato = "Non specificato";
                            break;
                    }
                    allegato.Tipo = tipoAllegato;

                    if (!string.IsNullOrEmpty(all.fileSize) && Convert.ToInt32(all.fileSize) > 0)
                    {
                        FileDocumento fd = ( await this._mediator.Send(new Application.Requests.GetFileDocument(all, infoUtente)) ).output;
                        allegato.File = new();
                    }
                    lstAll.Add(allegato);
                }
            }

            return lstAll.ToArray();

        }
        private string ReplaceInvalidCharFile(string fileName)
        {
            string resultPath = fileName;
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }
        private async Task<bool> SaveFile(DocsPaVO.documento.FileDocumento fileDoc, string instancePath, DocsPaVO.InstanceAccess.InstanceAccessDocument doc, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.documento.FileRequest objFileRequest, bool isDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string err = string.Empty;
            string fullName = string.Empty;

            //nuova struttura directory!!!!!!!!!!!
            string rootXml = Path.Combine(instancePath, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;
            if (doc.INFO_PROJECT != null)
            {
                instancePath = Path.Combine(instancePath, Path.Combine("Fascicoli", ReplaceInvalidChar(doc.INFO_PROJECT.CODE_PROJECT)));
                pathDoc = Path.Combine(instancePath, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
            }

            //Creo una sottocartella (nominata con il DocNumber) per contenere anche gli allegati ed i metadati
            string pathContainsAttach = Path.Combine(instancePath, Path.Combine("Documenti", schDoc.docNumber));

            //Inizializzo il path relativo del supporto destinato alla Conservazione ed elimino i caratteri speciali
            pathDoc = ReplaceInvalidChar(pathDoc);
            string path_supporto = '\u005C'.ToString() + pathDoc;

            //se è un allegato devo creare la sottocartella per gli allegati
            bool isAll = !isDoc;
            if (isAll)
            {
                pathContainsAttach = Path.Combine(pathContainsAttach, "Allegati");
                path_supporto = Path.Combine(path_supporto, "Allegati");
            }

            //normalizzo il percorso eliminando i caratteri speciali
            pathContainsAttach = ReplaceInvalidChar(pathContainsAttach);

            if (!Directory.Exists(pathContainsAttach))
            {
                Directory.CreateDirectory(pathContainsAttach);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            rootXml = ReplaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                Directory.CreateDirectory(rootXml);
            }
            string fileName = string.Empty;
            if (fileDoc != null)
            {
                fileName = ReplaceInvalidCharFile(fileDoc.nomeOriginale);

                fullName = pathContainsAttach + '\u005C'.ToString() + fileName;
                path_supporto = Path.Combine(path_supporto, fileName);
                //Se è un allegato oppure è un documento abilitato all'interno dell'istanza, salvo il file
                if (!isDoc || doc.ENABLE)
                {
                    FileStream file = null;
                    try
                    {
                        file = File.Create(fullName);
                        file.Write(fileDoc.content, 0, fileDoc.length);
                    }
                    catch (Exception e)
                    {
                        err = "Errore nella gestione del salvataggio del File " + e.Message;
                        this._logger.LogDebug(err);
                        result = false;
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Flush();
                            file.Close();
                        }
                    }
                }
            }
            //Se è il documento principale salvo i metadati su file XML

            if (isDoc)
            {
                try
                {
                    string nameXml = string.IsNullOrEmpty(fileName) ? "fileMetadati" : fileName;
                    string xmlIstance = await this.XmlDoc(fileDoc, schDoc, objFileRequest, doc, infoUtente);
                    SaveMetadatiString(Path.Combine(pathContainsAttach, nameXml), xmlIstance);
                }
                catch (Exception eXml)
                {
                    err = "Errore nella scrittura del file XML dei metadati." + eXml.Message;
                    this._logger.LogDebug(err);
                    result = false;
                }
            }
            return result;
        }
        

        private async Task<int> PutDocumenti(string instancePath, DocsPaVO.InstanceAccess.InstanceAccessDocument doc, bool isDichiarazioneConformita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int result = 1;
            string err = string.Empty;

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER, doc.INFO_DOCUMENT.DOCNUMBER))).output;
            DocsPaVO.documento.FileDocumento fd = null;
            string dataCreazione_or_proto = string.Empty;
            if (sch.protocollo != null && !string.IsNullOrEmpty(sch.protocollo.dataProtocollazione))
            {
                dataCreazione_or_proto = sch.protocollo.dataProtocollazione;
            }
            else
            {
                CultureInfo culture = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                dataCreazione_or_proto = DateTime.ParseExact(sch.dataCreazione, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToShortDateString();
            }

            try
            {
                infoDoc.Add(doc.ID_INSTANCE_ACCESS_DOCUMENT, dataCreazione_or_proto);

                //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                if (sch.documenti != null && sch.documenti[0] != null)
                {
                    try
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];
                        fr.repositoryContext = null;
                        if (Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                        {
                            //da modificare
                            fd = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato(fr, infoUtente))).output;
                            if (fd == null)
                            {
                                throw new Exception(Resources.errorFindingFile);
                            }
                        }
                        //salvataggio nella relativa cartella di conservazione del file
                        if (!await SaveFile(fd, instancePath, doc, sch, fr, true, infoUtente))
                            throw new Exception(Resources.errorSavingFile);
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception:ex,message: ex.Message);
                        result = 0;
                    }
                }
                //Recupero tutti gli allegati associati al documento corrente
                if (!isDichiarazioneConformita)
                {
                    for (int i = 0; sch.allegati != null && i < sch.allegati.Count(); i++)
                    {
                        DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];
                        if (doc.ATTACHMENTS.Find(a => a.ID_ATTACH.Equals(documentoAllegato.docNumber) && a.ENABLE) != null)
                        {
                            DocsPaVO.documento.FileDocumento fdAll = null;
                            if (Int32.Parse(documentoAllegato.fileSize) > 0)
                            {
                                try
                                {
                                    DocsPaVO.documento.FileRequest frAll = (DocsPaVO.documento.FileRequest)sch.allegati[i];
                                    //fdAll = BusinessLogic.Documenti.FileManager.getFile(frAll, infoUtente);
                                    fdAll = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato(frAll, infoUtente))).output;
                                    if (fdAll == null)
                                        throw new Exception();

                                    //salvataggio nella relativa cartella di conservazione del file dell'allegato
                                    if (! await SaveFile(fdAll, instancePath, doc, sch, frAll, false, infoUtente))
                                        throw new Exception();
                                }
                                catch (Exception ex)
                                {
                                    this._logger.LogError(Resources.errorFindingFile);
                                    result = 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(Resources.errorFindingFile);
                result = 0;
            }
            return result;
        }

        private string SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }

        private async Task<string> XmlInstance(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente)
        {
            var istanza = new DocsPaVO.InstanceAccess.Metadata.Instance();

            istanza.IdIstanza = instance.ID_INSTANCE_ACCESS;
            istanza.Descrizione = instance.DESCRIPTION;
            if (instance.RICHIEDENTE != null)
            {
                istanza.RichiedenteIstanza = new DocsPaVO.InstanceAccess.Metadata.Richiedente
                {
                    CodiceRichiedente = instance.RICHIEDENTE.codiceRubrica,
                    DescrizioneRichiedente = instance.RICHIEDENTE.descrizione
                };
            }
            if (!instance.CREATION_DATE.Equals(DateTime.MinValue))
            {
                istanza.DataRichiesta = instance.CREATION_DATE.ToShortDateString();
            }
            if (!instance.CLOSE_DATE.Equals(DateTime.MinValue))
                istanza.DataChiusura = instance.CLOSE_DATE.ToShortDateString();
            if (!string.IsNullOrEmpty(instance.ID_DOCUMENT_REQUEST))
            {
                DocsPaVO.documento.SchedaDocumento schDoc = (await this._mediator.Send( new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumento(infoUtente, instance.ID_DOCUMENT_REQUEST, instance.ID_DOCUMENT_REQUEST))).output;
                istanza.ProtocolloRichiesto = new DocsPaVO.InstanceAccess.Metadata.ProtocolloRichiesto()
                {
                    Oggetto = schDoc.oggetto.descrizione,
                    Segnatura = (schDoc.protocollo != null && !string.IsNullOrEmpty(schDoc.protocollo.segnatura)) ? schDoc.protocollo.segnatura : string.Empty,
                    DataCreazione = schDoc.dataCreazione,
                    IdDocumento = schDoc.docNumber
                };

            }
            istanza.Note = instance.NOTE;

            return this.SerializeObject<DocsPaVO.InstanceAccess.Metadata.Instance>(istanza);
        }
        private bool SaveMetadatiString(string fullName, string contenutoXML)
        {
            bool result = false;
            TextWriter textWr = null;
            XmlTextWriter XmlWr = null;
            try
            {
                string pathXml = fullName + ".xml";
                textWr = new StreamWriter(pathXml, false, Encoding.UTF8);
                XmlWr = new XmlTextWriter(textWr);


                if (!string.IsNullOrEmpty(contenutoXML))
                {
                    XmlWr.WriteRaw(contenutoXML);
                    result = true;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex.Message);
            }
            finally
            {
                if (textWr != null)
                {
                    XmlWr.Flush();
                    textWr.Flush();
                    XmlWr.Close();
                    textWr.Close();
                }
            }
            return result;
        }
        private void CreateStaticFiles(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.activeFname),Resources.active);
            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.bgBodyFname),Resources.bg_body);
            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.colSepFname),Resources.col_sep);
            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.lockFname),Resources._lock);
            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.logoFname),Resources.logo);
            System.IO.File.WriteAllBytes(string.Format(Resources.sFormat,path,Resources.mainFname),Resources.main);
        }
        private async Task<bool> PrepareInstanceZip(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string? idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            infoDoc = new();
            string rootPath = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads");
            

            string istancePath = string.Empty;
            string baseFolder = string.Empty;
            string uploadSuffix = string.Empty; 
            try
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Application.Requests.GetTemplateInstanceAccess(infoUtente))).output;
                if (!string.IsNullOrEmpty(rootPath))
                {
                    DocsPaVO.amministrazione.InfoAmministrazione infoAmm = null;
                    // Inizio a creare l'istanza per il download
                    infoAmm = (await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(idAmministrazione))).output;

                    istancePath = Path.Combine(rootPath, infoAmm.Codice);
                    istancePath = Path.Combine(istancePath, instanceAccess.ID_INSTANCE_ACCESS).PathAsUnixPath();
                    uploadSuffix = Path.Combine(uploadSuffix, infoAmm.Codice, instanceAccess.ID_INSTANCE_ACCESS).PathAsUnixPath();
                    baseFolder = istancePath;
                    istancePath = Path.Combine(istancePath, Guid.NewGuid().ToString()).PathAsUnixPath();

                    if (Directory.Exists(istancePath))
                    {
                        Directory.Delete(istancePath, true);
                    }
                    this.CreateStaticFiles(Path.Combine(istancePath,"static"));

                    if (instanceAccess != null && instanceAccess.DOCUMENTS != null && instanceAccess.DOCUMENTS.Count > 0)
                    {
                        string currPrj = string.Empty;
                        //cicla per tutti gli elementi delll'istanza 
                        for (int j = 0; j < instanceAccess.DOCUMENTS.Count; j++)
                        {
                            InstanceAccessDocument item = instanceAccess.DOCUMENTS[j];
                            if (item.INFO_PROJECT != null && item.INFO_PROJECT.ID_PROJECT != currPrj)
                            {
                                currPrj = item.INFO_PROJECT.ID_PROJECT;
                                {
                                    string pf = Path.Combine(istancePath, Path.Combine("Fascicoli", ReplaceInvalidChar(item.INFO_PROJECT.CODE_PROJECT)));
                                    pf = ReplaceInvalidChar(pf);
                                    if (!Directory.Exists(pf))
                                        Directory.CreateDirectory(pf);
                                }

                            }

                            //Mi creo il file XML dell'istanza
                            string xmlIstance = await this.XmlInstance(instanceAccess, infoUtente);
                            this.SaveMetadatiString(Path.Combine(istancePath, "dati_istanza"),xmlIstance);
                            bool isDichiarazioneConformita = template != null && (item.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE));


                            if (await PutDocumenti(istancePath, item, isDichiarazioneConformita, infoUtente) != 1)
                            {
                                result = false;
                                break;
                            }

                            
                        }
                    }
                    if (result)
                    {
                        await IndexFolderHtml(instanceAccess, infoUtente, istancePath, template);
                        // zip creation and unused folder distruction
                        if (!await ZipFolder(instanceAccess.ID_INSTANCE_ACCESS, istancePath, baseFolder, infoUtente))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if (Directory.Exists(istancePath))
                        {
                            Directory.Delete(istancePath, true);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
                if (Directory.Exists(istancePath))
                {
                    Directory.Delete(istancePath, true);
                }
                if (File.Exists(istancePath + ".ZIP"))
                {
                    File.Delete(istancePath + ".ZIP");
                }
            }
            return result;

        }

        private async Task<bool> ZipFolder(string idIstanza, string sourceDirectory,string baseDir, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            string zipFilePath = Path.Combine(baseDir, idIstanza) + ".ZIP";
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            try
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath.PathAsUnixPath());
                
                if (Directory.Exists(sourceDirectory))
                {
                    Directory.Delete(sourceDirectory,true);
                }
                result = true;
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception:ex,message:ex.Message);
                result = false;
            }
            return result;

        }
        private async Task IndexFolderHtml(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente, string htmlPath, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            bool res = false;
            string[] htmlPathArray = new string[7];
            string[] titolo = new string[7];
            TextWriter twriter = null;
            titolo[0] = string.Empty;

            htmlPathArray[0] = Path.Combine(htmlPath, "index.html");

            //nuova struttura directory!!!!!!!!!!!
            htmlPath = Path.Combine(htmlPath, "html");
            if (!Directory.Exists(htmlPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(htmlPath);
            }

            titolo[1] = "Ricerca per numero documento";
            htmlPathArray[1] = Path.Combine(htmlPath, "RicDocNumber.html");
            res = CreateHTML(instanceAccess, htmlPathArray[1], "docNumber", titolo[1], template);
            titolo[2] = "Ricerca per segnatura o numero di documento";
            htmlPathArray[2] = Path.Combine(htmlPath, "RicSegnatura.html");
            res = CreateHTML(instanceAccess, htmlPathArray[2], "segnatura", titolo[2], template);
            titolo[3] = "Ricerca per descrizione oggetto";
            htmlPathArray[3] = Path.Combine(htmlPath, "RicOggetto.html");
            res = CreateHTML(instanceAccess, htmlPathArray[3], "oggetto", titolo[3], template);
            titolo[4] = "Ricerca per codice fascicolo";
            htmlPathArray[4] = Path.Combine(htmlPath, "RicFascicolo.html");
            res = CreateHTML(instanceAccess, htmlPathArray[4], "fascicolo", titolo[4], template);
            titolo[5] = "Ricerca per data di creazione o protocollazione";
            htmlPathArray[5] = Path.Combine(htmlPath, "RicData.html");
            res = CreateHTML(instanceAccess, htmlPathArray[5], "data", titolo[5], template);
            titolo[6] = "Ricerca per nome file";
            htmlPathArray[6] = Path.Combine(htmlPath, "RicFileName.html");
            res = CreateHTML(instanceAccess, htmlPathArray[6], "fileName", titolo[6], template);

            var infoAmm = (await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione))).output;
            try
            {
                if (!File.Exists(htmlPathArray[0]))
                {
                    twriter = new StreamWriter(htmlPathArray[0], false, Encoding.UTF8);
                    twriter.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                    twriter.WriteLine("<HTML>");
                    twriter.WriteLine("<HEAD>");
                    twriter.WriteLine("<TITLE> INDICE RICERCHE </TITLE>");
                    twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"static/main.css\" />");
                    twriter.WriteLine("</HEAD>");
                    twriter.WriteLine("<BODY>");
                    twriter.WriteLine("<div class=\"site-container\">");
                    twriter.WriteLine("<div class=\"header\">");
                    twriter.WriteLine("<img src=\"static/logo.jpg\" />");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"site-title\">");
                    twriter.WriteLine("<h3>Tutti i file contenuti all’interno di questo supporto sono UFFICIALI</h3>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"body-content\">");
                    twriter.WriteLine("<h4 class=\"title-indicizza\">Indicizza per: </h4>");
                    twriter.WriteLine("<div class=\"col-sx\">");
                    twriter.WriteLine("<ul class=\"index\">");

                    for (int i = 1; i < htmlPathArray.Length; i++)
                    {
                        string path_relativo = "." + '\u002F'.ToString() + "html" + '\u002F'.ToString() + Path.GetFileName(htmlPathArray[i]);
                        twriter.WriteLine("<li><a target=\"content\" href='" + path_relativo + "'>" + titolo[i] + "</a></li>");
                    }

                    twriter.WriteLine("</ul>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"col-dx\">");
                    twriter.WriteLine("<iframe name=\"content\" class=\"content-frame\"></iframe>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"cl\"></div>");
                    twriter.WriteLine("<div class=\"sep\"></div>");
                    twriter.WriteLine("<table class=\"footer-table\">");
                    twriter.WriteLine(String.Format("<tr><th>Ente:</th><td>{0}</td></tr>", infoAmm.Descrizione + " - " + infoAmm.Codice));
                    twriter.WriteLine(String.Format("<tr><th>Istanza numero:</th><td>{0}</td></tr>", instanceAccess.ID_INSTANCE_ACCESS));
                    twriter.WriteLine(String.Format("<tr><th>Descrizione:</th><td>{0}</td></tr>", instanceAccess.DESCRIPTION));
                    twriter.WriteLine(String.Format("<tr><th>Richiedente:</th><td>{0}</td></tr>", instanceAccess.RICHIEDENTE.descrizione));
                    twriter.WriteLine(String.Format("<tr><th>Data richiesta:</th><td>{0}</td></tr>", instanceAccess.CREATION_DATE));
                    twriter.WriteLine(String.Format("<tr><th>Id documento  richiesta:</th><td>{0}</td></tr>", instanceAccess.ID_DOCUMENT_REQUEST));
                    twriter.WriteLine(String.Format("<tr><th>Note istanza:</th><td>{0}</td></tr>", instanceAccess.NOTE));
                    twriter.WriteLine("</table>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<script>");
                    twriter.WriteLine("var elems = document.getElementsByTagName(\"a\");");
                    twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
                    twriter.WriteLine("elems[i].onclick = function () {");
                    twriter.WriteLine("setActive(this);");
                    twriter.WriteLine("};");
                    twriter.WriteLine("}");
                    twriter.WriteLine("function setActive(elem){");
                    twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
                    twriter.WriteLine("elems[i].className = \"\";");
                    twriter.WriteLine("}");
                    twriter.WriteLine("elem.className = \"active\";");
                    twriter.WriteLine("}");
                    twriter.WriteLine("</script>");
                    twriter.WriteLine("</BODY>");
                    twriter.WriteLine("</HTML>");

                }
            }
            catch (Exception exHtml)
            {
                string err = "Errore nella creazione del main index Html : " + exHtml.Message;
            }
            finally
            {
                if (twriter != null)
                {
                    twriter.Flush();
                    twriter.Close();
                }
            }
        }

        private bool CreateHTML(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, string HtmlPath, string tipoRic, string title, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            List<InstanceAccessDocument> listDocuments = instanceAccess.DOCUMENTS;
            bool result = true;
            TextWriter twriter = null;
            string paragrafo = string.Empty;

            if (listDocuments != null && listDocuments.Count() > 0)
            {
                try
                {
                    //Ordino la lista in base al tipo di ricerca
                    switch (tipoRic)
                    {
                        case "docNumber":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.DOCNUMBER descending select d).ToList();
                            break;
                        case "segnatura":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.NUMBER_PROTO descending, d.INFO_DOCUMENT.DOCNUMBER descending select d).ToList();
                            break;
                        case "oggetto":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.OBJECT ascending select d).ToList();
                            break;
                        case "fascicolo":
                            listDocuments = listDocuments.OrderBy(x => x.INFO_PROJECT != null).ThenByDescending(x => x.INFO_PROJECT.CODE_PROJECT).ToList();
                            break;
                        case "data":
                            var appoggio = (from d in listDocuments
                                            select new
                                            {
                                                doc = d,
                                                data = infoDoc[d.ID_INSTANCE_ACCESS_DOCUMENT]
                                            });
                            
                            listDocuments = (from a in appoggio.OrderByDescending(x => x.data) select a.doc).ToList();
                            break;
                        case "fileName":
                            listDocuments = listDocuments.OrderBy(d => d.INFO_DOCUMENT.FILE_NAME).Select(d => d).ToList();
                            break;
                    }

                    if (!File.Exists(HtmlPath))
                    {
                        twriter = new StreamWriter(HtmlPath, false, Encoding.UTF8);
                        twriter.WriteLine("<HTML>");
                        twriter.WriteLine("<HEAD>");
                        twriter.WriteLine("<TITLE> Ricerca per " + title + "  </TITLE>");
                        twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"../static/main.css\" />");
                        twriter.WriteLine("</HEAD>");
                        twriter.WriteLine("<BODY class=\"content\">");
                        twriter.WriteLine("<h3 align=center> Ricerca per " + title + "  </h3>");
                        twriter.WriteLine("<br><br>");
                        twriter.WriteLine("<ol type='1'>");

                        ArrayList arrayTipologia = new ArrayList();

                        for (int i = 0; i < listDocuments.Count; i++)
                        {
                            bool isDichiarazioneConformita = template != null && (listDocuments[i].INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE));
                            //i tag <ul> e <li> lo chiudo e lo riapro in questo punto dopo aver controllato se il paragrafo
                            //è diverso da quello appena scritto!!!
                            //SE è IL PRIMO ELEMENTO NON DEVO METTERE I TAG DI CHIUSURA ALL'INIZIO!!!!!
                            string valoreRicerca = ValoreRicerca(listDocuments[i], tipoRic);
                            if (i > 0 && paragrafo != valoreRicerca)
                            {
                                twriter.WriteLine("</ul>");
                                twriter.WriteLine("</li>");
                                twriter.WriteLine("<li><b>" + valoreRicerca + "</b>");
                                twriter.WriteLine("<ul type='disc'>");
                                paragrafo = valoreRicerca;
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    twriter.WriteLine("<li><b>" + valoreRicerca + "</b>");
                                    twriter.WriteLine("<ul type='disc'>");
                                    paragrafo = valoreRicerca;
                                }
                            }
                            string pathFasc = string.Empty;
                            if (listDocuments[i].INFO_PROJECT != null && !string.IsNullOrEmpty(listDocuments[i].INFO_PROJECT.CODE_PROJECT))
                            {
                                pathFasc = @"/Fascicoli/" + listDocuments[i].INFO_PROJECT.CODE_PROJECT;
                            }
                            string fileNameDocument = listDocuments[i].INFO_DOCUMENT.FILE_NAME.Replace('\u005C', '\u002F');
                            if (string.IsNullOrEmpty(fileNameDocument) || !listDocuments[i].ENABLE)
                            {
                                twriter.WriteLine("<li>" + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "</a></li>");
                            }
                            else
                            {
                                twriter.WriteLine("<li><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + fileNameDocument + "'>"
                                    + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + fileNameDocument + "</a></li>");
                            }
                            twriter.WriteLine("<li type='circle'><b>File metadati: </b><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + (string.IsNullOrEmpty(fileNameDocument) ? "fileMetadati" : fileNameDocument) + ".xml'>" +
                                pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + (string.IsNullOrEmpty(fileNameDocument) ? "fileMetadati" : fileNameDocument) + ".xml</a></li>");
                            if (listDocuments[i].ATTACHMENTS != null && listDocuments[i].ATTACHMENTS.Count > 0 && !isDichiarazioneConformita)
                            {
                                for (int j = 0; j < listDocuments[i].ATTACHMENTS.Count; j++)
                                {
                                    if (listDocuments[i].ATTACHMENTS[j].ENABLE)
                                    {
                                        twriter.WriteLine("<ul type='square'>");
                                        if (!string.IsNullOrEmpty(listDocuments[i].ATTACHMENTS[j].FILE_NAME))
                                        {
                                            twriter.WriteLine("<li><b>File allegato: </b><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/Allegati/" + listDocuments[i].ATTACHMENTS[j].FILE_NAME.Replace('\u005C', '\u002F') + "'>"
                                              + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/Allegati/" + listDocuments[i].ATTACHMENTS[j].FILE_NAME + "</a></li>");

                                        }
                                        twriter.WriteLine("</ul>");
                                    }
                                }
                            }

                        }
                        twriter.WriteLine("</ol>");
                        twriter.WriteLine("</BODY>");
                        twriter.WriteLine("</HTML>");
                    }
                }
                catch (Exception exHtml)
                {
                    string err = "Errore nella creazione della pagina " + HtmlPath + " : " + exHtml.Message;
                    result = false;
                }
                finally
                {
                    if (twriter != null)
                    {
                        twriter.Flush();
                        twriter.Close();
                    }
                }
            }
            return result;
        }

        private async Task<bool> CreateDownloadInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                await this.UpdateInstanceStartDownload(instanceAccess, infoUtente, "1");
                result = await PrepareInstanceZip(instanceAccess, infoUtente);
                if (result)
                {
                    await this.UpdateInstanceStartDownload(instanceAccess, infoUtente, "2");
                }
                else
                {
                    await this.UpdateInstanceStartDownload(instanceAccess, infoUtente, "0");
                }

            }
            catch(Exception ex)
            {
                await this.UpdateInstanceStartDownload(instanceAccess, infoUtente, "0");
                result = false;
                this._logger.LogWebMethodError(ex);
            }
            
            return result;
        }
        private string ValoreRicerca(InstanceAccessDocument doc, string tipoRic)
        {
            string paragrafo = string.Empty;

            switch (tipoRic)
            {
                case "docNumber":
                    paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    break;
                case "segnatura":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.NUMBER_PROTO))
                    {
                        paragrafo = doc.INFO_DOCUMENT.NUMBER_PROTO;
                    }
                    else
                    {
                        paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    }
                    break;
                case "oggetto":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.OBJECT))
                    {
                        paragrafo = doc.INFO_DOCUMENT.OBJECT;
                    }
                    else
                    {
                        paragrafo = "Descrizione oggetto mancante";
                    }
                    break;
                case "fascicolo":
                    if (doc.INFO_PROJECT != null && !string.IsNullOrEmpty(doc.INFO_PROJECT.CODE_PROJECT))
                    {
                        paragrafo = doc.INFO_PROJECT.CODE_PROJECT;
                    }
                    else
                    {
                        paragrafo = "Documenti non inseriti tramite fascicolo";
                    }
                    break;
                case "data":
                    paragrafo = infoDoc[doc.ID_INSTANCE_ACCESS_DOCUMENT].ToString();
                    break;
                case "fileName":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.FILE_NAME))
                    {
                        paragrafo = doc.INFO_DOCUMENT.FILE_NAME;
                    }
                    else
                    {
                        paragrafo = "nessun documento acquisito";
                    }
                    break;

                default:
                    paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    break;
            }

            return paragrafo;
        }
        private async Task<string> GetIdProfileDownload(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string idProfile = string.Empty;

            idProfile = await this._dbContext.InstanceAccessEntities.AsNoTracking().Where(ia => ia.SYSTEM_ID == idInstanceAccess.AsLong()).Select(ia => ia.ID_PROFILE_DOWNLOAD.ToString()).FirstOrDefaultAsync();

            return idProfile;
        }

        private async Task UpdateIdProfileDownload(string idProfile, string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            InstanceAccessEntity? iaToUpdate = await this._dbContext.InstanceAccessEntities.FirstOrDefaultAsync( ia => ia.SYSTEM_ID == idInstanceAccess.AsLong());
            if(iaToUpdate != null)
            {
                iaToUpdate.ID_PROFILE_DOWNLOAD = idProfile.AsLong();
                await ((DbContext)this._dbContext).SaveChangesAsync();
            }


        }

        private async Task<bool> CreateDownloadInstanceAccess(InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            bool result = false;
            string idProfile = string.Empty;


            idProfile = await this.GetIdProfileDownload(instanceAccess.ID_INSTANCE_ACCESS, infoUtente);

            if (string.IsNullOrEmpty(idProfile) || idProfile.Equals("0"))
            {
                DocsPaVO.documento.SchedaDocumento newDoc = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.NewSchedaDocumento(infoUtente))).output;
                newDoc.tipoProto = "G";
                newDoc.oggetto = new DocsPaVO.documento.Oggetto() { descrizione = "Preparazione download istanza ID: " + instanceAccess.ID_INSTANCE_ACCESS };
                newDoc = (await this._mediator.Send(new Application.Requests.DocumentoAddDocGrigia(newDoc, infoUtente, ruolo))).output;
                idProfile = newDoc.systemId;
                await this.UpdateIdProfileDownload(idProfile, instanceAccess.ID_INSTANCE_ACCESS, infoUtente);
            }
            result = await this.CreateDownloadInstanceAccess(instanceAccess, infoUtente);

            return result;
        }

        public AsyncCreateDownloadHandler(
            ILogger<AsyncCreateDownloadHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            IConfigurationService configurationService,
            IClaimsPrincipalService claimsPrincipalService
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<AsyncCreateDownloadResult> Handle(AsyncCreateDownloadRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await this.CreateDownloadInstanceAccess(request.instanceAccess,request.infoUtente,request.ruolo);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new();
        }
    }
}
