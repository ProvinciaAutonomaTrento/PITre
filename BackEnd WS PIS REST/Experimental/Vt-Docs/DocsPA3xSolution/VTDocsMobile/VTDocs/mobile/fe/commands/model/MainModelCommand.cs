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
    public abstract class MainModelCommand : Command<MainModel>
    {
        private ILog logger = LogManager.GetLogger(typeof(MainModelCommand));

        public override MainModel Execute()
        {
            logger.Info("begin");
            MainModel res = new MainModel();

            if (NavigationHandler.CurrentUser == null)
            {
                res.SessionExpired = true;
                return res;
            }
            res.DescrUtente = NavigationHandler.CurrentUser.Descrizione;
            res.IdRuolo = NavigationHandler.RuoloInfo.Id;
            res.DescrRuolo = NavigationHandler.RuoloInfo.Descrizione;
            if (NavigationHandler.Model != null && NavigationHandler.RememberLastTab)
            {
                if (NavigationHandler.Model.PreviousTabModel != null)
                    if (NavigationHandler.Model.TabModel == null)
                        NavigationHandler.Model.TabModel = NavigationHandler.Model.PreviousTabModel;

                if (!(NavigationHandler.Model.PreviousTabName == "DETTAGLIO_DOC" && NavigationHandler.Model.TabShow.ToString() == "DETTAGLIO_DOC"))
                {
                    res.PreviousTabModel2 = NavigationHandler.Model.PreviousTabModel;
                    res.PreviousTabName2 = NavigationHandler.Model.PreviousTabName;
                    res.PreviousTabModel = NavigationHandler.Model.TabModel;
                    res.PreviousTabName = NavigationHandler.Model.TabShow.ToString();
                }
                else
                {
                    res.PreviousTabModel2 = NavigationHandler.Model.PreviousTabModel2;
                    res.PreviousTabName2 = NavigationHandler.Model.PreviousTabName2;
                    res.PreviousTabModel = NavigationHandler.Model.PreviousTabModel;
                    res.PreviousTabName = NavigationHandler.Model.PreviousTabName;
                }
            }
            try
            {
                ExecuteParticular(res);
            }
            catch (Exception e)
            {
                logger.Error("Exception in calling executeParticular: " + e);
                addSystemError(res);
            }
            if (NavigationHandler.ToDoListMemento != null)
            {
                res.ToDoListTotalElements = NavigationHandler.ToDoListMemento.NumElements;
            }
            RuoloInfo[] ruoli = NavigationHandler.CurrentUser.Ruoli;
            res.Ruoli = new List<Ruolo>();
            foreach (RuoloInfo temp in ruoli) res.Ruoli.Add(new Ruolo(temp));
            res.NumDeleghe = 0;
            if (NavigationHandler.DelegaEsercitata != null)
            {
                res.DelegaEsercitata = NavigationHandler.DelegaEsercitata;
            }
            NavigationHandler.Model = res;
            logger.Info("end");
            return res;
        }

        protected void addSystemError(MainModel input)
        {
            input.Errori = new List<string> { Resources.Errors.Common_SystemError };
        }

        protected abstract void ExecuteParticular(MainModel model);
    }
}
