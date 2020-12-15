using System;
using System.Collections.Generic;
using System.Text;
using DocsPaDocumentale.Interfaces;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.Validations;
using DocsPaDocumentale_DOCUMENTUM.DocsPaObjectTypes;
using DocsPaDocumentale_DOCUMENTUM.DctmServices;
using Emc.Documentum.FS.DataModel.Core.Content;
using Emc.Documentum.FS.DataModel.Core;
using Emc.Documentum.FS.Services.Core;
using Emc.Documentum.FS.DataModel.Core.Properties;
using Emc.Documentum.FS.DataModel.Core.Query;
using Emc.Documentum.FS.Runtime.Context;
using Emc.Documentum.FS.DataModel.Core.Context;
using log4net;

namespace DocsPaDocumentale_DOCUMENTUM.Documentale
{
    /// <summary>
    /// 
    /// </summary>
    public class UserManager : IUserManager
    {
        private ILog logger = LogManager.GetLogger(typeof(UserManager));
        #region Ctors, constants, variables

        /// <summary>
        /// </summary>
        public UserManager()
        {
        }

        #endregion

        #region Public methods


        /// <summary>
        /// verifica che il server DCTM risponda correttamente, effettuando un controllo sulle credenziali dell'amministratore
        /// </summary>
        /// <returns></returns>
        public virtual bool Checkconnection()
        {
            bool retValue = false;
            string userAdm = DctmConfigurations.GetDocumentumSuperUser();
            //loginResult = UserLogin.LoginResult.APPLICATION_ERROR;

            try
            {

                RepositoryIdentity identity = DctmServices.DctmRepositoryIdentityHelper.GetIdentity(
                             DctmConfigurations.GetRepositoryName(),
                             userAdm,
                             DctmConfigurations.GetDocumentumSuperUserPwd(),
                             "");

                string token = DctmServices.DctmRepositoryIdentityHelper.CreateAuthenticationToken(identity);

                IObjectService objectService = DctmServiceFactory.GetServiceInstance<IObjectService>(token);

                Qualification qual = new Qualification("dm_docbaseid_map enable(RETURN_TOP 1)");

                ObjectIdentity objectIdentity = new ObjectIdentity(qual, DctmConfigurations.GetRepositoryName());
                objectIdentity.ValueType = ObjectIdentityType.QUALIFICATION;
                objectIdentity.valueTypeSpecified = true;

                DataPackage dataPackage = objectService.Get(new ObjectIdentitySet(objectIdentity), null);

                retValue = (dataPackage != null);
            }
            /*
                        catch (Emc.Documentum.FS.Runtime.AuthenticationException exAuth)
                        {
                            //AuthenticationException - Exception in com.emc.documentum.fs.rt
                            //Exception which is raised when authentication errors occur
                            //  loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_DTCM_USER;
                            retValue = false;

                            logger.Error(string.Format("Credenziali utente DTCM non valide: '{0}'", userAdm + " " + exAuth.Message));
                        }
            */
            catch (Emc.Documentum.FS.Runtime.ServiceInvocationException exServiceInvocation)
            {
                //AuthenticationException - Exception in com.emc.documentum.fs.rt
                //Exception which is raised when authentication errors occur
                //  loginResult = DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT;
                retValue = false;

                logger.Error(string.Format("Errore nel tentativo di contattare i servizi DCTM: '{0}'", userAdm + " " + exServiceInvocation.Message));
            }
            /*
                        catch (Emc.Documentum.FS.Runtime.ServiceException exService)
                        {
                            //AuthenticationException - Exception in com.emc.documentum.fs.rt
                            //Exception which is raised when authentication errors occur
                            //  loginResult = DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT;
                            retValue = false;

                            logger.Error(string.Format("Errore nel tentativo di contattare i servizi DTCM: '{0}'", userAdm + " " + exService.Message));
                        }
            */
            catch (Exception ex)
            {
                //AuthenticationException - Exception in com.emc.documentum.fs.rt
                //Exception which is raised when authentication errors occur
                //  loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                retValue = false;

                logger.Error(string.Format("Error durante il controllo checkpage utente : '{0}'", userAdm + " errore: " + ex.Message));
            }

            return retValue;

        }

        /// <summary>
        /// Impersonate come utente superuser documentum
        /// </summary>
        /// <returns></returns>
        internal static string ImpersonateSuperUser()
        {
            // Per creare il folder per contenere i documenti, è necessario fare l'impersonate come utente amministratore (superuser in dctm)
            RepositoryIdentity superIdentity = DctmRepositoryIdentityHelper.GetIdentity(DctmConfigurations.GetRepositoryName(),
                                                     DctmConfigurations.GetDocumentumSuperUser(),
                                                     DctmConfigurations.GetDocumentumSuperUserPwd(),
                                                     string.Empty);

            return DctmRepositoryIdentityHelper.CreateAuthenticationToken(superIdentity);        
        }
        
        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="oldPassword"/></param>
        /// <param name="user"></param>
        ///// <returns></returns>
        public ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            ValidationResultInfo retValue = new ValidationResultInfo();

            try
            {
                // La password deve essere modificata con le credenziali di superuser
                IObjectService objectService = DctmServiceFactory.GetServiceInstance<IObjectService>(UserManager.ImpersonateSuperUser());
                
                ObjectIdentity identity = Dfs4DocsPa.getUserIdentityByName(TypeUtente.NormalizeUserName(user.UserName));

                DataObject userDataObject = new DataObject(identity, ObjectTypes.UTENTE);
                userDataObject.Properties.Set<string>("user_password", user.Password);

                DataPackage dataPackage = new DataPackage(userDataObject);
                dataPackage = objectService.Update(dataPackage, null);

                retValue.Value = (dataPackage.DataObjects.Count > 0);

                if (!retValue.Value)
                    throw new ApplicationException("Password non aggiornata");
                else
                {
                    RepositoryIdentity newIdentity = DctmRepositoryIdentityHelper.GetIdentity(DctmConfigurations.GetRepositoryName(), user.UserName, user.Password, string.Empty);
                    user.DST = DctmRepositoryIdentityHelper.CreateAuthenticationToken(newIdentity);

                    logger.Debug(string.Format("Documentum.ChangePassword: password modificata per l'utente {0}", user.UserName));
                }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in Documentum.ChangePassword:\n{0}", ex.ToString()));

                retValue.BrokenRules.Add(new BrokenRule("ChangePassword_ERROR", "Errore nella modifica della password per il documentale DOCUMENTUM", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error));
            }

            retValue.Value = (retValue.BrokenRules.Count == 0);

            return retValue;
        }

        /// <summary>
        /// Effettua il login di un utente amministratore
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool retValue = false;
            utente = null;
            loginResult = UserLogin.LoginResult.UNKNOWN_USER;

            DocsPaVO.utente.Utente utenteDocsPa;
            retValue = (this.LoginUser(userLogin, out utenteDocsPa, out loginResult));

            if (retValue)
            {
                loginResult = UserLogin.LoginResult.OK;

                utente = new DocsPaVO.amministrazione.InfoUtenteAmministratore();
                utente.dst = utenteDocsPa.dst;
            }

            return retValue;
        }

        /// <summary>
        /// Login al sistema documentale
        /// </summary>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool retValue = false;
            utente = null;
            loginResult = UserLogin.LoginResult.UNKNOWN_USER;

            try
            {
                string userName = TypeUtente.NormalizeUserName(userLogin.UserName);
                string userPassword = string.Empty;
                // Modifica 21-12-2012, recupero ticket documentum se login tramite token.
                //if (DocsPaServices.DocsPaQueryHelper.isUtenteDominioOrLdap(userLogin))
                if(userLogin.SSOLogin)
                    // L'utente è agganciato in amministrazione ad un dominio,
                    // pertanto viene richiamato il servizio documentum per la generazione del ticket di autenticazione
                    userPassword = DctmTokenFactoryHelper.Generate(userName);
                else
                    userPassword = userLogin.Password;

                RepositoryIdentity identity = DctmServices.DctmRepositoryIdentityHelper.GetIdentity(
                                DctmConfigurations.GetRepositoryName(),
                                userName,
                                userPassword,
                                userLogin.Dominio);

                string token = DctmServices.DctmRepositoryIdentityHelper.CreateAuthenticationToken(identity);

                // Verifica validità credenziali
                if (this.VerifyCredentials(userName, token, out  loginResult))
                {
                    utente = new Utente();
                    utente.dst = token;
                    loginResult = DocsPaVO.utente.UserLogin.LoginResult.OK;
                }

                //per LDAP oppure per SHIBBOLETH
                if (userLogin.SSOLogin)
                    utente.dst = UserManager.ImpersonateSuperUser();
                //FINE per LDAP oppure per SHIBBOLETH


                retValue = (loginResult == DocsPaVO.utente.UserLogin.LoginResult.OK);
            }
            catch (Exception ex)
            {
                retValue = false;
                utente = null;
                loginResult = UserLogin.LoginResult.UNKNOWN_USER;

                logger.Debug(string.Format("Errore in Documentum.Login:\n{0}", ex.ToString()));
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public bool LogoutUser(string dst)
        {
            return true;
        }

        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        public string GetSuperUserAuthenticationToken()
        {
            return UserManager.ImpersonateSuperUser();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Verifica credenziali utente
        /// </summary>
        /// <param name="userName"></param>
        protected virtual bool VerifyCredentials(string userName, string authenticationToken, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            bool retValue = false;
            loginResult = UserLogin.LoginResult.APPLICATION_ERROR;
            try
            {
                ObjectIdentity identity = Dfs4DocsPa.getUserHomeFolderIdentity(userName);

                IObjectService objectService = DctmServiceFactory.GetServiceInstance<IObjectService>(authenticationToken);
                logger.Debug("Inizio richiamo authenticationToken");
                DataPackage dataPackage = objectService.Get(new ObjectIdentitySet(identity), null);
                logger.Debug("Fine chiamata  authenticationToken");
                retValue = (dataPackage != null);
            }
/*
         catch (Emc.Documentum.FS.Runtime.AuthenticationException exAuth)
        {
            //AuthenticationException - Exception in com.emc.documentum.fs.rt
            //Exception which is raised when authentication errors occur
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_DTCM_USER;
            retValue = false;

            logger.Debug(string.Format("Credenziali utente DTCM non valide: '{0}'", userName));
        }
*/ 
        catch (Emc.Documentum.FS.Runtime.ServiceInvocationException exServiceInvocation)
        {
            //AuthenticationException - Exception in com.emc.documentum.fs.rt
            //Exception which is raised when authentication errors occur
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT;
            retValue = false;

            logger.Debug(string.Format("Errore nel tentativo di contattare i servizi DOCUMENTUM: '{0}'", userName));
        }
 /*
        catch (Emc.Documentum.FS.Runtime.ServiceException exService)
        {
            //AuthenticationException - Exception in com.emc.documentum.fs.rt
            //Exception which is raised when authentication errors occur
            loginResult = DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT;
            retValue = false;

            logger.Debug(string.Format("Errore nel tentativo di contattare i servizi DTCM: '{0}'", userName));
        }
 */
            catch (Exception ex)
            {
                //AuthenticationException - Exception in com.emc.documentum.fs.rt
                //Exception which is raised when authentication errors occur
                loginResult = DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER;
                retValue = false;

                logger.DebugFormat("Credenziali utente non DCTM valide: '{0}'  msg {1} stk {2}", userName,ex.Message,ex.StackTrace);
            }

            return retValue;
        }

        #endregion
    }
}