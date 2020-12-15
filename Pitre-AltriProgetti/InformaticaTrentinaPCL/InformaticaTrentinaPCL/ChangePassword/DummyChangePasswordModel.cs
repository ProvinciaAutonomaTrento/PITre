using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.ChangePassword.MVP;
using InformaticaTrentinaPCL.Login;
using InformaticaTrentinaPCL.Login.Network;
using Newtonsoft.Json;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.ChangePassword
{
    public class DummyChangePasswordModel : WS, IChangePasswordModel
    {
		string loginResponseOK = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':0,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        string loginResponseKO_ShowListAdministration = "{'UserInfo':{'IdAmministrazione':'14902680','Dst':'48754673ad294c4abd72e29f37996908','Ruoli':[{'Descrizione':'ClientManagerAMS-DE','IdGruppo':'14914268','Codice':'CMAN','IdUO':'14914240','Livello':'150','Registri':[{'SystemId':'14912763'}],'Id':'14914269'},{'Descrizione':'LegalStatement-DE','IdGruppo':'14946029','Codice':'REV','IdUO':'14914483','Livello':'110','Registri':[{'SystemId':'14912763'}],'Id':'14946030'}],'Token':'SSO=7jxEMbLtzW1kissDf+8uCZT0/mmVLZGlgM07pWRLhzSkEIa40RdvVVwPFpEYANkkIneib4rhBJUtaDdjFAwrqbX0cnw7wq3UbSoX9QH1gBo=','DelegatoInfo':null,'InfoUtente':{'idCorrGlobali':null,'idPeople':'14914353','userId':'MOHMEC','dst':'48754673ad294c4abd72e29f37996908','idGruppo':null,'idAmministrazione':'14902680','sede':null,'urlWA':null,'delegato':null,'extApplications':null,'codWorkingApplication':null,'matricola':null,'diSistema':null},'Utente':{'idPeople':'14914353','dst':null,'userId':null,'notifica':null,'telefono':null,'nome':'Christoph','cognome':'Mohme','dominio':null,'amministratore':false,'assegnante':false,'assegnatario':false,'sede':null,'urlWA':null,'ruoli':null,'sessionID':null,'qualifiche':null,'matricola':null,'extApplications':null,'codWorkingApplication':null,'disabilitato':null,'systemId':null,'descrizione':'MohmeChristoph','codiceCorrispondente':null,'codiceRubrica':null,'idAmministrazione':'14902680','tipoCorrispondente':null,'tipoIE':null,'idRegistro':null,'canalePref':null,'dettagli':false,'info':null,'serverPosta':null,'idOld':null,'email':null,'fromEmail':null,'notificaConAllegato':false,'errore':null,'codiceAOO':null,'codiceAmm':null,'dta_fine':null,'indirizzo':'','citta':'','cap':'','prov':'','nazionalita':'','telefono1':'','telefono2':'','fax':'','codfisc':'','partitaiva':'','note':'','inRubricaComune':false,'protocolloDestinatario':null,'codDescAmministrizazione':'','localita':'','luogoDINascita':'','dataNascita':'','titolo':'','oldDescrizione':'','disabledTrasm':false,'interoperanteRGS':false,'_urls':[],'_mailsCorr':[]},'Descrizione':'MohmeChristoph','UserId':'MOHMEC','IdPeople':'14914353'},'OTPAllowed':false,'Code':3,'ErrorMessageLogin':'','InfoMemento':{'Alias':'TEST_Alias','Dominio':'TEST_Dominio'}}";
        TypeLogin typeLogin;

        public DummyChangePasswordModel(TypeLogin typeLogin)
        {
            this.typeLogin = typeLogin;
        }

        public async Task<LoginResponseModel> GetDoLogin(LoginRequestModel request)
        {
            LoginResponseModel response = await getResponseMockedOK();

            return response;
        }

        private async Task<LoginResponseModel> getResponseMockedKO()
		{
			await Task.Delay(1000);
			LoginResponseModel response = new LoginResponseModel(3, HttpStatusCode.NotFound);

			return response;
		}

		private async Task<LoginResponseModel> getResponseMockedOK()
		{
			LoginResponseModel loginResponseModel;

			try
			{
				await Task.Delay(1000 * 2, getCancellationToken());
                //loginResponseModel = new LoginResponseModel();

                //loginResponseModel.StatusCode = HttpStatusCode.OK;
                //UserInfo mockUser = new UserInfo();
                //mockUser.username = "username";
                //mockUser.descrizione = "Paolo Rossi";
                //mockUser.idPeople = "A00001";
                //RuoloInfo ruolo = new RuoloInfo("R0001", "Code Role", "Segreteria dip. Organizzazione Personale e Affari Generali");
                //mockUser.ruoli = new List<RuoloInfo>();
                //mockUser.ruoli.Add(ruolo);
                //mockUser.URLImageUser = "https://goo.gl/hRxESQ";

                //loginResponseModel.userInfo = mockUser;

                loginResponseOK = typeLogin == TypeLogin.DEFAULT_LOGIN ? loginResponseOK : loginResponseKO_ShowListAdministration;
                loginResponseModel = JsonConvert.DeserializeObject<LoginResponseModel>(loginResponseOK);

			}
			catch (OperationCanceledException)
			{
				loginResponseModel = new LoginResponseModel();
				loginResponseModel.IsCancelled = true;
			}

			return loginResponseModel;
		}
	}
}
