using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class AdlCommand : MainModelCommand
    {
        protected List<IdName> _path = new List<IdName>();
        protected int _currentPage = 1;
        protected string _idRicSalvata;
        protected string _testo;
        protected bool _doRicerca;
        protected RicercaType _tipoRicerca;
        protected RicercaSalvataType _tipoRicSalvata;
        private ILog logger = LogManager.GetLogger(typeof(RicercaCommand));

        public AdlCommand()
        {
            AdlMemento memento = NavigationHandler.AdlMemento;
            if (memento != null)
            {
                this._currentPage = memento.CurrentPage;
                this._idRicSalvata = memento.IdRicSalvata;
                this._path = memento.Path;
                this._tipoRicSalvata = memento.TipoRicSalvata;
                this._testo = memento.Testo;
                this._tipoRicerca = memento.TipoRicerca;
                this._doRicerca = memento.DoRicerca;
            }
        }

        public AdlMemento Memento
        {
            get
            {
                return new AdlMemento(_path, _currentPage, _idRicSalvata, _tipoRicSalvata,_testo,_tipoRicerca,_doRicerca);
            }
        }

        private string IdParent
        {
            get
            {
                if (_path.Count > 0)
                {
                    return _path[_path.Count - 1].Id;
                }
                else
                {
                    return null;
                }
            }
        }

        private string IdFasc
        {
            get
            {
                if (_path.Count > 0)
                {
                    return _path[0].Id;
                }
                else
                {
                    return null;
                }
            }
        }

        private string NomeParent
        {
            get
            {
                if (_path.Count > 0)
                {
                    return _path[_path.Count - 1].Name;
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = Tab.AREA_DI_LAVORO;
            AdlModel adlModel = new AdlModel(0, ConfigurationHandler.NumResultsForPage);
            if (!_doRicerca)
            {
                _doRicerca = true;
                _tipoRicerca = RicercaType.RIC_DOCUMENTO_ADL;
                _testo = "";
            }

            if (_doRicerca){
                RicercaRequest ricRequest = new RicercaRequest();
                
                //Normalizzo per ADL
                if (_tipoRicerca == RicercaType.RIC_DOCUMENTO) _tipoRicerca = RicercaType.RIC_DOCUMENTO_ADL;
                if (_tipoRicerca == RicercaType.RIC_FASCICOLO) _tipoRicerca = RicercaType.RIC_FASCICOLO_ADL; 

                ricRequest.UserInfo = NavigationHandler.CurrentUser;
                if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                    ricRequest.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                ricRequest.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                ricRequest.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                ricRequest.IdRicercaSalvata = _idRicSalvata;
                ricRequest.TypeRicercaSalvata = _tipoRicSalvata;
                ricRequest.Text = _testo;
                ricRequest.TypeRicerca = _tipoRicerca;
                ricRequest.EnableProfilazione = ConfigurationHandler.RicercaEnableProfilazione;
                ricRequest.EnableUfficioRef = ConfigurationHandler.RicercaEnableUfficioRef;
                ricRequest.RequestedPage = _currentPage;
                ricRequest.ParentFolderId = IdParent;
                ricRequest.FascId = IdFasc;
                ricRequest.PageSize = ConfigurationHandler.NumResultsForPage;
                RicercaResponse response = WSStub.ricerca(ricRequest);
                adlModel = new AdlModel(response.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                adlModel.Risultati = response.Risultati;
                adlModel.IdParent = IdParent;
                adlModel.NomeParent = NomeParent;
                adlModel.Testo = _testo;
                adlModel.TypeRicerca = _tipoRicerca;
                if (response.TotalRecordCount == 0)
                {
                    adlModel.CurrentPage = 0;
                }
                else
                {
                    adlModel.CurrentPage = _currentPage;
                }
                adlModel.NumElements = response.TotalRecordCount;
            }
            GetRicSalvateRequest request = new GetRicSalvateRequest();
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.UserInfo = NavigationHandler.CurrentUser;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            adlModel.RicercaInAdl = "ADL";
            model.TabModel = adlModel;
            NavigationHandler.AdlMemento = Memento;
            logger.Info("end");
        }
    }

    public class AdlExecuteCommand : AdlCommand 
    {
        public AdlExecuteCommand(string idRicSalvata, RicercaSalvataType tipoRicSalvata)
            : base()
        {
            this._idRicSalvata = idRicSalvata;
            this._tipoRicSalvata = tipoRicSalvata;
            this._currentPage = 1;
            this._doRicerca = true;
        }
    }

    public class AdlTestoExecuteCommand : AdlCommand
    {
        public AdlTestoExecuteCommand(string testo, RicercaType tipoRic)
            : base()
        {
            this._idRicSalvata = null;
            this._testo = testo;
            this._tipoRicerca = tipoRic;
            this._currentPage = 1;
            this._doRicerca = true;
        }
    }

    public class AdlChangePageCommand : AdlCommand
    {
        public AdlChangePageCommand(int numPage)
            : base()
        {
            this._currentPage = numPage;
        }
    }

    public class AdlEnterInFolderCommand : AdlCommand
    {

        public AdlEnterInFolderCommand(string idParent, string nameParent)
            : base()
        {
            IdName idName = new IdName(idParent, nameParent);
            _path.Add(idName);
            _currentPage = 1;
        }
    }

    public class AdlUpFolderCommand : AdlCommand
    {
        public AdlUpFolderCommand()
            : base()
        {
            _path.RemoveAt(_path.Count - 1);
            _currentPage = 1;
        }

    }

    public class AddToAdlCommand : Command<MainModel>
    {
        string id;
        string tipoProto;
        bool fascicolo;

        public AddToAdlCommand(string Id, string TipoProto, bool Fascicolo)
        {
            this.id = Id;
            this.tipoProto = TipoProto;
            this.fascicolo = Fascicolo;
        }


        public override MainModel Execute()
        {
            ADLActionRequest request = new ADLActionRequest();
            request.DocInfo = new DocInfo { IdDoc = id, TipoProto = tipoProto };
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.AdlAction = ADLActionRequest.ADLActions.ADD;
            if (fascicolo)
                request.DocInfo.TipoProto = "fasc";

            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;


            ADLActionResponse resp = WSStub.ADLAction(request);

            if (resp.Code == AddToADLResponseCode.OK)
            {
                NavigationHandler.ToDoListMemento = null;
                return new ToDoListCommand().Execute();
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

    public class RemoveFromAdlCommand : Command<MainModel>
    {
        string id;
        string tipoProto;
        bool fascicolo;

        public RemoveFromAdlCommand(string Id, string TipoProto, bool Fascicolo)
        {
            this.id = Id;
            this.tipoProto = TipoProto;
            this.fascicolo = Fascicolo;
        }


        public override MainModel Execute()
        {
            ADLActionRequest request = new ADLActionRequest();
            request.DocInfo = new DocInfo { IdDoc = id, TipoProto = tipoProto };
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.AdlAction = ADLActionRequest.ADLActions.REMOVE;
            if (fascicolo)
                request.DocInfo.TipoProto = "fasc";

            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;


            ADLActionResponse resp = WSStub.ADLAction(request);

            if (resp.Code == AddToADLResponseCode.OK)
            {

                MainModel model = NavigationHandler.Model;
                return new AdlCommand().Execute();
                /*
                NavigationHandler.ToDoListMemento = null;
                return new ToDoListCommand().Execute();
                 */
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

}
