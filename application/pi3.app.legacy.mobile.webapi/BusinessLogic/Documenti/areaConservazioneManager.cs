// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ricerche;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Documenti;

public class areaConservazioneManager
{
    public static List<SearchResultInfo> getListaDocumentiByIdProject(string idProject)
    {
        DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        List<SearchResultInfo> tempList = fascicoli.getIdDocFasc(idProject);
        return tempList;
    }

}
