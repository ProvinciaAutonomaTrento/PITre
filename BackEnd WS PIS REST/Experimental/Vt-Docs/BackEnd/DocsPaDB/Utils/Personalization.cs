using System;
using System.Collections;
using System.Data;
//using System.Data.OleDb;

namespace DocsPaDB.Utils
{
	/// <summary>
	/// </summary>
	public class Personalization
	{
		private static Hashtable objects;
	    //private	Database database;
		//private DocsPaWS.Utils.Debug debug;
		private string codiceAmministrazione;
		private string separator;
		
		private string sepFascicolo = "/";
		private ArrayList formatFascicolo;

		private string sepSegnatura = "/";
		private ArrayList formatSegnatura;

		private string segnatura;
		private string fascicolatura;
		
		private string library;
		private string interopPrefix= "INTEROP_";

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="debug"></param>
		private Personalization(string idAmm)
		{
			//this.debug=debug;
			//database = dbControl.getDatabase();
			
			// TODO: Utilizzare il progetto DocsPaDbManagement
			//database.openConnection();
			getSeparatorTable(idAmm);
//			setSepFascicolo(idAmm);
//			setFormatFascicolo(idAmm);
//			setSepSegnatura(idAmm);
//			setFormatSegnatura(idAmm);
			setLibrary (idAmm);
			setCodiceAmministrazione(idAmm);

			SetSegnaturaFascicolatura(idAmm);
			
			// TODO: Utilizzare il progetto DocsPaDbManagement
			//database.closeConnection();
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		/// <param name="debug"></param>
		/// <returns></returns>
		public static Personalization getInstance(string idAmm)
		{
            //if (objects == null)
            //    objects = Hashtable.Synchronized(new Hashtable());

            //string key = idAmm.ToLower();
            
            //Personalization retValue = null;
            
            //if (!objects.ContainsKey(key))
            //{
            //    retValue = new Personalization(key);

            //    objects.Add(key, retValue);
            //}
            //else
            //{
            //    retValue = (Personalization)objects[key];
            //}

            //return retValue;

            if (objects == null || !objects.ContainsKey(idAmm))
            {
                try
                {
                    if (objects == null)
                    {
                        objects = new Hashtable();
                    }

                    Personalization newObject = new Personalization(idAmm);
                    objects.Add(idAmm, newObject);

                    return newObject;
                }
                catch (ArgumentException e)
                {
                    return (Personalization)objects[idAmm];
                }
            }
            else
            {
                return (Personalization)objects[idAmm];
            }			
	    }


		public static void Reset()
		{
			objects=null;
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void getSeparatorTable(string idAmm)
		{
		    //DataSet dataSet=new DataSet();
			DataSet dataSet;
			//string queryString="SELECT CHA_SEPARATORE FROM DPA_AMMINISTRA WHERE SYSTEM_ID='"+idAmm+"'";
			//database.fillTable(queryString,dataSet,"SEPARATORE");
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			obj.getSepar(out dataSet,idAmm);

			separator= dataSet.Tables["SEPARATORE"].Rows[0]["CHA_SEPARATORE"].ToString();
		    dataSet.Clear();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getSeparator()
		{
            return separator;
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setSepFascicolo(string idAmm) 
		{
			//string queryString = "SELECT CHA_STR_FISSA FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;					
			//this.sepFascicolo = database.executeScalar(queryString).ToString();
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			this.sepFascicolo = obj.getSeparFasc(idAmm);
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setSepSegnatura(string idAmm) 
		{
			//string queryString = "SELECT CHA_STR_SEGNATURA FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
			//this.sepSegnatura = database.executeScalar(queryString).ToString();
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			this.sepSegnatura = obj.getSepSeg(idAmm);
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void SetSegnaturaFascicolatura(string idAmm) 
		{
			DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();				
			amministrazioneXml.GetSegnaturaFascicolatura(idAmm, out this.segnatura, out this.fascicolatura);
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setFormatFascicolo(string idAmm) 
		{
			formatFascicolo = new ArrayList();
			//string queryString = "SELECT VAR_STRINGA,ID_PESO FROM DPA_FORMATTA_FASC WHERE CHA_VISUALIZZA = '1' AND ID_AMM = " + idAmm +  " ORDER BY ID_PESO";
			
			/*
			IDataReader dr = database.executeReader(queryString);

			while (dr.Read()) 
			{
				this.formatFascicolo.Add(dr.GetString(0)); 
			}

			dr.Close();
			*/

			DataSet dataSet;
			//this.ExecuteQuery(out dataSet, queryString);
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			obj.getFormatFasc(out dataSet, idAmm);

			foreach(DataRow row in dataSet.Tables[0].Rows)
			{
				this.formatFascicolo.Add(row[0].ToString()); 
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setFormatSegnatura(string idAmm) 
		{
			this.formatSegnatura = new ArrayList();
			//string queryString = "SELECT VAR_STRINGA,ID_PESO FROM DPA_FORMATTA_SEGN WHERE CHA_VISUALIZZA = '1' AND ID_AMM = " + idAmm +  " ORDER BY ID_PESO";			                      
			
			/*
			IDataReader dr = database.executeReader(queryString);
			
			while (dr.Read()) 
			{
				this.formatSegnatura.Add(dr.GetString(0)); 
			}
			dr.Close();
			*/

			DataSet dataSet;
			//this.ExecuteQuery(out dataSet, queryString);
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			obj.getFormatSegn(out dataSet, idAmm);

			foreach(DataRow row in dataSet.Tables[0].Rows)
			{
				this.formatSegnatura.Add(row[0].ToString()); 
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setLibrary (string idAmm) 
		{
			//string sqlString = "SELECT VAR_LIBRERIA FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
			//library = database.executeScalar(sqlString).ToString();	
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			library = obj.getLib(idAmm);
		}

		/// <summary>
		/// </summary>
		private void setInteropPrefix()
		{
			//da riempire con richiamo al DB;
		}

		/// <summary>
		/// </summary>
		/// <param name="idAmm"></param>
		private void setCodiceAmministrazione (string idAmm) 
		{
			//string sqlString = "SELECT VAR_CODICE_AMM FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
			//codiceAmministrazione = database.executeScalar(sqlString).ToString();	
			DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();				
			codiceAmministrazione = obj.getCodAmm(idAmm);
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getSepFascicolo() 
		{
			return this.sepFascicolo;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public ArrayList getFormatFascicolo() 
		{
			return this.formatFascicolo;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getSepSegnatura() 
		{
			return this.sepSegnatura;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public ArrayList getFormatSegnatura() 
		{
			return this.formatSegnatura;
		}

		/// <summary>
		/// </summary>
		public string FormatoSegnatura 
		{
			get
			{
				return this.segnatura;
			}
		}

		/// <summary>
		/// </summary>
		public string FormatoFascicolatura
		{
			get
			{
				return this.fascicolatura;
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getLibrary() 
		{
			return this.library;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getInteropPrefix() 
		{
		    return this.interopPrefix;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string getCodiceAmministrazione() 
		{
			return this.codiceAmministrazione;
		}
	}
}
