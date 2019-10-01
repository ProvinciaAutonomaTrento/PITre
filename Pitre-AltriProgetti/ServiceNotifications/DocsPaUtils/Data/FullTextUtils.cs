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
        
      
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetSpecialChars()
        {
          

                return string.Empty;
        }
    }
}