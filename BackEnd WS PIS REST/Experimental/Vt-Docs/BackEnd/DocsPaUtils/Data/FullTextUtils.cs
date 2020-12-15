using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace DocsPaUtils.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FullTextUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public FullTextUtils()
        { }

        /// <summary>
        /// Parsing della stringa di query per individuare e sostituire
        /// eventuali caratteri non ammessi dal motore di database
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public abstract string ParseTextSpecialChars(string queryString);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetWildcardChar()
        {
            string valoreChiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FULLTEXT_ESCAPE_CHARACTER");
            if (!string.IsNullOrEmpty(valoreChiave))
                return valoreChiave;
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetSpecialChars()
        {
            string valoreChiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FULLTEXT_SPECIAL_CHARACTERS");
            if (!string.IsNullOrEmpty(valoreChiave))
                return valoreChiave;
            else
                return string.Empty;
        }
    }
}