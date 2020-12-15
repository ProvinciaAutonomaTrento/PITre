using DocsPaVO.utente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using log4net;
using DocsPaVO.Mobile;
using DocsPaVO.trasmissione;

namespace DocsPaRESTWS.Manager
{
    public class Utils
    {

        private static ILog logger = LogManager.GetLogger(typeof(Utils));

        public static string CreateAuthToken(Utente utente, Ruolo ruolo)
        {
            //string token = DocsPaUtils.Security.SSOAuthTokenHelper.Generate(utente.userId);

            string ss_token = GetToken(utente, ruolo);
            return ss_token;
        }

        public static string GetToken(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            //Controllo Correttezza Ruolo
            bool okRuolo = false;
            if (utente != null && utente.ruoli != null && utente.ruoli.Count > 0)
            {
                foreach (DocsPaVO.utente.Ruolo rl in utente.ruoli)
                {
                    if (rl.idGruppo == ruolo.idGruppo)
                        okRuolo = true;
                }
            }
            else okRuolo = true;

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

                    tokenDiAutenticazione = Utils.Encrypt(clearToken);
                }
                catch (Exception e)
                {
                    //  logger.Debug("Errore durante il GetInfoUtente.", e);
                }

                tokenDiAutenticazione = "SSO=" + tokenDiAutenticazione;
                return tokenDiAutenticazione;
            }
            else
            {
                //logger.Debug("L'utente : " + utente.descrizione + " non appartiene al ruolo : " + ruolo.descrizione);
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

        public static TrasmInfo buildInstanceTrasmInfo(Trasmissione input, DocsPaVO.utente.Utente delegato, InfoUtente infoUt)
        {
            TrasmInfo trasmInfo = new TrasmInfo();
            trasmInfo.IdTrasm = input.systemId;
            trasmInfo.NoteGenerali = input.noteGenerali;
            trasmInfo.Mittente = input.utente.descrizione;
            if (delegato != null)
            {
                trasmInfo.Mittente = delegato.descrizione + " delegato da " + input.utente.descrizione;
            }
            foreach (TrasmissioneSingola temp in input.trasmissioniSingole)
            {
                foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                {
                    if (infoUt.userId.Equals(tempUt.utente.userId))
                    {
                        trasmInfo.Ragione = temp.ragione.descrizione;
                        trasmInfo.NoteIndividuali = temp.noteSingole;
                        trasmInfo.IdTrasmUtente = tempUt.systemId;
                        if (!String.IsNullOrEmpty(tempUt.dataAccettata)) trasmInfo.Accettata = true;
                        if (!String.IsNullOrEmpty(tempUt.dataRifiutata)) trasmInfo.Rifiutata = true;

                        if ("W".Equals(temp.ragione.tipo)) trasmInfo.HasWorkflow = true;
                    }
                }
            }
            trasmInfo.Data = toDate(input.dataInvio);
            return trasmInfo;
        }

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            return DateTime.ParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        }

        public static string buildDateString(DateTime data)
        {
            return buildDateString(data, false);
        }

        public static string buildDateString(DateTime data, bool isEnding)
        {
            if (isEnding)
            {
                return data.ToShortDateString() + " 23.59.59";
            }
            else
            {
                string ora = "" + data.Hour;
                if (ora.Length == 1) ora = "0" + ora;
                return data.ToShortDateString() + " " + ora + ".00.00";
            }
        }

        public static string getCondivisionToken(string idpeople, string iddocument)
        {
            string retval = "";
            string clearToken = string.Empty;
            clearToken += idpeople + "|";
            clearToken += iddocument + "|";
            clearToken += "chiavePerDoc|";
            clearToken += DateTime.Now.AddHours(72).ToString("u");
            
            retval = Utils.Encrypt(clearToken);

            return retval;
        }

        public static string ctrlCondivisioneToken(string idPeople, string idCorrPeople, string token)
        {
            string message = "OK";
            string clearToken = Utils.Decrypt(token);
            logger.Debug(idPeople + " - " + idCorrPeople);
            logger.Debug(clearToken);
            if (!string.IsNullOrWhiteSpace(clearToken))
            {
                string[] tkn = clearToken.Split('|');
                if (!(tkn[0] == idPeople || tkn[0] == idCorrPeople))
                    message = "WRONG_USER";
                else
                {
                    logger.Debug(tkn[3]);
                    DateTime dataTkn = DateTime.ParseExact(tkn[3], "u", System.Globalization.CultureInfo.InvariantCulture);

                    //DateTime.Parse(tkn[3]);
                    
                    if (dataTkn.CompareTo(DateTime.Now) > 0)
                        message = tkn[1];
                    else
                        message ="EXPIRED";
                }
            }
            else { message = "ERROR_TOKEN"; }
            logger.Debug(message);
            return message;
        }
    }
}