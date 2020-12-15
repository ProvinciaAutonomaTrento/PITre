using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;


namespace VTDocs.mobile.fe.commands.model
{
    public class LibroFirmaCommand : MainModelCommand
    {
        protected List<IdName> _path = new List<IdName>();
        protected int _currentPage = 1;
        protected int _numElements = -1;
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaCommand));
        protected string _testo;
        protected bool _doRicerca;
        protected RicercaType _tipoRicerca;

        public LibroFirmaCommand()
        {
            LibroFirmaMemento memento = NavigationHandler.LibroFirmaMemento;
            if(memento!=null){
                this._path = memento.Path;
                this._currentPage = memento.CurrentPage;
                this._numElements = memento.NumElements;
                this._testo = memento.Testo;
                this._tipoRicerca = memento.TipoRicerca;
                this._doRicerca = memento.DoRicerca;
            }
        }

        private LibroFirmaMemento Memento
        {
            get
            {
                return new LibroFirmaMemento(_currentPage, _numElements, _testo, _tipoRicerca);
            }
        }

        public void ExecuteExternal(MainModel model)
        {
            ExecuteParticular(model);

        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = Tab.LIBRO_FIRMA;

            if (string.IsNullOrEmpty(_testo))
            {
                _doRicerca = true;
                _tipoRicerca = RicercaType.RIC_OGGETTO_LF;
                _testo = string.Empty;
            }

            LibroFirmaRequest request = new LibroFirmaRequest();
            request.UserInfo = NavigationHandler.CurrentUser;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.PageSize = ConfigurationHandler.NumResultsForPage;
            request.RequestedPage = _currentPage;
            request.Testo = _testo;
            request.TipoRicerca = _tipoRicerca;
           
            LibroFirmaResponse resp = WSStub.GetLibroFirmaElements(request);
            logger.Info("responseCode: " + resp.Code);
            if (resp.Code == LibroFirmaResponseCode.OK)
            {
                LibroFirmaModel tabModel = new LibroFirmaModel(resp.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                tabModel.LibroFirmaElements = resp.Elements;
                if (resp.TotalRecordCount == 0)
                {
                    tabModel.CurrentPage = 0;
                }
                else
                {
                    tabModel.CurrentPage = _currentPage;
                }
                tabModel.NumElements = resp.TotalRecordCount;
                tabModel.Testo = _testo;
                tabModel.TypeRicerca = _tipoRicerca;
                model.TabModel = tabModel;
                
                NavigationHandler.LibroFirmaMemento = Memento;
            }
            else
            {
                logger.Info("add system error");
                addSystemError(model);
            }
            logger.Info("end");
        }

    }

   
    public class LfChangePageCommand : LibroFirmaCommand
    {
        public LfChangePageCommand(int numPage)
            : base()
        {
            _currentPage = numPage;
        }
    }

    public class RespingiSelezionatiElementiLf : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaCommand));

        public void ExecuteExternal(MainModel model)
        {
            ExecuteParticular(model);

        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            try
            {
                LibroFirmaRequest request = new LibroFirmaRequest();
                request.UserInfo = NavigationHandler.CurrentUser;
                request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                request.PageSize = ConfigurationHandler.NumResultsForPage;
                request.RequestedPage = 1;
                LibroFirmaMemento memento = NavigationHandler.LibroFirmaMemento;
                if (memento != null)
                {
                    request.Testo = memento.Testo;
                    request.TipoRicerca = memento.TipoRicerca;
                }
                else
                {
                    request.Testo = string.Empty;
                    request.TipoRicerca = RicercaType.RIC_OGGETTO_LF;
                }
                logger.Debug("chiamata ws...");
                LibroFirmaResponse response = WSStub.RespingiSelezionatiElementiLf(request);
                if (response.Code == LibroFirmaResponseCode.OK)
                {
                    LibroFirmaModel tabModel = new LibroFirmaModel(response.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                    tabModel.LibroFirmaElements = response.Elements;
                    if (response.TotalRecordCount == 0)
                    {
                        tabModel.CurrentPage = 0;
                    }
                    else
                    {
                        tabModel.CurrentPage = 1;
                    }
                    tabModel.NumElements = response.TotalRecordCount;
                    tabModel.Testo = request.Testo;
                    tabModel.TypeRicerca = request.TipoRicerca;
                    model.TabModel = tabModel;
                    model.TabShow = Tab.LIBRO_FIRMA;
                    NavigationHandler.LibroFirmaMemento = new LibroFirmaMemento(tabModel.CurrentPage, tabModel.NumElements, tabModel.Testo, tabModel.TypeRicerca);
                }
                else
                {
                    logger.Info("add system error");
                }
                logger.Info("end");
            }
            catch (Exception e)
            {
            }
        }
    }

    public class FirmaSelezionatiElementiLf : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaCommand));

        public void ExecuteExternal(MainModel model)
        {
            ExecuteParticular(model);

        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            try
            {
                LibroFirmaRequest request = new LibroFirmaRequest();
                request.UserInfo = NavigationHandler.CurrentUser;
                request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                request.PageSize = ConfigurationHandler.NumResultsForPage;
                request.RequestedPage = 1;
                LibroFirmaMemento memento = NavigationHandler.LibroFirmaMemento;
                if (memento != null)
                {
                    request.Testo = memento.Testo;
                    request.TipoRicerca = memento.TipoRicerca;
                }
                else
                {
                    request.Testo = string.Empty;
                    request.TipoRicerca = RicercaType.RIC_OGGETTO_LF;
                }
                logger.Debug("chiamata ws...");
                LibroFirmaResponse response = WSStub.FirmaSelezionatiElementiLf(request);
                if (response.Code == LibroFirmaResponseCode.OK)
                {
                    LibroFirmaModel tabModel = new LibroFirmaModel(response.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                    tabModel.LibroFirmaElements = response.Elements;
                    if (response.TotalRecordCount == 0)
                    {
                        tabModel.CurrentPage = 0;
                    }
                    else
                    {
                        tabModel.CurrentPage = 1;
                    }
                    tabModel.NumElements = response.TotalRecordCount;
                    tabModel.Testo = request.Testo;
                    tabModel.TypeRicerca = request.TipoRicerca;
                    model.TabModel = tabModel;
                    model.TabShow = Tab.LIBRO_FIRMA;
                    NavigationHandler.LibroFirmaMemento = new LibroFirmaMemento(tabModel.CurrentPage, tabModel.NumElements, tabModel.Testo, tabModel.TypeRicerca);
                }
                else
                {
                    logger.Info("add system error");
                }
                logger.Info("end");
            }
            catch (Exception e)
            {
            }
        }
    }

    public class CambiaStatoElementoLFCommand : Command<OperationResponse>
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaCommand));
        private LibroFirmaElement _element;
        private string _nuovoStato;

        public CambiaStatoElementoLFCommand(string idElemento, string dataAccettazione, string oldStato, string nuovoStato, string motivoespingimento, string idIstanzaProcesso)
        {
            _element = new LibroFirmaElement();
            _element.IdElemento = idElemento;
            _element.StatoFirma = oldStato;
            _element.DataAccettazione = dataAccettazione;
            _element.MotivoRespingimento = motivoespingimento;
            _element.IdIstanzaProcesso = idIstanzaProcesso;
            _nuovoStato  = nuovoStato;
        }

        public override OperationResponse Execute()
        {
            logger.Info("begin");
            OperationResponse res = new OperationResponse();
            try
            {
                LibroFirmaCambiaStatoRequest request = new LibroFirmaCambiaStatoRequest();
                request.elemento = _element;
                request.UserInfo = NavigationHandler.CurrentUser;
                request.IdorrGlobaliRuolo  = NavigationHandler.RuoloInfo.Id;
                request.NuovoStato = _nuovoStato;
                logger.Debug("chiamata ws...");
                LibroFirmaResponse response = WSStub.CambiaStatoElementoLibroFirma(request);
                if (response.Code == LibroFirmaResponseCode.OK)
                {
                    res.Success = true;
                }
                else
                {
                    res.Success = false;
                    res.Error = Resources.Errors.Common_SystemError;
                }
                logger.Debug("chiamata eseguita");
            }
            catch (Exception e)
            {
                res.Success = false;
                res.Error = Resources.Errors.Common_SystemError;
                return res;
            }
            return res;
        }
    }

    public class ExistsElementWithSignCadesCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ExistsElementWithSignCadesCommand));

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            UserInfo userInfo= NavigationHandler.CurrentUser;
            RuoloInfo ruolo = NavigationHandler.RuoloInfo;
            model.TabModel = NavigationHandler.Model.TabModel;
            bool result = WSStub.ExistsElementWithSignCades(userInfo, ruolo);
            if (result)
                (model.TabModel as LibroFirmaModel).ExistsSignCades = true;
            else
                (model.TabModel as LibroFirmaModel).ExistsSignCades = false;

            logger.Info("end");
        }
    }

    public class ExistsElementWithSignPadesCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(ExistsElementWithSignPadesCommand));

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            UserInfo userInfo = NavigationHandler.CurrentUser;
            RuoloInfo ruolo = NavigationHandler.RuoloInfo;
            model.TabModel = NavigationHandler.Model.TabModel;
            bool result = WSStub.ExistsElementWithSignPades(userInfo, ruolo);
            if (result)
                (model.TabModel as LibroFirmaModel).ExistsSignPades = true;
            else
                (model.TabModel as LibroFirmaModel).ExistsSignPades = false;

            logger.Info("end");
        }
    }

    public class LibroFirmaIsAutorizedCommand : MainModelCommand
    {
        private ILog logger = LogManager.GetLogger(typeof(LibroFirmaIsAutorizedCommand));

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            RuoloInfo ruolo = NavigationHandler.RuoloInfo;
            bool result = WSStub.LibroFirmaIsAutorized(ruolo);
            if (result)
                model.IsAutorizedLF = true;
            else
                model.IsAutorizedLF = false;

            logger.Info("end");
        }
    }

    public class LibroFirmaTestoExecuteCommand : LibroFirmaCommand
    {
        public LibroFirmaTestoExecuteCommand(string testo, RicercaType tipoRic)
            : base()
        {
            this._testo = testo;
            this._tipoRicerca = tipoRic;
            this._currentPage = 1;
            this._doRicerca = true;
        }
    }
}

