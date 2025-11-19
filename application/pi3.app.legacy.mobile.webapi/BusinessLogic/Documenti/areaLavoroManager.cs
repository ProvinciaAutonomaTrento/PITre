// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Documenti;

    public class areaLavoroManager
    {
        private static ILogger logger = Log.ForContext(typeof(areaLavoroManager));

        public static void execAddLavoroMethod(string idProfile, string tipoProto, string idRegistro, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fasc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            if (!doc.ExeAddLavoro(idProfile, tipoProto, idRegistro, infoUtente, fasc))
            {
                logger.Debug("Errore nella gestione dell'area lavoro (execAddLavoroMethod)");

                throw new Exception();
            }
            #region codice originale
            /*
		DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
		DataSet dataSet = new DataSet();
		try
		{
			db.openConnection();
			//costruzione della query
			string idPeople=infoUtente.idPeople;
			string idRuoloInUo=infoUtente.idCorrGlobali;
			System.DateTime now=System.DateTime.Now;
			CultureInfo ci = new CultureInfo("en-US"); 
			string dateString=DocsPaWS.Utils.dbControl.toDate(now.ToString("dd/MM/yyyy hh:mm:ss tt", ci ),true);
			if(infoDoc!=null)
			{
				string idProfile=infoDoc.idProfile;
                    string tipoProto=infoDoc.tipoProto;
				string queryDoc="SELECT SYSTEM_ID, CHA_TIPO_DOC FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROFILE="+idProfile;
				db.fillTable(queryDoc,dataSet,"DOC");
				
				if(dataSet.Tables["DOC"].Rows.Count==0)
				{
					//si esegue l'inserimento
					insertString="INSERT INTO DPA_AREA_LAVORO (SYSTEM_ID,ID_PEOPLE,ID_RUOLO_IN_UO,ID_PROFILE,CHA_TIPO_DOC,DTA_INS)";
					insertString=insertString+" VALUES ('1','"+idPeople+"','"+idRuoloInUo+"','"+idProfile+"','"+tipoProto+"',"+dateString+")";
					db.executeNonQuery(insertString);
				}
				else
				{
					//si fa l'update solo se il tipo documento vecchio è grigio e quello nuovo è diverso da grigio
					if(dataSet.Tables["DOC"].Rows[0]["CHA_TIPO_DOC"].ToString().Equals("G") && !tipoProto.Equals("G"))
					{
					    string updateString="UPDATE DPA_AREA_LAVORO SET CHA_TIPO_DOC="+tipoProto+" WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROFILE="+idProfile;
					    db.executeNonQuery(updateString);
					}
				}
			}
			if(fasc!=null)
			{
				string idProject=fasc.systemID;
				string tipoFasc=fasc.tipo;
				string queryFasc="SELECT SYSTEM_ID FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo+" AND ID_PROJECT="+idProject;
				db.fillTable(queryFasc,dataSet,"FASC");
				if(dataSet.Tables["FASC"].Rows.Count==0)
				{
					//si inserisce il nuovo dato
					insertString="INSERT INTO DPA_AREA_LAVORO (SYSTEM_ID,ID_PEOPLE,ID_RUOLO_IN_UO,ID_PROJECT,CHA_TIPO_FASC,DTA_INS)";
					insertString=insertString+" VALUES ('1','"+idPeople+"','"+idRuoloInUo+"','"+idProject+"','"+tipoFasc+"',"+dateString+")";
					db.executeNonQuery(insertString);
				}
			}
			db.closeConnection();
		}
		catch(Exception e)
		{
			db.closeConnection();
			throw e;
		}*/
            #endregion
        }

    public static void cancellaAreaLavoro(string idPeople, string idRuoloInUo, string idProfile, DocsPaVO.fascicolazione.Fascicolo fasc)
    {
        logger.Debug("cancellaAreaLavoro");
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
        /*attenzione: aggiunto controllo su fasc.systemID, in alcuni casi è null quindi si va in 
         * errore */
        /*if (!doc.DeleteAreaLavoro(fasc.systemID, infoDoc, infoUtente))
        {
            throw new Exception();
        }*/
        bool result;

        if (fasc == null)
        {
            result = doc.DeleteAreaLavoro(idPeople, idRuoloInUo, null, idProfile);
        }
        else
        {
            result = doc.DeleteAreaLavoro(idPeople, idRuoloInUo, fasc.systemID, idProfile);
        }
        if (!result)
        {
            //TODO : gestire la throw
            throw new Exception();
        }

        #region codice originale
        /*DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
        bool dbOpen=false;
        try
        {
            db.openConnection();
            dbOpen=true;
            //costruzione della query
            string idPeople=infoUtente.idPeople;
            string idRuoloInUo=infoUtente.idCorrGlobali;
            string deleteString="DELETE FROM DPA_AREA_LAVORO WHERE ID_PEOPLE="+idPeople+" AND ID_RUOLO_IN_UO="+idRuoloInUo;
            if(infoDoc!=null)
            {
               deleteString=deleteString+" AND ID_PROFILE="+infoDoc.idProfile;
            }
            else{
               deleteString=deleteString+" AND ID_PROJECT="+fasc.systemID;
            }
            db.executeNonQuery(deleteString);
            db.closeConnection();
        }
        catch(Exception e){
            if(dbOpen){
               db.closeConnection();
            }
            throw e;
        }*/
        #endregion
    }

}
