using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.DocsPaWR;

namespace NttDataWA.UIManager
{
    public class MultiCasellaManager
    {
        private static DocsPaWR.DocsPaWebService ws = ProxyManager.GetWS();

        #region layer interface to the backend
        public static CasellaRegistro[] GetMailRegistro(string idRegistro)
        {
            try
            {
                return ws.AmmGetMailRegistro(idRegistro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo UpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle)
        {
            try
            {
                return ws.AmmUpdateMailRegistro(idRegistro, caselle);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo InsertMailRegistro(string idRegistro, CasellaRegistro[] caselle, bool insertInteropInt)
        {
            try
            {
                return ws.AmmInsertMailRegistro(idRegistro, caselle, insertInteropInt);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo DeleteMailRegistro(string idRegistro, string casella)
        {
            try
            {
                return ws.AmmDeleteMailRegistro(idRegistro, casella);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static string GetMailPrincipaleRegistro(string idRegistro)
        {
            try
            {
                return ws.GetMailPrincipaleRegistro(idRegistro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo InsertRightMailRegistro(System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailRegistro)
        {
            try
            {
                return ws.AmmInsRightMailRegistro(rightRuoloMailRegistro.ToArray());
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo DeletelRightMailRegistro(string idRegistro, string idRuoloInAoo, string indirizzoEmail)
        {
            try
            {
                return ws.AmmDelRightMailRegistro(idRegistro, idRuoloInAoo, indirizzoEmail);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static ValidationResultInfo UpdateRightMailRegistro(RightRuoloMailRegistro[] visRuolo)
        {
            try
            {
                return ws.AmmUpdRightMailRegistro(visRuolo);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static DataSet GetRightMailRegistro(string idRegistro, string idRuoloInUO)
        {
            try
            {
                return ws.GetAmmRightMailRegistro(idRegistro, idRuoloInUO);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
        #endregion

        #region Methods management visibility

        /// <summary>
        /// 1- Attiva multi casella RF/REGISTRI, destinatari esterni
        /// Se questa chiave è a 0, non sarà possibile :
        /// FrontEnd: inserire più di una casella mail per i corrispondenti esterni nuovi o in modifica. 
        /// Amm.ne: non sarà possibile inserire più di una casella mail su Registri /RF.
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static bool IsEnabledMultiMail(string idAmm)
        {
            try
            {
                return ws.IsEnabledMultiMail(idAmm);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Return true se il ruolo ha cha_spedisci = 1:
        /// per destinatari interoperanti esterni su una o più delle caselle associate a registri/rf sui quali il ruolo ha visibilità. 
        /// per destinatari interoperanti interni
        /// </summary>
        /// <param name="page"></param>
        /// /// <param name="verifyDest"> "I" - check solo per destinatari interni, "E" - check solo per destinatariv esterni, "A" - check tutti</param>
        /// <returns></returns>
        public static bool RoleIsAuthorizedSend(Page page, string verifyDest)
        {
            try
            {
                DocsPaWebService ws = new DocsPaWebService();
                bool res = false;
                //prendo gli rf associati al ruolo 
                DocsPaWR.Registro[] rf = UserManager.getListaRegistriWithRF(page, "1", UserManager.getRegistroSelezionato(page).systemId);
                DocsPaWR.Registro[] registri = UserManager.getListaRegistriWithRF(page, "0", UserManager.getRegistroSelezionato(page).systemId);
                foreach (DocsPaWR.Registro registro in rf)
                {
                    DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.systemId, RoleManager.GetRoleInSession().systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        switch (verifyDest)
                        {
                            case "E":
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1") && !string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()))
                                        return res = true;
                                }
                                break;

                            case "I":
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1") && string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()) && ws.IsEnabledInteropInterna())
                                        return res = true;
                                }
                                break;
                            default:
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if ((row["SPEDISCI"].ToString().Equals("1") && !string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString())) ||
                                        ((row["SPEDISCI"].ToString().Equals("1") && string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()) && ws.IsEnabledInteropInterna())))
                                        return res = true;
                                }
                                break;
                        }
                    }
                }

                //prendo i registri associati al ruolo 

                foreach (DocsPaWR.Registro registro in registri)
                {
                    DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.systemId, RoleManager.GetRoleInSession().systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        switch (verifyDest)
                        {
                            case "E":
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1") && !string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()))
                                        return res = true;
                                }
                                break;

                            case "I":
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1") && string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()) && ws.IsEnabledInteropInterna())
                                        return res = true;
                                }
                                break;
                            default:
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if ((row["SPEDISCI"].ToString().Equals("1") && !string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString())) ||
                                        ((row["SPEDISCI"].ToString().Equals("1") && string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()) && ws.IsEnabledInteropInterna())))
                                        return res = true;
                                }
                                break;
                        }

                    }
                }
                return res;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Resistuisce l'elenco delle caselle associate al registro/rf per le quali il ruolo è abilitato alla consultazione
        /// </summary>
        /// <returns>List CasellaRegistro</returns>
        public static List<CasellaRegistro> GetComboRegisterConsult(string idRegistro)
        {
            try
            {
                List<CasellaRegistro> listCaselle = new List<CasellaRegistro>();
                string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(idRegistro);
                DataSet ds = MultiCasellaManager.GetRightMailRegistro(idRegistro, RoleManager.GetRoleInSession().systemId);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                            if (row["CONSULTA"].ToString().Equals("1"))
                            {
                                listCaselle.Add(new CasellaRegistro
                                {
                                    Principale = row["EMAIL_REGISTRO"].ToString().Equals(casellaPrincipale) ? "1" : "0",
                                    EmailRegistro = row["EMAIL_REGISTRO"].ToString(),
                                    Note = row["VAR_NOTE"].ToString()
                                });
                            }
                        }
                    }
                }
                return listCaselle;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return true se il ruolo ha cha_consulta a 1 sul registro/rf selezionato
        /// </summary>
        /// <param name="page"></param>
        /// <param name="idRegistro"></param>
        /// <param name="chaRF"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static bool RoleIsAuthorizedConsult(DocsPaWR.Registro registro, string idRuolo)
        {
            try
            {
                bool isEnable = true;
                //se ho selezionato un registro, ma ho visibilità di uno degli RF di quel registro, allora non devo poter fare check casella istit.
                if (GestManager.getRegistroSel().chaRF == "0")
                {
                    DocsPaWR.Registro[] regs = UserManager.getListaRegistriWithRF(idRuolo, "1", registro.systemId);
                    if (regs != null && regs.Length > 0)
                    {
                        for (int i = 0; i < regs.Length; i++)
                        {
                            if (MultiCasellaManager.RoleIsAuthorizedConsult(regs[i], idRuolo))
                            {

                                return !isEnable;
                            }
                        }

                    }
                }

                DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.systemId, idRuolo);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["CONSULTA"].ToString().Equals("1") && !string.IsNullOrEmpty(row["EMAIL_REGISTRO"].ToString()) && !row["EMAIL_REGISTRO"].ToString().Equals("&nbsp;"))
                                return isEnable;
                        }

                    }
                }
                // non ci sono caselle associate all'RF/registro o il ruolo non ha diritti di consultazione su alcuna casella di posta
                return !isEnable;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Return l'elenco dei registri/rf associati al ruolo, per i quali esiste almeno una casella sulla quale il ruolo è abilitato alla spedizione
        /// </summary>
        /// <returns>List</returns>
        public static List<DocsPaWR.Registro> GetRegisterEnabledSend(Page page)
        {
            try
            {
                List<DocsPaWR.Registro> listRegistriRF = new List<DocsPaWR.Registro>();
                //prendo gli rf associati al ruolo 
                DocsPaWR.Registro[] rf = UserManager.getListaRegistriWithRF(RoleManager.GetRoleInSession().systemId, "1", UserManager.getRegistroSelezionato(page).systemId);
                foreach (DocsPaWR.Registro registro in rf)
                {
                    DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.systemId, RoleManager.GetRoleInSession().systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                listRegistriRF.Add(registro);
                                break;
                            }
                        }
                    }
                }
                //prendo il registro corrente
                DataSet dsReg = MultiCasellaManager.GetRightMailRegistro(UserManager.getRegistroSelezionato(page).systemId, RoleManager.GetRoleInSession().systemId);
                if (dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                {
                    foreach (DataRow row in dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                    {
                        if (row["SPEDISCI"].ToString().Equals("1"))
                        {
                            listRegistriRF.Add(UserManager.getRegistroSelezionato(page));
                            break;
                        }
                    }
                }
                /*
                //prendo i registri associati al ruolo 
                DocsPaWR.Registro[] registri = UserManager.getListaRegistriWithRF(page, "0", UserManager.getRegistroSelezionato(page).systemId);
                foreach (DocsPaWR.Registro registro in registri)
                {
                    DataSet ds = MultiCasellaManager.GetRightMailRegistro(registro.systemId, RoleManager.GetRoleInSession().systemId);
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                listRegistriRF.Add(registro);
                                break;
                            }
                        }
                    }
                }*/
                return listRegistriRF;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Resistuisce l'elenco delle caselle associate al registro/rf per le quali il ruolo è abilitato in spedizione
        /// </summary>
        /// <returns>List CasellaRegistro</returns>
        public static List<CasellaRegistro> GetComboRegisterSend(string idRegistro)
        {
            try
            {
                List<CasellaRegistro> listCaselle = new List<CasellaRegistro>();
                string casellaPrincipale = MultiCasellaManager.GetMailPrincipaleRegistro(idRegistro);
                DataSet ds = MultiCasellaManager.GetRightMailRegistro(idRegistro, RoleManager.GetRoleInSession().systemId);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                        {
                            System.Text.StringBuilder formatMail = new System.Text.StringBuilder();
                            if (row["SPEDISCI"].ToString().Equals("1"))
                            {
                                listCaselle.Add(new CasellaRegistro
                                {
                                    Principale = row["EMAIL_REGISTRO"].ToString().Equals(casellaPrincipale) ? "1" : "0",
                                    EmailRegistro = row["EMAIL_REGISTRO"].ToString(),
                                    Note = row["VAR_NOTE"].ToString()
                                });
                            }
                        }
                    }
                }
                return listCaselle;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Return l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static DataSet GetAssDocAddress(string docNumber)
        {
            try
            {
                return ws.GetAssDocAddress(docNumber);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        /// <summary>
        /// Aggiorna l'associazione tra un documento (protocollato o predisposto alla protocollazione) e il mailAddress utilizzati per 
        /// l'invio della conferma di ricezione e la notifica di annullamento al mittente
        /// Solo per doc ricevuti per interoperabilità
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <param name="mailAddress"></param>
        /// <returns></returns>
        public static bool UpdateAssDocAddress(string docNumber, string idRegistro, string mailAddress)
        {
            try
            {
                return ws.UpdateAssDocAddress(docNumber, idRegistro, mailAddress);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        /// <summary>
        /// Elimina dal db l'associazione tra il documento e il mailAddress utilizzati per l'invio della conferma di ricezione e la notifica 
        /// di annullamento al mittente
        /// Invocato solo dopo l'eventuale annullamento ed invio della notifica di annullamento
        /// </summary>
        /// <param name="docNumber"></param>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static bool DeleteAssDocAddress(string docNumber, string idRegistro)
        {
            try
            {
                return ws.DeleteAssDocAddress(docNumber, idRegistro);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }
        #endregion

        #region Multi box corresponding external
        public static System.Collections.Generic.List<DocsPaWR.MailCorrispondente> GetMailCorrispondenteEsterno(string idCorrispondente)
        {
            try
            {
                List<DocsPaWR.MailCorrispondente> listCorr = ws.GetMailCorrEsterno(idCorrispondente).ToList();

                if (!IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
                {
                    MailCorrispondente corr = (from c in listCorr where c.Principale.Equals("1") select c).FirstOrDefault();
                    listCorr.Clear();
                    if (corr != null)
                        listCorr.Add(corr);
                }
                return listCorr;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }

        public static bool InsertMailCorrispondenteEsterno(System.Collections.Generic.List<DocsPaWR.MailCorrispondente> listCaselle, string idCorrispondente)
        {
            try
            {
                return ws.InsertMailCorrispondenteEsterno(listCaselle.ToArray(), idCorrispondente);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static bool DeleteMailCorrispondenteEsterno(string idCorrispondente)
        {
            try
            {
                bool res = true;
                if (!string.IsNullOrEmpty(idCorrispondente))
                    return ws.DeleteMailCorrispondenteEsterno(idCorrispondente);
                return res;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return false;
            }
        }

        public static List<DocsPaWR.MailCorrispondente> GetMailsAllCorrProto(int idDoc)
        {
            try
            {
                List<DocsPaWR.MailCorrispondente> listMailDest = ws.GetMailsAllCorrProto(idDoc).ToList();
                if (listMailDest != null && listMailDest.Count > 0)
                {
                    //seleziono solo la mail principale se non siamo abilitati al multi casella
                    if (!IsEnabledMultiMail(RoleManager.GetRoleInSession().idAmministrazione))
                    {
                        List<DocsPaWR.MailCorrispondente> listMailPrincDest = new List<MailCorrispondente>();
                        var corr = (from c in listMailDest where c.Principale.Equals("1") select c);
                        foreach (var mailPrincCorr in corr)
                        {
                            listMailPrincDest.Add(new MailCorrispondente()
                            {
                                systemId = mailPrincCorr.systemId,
                                Email = mailPrincCorr.Email,
                                Note = mailPrincCorr.Note,
                                Principale = mailPrincCorr.Principale
                            });
                        }
                        return listMailPrincDest;
                    }
                    return listMailDest;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        #endregion
        }
    }
}