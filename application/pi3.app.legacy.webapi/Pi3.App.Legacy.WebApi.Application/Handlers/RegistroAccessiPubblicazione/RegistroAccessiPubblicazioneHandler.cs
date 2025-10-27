// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.RegistroAccessi;
using DocsPaVO.RegistroAccessi.FOIA;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RegistroAccessiPubblicazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.RegistroAccessiPubblicazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RegistroAccessiPubblicazione
{
    public class RegistroAccessiPubblicazioneHandler : IRequestHandler<RegistroAccessiPubblicazioneRequest, RegistroAccessiPubblicazioneResult>
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger<RegistroAccessiPubblicazioneHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        #region GenerateReport
        private async Task<List<DocsPaVO.documento.FileDocumento>> RegistroAccessiGetReport(List<DocsPaVO.filtri.FiltroRicerca> searchFilters, string typology)
        {
            List<DocsPaVO.documento.FileDocumento> reports = new List<DocsPaVO.documento.FileDocumento>();
            string descrizioneAmm = string.Empty;

            try
            {
                searchFilters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore = typology;
                string idAmm = searchFilters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;
                if (!string.IsNullOrEmpty(idAmm))
                {
                    descrizioneAmm = ((await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(idAmm))).output).Descrizione;

                }

                DataSet dataSet = await this.ExtractDataRegistriAccessi(searchFilters);
                if (dataSet == null)
                {
                    throw new Exception(Resources.DataSetMessage);
                }


                DocsPaVO.Report.PrintReportRequestDataset printRequest = new DocsPaVO.Report.PrintReportRequestDataset();
                DocsPaVO.Report.PrintReportResponse printResponse = new DocsPaVO.Report.PrintReportResponse();

                printRequest.ReportKey = "RegistroAccessiPublish";
                printRequest.ContextName = "RegistroAccessiPublish";
                printRequest.Title = string.IsNullOrEmpty(descrizioneAmm) ? "Registro degli accessi" : "Registro degli accessi - " + descrizioneAmm;
                printRequest.SubTitle = typology;
                printRequest.AdditionalInformation = GetAdditionalInformation(searchFilters);
                printRequest.InputDataset = dataSet;


                string ext = string.Empty;
                string typologyFilename = typology.Replace(" ", "_");

                // 1 - Report Foglio di calcolo
                printRequest.ReportType = RegistroAccessiGetReportType(searchFilters, out ext);
                printResponse = (await this._mediator.Send(new Application.Requests.GenerateReport(printRequest))).output;
                printResponse.Document.name = string.Format(Resources.ExcelFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                printResponse.Document.fullName = string.Format(Resources.ExcelFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                reports.Add(printResponse.Document);

                try
                {
                    // 2 - Report PDF
                    printRequest.ReportType = DocsPaVO.Report.ReportTypeEnum.PDF;
                    printResponse = (await this._mediator.Send(new Application.Requests.GenerateReport(printRequest))).output;
                    printResponse.Document.name = string.Format(Resources.PdfFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                    printResponse.Document.fullName = string.Format(Resources.PdfFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                    reports.Add(printResponse.Document);
                }
                catch (Exception x)
                {

                }


                // 3 - Report XML FOIA
                DocsPaVO.documento.FileDocumento fileDocXml = new DocsPaVO.documento.FileDocumento();
                fileDocXml.content = RegistroAccessiGetXML(dataSet, typology);
                if (fileDocXml.content == null)
                {
                    throw new Exception(Resources.XmlGenerationMessage);
                }
                fileDocXml.length = fileDocXml.content.Length;
                fileDocXml.contentType = "text/xml";
                fileDocXml.estensioneFile = "xml";
                fileDocXml.name = string.Format(Resources.XmlFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                fileDocXml.fullName = string.Format(Resources.XmlFullName, typologyFilename, DateTime.Now.ToString("yyyyMMdd"));
                reports.Add(fileDocXml);

                return reports;

            }
            catch (Exception ex)
            {
                descrizioneAmm = string.Empty;
            }



            return reports;
        }
        #endregion
        #region report extraction and zip creation
        private async Task<RegistroAccessiReportResponse> PubblicaReg(RegistroAccessiReportRequest request)
        {
            RegistroAccessiReportResponse response = new RegistroAccessiReportResponse();


            // Lista dei report da produrre
            List<DocsPaVO.documento.FileDocumento> reportList = new List<DocsPaVO.documento.FileDocumento>();

            // Accesso documentale
            reportList.AddRange(await this.RegistroAccessiGetReport(request.filters, "Accesso Documentale"));

            // Accesso generalizzato e civico
            reportList.AddRange(await this.RegistroAccessiGetReport(request.filters, "Accesso Generalizzato e Civico"));

            // Archivio da restituire
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            fileDoc.contentType = "application/octet-stream";
            fileDoc.name = string.Format(Resources.ZipFullName, DateTime.Now.ToString("yyyyMMdd"));
            fileDoc.fullName = string.Format(Resources.ZipFullName, DateTime.Now.ToString("yyyyMMdd"));
            fileDoc.estensioneFile = "zip";

            fileDoc.content = this.RegistroAccessiCreateZipArchive(reportList);
            fileDoc.length = fileDoc.content.Length;

            response.document = fileDoc;
            response.success = true;

            return response;
        }

        private byte[] RegistroAccessiCreateZipArchive(List<DocsPaVO.documento.FileDocumento> reportList)
        {
            byte[] result;

            using (MemoryStream outputStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(outputStream, ZipArchiveMode.Create))
                {
                    foreach (DocsPaVO.documento.FileDocumento report in reportList)
                    {
                        ZipArchiveEntry entry = archive.CreateEntry(report.fullName);
                        using (var entryStream = entry.Open())
                        {
                            using (MemoryStream fileContent = new MemoryStream(report.content))
                            {
                                fileContent.CopyTo(entryStream);
                            }
                        }


                    }
                }

                result = outputStream.ToArray();
            }
            return result;
        }




        private byte[] RegistroAccessiGetXML(DataSet dataSet, string typology)
        {
            try
            {

                List<RichiestaDiAccesso> list = new List<RichiestaDiAccesso>();

                string folderId = string.Empty;

                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    if (!string.IsNullOrEmpty(dataRow["ID_PROJECT"].ToString()))
                    {
                        if (string.IsNullOrEmpty(folderId) || !dataRow["ID_PROJECT"].ToString().Equals(folderId))
                        {
                            // E' una nuova riga
                            folderId = dataRow["ID_PROJECT"].ToString();

                            RichiestaDiAccesso rda = new RichiestaDiAccesso();
                            rda.StatoRichiesta = dataRow["STATO_FASC"].ToString() == "C" ? StatoType.Chiusa : StatoType.InCorso;
                            rda.Oggetto = new DocsPaVO.RegistroAccessi.FOIA.Oggetto() { Sintesi = dataRow["DESCRIZIONE"].ToString() };

                            SetValoreCampoProfilato(rda, dataRow["NOME_CAMPO"].ToString(), dataRow["VALORE_CAMPO"].ToString());

                            list.Add(rda);
                        }
                        else
                        {
                            // E' una riga esistente
                            RichiestaDiAccesso rda = list.Last();
                            if (rda != null)
                            {
                                SetValoreCampoProfilato(rda, dataRow["NOME_CAMPO"].ToString(), dataRow["VALORE_CAMPO"].ToString());
                            }
                        }
                    }
                }


                foreach (RichiestaDiAccesso rda in list)
                {
                    SetStatoRichiesta(rda, typology);
                    CheckValues(rda);
                }


                byte[] serializedXML = GetSerializedXML(list);


                return serializedXML;
            }
            catch (Exception ex)
            {
                this._logger.LogDebug("Errore nella produzione dell'XML FOIA.\r\n{0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }
        #endregion

        #region data processing utils
        private void CheckValues(RichiestaDiAccesso rda)
        {
            // Controllo su differimento
            if (rda.Esito != null && rda.Esito.TipoEsito != null && rda.Esito.TipoEsito == EsitoType.Differimento)
            {
                rda.Esito = null;
            }
            if (rda.EsitoRiesame != null && rda.EsitoRiesame.TipoEsito != null && rda.EsitoRiesame.TipoEsito == EsitoType.Differimento)
            {
                rda.EsitoRiesame = null;
            }
        }
        private byte[] GetSerializedXML(List<RichiestaDiAccesso> reg)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(List<RichiestaDiAccesso>), new XmlRootAttribute("RegistroAccessi"));

            MemoryStream ms = new MemoryStream();

            serializer.Serialize(ms, reg);

            byte[] serializedXML = ms.ToArray();

            return serializedXML;
        }

        private void SetStatoRichiesta(RichiestaDiAccesso rda, string typology)
        {
            // Implementazione attuale:
            // La richiesta è CHIUSA se è definito un esito e non è definita la data notifica del blocco esito successivo
            // La richiesta è SOSPESA se sono presenti controinteressati ma non è stato ancora definito un esito
            // La richiesta è IN CORSO negli altri casi

            rda.StatoRichiesta = StatoType.InCorso;

            if (rda.Esito == null)
            {
                if (rda.PresenzaControinteressati)
                    rda.StatoRichiesta = StatoType.Sospesa;
            }
            else
            {
                if (rda.EsitoRiesame == null && typology == "Accesso Generalizzato e Civico")
                {
                    if (!string.IsNullOrEmpty(rda.DataNotificaRiesame))
                        rda.StatoRichiesta = StatoType.Chiusa;
                }
                else
                {
                    if (rda.EsitoRicorso == null)
                    {
                        if (!string.IsNullOrEmpty(rda.DataNotificaRicorso))
                            rda.StatoRichiesta = StatoType.Chiusa;
                    }
                    else
                    {
                        rda.StatoRichiesta = StatoType.Chiusa;
                    }
                }
            }

        }
        private string FormatDate(string dateString)
        {
            try
            {
                string result = getDate(dateString);
                string d = result.Split('/')[0];
                string m = result.Split('/')[1];
                string y = result.Split('/')[2];

                result = y + "-" + m + "-" + d;
                return result;
            }
            catch (Exception ex)
            {
                return dateString;
            }
        }

        private string getDate(string date)
        {
            string retVal = string.Empty;

            if (date.Length < 10)
            {
                retVal = date;
            }
            else
            {
                retVal = date.Substring(0, 10);
            }

            return retVal;
        }


        private void SetEsito(Esito esitoElement, string esitoValue)
        {
            switch (esitoValue.ToUpper())
            {
                case "ACCOGLIMENTO":
                    esitoElement.TipoEsito = EsitoType.Accoglimento;
                    break;
                case "DIFFERIMENTO":
                    // al momento non è gestito
                    // il campo verrà pulito in seguito
                    esitoElement.TipoEsito = EsitoType.Differimento;
                    break;
                case "ACCOGLIMENTO PARZIALE":
                case "RIFIUTO PARZIALE":
                    esitoElement.TipoEsito = EsitoType.AccoglimentoParziale;
                    break;
                case "RIFIUTO TOTALE":
                case "NON ACCOGLIMENTO":
                    esitoElement.TipoEsito = EsitoType.Rifiuto;
                    break;
                default:
                    break;
            }
        }
        private void SetValoreCampoProfilato(RichiestaDiAccesso rda, string name, string value)
        {
            switch (name.ToUpper().Trim())
            {
                case "DATA DI ARRIVO DELLA DOMANDA":
                    rda.DataCreazione = FormatDate(value);
                    break;
                case "PRESENZA CONTRO INTERESSATI":
                case "PRESENZA CONTROINTERESSATI":
                    if (value.ToUpper() == "SI")
                        rda.PresenzaControinteressati = true;
                    else
                        rda.PresenzaControinteressati = false;
                    break;
                case "ESITO":
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                        {
                            rda.Esito = new Esito();
                        }
                        SetEsito(rda.Esito, value);
                    }
                    else
                    {
                        rda.Esito = null;
                    }
                    break;
                case "DATA RISPOSTA":
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                        {
                            rda.Esito = new Esito();
                        }
                        rda.Esito.DataEsito = FormatDate(value);
                    }
                    break;
                case "MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                            rda.Esito = new Esito();

                        SetMotivoRifiuto(rda.Esito, value);
                    }
                    break;
                case "ALTRO - MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.Esito == null)
                            rda.Esito = new Esito();
                        rda.Esito.SintesiMotivazione = value;
                    }
                    break;
                case "RIESAME - ESITO":
                    if (!String.IsNullOrEmpty(value) && value.ToUpper() != "DIFFERIMENTO")
                    {
                        if (rda.EsitoRiesame == null)
                        {
                            rda.EsitoRiesame = new Esito();
                        }
                        SetEsito(rda.EsitoRiesame, value);
                    }
                    else
                    {
                        rda.EsitoRiesame = null;
                    }
                    break;
                case "RIESAME - MOTIVI DEL RIFIUTO PARZIALE, DEL RIFIUTO TOTALE O DEL DIFFERIMENTO":
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (rda.EsitoRiesame == null)
                            rda.EsitoRiesame = new Esito();

                        SetMotivoRifiuto(rda.EsitoRiesame, value);
                    }
                    break;
                case "RIESAME - DATA RISPOSTA":
                    if (!String.IsNullOrEmpty(value))
                    {
                        if (rda.EsitoRiesame == null)
                            rda.EsitoRiesame = new Esito();

                        rda.EsitoRiesame.DataEsito = FormatDate(value);
                    }
                    break;
                case "RICORSO - ESITO":
                    if (!String.IsNullOrEmpty(value) && value.ToUpper() != "DIFFERIMENTO")
                    {
                        if (rda.EsitoRicorso == null)
                        {
                            rda.EsitoRicorso = new Esito();
                        }
                        SetEsito(rda.EsitoRicorso, value);
                    }
                    else
                    {
                        rda.EsitoRicorso = null;
                    }
                    break;
                case "RIESAME - DATA DI PRESENTAZIONE DELLA DOMANDA":
                    if (!String.IsNullOrEmpty(value))
                    {
                        rda.DataNotificaRiesame = value;
                    }
                    break;
                case "RICORSO – DATA DI NOTIFICAZIONE DEL RICORSO GIURISDIZIONALE DELL'AMMINISTRAZIONE":
                    if (!String.IsNullOrEmpty(value))
                    {
                        rda.DataNotificaRicorso = value;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetMotivoRifiuto(Esito esitoElement, string rifiuto)
        {
            esitoElement.MotiviRifiuto = new MotiviRifiuto();

            switch (rifiuto)
            {
                case "sicurezza pubblica e ordine pubblico":
                    esitoElement.MotiviRifiuto.SicurezzaPubblica = true;
                    break;
                case "sicurezza nazionale":
                    esitoElement.MotiviRifiuto.SicurezzaNazionale = true;
                    break;
                case "difesa e questioni militari":
                    esitoElement.MotiviRifiuto.Difesa = true;
                    break;
                case "relazioni internazionali":
                    esitoElement.MotiviRifiuto.RelazioniInternazionali = true;
                    break;
                case "politica e stabilità finanziaria ed economica dello Stato":
                    esitoElement.MotiviRifiuto.Politica = true;
                    break;
                case "conduzione di indagini sui reati e loro perseguimento":
                    esitoElement.MotiviRifiuto.ConduzioneIndaginiReati = true;
                    break;
                case "segreto di Stato":
                    esitoElement.MotiviRifiuto.SegretoDiStato = true;
                    break;
                case "a protezione di interessi economici e commerciali":
                    esitoElement.MotiviRifiuto.InteressiEconomiciCommerciali = true;
                    break;
                case "a protezione di dati personali":
                    esitoElement.MotiviRifiuto.ProtezioneDatiPersonali = true;
                    break;
                case "divieto di divulgazione per norma di legge":
                    esitoElement.MotiviRifiuto.LibertaSegretezzaCorrispondenza = true;
                    break;
                case "dati richiesti non sono detenuti dall'Amministrazione provinciale":
                    esitoElement.MotiviRifiuto.InformazioneNonEsistente = true;
                    break;
                case "dati richiesti richiedono un'elaborazione a cui l'Amministrazione non è tenuta":
                case "domanda manifestamente irragionevole cd. richiesta massiva":
                    esitoElement.MotiviRifiuto.RichiestaOnerosa = true;
                    break;
                case "tutela del regolare svolgimento di attività ispettive":
                    esitoElement.MotiviRifiuto.AttivitaIspettive = true;
                    break;
                case "altri motivi (descriverli nel campo successivo)":
                    esitoElement.MotiviRifiuto.AltriMotivi = true;
                    break;
            }
        }

        private DocsPaVO.Report.ReportTypeEnum RegistroAccessiGetReportType(List<DocsPaVO.filtri.FiltroRicerca> filters, out string extString)
        {
            string reportType = filters.Where(f => f.argomento == "formato_export").FirstOrDefault().valore;
            DocsPaVO.Report.ReportTypeEnum reportTypeEnum;

            if (!String.IsNullOrEmpty(reportType) && reportType.ToUpper() == "ODS")
            {
                reportTypeEnum = DocsPaVO.Report.ReportTypeEnum.ODS;
                extString = "ods";
            }
            else
            {
                reportTypeEnum = DocsPaVO.Report.ReportTypeEnum.Excel;
                extString = "xls";
            }

            return reportTypeEnum;
        }

        private string GetAdditionalInformation(List<DocsPaVO.filtri.FiltroRicerca> searchFilters)
        {
            string additionalInformation = string.Empty;

            string creationDateInterval = searchFilters.Where(f => f.argomento == "data_creazione").FirstOrDefault().valore;
            string creationDateFrom = searchFilters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore;
            string creationDateTo = searchFilters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault().valore;

            if (creationDateInterval == "0")
            {
                if (!string.IsNullOrEmpty(creationDateFrom))
                {
                    additionalInformation = string.Format("Accessi del giorno {0}", creationDateFrom);
                }
            }
            else if (creationDateInterval == "1")
            {
                if (!string.IsNullOrEmpty(creationDateFrom) && !string.IsNullOrEmpty(creationDateTo))
                {
                    additionalInformation = string.Format("Accessi nel periodo dal {0} al {1}", creationDateFrom, creationDateTo);
                }
                else if (!string.IsNullOrEmpty(creationDateFrom) && string.IsNullOrEmpty(creationDateTo))
                {
                    additionalInformation = string.Format("Accessi successivi al giorno {0}", creationDateFrom);
                }
                else if (!string.IsNullOrEmpty(creationDateTo) && string.IsNullOrEmpty(creationDateFrom))
                {
                    additionalInformation = string.Format("Accessi precedenti al giorno {0}", creationDateTo);
                }
            }

            return additionalInformation;
        }

        private DateTime GetEoD(DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                23,
                59,
                59,
                999
            );
        }

        private DateTime GetSoD(DateTime date)
        {
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                0,
                0,
                0,
                0
            );
        }
        private DataRow ToDataRow(RegMetaData from, DataTable dt)
        {

            DataRow dr = dt.NewRow();

            foreach (PropertyInfo property in from.GetType().GetProperties())
            {
                if (property.GetValue(from) != null)
                {
                    dr[property.Name] = property.GetValue(from);
                }
            }

            return dr;
        }

        private class RegMetaData
        {
            public long? ID_PROJECT { get; set; }
            public long? POSIZIONE { get; set; }
            public string? CODICE { get; set; }
            public string? DESCRIZIONE { get; set; }
            public string? UFFICIO { get; set; }
            public string? STRUTTURA { get; set; }
            public DateTime? DATA_CREAZIONE { get; set; }
            public long? ANNO { get; set; }
            public string? STATO_FASC { get; set; }
            public string? TIPOLOGIA { get; set; }
            public string? NOME_CAMPO { get; set; }
            public string? VALORE_CAMPO { get; set; }
        }
        #endregion
        #region Data querying section
        private async Task<DataSet> ExtractDataRegistriAccessi(List<FiltroRicerca> filters)
        {
            string idAmmFilter = filters.Where(f => f.argomento == "id_amm").FirstOrDefault().valore;

            if (!string.IsNullOrEmpty(idAmmFilter))
            {
                string folderStatus = filters.Where(f => f.argomento == "stato").FirstOrDefault().valore;

                long sysIdTipoOggSep = await this._dbContext.TipoOggettoFascEntities.AsNoTracking()
                    .Where(t => t.TIPO != null && t.TIPO.ToUpper().Equals("SEPARATORE"))
                    .Select(t => t.SYSTEM_ID).FirstOrDefaultAsync();
                string? tipologia = filters.Where(f => f.argomento == "tipologia").FirstOrDefault().valore;

                var q1 =  _dbContext.ProjectEntities.AsNoTracking()
                    .Join(_dbContext.TipoFascEntities.AsNoTracking(),
                        a => a.ID_TIPO_FASC,
                        b => b.SYSTEM_ID,
                        (a, b) => new { a, b })
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                        j => j.a.ID_UO_CREATORE,
                        c => c.SYSTEM_ID,
                        (j, c) => new { j.a, j.b, c })
                    .Join(_dbContext.AssTemplatesFascEntities.AsNoTracking(),
                        j => j.a.ID_TIPO_FASC,
                        d => d.ID_TEMPLATE,
                        (j, d) => new { j.a, j.b, j.c, d })
                    .Join(_dbContext.OggettiCustomFascEntities.AsNoTracking(),
                        j => j.d.ID_OGGETTO,
                        e => e.SYSTEM_ID,
                        (j, e) => new { j.a, j.b, j.c, j.d, e })
                    .Join(_dbContext.OggettiCustomCompFascEntities.AsNoTracking(),
                        j => j.e.SYSTEM_ID,
                        f => f.ID_OGG_CUSTOM,
                        (j, f) => new { j.a, j.b, j.c, j.d, j.e, f })
                    .Join(_dbContext.CorrGlobaliEntities.AsNoTracking(),
                        j => j.a.ID_RUOLO_CREATORE,
                        l => l.SYSTEM_ID,
                        (j, l) => new { j.a, j.b, j.c, j.d, j.e, j.f, l })
                    .Where(j => (j.d.ID_PROJECT != null && j.d.ID_PROJECT.Equals(j.a.SYSTEM_ID.ToString())) &&
                            (j.b.ID_AMM == idAmmFilter.AsLong()) &&
                            (j.e.ID_TIPO_OGGETTO != sysIdTipoOggSep) &&
                            ((j.b.VAR_DESC_FASC != null && tipologia != null && j.b.VAR_DESC_FASC.ToUpper().Equals(tipologia.ToUpper())) ||
                                          (j.b.VAR_DESC_FASC == null && tipologia == null)))
                    .Select(j => new RegMetaData()
                    {
                        ID_PROJECT = j.a.SYSTEM_ID,
                        POSIZIONE = j.f.POSIZIONE,
                        CODICE = j.a.VAR_CODICE,
                        DESCRIZIONE = j.a.DESCRIPTION,
                        UFFICIO = string.Concat(string.Concat(j.c.VAR_CODICE, " - "), j.c.VAR_DESC_CORR),
                        STRUTTURA = IPi3DbContextMappedFunctions.GetCodRegCorcat(j.l.SYSTEM_ID),
                        DATA_CREAZIONE = j.a.DTA_CREAZIONE,
                        ANNO = j.a.ANNO_CREAZIONE,
                        STATO_FASC = j.a.CHA_STATO,
                        TIPOLOGIA = j.b.VAR_DESC_FASC,
                        NOME_CAMPO = j.e.DESCRIZIONE,
                        VALORE_CAMPO = IPi3DbContextMappedFunctions.GetValProfObjPrj(j.a.SYSTEM_ID, j.d.ID_OGGETTO.GetValueOrDefault())
                    });

                if (folderStatus == "O")
                {
                    q1 = q1.Where(a => a.STATO_FASC.Equals("A"));
                }
                else if (folderStatus == "C")
                {
                    q1 = q1.Where(a => a.STATO_FASC.Equals("C"));
                }

                string creationDateInterval = filters.Where(f => f.argomento == "data_creazione").FirstOrDefault().valore;
                DateTime? creationDateFrom = String.IsNullOrEmpty(filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore.ToString()) ? null : filters.Where(f => f.argomento == "data_creazione_da").FirstOrDefault().valore.AsDateTime();
                DateTime? creationDateTo = String.IsNullOrEmpty(filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault().valore.ToString()) ? null : filters.Where(f => f.argomento == "data_creazione_a").FirstOrDefault()?.valore.AsDateTime();

                if (creationDateInterval == "0")
                {
                    // Valore singolo
                    if (creationDateFrom.HasValue)
                    {
                        DateTime creationDateFromEoD = this.GetEoD(creationDateFrom.Value);
                        creationDateFrom = this.GetSoD(creationDateFrom.Value);

                        q1 = q1.Where(ent => ent.DATA_CREAZIONE >= creationDateFrom && ent.DATA_CREAZIONE < creationDateFromEoD);
                    }
                }
                if (creationDateInterval == "1")
                {
                    if (creationDateFrom.HasValue)
                    {
                        creationDateFrom = this.GetSoD(creationDateFrom.Value);
                        q1 = q1.Where(ent => ent.DATA_CREAZIONE >= creationDateFrom);
                    }
                    if (creationDateTo.HasValue)
                    {
                        creationDateTo = this.GetEoD(creationDateTo.Value);
                        q1 = q1.Where(ent => ent.DATA_CREAZIONE <= creationDateTo);
                    }
                }

                long? placeHolder = null;
                var q2 = _dbContext.TipoFascEntities.AsNoTracking()
                    .Join(_dbContext.AssTemplatesFascEntities.AsNoTracking(),
                        b => b.SYSTEM_ID,
                        d => d.ID_TEMPLATE,
                        (b, d) => new { b, d })
                    .Join(_dbContext.OggettiCustomFascEntities.AsNoTracking(),
                        j => j.d.ID_OGGETTO,
                        e => e.SYSTEM_ID,
                        (j, e) => new { j.b, j.d, e })
                    .Join(_dbContext.OggettiCustomCompFascEntities.AsNoTracking(),
                        j => j.e.SYSTEM_ID,
                        f => f.ID_OGG_CUSTOM,
                        (j, f) => new { j.b, j.d, j.e, f })
                    .Where(j => (j.d.ID_PROJECT == null) &&
                          (j.b.ID_AMM == idAmmFilter.AsLong()) &&
                          (j.e.ID_TIPO_OGGETTO != sysIdTipoOggSep) &&
                          ((j.b.VAR_DESC_FASC != null && tipologia != null && j.b.VAR_DESC_FASC.ToUpper().Equals(tipologia.ToUpper())) ||
                          (j.b.VAR_DESC_FASC == null && tipologia == null)))
                    .Select(j => new RegMetaData()
                    {
                        ID_PROJECT = placeHolder,
                        POSIZIONE = j.f.POSIZIONE,
                        CODICE = string.Empty,
                        DESCRIZIONE = string.Empty,
                        UFFICIO = string.Empty,
                        STRUTTURA = string.Empty,
                        DATA_CREAZIONE = null,
                        ANNO = placeHolder,
                        STATO_FASC = string.Empty,
                        TIPOLOGIA = j.b.VAR_DESC_FASC,
                        NOME_CAMPO = j.e.DESCRIZIONE,
                        VALORE_CAMPO = string.Empty
                    });

                DataTable dt = new DataTable();
                foreach (PropertyInfo property in typeof(RegMetaData).GetProperties())
                {
                    DataColumn column = new DataColumn();

                    column.ColumnName = property.Name;
                    Type type = property.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        column.DataType = Nullable.GetUnderlyingType(type);
                    }
                    else
                    {
                        column.DataType = property.PropertyType;
                    }

                    dt.Columns.Add(column);

                }


                var dataQ1 = (await q1.ToListAsync())
                    .OrderBy(row => row.ID_PROJECT)
                    .ThenBy(row => row.POSIZIONE);
                
                var dataQ2 = (await q2.ToListAsync())
                    .OrderBy(row => row.ID_PROJECT)
                    .ThenBy(row => row.POSIZIONE);

                foreach (var row in dataQ2)
                {
                    dt.Rows.Add(this.ToDataRow(row, dt));
                }

                foreach (var row in dataQ1)
                {
                    dt.Rows.Add(this.ToDataRow(row, dt));
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                return ds;

            }
            return null;

        }
        #endregion
        #region public Members

        public RegistroAccessiPubblicazioneHandler(
            IMediator mediator,
            ILogger<RegistroAccessiPubblicazioneHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._mediator = mediator;
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<RegistroAccessiPubblicazioneResult> Handle(RegistroAccessiPubblicazioneRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.RegistroAccessi.RegistroAccessiReportResponse output = new();
            try
            {
                output = await this.PubblicaReg(request.request);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output.success = false;
                output.document = null;
            }
            return new(output);
        }

        #endregion
    }
}
