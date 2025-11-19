// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Interoperabilita;
public class Notifica
{
    private static ILogger logger = Log.ForContext(typeof(Notifica));

    public static bool notificaByMail(string destAddress, string mittAddress, string subject, string body, string priority, string idAmm, CMAttachment[] attachments)
    {
        bool ret = true;
        System.Data.DataSet ds;
        SvrPosta svr = null;

        try
        {
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            obj.getSmtp(out ds, idAmm);

            if (ds.Tables["SERVER"].Rows.Count == 0)
            {
                ret = false;
                return ret;
            }

            //				string server = ds.Tables["SERVER"].Rows[0]["VAR_SMTP"].ToString();
            //				string smtp_user = ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].ToString();
            //				string smtp_pwd = ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].ToString();
            string smtp_user = "";
            string smtp_pwd = "";
            string server = ds.Tables["SERVER"].Rows[0]["VAR_SMTP"].ToString();
            if (!ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].Equals(System.DBNull.Value))
                smtp_user = ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].ToString();
            if (!ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].Equals(System.DBNull.Value))
                smtp_pwd = DocsPaUtils.Security.Crypter.Decode(ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].ToString(), smtp_user);

            string port = ds.Tables["SERVER"].Rows[0]["NUM_PORTA_SMTP"].ToString();
            string SmtpSsl = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_SSL"].ToString();
            string SmtpSTA = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_STA"].ToString();

            svr = new SvrPosta(server,
                smtp_user,
                smtp_pwd,
                port,
                Path.GetTempPath(),
              CMClientType.SMTP, SmtpSsl, "", SmtpSTA);

            svr.connect();

            svr.sendMail(mittAddress, destAddress, subject, body, attachments);
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
            ret = false;
        }
        finally
        {
            if (svr != null)
                svr.disconnect();
        } 
        return ret;
    }
}
