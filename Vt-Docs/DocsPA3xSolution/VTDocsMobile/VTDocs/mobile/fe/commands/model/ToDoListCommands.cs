using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocs.mobile.fe.model;
using VTDocsMobile.VTDocsWSMobile;
using log4net;

namespace VTDocs.mobile.fe.commands.model
{
    public class ToDoListCommand : MainModelCommand
    {
        protected List<IdName> _path = new List<IdName>();
        protected int _currentPage = 1;
        protected int _numElements = -1;
        protected string _idTrasm;
        protected string _idEvento;
        private ILog logger = LogManager.GetLogger(typeof(ToDoListCommand));

        public ToDoListCommand()
        {
            ToDoListMemento memento = NavigationHandler.ToDoListMemento;
            if(memento!=null){
                this._path = memento.Path;
                this._currentPage = memento.CurrentPage;
                this._numElements = memento.NumElements;
                this._idTrasm = memento.IdTrasm;
                this._idEvento = memento.IdEvento;
            }
        }

        private ToDoListMemento Memento
        {
            get
            {
                return new ToDoListMemento(_path,_currentPage,_numElements,_idTrasm, _idEvento);
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

        public void ExecuteExternal(MainModel model)
        {
            ExecuteParticular(model);

        }

        protected override void ExecuteParticular(MainModel model)
        {
            logger.Info("begin");
            model.TabShow = Tab.TODO_LIST;
            ToDoListRequest request = new ToDoListRequest();
            request.UserInfo = NavigationHandler.CurrentUser;

            if (NavigationHandler.DeleganteInfo != null) //Abbiamo una delega in esercizio
                request.UserInfo.DelegatoInfo = NavigationHandler.LoggedInfo;

            request.PageSize = ConfigurationHandler.NumResultsForPage;
            request.RequestedPage = _currentPage;
            request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
            request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
            request.Registri = NavigationHandler.Registri;
            request.ParentFolderId = IdParent;
            request.FascId = IdFasc;
            ToDoListResponse resp = WSStub.getTodoList(request);
            logger.Info("responseCode: " + resp.Code);
            if (resp.Code == ToDoListResponseCode.OK)
            {
                ToDoListModel tabModel = new ToDoListModel(resp.TotalRecordCount, ConfigurationHandler.NumResultsForPage);
                tabModel.IdParent = IdParent;
                tabModel.NomeParent = NomeParent;
                tabModel.ToDoListElements = resp.Elements;
                if (resp.TotalRecordCount == 0)
                {
                    tabModel.CurrentPage = 0;
                }
                else
                {
                    tabModel.CurrentPage = _currentPage;
                }
                if (_numElements < 0 || IdParent==null)
                {
                    _numElements = resp.TotalRecordCount;
                }
                if (_idTrasm != null)
                {
                    foreach (ToDoListElement el in tabModel.ToDoListElements)
                    {
                        el.IdTrasm = _idTrasm;
                    }
                }
                //Verifico se il pulsante visto è visibile
                bool setDataVista = ConfigurationHandler.IsVisibleButtonVisto;
                tabModel.SetDataVista = setDataVista;

                tabModel.NumElements = _numElements;
                model.TabModel = tabModel;
                NavigationHandler.ToDoListMemento = Memento;
            }
            else
            {
                logger.Info("add system error");
                addSystemError(model);
            }
            logger.Info("end");
        }

    }

    public class TDLEnterInFolderCommand : ToDoListCommand
    {

        public TDLEnterInFolderCommand(string idParent,string nameParent,string idTrasm) : base()
        {
            IdName idName = new IdName(idParent, nameParent);
            _path.Add(idName);
            _currentPage = 1;
            _idTrasm = idTrasm;
        }
    }

    public class TDLUpFolderCommand : ToDoListCommand
    {
        public TDLUpFolderCommand() : base()
        {
            _path.RemoveAt(_path.Count - 1);
            if (_path.Count == 0) _idTrasm = null;
            _currentPage = 1;
        }
    }

    public class TDLChangePageCommand : ToDoListCommand
    {
        public TDLChangePageCommand(int numPage)
            : base()
        {
            _currentPage = numPage;
        }
    }

    public class TDLRimuoviNotifica : Command<MainModel>
    {
        private string _idEvento;

        public TDLRimuoviNotifica(string idEvento)
        {
            _idEvento = idEvento;
        }

        public override MainModel Execute()
        {
            string idEvento = this._idEvento;
            bool result = WSStub.rimuoviNotifica(idEvento);
            if (result)
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
}