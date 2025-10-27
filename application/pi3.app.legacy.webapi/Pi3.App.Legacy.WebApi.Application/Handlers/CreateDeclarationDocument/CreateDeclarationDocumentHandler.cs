// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.InstanceAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateDeclarationDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.CreateDeclarationDocument;

using DocsPaVO.documento;
using DocsPaVO.Mobile;
using DocsPaVO.utente;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using DocumentFormat.OpenXml.Office2016.Excel;
using Pi3.Core.Services.Principal;
using Pi3.Core.SeedWork;
using Pi3.App.Legacy.WebApi.Application.Extensions;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CreateDeclarationDocument
{
    public class CreateDeclarationDocumentHandler : IRequestHandler<CreateDeclarationDocumentRequest, CreateDeclarationDocumentResult>
    {
        protected readonly ILogger<CreateDeclarationDocumentHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;



        private async Task<bool> AddAttachment(string docnumber, string descrizione, FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                Allegato all = new Allegato();
                all.docNumber = docnumber;
                all.TypeAttachment = 1;
                all.descrizione = descrizione.Replace("/", "//").Replace("'", "''");
                all.dataInserimento = DateTime.Today.ToString();
                all.version = "1";
                all = ( await this._mediator.Send(new Application.Requests.DocumentoAggiungiAllegato(infoUtente, all)) ).output;
                if (all != null)
                {
                    if ((!string.IsNullOrEmpty(fileReq.fileSize)) && Convert.ToInt32(fileReq.fileSize) > 0)
                    {
                        DocsPaVO.documento.FileDocumento fileDoc = (await this._mediator.Send( new Application.Requests.DocumentoGetFileFirmato(fileReq, infoUtente))).output;
                        if (fileDoc != null)
                        {
                            var r = (await this._mediator.Send(new Application.Requests.DocumentoPutFileNoException(all, fileDoc, infoUtente))).output;
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return result;
        }

        private async Task<DocsPaVO.InstanceAccess.InstanceAccess> CreateDec(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {

            bool result = true;
            DocsPaVO.InstanceAccess.InstanceAccess newInstance = null;
            string reportDichiarazioneConformita = string.Empty;
            string detailsDocument = @"{\rtf1\ansi";
            string typeRequest = string.Empty;
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();


            DocsPaVO.documento.SchedaDocumento newDoc = (await this._mediator.Send(new Application.Requests.NewSchedaDocumento(infoUtente) )).output;
            newDoc.oggetto = new DocsPaVO.documento.Oggetto() { descrizione = "Dichiarazione di conformita' per l'istanza " + instance.ID_INSTANCE_ACCESS };
            newDoc.tipoProto = "G";

            DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Application.Requests.GetTemplateInstanceAccess(infoUtente))).output;

            if (template != null)
            {
                InstanceAccessDocument instanceDoc = (from i in instance.DOCUMENTS where i.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE) select i).FirstOrDefault();
                if (instanceDoc != null)
                {
                    var res = (await this._mediator.Send(new Application.Requests.RemoveInstanceAccessDocuments((new List<InstanceAccessDocument>() { instanceDoc }).ToArray(), infoUtente)));
                    instance.DOCUMENTS.RemoveAll(d => d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(instanceDoc.ID_INSTANCE_ACCESS_DOCUMENT));
                }

                newDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                newDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                newDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                newDoc.template = template;

                reportDichiarazioneConformita = System.IO.File.ReadAllText(template.PATH_MODELLO_1.PathAsUnixPath());
                reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XID_ISTANZA", instance.ID_INSTANCE_ACCESS);
                reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XDATA_CREAZIONE", instance.CREATION_DATE.ToShortDateString());
                newDoc = (await this._mediator.Send(new Application.Requests.DocumentoAddDocGrigia(newDoc, infoUtente, ruolo))).output;
            }
            if (newDoc != null)
            {
                instance.DOCUMENTS = (from d in instance.DOCUMENTS orderby d.TYPE_REQUEST ascending select d).ToList();

                if (instance.DOCUMENTS != null)
                {
                    foreach (InstanceAccessDocument doc in instance.DOCUMENTS)
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDoc = ( await this._mediator.Send(new Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER, doc.INFO_DOCUMENT.DOCNUMBER))).output;
                        bool resultAtt = true;

                        if (doc.ENABLE)
                        {
                            DocsPaVO.documento.FileRequest fileReq = schedaDoc.documenti[0] as DocsPaVO.documento.FileRequest;
                            resultAtt = await AddAttachment(newDoc.docNumber, doc.INFO_DOCUMENT.OBJECT, fileReq, infoUtente);
                            detailsDocument += await BuildDocumentRTF(doc, infoUtente);
                        }
                        if (resultAtt)
                        {
                            if (schedaDoc.allegati != null && schedaDoc.allegati.Count() > 0)
                            {
                                foreach (Allegato a in schedaDoc.allegati)
                                {
                                    if ((from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(a.docNumber) && att.ENABLE select att).FirstOrDefault() != null)
                                    {
                                        detailsDocument += await BuildAttachmentRTF(a, doc);
                                        if (!await AddAttachment(newDoc.docNumber, schedaDoc.oggetto.descrizione, a, infoUtente))
                                        {
                                            result = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        if (!result)
                        {
                            break;
                        }

                    }

                    if (result)
                    {
                        detailsDocument += @"}";
                        reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XDOCUMENTI", detailsDocument);
                        SchedaDocumento schedaDoc = (await this._mediator.Send( new Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, newDoc.docNumber, newDoc.docNumber))).output;
                        fileDoc.content = this.ToByteArray(reportDichiarazioneConformita);
                        fileDoc.length = fileDoc.content.Length;
                        fileDoc.contentType = "application/rtf";
                        fileDoc.name = "DichiarazioneConformita.rtf";
                        FileRequest fileReq = schedaDoc.documenti[0] as FileRequest;
                        await this._mediator.Send(new Application.Requests.DocumentoPutFileNoException(fileReq, fileDoc, infoUtente));
                        InstanceAccessDocument instanceDoc = new InstanceAccessDocument()
                        {
                            ID_INSTANCE_ACCESS = instance.ID_INSTANCE_ACCESS,
                            DOCNUMBER = newDoc.docNumber,
                            ENABLE = true,
                            INFO_DOCUMENT = new InfoDocument() { DOCNUMBER = newDoc.docNumber },
                            ATTACHMENTS = BuildInstanceAccessAttachments(schedaDoc.allegati.ToList())
                        };

                        result = (await this._mediator.Send(new Application.Requests.InsertInstanceAccessDocuments(new List<InstanceAccessDocument>() { instanceDoc }.ToArray(), infoUtente))).output;
                        if (result)
                        {
                            newInstance = (await this._mediator.Send(new Application.Requests.GetInstanceAccessById(instance.ID_INSTANCE_ACCESS, infoUtente))).output;
                            if (newInstance == null)
                                result = false;
                        }
                    }
                }
                
            }
            return newInstance;

        }

        private  List<InstanceAccessAttachments> BuildInstanceAccessAttachments(List<Allegato> attachments)
        {
            List<InstanceAccessAttachments> instanceAttachments = new List<InstanceAccessAttachments>();
            if (attachments != null && attachments.Count() > 0)
            {
                foreach (Allegato a in attachments)
                {
                    InstanceAccessAttachments att = new InstanceAccessAttachments()
                    {
                        ID_ATTACH = a.docNumber,
                        ENABLE = true
                    };
                    instanceAttachments.Add(att);
                }
            }
            return instanceAttachments;
        }

        private  byte[] ToByteArray(string str)
        {
            char[] charArray = str.ToCharArray();
            System.Collections.ArrayList byteArr = new System.Collections.ArrayList();
            //byte[] res=new byte[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                if ((int)charArray[i] > 255)
                {
                    string utf = "\\u" + ((int)charArray[i]) + "G";
                    char[] utfChars = utf.ToCharArray();
                    for (int j = 0; j < utfChars.Length; j++)
                    {
                        byteArr.Add((byte)utfChars[j]);
                    }
                }
                else
                {
                    byteArr.Add((byte)charArray[i]);
                }
                //res[i]=(byte) charArray[i];
            }
            byte[] res = (byte[])byteArr.ToArray(typeof(byte));
            return res;
        }
        private async Task<string> BuildAttachmentRTF(Allegato all, InstanceAccessDocument doc)
        {
            string result = string.Empty;
            string tab = string.Empty;
            string hash = string.Empty;

            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant,true);
            
            if (!String.IsNullOrEmpty(all.path))
            {
                var aggregate = await this._documentBlobRepository.Get(idTenant, all.path);
                aggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);
                hash = aggregate.Hash?.ToString();
            }
            

            tab = doc.ENABLE ? @"\tab\tab" : @"\tab";
            if (!doc.ENABLE)
            {
                result += @"\line\b" + tab + @" Tipo richiesta: \b0 " + doc.TYPE_REQUEST;

                string num_proto = string.Empty;
                string dataProto = string.Empty;
                string dataCreazione = string.Empty;
                Dictionary<string, string> dati = await this.GetInfoDocument(doc.INFO_DOCUMENT.DOCNUMBER);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("num_proto"))
                    {
                        num_proto = dati["num_proto"];
                    }
                    if (dati.ContainsKey("data_creazione"))
                    {
                        dataCreazione = dati["data_creazione"];
                    }
                    if (dati.ContainsKey("data_protocollazione"))
                    {
                        dataProto = dati["data_protocollazione"];
                    }
                }
                result += @"\line\b" + tab + @" Doc: \b0 " + (string.IsNullOrEmpty(num_proto) ? doc.INFO_DOCUMENT.DOCNUMBER : num_proto);
                result += @"\line\b" + tab + @" Data protocollo/data creazione grigio: \b0 " + (string.IsNullOrEmpty(num_proto) ? dataCreazione : dataProto);
                result += @"\line\b" + tab + @" Classificazione: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_CLASSIFICATION : string.Empty);
                result += @"\line\b" + tab + @" Codice fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_PROJECT : string.Empty);
                result += @"\line\b" + tab + @" Descrizione fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.DESCRIPTION_PROJECT : string.Empty);
            }
            result += @"\line\b" + tab + @" Descrizione: \b0  " + all.descrizione;
            result += @"\line\b " + tab + @" Hash: \b0\fs20 " + hash + @"\fs24";

            result = @"\i" + result + @"\i0\line";
            return result;
        }

        private async Task<string> BuildDocumentRTF(InstanceAccessDocument doc, InfoUtente infoUtente)
        {
            string result = string.Empty;
            result += @"\line\tab\b Tipo richiesta: \b0 " + doc.TYPE_REQUEST;
            if (string.IsNullOrEmpty(doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE))
            {
                result += @"\line\tab\b Doc: \b0 " + (string.IsNullOrEmpty(doc.INFO_DOCUMENT.NUMBER_PROTO) ? doc.INFO_DOCUMENT.DOCNUMBER : doc.INFO_DOCUMENT.NUMBER_PROTO);
                result += @"\line\tab\b Data protocollo/data creazione grigio: \b0 " + doc.INFO_DOCUMENT.DATE_CREATION.ToShortDateString();
                result += @"\line\tab\b Tipologia documento: \b0 " + doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO + @" - \b Numero repertorio: \b0 " + doc.INFO_DOCUMENT.COUNTER_REPERTORY;
                result += @"\line\tab\b Oggetto: \b0 " + doc.INFO_DOCUMENT.OBJECT;
                result += @"\line\tab\b Hash: \b0\fs20  " + doc.INFO_DOCUMENT.HASH + @"\fs24";
                result += @"\line\tab\b Classificazione: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_CLASSIFICATION : string.Empty);
                result += @"\line\tab\b Codice fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_PROJECT : string.Empty);
                result += @"\line\tab\b Descrizione fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.DESCRIPTION_PROJECT : string.Empty);
            }
            else
            {
                string num_proto = string.Empty;
                string dataProto = string.Empty;
                string dataCreazione = string.Empty;
                Dictionary<string, string> dati = await this.GetInfoDocument(doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("num_proto"))
                    {
                        num_proto = dati["num_proto"];
                    }
                    if (dati.ContainsKey("data_creazione"))
                    {
                        dataCreazione = dati["data_creazione"];
                    }
                    if (dati.ContainsKey("data_protocollazione"))
                    {
                        dataProto = dati["data_protocollazione"];
                    }
                }
                result += @"\line\tab\b Doc: \b0 " + (string.IsNullOrEmpty(num_proto) ? doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE : num_proto);
                result += @"\line\tab\b Data protocollo/data creazione grigio: \b0 " + (string.IsNullOrEmpty(num_proto) ? dataCreazione : dataProto);
                result += @"\line\b\tab\ Descrizione: \b0  " + doc.INFO_DOCUMENT.OBJECT;
                result += @"\line\b\tab\ Hash: \b0\fs20 " + doc.INFO_DOCUMENT.HASH + @"\fs24";

            }
            result = @"\i" + result + @"\i0\line";
            return result;
        }

        private async Task<Dictionary<string, string>> GetInfoDocument(string idDocumento)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            var dataSet = await this._dbContext.ProfileEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idDocumento.AsLong()).Select( a => new
            {
                a.NUM_PROTO,
                DTA_PROTO = a.DTA_PROTO.AsDateFormat(),
                CREATION_DATE = a.CREATION_DATE.AsDateFormat()
            }).ToListAsync();

            foreach (var row in dataSet)
            {
                result.Add("num_proto", row.NUM_PROTO != null ? row.NUM_PROTO.ToString() : string.Empty);
                result.Add("data_creazione", row.CREATION_DATE);
                result.Add("data_protocollazione", row.DTA_PROTO);
            }
            return result;
        }

     

        public CreateDeclarationDocumentHandler(
            ILogger<CreateDeclarationDocumentHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            IDocumentBlobRepository documentBlobRepository,
            IClaimsPrincipalService claimsPrincipalService
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._documentBlobRepository = documentBlobRepository;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<CreateDeclarationDocumentResult> Handle(CreateDeclarationDocumentRequest request, CancellationToken cancellationToken)
        {
            InstanceAccess output = null;
            try
            {
                output = await CreateDec(request.instance,request.infoUser,request.role);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
