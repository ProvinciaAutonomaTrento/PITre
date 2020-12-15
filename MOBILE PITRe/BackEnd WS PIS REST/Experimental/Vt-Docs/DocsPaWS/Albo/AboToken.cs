using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaUtils;

namespace DocsPaWS.Albo
{
    public class AlboTokenVO
    {
        public String userID;
        public String CodRuolo;
        public String DocNumber;
    }

    public class AlboVO
    {
        public byte[] documentoPrincipale;

    }

    public class AlboToken
    {

        public string GenerateToken(String userID,  String CodRuolo, String DocNumber )
        {

            string AlbTok = string.Format("{0}|{1}|{2}|{3}", userID, CodRuolo, DocNumber,DateTime.Now.ToString());
            DocsPaUtils.Security.CryptoString c = new DocsPaUtils.Security.CryptoString("ALBO_TELEMATICO");
            return "TOK=" + c.Encrypt(AlbTok);

        }
        public AlboTokenVO DecryptToken(string token)
        {
            if (token.StartsWith ("TOK="))
            {
                token = token.Replace("TOK=", string.Empty);
                DocsPaUtils.Security.CryptoString c = new DocsPaUtils.Security.CryptoString("ALBO_TELEMATICO");
                token = c.Decrypt(token).Replace ("\0",string.Empty);
                string[] tokField = token.Split('|');
                AlboTokenVO reval = new AlboTokenVO { userID = tokField[0], CodRuolo = tokField[1], DocNumber = tokField[2] };
                return reval;
            }
            return null;
        }
    }
}