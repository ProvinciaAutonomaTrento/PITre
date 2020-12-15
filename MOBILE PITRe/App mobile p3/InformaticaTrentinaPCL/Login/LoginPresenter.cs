using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.MVP;
using InformaticaTrentinaPCL.Login.MVPD;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;
using InformaticaTrentinaPCL.VerifyUpdate;

namespace InformaticaTrentinaPCL.Login
{
    public class LoginPresenter : ILoginPresenter
    {
        protected string loginResponse_DEFAULT_LOGIN = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':0,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        protected string loginResponse_EXPIRED_LOGIN = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':2,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        protected string loginResponseOK = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':0,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        protected string loginResponseKO_ShowListAdministration = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':3,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        protected string loginResponse_multiAmm = "{ 'UserInfo': null,'OTPAllowed': false,'ShareAllowed': false,'Code': 3,'ErrorMessageLogin': 'MultiAmministrazione','InfoMemento': null}";
        //protected ILoginView view;
        protected ILoginView view;

        private ILoginViewChooseInstance viewChooseInstance;
        protected ILoginModel model;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected IVersionManager versionManager;
        protected IReachability reachability;
        protected LoginObject loginObject;
        
        public LoginPresenter(ILoginView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.versionManager = nativeFactory.GetVersionManager();
            this.loginObject = new LoginObject();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            this.model = new DummyLoginModel(TypeLogin.DEFAULT_LOGIN, loginResponse_DEFAULT_LOGIN);
#else
            this.model = new LoginModel();
#endif
        }

        public LoginPresenter(ILoginView view, INativeFactory nativeFactory, ILoginViewChooseInstance viewChooseInstance)
        {
            this.view = view;
            this.viewChooseInstance = viewChooseInstance;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.versionManager = nativeFactory.GetVersionManager();
            this.loginObject = new LoginObject();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            this.model = new DummyLoginModel(TypeLogin.DEFAULT_LOGIN, loginResponse_DEFAULT_LOGIN);
#else
            this.model = new LoginModel();
#endif
        }

        public async void VerifyUpdate(string brand="" , string model ="")
        {
            VerifyUpdateModel vfModel = new VerifyUpdateModel();
            view.OnUpdateLoader(true);
            VerifyUpdateRequestModel request = new VerifyUpdateRequestModel(new VerifyUpdateRequestModel.Body(GetAppVersion(),model,brand));
            VerifyUpdateResponseModel response = await vfModel.GetDoVerifyUpdate(request);

            view.OnUpdateLoader(false);
           this.ManageUpdateResponse(response);
        }
        
        #region IImplementation
        public async void LoginAsync(bool saveLogin)
        {
            view.OnUpdateLoader(true);
            LoginRequestModel request = new LoginRequestModel(new LoginRequestModel.Body(loginObject.username, loginObject.password, loginObject.oldPassword, loginObject.administration));
			LoginResponseModel response = await model.GetDoLogin(request);

            view.OnUpdateLoader(false);
            this.ManageLoginResponse(response);
        }

        public void OnViewReady()
        {
            this.CheckCredentials();

        }

        public void UpdateUsername(string username)
        {
            loginObject.username = username;
            this.CheckCredentials();
        }

        public void UpdatePassword(string pwd)
        {
            loginObject.password = pwd;
            this.CheckCredentials();
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public string GetAppVersion(){
            var version = versionManager.getAppVersion();
            return version;
        }
        #endregion

        #region ManageResponse
        public void ManageLoginResponse(LoginResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response,reachability);
            if (responseHelper.IsValidResponse())
            {
               switch (response.Code)
           
                {
                    case 0:
                        this.OnLoginOk(response.userInfo, response.todoListRemoval, response.shareAllowed);
                        break;
                    case 1:
                        view.ShowError(LocalizedString.LOGIN_ERROR_CODE_1.Get());
                        break;
                    case 2:
                        view.ShowChangePassword();
                        break;
                    case 3:
                        view.ShowListAdministration();
                        break;
                    //// invalid otp otp per recupeor password  ( reset password )
                    case 9:
                        view.ShowError(LocalizedString.INVALID_OTP.Get());
                        //    this.OnLoginOk(response.userInfo, response.todoListRemoval, response.shareAllowed);
                        break;
                    case 10:
                        view.ShowError(LocalizedString.PASSWORD_EQUALITY.Get());
                        //    this.OnLoginOk(response.userInfo, response.todoListRemoval, response.shareAllowed);
                        break;
                    case 11:
                        view.ShowError(LocalizedString.DOMAIN_AUTH_ENABLED.Get());
                        //    this.OnLoginOk(response.userInfo, response.todoListRemoval, response.shareAllowed);
                        break;

                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

       

        public void ManageUpdateResponse(VerifyUpdateResponseModel response)
        {
            try
            {
                ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
                if (responseHelper.IsValidResponse() && !String.IsNullOrEmpty(response.url))
                {
                    view.ShowUpdatePopup(response.url);
                }
                // else LoginAsync(false);
            }
            catch(Exception e)
            {

            }

        }

        #endregion



        #region ViewCallback
        private void OnLoginOk(UserInfo userLogged, string todoListRemoval, bool shareAllowed)
        {
            UserInfo user ;

            //test
            // string sruoli = user.ruoli.ToString();
            // string sruoliFiltered = user.ruoliFiltered.ToString();
            user = userLogged;
           // user.delegato = userLogged;

          //  user = new UserInfo();
            //user.idAmministrazione = "361";
            //user.idPeople = "93119";

            //user = new UserInfo();
            //user.idAmministrazione = "361";
            //user.idPeople = "93119";
            //ChangeRole.RuoloInfo r0 = new ChangeRole.RuoloInfo("76458734", "U257SEG", "Segreteria Ufficio Informatica");
            //ChangeRole.RuoloInfo r1 = new ChangeRole.RuoloInfo("77646293", "S172SEG", "Segreteria Servizio supporto alla Direzione generale, ICT e Semplificazione amministrativa");
            //ChangeRole.RuoloInfo r2 = new ChangeRole.RuoloInfo("139227", "GESTPAT", "Gestore prospetti statistici Provincia Autonoma di Trento");
            //ChangeRole.RuoloInfo r3 = new ChangeRole.RuoloInfo("7097186", "U257COMG", "Componente Gestionale Ufficio Informatica");
            //List<ChangeRole.RuoloInfo> ruoli = new List<ChangeRole.RuoloInfo>();
            //ruoli.Add(r0);
            //ruoli.Add(r1);
            //ruoli.Add(r2);
            //ruoli.Add(r3);
            ////List<ChangeRole.RuoloInfo> ruoliF = new List<ChangeRole.RuoloInfo>();
            ////ruoliF.Add(r1);
            ////ruoliF.Add(r2);
            ////ruoliF.Add(r3);
            //user.ruoli = ruoli;
            //// user.delegato.ruoliFiltered = ruoliF;
            //user.username = "PR40780";
            //user.descrizione = "Schonhaut Michela";

           

            //test

          //  UserInfo user = userLogged;  //test
            sessionData.SetUserInfo(user);
            sessionData.isTodoListRemovalManual = todoListRemoval == "Manual";
            sessionData.shareAllowed = shareAllowed;
            LoginEventInfo eventInfo = new LoginEventInfo();
            eventInfo.username = user.username;
            analyticsManager.LoginEvent(eventInfo);

          

            view.OnLoginOK(userLogged);
        }

      

        #endregion

        public virtual void CheckCredentials()
        {
            bool enabled = loginObject.username.Trim().Length >= 1 && loginObject.password.Trim().Length >= 1;
            view.EnableButton(enabled);
        }

        /// <summary>
        /// Opens the view choose instance.
        /// </summary>
        public void OpenViewChooseInstance()
        {
            viewChooseInstance?.OpenViewChangeInstance();
        }

        public void OpenViewRecuperaPassword()
        {
            viewChooseInstance?.OpenViewRecuperaPassword();
        }

        public void UpdateAdministration(AmministrazioneModel amministrazione, LoginAdministrationState state, bool isUserAction = true)
        {
            this.loginObject.administration = amministrazione;
            this.loginObject.labelAdministrationState = state;
            if (isUserAction && state == LoginAdministrationState.UNSELECTED)
            {
                view.ShowListAdministration();
            }
            this.CheckCredentials();
        }

        /// <summary>
        /// Metodo usato solo in modalità DEBUG o SVILUPPO
        /// </summary>
        public void ChangeServer()
        {
            NetworkConstants.isCollaudo = !NetworkConstants.isCollaudo;
            model = new LoginModel();
            view.OnServerChanged("Server changed to: "+(NetworkConstants.isCollaudo ? "COLLAUDO" : "DEBUG"));
        }

        /// <summary>
        /// Sets the URL constant.
        /// </summary>
        /// <param name="url">URL.</param>
        public void SetUrlConstant(string url)
        {
            var urlArray = url.ToCharArray();
            if (urlArray[url.Length-1]=='/')
            {
                url = url.Remove(url.Length - 1);
            }
            NetworkConstants.SetUrlPrefered(url);
            model = new LoginModel();
        }
    }

 /*   public class LoginObject
    {
        public string username = "";
        public string password = "";
        public string oldPassword = "";
        public string repeatedPassword = "";
        public AmministrazioneModel administration;
        public LoginAdministrationState labelAdministrationState = LoginAdministrationState.DEFAULT;
        public LoginObject() { }
    } */
}
