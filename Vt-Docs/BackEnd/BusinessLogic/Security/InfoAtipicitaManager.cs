using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Security
{
    public class InfoAtipicitaManager
    {
        public static DocsPaVO.Security.InfoAtipicita GetInfoAtipicita(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico tipoOggetto, string idDocOrFasc)
        {
            DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
            return documentale.GetInfoAtipicita(infoUtente, tipoOggetto, idDocOrFasc);
        }
    }
}
