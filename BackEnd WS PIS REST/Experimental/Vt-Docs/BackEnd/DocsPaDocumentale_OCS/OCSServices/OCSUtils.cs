using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Globalization;
using System.Xml;
using DocsPaVO;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaDocumentale_OCS.DocsPaServices;
using DocsPaDocumentale_OCS.CorteContentServices;

namespace DocsPaDocumentale_OCS.OCSServices
{
    /// <summary>
    /// 
    /// </summary>
    public class OCSUtils
    {
        /// <summary>
        /// Verifica se l'esito della chiamata ad un servizio OCS sia valido o meno
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool isValidServiceResult(ResultType result)
        {
            return (result.code == OCSResultTypeCodes.SUCCESSFULL);
        }

        /// <summary>
        /// Solleva un'eccezione OCS nel caso in cui il valore di ritorno
        /// di un servizio sia non valido
        /// </summary>
        /// <param name="result"></param>
        public static void throwExceptionIfInvalidResult(ResultType result)
        {
            if (!isValidServiceResult(result))
                throw new OCSException(result);
        }

        /// <summary>
        /// Creazione di un oggetto MetadataType
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MetadataType getMetadataItem(string name, params string[] value)
        {
            MetadataType item = new MetadataType();
            item.name = name;
            item.value = value;
            return item;
        }

        /// <summary>
        /// Reperimento credenziali OCS da superutente
        /// </summary>
        /// <returns></returns>
        public static UserCredentialsType getApplicationUserCredentials()
        {
            UserCredentialsType userCred = new UserCredentialsType();
            userCred.userId = OCSConfigurations.GetSuperUser();
            userCred.password = OCSConfigurations.GetSuperUserPwd();
            return userCred;
        }

        /// <summary>
        /// Reperimento credenziali OCS per l'utente richiesto
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static UserCredentialsType getUserCredentials(InfoUtente infoUtente)
        {
            CorteContentServices.UserCredentialsType userCred = new UserCredentialsType();
            string decryptedToken = OCSServices.OCSTokenHelper.Decrypt(infoUtente.dst);

            // Creazione oggetto RepositoryIdentity
            if (!string.IsNullOrEmpty(decryptedToken))
            {
                string[] items = decryptedToken.Split('|');

                if (items.Length == 2)
                {
                    userCred.userId = items[0];
                    userCred.password = items[1];
                }
                    
            }          
            return userCred;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thing"></param>
        /// <returns></returns>
        public static CorteContentServices.GrantsType getAclSecurity(string thing)
        {
            // Reperimento metadati in tabella Security di DocsPa
            List<UserGrantType> userGrantList = new List<UserGrantType>();
            List<GroupGrantType> groupGrantList = new List<GroupGrantType>();

            CorteContentServices.UserGrantType userGrant;
            CorteContentServices.GroupGrantType groupGrant;

            CorteContentServices.GrantsType grants = new CorteContentServices.GrantsType();

            foreach (DocsPaQueryHelper.DocsPaSecurityItem securityItem in DocsPaQueryHelper.getSecurityItems(thing))
            {
                if (securityItem.IsPeople)
                {
                    userGrant = new CorteContentServices.UserGrantType();
                    userGrant.userId = securityItem.CodiceRubrica;
                    userGrant.accessType = DocsPaObjectType.ObjectTypes.ROLE_READER;
                    userGrantList.Add(userGrant);
                }
                else
                {
                    groupGrant = new CorteContentServices.GroupGrantType();
                    groupGrant.name = securityItem.CodiceRubrica;
                    groupGrant.accessType = DocsPaObjectType.ObjectTypes.ROLE_READER;
                    groupGrantList.Add(groupGrant);
                }
            }
            if (userGrantList.Count > 0)
                grants.usersGrant = userGrantList.ToArray();

            if (groupGrantList.Count > 0)
                grants.groupsGrant = groupGrantList.ToArray();

            return grants;
        }

        /// <summary>
        /// formattazione delle date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime getOCSDateFormat(string date)
        {
            return DateTime.Parse(date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string getOCSDateStringFormat(string date)
        {
            DateTime ocsDate;
            string stringDate = "";
            CultureInfo ci = new CultureInfo("it-IT");
            string[] formati = { "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
            string format = "yyyy-MM-ddTHH:mm:ss";
            ocsDate = DateTime.ParseExact(date, formati, ci.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
            stringDate = ocsDate.ToString(format).Replace(".", ":");
            return stringDate;
        }
    }
}
