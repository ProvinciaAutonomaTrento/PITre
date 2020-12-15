using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Collections;

namespace DocsPaWS.SmartServices
{
    public class Utils
    {
        public static string Encrypt(string toEncrypt)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
                
                //Di default disabilito l'hashing
                bool useHashing = false;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";
                
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Decrypt(string cipherString)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                //Di default disabilito l'hashing
                bool useHashing = false;

                //La chiave deve essere di 24 caratteri
                string key = "ValueTeamDocsPa3Services";

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DocsPaVO.utente.InfoUtente getInfoUtenteFromToken(string tokenDiAutenticazione)
        {
            try
            {
                string clearToken = Utils.Decrypt(tokenDiAutenticazione);
                string[] arrayInfoUtente = clearToken.Split('|');

                DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();
                infoUtente.idCorrGlobali = arrayInfoUtente[0];
                infoUtente.idPeople = arrayInfoUtente[1];
                infoUtente.idGruppo = arrayInfoUtente[2];
                infoUtente.dst = arrayInfoUtente[3];
                infoUtente.idAmministrazione = arrayInfoUtente[4];
                infoUtente.userId = arrayInfoUtente[5];
                infoUtente.sede = arrayInfoUtente[6];
                infoUtente.urlWA = arrayInfoUtente[7];

                return infoUtente;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DocsPaVO.utente.Ruolo getRuoloFromInfoUtente(DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                ArrayList ruoli = BusinessLogic.Utenti.UserManager.getRuoliUtente(infoUtente.idPeople);

                foreach (DocsPaVO.utente.Ruolo ruolo in ruoli)
                {
                    if (ruolo.systemId == infoUtente.idCorrGlobali)
                        return ruolo;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
