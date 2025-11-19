// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System;
using System.Collections;

namespace DocsPaDB.Query_DocsPAWS.Formazione;

public class UnitaOrganizzativa : DBProvider
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(UnitaOrganizzativa));

    /// <summary>
    ///
    /// </summary>
    /// <param name="idUo"></param>
    /// <returns></returns>
    public bool PulisciUnitaOrganizzativa(string idUo, DocsPaVO.utente.InfoUtente infoUtente)
    {
        bool result = true;
        int retValue = 0;
        try
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                ArrayList sp_params = new ArrayList();
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("idUo", Int32.Parse(idUo)));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32));

                retValue = dbProvider.ExecuteStoredProcedure("SP_PULISCI_UO", sp_params, null);
                if(retValue != 1)
                {
                    result = false;
                }
            }
        }
        catch (Exception e)
        {
            result = false;
            logger.Error("Errore in PulisciUnitaOrganizzativa: " + e.Message);
        }

        return result;
    }

}
