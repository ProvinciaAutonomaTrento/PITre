using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.DocsPaWS;

namespace TestProject
{
    [TestClass]
    public class LoginAndImpersonate
    {
        DocsPaWebService ws;

        [TestInitialize]
        public void InitializeTest()
        {
            this.ws = new DocsPaWebService();
            this.ws.Timeout = System.Threading.Timeout.Infinite;
        }

        [TestCleanup]
        public void DestroyAll()
        {
            this.ws = null;
        }
        public void LoginAndImpersonateTest()
        {
            InfoUtente userInfo = this.ExecuteLoginAndImpersonate();
            
            // Asserzioni
            // 1. Lo userInfo è valorizzato
            Assert.IsNotNull(userInfo);

            // 2. Lo user info ha username impostata a "PR34019"
            Assert.IsTrue(userInfo.userId.ToUpper().Equals("PR34019"));

        }
        public void LoginImpersonateAndCreateDocumentTest()
        {
            InfoUtente user = this.ExecuteLoginAndImpersonate();

            SchedaDocumento documento = this.ws.NewSchedaDocumento(user);
            documento.oggetto = new Oggetto() { descrizione = "Documento di test" };

            Ruolo ruolo = this.ws.getRuoloByIdGruppo(user.idGruppo);

            Registro registro = this.ws.GetRegistroBySistemId("86107");
            documento.registro = registro;

            documento = this.ws.DocumentoAddDocGrigia(documento, user, ruolo);

            Assert.IsFalse(String.IsNullOrEmpty(documento.systemId));
 
        }

        private InfoUtente ExecuteLoginAndImpersonate()
        {
            // Creazione oggetto UserLogin con le informazioni sull'utente PR28911
            UserLogin userToLog = new UserLogin();
            userToLog.UserName = "PR28911";
            userToLog.Password = "pwd";

            // Login e impersonate con PR34019
            InfoUtente userInfo = this.ws.LoginAndImpersonate(userToLog, "PR34019");

            return userInfo;
 
        }
    }
}
