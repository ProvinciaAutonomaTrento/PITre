using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestProject.DocsPaWS;
using System.IO;

namespace TestProject.Repertori
{
    [TestClass]
    public class Repertori
    {
        private DocsPaWebService _docsPaWS = new DocsPaWebService();
        private Ruolo _role;
        private Utente _user;

        [TestInitialize()]
        public void StartUp()
        {
            // Impostazione timeout ws
            this._docsPaWS.Timeout = System.Threading.Timeout.Infinite;

            // Recupero settings dell'istanza del reprrtorio 898 per l'rf 1186694
            //RegistroRepertorioSettingsResponse response = this._docsPaWS.GetRegisterSettings(
            //    new RegistroRepertorioSettingsRequest() 
            //    { 
            //        CounterId = "898",
            //        RfId = "1186694", 
            //        SettingsType = SettingsType.G, 
            //        TipologyKind = TipologyKind.D 
            //    });

            //// Recupero dettagli ruolo e utente stampatore
            //this._role = this._docsPaWS.getRuoloByIdGruppo(response.RegistroRepertorioSingleSettings.PrinterRoleRespId);
            //this._user = this._docsPaWS.getUtenteById(response.RegistroRepertorioSingleSettings.PrinterUserRespId);
            
        }

        // Generazione report di stampa
        [TestMethod]
        public void GeneraFileReportTest()
        {
            // Generazione filtri per la stampa
            FiltroRicerca[] filters = new FiltroRicerca[]
            {
                new FiltroRicerca() { argomento = "idCounter", valore = "898" },
                new FiltroRicerca() { argomento ="idRegistry", valore = String.Empty },
                new FiltroRicerca() { argomento = "idRf", valore = "1186694" }
            };

            PrintReportResponse response = this._docsPaWS.GenerateReport(
                    new PrintReportRequest()
                    {
                        UserInfo = new InfoUtente() { userId = this._user.userId, idAmministrazione = this._user.idAmministrazione, idPeople = this._user.idPeople, idGruppo = this._role.systemId, idCorrGlobali = this._user.systemId },
                        SearchFilters = filters,
                        ReportType = ReportTypeEnum.PDF,
                        ReportKey = "StampaRegistriRepertori",
                        ContextName = "StampaRegistriRepertori"
                    });

            File.WriteAllBytes(@"c:\Test\stampa.pdf", response.Document.content);
            

        }

        [TestMethod]
        public void GeneraDocumentoStampaTest()
        {
            this._docsPaWS.GeneratePrintRepertorio(
                new GeneratePrintRepertorioRequest() 
                { 
                    CounterId = "898", 
                    RegistryId = String.Empty, 
                    RfId = "1186694", 
                    Role = this._role,
                    UserInfo = new InfoUtente() { userId = this._user.userId, idAmministrazione = this._user.idAmministrazione, idPeople = this._user.idPeople, idGruppo = this._role.idGruppo, idCorrGlobali = this._user.systemId }
                });

        }

        [TestMethod]
        public void CambioStatoRepertorio()
        {
            bool result = this._docsPaWS.ChangeRepertorioState(
                new ChangeRepertorioStateRequest() 
                { 
                    CounterId = "898", 
                    IdAmm = "361", 
                    RegistryId = String.Empty, 
                    RfId = "1186694" 
                }).ChangeStateResult;

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetListaRepertoriPerStampaAutomatica()
        {
            GetRegistersToPrintAutomaticServiceResponse response = this._docsPaWS.GetRegistersToPrint();

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.RegistersToPrint);
            Assert.IsTrue(response.RegistersToPrint.Length > 0);
        }
    }
}
