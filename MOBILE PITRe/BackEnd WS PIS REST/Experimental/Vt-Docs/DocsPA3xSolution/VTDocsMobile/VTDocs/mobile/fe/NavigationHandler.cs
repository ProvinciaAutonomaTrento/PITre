
using System.Web;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.model;
using VTDocs.mobile.fe.commands;
using log4net;

namespace VTDocs.mobile.fe
{
    public class NavigationHandler
    {
        private string LOGGED_SESSION_KEY = "LOGGED_SESSION_KEY";
        private string DELEGANTE_SESSION_KEY = "DELEGANTE_SESSION_KEY";
        private string ROLE_SESSION_KEY = "ROLE_SESSION_KEY";
        private string MODEL_SESSION_KEY = "MODEL_SESSION_KEY";
        private string TDL_MEMENTO_SESSION_KEY = "TDL_COMMAND_MEMENTO_SESSION_KEY";
        private string RICERCA_MEMENTO_SESSION_KEY = "RICERCA_MEMENTO_SESSION_KEY";
        private string ADL_MEMENTO_SESSION_KEY = "ADL_MEMENTO_SESSION_KEY";
        private string DELEGA_ESERCITATA_SESSION_KEY = "DELEGA_ESERCITATA_SESSION_KEY";
        private string LF_MEMENTO_SESSION_KEY = "LF_MEMENTO_SESSION_KEY";

        public UserInfo LoggedInfo
        {
            get { return (UserInfo) HttpContext.Current.Session[LOGGED_SESSION_KEY];}
            set { 
                HttpContext.Current.Session.Add(LOGGED_SESSION_KEY, value);
            }
        }

        public UserInfo DeleganteInfo
        {
            get { return (UserInfo)HttpContext.Current.Session[DELEGANTE_SESSION_KEY]; }
            set
            {
                HttpContext.Current.Session[ROLE_SESSION_KEY] = null;
                HttpContext.Current.Session.Add(DELEGANTE_SESSION_KEY, value);
            }
        }

        public UserInfo CurrentUser
        {
            get
            {
                return (DeleganteInfo != null) ? DeleganteInfo : LoggedInfo;
            }
        }

        public RuoloInfo RuoloInfo
        {
            get {
                if (HttpContext.Current.Session[ROLE_SESSION_KEY] != null)
                {
                    return (RuoloInfo)HttpContext.Current.Session[ROLE_SESSION_KEY];
                }
                else if (CurrentUser != null && CurrentUser.Ruoli != null && CurrentUser.Ruoli.Length > 0)
                {
                    HttpContext.Current.Session.Add(ROLE_SESSION_KEY, CurrentUser.Ruoli[0]);
                    return (RuoloInfo)HttpContext.Current.Session[ROLE_SESSION_KEY];
                }
                else
                {
                    return null;
                }
            }
            set { 
                HttpContext.Current.Session.Add(ROLE_SESSION_KEY, value); 
            }
        }

        public string Registri
        {
            get {
                string res = "0";
                RuoloInfo ruolo = RuoloInfo;
                if (ruolo!=null && ruolo.Registri!=null &&  ruolo.Registri.Length> 0)
                {
                    foreach (RegistroInfo registro in RuoloInfo.Registri)
                    {
                        res = res + "," + registro.SystemId;
                    }
                    return res;
                }
                else return res;
            }
        }

        public MainModel Model
        {
            get
            {
                return (MainModel) HttpContext.Current.Session[MODEL_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(MODEL_SESSION_KEY, value);
            }

        }

        public ToDoListMemento ToDoListMemento
        {
            get
            {
                return (ToDoListMemento)HttpContext.Current.Session[TDL_MEMENTO_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(TDL_MEMENTO_SESSION_KEY, value);
            }
        }

        public AdlMemento AdlMemento
        {
            get
            {
                return (AdlMemento)HttpContext.Current.Session[ADL_MEMENTO_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(ADL_MEMENTO_SESSION_KEY, value);
            }
        }

        public RicercaMemento RicercaMemento
        {
            get
            {
                return (RicercaMemento)HttpContext.Current.Session[RICERCA_MEMENTO_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(RICERCA_MEMENTO_SESSION_KEY, value);
            }
        }

        public LibroFirmaMemento LibroFirmaMemento
        {
            get
            {
                return (LibroFirmaMemento)HttpContext.Current.Session[LF_MEMENTO_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(LF_MEMENTO_SESSION_KEY, value);
            }
        }

        public Delega DelegaEsercitata
        {
            get
            {
                return (Delega)HttpContext.Current.Session[DELEGA_ESERCITATA_SESSION_KEY];
            }
            set
            {
                HttpContext.Current.Session.Add(DELEGA_ESERCITATA_SESSION_KEY, value);
            }
        }

        public string SessionId
        {
            get
            {
                return HttpContext.Current.Session.SessionID;
            }
        }

        public string IpAddress
        {
            get
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        public string UserId
        {
            get
            {
                if (this.CurrentUser != null) return CurrentUser.UserId;
                return null;
            }
        }

        public ViewType ViewType
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings["IdDevice"];
                if (!string.IsNullOrEmpty(confValue))
                {
                    if ("I".Equals(confValue))
                    {
                        return ViewType.IPAD;
                    }
                    else if ("G".Equals(confValue))
                    {
                        return ViewType.GALAXY;
                    }
                    else 
                    {
                        return ViewType.NORMAL;
                    }
                }
                //USERGAGENT
                string userAgent=HttpContext.Current.Request.UserAgent.ToLower();
                if (
                    userAgent.Contains("ipad")||
                    userAgent.Contains("gt-p51") ||
                    userAgent.Contains("nexus 7") ||
                    userAgent.ToUpper().Contains("GT-P7500") ||
                    userAgent.ToUpper().Contains("GT P7500")
                    
                   
                    )
                {
                    return ViewType.IPAD;
                }
                else if (
                    userAgent.Contains ("sch-i800") ||
                    userAgent.Contains ("gt-p1000") ||
                    userAgent.Contains ("gt-p31") ||
                    userAgent.Contains("gt-i93")
                    
                    )
                {
                    return ViewType.GALAXY;
                }
                else
                {
                    return ViewType.NORMAL;
                }
            }
        }

        public bool RememberLastTab
        {
            get
            {
                //8-2-2011: prima valeva solo per IPAD, adesso vale per tutti i dispositivi
                return true;
            }

        }

        public string NoPreviewImage
        {
            get
            {
                if (ViewType == ViewType.IPAD)
                {
                    return "~/Content/img/ipad/no_preview.jpg";
                }
                else if (ViewType == ViewType.GALAXY)
                {
                    return "~/Content/img/galaxy/no_preview.jpg";
                } else 
                {
                    return "~/Content/img/no_preview.jpg";
                }
            }
        }

        public void clearSession()
        {
            HttpContext.Current.Session[LOGGED_SESSION_KEY] = null;
            HttpContext.Current.Session[DELEGANTE_SESSION_KEY] = null;
            HttpContext.Current.Session[ROLE_SESSION_KEY] = null;
            HttpContext.Current.Session[MODEL_SESSION_KEY] = null;
            HttpContext.Current.Session[TDL_MEMENTO_SESSION_KEY] = null;
            HttpContext.Current.Session[RICERCA_MEMENTO_SESSION_KEY] = null;
            HttpContext.Current.Session[ADL_MEMENTO_SESSION_KEY] = null;
            HttpContext.Current.Session[DELEGA_ESERCITATA_SESSION_KEY] = null;
            HttpContext.Current.Session[LF_MEMENTO_SESSION_KEY] = null;           
        }
    }

}
