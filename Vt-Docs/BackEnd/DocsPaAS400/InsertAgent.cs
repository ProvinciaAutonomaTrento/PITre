using System;
using DocsPaAS400.tableFields;
using DocsPaAS400.fields;
using System.Collections;
using log4net;

namespace DocsPaAS400
{
	/// <summary>
	/// Summary description for InsertAgent.
	/// </summary>
	public class InsertAgent
	{
        private ILog logger = LogManager.GetLogger(typeof(InsertAgent));

		public void addDocFolder(DocsPaVO.documento.SchedaDocumento schedaDoc,string idProfile,string idFolder)
		{DocsPaDB.DBProvider DB=null;
			try
			{
			DB=new DocsPaDB.DBProvider();
				#region CODICE docspa2.0			 
//			string queryString = 
//				"SELECT COUNT (*) FROM PROJECT_COMPONENTS WHERE PROJECT_ID=" + idFolder +
//				" AND LINK=" + idProfile;
//			DocsPaUtils.Query q=new DocsPaUtils.Query(queryString);
//			
//			string rows="";
//			if(!DB.ExecuteScalar(out rows,queryString))
//				throw new Exception("Errore nella Query "+queryString);
//			
//		if(rows.Equals("0"))
//			throw new Exception("Il documento è già presente nel folder");
				#endregion
			bool classified=isClassified(idProfile,DB);
			
				

				// questa controllo mi serve perchè solo se è la prima fasc del doc finisce in as400.
				
				if(!classified)
				{
					DB.BeginTransaction();
					ArrayList queries=getInsertQueries(DocsPaAS400.Constants.CREATE_MODIFY_OPERATION,schedaDoc,DB);
				
					for(int i=0;i<queries.Count;i++)
					{
						DB.ExecuteLockedNonQuery((string) queries[i]);
					}
					DB.CommitTransaction();
				}
				
			}
			catch(Exception e)
			{
              DB.RollbackTransaction();
			   throw e;
			}
			finally
			{
				if(DB!=null)
					DB.Dispose();
			}

		}

		public ArrayList getInsertQueries(string operation,DocsPaVO.documento.SchedaDocumento schedaDoc,DocsPaDB.DBProvider db)
		{
			//DocsPaDB.DBProvider db=null;
			logger.Debug("DocsPaAS400: getInsertQueries");
			ArrayList res=new ArrayList();
			if(schedaDoc.protocollo==null || schedaDoc.protocollo.numero==null || schedaDoc.protocollo.numero.Equals(""))
			{
			    logger.Debug("Documento non protocollato");	
				return res;
			}
			string folderCode=getFolderCode(schedaDoc);
			if(folderCode!=null && !folderCode.Equals(""))
			{
				logger.Debug("Folder code="+folderCode);
			}
			else
			{
                logger.Debug("Folder code nullo");
				return res;
			}
			try
			{
				

				string key="";
				string keyQuery=getSelectKeyQuery();
				logger.Debug(keyQuery);
				
				string temp="";
				//db=new DocsPaDB.DBProvider();
				//db.ExecuteScalar(out temp,keyQuery);
			using ( System.Data.IDataReader reader=db.ExecuteReader(keyQuery))
			 {
				 if (reader.Read())
					 temp=reader.GetValue(reader.GetOrdinal("K")).ToString();
			 }
				
			 
				if(temp==null || temp.Equals(""))
				{
					key="0";
				}
				else
				{
					int val=Int32.Parse(temp)+1;
					key=val.ToString();
					
					
				}
				logger.Debug("lkey:"+key);

				System.Collections.ArrayList queries=getQueryArray(key,operation,folderCode,schedaDoc);            
				for(int i=0;i<queries.Count;i++)
				{
					string query=(string) queries[i];
					logger.Debug(query);
					res.Add(query);
				}
				string insertLogQuery=getInsertLogQuery(key);
				logger.Debug(insertLogQuery);
				res.Add(insertLogQuery);
				
			}
			catch(Exception e)
			{
				
				throw e;
			}
			finally
			{
				
			}
			return res;
		}

		private System.Collections.ArrayList getQueryArray(string key,string operation,string folderCode,DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			logger.Debug("Costruzione queries");
			System.Collections.ArrayList queries=new System.Collections.ArrayList();
			TableField[] fields=new TableField[]{TableFieldAgent.getTableFieldInstance(Constants.JOUREC_FIELD_NAME),
												 TableFieldAgent.getTableFieldInstance(Constants.JOUDOC_FIELD_NAME),
				                                 TableFieldAgent.getTableFieldInstance(Constants.JOUCOD_FIELD_NAME),
			                                     TableFieldAgent.getTableFieldInstance(Constants.JOUPRO_FIELD_NAME),
				                                 TableFieldAgent.getTableFieldInstance(Constants.JOUTYP_FIELD_NAME),
				                                 TableFieldAgent.getTableFieldInstance(Constants.JOULEN_FIELD_NAME),
				                                 TableFieldAgent.getTableFieldInstance(Constants.JOUDES_FIELD_NAME),
			                                     };
			for(int i=0;i<=Constants.NUM_CODES;i++)
			{
               //COSTRUZIONE DELLA SINGOLA QUERY
			   //costruzione insert context
               InsertContext ic=new InsertContext();
			   ic.numRow=i;
			   ic.operation=operation;
			   ic.schedaDoc=schedaDoc;
               ic.val=getValue(i,schedaDoc,folderCode,operation);
               string queryString=getQuery(ic,key,fields);
			   logger.Debug("Creata query "+i+": "+queryString);
               queries.Add(queryString);
			}
			//INSERIMENTO DELL'OGGETTO
			logger.Debug("Inserimento oggetto");
			string oggettoString=schedaDoc.oggetto.descrizione;
			Oggetto oggetto=new Oggetto(oggettoString);
			ArrayList oggettoList=oggetto.split(Constants.OBJECT_ROW_LENGTH);
			for(int i=0;i<oggettoList.Count;i++)
			{
				InsertContext ic=new InsertContext();
				ic.numRow=Constants.NUM_CODES+1+i;
				ic.operation=operation;
				ic.schedaDoc=schedaDoc;
                ic.val=(string) oggettoList[i];
				string queryString=getQuery(ic,key,fields);
				logger.Debug("Creata query "+(Constants.NUM_CODES+1+i)+": "+queryString);
				queries.Add(queryString);
			}
																			  
			return queries;
		}

		private string getQuery(InsertContext ic,string key,TableField[] fields)
		{
			string queryString="INSERT INTO "+Constants.TEWDOCSPA_TABLE_NAME+" (["+Constants.KEY_FIELD_NAME+"],";
			for(int j=0;j<fields.Length;j++)
			{
				queryString=queryString+"["+fields[j].getFieldName()+"]";
				if(j<fields.Length-1)
				{
					queryString=queryString+",";
				}
			}
			queryString=queryString+") VALUES (";
			queryString=queryString+key+",";
			for(int j=0;j<fields.Length;j++)
			{
				queryString=queryString+fields[j].getFieldSQLValue(ic);
				if(j<fields.Length-1)
				{
					queryString=queryString+",";
				}
			}
			queryString=queryString+")";
            return queryString;
		}
		
		private string getValue(int numRow,DocsPaVO.documento.SchedaDocumento schedaDoc,string folderCode,string operation)
		{
	        string res="";
			switch(numRow)
			{
				case 0: //prima riga 
					    res="";
                        break;
				case 1: //numero protocollo
					    res=Utils.sevenChars(schedaDoc.protocollo.numero);
					    break;
				case 2: //data protocollo
					    res=Utils.getDate(schedaDoc.protocollo.dataProtocollazione);
					    break;
				case 3: //anno protocollo
					    res=Utils.getYear(schedaDoc.protocollo.dataProtocollazione);
				        break;
				case 4: //arrivo/partenza
					    if(schedaDoc.protocollo.GetType().Equals(typeof(DocsPaVO.documento.ProtocolloEntrata)))
						{
                          res="A";
						}
						else
						{
                          res="P";
						}
					    break;
                case 5: //classificazione 0
					    res=Utils.getClassificationCode(schedaDoc.registro.idAmministrazione,folderCode,0);
					    break;
				case 6: //classificazione 1
					    res=Utils.getClassificationCode(schedaDoc.registro.idAmministrazione,folderCode,1);
					    break;
				case 7: //classificazione 2
					    res=Utils.getClassificationCode(schedaDoc.registro.idAmministrazione,folderCode,2);
				        break;			case 8: //classificazione 3
					    res=Utils.getClassificationCode(schedaDoc.registro.idAmministrazione,folderCode,3);
					    break;
				case 9: //classificazione 4
					    res=Utils.getClassificationCode(schedaDoc.registro.idAmministrazione,folderCode,4);
					    break;
				case 10://allegati 
					    /*if(schedaDoc.allegati==null)
						{
							 res="0";
						 }
						 else
						 {
                             res=schedaDoc.allegati.Count.ToString();
						 }*/
					     res="";
					     break;
				case 11: //dettagli mittente
					     res=Utils.getDettagliMittDest(schedaDoc);
					     break;
				case 12: //numero protocollo mittente
					     res=Utils.getNumProtMitt(schedaDoc);
					     break;
                case 13: //data protocollo mittente
					     res=Utils.getDataProtMitt(schedaDoc);
					     break;
				case 14: //provenienza mittente
					     res=Utils.getProvenienzaMitt(schedaDoc);
					     break;
				case 15: //tipologia atto
					     if(schedaDoc.tipologiaAtto!=null)
						 {
							 res=Utils.getCorrectString(schedaDoc.tipologiaAtto.descrizione);
						 }
						 else
						 {
                             res="";
						 }
					     break;
				case 16: //da lasciare vuota
					     res="";
					     break;
				case 17: //assegnazione: da lasciare vuota
					     res="";
					     break;
				case 18: //numero oggetto consiliare
					     res=Utils.getCorrectString(schedaDoc.numOggetto);
					     break;
				case 19: //commissione referente
					     res=Utils.getCorrectString(schedaDoc.commissioneRef);
				         break;
				case 20: //annullamento
					     if(operation.Equals(Constants.DELETE_OPERATION))
					     {
                            res="A";
					     }
					     else
					     {
                            res="";
					     }
					     break;
			    case 21: //iniziali operatore
					     res=Constants.OPERATOR_INIT;
					     break;
			}
			return res;
		}

		private string getSelectKeyQuery()
		{
           logger.Debug("getSelectKeyQuery");
		   string res="";
 res="SELECT MAX(A.["+Constants.KEY_FIELD_NAME+"]) as K FROM "+Constants.TEWDOCSPA_TABLE_NAME+" A,"+Constants.LOGDOCSPA_TABLE_NAME+" B WHERE A.["+Constants.KEY_FIELD_NAME+"]=B.["+Constants.KEY_FIELD_NAME+"]";
		   return res;
		}

		private string getInsertLogQuery(string key)
		{
            logger.Debug("getInsertLogQuery");
			string res="";
			res="INSERT INTO "+Constants.LOGDOCSPA_TABLE_NAME+" (["+Constants.KEY_FIELD_NAME+"],["+Constants.FLAG_FIELD_NAME+"],["+Constants.TS_DOCSPA_FIELD_NAME+"]) VALUES ("+key+",0,"+Utils.getTimestamp()+")";
            return res;
		}

		private string getFolderCode(DocsPaVO.documento.SchedaDocumento schedaDoc)
		{
			DocsPaDB.DBProvider db=null;
			try
			{
				logger.Debug("getFolderCode");
				logger.Debug("Metodo modificato per permettere l'invio di documenti inseriti in fascicoli procedimentali");
				string res=null;
				string q="SELECT PROJECT_ID FROM PROJECT_COMPONENTS WHERE LINK="+schedaDoc.systemId;
				DocsPaUtils.Query folderId=new DocsPaUtils.Query(q);
				//folderId.addColumn("PROGETTI_DOC.ID_PROGETTO",co);
				//folderId.addCondition("PROGETTI_DOC.ID_DOC_PROG",,"=", "AND", co);
				logger.Debug(q);

				string id="";
				db=new DocsPaDB.DBProvider();
				db.ExecuteScalar(out id,q);
				if(id!=null && !id.Equals(""))
				{
					string codFasc=null;
					logger.Debug("Ricerca id fascicolo");
					string qf="SELECT ID_FASCICOLO FROM PROJECT WHERE SYSTEM_ID="+id;
					DocsPaUtils.Query folderCodeQuery=new DocsPaUtils.Query(qf);
					//folderCodeQuery.addColumn("PROGETTI.ID_FASCICOLO",co);
					//folderCodeQuery.addCondition("PROGETTI.SYSTEM_ID",id,"=","AND",co);
					db.ExecuteScalar(out   codFasc,qf);

					if(codFasc!=null && !codFasc.Equals(""))
					{
						logger.Debug("Ricerca codice fascicolo");
						string qc="SELECT UPPER(VAR_CODICE) FROM PROJECT WHERE SYSTEM_ID="+codFasc;
			
						DocsPaUtils.Query fascCodeQuery=new DocsPaUtils.Query(qc);
						//	fascCodeQuery.addColumn("PROGETTI.VAR_CODICE",co);
						//	fascCodeQuery.addCondition("PROGETTI.SYSTEM_ID",codFasc,"=","AND",co);
						db.ExecuteScalar(out res,qc);
					}
				}
				return res;
			}
			finally
			{
				if(db!=null)
					db.Dispose();
			}
		}
		
		private bool isClassified(string idProfile,DocsPaDB.DBProvider db)
		{
           
			bool res=false;
			string queryString = 
				"SELECT COUNT (*) FROM PROJECT_COMPONENTS WHERE LINK=" + idProfile + " AND TYPE='D'";
           string  queryRes="";
			db.ExecuteScalar(out  queryRes,queryString);
		
			
			if(queryRes.Equals("1")) //Docspa30, prima lo fascicola poi parte as400, che deve fare le sue insert
									//solo se è la prima classsificazione/fascicolazione.
			{
				res=false;
			}
			else
			{
                res=true;
			}
			return res;
		}

	}
}
