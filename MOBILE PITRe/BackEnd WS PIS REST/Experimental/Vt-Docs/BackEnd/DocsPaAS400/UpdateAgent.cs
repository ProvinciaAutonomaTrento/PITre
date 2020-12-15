using System;
using System.Data;
using System.Collections;
using DocsPaAS400.fields;
using log4net;

namespace DocsPaAS400
{
	/// <summary>
	/// Summary description for UpdateAgent.
	/// </summary>
	public class UpdateAgent
	{
        private static ILog logger = LogManager.GetLogger(typeof(UpdateAgent));

		public ArrayList update()
		{
            ArrayList res=new ArrayList();
			ArrayList records=getUpdateKeys();
			for(int i=0;i<records.Count;i++)
			{
				string key=(string) records[i];
				Oggetto oggetto=getOggettoFromKey(key);
				res.Add(updateOggetto(oggetto));
			}
			return res;
		}

		private string updateOggetto(Oggetto oggetto)
		{
           logger.Debug("updateOggetto");
			try
			{ 
				logger.Debug("Inizio transazione");
				//co.db.beginTransaction(co.debug);
				//RICERCA NELLA TABELLA DELL'OGGETTARIO
				getInfoOggetto(oggetto);
                string oggettarioQuery=getQueryOggettario(oggetto);          
				//co.db.commitTransaction(co.debug);
				return oggettarioQuery;
			}
			catch(Exception e)
			{
				//co.db.rollbackTransaction(co.debug);
				return e.Message;
			}
			return null;
		}

		private ArrayList getUpdateKeys()
		{
			logger.Debug("getUpdateKeys");
			ArrayList res=new ArrayList();
            string queryString="SELECT DISTINCT A.["+Constants.KEY_FIELD_NAME+"] FROM "+Constants.LOGAS400_TABLE_NAME+" A,"+Constants.TEWAS400_TABLE_NAME+" B";
			queryString=queryString+" WHERE A.["+Constants.KEY_FIELD_NAME+"]=B.["+Constants.KEY_FIELD_NAME+"]";
			queryString=queryString+" AND A.["+Constants.FLAG_FIELD_NAME+"]=0";
            logger.Debug(queryString);
			AS400Database db=new AS400Database();
			IDataReader dr=db.executeReader(queryString);
			logger.Debug("Query eseguita");

			while(dr.Read())
			{
			   logger.Debug("Qui");
               string temp=dr.GetValue(0).ToString();
			   logger.Debug("Ris: "+temp);
			   res.Add(temp);
			}
            return res;
		}

		private Oggetto getOggettoFromKey(string key)
		{
            logger.Debug("getOggettoFromKey");
			Oggetto res=null;
            string queryString="SELECT ["+Constants.JOUDES_FIELD_NAME+"] FROM "+Constants.TEWAS400_TABLE_NAME;
			queryString=queryString+" WHERE ["+Constants.KEY_FIELD_NAME+"]="+key;
            queryString=queryString+" AND (["+Constants.JOUCOD_FIELD_NAME+"]="+Constants.JOUCOD_OGGETTO_VALUE;
			queryString=queryString+" OR ["+Constants.JOUCOD_FIELD_NAME+"]="+Constants.JOUCOD_PROTO_VALUE;
			queryString=queryString+") ORDER BY ["+Constants.JOUPRO_FIELD_NAME+"]";
            AS400Database db=new AS400Database();
			IDataReader dr=db.executeReader(queryString);
            ArrayList valuesList=new ArrayList();
			dr.Read();
			string numProt=dr.GetValue(0).ToString();
			while(dr.Read())
			{
				string temp=dr.GetValue(0).ToString();
				valuesList.Add(temp);
			}
			if(valuesList.Count>0)
			{
				res=new Oggetto(valuesList);
				res.setNumProt(Int32.Parse(numProt).ToString());
			}
			return res;
		}

		private string getQueryOggettario(Oggetto oggetto)
		{
           logger.Debug("getQueryOggettario");
		   string res=null;
			string q="SELECT SYSTEM_ID FROM DPA_OGGETTARIO WHERE UPPER(VAR_DESC_OGGETTO)=UPPER("+oggetto.getValue()+")";
		  DocsPaUtils.Query query=new DocsPaUtils.Query(q);
           //query.addColumn("OGGETTARIO.SYSTEM_ID",co);
		   //query.addCondition("OGGETTARIO.VAR_DESC_OGGETTO",oggetto.getValue(),"=","AND");
          // query.getSQL()
			res=query.getSQL()+" "+oggetto.getNumProt();
		   return res;
		}

		private void getInfoOggetto(Oggetto oggetto)
		{
            logger.Debug("getInfoOggetto");
			DocsPaUtils.Query infoQuery=new DocsPaUtils.Query("");
		}

		private string getCommRefFromKey(string key)
		{
			logger.Debug("getCommRefFromKey");
			string res=null;
			string queryString="SELECT ["+Constants.JOUDES_FIELD_NAME+"] FROM "+Constants.TEWAS400_TABLE_NAME;
			queryString=queryString+" WHERE ["+Constants.KEY_FIELD_NAME+"]="+key;
			queryString=queryString+" AND ["+Constants.JOUCOD_FIELD_NAME+"]="+Constants.JOUCOD_COMM_REF_VALUE;
			AS400Database db=new AS400Database();
			res=(string) db.executeScalar(queryString);
            return res;
		}

	}
}
