using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaDB;
using DocsPaVO.areaConservazione;
using DocsPaVO.utente;


using log4net;

namespace DocsPaConservazione
{
    public class RegistroConservazione
    {
    


      /// <summary>
        /// metodo per inserimento nel registro di conservazione. 
        /// 
        /// </summary>
        /// <param name="registroCons"> ogetto con i dati del registro </param>
        
        /// <returns></returns>
        public bool inserimentoInRegistroCons(DocsPaVO.Conservazione.RegistroCons registroCons, InfoUtente infoUt)
        {         
            try
            {
                // if (!(infoUt.extApplications != null && infoUt.extApplications.Count > 0 && ((DocsPaVO.utente.ExtApplication)infoUt.extApplications[0]).codice.Equals("CS")))
                if (!(infoUt.codWorkingApplication != null && infoUt.codWorkingApplication.Equals("CS")))
                return false;

                    DocsPaDB.Query_DocsPAWS.Conservazione cons = new DocsPaDB.Query_DocsPAWS.Conservazione();
                    string rigaIns = cons.InsertInRegistroCons(registroCons);


                    if (String.IsNullOrEmpty(rigaIns))
                        return false;

            }

            catch (Exception exc)
            {
                string err = exc.Message;
                //logger.Debug(err);
                return false;
            }
            return true;
        }

    }
}
