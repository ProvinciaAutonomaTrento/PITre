using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.IO;
using log4net;

namespace BusinessLogic.Report
{
	/// <summary>
	/// </summary>
	public class RegistriStampa
	{
        private static ILog logger = LogManager.GetLogger(typeof(RegistriStampa));
		/// <summary>
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="objRuolo"></param>
		/// <param name="registro"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.StampaRegistroResult stampaRegistro(
						DocsPaVO.utente.InfoUtente infoUtente, 
						DocsPaVO.utente.Ruolo objRuolo, 
						DocsPaVO.utente.Registro registro)
		{

            DocsPaVO.documento.StampaRegistroResult stampaRegRes = new DocsPaVO.documento.StampaRegistroResult();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                System.Data.DataSet ds;

                string basePathFiles = DocsPaUtils.Functions.Functions.GetReportsPath();
                DocsPaUtils.Functions.Functions.CreateDirectory(basePathFiles);

                try
                {
                    int anno = 0;
                    int protStart = 0;
                    int protEnd = 0;
                    int annoRif = 0;
                    bool okStampa = false;

                    int maxProt = 0;

                    //data ultima stampa
                    string dataUltimaStampa = null;

                    DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
                    report.GetLastDateReg(out ds, registro);

                    System.Data.DataRow regRow = ds.Tables["REG"].Rows[0];

                    if (regRow["CHA_STATO"].ToString().Equals("A"))
                    {
                        //registro aperto: il registro deve essere chiuso
                        logger.Debug("Registro aperto");
                        stampaRegRes.errore = "Attenzione! registro aperto. Per poter stampare, il registro deve essere chiuso.";
                        okStampa = false;
                    }
                    else
                    {
                        // Acquisizione data da registro
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("it-IT");
                        string[] dateFormats = { "dd/MM/yyyy", "dd/MM/yyyy h:mm:ss", "dd/MM/yyyy h.mm.ss", "dd/MM/yyyy HH.mm.ss", "MM/dd/yyyy", "MM/dd/yyyy h:mm:ss", "MM/dd/yyyy h.mm.ss", "MM/dd/yyyy HH.mm.ss" };
                        string regDate = regRow["DTA_CLOSE"].ToString().Trim();
                        DateTime date = DateTime.ParseExact(regDate,
                                                            dateFormats,
                                                            ci.DateTimeFormat,
                                                            System.Globalization.DateTimeStyles.AllowWhiteSpaces);

                        annoRif = date.Year;
                        logger.Debug("annoRif=" + annoRif);

                        string idReg = regRow["SYSTEM_ID"].ToString();

                        report.GetStampReg(ds, regRow);

                        if (ds.Tables["STAMPAREG"].Rows.Count == 0)
                        {
                            //Se non trovo neanche una stampa inizio la stampa registro di protocollo dal primo anno di protocollazione da 1 all'ultimo protocollo per quell'anno (tabella Registri) 
                            dataUltimaStampa = null;
                            logger.Debug("Nessuna stampa trovata");
                            protStart = 1;

                            report.GetProfile(ds, regRow);

                            if (ds.Tables["NUM_ANNO_PROTO"].Rows.Count == 0)
                            {
                                //Non ci sono protocolli nel registro 
                                logger.Debug("Nessun protocollo nel registro");
                                stampaRegRes.errore = "Nessun protocollo nel registro";
                                okStampa = false;
                            }
                            else
                            {
                                logger.Debug("Trovato protocollo nel registro");
                                maxProt = maxNumProto(Int32.Parse(ds.Tables["NUM_ANNO_PROTO"].Rows[0]["NUM_ANNO_PROTO"].ToString()), idReg);
                                anno = Int32.Parse(ds.Tables["NUM_ANNO_PROTO"].Rows[0]["NUM_ANNO_PROTO"].ToString());

                                if (maxProt == -1)
                                {
                                    //Non ci sono protocolli da stampare per quest'anno 
                                    logger.Debug("Nessun protocollo da stampare per quest'anno");
                                    stampaRegRes.errore = "Nessun protocollo da stampare per quest'anno";
                                    okStampa = false;
                                }
                                else
                                {
                                    //MODIFICA FATTA PER MARCHE
                                    //protEnd=maxProt;
                                    protEnd = getSup(maxProt, registro.systemId, protStart);
                                    okStampa = true;
                                }
                            }
                        }
                        else
                        {
                            //Se ne trovo prendo l'ultimo numero di protocollo stampato dal record e l'anno e stampo fino all'ultimo Protocollo per quell'anno (tabella Registri) 
                            logger.Debug("Stampa trovata");
                            int protoUlimaStampa = Int32.Parse(ds.Tables["STAMPAREG"].Rows[0]["NUM_PROTO_END"].ToString());
                            protStart = Int32.Parse(ds.Tables["STAMPAREG"].Rows[0]["NUM_PROTO_END"].ToString()) + 1;
                            anno = Int32.Parse(ds.Tables["STAMPAREG"].Rows[0]["NUM_ANNO"].ToString());
                            char[] space = { ' ' };
                            dataUltimaStampa = ds.Tables["STAMPAREG"].Rows[0]["DTA_STAMPA"].ToString().TrimEnd();
                            logger.Debug(dataUltimaStampa);
                            maxProt = maxNumProto(anno, idReg);

                            if (maxProt == -1)
                            {
                                //Non ci sono protocolli da stampare per quest'anno 
                                logger.Debug("Nessun protocollo per quest'anno");
                                stampaRegRes.errore = "Nessun protocollo per quest'anno";
                                okStampa = false;
                            }
                            else
                            {
                                //MODIFICA FATTA PER MARCHE
                                //protEnd=maxProt;
                                protEnd = getSup(maxProt, registro.systemId, protStart);
                                //if(maxProt < protStart)
                                if (protEnd < protStart)
                                {
                                    if (annoRif > anno)
                                    {
                                        //si esamina l'anno successivo
                                        logger.Debug("Si esamina l'anno successivo");
                                        anno = anno + 1;
                                        maxProt = maxNumProto(anno, idReg);

                                        if (maxProt == -1)
                                        {
                                            //Non ci sono protocolli da stampare per quest'anno 
                                            logger.Debug("Non ci sono protocolli da stampare");
                                            stampaRegRes.errore = "Non ci sono protocolli da stampare";
                                            okStampa = false;
                                        }
                                        else
                                        {
                                            //MODIFICA FATTA PER MARCHE
                                            //protEnd=maxProt;
                                            protStart = 1; // questa riga era commentata... infatti nel passaggio dal 2005 al 2006 s'è skiantato! scommentata da gadamo 02/01/2006... TODO: riscrivere tutto!
                                            protEnd = getSup(maxProt, registro.systemId, protStart);

                                            logger.Debug("protStart: " + protStart);
                                            logger.Debug("protEnd: " + protEnd);
                                            okStampa = checkOkStampa(protStart, protEnd);
                                        }
                                    }
                                    else
                                    {
                                        if (maxProt == protoUlimaStampa)//solo modifiche
                                        {
                                            okStampa = true;
                                        }
                                        else
                                        {
                                            okStampa = false;
                                            stampaRegRes.errore = "Non ci sono protocolli da stampare";
                                        }
                                    }
                                }
                                else
                                {
                                    okStampa = true;
                                }

                            }
                        }
                        if (okStampa)
                        {
                            string pdfFileName = "";
                            bool hasProtocolli;
                            bool hasVariazioni;

                            //genera, protocolla e inserisce il file di stampa
                            GeneraFilePDF(infoUtente,
                                          idReg,
                                          protStart,
                                          protEnd,
                                          anno,
                                          dataUltimaStampa,
                                          out pdfFileName,
                                          out hasProtocolli,
                                          out hasVariazioni);

                            if (pdfFileName != "")
                            {
                                // Fix per bug su numprotostart > numprotoend in caso di stampe contenenti solo variazioni
                                if (hasVariazioni && protStart > protEnd)
                                {
                                    protStart = protEnd;
                                }

                                //si inserisce il file generato nel documentale
                                stampaRegRes.docNumber = insertDocumentale(idReg, anno, protStart, protEnd, infoUtente, objRuolo, pdfFileName, hasProtocolli, hasVariazioni, dataUltimaStampa);

                                //si aggiorna la tabella DPA_STAMPAREGISTRI
                                aggiornaStampaReg(idReg, protStart, protEnd, anno, stampaRegRes.docNumber);

                                //si aggiorna la tabella PROFILE
                                aggiornaProfile(idReg, anno, protStart, protEnd);

                                //invio in conservazione
                                try
                                {
                                    BusinessLogic.Conservazione.ConservazioneManager cons = new Conservazione.ConservazioneManager();
                                    if (cons.GetStatoAttivazione(registro.idAmministrazione))
                                    {
                                        cons.ExecuteVersamentoSingolo(stampaRegRes.docNumber, registro.idAmministrazione, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Debug("Errore non gestito nel versamento della stampa!", ex);
                                }

                                transactionContext.Complete();
                            }
                            else
                            {
                                stampaRegRes.errore = "Non ci sono protocolli da stampare";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    stampaRegRes.errore = "Eccezione: " + e.Message;
                    logger.Debug(e);
                }

                logger.Debug("END : DocsPAWS > report > RegistriStampa > stampaRegistro");
            }

			return stampaRegRes;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="infoUtente"></param>
		/// <param name="objRuolo"></param>
		/// <param name="registro"></param>
		/// <param name="filters"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.StampaRegistroResult StampaRegistroWithFilters(
										DocsPaVO.utente.InfoUtente infoUtente, 
										DocsPaVO.utente.Ruolo objRuolo, 
										DocsPaVO.utente.Registro registro,
										DocsPaVO.filtri.FiltroRicerca[][] filters,
										out DocsPaVO.documento.FileDocumento fileDoc)
		{			
			logger.Debug("StampaRegistroWithFilters");

			DocsPaVO.documento.StampaRegistroResult stampaRegRes=new DocsPaVO.documento.StampaRegistroResult();
						
			string rootPath=AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";            
			string templateFilePath=rootPath + "XMLRepStampaRegistroWithFilters.xml";
			string schemaFilePath=rootPath + "XMLReport.xsd";

			DocsPaDB.Query_DocsPAWS.Report dsoReport=new DocsPaDB.Query_DocsPAWS.Report();				

			DataSet dsStampaRegistro=new DataSet();

			// Recupero documenti da stampare
			DataTable tableProfile = (getDocProtocollatiWithFilters(filters, registro)).Tables[0];
				
			/* Commentato perchè viene utilizzata la query ottimizzata
			 * Vedi chiamata sopra "getDocProtocollati"
			// Lettura dati tabella profilo
			dsoReport.GetProfOgg(dsStampaRegistro,registro.systemId,filters);

			DataTable tableProfile=dsStampaRegistro.Tables["PROFILE"];
			
			// Aggiunta delle colonne aggiuntive necessarie per la stampa
			tableProfile.Columns.AddRange(new DataColumn[]
											{
												new DataColumn("MITT_DEST",typeof(string)),
												new DataColumn("FASCICOLI",typeof(string))											
											}
										);
			if(tableProfile!=null && tableProfile.Rows.Count>0)
			{		
				// NB: IMPLEMENTARE UNA FUNZIONE DI COMPLETAMENTO SPECIFICA PER QUESTA STAMPA (overload)
				foreach (DataRow row in tableProfile.Rows)
					CompleteRowForPrintPDF(infoUtente.idPeople,infoUtente.idGruppo,row,true);
			}
			*/
			
			#region INIZIO CREAZIONE STREAM PDF

			// Apertura stream su file template per la stampa
			FileStream fs=null;
			MemoryStream memoryStream=null;
			FileStream outputStream=null;
			
			try
			{
				// Lettura del file template xml
				fs=new FileStream(templateFilePath,FileMode.Open,FileAccess.Read);

				StampaPDF.Report stampaRegistro=new StampaPDF.Report(fs,schemaFilePath);
					
				// Creazione stampa

				// Sostituzione parametri dinamici della stampa
				Hashtable printParams=new Hashtable();
				printParams.Add("@param0",registro.descrizione);
				printParams.Add("@param1",GetProfOggFilterDescription(filters));

				stampaRegistro.appendParagraph("PAR_REGISTRO",printParams,false);
				stampaRegistro.appendParagraph("PAR_RANGE_PROTOCOLLO",printParams,false);
				stampaRegistro.appendTable("TABLE_PROTOCOLLO",tableProfile,false);
							
				// Chiusura stream file template xml
				fs.Close();
				fs=null;

				// Composizione del nome del file PDF di output
				string outputFileName=infoUtente.userId + "_" + DateTime.Now.ToLongTimeString() + ".pdf";

				// Reperimento contenuto stampa in un memory stream
				memoryStream=stampaRegistro.getStream();

				// Chiusura stream stampa
				stampaRegistro.close();
				stampaRegistro=null;

				Byte[] data=memoryStream.GetBuffer();

				memoryStream.Close();
				memoryStream=null;

				fileDoc = new DocsPaVO.documento.FileDocumento();
				fileDoc.name = outputFileName;
				fileDoc.path = "";
				fileDoc.fullName = '\u005C'.ToString() + fileDoc.name;
				fileDoc.length = (int) data.Length;
				fileDoc.content = data;		
				fileDoc.contentType ="application/pdf";

			}
			catch (Exception e)
			{
				throw e;
			}
			finally
			{
				// Deallocazione risorse utilizzate
				if (fs!=null)
				{
					fs.Close();
					fs=null;
				}

				if (memoryStream!=null)
				{
					memoryStream.Close();
					memoryStream=null;
				}

				if (outputStream!=null)
				{
					outputStream.Flush();
					outputStream.Close();
					outputStream=null;
				}
			}

			#endregion FINE CREAZIONE FILE PDF

			return stampaRegRes;
		}

		/// <summary>
		/// Composizione della descrizione del filtro dinamico fornito in ingresso
		/// </summary>
		/// <param name="filterList"></param>
		/// <returns></returns>
		private static string GetProfOggFilterDescription(DocsPaVO.filtri.FiltroRicerca[][] filterList)
		{
			System.Collections.Hashtable filters=new System.Collections.Hashtable();
			
			for (int i=0;i<filterList.Length;i++) 
			{
				for (int j=0;j<filterList[i].Length;j++)
				{
					DocsPaVO.filtri.FiltroRicerca filterItem=filterList[i][j];					
					if (filterItem.valore != null && !filterItem.valore.Equals("")) 
						filters.Add(filterItem.argomento,filterItem.valore);
					filterItem=null;
				}
			}

			// Composizione descrizione del filtro
			string filterDescription="";

			// Filtro range per numero protocollo
			if (filters.ContainsKey("NUM_PROTOCOLLO_DAL") && filters.ContainsKey("NUM_PROTOCOLLO_AL"))
				filterDescription += " - Protocollo dal num. " + (string) filters["NUM_PROTOCOLLO_DAL"] + " al num. " +  (string) filters["NUM_PROTOCOLLO_AL"];

			// Filtro per numero protocollo singolo
			if (filters.ContainsKey("NUM_PROTOCOLLO_DAL") && !filters.ContainsKey("NUM_PROTOCOLLO_AL"))
			{
				if (filterDescription!="")
					filterDescription += Environment.NewLine;
				filterDescription += " - Protocollo num. " + (string) filters["NUM_PROTOCOLLO_DAL"];
			}

			// Filtro range per data protocollo
			if (filters.ContainsKey("DATA_PROT_SUCCESSIVA_AL") && filters.ContainsKey("DATA_PROT_PRECEDENTE_IL"))
			{
				if (filterDescription!="")
					filterDescription += Environment.NewLine;
				filterDescription += " - Protocollo dal " +  (string) filters["DATA_PROT_SUCCESSIVA_AL"] + " al " + (string) filters["DATA_PROT_PRECEDENTE_IL"];
			}
			
			// Filtro per data protocollo singola
			if (filters.ContainsKey("DATA_PROT_SUCCESSIVA_AL") && !filters.ContainsKey("DATA_PROT_PRECEDENTE_IL"))
			{
				if (filterDescription!="")
					filterDescription += Environment.NewLine;					
				filterDescription += " - Protocollo del " +  (string) filters["DATA_PROT_SUCCESSIVA_AL"];
			}
				
			// Filtro per anno protocollo
			if (filters.ContainsKey("ANNO_PROTOCOLLO"))
			{
				if (filterDescription!="")
					filterDescription += Environment.NewLine;
				filterDescription += " - Anno di protocollazione: " + (string) filters["ANNO_PROTOCOLLO"];
			}

			filters=null;

			if (filterDescription=="")
				filterDescription="Nessun filtro impostato";
			else
				filterDescription="Filtro impostato:" + Environment.NewLine + filterDescription;

			return filterDescription;
		}



		/// <summary>
		/// Creazione stampa registro protocollazione in formato PDF
		/// 
		/// 1) Impostazione percorsi
		/// 2) Accesso ai dati per lettura descrizione registro
		/// 3) 
		/// 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="idGruppo"></param>
		/// <param name="idReg"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="anno"></param>
		/// <param name="dataUltimaStampa"></param>
		/// <param name="pdfFileName"></param>
		private static void GeneraFilePDF(DocsPaVO.utente.InfoUtente infoUtente,
											string idReg,
											int protStart,
											int protEnd,
											int anno,
											string dataUltimaStampa,
											out string pdfFileName,
											out bool hasProtocolli,
											out bool hasVariazioni)
		{			
			logger.Debug("GeneraFilePDF");
			
			string rootPath=AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";            
			string templateFilePath=rootPath + "XMLRepStampaRegistro.xml";
			string schemaFilePath=rootPath + "XMLReport.xsd";
			pdfFileName = "";
			FileStream fs=null;
			
			try
			{
				// Dataset contenente i dati della stampa
				DataSet dsStampaRegistro=null;

				// Lettura descrizione registro
				DocsPaDB.Query_DocsPAWS.Report dsoReport=new DocsPaDB.Query_DocsPAWS.Report();				
				dsoReport.GetCodReg(out dsStampaRegistro,idReg);

				string descrizioneRegistro="";
            string descrizioneAmministrazione = "";
            if (dsStampaRegistro.Tables["REG"].Rows.Count > 0)
            {
               //descrizioneRegistro = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_CODICE"].ToString();
               descrizioneRegistro = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_DESC_REGISTRO"].ToString();
               descrizioneAmministrazione = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_DESC_AMM"].ToString();
            }

				hasVariazioni=false;
				hasProtocolli=false;

				// Recupero documenti da stampare
                //modifica                
                //DataTable tableProfile1 = null;
                //tableProfile1 = (getDocProtocollatiCache(idReg, anno, protStart, protEnd)).Tables[0];
                DataTable tableProfile = (getDocProtocollati(idReg, anno, protStart, protEnd)).Tables[0];
                //if(tableProfile1 != null)    
                //    tableProfile.Merge(tableProfile1);
                ////modifica
				#region INIZIO GESTIONE VARIAZIONI

				DataTable tableVariazioni=null;
				
				if (dataUltimaStampa!=null && dataUltimaStampa!="")
				{
					logger.Debug("Parte variazioni registri: "+ dataUltimaStampa);	

					dsoReport.GetProfOgg2(out dsStampaRegistro,anno,protStart,idReg,dataUltimaStampa);

					tableVariazioni=dsStampaRegistro.Tables["PROTO_VARIATI"];

					if(tableVariazioni!=null && tableVariazioni.Rows.Count>0)
					{
						hasVariazioni=true;

						// Aggiunta delle colonne aggiuntive necessarie per la stampa
						AppendColumnsForPrintPDF(tableVariazioni);

						// Sono presenti individuati documenti protocollati già stampati
						// ma modificati in una fase successiva: verranno accodati alla stampa
						foreach (DataRow row in tableVariazioni.Rows)
							CompleteRowForPrintPDF(infoUtente, row, false);
					}
					
				}
				
				#endregion FINE GESTIONE VARIAZIONI

				#region INIZIO CREAZIONE FILE PDF


				// Apertura stream su file template per la stampa
				int start = System.Environment.TickCount;
				fs=new FileStream(templateFilePath,FileMode.Open,FileAccess.Read);

				StampaPDF.Report stampaRegistro=new StampaPDF.Report(fs,schemaFilePath);
				
				// Creazione stampa

				// Sostituzione parametri dinamici della stampa:
				// - DaNumeroProtocollo
				// - ANumeroProtocollo
				// - DataOraStampa
				Hashtable printParams=new Hashtable();
				if(tableProfile!=null && tableProfile.Rows.Count>0)
				{
					hasProtocolli=true;
               printParams.Add("@paramAmm",descrizioneAmministrazione);
					printParams.Add("@param0",descrizioneRegistro);
					printParams.Add("@param1",protStart);
					printParams.Add("@param2",protEnd);
               
               stampaRegistro.appendParagraph("PAR_AMMINISTRA", printParams, false);
					stampaRegistro.appendParagraph("PAR_REGISTRO",printParams,false);
					stampaRegistro.appendParagraph("PAR_RANGE_PROTOCOLLO",printParams,false);

                    //MODIFICATO DA FABIO PER ETICHETTE PROTOCOLLI
                    DataTable tableProfileMod = tableProfile;
                    int i = 0;
                    foreach (DataRow rowMod in tableProfileMod.Rows)
                    {
                        rowMod.Table.Rows[i]["CHA_TIPO_PROTO"] = getLettereProtocolli(infoUtente, rowMod.Table.Rows[i]["CHA_TIPO_PROTO"].ToString());
                        //modifica
                        if(!string.IsNullOrEmpty(rowMod.Table.Rows[i]["IMPRONTA_CACHE"].ToString()))
                        {
                            string impronta = rowMod.Table.Rows[i]["IMPRONTA_CACHE"].ToString();
                            rowMod.Table.Rows[i]["IMPRONTA"] = impronta;
                        }
                        //fine modifica
                        i++;
                    }
                    stampaRegistro.appendTable("TABLE_PROTOCOLLO", tableProfileMod, false);
				}

				if (hasVariazioni)
				{
					printParams.Clear();
					printParams.Add("@param0",dataUltimaStampa);

					// Se sono presenti delle variazioni di documenti, 
					// vengono accodati alla stampa a partire da una nuova pagina
					stampaRegistro.appendParagraph("PAR_RANGE_VARIAZIONI",printParams,true);					
					stampaRegistro.appendTable("TABLE_VARIAZIONI",tableVariazioni,false);
				}
					
	
				// Chiusura stream file template
				fs.Close();

				// Composizione del nome del file PDF di output 
				// "CodRegistro_ProtoInit_ProtoFin.PDF"
				

				if (hasProtocolli || hasVariazioni)
				{
					string outputFileName=string.Format("{0}_{1}_{2}.{3}",descrizioneRegistro,protStart,protEnd,"PDF");

					MemoryStream memoryStream=stampaRegistro.getStream();

					stampaRegistro.close();

					Stream outputStream=new FileStream(
										DocsPaUtils.Functions.Functions.GetReportsPath() + @"\" + outputFileName,
										FileMode.Create,
										FileAccess.Write);
				
					Byte[] data=memoryStream.GetBuffer();

					outputStream.Write(data,0,data.Length);

					memoryStream.Close();
					memoryStream=null;

					outputStream.Flush();
					outputStream.Close();
					outputStream=null;

					// Impostazione del nome del file pdf come parametro di output
					pdfFileName=outputFileName;

					int end = System.Environment.TickCount;
					System.Diagnostics.Debug.WriteLine("Tempo creazione PDF : "+(end-start));
				}
				#endregion FINE CREAZIONE FILE PDF
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione di Report (generaFile)",e);
				throw e;
			}
			finally
			{
				if (fs!=null)
					fs.Close();
			}

			fs=null;
		}


		/// <summary>
		/// Creazione stampa documenti registro con filtri
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="idGruppo"></param>
		/// <param name="idReg"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="anno"></param>
		/// <param name="dataUltimaStampa"></param>
		private static void GeneraStampaRegistroPDFWithFilters(string idPeople, 
															string idGruppo, 
															string idReg,
															int protStart,
															int protEnd,
															int anno,
															string dataUltimaStampa)
		{
			logger.Debug("GeneraStampaRegistroWithFilters");
			
			string rootPath=AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";            
			string templateFilePath=rootPath + "XMLRepStampaRegistro.xml";
			string schemaFilePath=rootPath + "XMLReport.xsd";



		}

		/// <summary>
		/// </summary>
		/// <param name="utente"></param>
		/// <param name="idReg"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="anno"></param>
		/// <param name="dataUltimaStampa"></param>
		/// <param name="debug"></param>
		private static void generaFile(DocsPaVO.utente.InfoUtente infoUtente, string idReg,int protStart,int protEnd,int anno,string dataUltimaStampa)
		{
			logger.Debug("generaFile");
			//DocsPa_V15_Utils.database.SqlServerAgent db=new DocsPa_V15_Utils.database.SqlServerAgent();
			//System.Data.DataSet ds=new System.Data.DataSet();
			System.Data.DataSet ds;
			//System.IO.StreamWriter sw=null;
			System.IO.FileStream fs=null;
			bool streamOpen=false;
			//bool dbOpen=false;  
			string path = AppDomain.CurrentDomain.BaseDirectory + @"\report\StampaReg";            
			string recordTableMaster = ReportUtils.stringFile(path + "\\RecordTable2.txt");
			string recordTable = ReportUtils.stringFile(path + "\\RecordTable.txt");

			try
			{
				string descReg="";
				string filename="Stampa" + anno + "_" + protStart + "_" + protEnd + ".rtf";

				#region Codice Commentato
	            /*
				string regQueryString="SELECT VAR_CODICE FROM DPA_EL_REGISTRI WHERE SYSTEM_ID="+idReg;
				logger.Debug(regQueryString);
				db.fillTable(regQueryString,ds,"REG");
				*/
				#endregion 

				DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();				
				report.GetCodReg(out ds,idReg);

				if(ds.Tables["REG"].Rows.Count>0)
				{
					descReg=ds.Tables["REG"].Rows[0]["VAR_CODICE"].ToString();
				}
			  
				//CREAZIONE DEL FILE RTF
				//sw=new  System.IO.StreamWriter(path+"\\stampa.rtf",true,System.Text.Encoding.Unicode);
				//fs=new System.IO.FileStream(ReportUtils.getPathName() + "\\stampa.rtf",System.IO.FileMode.OpenOrCreate);
				fs=new System.IO.FileStream(DocsPaUtils.Functions.Functions.GetReportsPath() + "\\stampa.rtf",System.IO.FileMode.OpenOrCreate);
				streamOpen=true;

				//lettura, modifica e scrittura dell'header	  
				logger.Debug("Creazione dell'header");
				string header=ReportUtils.stringFile(path+"\\HeaderFooter.txt");
				header=header.Replace("XREGISTRO", descReg);
				header=header.Replace("XINIZIO", protStart.ToString());
				header=header.Replace("XFINE", protEnd.ToString());

				/*[Modifica]
					* Autore: massimo digregorio
					* Data:30/09/2003
					* Descrizione:x bug sulle data in testa alla sezione nuovi protocolli
					* Richiedente:pino
					* 
				* old:
				*				string data=Convert.ToString(System.DateTime.Now);
				*new:			  
				*/

				string data=System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

				header=header.Replace("XOGGI", data);
				//sw.Write(header);
				fs.Write(ReportUtils.toByteArray(header),0,ReportUtils.toByteArray(header).Length);

				#region Codice Commentato
				/*
				string profileString ="SELECT DISTINCT " +
						   "A.SYSTEM_ID,A.DOCNUMBER, A.CHA_TIPO_PROTO, A.DTA_ANNULLA," +
						   "A.NUM_PROTO, " + DocsPa_V15_Utils.dbControl.toChar("A.DTA_PROTO",false) + " AS DATA, " +
						   "E.VAR_DESC_OGGETTO AS OGGETTO, A.ID_REGISTRO, A.CHA_DA_PROTO " +
						   "FROM " +
						   "PROFILE A, DPA_OGGETTARIO E "+
						   "WHERE " +
						   "A.CHA_TIPO_PROTO IN('A', 'P') " + 
						   "AND E.SYSTEM_ID=A.ID_OGGETTO AND " +
						   "A.NUM_ANNO_PROTO="+anno+" AND A.NUM_PROTO>="+protStart+" AND A.NUM_PROTO<="+protEnd+" AND A.ID_REGISTRO="+idReg;		
				profileString += " ORDER BY A.SYSTEM_ID";
				logger.Debug(profileString);
				db.fillTable(profileString,ds,"PROFILE");
				*/
				#endregion 

				report.GetProfOgg(ds,anno,protStart,protEnd,idReg);

				//recordTable 1 e recordTable 2
				if(ds.Tables["PROFILE"]!=null && ds.Tables["PROFILE"].Rows.Count>0)
				{		
					#region Codice Commentato
					//addRowInTable(utente, ref recordTable,db,ds.Tables["PROFILE"].Rows[0]);
					//si scrive nel file rtf
					//sw.Write(recordTable);
					#endregion 

					fs.Write(ReportUtils.toByteArray(recordTable),0,ReportUtils.toByteArray(recordTable).Length);

					//recordTable 2
					logger.Debug("recordTable2");
				  
					for(int i=0;i<ds.Tables["PROFILE"].Rows.Count;i++)
					{
						string temp=recordTableMaster;
                        addRowInTable(infoUtente, ref temp, ds.Tables["PROFILE"].Rows[i]);
						//sw.Write(temp);
						fs.Write(ReportUtils.toByteArray(temp),0,ReportUtils.toByteArray(temp).Length);
					}
				}

				if(dataUltimaStampa!=null)
				{
					logger.Debug("Parte variazioni registri: "+dataUltimaStampa);	
				
					System.IFormatProvider format = new System.Globalization.CultureInfo("it-IT", true);
					DateTime xDataTime = Convert.ToDateTime(dataUltimaStampa, format);

					#region Codice Commentato
					//PARTE RELATIVA ALLE VARIAZIONI DEI PROTOCOLLI PRECEDENTI
					//si prendono i protocolli che hanno subito variazioni nell'intervallo di tempo indicato
					/*
					string protoVariatiString ="SELECT DISTINCT " +
						"A.SYSTEM_ID,A.DOCNUMBER, A.CHA_TIPO_PROTO, A.CHA_MOD_OGGETTO, A.CHA_MOD_MITT_DEST, A.CHA_MOD_MITT_INT, A.DTA_ANNULLA," +
						"A.NUM_PROTO, " + DocsPa_V15_Utils.dbControl.toChar("A.DTA_PROTO",false) + " AS DATA, " +
						"E.VAR_DESC_OGGETTO AS OGGETTO, A.ID_REGISTRO, A.CHA_DA_PROTO " +
						"FROM " +
						"PROFILE A, DPA_OGGETTARIO E "+
						"WHERE " +
						"A.CHA_TIPO_PROTO IN('A', 'P') " + 
						"AND E.SYSTEM_ID=A.ID_OGGETTO AND " +
						"A.NUM_ANNO_PROTO="+anno+" AND A.NUM_PROTO < "+protStart+" AND A.ID_REGISTRO="+idReg;
	/* massimo digregorio 2004/02/11 x REGMARCHE
	//old:
	//					protoVariatiString += " AND A.CHA_CONGELATO='1' AND (A.CHA_MOD_OGGETTO='1' OR A.CHA_MOD_MITT_DEST='1' OR A.CHA_MOD_MITT_INT='1' OR A.DTA_ANNULLA>"+DocsPa_V15_Utils.dbControl.toDate(DocsPaWS.Utils.DateControl.toDateFromDB(dataUltimaStampa),true)+")";
	New:*/
					/*
					protoVariatiString += " AND A.CHA_CONGELATO='1' ";
					protoVariatiString += " AND ( ";
					protoVariatiString +=         " A.DTA_ANNULLA > "+DocsPa_V15_Utils.dbControl.toDate(DocsPaWS.Utils.DateControl.toDateFromDB(dataUltimaStampa),true);
					protoVariatiString +=         " OR ( ";
					protoVariatiString +=                " A.CHA_MOD_OGGETTO = '1' ";
					protoVariatiString +=                " AND ";
					protoVariatiString +=                " A.SYSTEM_ID IN ";
					protoVariatiString +=                           "( ";
					protoVariatiString +=                           " SELECT [ID_PROFILE] FROM [DPA_OGGETTI_STO] ";
					protoVariatiString +=                           " WHERE ";
					protoVariatiString +=                           " [DTA_MODIFICA] > " +DocsPa_V15_Utils.dbControl.toDate(DocsPaWS.Utils.DateControl.toDateFromDB(dataUltimaStampa),true);
					protoVariatiString +=                           ") ";
					protoVariatiString +=            " ) ";
					protoVariatiString +=         " OR ( ";
					protoVariatiString +=                " (A.CHA_MOD_MITT_DEST='1' OR A.CHA_MOD_MITT_INT='1' ) ";
					protoVariatiString +=                " AND ";
					protoVariatiString +=                " A.SYSTEM_ID IN ";
					protoVariatiString +=                           "( ";
					protoVariatiString +=                           " SELECT [ID_PROFILE] FROM [DPA_CORR_STO] ";
					protoVariatiString +=                           " WHERE ";										
					protoVariatiString +=                           " [DTA_MODIFICA] > " +DocsPa_V15_Utils.dbControl.toDate(DocsPaWS.Utils.DateControl.toDateFromDB(dataUltimaStampa),true);
					protoVariatiString +=                           ") ";
					protoVariatiString +=            " ) ";
					protoVariatiString +=       ")";
	//END NEW
					protoVariatiString += " ORDER BY A.SYSTEM_ID"; 
					logger.Debug(protoVariatiString);
					db.fillTable(protoVariatiString,ds,"PROTO_VARIATI");
					*/
					#endregion

					report.GetProfOgg2(out ds,anno,protStart,idReg,dataUltimaStampa);

					if(ds.Tables["PROTO_VARIATI"]!=null && ds.Tables["PROTO_VARIATI"].Rows.Count>0)
					{
						//aggiunta dell'item per la tabella
						string headerMod=ReportUtils.stringFile(path+"\\headerMod.txt");
						//sostituzione della data nel template
/*[Modifica]
	* Autore: massimo digregorio
	* Data:30/09/2003
	* Descrizione:x bug sulle data in testa alla sezione protoc. modificati
	* Richiedente:pino
	* 
* old:
*						string xdata=xDataTime.Day+"/"+xDataTime.Month+"/"+xDataTime.Year+", ORA "+xDataTime.Hour+":"+xDataTime.Minute;
*new:			  
*/
						string xdata = xDataTime.ToString("dd/MM/yyyy") + ",ORA " + xDataTime.ToString("HH:mm");
						
						headerMod = headerMod.Replace("XDATA", xdata);
						//sw.Write(headerMod);
						fs.Write(ReportUtils.toByteArray(headerMod),0,ReportUtils.toByteArray(headerMod).Length);
						
						//intestazione
						fs.Write(ReportUtils.toByteArray(recordTable),0,ReportUtils.toByteArray(recordTable).Length);

						for(int i=0;i<ds.Tables["PROTO_VARIATI"].Rows.Count;i++)
						{							 
							string temp=recordTableMaster;
                            addRowInTable(infoUtente, ref temp, ds.Tables["PROTO_VARIATI"].Rows[i]);
							//sw.Write(temp);
							fs.Write(ReportUtils.toByteArray(temp),0,ReportUtils.toByteArray(temp).Length);
						}
					}
					//FINE PARTE VARIAZIONI
				}

				//file bottom
				logger.Debug("Creazione del bottom");
				string bottom=ReportUtils.stringFile(path+"\\Bottom.txt");
				//sw.Write(bottom);
				fs.Write(ReportUtils.toByteArray(bottom),0,ReportUtils.toByteArray(bottom).Length);
				  
				//chiusura dello stream
				//sw.Close();
				fs.Close();
				  
				//chiusura connessione db
				//db.closeConnection();
			}
			catch(Exception e)
			{
				if(streamOpen==true)
				{
				  //sw.Close();
				  fs.Close();
				}
				
				/*if(dbOpen==true)
				{
				  db.closeConnection();
				}*/
			   
				logger.Debug("Errore nella gestione di Report (generaFile)",e);
				throw e;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <returns></returns>
		private static bool checkOkStampa(int protStart,int protEnd)
		{
			if (protEnd < protStart)
			{
				return false;
			}
			else
			{
				return true;
			}	
		}

		/// <summary>
		/// </summary>
		/// <param name="anno"></param>
		/// <param name="idReg"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static int maxNumProto(int anno,string idReg)
		{
			logger.Debug("maxNumProto");
			//DocsPa_V15_Utils.database.SqlServerAgent db=new DocsPa_V15_Utils.database.SqlServerAgent();
			//System.Data.DataSet ds=new System.Data.DataSet();
			System.Data.DataSet ds;
			int res=0;
			
			try
			{
				#region Codice Commentato
				/*
				string numProtoQueryString="SELECT MAX(NUM_PROTO) AS NUMMAX FROM PROFILE WHERE ID_REGISTRO = " + idReg + " AND NUM_ANNO_PROTO = " + anno + " AND (CHA_TIPO_PROTO = 'P' OR CHA_TIPO_PROTO = 'A')";
				logger.Debug(numProtoQueryString);
				db.fillTable(numProtoQueryString,ds,"NUM_PROTO");
				*/
				#endregion 

				DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();				
				report.MaxNrProt(out ds,anno,idReg);

				if(ds.Tables["NUM_PROTO"].Rows.Count==0)
				{
					res=-1;
				}
				else
				{
                    res=Int32.Parse(ds.Tables["NUM_PROTO"].Rows[0]["NUMMAX"].ToString());
				}
                
				return res;
			}
			catch(Exception)
			{
			    return -1;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="docnumber"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static string getHash(string docnumber)
		{
		   System.Data.DataSet ds;
			string hash=null;

			try
			{
				logger.Debug("getHash");

				#region Codice Commentato
				/*
				string hashQueryString="SELECT A.VAR_IMPRONTA,B.VERSION FROM COMPONENTS A, VERSIONS B WHERE A.VERSION_ID=B.VERSION_ID";
				hashQueryString=hashQueryString+" AND A.DOCNUMBER="+docnumber+" ORDER BY B.VERSION DESC";
				db.fillTable(hashQueryString,ds,"HASH");
				*/
				#endregion 

				DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();				
				report.GetImpVers(out ds,docnumber);

				hash="";

				if(ds.Tables["HASH"].Rows.Count > 0)
				{
					if(ds.Tables["HASH"].Rows[0]["VAR_IMPRONTA"]!=null && !ds.Tables["HASH"].Rows[0]["VAR_IMPRONTA"].ToString().Equals(""))
					{
						hash=ds.Tables["HASH"].Rows[0]["VAR_IMPRONTA"].ToString();
					}
				}
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione di Report (getHash)",e);
				throw e;
			}

			return hash;
		}

		/// <summary>
		/// </summary>
		/// <param name="idReg"></param>
		/// <param name="anno"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="infoUtente"></param>
		/// <param name="objRuolo"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static string insertDocumentale(string idReg,
									int anno, 
									int protStart, 
									int protEnd,
									DocsPaVO.utente.InfoUtente infoUtente,
									DocsPaVO.utente.Ruolo objRuolo,
									string pdfFileName,bool hasProtocolli,bool hasVariazioni,string dataUltimaStampa)
		{
            bool documentCreated = false;

            //inserimento della scheda nel documentale
            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);

            DocsPaVO.documento.SchedaDocumento schedaDocumento = new DocsPaVO.documento.SchedaDocumento();

			try
			{
            	
                DocsPaVO.utente.Registro reg=new DocsPaVO.utente.Registro();
                reg.systemId=idReg;
                schedaDocumento.registro = reg;
                schedaDocumento.appId = ((DocsPaVO.documento.Applicazione)BusinessLogic.Documenti.FileManager.getApplicazioni("PDF")[0]).application;
                schedaDocumento.idPeople = infoUtente.idPeople;
                schedaDocumento.userId = infoUtente.userId;
                schedaDocumento.typeId = "MAIL";

              

				string oggetto=string.Empty;
				
                if (hasProtocolli)
					oggetto = "Giornale di Protocollo - Anno " + anno + " - dal prot. n° " + protStart + " al prot. n° " + protEnd;
				else 
				{
					if (hasVariazioni)
				        oggetto = "Modifiche apportate ai documenti protocollati prima del " + dataUltimaStampa;
				}

				DocsPaVO.documento.Oggetto ogg=new DocsPaVO.documento.Oggetto();
				ogg.descrizione=oggetto;
                schedaDocumento.oggetto = ogg;

                // Tipologia documento: Stampa registro
                schedaDocumento.tipoProto = "R";

				//inserimento dell'oggetto
                schedaDocumento = BusinessLogic.Documenti.ProtoManager.addOggettoLocked(infoUtente.idAmministrazione, schedaDocumento);

                // Ruoli superiori che hanno la visibilità sul documento 
                DocsPaVO.utente.Ruolo[] ruoliSuperiori;

                documentCreated = (documentManager.CreateDocumentoStampaRegistro(schedaDocumento, objRuolo, out ruoliSuperiori));

                if (documentCreated)
                {
                    // Notifica evento documento creato
                    DocsPaDocumentale.Interfaces.IAclEventListener eventsNotification = new DocsPaDocumentale.Documentale.AclEventListener(infoUtente);
                    eventsNotification.DocumentoCreatoEventHandler(schedaDocumento, objRuolo, ruoliSuperiori);

                    //salvataggio fisico del file
                    DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                    fd.fullName = DocsPaUtils.Functions.Functions.GetReportsPath() + @"\" + pdfFileName;
                    fd.content = File.ReadAllBytes(fd.fullName);
                    fd.length = fd.content.Length;
                    fd.name = pdfFileName;

                    DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest) schedaDocumento.documenti[0];
                    fileRequest.descrizione = DocsPaVO.documento.Documento.STAMPA_REGISTRO;

                    fileRequest = BusinessLogic.Documenti.FileManager.putFile(fileRequest, fd, infoUtente);

                    if (fileRequest == null)
                        throw new ApplicationException("Si è verificato un errore nell'upload del documento per la stampa del registro di protocollo");
                    //modifica
                    if(File.Exists(fd.fullName))
                        File.Delete(fd.fullName);
                    //modifica
                    return schedaDocumento.docNumber;
                }
                else
                    throw new ApplicationException("Errore nella creazione del documento per la stampa del registro di protocollo");
			}
			catch (Exception ex)
			{
                if (documentCreated)
                {
                    // Se si è verificato un errore successivamente la creazione del documento, viene rimosso dal documentale
                    documentManager.Remove(new DocsPaVO.documento.InfoDocumento(schedaDocumento));
                }

                logger.Debug(ex);

                throw ex;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="systemIdDocumento"></param>
		/// <param name="utente"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static System.Collections.ArrayList getClassificazioni(DocsPaVO.utente.InfoUtente infoUtente, string systemIdDocumento)
		{
			bool l_corr=false;
			//DocsPa_V15_Utils.database.SqlServerAgent db = new DocsPa_V15_Utils.database.SqlServerAgent();
			//db.openConnection();
			DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.InfoDocumento l_infoDocumento = doc.GetInfoDocumento(null, null, systemIdDocumento, l_corr);
			//db.closeConnection();	
            return BusinessLogic.Fascicoli.FascicoloManager.getFascicoliDaDoc(infoUtente, l_infoDocumento.idProfile);
		}
		
		/// <summary>
		/// </summary>
		/// <param name="utente"></param>
		/// <param name="temp"></param>
		/// <param name="db"></param>
		/// <param name="profile"></param>
		/// <param name="debug"></param>
		private static void addRowInTable(DocsPaVO.utente.InfoUtente infoUtente, ref string temp, DataRow profile)
		{			
			logger.Debug("addRowInTable");

			if(profile["DTA_ANNULLA"]!=null && !profile["DTA_ANNULLA"].ToString().Equals(""))
			{
				temp = temp.Replace("XSTILE","\\b\\strike\\f2\\cf6");
			}
			else
			{
				temp = temp.Replace("XSTILE", "\\f2");
			}
			
			temp = temp.Replace("XPROTOCOLLO", profile["NUM_PROTO"].ToString());
			temp = temp.Replace("XDATA", profile["DATA"].ToString());
			temp = temp.Replace("XTIPO", profile["CHA_TIPO_PROTO"].ToString());
			temp = temp.Replace("XOGGETTO", profile["OGGETTO"].ToString());
			  
			string mittDest2="";
			logger.Debug("Ricerca dei mittenti");
			System.Collections.ArrayList mittDestArray2=getCorrispondenti(profile["SYSTEM_ID"].ToString());
			
			for(int j=0;j<mittDestArray2.Count;j++)
			{
				mittDest2=mittDest2+(string) mittDestArray2[j]+"  \\par ";
			}
			
			logger.Debug("mittDest="+mittDest2);
			temp = temp.Replace("XMITTDEST",mittDest2);

			//Marco Brizi 09/05/2003 - inizio
			string l_classificazioni="";
			string l_separatore="";
			string l_systemIdDocumento=profile["SYSTEM_ID"].ToString();
			logger.Debug("Ricerca delle classificazioni");
			System.Collections.ArrayList l_classificazioniArray=getClassificazioni(infoUtente, l_systemIdDocumento);
			
			for(int j=0;j<=l_classificazioniArray.Count-1;j++)
			{					
				l_classificazioni=l_classificazioni+l_separatore+((DocsPaVO.fascicolazione.Fascicolo)l_classificazioniArray[j]).codice.ToString();
				l_separatore=", ";
			}
			
			logger.Debug("classificazioni="+l_classificazioni);
			temp = temp.Replace("XCLASSIFICAZIONI",l_classificazioni);
			//Marco Brizi 09/05/2003 - fine
			
			temp = temp.Replace("XHASH",getHash(profile["DOCNUMBER"].ToString()));
		}


		/// <summary>
		/// Aggiunta delle colonne aggiuntive necessarie per la stampa
		/// dei registri in pdf
		/// </summary>
		/// <param name="dataTable"></param>
		private static void AppendColumnsForPrintPDF(DataTable dataTable)
		{
			dataTable.Columns.AddRange(
										new DataColumn[]
										{
											new DataColumn("MITT_DEST",typeof(string)),
											new DataColumn("FASCICOLI",typeof(string)),
											new DataColumn("IMPRONTA",typeof(string))
										}
									  );
		}

		/// <summary>
		/// Inserimento valori aggiuntivi necessari per la stampa registri in PDF
		/// (per singolo protocollo)
		/// </summary>
		/// <param name="idPeople"></param>
		/// <param name="idGruppo"></param>
		/// <param name="temp"></param>
		/// <param name="profile"></param>
		private static void CompleteRowForPrintPDF(DocsPaVO.utente.InfoUtente infoUtente,
													DataRow dataRow,
													bool printWithFilters)
		{			
			logger.Debug("CompleteRowForPrintPDF");
			
			logger.Debug("Ricerca dei mittenti");
			System.Collections.ArrayList listMittDest=getCorrispondenti(dataRow["SYSTEM_ID"].ToString());
			
			string mittDest=string.Empty;

			for(int i=0;i<listMittDest.Count;i++)
			{
				if (!mittDest.Equals(string.Empty))
					mittDest += "; ";
				mittDest += (string) listMittDest[i];
			}

			dataRow["MITT_DEST"]=mittDest;

			logger.Debug("mittDest=" + mittDest);

			string classificazioni="";
			string separatore="";
			string systemIdDocumento=dataRow["SYSTEM_ID"].ToString();

			logger.Debug("Ricerca delle classificazioni");

            System.Collections.ArrayList classificazioniArray = getClassificazioni(infoUtente, systemIdDocumento);
			
			for(int i=0; i<=classificazioniArray.Count - 1; i++)
			{					
				classificazioni=classificazioni + separatore + ((DocsPaVO.fascicolazione.Fascicolo) classificazioniArray[i]).codice.ToString();
				separatore=", ";
			}

			dataRow["FASCICOLI"]=classificazioni;
			
			logger.Debug("classificazioni=" + classificazioni);

			if (!printWithFilters)
			{
				// Reperimento dell'impronta digitale, soltanto se stampa registro
				dataRow["IMPRONTA"]=getHash(dataRow["DOCNUMBER"].ToString());
			}
		}


		/// <summary>
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="data"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static bool isVariato(System.Data.DataRow profile, DateTime data)
		{
			logger.Debug("isVariato");
			bool variato=true;

			#region Codice Commentato
			//string ruoloVarString="";	
           // DocsPa_V15_Utils.database.SqlServerAgent db=new DocsPa_V15_Utils.database.SqlServerAgent();
			//bool dbOpen=false;
			#endregion 
			
			try
			{
                //db.openConnection();
				//dbOpen=true;
				
				DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();								

				if(profile["CHA_MOD_OGGETTO"].ToString().Equals("1"))
				{
					#region Codice Commentato
					/*
					ruoloVarString="SELECT "+Utils.dbControl.toChar("DTA_MODIFICA",true)+" AS DTA_MODIFICA FROM DPA_OGGETTI_STO WHERE ID_PROFILE="+profile["SYSTEM_ID"].ToString();
					logger.Debug(ruoloVarString);
					string dataVarString=(string)db.executeScalar(ruoloVarString);
					*/
					#endregion 

					string dataVarString = report.GetDateOgg(profile);

					logger.Debug(dataVarString);
					
					if(dataVarString!=null)
					{
					    DateTime dataVar=Convert.ToDateTime(dataVarString);
						logger.Debug("dataVar:"+dataVar.Year+" "+dataVar.Month+" "+dataVar.Day+" "+dataVar.Hour+" "+dataVar.Minute+" "+dataVar.Second);
						
						if(dataVar<data)
						{
						   return false;
						}
					}
				}

				if(profile["CHA_MOD_MITT_DEST"].ToString().Equals("1") || profile["CHA_MOD_MITT_INT"].ToString().Equals("1"))
				{
					#region Codice Commentato
					/*
					ruoloVarString="SELECT "+Utils.dbControl.toChar("DTA_MODIFICA",true)+" AS DTA_MODIFICA FROM DPA_CORR_STO WHERE ID_PROFILE="+profile["SYSTEM_ID"].ToString();
					logger.Debug(ruoloVarString);
					string dataVarString=(string)db.executeScalar(ruoloVarString);
					*/
					#endregion 

					string dataVarString = report.GetDateCorr(profile);

					logger.Debug(dataVarString);
					
					if(dataVarString!=null)
					{
						System.DateTime dataVar=Convert.ToDateTime(dataVarString);
						
						if(dataVar<data)
						{
						   return false;
						}
					}
				}

				return variato;
			}
			catch(Exception e)
			{
				#region Codice Commentato
				/*if(dbOpen)
				{
				  db.closeConnection();
				}*/
				#endregion 

				logger.Debug(e.Message);
				
				logger.Debug("Errore nella gestione di Report (isVariato)",e);
				throw e;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="idReg"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="anno"></param>
		/// <param name="docNumber"></param>
		/// <param name="debug"></param>
		private static void aggiornaStampaReg(string idReg,int protStart,int protEnd,int anno, string docNumber)
		{
		    logger.Debug("aggiornaStampaReg");
			
			//DocsPa_V15_Utils.database.SqlServerAgent db=new DocsPa_V15_Utils.database.SqlServerAgent();
			DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
			if (!report.AggiornaStampaReg(idReg, protStart, protEnd, anno, docNumber))
			{
				logger.Debug("Errore nella gestione di Report (aggiornaStampaReg)");
				throw new Exception();
			}

			#region Codice Commentato
			/*bool dbOpen=false;
			
			try
			{
				db.openConnection();
			
				dbOpen=true;
				DateTime now=System.DateTime.Now;
				string insertString="INSERT INTO DPA_STAMPAREGISTRI (";
				insertString=insertString+DocsPaDbManagement.Functions.Functions.GetSystemIdColName();
				insertString=insertString+"ID_REGISTRO,NUM_PROTO_START,NUM_PROTO_END,NUM_ANNO,NUM_ORD_FILE,DOCNUMBER,DTA_STAMPA) ";
				insertString = insertString + "VALUES (" + DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_STAMPAREGISTRI") + idReg + "," + protStart + "," + protEnd + "," + anno + ",1," + docNumber + "," + DocsPaDbManagement.Functions.Functions.GetDate() + ")";
				logger.Debug(insertString);
				
				db.executeNonQuery(insertString);
			}
			catch(Exception e)
			{
			  if(dbOpen)
			  {
			    db.closeConnection();
			  }

			  throw e;
			}*/
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="idReg"></param>
		/// <param name="anno"></param>
		/// <param name="protStart"></param>
		/// <param name="protEnd"></param>
		/// <param name="debug"></param>
		private static void aggiornaProfile(string idReg, int anno,int protStart,int protEnd)
		{
			#region Codice Commentato
			/*
			logger.Debug("aggiornaProfile");
			DocsPa_V15_Utils.Database db=DocsPa_V15_Utils.dbControl.getDatabase();
			//System.Data.DataSet ds=new System.Data.DataSet();
			System.Data.DataSet ds;
			bool dbOpen=false;
			bool dbTrans=false;
			
			try
			{
				System.Collections.ArrayList updateQueries=new System.Collections.ArrayList();
				
				db.openConnection();
				dbOpen=true;
                /*
				string profileString ="SELECT DISTINCT A.SYSTEM_ID " +
					"FROM PROFILE A "+
					"WHERE " +
					"A.CHA_TIPO_PROTO IN('A', 'P') " + 
					"AND A.NUM_ANNO_PROTO="+anno+" AND A.NUM_PROTO>="+protStart+" AND A.NUM_PROTO<="+protEnd+" AND A.ID_REGISTRO="+idReg;		
				profileString += " ORDER BY A.SYSTEM_ID";
				logger.Debug(profileString);
				db.fillTable(profileString,ds,"PROFILE");
				*/
				/*
				DocsPaDB.Query_DocsPAWS.Report obj = new DocsPaDB.Query_DocsPAWS.Report();				
				obj.getProfil(out ds,anno,protStart,protEnd,idReg);
				
				for(int i=0;i<ds.Tables["PROFILE"].Rows.Count;i++)
				{
				  // TODO: vedere migliorie effettuate sulla versione 2.0
				  string updateQuery="UPDATE PROFILE SET CHA_CONGELATO='1' WHERE SYSTEM_ID="+ds.Tables["PROFILE"].Rows[i]["SYSTEM_ID"];
				  updateQueries.Add(updateQuery);
				  logger.Debug("Aggiunta query: "+updateQuery);
				}

				//si esegue l'update
				db.beginTransaction();
				dbTrans=true;
				
				for(int i=0;i<updateQueries.Count;i++)
				{
				  db.executeNonQuery((string) updateQueries[i]);
				  logger.Debug("Eseguito update "+i);
				}
				
				db.commitTransaction();
			}
			catch(Exception e)
			{
				if(dbTrans)
				{
					db.rollbackTransaction();
				}
				
				if(dbOpen)
				{
					db.closeConnection();
				}
			
				throw e;
			}
			*/
			#endregion

			DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
			if (!report.AggiornaProfile(idReg,anno,protStart,protEnd))
			{
				logger.Debug("Errore nella gestione di Report (aggiornaProfile)");
				throw new Exception();
			}
		} 

		/// <summary>
		/// </summary>
		/// <param name="idDocProto"></param>
		/// <param name="db"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static ArrayList getCorrispondenti (string idDocProto) 
		{
			ArrayList list = new ArrayList();
			DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
			list = report.GetCorrispondenti(idDocProto);
			if (list == null)
			{
				logger.Debug("Errore nella gestione di Report (getCorrispondenti)");
				throw new Exception();
			}
			return list;

			#region Codice Commentato
			/*logger.Debug("getCorrispondenti");
			ArrayList mittDest = new ArrayList();
			string queryString = DocManager.getQueryCorrispondente(idDocProto);	
			logger.Debug(queryString);
			IDataReader dr = db.executeReader(queryString);
			
			while (dr.Read()) 
			{
				if(dr.GetValue(5).ToString().Equals("M")||dr.GetValue(5).ToString().Equals("D"))
				{
					mittDest.Add(DocManager.getCorrispondente(dr).descrizione);
				}

				if(dr.GetValue(5).ToString().Equals("C"))
				{
					mittDest.Add(DocManager.getCorrispondente(dr).descrizione+" (P.C.)");
				}

			}
			
			dr.Close();
			
			return mittDest;*/
			#endregion
		}

		/// <summary>
		/// </summary>
		/// <param name="num"></param>
		/// <param name="idReg"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		private static int getSup(int num,string idReg,int numProtStart)
		{
			logger.Debug("getSup");
			int res=0;
			string param=ConfigurationManager.AppSettings["NumProtSup_"+idReg];
			
			if(param==null || param.Equals("0"))
			{ 
				res= num;
			}
			else
			{
				int numPar=Int32.Parse(param);
				//26/02/2004 - considero il parametro NumProtSup_id_Reg non come il protocollo più grande ma come il numero di protocolli da stampare
				numPar = numPar + numProtStart -1;
				if(numPar<num)
				{
					res=numPar;
				}
				else
				{
					res=num;
				}
			}
			
			logger.Debug("res: "+res);
		
			return res; 
		}
	
		private static DataSet getDocProtocollati(string idReg, int anno, int protStart, int protEnd)
		{
			DataSet ds = new DataSet();
			DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
			report.getDocProtocollati(out ds, idReg, anno, protStart, protEnd);
			return ds;
		}
        //private static DataSet getDocProtocollatiCache(string idReg, int anno, int protStart, int protEnd)
        //{
        //    DataSet ds = new DataSet();
        //    DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
        //    report.getDocProtocollatiCache(out ds, idReg, anno, protStart, protEnd);
        //    return ds;
        //}

		private static DataSet getDocProtocollatiWithFilters(DocsPaVO.filtri.FiltroRicerca[][] filters, DocsPaVO.utente.Registro registro)
		{	
			DataSet ds = new DataSet();
			DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
			report.getDocProtocollatiWithFilters(out ds, filters, registro);
			return ds;
		}

        //INSERITA DA FABIO PRENDE LE ETICHETTE DEI PROTOCOLLI
        private static string getLettereProtocolli(DocsPaVO.utente.InfoUtente infoUtente, string etichetta)
        {
            DocsPaVO.documento.EtichettaInfo[] etichette;
            etichette = BusinessLogic.Documenti.EtichetteManager.GetInstance(infoUtente, infoUtente.idAmministrazione);
            if (etichetta.Equals("A"))
            {
                return etichette[0].Descrizione;
            }
            else
            {
                if (etichetta.Equals("P"))
                {
                    return etichette[1].Descrizione;
                }
                else
                {
                    if (etichetta.Equals("I"))
                    {
                        return etichette[2].Descrizione;
                    }
                    else
                    {
                        if (etichetta.Equals("ALL"))
                        {
                            return etichette[4].Descrizione;
                        }
                        else
                        {
                            return etichette[3].Descrizione;
                        }
                    }
                }
            }
        }

        public static string StampaRegExperimental(string username, string codeReg, string protoDa, string protoA, string anno, string dataStampa, string idOldDoc, string idAmm )
        {
            logger.Debug("StampaRegistroWithFilters");
            string retVal = "";
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {

                string outputFileName = "";
                string rootPath = AppDomain.CurrentDomain.BaseDirectory + "report/TemplateXML/";
                string templateFilePath = rootPath + "XMLRepStampaRegistro.xml";
                string schemaFilePath = rootPath + "XMLReport.xsd";
                string basePathFiles = DocsPaUtils.Functions.Functions.GetReportsPath();
                DocsPaUtils.Functions.Functions.CreateDirectory(basePathFiles);
                DocsPaDB.Query_DocsPAWS.Report dsoReport = new DocsPaDB.Query_DocsPAWS.Report();

                DataSet dsStampaRegistro = new DataSet();
                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(codeReg);
                if (reg != null)
                {

                    #region creazione filtri
                    DocsPaVO.filtri.FiltroRicerca[][] qV;
                    DocsPaVO.filtri.FiltroRicerca[] fVList;
                    qV = new DocsPaVO.filtri.FiltroRicerca[1][];
                    qV[0] = new DocsPaVO.filtri.FiltroRicerca[1];
                    fVList = new DocsPaVO.filtri.FiltroRicerca[3];

                    DocsPaVO.filtri.FiltroRicerca filtro = new DocsPaVO.filtri.FiltroRicerca();
                    filtro.argomento = "ANNO_PROTOCOLLO";
                    filtro.valore = anno;
                    fVList[0] = filtro;

                    filtro = new DocsPaVO.filtri.FiltroRicerca();
                    filtro.argomento = "NUM_PROTOCOLLO_DAL";
                    filtro.valore = protoDa;
                    fVList[1] = filtro;

                    filtro = new DocsPaVO.filtri.FiltroRicerca();
                    filtro.argomento = "NUM_PROTOCOLLO_AL";
                    filtro.valore = protoA;
                    fVList[2] = filtro;

                    qV[0] = fVList;
                    #endregion
                    // Recupero documenti da stampare
                    DataTable tableProfile = (getDocProtocollatiWithFilters(qV, reg)).Tables[0];

                    #region prelievo info utili
                    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(username, idAmm);
                    DocsPaVO.utente.Ruolo ruolo;
                    DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[])BusinessLogic.Utenti.UserManager.getRuoliUtente(utente.idPeople).ToArray(typeof(DocsPaVO.utente.Ruolo));
                    if (ruoli != null && ruoli.Length > 0)
                    { ruolo = ruoli[0]; }
                    else
                    { ruolo = null; }

                    dsoReport.GetCodReg(out dsStampaRegistro, reg.systemId);

                    string descrizioneRegistro = "";
                    string descrizioneAmministrazione = "";
                    if (dsStampaRegistro.Tables["REG"].Rows.Count > 0)
                    {
                        //descrizioneRegistro = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_CODICE"].ToString();
                        descrizioneRegistro = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_DESC_REGISTRO"].ToString();
                        descrizioneAmministrazione = dsStampaRegistro.Tables["REG"].Rows[0]["VAR_DESC_AMM"].ToString();
                    }

                    DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

                    if (infoUtente != null && string.IsNullOrEmpty(infoUtente.dst))
                        infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();


                    #endregion

                    #region INIZIO CREAZIONE STREAM PDF
                    // Apertura stream su file template per la stampa
                    FileStream fs = null;
                    
                    try
                    {
                        
                        // Lettura del file template xml


                        fs = new FileStream(templateFilePath, FileMode.Open, FileAccess.Read);

                        StampaPDF.Report stampaRegistro = new StampaPDF.Report(fs, schemaFilePath, null, dataStampa);

                        // Creazione stampa

                        // Sostituzione parametri dinamici della stampa
                        Hashtable printParams = new Hashtable();
                        printParams.Add("@paramAmm", descrizioneAmministrazione);
                        printParams.Add("@param0", descrizioneRegistro + " La presente stampa riporta tutte le registrazioni giornaliere di protocollo nell'intervallo indicato. Si è resa necessaria in quanto problematiche tecniche del sistema avevano inizialmente impedito la corretta stampa.");
                        printParams.Add("@param1", protoDa);
                        printParams.Add("@param2", protoA);

                        stampaRegistro.appendParagraph("PAR_AMMINISTRA", printParams, false);
                        stampaRegistro.appendParagraph("PAR_REGISTRO", printParams, false);
                        stampaRegistro.appendParagraph("PAR_RANGE_PROTOCOLLO", printParams, false);

                        //DataTable tableProfileMod = tableProfile;
                        //int i = 0;
                        //foreach (DataRow rowMod in tableProfileMod.Rows)
                        //{
                        //    rowMod.Table.Rows[i]["CHA_TIPO_PROTO"] = getLettereProtocolli(infoUtente, rowMod.Table.Rows[i]["CHA_TIPO_PROTO"].ToString());
                        //    //modifica
                        //    if (!string.IsNullOrEmpty(rowMod.Table.Rows[i]["IMPRONTA_CACHE"].ToString()))
                        //    {
                        //        string impronta = rowMod.Table.Rows[i]["IMPRONTA_CACHE"].ToString();
                        //        rowMod.Table.Rows[i]["IMPRONTA"] = impronta;
                        //    }
                        //    //fine modifica
                        //    i++;
                        //}
                        stampaRegistro.appendTable("TABLE_PROTOCOLLO", tableProfile, false);

                        // Chiusura stream file template xml
                        fs.Close();
                        fs = null;

                        // Composizione del nome del file PDF di output
                        outputFileName = string.Format("{0}_{1}_{2}.{3}", descrizioneRegistro, protoDa, protoA, "PDF");

                        MemoryStream memoryStream = stampaRegistro.getStream();

                        stampaRegistro.close();

                        Stream outputStream = new FileStream(
                                            DocsPaUtils.Functions.Functions.GetReportsPath() + @"\" + outputFileName,
                                            FileMode.Create,
                                            FileAccess.Write);

                        Byte[] data = memoryStream.GetBuffer();

                        outputStream.Write(data, 0, data.Length);

                        memoryStream.Close();
                        memoryStream = null;

                        outputStream.Flush();
                        outputStream.Close();
                        outputStream = null;
                       
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    

                    #endregion FINE CREAZIONE FILE PDF


                    if (outputFileName != "")
                    {
                        //si inserisce il file generato nel documentale
                        retVal = insertDocumentale(reg.systemId, Int32.Parse(anno), Int32.Parse(protoDa), Int32.Parse(protoA), infoUtente, ruolo, outputFileName, true, false, null);

                        //si aggiorna la tabella DPA_STAMPAREGISTRI
                        aggiornaStampaReg(reg.systemId, Int32.Parse(protoDa), Int32.Parse(protoA), Int32.Parse(anno), retVal);

                        //si aggiorna la tabella PROFILE
                        aggiornaProfile(reg.systemId, Int32.Parse(anno), Int32.Parse(protoDa), Int32.Parse(protoA));

                        DocsPaDB.Query_DocsPAWS.Documenti docsDB = new DocsPaDB.Query_DocsPAWS.Documenti();
                        docsDB.CestinaDocumento(utente.idPeople, idOldDoc, "Rigenerazione");

                        if (!string.IsNullOrEmpty(dataStampa))
                        {
                            dsoReport.UpdStampaExp(retVal, dataStampa);
                        }


                        transactionContext.Complete();
                    }
                    else
                    {
                        
                    }
                }

            }


            return retVal;
        }

        public static string StampaRegExp2(string idDocument, string username)
        {
            string retval = "";
            DocsPaDB.Query_DocsPAWS.Report x1 = new DocsPaDB.Query_DocsPAWS.Report();
            DataSet ds= new DataSet();
            x1.getDatiStampaReg(idDocument, out ds);
            if (ds != null)
            {
                //ds.Tables[0].Rows[0][""].ToString();
                DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(ds.Tables[0].Rows[0]["ID_REGISTRO"].ToString());
                string protoDa = ds.Tables[0].Rows[0]["NUM_PROTO_START"].ToString();
                string protoA = ds.Tables[0].Rows[0]["NUM_PROTO_END"].ToString();
                 string anno=ds.Tables[0].Rows[0]["NUM_ANNO"].ToString();
                 string dataStampa=ds.Tables[0].Rows[0]["DATASTAMPA"].ToString();

                 retval = StampaRegExperimental(username, reg.codRegistro, protoDa, protoA, anno, dataStampa, idDocument, reg.idAmministrazione);
            
            }

            return retval;
        }
	}
}
