using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using DocsPaDocumentale.Interfaces;
using DocsPaVO.amministrazione;
using DocsPaVO.utente;

namespace DocsPaDocumentale_FILENET.Documentale
{
    /// <summary>
    /// Gestione dell'amministrazione nel documentale
    /// </summary>
    public class AmministrazioneManager : IAmministrazioneManager
    {
        #region Ctors, constants, variables

        /// <summary>
        /// 
        /// </summary>
        private InfoUtenteAmministratore _infoUtente = null;

        /// <summary>
        /// 
        /// </summary>
        private IAmministrazioneManager _instanceETDOCS = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        public AmministrazioneManager(InfoUtenteAmministratore infoUtente)
        {
            this._infoUtente = infoUtente;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Inserimento di una nuova amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Insert(InfoAmministrazione info)
        {
            info.LibreriaDB = this.GetDatabaseInfo(ConfigurationManager.AppSettings["connectionString"].ToUpper());

            return this.InstanceETDOCS.Insert(info);
        }

        /// <summary>
        /// Aggiornamento di un'amministrazione esistente nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Update(InfoAmministrazione info)
        {
            return this.InstanceETDOCS.Update(info);
        }

        /// <summary>
        /// Cancellazione di un'amministrazione nel documentale
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public EsitoOperazione Delete(InfoAmministrazione info)
        {
            return this.InstanceETDOCS.Delete(info);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected InfoUtenteAmministratore InfoUtente
        {
            get
            {
                return this._infoUtente;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IAmministrazioneManager InstanceETDOCS
        {
            get
            {
                if (this._instanceETDOCS == null)
                    this._instanceETDOCS = new DocsPaDocumentale_ETDOCS.Documentale.AmministrazioneManager(this.InfoUtente);
                return this._instanceETDOCS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected virtual string GetDatabaseInfo(string connectionString)
        {
            //"server=ets1363; uid=docsadm; pwd=docsadm; database=geddoc54;"
            string pattern = @"=(?<server>.+); uid=(?<uid>.+); pwd=(?<pwd>.+); database=(?<databaseName>\w+);";
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            Match match = reg.Match(connectionString);
            if (match.Success)
            {
                string server = match.Groups["server"].ToString();
                string database = match.Groups["databaseName"].ToString();
                return database + "^" + server;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
