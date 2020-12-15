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
    public class RicercaCommand : MainModelCommand
    {
        protected List<IdName> _path = new List<IdName>();
        protected int _currentPage = 1;
        protected string _idRicSalvata;
        protected string _testo;
        protected bool _doRicerca;
        protected RicercaType _tipoRicerca;
        protected RicercaSalvataType _tipoRicSalvata;
        private ILog logger = LogManager.GetLogger(typeof(RicercaCommand));

        public RicercaCommand()
        {
            RicercaMemento memento = NavigationHandler.RicercaMemento;
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

        public RicercaMemento Memento
        {
            get
            {
                return new RicercaMemento(_path, _currentPage, _idRicSalvata, _tipoRicSalvata,_testo,_tipoRicerca,_doRicerca);
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
            model.TabShow = Tab.RICERCA;
            RicercaModel ricModel=new RicercaModel(0,ConfigurationHandler.NumResultsForPage);
      

            if (_doRicerca)
            {
                RicercaRequest ricRequest = new RicercaRequest();
                //Normalizzo per Ricerche
                if (_tipoRicerca == RicercaType.RIC_DOCUMENTO_ADL) _tipoRicerca = RicercaType.RIC_DOCUMENTO;
                if (_tipoRicerca == RicercaType.RIC_FASCICOLO_ADL) _tipoRicerca = RicercaType.RIC_FASCICOLO; 

                ricRequest.UserInfo = NavigationHandler.CurrentUser;
                if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                    ricRequest.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

                ricRequest.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                ricRequest.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                ricRequest.IdRicercaSalvata = _idRicSalvata;
                ricRequest.TypeRicercaSalvata = _tipoRicSalvata;
                if (string.IsNullOrEmpty(_idRicSalvata))
                    ricRequest.Text = _testo;
                else
                    ricRequest.Text = string.Empty;
                ricRequest.TypeRicerca = _tipoRicerca;
                ricRequest.EnableProfilazione = ConfigurationHandler.RicercaEnableProfilazione;
                ricRequest.EnableUfficioRef = ConfigurationHandler.RicercaEnableUfficioRef;
                ricRequest.RequestedPage = _currentPage;
                ricRequest.ParentFolderId = IdParent;
                ricRequest.FascId = IdFasc;
                ricRequest.PageSize = ConfigurationHandler.NumResultsForPage;
                RicercaResponse response = WSStub.ricerca(ricRequest);
                ricModel = new RicercaModel(response.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                ricModel.Risultati = response.Risultati;
                ricModel.IdParent = IdParent;
                ricModel.NomeParent = NomeParent;
                ricModel.IdRicercaSalvata = _idRicSalvata;
                ricModel.TypeRicercaSalvata = _tipoRicSalvata;
                if (string.IsNullOrEmpty(_idRicSalvata))
                    ricModel.Testo = _testo;
                else
                    ricModel.Testo = string.Empty;
                ricModel.TypeRicerca = _tipoRicerca;
                if (response.TotalRecordCount == 0)
                {
                    ricModel.CurrentPage = 0;
                }
                else
                {
                    ricModel.CurrentPage = _currentPage;
                }
                ricModel.NumElements = response.TotalRecordCount;
            }
            GetRicSalvateRequest request = new GetRicSalvateRequest();
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.UserInfo = NavigationHandler.CurrentUser;
            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            GetRicSalvateResponse resp = WSStub.getRicercheSalvate(request);
            ricModel.Ricerche = resp.Risultati;
            model.TabModel = ricModel;
            NavigationHandler.RicercaMemento = Memento;
            logger.Info("end");
        }
    }

    public class RicExecuteCommand : RicercaCommand
    {
        public RicExecuteCommand(string idRicSalvata, RicercaSalvataType tipoRicSalvata)
            : base()
        {
            this._idRicSalvata = idRicSalvata;
            this._tipoRicSalvata = tipoRicSalvata;
            this._currentPage = 1;
            this._doRicerca = true;
        }
    }

    public class RicTestoExecuteCommand : RicercaCommand
    {
        public RicTestoExecuteCommand(string testo, RicercaType tipoRic)
            : base()
        {
            this._idRicSalvata = null;
            this._testo = testo;
            this._tipoRicerca = tipoRic;
            this._currentPage = 1;
            this._doRicerca = true;
        }
    }

    public class RicChangePageCommand : RicercaCommand
    {
        public RicChangePageCommand(int numPage)
            : base()
        {
            this._currentPage = numPage;
        }
    }

    public class RicEnterInFolderCommand : RicercaCommand
    {

        public RicEnterInFolderCommand(string idParent, string nameParent)
            : base()
        {
            IdName idName = new IdName(idParent, nameParent);
            _path.Add(idName);
            _currentPage = 1;
        }
    }

    public class RicUpFolderCommand : RicercaCommand
    {
        public RicUpFolderCommand()
            : base()
        {
            _path.RemoveAt(_path.Count - 1);
            _currentPage = 1;
        }

    }
}
