// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.AmministrazioneXml
{
    public class TitolarioXml
    {
        public string filtroRicTitAmm(string codice, string descrizione, string codAmm, string idRegistro)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.filtroRicTitolarioAmm(codice, descrizione, codAmm, idRegistro);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string registriInAmm(string codAmm)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.RegistriInAmministrazione(codAmm);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string getCodLiv(string codliv, string livello, string codAmm, string idTitolario, string idRegistro)
        {
            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = obj.getCodiceLivello(codliv, livello, codAmm, idTitolario, idRegistro);
            }
            catch
            {
                result = "";
            }
            return result;
        }

    }
}
