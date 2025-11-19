// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System;
using System.Collections;
using System.Data;

namespace DocsPaDB.Query_DocsPAWS;

public class FriendApplication
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(FriendApplication));

    public DocsPaVO.FriendApplication.FriendApplication getFriendApplication(string friendApplication, string codiceRegistro)
    {
        DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
        DocsPaVO.FriendApplication.FriendApplication friendApplicationResutl = null;

        try
        {
            DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("GET_FRIEND_APPLICATION");
            queryMng.setParam("friendApplication", friendApplication);
            queryMng.setParam("codRegistro", codiceRegistro);
            string commandText = queryMng.getSQL();
            System.Diagnostics.Debug.WriteLine("SQL - getFriendApplication - FriendApplication.cs - QUERY : " + commandText);
            logger.Error("SQL - getFriendApplication - FriendApplication.cs - QUERY : " + commandText);
            DataSet ds = new DataSet();
            dbProvider.ExecuteQuery(ds, commandText);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                friendApplicationResutl = new DocsPaVO.FriendApplication.FriendApplication();
                friendApplicationResutl.codiceApplicazione = ds.Tables[0].Rows[i]["COD_APPLICAZIONE"].ToString();
                friendApplicationResutl.codiceRegistro = ds.Tables[0].Rows[i]["COD_REGISTRO"].ToString();
                friendApplicationResutl.idPeopleFactory = ds.Tables[0].Rows[i]["ID_PEOPLE_FACTORY"].ToString();
                friendApplicationResutl.idGruppoFactory = ds.Tables[0].Rows[i]["ID_GRUPPO_FACTORY"].ToString();
                friendApplicationResutl.idRegistro = ds.Tables[0].Rows[i]["ID_REGISTRO"].ToString();

                Utenti utenti = new Utenti();
                friendApplicationResutl.utente = utenti.getUtenteById(friendApplicationResutl.idPeopleFactory);
                friendApplicationResutl.utente.idPeople = friendApplicationResutl.idPeopleFactory;

                friendApplicationResutl.ruolo = utenti.GetRuoloByIdGruppo(friendApplicationResutl.idGruppoFactory);
                utenti.GetRegistro(friendApplicationResutl.idRegistro, ref friendApplicationResutl.registro);
                friendApplicationResutl.utente.ruoli = new ArrayList();
                friendApplicationResutl.utente.ruoli.Add(friendApplicationResutl.ruolo);
            }
        }
        catch(Exception ex)
        {
            logger.Error("Errore in FriendApplication  - metodo: getFriendApplication", ex);
            throw ex;
        }
        finally
        {
            dbProvider.Dispose();
        }
        return friendApplicationResutl;
    }
}
