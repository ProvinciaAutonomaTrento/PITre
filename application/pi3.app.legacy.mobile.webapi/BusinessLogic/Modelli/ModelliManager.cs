// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Modelli
{
    public class ModelliManager
    {
        public static DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor modelProcessor = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

            return modelProcessor.GetModelProcessors();
        }

        public static byte[] GetFileFromPath(string path)
        {

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] retValue = new byte[stream.Length];
            var read = stream.Read(retValue, 0, retValue.Length);
            stream.Flush();
            stream.Close();
            stream = null;
            return retValue;


        }

    }
}
