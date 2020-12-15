using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DocsPaVO.Modelli;
using log4net;

namespace BusinessLogic.Modelli
{
    /// <summary>
    /// Classe per l'elaborazione dei modelli documento
    /// </summary>
    public class DocModelProcessor : BaseDocModelProcessor 
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocModelProcessor));
        /// <summary>
        /// Elaborazione dei modelli documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ModelResponse ProcessModel(ModelRequest request)
        {
            DocsPaVO.Modelli.ModelResponse modelResponse = new DocsPaVO.Modelli.ModelResponse();

            try
            {
                modelResponse.DocumentId = request.DocumentId;

                // Reperimento del motore word processor installato sul client per l'elaborazione del modello
                modelResponse.ProcessorInfo = ModelliManager.GetCurrentModelProcessor(request.UserInfo);

                if (modelResponse.ProcessorInfo == null)
                    throw new ApplicationException("In amministrazione non risulta impostato alcun software per generare il documento");

                // Reperimento scheda documento
                DocsPaVO.documento.SchedaDocumento document = GetDocument(request.UserInfo, request.DocumentId);

                string modelPath = string.Empty;

                if (document.documentoPrincipale != null && request.ModelType.StartsWith(BaseDocModelProcessor.MODEL_ATTATCHMENT))
                {
                    request.ModelType = request.ModelType.Substring(1, 1);

                    // Caricamento documento allegato
                    DocsPaVO.documento.SchedaDocumento attatchment = BusinessLogic.Documenti.DocManager.getDettaglio(request.UserInfo, document.documentoPrincipale.idProfile, document.documentoPrincipale.docNumber);

                    // Reperimento path del modello allegato
                    modelPath = this.GetAttatchmentModelPath(request.ModelType, attatchment.docNumber);
                    
                    // Caricamento del modello dell'allegato
                    modelResponse.DocumentModel = this.LoadModel(request, modelPath, attatchment);
                }
                else
                {
                    // Reperimento path del modello
                    modelPath = this.GetModelPath(request.ModelType, document.docNumber);

                    // Se è richiesto il modello per la versione 2 ma non è disponibile,
                    // viene forzato l'utilizzo del modello 1
                    if (request.ModelType == BaseDocModelProcessor.MODEL_VERSION_2 && 
                        string.IsNullOrEmpty(modelPath))
                    {
                        request.ModelType = BaseDocModelProcessor.MODEL_VERSION_1;

                        modelPath = this.GetModelPath(request.ModelType, document.docNumber);
                    }

                    // Caricamento del modello del documento
                    modelResponse.DocumentModel = this.LoadModel(request, modelPath, document);

                    if (request.ModelType == BaseDocModelProcessor.MODEL_VERSION_2)
                    {
                        // Recupero del contenuto della versione corrente del documento
                        DocsPaVO.documento.FileRequest previousVersion = (DocsPaVO.documento.FileRequest)document.documenti[0];

                        DocsPaVO.documento.FileDocumento previousVersionFile = BusinessLogic.Documenti.FileManager.getFile(previousVersion, request.UserInfo);

                        if (previousVersionFile != null)
                        {
                            // Il contenuto del file della versione corrente del documento
                            // viene inserito nella proprietà contentreplacement
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
            }
            catch (Exception ex)
            {
                modelResponse = new DocsPaVO.Modelli.ModelResponse();
                modelResponse.Exception = ex.Message;

                logger.Debug(string.Format("Errore nel reperimento dei dati del modello: {0}", ex.Message));
            }

            return modelResponse;
        }

        /// <summary>
        /// Reperimento della scheda documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override DocsPaVO.documento.SchedaDocumento GetDocument(DocsPaVO.utente.InfoUtente infoUtente, string id)
        {
            // Reperimento scheda documento
            DocsPaVO.documento.SchedaDocumento document = base.GetDocument(infoUtente, id);

            int accessRights;
            Int32.TryParse(document.accessRights, out accessRights);
            if (accessRights <= 45)
                throw new ApplicationException("Il documento è in sola lettura, impossibile elaborare il modello");

            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="pathModello"></param>
        protected override DocsPaVO.Modelli.ModelKeyValuePair[] GetModelKeyValuePairs(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            ArrayList listaOggetti = GetOggettiProfilazione(schedaDocumento.docNumber, infoUtente.idAmministrazione, schedaDocumento.tipologiaAtto.descrizione);
            
            this.FetchCommonFields(listaOggetti, infoUtente, schedaDocumento);

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

        /// <summary>
        /// Imposta qual'è il modello di riferimento
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="docNumber"></param>
        private string GetModelPath(string modelType, string docNumber)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PATH_MODELLO");

            queryDef.setParam("numeroModello", modelType);
            queryDef.setParam("docNumber", docNumber);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            string field;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                dbProvider.ExecuteScalar(out field, commandText);
            }

            return field;
        }

        /// <summary>
        /// Imposta qual'è il modello di riferimento
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="docNumber"></param>
        private string GetAttatchmentModelPath(string modelType, string docNumber)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PATH_MODELLO_ALLEGATO");

            queryDef.setParam("numeroModello", modelType);
            queryDef.setParam("docNumber", docNumber);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            
            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string field;
                dbProvider.ExecuteScalar(out field, commandText);
                return field;
            }
        }

        /// <summary>
        /// Caricamento del modello del documento
        /// </summary>
        /// <param name="request"></param>
        /// <param name="modelPath"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private Model LoadModel(ModelRequest request, string modelPath, DocsPaVO.documento.SchedaDocumento document)
        {
            if (File.Exists(modelPath))
            {
                Model model = new Model();

                // Caricamento dei tag con i corrispondenti valori
                model.KeyValuePairs = GetModelKeyValuePairs(request.UserInfo, document);

                model.ModelType = request.ModelType;
                model.File.FileName = Path.GetFileName(modelPath);
                model.File.Content = File.ReadAllBytes(modelPath);
                return model;
            }
            else
                throw new ApplicationException("File modello inesistente");
        }

        /// <summary>
        /// reperisce gli oggetti della profilazione dinamica
        /// </summary>
        /// <param name="recordID"></param>
        /// <returns></returns>
        private ArrayList GetOggettiProfilazione(string docNumber, string idAmministrazione, string tipoAtto)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(docNumber);
            ArrayList listaChiaviValori = new ArrayList();

            if (template != null && template.ELENCO_OGGETTI != null)
            {
                for (int i = 0; i < template.ELENCO_OGGETTI.Count; i++)
                {
                    DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)template.ELENCO_OGGETTI[i];

                    if (oggettoCustom != null)
                    {
                        string[] itemToAdd = null;

                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Corrispondente":
                                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(oggettoCustom.VALORE_DATABASE);
                                
                                
                                itemToAdd = new string[8] { "", "", "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                if (corr != null)
                                {
                                    itemToAdd[1] = corr.descrizione;
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = BusinessLogic.Utenti.UserManager.getDettagliIndirizzoCorrispondente(oggettoCustom.VALORE_DATABASE);
                                    if (corrIndirizzo != null)
                                    {
                                        //
                                        oggettoCustom.INDIRIZZO += corr.descrizione + Environment.NewLine + corrIndirizzo.indirizzo + Environment.NewLine +
                                            corrIndirizzo.cap + '-' + corrIndirizzo.citta + '-' + corrIndirizzo.localita;
                                        itemToAdd[3] = oggettoCustom.INDIRIZZO;
                                        oggettoCustom.TELEFONO += corr.descrizione + Environment.NewLine + corrIndirizzo.telefono1 + '-' + corrIndirizzo.telefono2;
                                        itemToAdd[6] = oggettoCustom.TELEFONO;
                                        oggettoCustom.INDIRIZZO_TELEFONO += oggettoCustom.INDIRIZZO + Environment.NewLine + corrIndirizzo.telefono1 +
                                           '-' + corrIndirizzo.telefono2;
                                        itemToAdd[7] = oggettoCustom.INDIRIZZO_TELEFONO;

                                    }

                                }
                                
                                
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                itemToAdd[5] = oggettoCustom.ID_AOO_RF;
                                listaChiaviValori.Add(itemToAdd);

                                break;
                            case "Contatore":
                                string formato = oggettoCustom.FORMATO_CONTATORE;
                                formato = formato.Replace("ANNO", oggettoCustom.ANNO);

                                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (reg != null && !string.IsNullOrEmpty(reg.codRegistro))
                                    formato = formato.Replace("AOO", reg.codRegistro);

                                DocsPaVO.utente.Registro rf = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                if (rf != null && !string.IsNullOrEmpty(rf.codRegistro))
                                    formato = formato.Replace("RF", rf.codRegistro);

                                formato = formato.Replace("CONTATORE", oggettoCustom.VALORE_DATABASE);
                               
                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = formato;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                itemToAdd[3] = oggettoCustom.FORMATO_CONTATORE;
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] = 
                                listaChiaviValori.Add(itemToAdd);

                                break;
                            case "CasellaDiSelezione":
                                string valore = string.Empty;
                                foreach (string val in oggettoCustom.VALORI_SELEZIONATI)
                                {
                                    if (val != null && val != "")
                                        valore += val + "-";
                                }
                                if (valore.Length > 1)
                                    valore = valore.Substring(0, valore.Length - 1);

                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = valore;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] =
                                listaChiaviValori.Add(itemToAdd);
                                break;
                            default:
                                itemToAdd = new string[6] { "", "", "", "", "", "" };
                                itemToAdd[0] = oggettoCustom.DESCRIZIONE;
                                itemToAdd[1] = oggettoCustom.VALORE_DATABASE;
                                itemToAdd[2] = oggettoCustom.TIPO.DESCRIZIONE_TIPO;
                                //itemToAdd[3] = 
                                itemToAdd[4] = oggettoCustom.ANNO;
                                //itemToAdd[5] =
                                listaChiaviValori.Add(itemToAdd);
                                break;
                        }
                    }
                }
            }

            return listaChiaviValori;
        }
    }
}
