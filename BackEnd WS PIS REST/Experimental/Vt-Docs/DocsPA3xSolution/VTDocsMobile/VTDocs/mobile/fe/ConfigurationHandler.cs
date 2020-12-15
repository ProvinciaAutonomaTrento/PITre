using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VTDocs.mobile.fe
{
    public class ConfigurationHandler
    {

        private string Prefix
        {
            get
            {
                NavigationHandler nh = new NavigationHandler();
                string prefix = "Normal";
                if (nh.ViewType == ViewType.IPAD)
                {
                    prefix = "Ipad";
                } else
                if (nh.ViewType == ViewType.GALAXY)
                {
                    prefix = "Galaxy";
                }
                return prefix;
            }
        }

        public bool RicercaEnableProfilazione
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings["ProfilazioneDinamicaFasc"];
                if (confValue != null && confValue == "1")
                    return true;
                return false;
            }
        }

        public bool RicercaEnableUfficioRef
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings["EnableUfficioRef"];
                if (confValue != null && confValue == "1")
                    return true;
                return false;
            }
        }

        public int NumResultsForPage
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings[Prefix+".NumResultsForPage"];
                if (confValue != null) return Int32.Parse(confValue);
                return 5;
            }

        }

        public bool RemoveTrasmInTDL
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings["RemoveTrasmInTDL"];
                if (confValue != null && confValue == "1")
                    return true;
                return false;
            }
        }

        public int MaxStringLength
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings[Prefix+".MaxStringLength"];
                if (confValue != null) return Int32.Parse(confValue);
                return 100;
            }

        }

        public int MaxNumRisultatiAutocomplete
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings[Prefix + ".MaxNumRisultatiAutocomplete"];
                if (confValue != null) return Int32.Parse(confValue);
                return 20;
            }
        }

        public int MaxNumRisultatiAutocompleteURP
        {
            get
            {
                string confValue = System.Configuration.ConfigurationManager.AppSettings[Prefix + ".MaxNumRisultatiAutocompleteURP"];
                if (confValue != null) return Int32.Parse(confValue);
                return 5;
            }
        }

        public string LoginImagePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[Prefix+".LoginImagePath"];
            }
        }

        public string IconImagePath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings[Prefix + ".IconImagePath"];
            }
        }

        public string Titolo
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["Titolo"];
            }
        }

        public string SkinFolder
        {
            get
            {
                string temp=System.Configuration.ConfigurationManager.AppSettings["Skin"];
                if(string.IsNullOrEmpty(temp)) temp="Green";
                return temp;
            }
        }

        public bool SmistamentoCollapseRuoli
        {
            get
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["SmistamentoCollapseRuoli"];
                if (!string.IsNullOrEmpty(value) && value == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
        public string GetMobileVersion
        {
            get
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                else //default 
                {
                    return string.Empty;
                }
            }

        }
        public bool IsVisibleButtonVisto
        {
            get
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                if (!string.IsNullOrEmpty(value) && value == "2")
                {
                    return true;
                }
                else //default 
                {
                    return false;
                }
            }
        }
    }
}