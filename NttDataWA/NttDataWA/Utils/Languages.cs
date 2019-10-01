using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace NttDataWA.Utils
{
    public class Languages
    {
        private static Dictionary<string, Dictionary<string, string>> _labelLanguages = null;
        private static Dictionary<string, Dictionary<string, string>> _messagesLanguages = null;
        private static Dictionary<string, string> _directionLanguages = null;
        private static Dictionary<string, string> _descriptionLanguages = null;

        private static string NameLabelsFile = "Labels.txt";
        private static string NameMessagesFile = "Messages.txt";

        private static string RightDirection = "rtl";
        private static string LeftDirection = "ltr";

        #region public
        //Initializes languages
        public static void InitializesLanguages()
        {
            try
            {
                if (_labelLanguages == null || _messagesLanguages == null || _labelLanguages.Count == 0 || _messagesLanguages.Count ==0 )
                {
                    _labelLanguages = new Dictionary<string, Dictionary<string, string>>();
                    _messagesLanguages = new Dictionary<string, Dictionary<string, string>>();
                    _directionLanguages = new Dictionary<string, string>();
                    _descriptionLanguages = new Dictionary<string, string>();

                    //lock (_labelLanguages) lock (_messagesLanguages) lock (_directionLanguages) lock (_descriptionLanguages)
                    //            {
                                    ReadFileLanguages();
                                //}
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// Get label of choice language 
        /// </summary>
        /// <param name="code">code</param>
        /// <param name="selectedLanguage">selectedLanguage</param>
        /// <returns></returns>
        public static string GetLabelFromCode(string code, string selectedLanguage)
        {
            try
            {
                string result = string.Empty;
                if (_labelLanguages != null)
                {
                    if (!string.IsNullOrEmpty(selectedLanguage))
                    {
                        if (_labelLanguages.ContainsKey(selectedLanguage))
                        {
                            result = (_labelLanguages[selectedLanguage])[code];
                        }
                    }
                    else
                    {
                        if (_labelLanguages.ContainsKey("Italian"))
                        {
                            result = (_labelLanguages["Italian"])[code];
                        }
                        else if (_labelLanguages.ContainsKey("English"))
                        {
                            result = (_labelLanguages["English"])[code];
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                //*********************************************DEBUG*************************************************
                //********************************************* INIZIO **********************************************
                //***************************************************************************************************
                //UIManager.AdministrationManager.DiagnosticError(ex);
                //***************************************************************************************************
                //***********************************************FINE************************************************
                return null;
            }
        }

        /// <summary>
        /// Get message of choice language 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="selectedLanguage"></param>
        /// <returns></returns>
        public static string GetMessageFromCode(string code, string selectedLanguage)
        {
            try
            {
                string result = string.Empty;
                if (_messagesLanguages != null)
                {
                    if (!string.IsNullOrEmpty(selectedLanguage))
                    {
                        if (_messagesLanguages.ContainsKey(selectedLanguage))
                        {
                            result = (_messagesLanguages[selectedLanguage])[code];
                        }
                    }
                    else
                    {
                        if (_messagesLanguages.ContainsKey("Italian"))
                        {
                            result = (_messagesLanguages["Italian"])[code];
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                //UIManager.AdministrationManager.DiagnosticError(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get descrition of choice language 
        /// </summary>
        /// <param name="selectedLanguage">string</param>
        /// <returns></returns>
        public static string GetDescriptionFromCode(string selectedLanguage)
        {
            try
            {
                string result = string.Empty;
                if (_descriptionLanguages != null)
                {
                    if (!string.IsNullOrEmpty(selectedLanguage))
                    {
                        if (_descriptionLanguages.ContainsKey(selectedLanguage))
                        {
                            result = _descriptionLanguages[selectedLanguage];
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Language direction (left/right)
        /// </summary>
        /// <param name="selectedLanguage">String</param>
        /// <returns>String</returns>
        public static string GetLanguageDirection(string selectedLanguage)
        {
            try
            {
                string result = string.Empty;
                if (_directionLanguages != null)
                {
                    if (!string.IsNullOrEmpty(selectedLanguage))
                    {
                        if (_directionLanguages.ContainsKey(selectedLanguage))
                        {
                            result = _directionLanguages[selectedLanguage];
                            if (result.ToUpper().Equals("LEFT"))
                            {
                                result = LeftDirection;
                            }
                            else
                            {
                                result = RightDirection;
                            }
                        }
                    }
                    else
                    {
                        result = LeftDirection;
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return true if multilanguages is active
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsEnableMultiLanguages()
        {
            try
            {
                bool result = false;
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.MULTI_LANGUAGES.ToString()]) && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.MULTI_LANGUAGES.ToString()]))
                {
                    result = true;
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Get available languages
        /// </summary>
        /// <returns>List<string></returns>
        public static List<String> GetAvailableLanguages()
        {
            try
            {
                List<string> result = new List<string>();
                string listLanguages = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LANGUAGES.ToString()];
                if (!string.IsNullOrEmpty(listLanguages))
                {
                    string[] arrayLanguages = listLanguages.Split(';');
                    if (arrayLanguages != null && arrayLanguages.Length > 0)
                    {
                        foreach (string lang in arrayLanguages)
                        {
                            result.Add(lang);
                        }
                    }
                }
                return result;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion

        #region private

        /// <summary>
        /// Read languages from file and initializes the languages of the singleton
        /// </summary>
        private static void ReadFileLanguages()
        {
            try
            {
                //Folder path
                string basePathFiles = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LANGUAGES_PATH.ToString()];

                //Languages available
                string listLanguages = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LANGUAGES.ToString()];

                //Languages direction
                string listLanguagesDirection = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LANGUAGES_DIRECTION.ToString()];

                //Languages description
                string listLanguagesDescription = System.Configuration.ConfigurationManager.AppSettings[Utils.WebConfigKeys.LANGUAGES_LABEL.ToString()];

                if (!string.IsNullOrEmpty(listLanguages))
                {
                    string[] arrayLanguages = listLanguages.Split(';');
                    string[] arrayDirectionLanguages = listLanguagesDirection.Split(';');
                    string[] arrayDescriptionLanguages = listLanguagesDescription.Split(';');

                    if (arrayLanguages != null && arrayLanguages.Length > 0)
                    {
                        int i = 0;
                        foreach (string lang in arrayLanguages)
                        {
                            if (!_directionLanguages.ContainsKey(lang))
                            {
                                _directionLanguages.Add(lang, arrayDirectionLanguages[i]);
                            }

                            if (!_descriptionLanguages.ContainsKey(lang))
                            {
                                _descriptionLanguages.Add(lang, arrayDescriptionLanguages[i]);
                            }

                            string basePathLabelsFiles = lang + "\\" + NameLabelsFile;

                            basePathLabelsFiles = basePathFiles.Replace("%DATA", basePathLabelsFiles);

                            using (StreamReader reader = new StreamReader(basePathLabelsFiles))
                            {
                                string line = string.Empty;
                                string descrizione = string.Empty;
                                Dictionary<string, string> temporaryLabelsDictonary = new Dictionary<string, string>();
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        string[] words = line.Split('=');
                                        if (words != null && words.Length == 2)
                                        {
                                            if (!temporaryLabelsDictonary.ContainsKey(words[0]))
                                            {
                                                temporaryLabelsDictonary.Add(words[0], words[1]);
                                            }
                                        }
                                    }
                                }
                                if (!_labelLanguages.ContainsKey(lang))
                                {
                                    _labelLanguages.Add(lang, temporaryLabelsDictonary);
                                }
                            }

                            string basePathMessagesFiles = lang + "\\" + NameMessagesFile;

                            basePathMessagesFiles = basePathFiles.Replace("%DATA", basePathMessagesFiles);

                            using (StreamReader reader = new StreamReader(basePathMessagesFiles))
                            {
                                string line = string.Empty;
                                string descrizione = string.Empty;
                                Dictionary<string, string> temporaryMessageDictonary = new Dictionary<string, string>();
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        string[] words = line.Split('=');
                                        if (words != null && words.Length == 2)
                                        {
                                            if (!temporaryMessageDictonary.ContainsKey(words[0]))
                                            {
                                                temporaryMessageDictonary.Add(words[0], words[1]);
                                            }
                                        }
                                    }
                                }
                                if (!_messagesLanguages.ContainsKey(lang))
                                {
                                    _messagesLanguages.Add(lang, temporaryMessageDictonary);
                                }
                            }

                            i++;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }
        #endregion
    }
}