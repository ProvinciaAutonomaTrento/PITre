using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace DocsPaWS
{
    public static class WSManager
    {

        #region METODI DUPLICATI //duplicati


        #region utilità

        public static bool canaleWSAperto(DocsPaWS.DocsPaWebService ws)
        {
            if (ws == null)
                apriCanaleWS(out ws);
            return (ws != null);
        }
        public static void apriCanaleWS(out DocsPaWS.DocsPaWebService ws)
        {
            ws = new DocsPaWS.DocsPaWebService();
        }
        public static void chiudiCanaleWS(DocsPaWS.DocsPaWebService ws)
        {
            ws.Dispose();
        }
        #endregion


        public static void verificaUtente(DocsPaVO.utente.Utente Utente)
        {
            if (Utente == null)
                throw (new Exception("Utente non valido"));
        }

        public static void verificaRuolo(DocsPaVO.utente.Ruolo Ruolo)
        {
            if (Ruolo == null)
                throw (new Exception("oggetto Ruolo non valido"));
        }

        public static DocsPaVO.utente.InfoUtente getInfoUtenteAttuale(DocsPaVO.utente.Utente Utente, DocsPaVO.utente.Ruolo Ruolo)
        {
            verificaUtente(Utente);
            verificaRuolo(Ruolo);

            return getInfoUtente(Utente, Ruolo);
        }

        public static DocsPaVO.utente.InfoUtente getInfoUtente(DocsPaVO.utente.Utente utente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente();

            infoUtente.idCorrGlobali = ruolo.systemId;
            infoUtente.idPeople = utente.idPeople;
            infoUtente.idGruppo = ruolo.idGruppo;
            infoUtente.dst = utente.dst;
            infoUtente.idAmministrazione = utente.idAmministrazione;
            infoUtente.userId = utente.userId;

            return infoUtente;
        }

        public static bool Logoff(DocsPaVO.utente.Utente Utente, DocsPaWS.DocsPaWebService ws)
        {
            //			true  - Logoff effettuato
            //			false - errore
            bool retValue = false;

            try
            {

                verificaUtente(Utente);
                ws.Logoff(Utente.userId, Utente.idAmministrazione, Utente.sessionID, Utente.dst);
                retValue = true;
            }
            catch (System.SystemException ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                retValue = false;
            }

            return retValue;

        }


        public static string getStatoRegistro(DocsPaVO.utente.Registro reg)
        {
            /*retValue:
            R = Rosso -  CHIUSO
            V = Verde -  APERTO
            G = Giallo - APERTO IN GIALLO
            */
            string dataApertura = reg.dataApertura;

            if (!dataApertura.Equals(""))
            {

                DateTime dt_cor = DateTime.Now;

                CultureInfo ci = new CultureInfo("it-IT");

                string[] formati ={ "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy" };

                DateTime d_ap = DateTime.ParseExact(dataApertura, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);

                //aggiungo un giorno per fare il confronto con now (che comprende anche minuti e secondi)
                d_ap = d_ap.AddDays(1);

                string mydate = dt_cor.ToString(ci);

                string statoAperto = "A";
                if (reg.stato.Equals(statoAperto))
                {
                    if (dt_cor.CompareTo(d_ap) > 0)
                    {
                        //data odierna maggiore della data di apertura del registro
                        return "G";
                    }
                    else
                        return "V";
                }
            }
            return "R";

        }

        public static bool verificaRisposta(DocsPaVO.utente.UserLogin.LoginResult Lr, out System.Text.StringBuilder descErr)
        {
            descErr = new StringBuilder();
            bool retValue = false;
            switch (Lr)
            {
                case DocsPaVO.utente.UserLogin.LoginResult.OK:
                    {
                        descErr.Remove(0, descErr.Length);
                        retValue = true;
                        break;
                    }
                case DocsPaVO.utente.UserLogin.LoginResult.APPLICATION_ERROR:
                    {
                        descErr.Append(" Errore generico di applicazione.");
                        retValue = false;
                        break;
                    }
                case DocsPaVO.utente.UserLogin.LoginResult.DISABLED_USER:
                    {
                        descErr.Append(" Utente non abilitato.");
                        retValue = false;
                        break;
                    }
                case DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER:
                    {
                        descErr.Append(" Utente non abilitato.");
                        retValue = false;
                        break;
                    }
                case DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN:
                    {
                        descErr.Append(" Impossibile autenticare l'utente.");
                        retValue = false;
                        break;
                    }
                default:
                    {
                        descErr.AppendFormat(" Nessun dettaglio sull' errore {0}.", Lr.ToString());
                        retValue = false;
                        break;
                    }
                    return retValue;
            }
            return retValue;
        }

        public static bool HMDiritti(string accessRights, DocsPaVO.utente.Ruolo ruolo, string funzione)
        {
            bool funzAbilitato = false;
            funzAbilitato = verificaFunzionalita(ruolo, funzione);
            if (!funzAbilitato)
                return false;
            bool disabilita = false;
            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();
            if ((Convert.ToInt32(accessRights) < HMD.HMdiritti_Write)
                || (Convert.ToInt32(accessRights) < HMD.HMdiritti_Eredita))
            {
                disabilita = true;
                return disabilita;
            }
            return disabilita;
        }

        public static bool verificaFunzionalita(DocsPaVO.utente.Ruolo ruo, string codFunz)
        {
            if (codFunz == null || codFunz.Equals(""))
                return true;
            bool trovato = false;
            for (int i = 0; i < ruo.funzioni.Count; i++)
            {
                if (((DocsPaVO.utente.Funzione)ruo.funzioni[i]).codice.ToUpper() == codFunz.ToUpper())
                {
                    trovato = true;
                    break;
                }
            }
            return trovato;
        }

        public static bool ValidateEmail(string email)
        {
            string regex = @"^((\&quot;[^\&quot;\f\n\r\t\v\b]+\&quot;)|([\w\!\#\$\%\&amp;\'\*\+\-\~\/\^\`\|\{\}]+(\.[\w\!\#\$\%\&amp;\'\*\+\-\~\/\^\`\|\{\}]+)*))@((\[(((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9])))\])|(((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9]))\.((25[0-5])|(2[0-4][0-9])|([0-1]?[0-9]?[0-9])))|((([A-Za-z0-9\-])+\.)+[A-Za-z\-]+))$";
            System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);

            Match m = reg.Match(email);
            return m.Success;
        }

        public static bool insertCorrEsterno(string Codice, string Codrubrica,
                                        string Descrizione, string Tipo_urp,
                                        string Email, string Indirizzo, string Cap,
                                        string Citta, string Prov,
                                        string Cognome, string Nome,
                                        DocsPaVO.utente.InfoUtente infoUtente,
                                        string idRegistro,
                                        DocsPaWS.DocsPaWebService ws)
        {
            #region new
            DocsPaVO.utente.Corrispondente resultCorr = null;
            DocsPaVO.utente.Corrispondente newCorr = null;

            //DocsPaVO.utente.InfoUtente infoUtente = this.getInfoUtente(this.Utente, this.Ruolo);
            //string idRegistro = this.Registro.systemId;

            try
            {
                switch (Tipo_urp)
                {
                    case "U":
                        {
                            newCorr = new DocsPaVO.utente.UnitaOrganizzativa();
                            break;
                        }
                    case "P":
                        {
                            newCorr = new DocsPaVO.utente.Utente();
                            ((DocsPaVO.utente.Utente)newCorr).cognome = Cognome;
                            ((DocsPaVO.utente.Utente)newCorr).nome = Nome;
                            break;
                        }
                    default:
                        break;
                }

                newCorr.codiceCorrispondente = Codice;
                newCorr.codiceRubrica = Codrubrica;
                newCorr.descrizione = Descrizione;
                newCorr.tipoCorrispondente = Tipo_urp;
                newCorr.email = Email;

                #region dati costanti
                newCorr.idAmministrazione = infoUtente.idAmministrazione;
                newCorr.idRegistro = idRegistro;
                newCorr.codiceAmm = "";
                newCorr.codiceAOO = "";
                //dati canale
                DocsPaVO.utente.Canale canale = new DocsPaVO.utente.Canale();
                canale.systemId = "2"; //EMAIL
                newCorr.canalePref = canale;

                /*massimo digregorio: 
                 * necessari	sulla 2.0...
                 * sulla 3 non ho trovato nulla in riferimento, quindi non li gestisco
                                newCorr.tipoIE = "E";
                                newCorr.tipoCorrispondente = "S";
                */

                #endregion dati costanti

                //dati dettagli corrispondente
                DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                dettagli.Corrispondente.AddCorrispondenteRow(
                    Indirizzo,
                    Citta,
                    Cap,
                    Prov,
                    "", //nazionalita
                    "", //txt_telefono1
                    "", //txt_telefono2
                    "", //txt_fax
                    "", //txt_codfisc
                    "", //txt_note
                    "", //località
                    "", // luogoNascita
                    "", // dataNascita
                    "", // titolo
                    //"",// partita iva
                    "");// codice ipa
                newCorr.info = dettagli;
                newCorr.dettagli = true;

                //eseguo l'inserimento
                resultCorr = ws.AddressbookInsertCorrispondente(newCorr, null, infoUtente);
            }
            catch (Exception e)
            {
                throw new Exception("Errore in inserimento corrispondente esterno - DocsPaWSLite.asmx ERRORE: " + e.Message);
            }
            if (resultCorr != null && resultCorr.errore == null)
                return true;
            else
                return false;

            #endregion new
        }
        #endregion
    }
}

