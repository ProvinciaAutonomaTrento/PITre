using System;

namespace BusinessLogic.Interoperabilità
{
    /// <summary>
    /// Summary description for IMailConnector.
    /// </summary>
    public interface IMailConnector
    {
        CMMsg getMessage(int index);
        CMMsg getMessage(byte[] email);
        
        string getBodyFromMail(string email);
        void deleteSingleMessage(int i);
        void connect();
        void disconnect();
        int messageCount();

        void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments, CMMailHeaders[] headers, out string outError);
        void sendMail(string sFrom, string sTo, string sCC, string sBCC, string sSubject, string sBody, CMMailFormat format, CMAttachment[] attachments,out string outError);
        void sendMail(string sFrom, string sTo, string sSubject, string sBody, CMAttachment[] attachments);
        void sendMail(string sFrom, string sTo, string sSubject, string sBody);
        
        bool moveImap(int index, bool elaborata);
        bool cancellaMailImap();

        bool provaConnessione(DocsPaVO.amministrazione.OrgRegistro.MailRegistro mailRegistro, out string errore, string tipoConnessione);
        bool getMessagePec(int index);
        bool salvaMailInLocale(int indexMail, string pathFile, string NomeDellaMail);

        CMMsg getMessage(string uidl);
        string[] getUidls();
        bool salvaMailInLocale(string uidl, string pathFile, string NomeDellaMail);
        void deleteSingleMessage(string uidl);
        bool getMessagePec(string uidl);
        bool moveImap(string uid, bool elaborata);
    }
}
