// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
using DocsPaVO.Modelli;
using DocsPaVO.Note;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Text;

using FascicolazioneGetFascicoliDaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFascicoliDaDoc;
using GetDocumentModelAsXmlRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDocumentModelAsXml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentModelAsXml
{
    public class GetDocumentModelAsXmlHandler : IRequestHandler<GetDocumentModelAsXmlRequest, GetDocumentModelAsXmlResult>
    {
        #region Public Members

        public GetDocumentModelAsXmlHandler(
            ILogger<GetDocumentModelAsXmlHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IConfigurationService configurationService)

        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._configurationService = configurationService;
        }

        public async Task<GetDocumentModelAsXmlResult> Handle(GetDocumentModelAsXmlRequest request, CancellationToken cancellationToken)
        {
            var modelResponse = new DocsPaVO.Modelli.ModelResponse()
            {
                DocumentId = request.modelRequest.DocumentId
            };

            try
            {
                modelResponse.ProcessorInfo = await this.EnsureClientModelProcessorEntity(request);

                var getDettaglioDocumentoResult = await this._mediator.Send(new Requests.DocumentoGetDettaglioDocumento(request.modelRequest.UserInfo, request.modelRequest.DocumentId, request.modelRequest.DocumentId));

                var schedaDocumento = getDettaglioDocumentoResult.output;

                modelResponse.DocumentModel = await this.LoadModel(request, schedaDocumento);

                if (request.modelRequest.ModelType == MODEL_VERSION_2)
                {
                    // Recupero del contenuto della versione corrente del documento
                    DocsPaVO.documento.FileRequest previousVersion = (DocsPaVO.documento.FileRequest)schedaDocumento.documenti[0];

                    DocsPaVO.documento.FileDocumento previousVersionFile = (await this._mediator.Send(new Requests.DocumentoGetFile(previousVersion, request.modelRequest.UserInfo))).output;

                    if (previousVersionFile != null)
                    {
                        // Il contenuto del file della versione corrente del documento
                        // viene inserito nella propriet� contentreplacement
                        // per essere inserito nel contenuto del nuovo documento da elaborare
                        DocsPaVO.Modelli.IncludeSection section = new DocsPaVO.Modelli.IncludeSection();
                        section.Begin = "Start_Body";
                        section.End = "End_Body";
                        section.File.FileName = Path.GetFileName(previousVersion.fileName);
                        section.File.Content = previousVersionFile.content;
                        modelResponse.IncludeSections = new DocsPaVO.Modelli.IncludeSection[1] { section };
                    }
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                modelResponse.Exception = pi3Ex.Message;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                modelResponse.Exception = ex.Message;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new GetDocumentModelAsXmlResult(
                DocsPaVO.Modelli.ModelResponse.ToXml(modelResponse));
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentModelAsXmlHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        protected readonly IConfigurationService _configurationService;

        protected const string MODEL_VERSION_1 = "1";
        protected const string MODEL_VERSION_2 = "2";
        protected const string MODEL_ATTATCHMENT = "A";
        protected const string MODEL_STAMPA_RICEVUTA = "STAMPA_RICEVUTA";

        public struct DocumentCommonFields
        {
            public const string OGGETTO = "Oggetto";
            public const string DATA_CREAZIONE = "Data creazione";
            public const string ID_DOCUMENTO = "ID Documento";
            public const string NOTE = "Note";
            public const string TIPOLOGIA = "Tipologia";
            public const string CREATORE = "Creatore";
            public const string RUOLO_CREATORE = "Ruolo creatore";
            public const string UO_CREATORE = "UO creatore";
            public const string INDIRIZZO_UO_CREATORE = "Indirizzo uo creatore";
            public const string CITTA_UO_CREATORE = "Citta' uo creatore";
            public const string CAP_UO_CREATORE = "Cap uo creatore";
            public const string NAZIONE_UO_CREATORE = "Nazione uo creatore";
            public const string PROVINCIA_UO_CREATORE = "Provincia uo creatore";
            public const string TEL1_UO_CREATORE = "Telefono1 uo creatore";
            public const string TEL2_UO_CREATORE = "Telefono2 uo creatore";
            public const string FAX_UO_CREATORE = "Fax uo creatore";
            public const string NUM_PROTOCOLLO = "Numero protocollo";
            public const string SEGNATURA = "Segnatura";
            public const string DATA_PROTOCOLLO = "Data protocollo";
            public const string REGISTRO = "Registro";
            public const string CODICE_REGISTRO = "Codice registro";
            public const string PROTOCOLLATORE = "Protocollatore";
            public const string RUOLO_PROTOCOLLATORE = "Ruolo protocollatore";
            public const string UO_PROTOCOLLATORE = "UO protocollatore";
            public const string INDIRIZZO_UO_PROT = "Indirizzo uo prot";
            public const string CITTA_UO_PROT = "Citta' uo prot";
            public const string CAP_UO_PROT = "Cap uo prot";
            public const string NAZIONE_UO_PROT = "Nazione uo prot";
            public const string PROVINCIA_UO_PROT = "Provincia uo prot";
            public const string TEL1_UO_PROT = "Telefono1 uo prot";
            public const string TEL2_UO_PROT = "Telefono2 uo prot";
            public const string FAX_UO_PROT = "Fax uo prot";
            public const string MITTENTE = "Mittente";
            public const string MITTENTE_INDIRIZZO = "Mittente$indirizzo";
            public const string MITTENTE_TELEFONO = "Mittente$telefono";
            public const string MITTENTE_INDIRIZZO_TELEFONO = "Mittente$indirizzo$telefono";
            public const string MITTENTI_MULTIPLI = "Mittenti_multipli";
            public const string MITTENTI_MULTIPLI_INDIRIZZO = "Mittenti_multipli$indirizzo";
            public const string MITTENTI_MULTIPLI_TELEFONO = "Mittenti_multipli$telefono";
            public const string MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO = "Mittenti_multipli$indirizzo$telefono";
            public const string DESTINATARI = "Destinatari";
            public const string DESTINATARI_INDIRIZZO = "Destinatari$indirizzo";
            public const string DESTINATARI_TELEFONO = "Destinatari$telefono";
            public const string DESTINATARI_INDIRIZZO_TELEFONO = "Destinatari$indirizzo$telefono";
            public const string DESTINATARI_CC = "Destinatari CC";
            public const string DESTINATARI_CC_INDIRIZZO = "Destinatari CC$indirizzo";
            public const string DESTINATARI_CC_TELEFONO = "Destinatari CC$telefono";
            public const string DESTINATARI_CC_INDIRIZZO_TELEFONO = "Destinatari CC$indirizzo$telefono";
            public const string COD_RESP_UO = "Codice Responsabile UO";
            public const string DESC_RESP_UO = "Descrizione Responsabile UO";
            public const string LISTA_UTENTE_RESP_UO = "Lista Utenti Ruolo Responsabile UO";
            public const string AMMINISTRAZIONE = "Amministrazione";
            public const string DATA_ORA_PROTOCOLLO = "Data Ora Protocollo";
            public const string NUM_ALLEGATI = "Numero Allegati";
            public const string UFF_REF_DESC = "Descrizione Ufficio Referente";
            public const string UFF_REF_COD = "Codice Ufficio Referente";
            public const string UO_PADRE = "Uo superiore";
            public const string COD_UO_PADRE = "Codice Uo superiore";
            public const string INDIRIZZO_UO_PADRE = "Indirizzo uo superiore";
            public const string CITTA_UO_PADRE = "Citta' uo superiore";
            public const string CAP_UO_PADRE = "Cap uo superiore";
            public const string PROVINCIA_UO_PADRE = "Provincia uo superiore";
            public const string TEL1_UO_PADRE = "Telefono1 uo superiore";
            public const string TEL2_UO_PADRE = "Telefono2 uo superiore";
            public const string FAX_UO_PADRE = "Fax uo superiore";
            public const string CLASSIFICHE = "Classifiche";
        }

        protected async Task<ModelProcessorInfo> EnsureClientModelProcessorEntity(GetDocumentModelAsXmlRequest request)
        {
            var clientModelProcessorEntity = await this._pi3DbContext.ClientModelProcessorsEntities.AsNoTracking()
                       .Where(p => this._pi3DbContext.PeopleEntities.AsNoTracking()
                                   .Where(pl => pl.SYSTEM_ID == request.modelRequest.UserInfo.idPeople.AsLong()
                                           && pl.ID_CLIENT_MODEL_PROCESSOR == p.SYSTEM_ID)
                                   .Any())
                       .Select(p => p)
                       .FirstOrDefaultAsync();

            if (clientModelProcessorEntity == null)
            {
                clientModelProcessorEntity = await this._pi3DbContext.ClientModelProcessorsEntities.AsNoTracking()
                    .Where(p => this._pi3DbContext.AmministraEntities.AsNoTracking()
                                .Where(a => a.SYSTEM_ID == request.modelRequest.UserInfo.idAmministrazione.AsLong()
                                        && a.ID_CLIENT_MODEL_PROCESSOR == p.SYSTEM_ID)
                                .Any())
                    .Select(p => p)
                    .FirstOrDefaultAsync();
            }

            if (clientModelProcessorEntity == null)
                throw new GetDocumentModelAsXmlPi3Exception(ErrorDescriptions.NessunGeneratoreDocumentoImpostato);

            return new ModelProcessorInfo()
            {
                Id = (int)clientModelProcessorEntity.SYSTEM_ID,
                Name = clientModelProcessorEntity.NAME,
                ClassId = clientModelProcessorEntity.CLASS_ID,
                SupportedExtensions = clientModelProcessorEntity.SUPPORTED_EXTENSIONS
            };
        }

        protected async Task<string> GetModelPath(GetDocumentModelAsXmlRequest request)
        {

            var tipoAttoEntity = await this._pi3DbContext.TipoAttoEntities.AsNoTracking()
                        .Where(ta => this._pi3DbContext.ProfileEntities.AsNoTracking()
                                    .Where(p => p.DOCNUMBER == request.modelRequest.DocumentId.AsLong()
                                                && p.ID_TIPO_ATTO == ta.SYSTEM_ID)
                                    .Any())
                        .Select(ta => new
                        {
                            ta.PATH_MOD_1,
                            ta.PATH_MOD_2,
                            ta.PATH_ALL_1
                        })
                        .FirstAsync();

            string modelPath = null!;
            if (request.modelRequest.ModelType == MODEL_VERSION_1)
                modelPath = tipoAttoEntity.PATH_MOD_1!;
            else if (request.modelRequest.ModelType == MODEL_VERSION_2)
                modelPath = tipoAttoEntity.PATH_MOD_2! ?? tipoAttoEntity.PATH_MOD_1;
            else if (request.modelRequest.ModelType == MODEL_ATTATCHMENT)
                modelPath = tipoAttoEntity.PATH_ALL_1!;

            return modelPath!;
        }

        protected async Task<Model> LoadModel(GetDocumentModelAsXmlRequest request, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            //var getDettaglioDocumentoResult = await this._mediator.Send(new Requests.DocumentoGetDettaglioDocumento(request.modelRequest.UserInfo, request.modelRequest.DocumentId, request.modelRequest.DocumentId));

            //var schedaDocumento = getDettaglioDocumentoResult.output;
            string modelPath = null!;

            if (schedaDocumento.documentoPrincipale != null && request.modelRequest.ModelType.StartsWith(MODEL_ATTATCHMENT))
            {
                request.modelRequest.ModelType = request.modelRequest.ModelType.Substring(1, 1);

                // Caricamento documento allegato
                var getDettaglioDocumentoResult = await this._mediator.Send(new Requests.DocumentoGetDettaglioDocumento(request.modelRequest.UserInfo, schedaDocumento.documentoPrincipale.idProfile, schedaDocumento.documentoPrincipale.docNumber));
                schedaDocumento = getDettaglioDocumentoResult.output;
            }

            modelPath = await this.GetModelPath(request);

            if (string.IsNullOrWhiteSpace(modelPath))
            {
                throw new GetDocumentModelAsXmlPi3Exception(string.Format(ErrorDescriptions.ModelloNonTrovato, request.modelRequest.ModelType));
            }

            modelPath = modelPath.PathAsUnixPath();

            FileContent fileContent = null!;

            if (!File.Exists(modelPath))
            {
                // Se il modello non � stato trovato, restituizione del modello predefinito
                var defaultModel = DefaultModels.ResourceManager.GetObject(Path.GetExtension(modelPath).Replace(".", string.Empty).ToLower());
                if (defaultModel == null)
                {
                    // Modello predefinito non trovato
                    throw new GetDocumentModelAsXmlPi3Exception(string.Format(ErrorDescriptions.ModelloPredefinitoNonTrovato, request.modelRequest.ModelType));
                }

                byte[] content = null!;

                if (defaultModel.GetType() == typeof(string))
                    content = Encoding.UTF8.GetBytes((string)defaultModel);
                else
                    content = (byte[])defaultModel;

                fileContent = new FileContent()
                {
                    Content = content,
                    FileName = $"DefaultModel{Path.GetExtension(modelPath)}"
                };
            }
            else
            {
                fileContent = new FileContent()
                {
                    Content = File.ReadAllBytes(modelPath),
                    FileName = Path.GetFileName(modelPath)
                };
            }

            var documentModel = new Model()
            {
                KeyValuePairs = await this.GetModelKeyValuePairs(request.modelRequest.UserInfo, schedaDocumento),
                ModelType = request.modelRequest.ModelType,
                File = fileContent
            };

           

            return documentModel;
        }

        protected async Task<DocsPaVO.Modelli.ModelKeyValuePair[]> GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            ArrayList listaOggetti = await this.GetOggettiProfilazione(schedaDocumento.docNumber, infoUtente.idAmministrazione, schedaDocumento);

            await this.FetchCommonFields(listaOggetti, infoUtente, schedaDocumento);

            List<DocsPaVO.Modelli.ModelKeyValuePair> list = new List<DocsPaVO.Modelli.ModelKeyValuePair>();

            foreach (string[] items in listaOggetti)
            {
                DocsPaVO.Modelli.ModelKeyValuePair pair = new DocsPaVO.Modelli.ModelKeyValuePair();
                pair.Key = items[0];
                pair.Value = items[1];
                list.Add(pair);
            }

            return list.ToArray();
        }

        private async Task<ArrayList> GetOggettiProfilazione(string docNumber, string idAmministrazione, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            ArrayList listaChiaviValori = new ArrayList();

            if (!string.IsNullOrWhiteSpace(schedaDocumento.tipologiaAtto.systemId))
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Requests.getTemplateDettagli(schedaDocumento.docNumber))).output;

                if (template != null && template.ELENCO_OGGETTI != null)
                {
                    for (int i = 0; i < template.ELENCO_OGGETTI.Length; i++)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                        if (oggettoCustom != null)
                        {
                            switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                            {
                                case "Corrispondente":

                                    string[] itemToAdd = new string[23] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

                                    DocsPaVO.utente.Corrispondente corr = await this.GetCorrispondenteBySystemID(oggettoCustom.VALORE_DATABASE);

                                    itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                    itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                    itemToAdd[4] = oggettoCustom.ANNO;
                                    itemToAdd[5] = oggettoCustom.ID_AOO_RF;

                                    if (corr != null)
                                    {
                                        oggettoCustom.INDIRIZZO += corr.descrizione + '\n' + corr.indirizzo + '\n' + corr.cap + '\n' + corr.citta + '\n' + corr.localita;

                                        string addressAndEmail = "";
                                        string textPrincipalEmail = "";
                                        string corrEmails = "";

                                        if (corr.Emails.Count > 0)
                                        {
                                            getEmailInfo(corr, out addressAndEmail, out textPrincipalEmail, out corrEmails);
                                        }

                                        itemToAdd[1] = !string.IsNullOrEmpty(corr.descrizione) ? corr.descrizione : string.Empty;
                                        itemToAdd[6] = !string.IsNullOrEmpty(corr.telefono1) ? corr.telefono1 : string.Empty;
                                        itemToAdd[7] = !string.IsNullOrEmpty(corr.telefono2) ? corr.telefono2 : string.Empty;
                                        itemToAdd[8] = !string.IsNullOrEmpty(corr.citta) ? corr.citta : string.Empty;
                                        itemToAdd[9] = !string.IsNullOrEmpty(corr.cap) ? corr.cap : string.Empty;
                                        itemToAdd[10] = !string.IsNullOrEmpty(corr.prov) ? corr.prov : string.Empty;
                                        itemToAdd[11] = !string.IsNullOrEmpty(corr.localita) ? corr.localita : string.Empty;
                                        itemToAdd[12] = !string.IsNullOrEmpty(corr.nazionalita) ? corr.nazionalita : string.Empty;
                                        itemToAdd[13] = !string.IsNullOrEmpty(corr.fax) ? corr.fax : string.Empty;
                                        itemToAdd[14] = !string.IsNullOrEmpty(corr.codfisc) ? corr.codfisc : string.Empty;
                                        itemToAdd[15] = !string.IsNullOrEmpty(corr.partitaiva) ? corr.partitaiva : string.Empty;
                                        itemToAdd[16] = !string.IsNullOrEmpty(corrEmails) ? corrEmails : string.Empty;
                                        itemToAdd[17] = !string.IsNullOrEmpty(corr.codiceAOO) ? corr.codiceAOO : string.Empty;
                                        itemToAdd[18] = !string.IsNullOrEmpty(corr.codiceAmm) ? corr.codiceAmm : string.Empty;
                                        itemToAdd[19] = !string.IsNullOrEmpty(corr.note) ? corr.note : string.Empty;
                                        itemToAdd[20] = !string.IsNullOrEmpty(corr.indirizzo) ? corr.indirizzo : string.Empty;
                                        itemToAdd[21] = !string.IsNullOrEmpty(addressAndEmail) ? addressAndEmail : oggettoCustom.INDIRIZZO;
                                        itemToAdd[22] = !string.IsNullOrEmpty(addressAndEmail) ? corr.indirizzo : string.Empty;
                                    }

                                    listaChiaviValori.Add(itemToAdd);

                                    break;

                                case "Contatore":

                                    string formato = await getFormato(oggettoCustom);

                                    itemToAdd = new string[5] { "", "", "", "", "" };
                                    itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                    itemToAdd[1] = formato;
                                    itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                    itemToAdd[3] = oggettoCustom.FORMATO_CONTATORE;
                                    itemToAdd[4] = oggettoCustom.ANNO;
                                    listaChiaviValori.Add(itemToAdd);

                                    break;

                                case "CasellaDiSelezione":

                                    string valore = getValoreSelezionato(oggettoCustom.VALORI_SELEZIONATI);

                                    itemToAdd = new string[5] { "", "", "", "", "" };
                                    itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                    itemToAdd[1] = valore;
                                    itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                    itemToAdd[4] = oggettoCustom.ANNO;
                                    listaChiaviValori.Add(itemToAdd);

                                    break;

                                case "Link":

                                    itemToAdd = new string[7] { "", "", "", "", "", "", "" };

                                    string linkToObject = await buildLink(oggettoCustom, idAmministrazione);

                                    itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                    itemToAdd[1] = oggettoCustom.VALORE_DATABASE.Split('|')[0];
                                    itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                    itemToAdd[4] = oggettoCustom.ANNO;
                                    itemToAdd[6] = linkToObject;

                                    listaChiaviValori.Add(itemToAdd);

                                    break;

                                default:
                                    itemToAdd = new string[6] { "", "", "", "", "", "" };
                                    itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                    itemToAdd[1] = oggettoCustom.VALORE_DATABASE;
                                    itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                    itemToAdd[4] = oggettoCustom.ANNO;

                                    listaChiaviValori.Add(itemToAdd);

                                    break;
                            }
                        }
                    }

                }
            }
            return listaChiaviValori;
        }

        private async Task<string> buildLink(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, string idAmministrazione)
        {

            var tipoLink = oggettoCustom.TIPO_LINK;
            var tipoObjLlink = oggettoCustom.TIPO_OBJ_LINK;

            if (tipoLink.ToUpper().Equals("INTERNO"))
            {
                string pathFE = "";
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();
                string tipoObj = tipoObjLlink.ToUpper().Equals("DOCUMENTO") ? "D" : "F";

                string idObj = oggettoCustom.VALORE_DATABASE.Split('|')[4];

                if (tipoObjLlink.ToUpper().Equals("FASCICOLO"))
                {
                    DocsPaVO.fascicolazione.Fascicolo result = null;
                    result = (await this._mediator.Send(new Requests.FascicolazioneGetFascicoloByIdNoSecurity(idObj))).output;
                    idObj = result.codice;
                }

                string linkToObject = string.Format("{0}VisualizzaOggetto.htm?idAmministrazione={1}&tipoOggetto={2}&idObj={3}", pathFE, idAmministrazione, tipoObj, idObj);

                return linkToObject;
            }
            else //esterno
            {
                return oggettoCustom.VALORE_DATABASE.Split('|')[4];
            }
        }

        private string getValoreSelezionato(string[] valoriSelezionati)
        {
            string valore = string.Empty;
            foreach (string val in valoriSelezionati)
            {
                if (val != null && val != "")
                    valore += val + "-";
            }
            if (valore.Length > 1)
                valore = valore.Substring(0, valore.Length - 1);

            return valore;
        }

        private async Task<string> getFormato(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom)
        {
            string formato = oggettoCustom.FORMATO_CONTATORE;
            formato = formato.Replace("ANNO", oggettoCustom.ANNO);

            DocsPaVO.utente.Registro reg = (await this._mediator.Send(new Requests.GetRegistroBySistemId(
            oggettoCustom.ID_AOO_RF
                       ))).output;

            if (reg != null && !string.IsNullOrEmpty(reg.codRegistro))
            {
                formato = formato.Replace("AOO", reg.codRegistro);
                formato = formato.Replace("RF", reg.codRegistro);
            }

            formato = formato.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);

            return formato;
        }

        private static void getEmailInfo(Corrispondente corr, out string addressAndEmail, out string textPrincipalEmail, out string corrEmails)
        {
            DocsPaVO.utente.MailCorrispondente principalEmail = corr.Emails.Find(e => e.Principale.Equals("1"));

            if (principalEmail != null && !String.IsNullOrEmpty(principalEmail.Email))
            {
                addressAndEmail = corr.descrizione + '\n' + corr.indirizzo + '\n' + corr.cap + '\n' + corr.citta + '\n' + corr.localita + '\n' + principalEmail.Email;
                textPrincipalEmail = principalEmail.Email;
            }
            else
            {
                addressAndEmail = "";
                textPrincipalEmail = "";
            }

            corrEmails = corr.Emails[0].Email;
            for (int y = 1; y < corr.Emails.Count; y++)
            {
                corrEmails = corrEmails + ", " + corr.Emails[y].Email;
            }
        }

        private async Task<Corrispondente> GetCorrispondenteBySystemID(string systemID)
        {


            DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();

            try
            {
                if (!string.IsNullOrEmpty(systemID))
                {
                    DocsPaVO.utente.Corrispondente corrExists = new DocsPaVO.utente.Corrispondente();
                    bool existCorrispondente = false;
                    string idAmministrazione = string.Empty;

                    DocsPaVO.addressbook.TipoUtente tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;

                    corrExists = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(cg => cg.SYSTEM_ID == systemID.AsLong()).Select(cg => new DocsPaVO.utente.Corrispondente
                    {
                        idAmministrazione = cg.ID_AMM.ToString(),
                        tipoIE = cg.CHA_TIPO_IE
                    }).FirstOrDefaultAsync();

                    if (corrExists != null)
                    {
                        existCorrispondente = true;
                        idAmministrazione = corrExists.idAmministrazione ?? string.Empty;
                        if (corrExists.tipoIE.Equals("I"))
                            tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                        else if (corrExists.tipoIE.Equals("E"))
                            tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                    }

                    if (existCorrispondente)
                        // Reperimento dei metadati del corrisponente
                        corrispondente = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(
                        systemID,
                        tipoUtente,
                        new InfoUtente()

                       ))).output;

                    if (corrispondente != null)
                    {
                        var dett = await (from c in this._pi3DbContext.DettGlobaliEntities.AsNoTracking()
                                    where c.ID_CORR_GLOBALI == corrispondente.systemId.AsLong()
                                    select c).FirstOrDefaultAsync();

                        if (dett != null)
                        {
                            corrispondente.indirizzo = dett.VAR_INDIRIZZO;
                            corrispondente.cap = dett.VAR_CAP;
                            corrispondente.prov = dett.VAR_PROVINCIA;
                            corrispondente.citta = dett.VAR_CITTA;
                            corrispondente.localita = dett.VAR_LOCALITA;
                            corrispondente.telefono1 = dett.VAR_TELEFONO;
                            corrispondente.telefono2 = dett.VAR_TELEFONO2;
                            corrispondente.note = dett.VAR_NOTE;
                        }
                    }

                    else
                        _logger.LogError(string.Format(ErrorDescriptions.XmlEccezioneCorrNotFound, systemID));

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(ErrorDescriptions.XmlEccezioneCorrNotFound, systemID), ex);
            }
            finally
            {
                _logger.LogDebug("END - GetCorrispondenteBySystemID");
            }

            return corrispondente;
        }

        protected virtual async Task FetchCommonFields(ArrayList keyValues, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            //Campi profilati di tipo Corrispondente
            for (int i = 0; i < keyValues.Count; i++)
            {
                //Campi indirizzo, telefono e inidirizzo+telefono per il Corrispondente profilato
                string[] corrProf = (string[])keyValues[i];

                if (corrProf != null && corrProf.Length > 2 && corrProf[2] == "Corrispondente")
                {

                    string field = corrProf[0] + "$indirizzo";
                    AppendKeyValue(field, corrProf[3], keyValues);
                    string field1 = corrProf[0] + "$telefonoPrincipale";
                    AppendKeyValue(field1, corrProf[6], keyValues);
                    //string field2 = corrProf[0] + "$indirizzo$telefono";
                    //AppendKeyValue(field2, corrProf[7], keyValues);

                    //mev aggiunta campi corrispondente
                    string field2 = corrProf[0] + "$telefonoSecondario";
                    AppendKeyValue(field2, corrProf[7], keyValues);
                    string field3 = corrProf[0] + "$citta";
                    AppendKeyValue(field3, corrProf[8], keyValues);
                    string field4 = corrProf[0] + "$cap";
                    AppendKeyValue(field4, corrProf[9], keyValues);
                    string field5 = corrProf[0] + "$provincia";
                    AppendKeyValue(field5, corrProf[10], keyValues);
                    string field6 = corrProf[0] + "$localita";
                    AppendKeyValue(field6, corrProf[11], keyValues);
                    string field7 = corrProf[0] + "$nazionalita";
                    AppendKeyValue(field7, corrProf[12], keyValues);
                    string field8 = corrProf[0] + "$fax";
                    AppendKeyValue(field8, corrProf[13], keyValues);
                    string field9 = corrProf[0] + "$codiceFiscale";
                    AppendKeyValue(field9, corrProf[14], keyValues);
                    string field10 = corrProf[0] + "$partitaIva";
                    AppendKeyValue(field10, corrProf[15], keyValues);
                    string field11 = corrProf[0] + "$emails";
                    AppendKeyValue(field11, corrProf[16], keyValues);
                    string field12 = corrProf[0] + "$codiceAOO";
                    AppendKeyValue(field12, corrProf[17], keyValues);
                    string field13 = corrProf[0] + "$codiceAmministrazione";
                    AppendKeyValue(field13, corrProf[18], keyValues);
                    string field14 = corrProf[0] + "$note";
                    AppendKeyValue(field14, corrProf[19], keyValues);
                    string field16 = corrProf[0] + "$viaECivico";
                    AppendKeyValue(field16, corrProf[20], keyValues);
                    string field17 = corrProf[0] + "$indirizzo$emailPrincipale";
                    AppendKeyValue(field17, corrProf[21], keyValues);
                    string field18 = corrProf[0] + "$emailPrincipale";
                    AppendKeyValue(field18, corrProf[22], keyValues);
                }

                //Campi per il link
                if (corrProf != null && corrProf.Length > 2 && corrProf[2] == "Link")
                {

                    string field1 = corrProf[0] + "$linkDiretto";
                    AppendKeyValue(field1, corrProf[6], keyValues);

                }
            }
            // Inizializzazione campi comuni
            this.InitCommonFields(keyValues);
            DocsPaVO.amministrazione.OrgRuolo ruoloResp = new DocsPaVO.amministrazione.OrgRuolo();

            // DESCRIZIONE AMMINISTRAZIONE
            DocsPaVO.amministrazione.InfoAmministrazione infoAmm = (await this._mediator.Send(new Requests.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione))).output;

            if (infoAmm != null)
                AppendKeyValue(DocumentCommonFields.AMMINISTRAZIONE, infoAmm.Descrizione, keyValues);

            // OGGETTO
            AppendKeyValue(DocumentCommonFields.OGGETTO, schedaDocumento.oggetto.descrizione, keyValues);

            // DATA CREAZIONE
            AppendKeyValue(DocumentCommonFields.DATA_CREAZIONE, schedaDocumento.dataCreazione, keyValues);

            // ID DOCUMENTO
            AppendKeyValue(DocumentCommonFields.ID_DOCUMENTO, schedaDocumento.docNumber, keyValues);

            // NOTE (Reperimento dell'ultima nota visibile a tutti)
            string testoNote = await this.FetchNote(schedaDocumento.systemId, infoUtente);
            AppendKeyValue(DocumentCommonFields.NOTE, testoNote, keyValues);

            // TIPOLOGIA
            if (schedaDocumento.tipologiaAtto != null)
                AppendKeyValue(DocumentCommonFields.TIPOLOGIA, schedaDocumento.tipologiaAtto.descrizione, keyValues);

            // CREATORE	
            if (schedaDocumento.creatoreDocumento != null)
                await AppendKeyValueCreatore(schedaDocumento, keyValues);

            //Ruolo responsabile della UO
            if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                await AppendKeyValueRespUO(schedaDocumento.creatoreDocumento.idCorrGlob_UO, keyValues);

            // Classificazioni del documento
            string classifiche = await GetClassificazione(infoUtente, schedaDocumento.systemId, keyValues);
            AppendKeyValue(DocumentCommonFields.CLASSIFICHE, classifiche, keyValues);

            // campi del documento protocollato
            if (schedaDocumento.protocollo != null)
            {
                // NUMERO PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, schedaDocumento.protocollo.numero, keyValues);

                // SEGNATURA
                AppendKeyValue(DocumentCommonFields.SEGNATURA, schedaDocumento.protocollo.segnatura, keyValues);

                // DATA PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, schedaDocumento.protocollo.dataProtocollazione, keyValues);

                // DATA ORA PROTOCOLLO
                AppendKeyValue(DocumentCommonFields.DATA_ORA_PROTOCOLLO, (await this.getDataOraProtcollo(schedaDocumento.docNumber)), keyValues);

                // REGISTRO
                AppendKeyValue(DocumentCommonFields.REGISTRO, schedaDocumento.registro.descrizione, keyValues);

                // CODICE REGISTRO
                AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, schedaDocumento.registro.codRegistro, keyValues);

                //NUMERO ALLEGATI
                var listaAllegati = (await this._mediator.Send(new Requests.DocumentoGetAllegati(schedaDocumento.docNumber, string.Empty, string.Empty))).output.ToArray();
                if (listaAllegati != null)
                    AppendKeyValue(DocumentCommonFields.NUM_ALLEGATI, Convert.ToString(listaAllegati.Length), keyValues);

                //PROTOCOLLATORE
                if (schedaDocumento.protocollatore != null)
                    await AppendKeyValueProtocollatore(schedaDocumento.protocollatore, keyValues);


                if (schedaDocumento.tipoProto != null)
                {
                    await this.AppendKeyValueByTipoProtocollo(schedaDocumento, keyValues);
                }
            }
            else
            {
                AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.SEGNATURA, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.REGISTRO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            }
        }

        private async Task AppendKeyValueByTipoProtocollo(DocsPaVO.documento.SchedaDocumento schedaDocumento, ArrayList keyValues)
        {
            string listaDestinatari = string.Empty;
            string listaDestinatariIndirizzi = string.Empty;
            string listaDestinatariTelefono = string.Empty;
            string listaDestinatariIndirizzoTelefono = string.Empty;
            string listaMittentiMultipli = string.Empty;
            string listaMittentiMultipliIndirizzo = string.Empty;
            string listaMittentiMultipliTelefono = string.Empty;
            string listaMittentiMultipliIndirizzoTelefono = string.Empty;
            string mittenteIndirizzo = string.Empty;
            string mittenteTelefono = string.Empty;
            string mittenteIndirizzoTelefono = string.Empty;
            DocsPaVO.utente.Corrispondente mittIndirizzo = new DocsPaVO.utente.Corrispondente();

            switch (schedaDocumento.tipoProto)
            {

                // Protocollo in INGRESSO (Arrivo)
                case "A":
                    DocsPaVO.documento.ProtocolloEntrata protArr = new DocsPaVO.documento.ProtocolloEntrata();
                    protArr = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                    // MITTENTE
                    mittIndirizzo = await this.getDettagliIndirizzoCorrispondente(protArr.mittente.systemId);
                    this.BuildMittSingoloIndirizzo(mittIndirizzo, protArr.mittente.descrizione, ref mittenteIndirizzo, ref mittenteTelefono, ref mittenteIndirizzoTelefono);
                    this.AppendKeyValueProtCorr(keyValues, mittenteIndirizzo, mittenteTelefono, mittenteIndirizzoTelefono, protArr.mittente.descrizione);

                    // MITTENTE MULTIPLO
                    foreach (DocsPaVO.utente.Corrispondente mittMult in protArr.mittenti)
                    {
                        if (listaMittentiMultipli != string.Empty)
                            listaMittentiMultipli += Environment.NewLine;
                        listaMittentiMultipli += mittMult.descrizione;

                        DocsPaVO.utente.Corrispondente corrIndirizzo = await this.getDettagliIndirizzoCorrispondente(mittMult.systemId);
                        this.BuildCorrMultiploIndirizzo(corrIndirizzo, mittMult.descrizione, ref listaMittentiMultipli, ref listaMittentiMultipliIndirizzo, ref listaMittentiMultipliTelefono, ref listaMittentiMultipliIndirizzoTelefono);

                    }


                    this.AppendKeyValueCorrMultiplo(listaMittentiMultipli, listaMittentiMultipliIndirizzo, listaMittentiMultipliTelefono, listaMittentiMultipliIndirizzoTelefono, keyValues);

                    if (protArr.ufficioReferente != null)
                    {
                        // CODICE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_COD, protArr.ufficioReferente.codiceRubrica, keyValues);

                        // DESCRIZIONE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, protArr.ufficioReferente.descrizione, keyValues);
                    }

                    break;

                // Protocollo in USCITA (Partenza)
                case "P":
                    DocsPaVO.documento.ProtocolloUscita protUsc = new DocsPaVO.documento.ProtocolloUscita();
                    protUsc = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;

                    // MITTENTE
                    mittIndirizzo = await this.getDettagliIndirizzoCorrispondente(protUsc.mittente.systemId);

                    // DESTINATARI	
                    foreach (DocsPaVO.utente.Corrispondente utCorr in protUsc.destinatari)
                    {
                        if (listaDestinatari != string.Empty)
                            listaDestinatari += Environment.NewLine;
                        listaDestinatari += utCorr.descrizione;


                        DocsPaVO.utente.Corrispondente corrIndirizzo = await this.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                        this.BuildCorrMultiploIndirizzo(corrIndirizzo, utCorr.descrizione, ref listaDestinatari, ref listaDestinatariIndirizzi, ref listaDestinatariTelefono, ref listaDestinatariIndirizzoTelefono);
                    }

                    // DESTINATARI CC
                    string listaDestinatariCC = string.Empty;
                    string listaDestinatariCCIndirizzi = string.Empty;
                    string listaDestinatariCCTelefono = string.Empty;
                    string listaDestinatarCCiIndirizzoTelefono = string.Empty;
                    foreach (DocsPaVO.utente.Corrispondente utCorr in protUsc.destinatariConoscenza)
                    {
                        if (listaDestinatariCC != string.Empty)
                            listaDestinatariCC += Environment.NewLine;
                        listaDestinatariCC += utCorr.descrizione;

                        DocsPaVO.utente.Corrispondente corrIndirizzo = await this.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                        this.BuildCorrMultiploIndirizzo(corrIndirizzo, utCorr.descrizione, ref listaDestinatariCC, ref listaDestinatariCCIndirizzi, ref listaDestinatariCCTelefono, ref listaDestinatarCCiIndirizzoTelefono);

                    }

                    this.AppendKeyValueProtCorr(keyValues, mittenteIndirizzo, mittenteTelefono, mittenteIndirizzoTelefono, protUsc.mittente.descrizione, listaDestinatari, listaDestinatariIndirizzi, listaDestinatariTelefono, listaDestinatariIndirizzoTelefono, listaDestinatariCC, listaDestinatariCCIndirizzi, listaDestinatariCCTelefono, listaDestinatarCCiIndirizzoTelefono);

                    if (protUsc.ufficioReferente != null)
                    {
                        // CODICE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_COD, protUsc.ufficioReferente.codiceRubrica, keyValues);

                        // DESCRIZIONE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, protUsc.ufficioReferente.descrizione, keyValues);
                    }

                    break;

                // Protocollo INTERNO
                case "I":

                    DocsPaVO.documento.ProtocolloInterno protInt = new DocsPaVO.documento.ProtocolloInterno();
                    protInt = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;

                    // MITTENTE
                    mittIndirizzo = await this.getDettagliIndirizzoCorrispondente(protInt.mittente.systemId);

                    // DESTINATARI	
                    foreach (DocsPaVO.utente.Corrispondente utCorr in protInt.destinatari)
                    {
                        if (listaDestinatari != string.Empty)
                            listaDestinatari += Environment.NewLine;
                        listaDestinatari += utCorr.descrizione;


                        DocsPaVO.utente.Corrispondente corrIndirizzo = await this.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                        this.BuildCorrMultiploIndirizzo(corrIndirizzo, utCorr.descrizione, ref listaDestinatari, ref listaDestinatariIndirizzi, ref listaDestinatariTelefono, ref listaDestinatariIndirizzoTelefono);
                    }

                    // DESTINATARI CC
                    string listaDestinatariCCInt = string.Empty;
                    string listaDestinatariCCIndirizziInt = string.Empty;
                    string listaDestinatariCCTelefonoInt = string.Empty;
                    string listaDestinatarCCiIndirizzoTelefonoInt = string.Empty;

                    foreach (DocsPaVO.utente.Corrispondente utCorr in protInt.destinatariConoscenza)
                    {
                        if (listaDestinatariCCInt != string.Empty)
                            listaDestinatariCCInt += Environment.NewLine;
                        listaDestinatariCCInt += utCorr.descrizione;

                        DocsPaVO.utente.Corrispondente corrIndirizzo = await this.getDettagliIndirizzoCorrispondente(utCorr.systemId);
                        this.BuildCorrMultiploIndirizzo(corrIndirizzo, utCorr.descrizione, ref listaDestinatariCCInt, ref listaDestinatariCCIndirizziInt, ref listaDestinatariCCTelefonoInt, ref listaDestinatarCCiIndirizzoTelefonoInt);

                    }

                    this.AppendKeyValueProtCorr(keyValues, mittenteIndirizzo, mittenteTelefono, mittenteIndirizzoTelefono, protInt.mittente.descrizione, listaDestinatari, listaDestinatariIndirizzi, listaDestinatariTelefono, listaDestinatariIndirizzoTelefono, listaDestinatariCCInt, listaDestinatariCCIndirizziInt, listaDestinatariCCTelefonoInt, listaDestinatarCCiIndirizzoTelefonoInt);

                    if (protInt.ufficioReferente != null)
                    {
                        // CODICE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_COD, protInt.ufficioReferente.codiceRubrica, keyValues);

                        // DESCRIZIONE UFFICIO REFERENTE
                        AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, protInt.ufficioReferente.descrizione, keyValues);
                    }
                    break;


            }

        }

        private void AppendKeyValueCorrMultiplo(string listaMittentiMultipli, string listaMittentiMultipliIndirizzo, string listaMittentiMultipliTelefono, string listaMittentiMultipliIndirizzoTelefono, ArrayList keyValues)
        {
            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI, !string.IsNullOrEmpty(listaMittentiMultipli) ? listaMittentiMultipli : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, !string.IsNullOrEmpty(listaMittentiMultipliIndirizzo) ? listaMittentiMultipliIndirizzo : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, !string.IsNullOrEmpty(listaMittentiMultipliTelefono) ? listaMittentiMultipliTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, !string.IsNullOrEmpty(listaMittentiMultipliIndirizzoTelefono) ? listaMittentiMultipliIndirizzoTelefono : string.Empty, keyValues);
        }

        private void BuildCorrMultiploIndirizzo(Corrispondente corrIndirizzo, string descrizione, ref string listaMittentiMultipli, ref string listaMittentiMultipliIndirizzo, ref string listaMittentiMultipliTelefono, ref string listaMittentiMultipliIndirizzoTelefono)
        {
            if (listaMittentiMultipliIndirizzo != string.Empty)
                listaMittentiMultipliIndirizzo += Environment.NewLine;
            listaMittentiMultipliIndirizzo += descrizione + Environment.NewLine + corrIndirizzo.indirizzo
                + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita;

            if (listaMittentiMultipliTelefono != string.Empty)
                listaMittentiMultipliTelefono += Environment.NewLine;
            listaMittentiMultipliTelefono += descrizione + Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;

            if (listaMittentiMultipliIndirizzoTelefono != string.Empty)
                listaMittentiMultipliIndirizzoTelefono += Environment.NewLine;
            listaMittentiMultipliIndirizzoTelefono += descrizione + Environment.NewLine + corrIndirizzo.indirizzo
            + Environment.NewLine + corrIndirizzo.cap + "-" + corrIndirizzo.citta + "-" + corrIndirizzo.localita +
            Environment.NewLine + corrIndirizzo.telefono1 + "-" + corrIndirizzo.telefono2;


        }

        private void AppendKeyValueProtCorr(ArrayList keyValues, string mittenteIndirizzo, string mittenteTelefono, string mittenteIndirizzoTelefono, string descrizione, string listaDestinatari = "", string listaDestinatariIndirizzi = "", string listaDestinatariTelefono = "", string listaDestinatariIndirizzoTelefono = "", string listaDestinatariCC = "", string listaDestinatariCCIndirizzi = "", string listaDestinatariCCTelefono = "", string listaDestinatarCCiIndirizzoTelefono = "")
        {
            AppendKeyValue(DocumentCommonFields.MITTENTE, !string.IsNullOrEmpty(descrizione) ? descrizione : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, !string.IsNullOrEmpty(mittenteIndirizzo) ? mittenteIndirizzo : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, !string.IsNullOrEmpty(mittenteTelefono) ? mittenteTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, !string.IsNullOrEmpty(mittenteIndirizzoTelefono) ? mittenteIndirizzoTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI, !string.IsNullOrEmpty(listaDestinatari) ? listaDestinatari : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, !string.IsNullOrEmpty(listaDestinatariIndirizzi) ? listaDestinatariIndirizzi : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, !string.IsNullOrEmpty(listaDestinatariTelefono) ? listaDestinatariTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, !string.IsNullOrEmpty(listaDestinatariIndirizzoTelefono) ? listaDestinatariIndirizzoTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, !string.IsNullOrEmpty(listaDestinatariCC) ? listaDestinatariCC : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, !string.IsNullOrEmpty(listaDestinatariCCIndirizzi) ? listaDestinatariCCIndirizzi : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, !string.IsNullOrEmpty(listaDestinatariCCTelefono) ? listaDestinatariCCTelefono : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, !string.IsNullOrEmpty(listaDestinatarCCiIndirizzoTelefono) ? listaDestinatarCCiIndirizzoTelefono : string.Empty, keyValues);
            
        }

        private void BuildMittSingoloIndirizzo(Corrispondente mittIndirizzo, string descrizione, ref string mittenteIndirizzo, ref string mittenteTelefono, ref string mittenteIndirizzoTelefono)
        {
            mittenteIndirizzo += descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita;
            mittenteTelefono += descrizione + Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;
            mittenteIndirizzoTelefono += descrizione + Environment.NewLine + mittIndirizzo.indirizzo +
                Environment.NewLine + mittIndirizzo.cap + "-" + mittIndirizzo.citta + "-" + mittIndirizzo.localita +
                Environment.NewLine + mittIndirizzo.telefono1 + "-" + mittIndirizzo.telefono2;
        }

        private async Task<Corrispondente> getDettagliIndirizzoCorrispondente(string systemId)
        {
            DocsPaVO.utente.Corrispondente corr = new();

            var dett = await (from c in this._pi3DbContext.DettGlobaliEntities.AsNoTracking()
                              where c.ID_CORR_GLOBALI == systemId.AsLong()
                              select c).FirstOrDefaultAsync();

            if (dett != null)
            {
                corr.indirizzo = dett.VAR_INDIRIZZO;
                corr.cap = dett.VAR_CAP;
                corr.prov = dett.VAR_PROVINCIA;
                corr.citta = dett.VAR_CITTA;
                corr.localita = dett.VAR_LOCALITA;
                corr.telefono1 = dett.VAR_TELEFONO;
                corr.telefono2 = dett.VAR_TELEFONO2;
                corr.note = dett.VAR_NOTE;
            }

            return corr;
        }

        private async Task AppendKeyValueProtocollatore(Protocollatore protocollatore, ArrayList keyValues)
        {

            //// PROTOCOLLATORE						
            if (protocollatore.utente_idPeople != null && protocollatore.utente_idPeople != string.Empty)
            {
                var corr = await this.GetCorrispondenteBySystemID((await this.GetIDUtCorr(protocollatore.utente_idPeople)).ToString());
                AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, corr.descrizione, keyValues);
            }

            //// RUOLO PROTOCOLLATORE					
            if (protocollatore.ruolo_idCorrGlobali != null && protocollatore.ruolo_idCorrGlobali != string.Empty)
            {
                var corr = await this.GetCorrispondenteBySystemID(protocollatore.ruolo_idCorrGlobali);
                AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, corr.descrizione, keyValues);
            }

            //// UO PROTOCOLLATORE					
            if (protocollatore.uo_idCorrGlobali != null && protocollatore.uo_idCorrGlobali != string.Empty)
            {
                //descrizione
                DocsPaVO.amministrazione.OrgDettagliGlobali dettCorrUOParent = new DocsPaVO.amministrazione.OrgDettagliGlobali();
                DocsPaVO.utente.Corrispondente corrUOParent = new DocsPaVO.utente.Corrispondente();
                var corrUO = await this.GetCorrispondenteBySystemID(protocollatore.uo_idCorrGlobali);
                AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, corrUO.descrizione, keyValues);

                //dettagli
                var dettCorrUO = await this.AmmGetDatiStampaBuste(protocollatore.uo_idCorrGlobali.AsLong());
                if (corrUO != null)
                {
                    AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, dettCorrUO.Indirizzo, keyValues);
                    AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, dettCorrUO.Citta, keyValues);
                    AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, dettCorrUO.Cap, keyValues);
                    AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, dettCorrUO.Nazione, keyValues);
                    AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, dettCorrUO.Provincia, keyValues);
                    AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, dettCorrUO.Telefono1, keyValues);
                    AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, dettCorrUO.Telefono2, keyValues);
                    AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, dettCorrUO.Fax, keyValues);
                }

                // UO PADRE
                //DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                int idParent = await this.AmmListaIDParentRicercaUO(Convert.ToInt32(protocollatore.uo_idCorrGlobali));
                if (idParent != 0)
                {
                    corrUOParent = await this.GetCorrispondenteBySystemID(Convert.ToString(idParent));

                    //dettagli
                    if (corrUOParent != null)
                        dettCorrUOParent = await this.AmmGetDatiStampaBuste(corrUOParent.systemId.AsLong());

                }
                AppendKeyValueUOParent(corrUOParent, dettCorrUOParent, keyValues);
            }
        }

        private async Task<string> getDataOraProtcollo(string idProto)
        {
            var data = await (from p in this._pi3DbContext.ProfileEntities.AsNoTracking()
                              where p.SYSTEM_ID == idProto.AsLong()
                              select p.DTA_PROTO).FirstOrDefaultAsync();
            return data != null ? data.AsDateTimeFormat() : null;
        }

        private async Task AppendKeyValueRespUO(string idCorrGlob_UO, ArrayList keyValues)
        {
            var ruoloResp = await this.AmmGetRuoloResponsabileUO(idCorrGlob_UO);

            if (ruoloResp != null)
            {
                string listaUtenti = string.Empty;
                if (ruoloResp.Utenti != null && ruoloResp.Utenti.Length > 0)
                {
                    listaUtenti = GetListaUtenti(ruoloResp);



                }
                AppendKeyValue(DocumentCommonFields.COD_RESP_UO, !string.IsNullOrEmpty(ruoloResp.CodiceRubrica) ? ruoloResp.CodiceRubrica : string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.DESC_RESP_UO, !string.IsNullOrEmpty(ruoloResp.Descrizione) ? ruoloResp.Descrizione : string.Empty, keyValues);
                AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, !string.IsNullOrEmpty(listaUtenti) ? listaUtenti : string.Empty, keyValues);
            }
        }

        private async Task<string> GetClassificazione(InfoUtente infoUtente, string systemId, ArrayList keyValues)
        {

            string classifiche = string.Empty;
            var fascicoli = await this._mediator.Send(new FascicolazioneGetFascicoliDaDocRequest(infoUtente, systemId));
            var listaFascicoli = fascicoli.output.ToArray();
            foreach (DocsPaVO.fascicolazione.Fascicolo item in listaFascicoli)
            {
                if (classifiche != string.Empty)
                    classifiche += Environment.NewLine;
                classifiche += item.codice;
            }

            return classifiche;

        }

        private string GetListaUtenti(OrgRuolo ruoloResp)
        {
            string listaUtenti = string.Empty;

            for (int i = 0; i < ruoloResp.Utenti.Length; i++)
            {
                DocsPaVO.amministrazione.OrgUtente ut = ((DocsPaVO.amministrazione.OrgUtente)ruoloResp.Utenti[i]);
                listaUtenti += ut.Nome + " " + ut.Cognome + ",";
            }
            if (listaUtenti != "")
                listaUtenti = listaUtenti.Substring(0, listaUtenti.Length - 1);

            return listaUtenti;
        }

        private async Task<OrgRuolo> AmmGetRuoloResponsabileUO(string idUo)
        {
            var ruoloResp = await (from a in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                   where a.ID_UO == idUo.AsLong() && a.CHA_RESPONSABILE != null && a.CHA_RESPONSABILE.Equals("1") && !a.DTA_FINE.HasValue
                                   select new DocsPaVO.amministrazione.OrgRuolo()
                                   {
                                       IDCorrGlobale = a.SYSTEM_ID.ToString(),
                                       IDGruppo = a.ID_GRUPPO != null ? a.ID_GRUPPO.ToString() : null,
                                       IDTipoRuolo = a.ID_TIPO_RUOLO != null ? a.ID_TIPO_RUOLO.ToString() : null,
                                       Codice = a.VAR_CODICE,
                                       CodiceRubrica = a.VAR_COD_RUBRICA,
                                       Descrizione = a.VAR_DESC_CORR,
                                       DiRiferimento = a.CHA_RIFERIMENTO,
                                       IDAmministrazione = a.ID_AMM != null ? a.ID_AMM.ToString() : null,
                                       Responsabile = a.CHA_RESPONSABILE
                                   }).FirstOrDefaultAsync();

            if (ruoloResp != null)
            {
                ruoloResp.Utenti = (await this._mediator.Send(new Application.Requests.AmmGetListUtentiRuolo(ruoloResp.IDGruppo))).output;
            }

            return ruoloResp;
        }

        private async Task AppendKeyValueCreatore(DocsPaVO.documento.SchedaDocumento schedaDocumento, ArrayList keyValues)
        {

            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            DocsPaVO.utente.Corrispondente corrParent = new DocsPaVO.utente.Corrispondente();
            //DocsPaDB.Query_DocsPAWS.Utenti obj = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.amministrazione.OrgDettagliGlobali dettCorr = new DocsPaVO.amministrazione.OrgDettagliGlobali();
            DocsPaVO.amministrazione.OrgDettagliGlobali dettCorrParent = new DocsPaVO.amministrazione.OrgDettagliGlobali();

            if (schedaDocumento.creatoreDocumento.idPeople != null && schedaDocumento.creatoreDocumento.idPeople != string.Empty)
            {
                corr = await this.GetCorrispondenteBySystemID(await this.GetIDUtCorr(schedaDocumento.creatoreDocumento.idPeople));
                AppendKeyValue(DocumentCommonFields.CREATORE, corr.descrizione, keyValues);
            }

            // RUOLO CREATORE				
            if (schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != null && schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != string.Empty)
            {
                corr = await this.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo);
                AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, corr.descrizione, keyValues);
            }
            else
                AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, string.Empty, keyValues);

            // UO CREATORE				
            if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
            {
                //descrizione
                corr = await this.GetCorrispondenteBySystemID(schedaDocumento.creatoreDocumento.idCorrGlob_UO);

                // UO PADRE
                string idCorrGlobali = string.Empty;
                if (schedaDocumento.protocollatore != null)
                    idCorrGlobali = schedaDocumento.protocollatore.uo_idCorrGlobali;
                else
                    idCorrGlobali = schedaDocumento.creatoreDocumento.idCorrGlob_UO;

                int idParent = await this.AmmListaIDParentRicercaUO(Convert.ToInt32(idCorrGlobali));
                if (idParent != 0)
                {
                    corrParent = await this.GetCorrispondenteBySystemID(Convert.ToString(idParent));

                    //dettagli
                    if (corrParent != null)
                        dettCorrParent = await this.AmmGetDatiStampaBuste(corrParent.systemId.AsLong());

                }

                AppendKeyValueUOParent(corrParent, dettCorrParent, keyValues);

                //dettagli
                dettCorr = await this.AmmGetDatiStampaBuste(schedaDocumento.creatoreDocumento.idCorrGlob_UO.AsLong());
            }

            AppendKeyValueUOCreatore(corr, dettCorr, keyValues);
        }

        private void AppendKeyValueUOCreatore(Corrispondente? corr, OrgDettagliGlobali dettCorr, ArrayList keyValues)
        {
            AppendKeyValue(DocumentCommonFields.UO_CREATORE, !string.IsNullOrEmpty(corr.descrizione) ? corr.descrizione : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_CREATORE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Indirizzo)) ? dettCorr.Indirizzo : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.CITTA_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Citta)) ? dettCorr.Citta : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.CAP_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Cap)) ? dettCorr.Cap : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.NAZIONE_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Nazione)) ? dettCorr.Nazione : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Provincia)) ? dettCorr.Provincia : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.TEL1_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Telefono1)) ? dettCorr.Telefono1 : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.TEL2_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Telefono2)) ? dettCorr.Telefono2 : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.FAX_UO_CREATORE, (dettCorr!=null &&!string.IsNullOrEmpty(dettCorr.Fax)) ? dettCorr.Fax : string.Empty, keyValues);
        }

        private async Task<OrgDettagliGlobali> AmmGetDatiStampaBuste(long systemId)
        {

            var dettGlob = await this._pi3DbContext.DettGlobaliEntities.AsNoTracking()
                .Where(x => x.ID_CORR_GLOBALI == systemId)
                .Select(x => new OrgDettagliGlobali()
                {
                    Indirizzo = x.VAR_INDIRIZZO,
                    Citta = x.VAR_CITTA,
                    Cap = x.VAR_CAP,
                    Nazione = x.VAR_NAZIONE,
                    Provincia = x.VAR_PROVINCIA,
                    Telefono1 = x.VAR_TELEFONO,
                    Telefono2 = x.VAR_TELEFONO2,
                    Fax = x.VAR_FAX,
                    Note = x.VAR_NOTE,
                    CodiceFiscale = x.VAR_COD_FISC,
                    PartitaIva = x.VAR_COD_PI
                }).FirstOrDefaultAsync();


            return dettGlob;
        }

        private void AppendKeyValueUOParent(Corrispondente corr, OrgDettagliGlobali? dettCorr, ArrayList keyValues)
        {
            AppendKeyValue(DocumentCommonFields.UO_PADRE, !string.IsNullOrEmpty(corr.descrizione) ? corr.descrizione : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, !string.IsNullOrEmpty(corr.codiceRubrica) ? corr.codiceRubrica : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE,(dettCorr!=null && !string.IsNullOrEmpty(dettCorr.Indirizzo)) ? dettCorr.Indirizzo : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Citta)) ? dettCorr.Citta : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.CAP_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Cap)) ? dettCorr.Cap : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Provincia)) ? dettCorr.Provincia : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Telefono1)) ? dettCorr.Telefono1 : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Telefono2)) ? dettCorr.Telefono2 : string.Empty, keyValues);
            AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, (dettCorr != null && !string.IsNullOrEmpty(dettCorr.Fax)) ? dettCorr.Fax : string.Empty, keyValues);
        }

        private async Task<int> AmmListaIDParentRicercaUO(int idUoParent)
        {
            int output = 0;
            var id = idUoParent.ToString().AsLong();
            var parent = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idUoParent).FirstOrDefaultAsync();
            if (parent != null)
            {
                output = parent.ID_PARENT != null ? (int)parent.ID_PARENT : 0;
            }

            return output;
        }

        private async Task<string> GetIDUtCorr(string idPeople)
        {
            var sysId = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idPeople.AsLong()).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

            return sysId != null ? sysId.ToString() : string.Empty;
        }

        private async Task<string> FetchNote(string systemId, InfoUtente infoUtente)
        {
            string testoNote = string.Empty;

            foreach (DocsPaVO.Note.InfoNota nota in await this.GetNote(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, systemId), null))
            {
                if (nota.TipoVisibilita == DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti)
                {
                    testoNote = nota.Testo;
                    break;
                }
            }

            return testoNote;

        }

        private async Task<IEnumerable<InfoNota>> GetNote(InfoUtente infoUtente, AssociazioneNota oggettoAssociato, FiltroRicercaNote filtroRicerca)
        {
            List<InfoNota> output = new();

            string idRuoloInUo = string.IsNullOrEmpty(infoUtente.idCorrGlobali) ? "0" : infoUtente.idCorrGlobali;
            var reg = await this._pi3DbContext.RuoloRegistroEntities.AsNoTracking().Where(r => r.ID_RUOLO_IN_UO != null && r.ID_RUOLO_IN_UO.Equals(r.ID_RUOLO_IN_UO)).Select(r => r.ID_REGISTRO).ToListAsync();
            var idRuoloCreatore = string.IsNullOrEmpty(infoUtente.idGruppo) ? "0" : infoUtente.idGruppo;

            var noteQuery = (from n in this._pi3DbContext.NoteEntities.AsNoTracking()
                             join p in this._pi3DbContext.PeopleEntities.AsNoTracking() on n.IDUTENTECREATORE equals p.SYSTEM_ID into pj
                             from pi in pj.DefaultIfEmpty()
                             join g in this._pi3DbContext.GroupEntities.AsNoTracking() on n.IDRUOLOCREATORE equals g.SYSTEM_ID into gj
                             from gi in gj.DefaultIfEmpty()
                             where (n.IDOGGETTOASSOCIATO == oggettoAssociato.Id.AsLong()) && (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("T") ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("F") && reg.Contains(n.IDRFASSOCIATO) ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("P") && n.IDUTENTECREATORE == infoUtente.idPeople.AsLong()) ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("R") && n.IDRUOLOCREATORE == idRuoloCreatore.AsLong())
                             ))
                             select new
                             {
                                 n.SYSTEM_ID,
                                 n.TESTO,
                                 n.DATACREAZIONE,
                                 n.IDUTENTECREATORE,
                                 n.IDRUOLOCREATORE,
                                 n.TIPOVISIBILITA,
                                 n.TIPOOGGETTOASSOCIATO,
                                 n.IDOGGETTOASSOCIATO,
                                 n.IDRFASSOCIATO,
                                 pi.USER_ID,
                                 pi.FULL_NAME,
                                 gi.GROUP_ID,
                                 gi.GROUP_NAME,
                                 n.IDPEOPLEDELEGATO,
                             });
            if (filtroRicerca!= null && !string.IsNullOrEmpty(filtroRicerca.Testo))
            {
                noteQuery = noteQuery.Where(n => n.TESTO != null && n.TESTO.Contains(filtroRicerca.Testo.Replace("'", "''")));
            }

            if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
            {
                noteQuery = noteQuery.Where(n => n.TIPOOGGETTOASSOCIATO != null && n.TIPOOGGETTOASSOCIATO.Equals("D"));
            }
            else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
            {
                noteQuery = noteQuery.Where(n => n.TIPOOGGETTOASSOCIATO != null && n.TIPOOGGETTOASSOCIATO.Equals("F"));

            }
            var maxNumChar = this.GetNumeroMassimoCaratteri(filtroRicerca);
            var note = await noteQuery.ToListAsync();
            foreach (var n in note)
            {
                InfoNota nota = new();
                nota.Id = n.SYSTEM_ID.ToString();

                if (maxNumChar > 0)
                {
                    if (maxNumChar > n.TESTO.Length)
                        nota.Testo = n.TESTO;
                    else
                    {
                        nota.Testo = n.TESTO.Substring(0, maxNumChar);
                    }
                }
                else
                {
                    nota.Testo = n.TESTO;
                }

                nota.DataCreazione = n.DATACREAZIONE;
                nota.TipoVisibilita = this.GetTipoVisibilita(n.TIPOVISIBILITA);

                InfoUtenteCreatoreNota creatore = new InfoUtenteCreatoreNota()
                {
                    IdUtente = n.IDUTENTECREATORE.ToString(),
                    IdRuolo = n.IDRUOLOCREATORE.ToString(),
                    DescrizioneUtente = n.USER_ID,
                    DescrizioneRuolo = n.GROUP_NAME
                };

                nota.UtenteCreatore = creatore;

                nota.SolaLettura = !n.IDUTENTECREATORE.Equals(infoUtente.idPeople);

                if (n.IDRFASSOCIATO != null)
                {
                    nota.IdRfAssociato = n.IDRFASSOCIATO.ToString();
                }
                string idPeopleDelegato = n.IDPEOPLEDELEGATO != null ? n.IDPEOPLEDELEGATO.ToString() : null;

                if (!string.IsNullOrEmpty(idPeopleDelegato) && !idPeopleDelegato.Equals("0"))
                {
                    nota.IdPeopleDelegato = idPeopleDelegato;
                    nota.DescrPeopleDelegato = await this.GetDescUtenteNoFiltroDisabled(idPeopleDelegato);
                }
                else
                {
                    nota.IdPeopleDelegato = string.Empty;
                    nota.DescrPeopleDelegato = string.Empty;
                }

                output.Add(nota);
            }



            return output;
        }

        private async Task<string> GetDescUtenteNoFiltroDisabled(string idPeopleDelegato)
        {
            var ut = await this._pi3DbContext.PeopleEntities.AsNoTracking().FirstOrDefaultAsync(c => c.SYSTEM_ID == idPeopleDelegato.AsLong());
            string desc = string.Empty;
            if (ut != null)
            {
                desc = ut.VAR_COGNOME + " " + ut.VAR_NOME;
            }
            return desc;

        }

        private TipiVisibilitaNotaEnum GetTipoVisibilita(string visibilita)
        {
            if (visibilita.Equals("T"))
                return TipiVisibilitaNotaEnum.Tutti;
            else if (visibilita.Equals("F"))
                return TipiVisibilitaNotaEnum.RF;
            else if (visibilita.Equals("R"))
                return TipiVisibilitaNotaEnum.Ruolo;
            else if (visibilita.Equals("P"))
                return TipiVisibilitaNotaEnum.Personale;
            else
                return TipiVisibilitaNotaEnum.Tutti;
        }

        private int GetNumeroMassimoCaratteri(FiltroRicercaNote filtroRicerca)
        {
            if (filtroRicerca != null)
                return filtroRicerca.NumeroMassimoCaratteriTesto;
            else
                return 0;
        }

        protected virtual void InitCommonFields(ArrayList keyValues)
        {
            this.AppendKeyValue(DocumentCommonFields.OGGETTO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_CREAZIONE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.ID_DOCUMENTO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NOTE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TIPOLOGIA, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.RUOLO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CAP_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NAZIONE_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_CREATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NUM_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.SEGNATURA, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.REGISTRO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CODICE_REGISTRO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CAP_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NAZIONE_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_PROT, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.COD_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DESC_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.AMMINISTRAZIONE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.DATA_ORA_PROTOCOLLO, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.NUM_ALLEGATI, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UFF_REF_DESC, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UFF_REF_COD, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.COD_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CITTA_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL1_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.TEL2_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.FAX_UO_PADRE, string.Empty, keyValues);
            this.AppendKeyValue(DocumentCommonFields.CLASSIFICHE, string.Empty, keyValues);
        }

        /// <summary>
        /// Aggiunge chiavi (comune)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="description"></param>
        /// <param name="listaChiaviValori"></param>
        /// <returns></returns>
        private void AppendKeyValue(string key, string description, ArrayList keyValues)
        {
            bool found = false;

            foreach (Array item in keyValues)
            {
                if (item.GetValue(0).ToString().ToUpper().Equals(key.ToUpper()))
                {
                    item.SetValue(description, 1);
                    found = true;
                    break;
                }
            }

            if (!found)
                keyValues.Add(new string[2]
                            {
                                key,
                                (string.IsNullOrEmpty(description) ? string.Empty : description)
                            });
        }

        #endregion
    }
}