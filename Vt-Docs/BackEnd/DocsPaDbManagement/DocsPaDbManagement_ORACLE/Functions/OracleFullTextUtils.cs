using System;
using System.Collections.Generic;
using System.Text;
using DocsPaUtils.Data;

namespace DocsPaDbManagement.Functions
{
    /// <summary>
    /// 
    /// </summary>
    public class OracleFullTextUtils : FullTextUtils
    {
        /// <summary>
        /// 
        /// </summary>
        public OracleFullTextUtils()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public override string ParseTextSpecialChars(string queryString)
        {
            string[] specialChars = this.GetSpecialChars().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string wildcardChar = this.GetWildcardChar();

            if (specialChars.Length > 0 && wildcardChar != string.Empty)
            {
                foreach (string item in specialChars)
                {
                    queryString = this.ReplaceSpecialChar(queryString, 0, item, wildcardChar);
                }
            }

            return queryString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="startIndex"></param>
        /// <param name="specialChar"></param>
        /// <param name="wildcardChar"></param>
        /// <returns></returns>
        protected string ReplaceSpecialChar(string queryString, int startIndex, string specialChar, string wildcardChar)
        {
            startIndex = queryString.IndexOf(specialChar, startIndex);

            if (startIndex > -1)
            {
                bool replaceInit = true;
                if ((startIndex - wildcardChar.Length) > 0)
                {
                    replaceInit = (queryString.Substring((startIndex - wildcardChar.Length), wildcardChar.Length) != wildcardChar);
                }

                if (replaceInit)
                {
                    queryString = queryString.Insert(startIndex, wildcardChar);
                    startIndex = (startIndex + wildcardChar.Length + 1);
                }
                else
                {
                    startIndex = (startIndex + wildcardChar.Length);
                }

                if (startIndex < queryString.Length)
                {
                    if (queryString.Substring(startIndex, wildcardChar.Length) != wildcardChar)
                    {
                        queryString = queryString.Insert(startIndex, wildcardChar);
                        startIndex = (startIndex + wildcardChar.Length);
                    }
                }
                else if (!queryString.EndsWith(wildcardChar))
                    queryString += wildcardChar;

                if (startIndex < queryString.Length)
                {
                    queryString = this.ReplaceSpecialChar(queryString, startIndex, specialChar, wildcardChar);
                }
            }

            return queryString;
        }
    }
}