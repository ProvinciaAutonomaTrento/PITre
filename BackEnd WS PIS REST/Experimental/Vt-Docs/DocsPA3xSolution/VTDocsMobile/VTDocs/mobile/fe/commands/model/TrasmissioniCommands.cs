using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.commands.model;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.model;
using VTDocs.mobile.fe.commands;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class VistoTrasmCommand : Command<MainModel>
    {
        private string _idTrasm;
        private string _idDoc;

        public VistoTrasmCommand(string idTrasm)
        {
            this._idTrasm = idTrasm;
        }

        public override MainModel Execute()
        {
            AccettaRifiutaTrasmRequest request = new AccettaRifiutaTrasmRequest();
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdTrasmissione = _idTrasm;
            request.UserInfo = NavigationHandler.CurrentUser;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            AccettaRifiutaTrasmResponse resp = WSStub.setDataVistaSP_TV(request);
            if (resp.Code == AccettaRifiutaTrasmResponseCode.OK)
            {
                NavigationHandler.ToDoListMemento = null;
                return new ToDoListCommand().Execute();
            }
            else if (resp.Code == AccettaRifiutaTrasmResponseCode.BL_ERROR)
            {
                //NavigationHandler.ToDoListMemento = null;
                MainModel model = NavigationHandler.Model;
                model.Errori = new List<string> { resp.Errore };
                return model;
            }
            else
            {
                MainModel model = NavigationHandler.Model;
                addSystemError(model);
                return model;
            }
        }

        private void addSystemError(MainModel input)
        {
            input.Errori = new List<string> { Resources.Errors.Common_SystemError };
        }
    }

    public class AccettaRifiutaTrasmCommand : Command<MainModel>
    {
        private string _idTrasm;
        private string _idTrasmUtente;
        private string _note;
        private string _tipoProto;
        private string _idDoc;
        private bool _putInAdl;
        private AccettaRifiutaAction _action;

        public AccettaRifiutaTrasmCommand(string idTrasm, string idTrasmUtente, string note, AccettaRifiutaAction action)
        {
            this._idTrasm = idTrasm;
            this._idTrasmUtente = idTrasmUtente;
            this._note = note;
            this._action = action;
        }

        public AccettaRifiutaTrasmCommand(string idTrasm, string idTrasmUtente, string note, AccettaRifiutaAction action,string idDoc,string tipoProto)
        {
            this._idTrasm = idTrasm;
            this._idTrasmUtente = idTrasmUtente;
            this._note = note;
            this._action = action;
            this._idDoc = idDoc;
            this._tipoProto = tipoProto;
            this._putInAdl = true;
        }

        public override MainModel Execute()
        {
            AccettaRifiutaTrasmRequest request = new AccettaRifiutaTrasmRequest();
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdTrasmissione = _idTrasm;
            request.IdTrasmissioneUtente = _idTrasmUtente;
            request.UserInfo = NavigationHandler.CurrentUser;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            request.Action = _action;
            request.Note = _note;
            AccettaRifiutaTrasmResponse resp = WSStub.accettaRifiutaTrasm(request);
            if (resp.Code == AccettaRifiutaTrasmResponseCode.OK)
            {
                if (_putInAdl)
                {
                    ADLActionRequest adlRequest = new ADLActionRequest();
                    adlRequest.DocInfo = new DocInfo { IdDoc = _idDoc, TipoProto = _tipoProto };
                    adlRequest.UserInfo = request.UserInfo;
                    adlRequest.IdGruppo = request.IdGruppo;
                    adlRequest.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                    adlRequest.AdlAction = ADLActionRequest.ADLActions.ADD;
                    ADLActionResponse adlResp = WSStub.ADLAction(adlRequest);
                    if (adlResp.Code != AddToADLResponseCode.OK)
                    {
                        MainModel model = NavigationHandler.Model;
                        addSystemError(model);
                        return model;
                    }
                }

                NavigationHandler.ToDoListMemento = null;
                return new ToDoListCommand().Execute();
            }
            else if (resp.Code == AccettaRifiutaTrasmResponseCode.BL_ERROR)
            {
                //NavigationHandler.ToDoListMemento = null;
                MainModel model = NavigationHandler.Model;
                model.Errori = new List<string> { resp.Errore };
                return model;
            }else{
                MainModel model = NavigationHandler.Model;
                addSystemError(model);
                return model;
            }
        }

        private void addSystemError(MainModel input)
        {
            input.Errori = new List<string> { Resources.Errors.Common_SystemError };
        }
    }

    public class TrasmissioneFormCommand : MainModelCommand
    {
        private string _idDocFasc;
        private bool _isFascicolo;
        private string _idTrasmPerAcc;
        private ILog logger = LogManager.GetLogger(typeof(TrasmissioneFormCommand));

        public TrasmissioneFormCommand(string idDocFasc,bool isFascicolo,string idTrasmPerAcc)
        {
            this._idDocFasc = idDocFasc;
            this._isFascicolo = isFascicolo;
            this._idTrasmPerAcc = idTrasmPerAcc;
        }

        protected override void ExecuteParticular(MainModel model)
        {
            
            model.TabShow = Tab.TRASMISSIONE;
            ListaModelliTrasmRequest req = new ListaModelliTrasmRequest();
            req.UserInfo = NavigationHandler.CurrentUser;
            
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                req.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            req.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            if (_isFascicolo) req.TrasmFasc = true;
            ListaModelliTrasmResponse resp = WSStub.getListaModelliTrasm(req);
            if (resp.Code == ListaModelliTrasmResponseCode.OK)
            {
                TrasmissioneModel tabModel = new TrasmissioneModel();
                tabModel.IdTrasmPerAcc = this._idTrasmPerAcc;
                if (typeof(DettaglioDFTrasmModel) == NavigationHandler.Model.TabModel.GetType())
                {
                    logger.Debug("ricavo info fasc/doc da dettaglio");
                    DettaglioDFTrasmModel dett = (DettaglioDFTrasmModel)NavigationHandler.Model.TabModel;
                    tabModel.DocInfo = dett.DocInfo;
                    tabModel.FascInfo = dett.FascInfo;
                }
                else
                {
                    if (_isFascicolo)
                    {
                        logger.Debug("ricavo info fasc");
                        GetFascInfoRequest request = new GetFascInfoRequest();
                        request.UserInfo = NavigationHandler.CurrentUser;
                        if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                            request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                        request.IdFasc = _idDocFasc;
                        request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                        request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                        GetFascInfoResponse response = WSStub.getFascInfo(request);
                        tabModel.FascInfo = response.FascInfo;
                    }
                    else
                    {
                        logger.Debug("ricavo info doc");
                        GetDocInfoRequest request = new GetDocInfoRequest();
                        request.UserInfo = NavigationHandler.CurrentUser;
                        if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                            request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                        request.IdDoc = _idDocFasc;
                        request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                        request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                        GetDocInfoResponse response = WSStub.getDocInfo(request);
                        tabModel.DocInfo = response.DocInfo;

                    }

                }

                //sto trasemttendo dopo l' accettazione imposto cantrasmit a true
                if (!String.IsNullOrEmpty(this._idTrasmPerAcc))
                {
                    if (tabModel.DocInfo!=null)
                        tabModel.DocInfo.CanTransmit = true;

                    if (tabModel.FascInfo != null)
                        tabModel.FascInfo.CanTransmit = true;
                }

                tabModel.ModelliTrasm = resp.Modelli;
                model.TabModel = tabModel;
            }
            else
            {
                addSystemError(model);
            }
        }
    }

    public class EseguiTrasmCommand : Command<OperationResponse>
    {
        private string _idTrasmModel;
        private string _note;
        private string _id;
        private bool _isDoc;
        
        public EseguiTrasmCommand(string idTrasmModel, string note,string id,bool isDoc)
        {
            this._idTrasmModel = idTrasmModel;
            this._note = note;
            this._id = id;
            this._isDoc = isDoc;
        }

        public override OperationResponse Execute()
        {
            OperationResponse res=new OperationResponse();
            try
            {
                EseguiTrasmRequest request = new EseguiTrasmRequest();
                request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                request.IdModelloTrasm = _idTrasmModel;
                if (_isDoc)
                {
                    request.IdDoc = _id;
                }
                else
                {
                    request.IdFasc = _id;
                }
                request.UserInfo = NavigationHandler.CurrentUser;

                if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                    request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                request.Note = _note;
                request.Path = getPath();
                EseguiTrasmResponse response = WSStub.eseguiTrasm(request);
                if (response.Code == EseguiTrasmResponseCode.OK)
                {
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Error = Resources.Errors.Common_SystemError;
                }
                //NavigationHandler.ToDoListMemento = null;
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Error = Resources.Errors.Common_SystemError;
            }
            return res;
        }

        private string getPath()
        {
            HttpRequest request = HttpContext.Current.Request;
            string path = request.Url.Scheme + "://" + request.Url.Host;
            if (!request.Url.Port.Equals(80))
                path += ":" + request.Url.Port;
            path += HttpContext.Current.Request.ApplicationPath;
            return path;
        }
    }
}