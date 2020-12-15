using System;
using System.Data;
using BusinessLogic.Reporting.Behaviors.DataExtraction;
using BusinessLogic.Reporting.Behaviors.ReportGenerator;
using DocsPaVO.Report;
using BusinessLogic.Reporting.Exceptions;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace BusinessLogic.Reporting
{
    /// <summary>
    /// Questa classe fornisce la logica per la generazione di report.
    /// Per creare un nuovo report estendere questa classe astratta.
    /// Richiamare GenerateReport per generare un report che sfrutti, come
    /// metodo di estrazione dati i querylist.
    /// </summary>
    public abstract class ReportGeneratorCommand
    {
        /// <summary>
        /// Metodo per il reperimento dei report registrati nel sistema.
        /// Per eregistrare una classe come report, decorare la classe con l'attributo
        /// ReportGeneratorAttribute
        /// </summary>
        /// <param name="contextName">Nome del contesto da cui viene invocata la ricerca di report registrati</param>
        /// <returns>Response con l'anagrafica dei report registrata</returns>
        public static PrintReportResponse GetRegisteredReport(String contextName)
        {
            // Oggetto da restituire
            PrintReportResponse response = new PrintReportResponse();
            response.ReportMetadata = new List<ReportMetadata>();

            // Recupero del tipo di questa classe
            Type myType = typeof(ReportGeneratorCommand);

            // Reperimento dei tipi decorati con l'attributo ReportGeneratorAttribute
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) &&
                                    t.GetCustomAttributes(typeof(ReportGeneratorAttribute), true).Length > 0).ToArray();
            
            foreach (Type t in types)
            {
                // Generazione della lista di metadati per gli attributi inerenti 
                // il contesto passato per parametro
                IEnumerable<ReportMetadata> tmp = new List<ReportMetadata>(
                    from a in t.GetCustomAttributes(typeof(ReportGeneratorAttribute), true) as ReportGeneratorAttribute[]
                    where a.ContextName == contextName
                    select new ReportMetadata()
                        {
                            ReportName = a.Name,
                            ReportKey = a.Key,
                            ExportableFields  = ((ReportGeneratorCommand)Activator.CreateInstance(t)).GetExportableFieldsCollection()
                        });

                if (tmp != null)
                    response.ReportMetadata.AddRange(tmp);

            }

            return response;

        }

        /// <summary>
        /// Collezione delle colonne esportabili, nel comportamento standard, se è null, vengono esportate tutte le colonne
        /// </summary>
        protected abstract HeaderColumnCollection GetExportableFieldsCollection();

        /// <summary>
        /// Metodo per il reperimento di un report a partire dalla request
        /// </summary>
        /// <param name="rquest">Request con le informazioni sul report da generare</param>
        /// <returns>Response generata dal command di reporting</returns>
        public static PrintReportResponse GetReport(PrintReportRequest request)
        { 
            PrintReportResponse response = null;

            // Recupero di tutti i tipi che hanno almeno un attributo ReportGeneratorAttribute
            Type myType = typeof(ReportGeneratorCommand);
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) &&
                                    t.GetCustomAttributes(typeof(ReportGeneratorAttribute), true).Length > 0).ToArray();

            // Individuazione di un tipo (se esiste), che abbia l'attributo ReportGeneratorAttribute con l'attributo Key uguale a quello
            // contenuto nella request
            foreach (Type t in types)
            {
                // Individuazione della prima classe che ha l'attributo ReportGeneratorAttribute valorizzato con key report
                // uguale a quello contenuto nella request e context name anch'esso pari a quello della request
                ReportGeneratorAttribute reportType = ((ReportGeneratorAttribute[])t.GetCustomAttributes(typeof(ReportGeneratorAttribute), true)).Where(
                    e => e.Key == request.ReportKey && e.ContextName == request.ContextName).FirstOrDefault();

                if (reportType != null)
                {
                    // Se il tipo è stato individuato, viene istanziata la classe e si genera il report
                    response = ((ReportGeneratorCommand)Activator.CreateInstance(t)).GenerateReport(request);
                    break;
                }

            }

            // Restituzione della response
            return response;
            
        }

        /// <summary>
        /// Metodo da richiamare per la generazione di un report. Il comportamento strandard prevede
        /// l'utilizzo di un behavior per l'estrazione dati basato su query list. Sovrascrivere questo
        /// metodo per ottenere un comportamento differente.
        /// </summary>
        /// <param name="request">Informazioni utili alla generazione del report</param>
        /// <returns>Outup del processo di generazione</returns>
        protected virtual PrintReportResponse GenerateReport(PrintReportRequest request)
        {
            IReportDataExtractionBehavior reportDataExtractor = new QueryListReportDataExtractionBehavior();
            IReportGeneratorBehavior formatGenerator = this.CreateReportGenerator(request);

            // Generazione del report
            return this.GenerateReport(request, reportDataExtractor, formatGenerator);
        }

        /// <summary>
        /// Metodo per l'instanziazione del report generator relativo al formato di esportazione
        /// scelto.
        /// </summary>
        /// <param name="request">Informazioni utilili alla generazione del report</param>
        /// <returns>Instanza dell'oggetto responsabile della generazione fisica del report</returns>
        /// <exception cref="ReportGeneratorNotFoundException"></exception>
        protected virtual IReportGeneratorBehavior CreateReportGenerator(PrintReportRequest request)
        {
            switch (request.ReportType)
            {
                case ReportTypeEnum.PDF:
                    return new PdfReportGeneratorBehavior();
                case ReportTypeEnum.Excel:
                    return new ExcelReportGeneratorBehavior();
                case ReportTypeEnum.ODS:
                    return new OdsReportGeneratorBehavior();
                default:
                    throw new ReportGeneratorNotFoundException();
            }
            
        }

        /// <summary>
        /// Metodo per la generazione del report
        /// </summary>
        /// <param name="request">Informazioni utili alla creazione del report</param>
        /// <param name="dataExtractor">Oggetto responsabile dell'estrazione dati</param>
        /// <param name="reportGeneration">Oggetto responsabile della generazione del report fisico</param>
        /// <returns>Risultato del processo di creazione report</returns>
        /// <exception cref="ReportGenerationFailedException" />
        protected virtual PrintReportResponse GenerateReport(PrintReportRequest request, 
                                                             IReportDataExtractionBehavior dataExtractor,
                                                             IReportGeneratorBehavior reportGeneration)
        {
            PrintReportResponse response = null;

            try
            {
                // Response da restituire
                response = new PrintReportResponse();

                // Recupero dati
                DataSet dataSet = dataExtractor.ExtractData(request);

                // Generazione dati per il report
                List<DocsPaVO.Report.Report> report = this.CreateReport(dataSet, request);

                // Generazione file report
                response.Document = reportGeneration.GenerateReport(request, report);
            }
            catch (Exception ex)
            {
                throw new ReportGenerationFailedException(ex.Message);
            }

            // Restituzione del report
            return response;
        }

        /// <summary>
        /// Metodo per la generazione del report da esportare
        /// </summary>
        /// <param name="dataSet">Dataset da cui estrarre i dati</param>
        /// <param name="request">Informazioni sulla richiesta</param>
        /// <returns>Report con i dati estratti</returns>
        protected virtual List<DocsPaVO.Report.Report> CreateReport(DataSet dataSet, PrintReportRequest request)
        {
            // Report da restiutuire
            DocsPaVO.Report.Report report = new DocsPaVO.Report.Report();

            // Impostazione delle proprietà di base del report
            report.CreationDate = DateTime.Now;
            report.Subtitle = request.SubTitle;
            report.Title = request.Title;
            report.AdditionalInformation = request.AdditionalInformation;
            report.ReportMapRow = new ReportMapRow();
            report.ReportHeader = new HeaderColumnCollection();

            // Generazione delle righe del report
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null)
            {
                // Generazione dell'header del report
                report.ReportHeader = this.GenerateReportHeader(dataSet, request.ColumnsToExport);

                report.ReportMapRow.Rows.AddRange(this.GenerateReportRows(dataSet, report.ReportHeader));
            }

            if (!request.ReportKey.Equals("ElencoDecretiSRC") &&
                !request.ReportKey.Equals("ElencoDecretiRestituitiSRC") &&
                !request.ReportKey.Equals("ElencoDecretiRestituitiConRilievoSRC") &&
                !request.ReportKey.Equals("ElencoDecretiSCCLA") &&
                !request.ReportKey.Equals("ElencoDecretiRestituitiSCCLA") &&
                !request.ReportKey.Equals("ElencoDecretiRestituitiConRilievoSCCLA")
                )
            {
                // Costruzione del summary
                report.Summary = String.Format("Righe estratte: {0}", report.ReportMapRow.Rows.Count);
            }
            // Restituzione del report generato
            return new List<DocsPaVO.Report.Report>() { report };
        }

        /// <summary>
        /// Metodo per la creazione dell'header del report. Per default le colonne vengono create in base al
        /// contenuto della collezione di campi esportabili ma se questa è null o vuota, vengono create
        /// delle colonne con nome pari al nome della colonna del dataset e lunghezza autocalcolata
        /// in base al contenuto. Sovrascrivere questo metodo se si desidera un comporamento differente
        /// </summary>
        /// <param name="dataSet">Dataset da cui estrarre i nomi delle colonne</param>
        /// <returns>Lista delle proprietà dell'intestazione</returns>
        //protected virtual List<HeaderProperty> GenerateReportHeader(ExportableFieldsCollection exportableFields)
        protected virtual HeaderColumnCollection GenerateReportHeader(DataSet dataSet, HeaderColumnCollection fieldsToExport)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            // Se la lista dei campi esportabili è nulla o se è vuota, ci si basa sul dataset
            if (fieldsToExport == null || fieldsToExport.Count == 0)
                header = this.GenerateReportHeaderFromDataSet(dataSet);
            else
                header = this.GenerateReportHeaderFromColumnCollection(fieldsToExport);

            return header;
        }

        /// <summary>
        /// Metodo per la costruzione dell'header a partire da un dataset.
        /// </summary>
        /// <param name="dataSet">Dataset da utilizzare per la generazione dell'header</param>
        /// <returns>Lista delle proprietà dell'header</returns>
        protected HeaderColumnCollection GenerateReportHeaderFromDataSet(DataSet dataSet)
        {
            HeaderColumnCollection header = new HeaderColumnCollection();

            foreach (DataColumn row in dataSet.Tables[0].Columns)
            {
                header.Add(new HeaderProperty()
                {
                    ColumnName = row.ColumnName,
                    OriginalName = row.ColumnName,
                    ColumnSize = 0,
                    Export = true,
                    DataType = HeaderProperty.ContentDataType.String
                });
            }

            return header;

        }

        /// <summary>
        /// Questo metodo crea l'header basandosi sulle proprietà della collection di colonne esportabili
        /// </summary>
        /// <param name="exportableCollection">Collection dei campi esportabili</param>
        /// <returns>Header del report</returns>
        protected HeaderColumnCollection GenerateReportHeaderFromColumnCollection(HeaderColumnCollection exportableCollection)
        {
            return new HeaderColumnCollection(exportableCollection.Where(e => e.Export == true));
        }

        /// <summary>
        /// Metodo per la generazione delle righe del report. Il comportamento standard prevede la creazione
        /// di una riga del report per ogni riga del dataset. Un valore viene aggiunto solo se lo si deve esportare.
        /// Sovrascrivere questo metodo se si desidera un comportamento differente.
        /// </summary>
        /// <param name="dataSet">Dataset da utilizzare per la generazione del report</param>
        /// <param name="header">Header del report</param>
        /// <returns>Lista delle righe del report</returns>
        protected virtual IEnumerable<ReportMapRowProperty> GenerateReportRows(DataSet dataSet, HeaderColumnCollection reportHeader)
        {
            List<ReportMapRowProperty> rows = new List<ReportMapRowProperty>();
       
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                // Riga da aggiungere al report
                ReportMapRowProperty reportRow = new ReportMapRowProperty();

                // Creazione della riga
                foreach (DataColumn dataColumn in row.Table.Columns)
                {
                                       
                    // Se bisogna esportare il campo, viene aggiunta una colonna
                    if (reportHeader[dataColumn.ColumnName].Export)
                    {                       
                        reportRow.Columns.Add(new ReportMapColumnProperty()
                        {
                            DataType = reportHeader[dataColumn.ColumnName].DataType,
                            Value = row[dataColumn].ToString(),
                            OriginalName = dataColumn.ColumnName
                        });
                    }
                }
                rows.Add(reportRow);
            }

            return rows;
        }
    }
}
