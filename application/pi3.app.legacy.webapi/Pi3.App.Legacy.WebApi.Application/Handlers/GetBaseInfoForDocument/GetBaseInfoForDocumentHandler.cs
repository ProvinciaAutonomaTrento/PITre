// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetBaseInfoForDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetBaseInfoForDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetBaseInfoForDocument
{
    public class GetBaseInfoForDocumentHandler : IRequestHandler<GetBaseInfoForDocumentRequest, GetBaseInfoForDocumentResult>
    {
        #region Public Members

        public GetBaseInfoForDocumentHandler(ILogger<GetBaseInfoForDocumentHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetBaseInfoForDocumentResult> Handle(GetBaseInfoForDocumentRequest request, CancellationToken cancellationToken)
        {

            List<DocsPaVO.documento.BaseInfoDoc> result = null;
            //Prendo paraMETRI RICHIESTA
            //.asLong() poer fare long number, da mettere alla fine 


            string idProfile = request.idProfile;
            string docNumber = request.docNumber;
            var versionNumber = request.versionNumber;

            // string idProfile, string docNumber, string versionNumber
            try
            {

                // Se idProfile non � specificato viene ricavato
                if (String.IsNullOrEmpty(idProfile))
                    idProfile = await GetIdProfileFromDocNumber(docNumber);

                // Altrimenti docNumber non � specificato viene ricavato
                if (String.IsNullOrEmpty(docNumber))
                    docNumber = await GetDocNumberFromIdProfile(idProfile);




                //query per lista
                var docNumberList = await this._dbContext.ProfileEntities.
                   Where(x => x.ID_DOCUMENTO_PRINCIPALE == idProfile.AsLong() || x.DOCNUMBER == docNumber.AsLong()).
                   Select(x => x.DOCNUMBER).ToListAsync();


                var dataSet = await this._dbContext.ProfileEntities.
                    Join(this._dbContext.ComponentEntities, p => p.DOCNUMBER, c => c.DOCNUMBER, (p, c) => new { p, c }).
                    Join(this._dbContext.VersionEntities, j1 => j1.c.VERSION_ID, v => v.VERSION_ID, (j1, v) => new { p = j1.p, c = j1.c, v }).
                    Where(j2 => docNumberList.Contains(j2.p.DOCNUMBER)).
                    Select(j2 => new
                    {
                        ID_DOCUMENTO_PRINCIPALE = j2.p.ID_DOCUMENTO_PRINCIPALE,
                        DOCNAME = j2.p.DOCNAME,
                        VAR_PROF_OGGETTO = j2.p.VAR_PROF_OGGETTO,
                        CHA_TIPO_PROTO = j2.p.CHA_TIPO_PROTO,
                        FILE_SIZE = j2.c.FILE_SIZE,
                        PATH = j2.c.PATH,
                        VAR_NOMEORIGINALE = j2.c.VAR_NOMEORIGINALE,
                        CHA_FIRMATO = j2.c.CHA_FIRMATO,
                        VERSION_LABEL = j2.v.VERSION_LABEL,
                        VERSION = j2.v.VERSION,
                        VERSION_ID = j2.v.VERSION_ID,
                        CHA_SEGNATURA = j2.v.CHA_SEGNATURA,
                        DOCNUMBER = j2.p.DOCNUMBER,
                        SYSTEM_ID = j2.p.SYSTEM_ID
                    }
                    ).
                    OrderBy(j2 => j2.VERSION_ID).ToListAsync();

                // ...si crea la lista che conterr� gli oggetti con le informazioni
                // di base sul documento e sui suoi eventuali allegati
                List<DocsPaVO.documento.BaseInfoDoc> baseInfoDocList = new List<DocsPaVO.documento.BaseInfoDoc>();


                //ciclo 
                if (dataSet != null)
                {
                    int indexAllegati = 0;

                    // ...si procede alla creazione degli oggetti con le informazioni
                    // sul documento richiesto e sui suoi eventuali allegati
                    // Per ogni riga nel data set...
                    foreach (var row in dataSet)
                    {
                        // ...si effettua un'interrogazione alla lista al fine di
                        // individuare un elemento con docNumber pari al docNumber
                        // specificato nella riga attualmente in esame
                        DocsPaVO.documento.BaseInfoDoc temp =
                            baseInfoDocList.Where(e => e.DocNumber == row.DOCNUMBER.ToString()).FirstOrDefault();

                        string versionLabel = string.Empty;

                        if (String.IsNullOrEmpty(row.ID_DOCUMENTO_PRINCIPALE.ToString()))
                            versionLabel = row.VERSION_LABEL.ToString();
                        else
                            versionLabel = FormatCodiceAllegato(++indexAllegati);

                        // Se � stato individuato un elemento...
                        if (temp != null)
                        {
                            // ...se il docNumber corrisponde a quello passato per parametro...
                            if (row.DOCNUMBER.ToString() == docNumber.ToString())
                            {
                                // ...se il parametro versionNumber � valorizzato...
                                if (!String.IsNullOrEmpty(versionNumber))
                                {
                                    // ...se la riga corrente ha VERSION uguale a quello passato per 
                                    // parametro si procede all'aggiornamento
                                    if (row.VERSION.ToString() == versionNumber)
                                    {
                                        UpdateBaseInfoDoc(
                                            temp,
                                            row.DOCNUMBER.ToString(),
                                            row.SYSTEM_ID.ToString(),
                                            !String.IsNullOrEmpty(row.ID_DOCUMENTO_PRINCIPALE.ToString()),
                                            row.VERSION.ToString(),
                                            row.DOCNAME.ToString(),
                                            row.VAR_NOMEORIGINALE.ToString(),
                                            row.FILE_SIZE.ToString(),
                                            row.VAR_PROF_OGGETTO.ToString(),
                                            row.CHA_TIPO_PROTO.ToString().ToUpper() != "G",
                                            versionLabel,
                                            row.VERSION_ID.ToString(),
                                            row.PATH.ToString());

                                        temp.Firmato = row.CHA_FIRMATO != null ? row.CHA_FIRMATO.ToString() : "0";
                                    }
                                }
                                else
                                {
                                    // ...altrimenti se VERSION della tupla attuale � maggiore
                                    // della versionNumber specificato in temp, si deve effettuare 
                                    // l'aggiornamento
                                    bool conSegnatura = false;
                                    //if (inoltroMassivo)
                                    //    conSegnatura = row.Table.Columns.Contains("CHA_SEGNATURA") && row["CHA_SEGNATURA"] != DBNull.Value && row["CHA_SEGNATURA"].ToString().Equals("1") ? true : false;
                                    if (Int32.Parse(row.VERSION.ToString()) > temp.VersionNumber && !conSegnatura)
                                        UpdateBaseInfoDoc(
                                          temp,
                                          row.DOCNUMBER.ToString(),
                                          row.SYSTEM_ID.ToString(),
                                          !String.IsNullOrEmpty(row.ID_DOCUMENTO_PRINCIPALE.ToString()),
                                          row.VERSION.ToString(),
                                          row.DOCNAME.ToString(),
                                          row.VAR_NOMEORIGINALE.ToString(),
                                          row.FILE_SIZE.ToString(),
                                          row.VAR_PROF_OGGETTO.ToString(),
                                          row.CHA_TIPO_PROTO.ToString().ToUpper() != "G",
                                          versionLabel,
                                          row.VERSION_ID.ToString(),
                                          row.PATH.ToString());
                                }
                            }
                            else
                            {
                                if (Int32.Parse(row.VERSION.ToString()) > temp.VersionNumber)
                                    UpdateBaseInfoDoc(
                                      temp,
                                      row.DOCNUMBER.ToString(),
                                      row.SYSTEM_ID.ToString(),
                                      !String.IsNullOrEmpty(row.ID_DOCUMENTO_PRINCIPALE.ToString()),
                                      row.VERSION.ToString(),
                                      row.DOCNAME.ToString(),
                                      row.VAR_NOMEORIGINALE.ToString(),
                                      row.FILE_SIZE.ToString(),
                                      row.VAR_PROF_OGGETTO.ToString(),
                                      row.CHA_TIPO_PROTO.ToString().ToUpper() != "G",
                                      versionLabel,
                                      row.VERSION_ID.ToString(),
                                      row.PATH.ToString());
                            }
                        }
                        else
                        {
                            baseInfoDocList.Add(CreateBaseInfoDoc(
                                row.DOCNUMBER.ToString(),
                                row.SYSTEM_ID.ToString(),
                                !String.IsNullOrEmpty(row.ID_DOCUMENTO_PRINCIPALE.ToString()),
                                row.VERSION.ToString(),
                                row.DOCNAME.ToString(),
                                row.VAR_NOMEORIGINALE?.ToString(),
                                row.FILE_SIZE.ToString(),
                                row.VAR_PROF_OGGETTO.ToString(),
                                row.CHA_TIPO_PROTO.ToString().ToUpper() != "G",
                                versionLabel,
                                row.VERSION_ID.ToString(),
                                row.PATH?.ToString(),
                                row.CHA_FIRMATO != null ? row.CHA_FIRMATO.ToString() : "0"));
                        }
                    }
                }
                else
                    // ...altrimenti si lancia un'eccezione per segnalare l'insuccesso
                    throw new Exception("Non � stato possibile recuperare le informazioni sul documento.");



                //

                result = baseInfoDocList;





            }
            catch (Exception)
            {

                throw;
            };

            return new GetBaseInfoForDocumentResult(result);
        }

        private async Task<string> GetDocNumberFromIdProfile(string idProfile)
        {
            var profileNumber = await this._dbContext.ProfileEntities.
                   Where(x => x.SYSTEM_ID == idProfile.AsLong()).
                   Select(x => x.DOCNUMBER).FirstOrDefaultAsync();

            return profileNumber.ToString();
        }

        private async Task<string> GetIdProfileFromDocNumber(string docNumber)
        {
            var profileNumber = await this._dbContext.ProfileEntities.
                   Where(x =>  x.DOCNUMBER == docNumber.AsLong()).
                   Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();


            return profileNumber.ToString();

            // "SELECT p.SYSTEM_ID FROM profile p WHERE p.DOCNUMBER = {0}",
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetBaseInfoForDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private static string FormatCodiceAllegato(int indexAllegato)
        {
            return string.Format("A{0:0#}", indexAllegato);
        }

        private void UpdateBaseInfoDoc(DocsPaVO.documento.BaseInfoDoc objToUpdate,
          string docNumber,
          string idProfile,
          bool isAttachment,
          string versionNumber,
          string docName,
          string OriginalFileName,
          string fileSize,
          string descritpion,
          bool isProto,
          string versionLabel,
          string versionId,
          string path)
        {
            // Aggiornamento dei dati
            objToUpdate.Name = docName;
            objToUpdate.FileSize = Int32.Parse(fileSize);
            objToUpdate.HaveFile = objToUpdate.FileSize > 0;
            objToUpdate.Description = descritpion;
            objToUpdate.IsProto = isProto;
            objToUpdate.VersionNumber = Int32.Parse(versionNumber);
            objToUpdate.VersionLabel = versionLabel;
            objToUpdate.VersionId = versionId;
            objToUpdate.FileName = path;
            objToUpdate.OriginalFileName = OriginalFileName;
            objToUpdate.IsAttachment = isAttachment;
            if (objToUpdate.HaveFile && objToUpdate.FileName.Contains("\\"))
                objToUpdate.Path = objToUpdate.FileName.Substring(0, objToUpdate.FileName.LastIndexOf("\\"));
            objToUpdate.DocNumber = docNumber;
            objToUpdate.IdProfile = idProfile;
            objToUpdate.IsPecAttachment = objToUpdate.Description.StartsWith("Ricevuta di ritorno delle Mail");
        }

        private DocsPaVO.documento.BaseInfoDoc CreateBaseInfoDoc(
           string docNumber,
           string idProfile,
           bool isAttachment,
           string versionNumber,
           string docName,
           string OriginalFileName,
           string fileSize,
           string description,
           bool isProto,
           string versionLabel,
           string versionId,
           string path,
           string firmato)
        {
            // L'oggetto da restituire
            DocsPaVO.documento.BaseInfoDoc baseInfoDoc = new DocsPaVO.documento.BaseInfoDoc();

            // Richiesta di aggiornamento dei dati di baseInfoDoc
            UpdateBaseInfoDoc(baseInfoDoc, docNumber, idProfile, isAttachment, versionNumber, docName, OriginalFileName,
                fileSize, description, isProto, versionLabel, versionId, path);
            baseInfoDoc.Firmato = firmato;

            // Restituzione dell'oggetto creato
            return baseInfoDoc;

        }
        #endregion
    }
}