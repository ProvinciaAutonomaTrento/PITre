using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.commands.model;
using log4net;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;

namespace VTDocs.mobile.fe.commands.model
{
    public class ListaDelegheCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ListaDelegheCommand));

        public void ExecuteExternal(MainModel model)
        {
            ExecuteParticular(model);
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = Tab.LISTA_DELEGHE;
            ListaDelegheModel tabModel = new ListaDelegheModel();
            DelegheRequest request = new DelegheRequest();
            request.UserInfo = NavigationHandler.CurrentUser;

            if (NavigationHandler.DelegaEsercitata != null)
            {
                //caso delle deleghe esercitate
                logger.Info("L'utente sta esercitando una delega");
                model.DelegaEsercitata = NavigationHandler.DelegaEsercitata;
            }
            else
            {
                request.StatoDelega = StatoDelega.ATTIVA;

                //Deleghe Ricevute
                request.TipoDelega = TipoDelega.RICEVUTA;
                DelegheResponse resp = WSStub.getListaDeleghe(request);
                if (resp.Code == DelegheResponseCode.OK)
                {
                    tabModel.DelegheRicevute = resp.Elements;

                    //Deleghe Assegnate
                    request.TipoDelega = TipoDelega.ASSEGNATA;
                    resp = WSStub.getListaDeleghe(request);
                    if (resp.Code == DelegheResponseCode.OK)
                    {
                        tabModel.DelegheAssegnate = resp.Elements;

                        //Mauro 08-01-2014
                        //Deleghe IMPOSTATA
                        request.TipoDelega = TipoDelega.ASSEGNATA;
                        request.StatoDelega = StatoDelega.IMPOSTATA;
                        resp = WSStub.getListaDeleghe(request);
                        if (resp.Code == DelegheResponseCode.OK)
                        {
                          tabModel.DelegheImpostate  = resp.Elements;
                        }
                        else model.Errori = new List<string> { Resources.Errors.Common_SystemError };
                    }
                    else
                    {
                        model.Errori = new List<string> { Resources.Errors.Common_SystemError };
                    }
                }
                else
                {
                    model.Errori = new List<string> { Resources.Errors.Common_SystemError };
                }
                //tutte le deleghe: assegnate e ricevute
            }
            model.TabModel = tabModel;
            logger.Info("end");
        }
    }

    public class AccettaDelegaCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(AccettaDelegaCommand));
        private DelegaInfo _delega;
        private static Dictionary<AccettaDelegaResponseCode, string> _errors;

        static AccettaDelegaCommand()
        {
            _errors = new Dictionary<AccettaDelegaResponseCode, string>();
            _errors[AccettaDelegaResponseCode.NO_RUOLI] = Resources.Errors.Common_SystemError;
            _errors[AccettaDelegaResponseCode.SYSTEM_ERROR] = Resources.Errors.Common_SystemError;
            _errors[AccettaDelegaResponseCode.USER_ALREADY_LOGGED_IN] = Resources.Errors.Common_SystemError;

        }

        public AccettaDelegaCommand(DelegaInfo delega)
        {
            this._delega = delega;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            AccettaDelegaRequest request = new AccettaDelegaRequest();
            request.Delega = _delega;
            request.UserInfo = NavigationHandler.CurrentUser;
            request.SessionId = NavigationHandler.SessionId;
            AccettaDelegaResponse response = WSStub.accettaDelega(request);
            if (response.Code == AccettaDelegaResponseCode.OK)
            {
                NavigationHandler.DelegaEsercitata = response.DelegaAccettata;
                NavigationHandler.DeleganteInfo = response.UserInfo;
                NavigationHandler.ToDoListMemento = null;
                NavigationHandler.RicercaMemento = null;
                model.DescrUtente = NavigationHandler.CurrentUser.Descrizione;
                model.IdRuolo = NavigationHandler.RuoloInfo.Id;
                model.DescrRuolo = NavigationHandler.RuoloInfo.Descrizione;

                new ToDoListCommand().ExecuteExternal(model);
            }
            else
            {
                if (_errors.ContainsKey(response.Code))
                {
                    model.Errori = new List<string> { _errors[response.Code] };
                }
                else
                {
                    model.Errori = new List<string> { Resources.Errors.Common_SystemError };
                }
            }

        }
    }

    public class DismettiDelegaCommand: MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(DismettiDelegaCommand));
        private static Dictionary<DismettiDelegaResponseCode, string> _errors;
        private string _idDelegante;

        static DismettiDelegaCommand()
        {
            _errors = new Dictionary<DismettiDelegaResponseCode, string>();
            _errors[DismettiDelegaResponseCode.SYSTEM_ERROR] = Resources.Errors.Common_SystemError;
            _errors[DismettiDelegaResponseCode.OPERATION_FAILED] = Resources.Errors.Common_SystemError;
        }

        public DismettiDelegaCommand(string idDelegante)
        {
            this._idDelegante = idDelegante;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            DismettiDelegaRequest request = new DismettiDelegaRequest();
            request.IdDelegante = _idDelegante;
            request.UserInfo = NavigationHandler.CurrentUser;
            DismettiDelegaResponse response = WSStub.dismettiDelega(request);
            if (response.Code == DismettiDelegaResponseCode.OK)
            {
                NavigationHandler.DeleganteInfo = null;
                NavigationHandler.ToDoListMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.DelegaEsercitata = null;
                model.DescrUtente = NavigationHandler.CurrentUser.Descrizione;
                model.IdRuolo = NavigationHandler.RuoloInfo.Id;
                model.DescrRuolo = NavigationHandler.RuoloInfo.Descrizione;
                new ToDoListCommand().ExecuteExternal(model);
            }
            else
            {
                if (_errors.ContainsKey(response.Code))
                {
                    model.Errori = new List<string> { _errors[response.Code] };
                }
                else
                {
                    model.Errori = new List<string> { Resources.Errors.Common_SystemError };
                }
            }
        }
    }

    public class CreaDelegaCommand : Command<OperationResponse>
    {
        private ILog logger = LogManager.GetLogger(typeof(DismettiDelegaCommand));
        private static Dictionary<DismettiDelegaResponseCode, string> _errors;
        private Delega _delega;

        static CreaDelegaCommand()
        {
            _errors = new Dictionary<DismettiDelegaResponseCode, string>();
            _errors[DismettiDelegaResponseCode.SYSTEM_ERROR] = Resources.Errors.Common_SystemError;
            _errors[DismettiDelegaResponseCode.OPERATION_FAILED] = Resources.Errors.Common_SystemError;
        }

        public CreaDelegaCommand(string idDelegato,string idRuolo,DateTime dataInizio,DateTime dataFine)
        {
            this._delega = new Delega();
            _delega.DataDecorrenza=dataInizio;
            _delega.DataScadenza=dataFine;
            _delega.IdDelegato=idDelegato;
            _delega.IdRuoloDelegante = idRuolo;
        }

        public override OperationResponse Execute()
        {
            logger.Info("begin");
            OperationResponse res = new OperationResponse();
            try
            {
                CreaDelegaRequest request = new CreaDelegaRequest();
                request.Delega = _delega;
                request.UserInfo = NavigationHandler.CurrentUser;
                logger.Debug("chiamata ws...");
                CreaDelegaResponse response = WSStub.creaDelega(request);
                logger.Debug("chiamata eseguita");
                if (response.Code == CreaDelegaResponseCode.OK)
                {
                    logger.Debug("delega creata");
                    res.Success = true;
                }
                else if (response.Code == CreaDelegaResponseCode.NOT_CREATED)
                {
                    logger.Debug("delega non creata");
                    res.Success = false;
                    res.Error = Resources.Errors.Deleghe_DelegaNotCreated;
                }
                else if (response.Code == CreaDelegaResponseCode.OVERLAPPING_PERIODS)
                {
                    logger.Debug("delega non creata per periodi sovrapposti");
                    res.Success = false;
                    res.Error = Resources.Errors.Deleghe_OverlappingPeriods;
                }
                else if (response.Code == CreaDelegaResponseCode.SYSTEM_ERROR)
                {
                    logger.Debug("errore di sistema");
                    res.Success = false;
                    res.Error = Resources.Errors.Common_SystemError;
                }
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Error = Resources.Errors.Common_SystemError;
            }
            return res;
        }
    }

    public class CreaDelegaDaModelloCommand : Command<OperationResponse>
    {
        private ILog logger = LogManager.GetLogger(typeof(CreaDelegaDaModelloCommand));
        private string _idModelloDelega;
        private DateTime _dataInizio;
        private DateTime _dataFine;

        public CreaDelegaDaModelloCommand(string idModelloDelega,DateTime dataInizio,DateTime dataFine)
        {
            this._idModelloDelega = idModelloDelega;
            this._dataInizio = dataInizio;
            this._dataFine = dataFine;
        }

        public override OperationResponse Execute()
        {
            logger.Info("begin");
            OperationResponse res = new OperationResponse();
            CreaDelegaDaModelloRequest request = new CreaDelegaDaModelloRequest();
            request.IdModelloDelega = _idModelloDelega;
            request.DataFine = _dataFine;
            request.DataInizio = _dataInizio;
            request.UserInfo=NavigationHandler.CurrentUser;
            try
            {
                logger.Info("Chiamata al ws...");
                CreaDelegaDaModelloResponse response = WSStub.creaDelegaDaModello(request);
                logger.Info("Chiamata effettuata");
                if (response.Code == CreaDelegaDaModelloResponseCode.OK)
                {
                    res.Success = true;
                    return res;
                }
                else
                {
                    res.Success = false;
                    res.Error = Resources.Errors.Common_SystemError;
                }
            }
            catch(Exception e)
            {
                res.Success = false;
                res.Error = Resources.Errors.Common_SystemError;
            }
            return res;
        }
    }

    public class CreaDelegaFormCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(CreaDelegaFormCommand));

        protected override void ExecuteParticular(MainModel model)
        {
            model.TabShow = Tab.CREA_DELEGA;
            CreaDelegaModel tabModel = new CreaDelegaModel();
            model.TabModel = tabModel;
            ListaModelliDelegaRequest request = new ListaModelliDelegaRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            ListaModelliDelegaResponse response = WSStub.getListaModelliDelega(request);
            if (response.Code == ListaModelliDelegaResponseCode.OK)
            {
                tabModel.ModelliDelega = response.Modelli;

            }
            else
            {
                addSystemError(model);
            }
        }
    }

    public class RevocaDelegaCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(RevocaDelegaCommand));
        private List<Delega> _deleghe;

        public RevocaDelegaCommand(List<Delega> deleghe)
        {
            this._deleghe = deleghe;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            RevocaDelegheRequest request = new RevocaDelegheRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.Deleghe = _deleghe.ToArray();
            RevocaDelegheResponse response = WSStub.revocaDeleghe(request);
            logger.Debug("codice: " + response.Code + " errore: " + response.Error);
            if (response.Code == RevocaDelegheResponseCode.OK)
            {
                new ListaDelegheCommand().ExecuteExternal(model);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Error))
                {
                    model.Errori=new List<string>{response.Error};
                }
                else
                {
                    addSystemError(model);
                }
            }
        }
    }
  
}
