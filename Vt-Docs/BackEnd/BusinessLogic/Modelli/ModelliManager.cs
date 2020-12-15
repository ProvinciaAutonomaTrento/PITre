using DocsPaDB;
using System;
using System.Data;
using System.Web;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.ComponentModel;        
using System.Reflection;
using System.Net.Sockets;
using log4net;

namespace BusinessLogic.Modelli
{	
	/// <summary>
	/// Classe per la gestione dei dati dei modelli
	/// </summary>
	public class ModelliManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(ModelliManager));
        /// <summary>
        /// Reperimento dei metadati relativi a tutti i word processor clientside disponibili nel sistema
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor modelProcessor = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

            return modelProcessor.GetModelProcessors();
        }

        /// <summary>
        /// Reperimento dei metadati relativi al word processor clientside correntemente impostato per l'utente
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.Modelli.ModelProcessorInfo GetCurrentModelProcessor(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor modelProcessor = new DocsPaDB.Query_DocsPAWS.ClientSideModelProcessor();

            DocsPaVO.Modelli.ModelProcessorInfo processor = modelProcessor.GetModelProcessorForUser(infoUtente.idPeople);
            
            if (processor == null)
                // Se per l'utente non risulta impostato alcun word processor,
                // viene reperito quello definito per l'amministrazione
                processor = modelProcessor.GetModelProcessorForAdmin(infoUtente.idAmministrazione);

            return processor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId_template"></param>
        /// <param name="privato"></param>
        public static void UpdatePrivatoTipoDoc(int systemId_template, string privato)
        {
            BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.UpdatePrivatoTipoDoc(systemId_template, privato);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemId_template"></param>
        /// <param name="privato"></param>
        public static void UpdatePrivatoTipoFasc(int systemId_template, string privato)
        {
            BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.UpdatePrivatoTipoFasc(systemId_template, privato);
        }

        public static byte[] GetFileFromPath(string path)
        {

            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] retValue = new byte[stream.Length];
            stream.Read(retValue, 0, retValue.Length);
            stream.Flush();
            stream.Close();
            stream = null;
            return retValue;


        }
	}
}
