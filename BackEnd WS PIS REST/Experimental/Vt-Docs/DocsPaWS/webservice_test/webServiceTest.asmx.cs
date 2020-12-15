using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Collections;
using log4net;

namespace DocsPaWS.webservice_test
{
    /// <summary>
    /// Summary description for webServiceTest
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class webServiceTest : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(webServiceTest));

        [WebMethod]
        public DocsPaVO.documento.FileDocumento[] outFileRequest(string userName, string pass,string codRuolo, string[] idDocNumber)
        {
            List<DocsPaVO.documento.FileDocumento> lista = new List<DocsPaVO.documento.FileDocumento>();
            DocsPaVO.utente.UserLogin user = new DocsPaVO.utente.UserLogin();
            user.UserName = userName;
            user.Password = pass;
            DocsPaVO.utente.UserLogin.LoginResult result;
            string ipaddress = string.Empty;

            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.Login.loginMethod(user, out result, true, null, out ipaddress);

            if (result == DocsPaVO.utente.UserLogin.LoginResult.OK)
            {

                DocsPaVO.utente.Ruolo ruolo = null;
                foreach (DocsPaVO.utente.Ruolo r in utente.ruoli)
                {
                    if (r.codiceRubrica.ToLower() == codRuolo.ToLower())
                    {
                        ruolo = r;
                        break;
                    }
                }


                if (ruolo != null)
                {
                    string token = GetToken(utente, ruolo);
                    DocsPaVO.utente.InfoUtente infoutente = null;
                    infoutente = getInfoUtenteFromToken(token);
                    DocsPaVO.documento.SchedaDocumento scheda = BusinessLogic.Documenti.DocManager.getDettaglio(infoutente, idDocNumber[0], idDocNumber[0]);


                    foreach (string docnumber in idDocNumber)
                    {
                        DocsPaVO.documento.FileDocumento filedoc = null;
                        DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoutente, docnumber, docnumber);
                        for (int i = 0; i < scheda.documenti.Count; i++)
                        {
                            filedoc = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)scheda.documenti[i], infoutente);
                            lista.Add(filedoc);
                        }

                        for (int i = 0; i < scheda.allegati.Count; i++)
                        {
                            filedoc = BusinessLogic.Documenti.FileManager.getFile((DocsPaVO.documento.FileRequest)scheda.allegati[i], infoutente);
                            lista.Add(filedoc);
                        }
                    }
                }
            }
            return lista.ToArray();
        }


        private DocsPaVO.utente.InfoUtente getInfoUtenteFromToken(string tokenDiAutenticazione)
        {
            try
            {
                string clearToken = Decrypt(tokenDiAutenticazione);
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

        private string Decrypt(string cipherString)
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


        private string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
            {
                if (rl.idGruppo == ruolo.idGruppo)
                    okRuolo = true;
            }

            if (okRuolo)
            {
                string tokenDiAutenticazione = null;
                try
                {
                    string clearToken = string.Empty;
                    clearToken += ruolo.systemId + "|";
                    clearToken += utente.idPeople + "|";
                    clearToken += ruolo.idGruppo + "|";
                    clearToken += utente.dst + "|";
                    clearToken += utente.idAmministrazione + "|";
                    clearToken += utente.userId + "|";
                    clearToken += utente.sede + "|";
                    clearToken += utente.urlWA;

                    tokenDiAutenticazione = Encrypt(clearToken);
                }
                catch (Exception e)
                {
                    logger.Debug("Errore durante il GetInfoUtente.", e);
                }

                return tokenDiAutenticazione;
            }
            else
            {
                logger.Debug("L'utente : " + utente.descrizione + " non appartiene al ruolo : " + ruolo.descrizione);
                return null;
            }
        }


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

        [WebMethod()]
        public string ricercaCodice(string codice)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.ricercaCodice(codice);
        }

        [WebMethod()]
        public bool codicePerlaPubblicazione(string codice)
        {
            return BusinessLogic.Pubblicazione.PubblicazioneDocumenti.codicePerlaPubblicazione(codice);
        }
    }
}
