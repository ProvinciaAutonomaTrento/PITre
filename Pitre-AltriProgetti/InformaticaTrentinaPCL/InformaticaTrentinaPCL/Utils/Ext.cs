using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Resources;

namespace InformaticaTrentinaPCL.Utils
{
    public static class Ext
    {
        public static string ToReadableString(this DateTime res)
		{
            return res.ToString("dd/MM/yyyy");
        }

        public static string ToReadableTimeString(this DateTime res)
        {
            return res.ToString("dd/MM/yyyy HH:mm");
        }

        public static bool IsSet(this DateTime datetime){
            return datetime != default(DateTime);
        }

        public static string Get(this LocalizedString sEnum)
        {
            return StringManager.Instance.GetString(sEnum);
        }
        
        public static string Capitalize(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("ARGH!");
            }
            return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
        }

        /// <summary>
        /// Creates the string for thank you page.
        /// </summary>
        /// <returns>The string for thank you page.</returns>
        /// <param name="totalRecordCountRejected">Total record count rejected.</param>
        /// <param name="totalRecordCountSigned">Total record count signed.</param>
        /// <param name="totalRecordCountSignedWithOtp">Total record count signed with otp.</param>
        public static Dictionary<string, string> CreateStringForThankYouPage(int totalRecordCountRejected, int totalRecordCountSigned, int totalRecordCountSignedWithOtp)
        {
            Dictionary<string, string> extra = new Dictionary<string, string>();
            extra.Add(Constants.DESCRIPTION_THANK_YOU_PAGE, string.Empty);
            return extra;
            //return CreateDescription(totalRecordCountRejected, totalRecordCountSigned, totalRecordCountSignedWithOtp);
        }

        public static Dictionary<string, string> CreateStringForThankYouPage(List<AbstractDocumentListItem> list)
        {
            Dictionary<string, string> extra = new Dictionary<string, string>();
            extra.Add(list.First().tipoDocumento.ToString(), CreateDocumentTypeDescription(list));
            AddKeyForDocumentTYpeDescription(extra, list.First());
            
            foreach(AbstractDocumentListItem item in list)
            {
                extra.Add(item.GetIdDocumento(), item.GetOggetto());
            }
            return extra;
        }

        private static void AddKeyForDocumentTYpeDescription(Dictionary<string, string> extra, AbstractDocumentListItem document)
        {
            if (document.tipoDocumento == TypeDocument.DOCUMENTO)
            {
                extra.Add(Constants.DOCUMENT_TYPE_DECRIPTION_KEY, LocalizedString.DOCUMENTO.Get());
            }
            else if (document.tipoDocumento == TypeDocument.FASCICOLO)
            {
                extra.Add(Constants.DOCUMENT_TYPE_DECRIPTION_KEY, LocalizedString.FASCICOLO.Get());
            }
            else
            {
                extra.Add(Constants.DOCUMENT_TYPE_DECRIPTION_KEY, "");
            }
        }

        public static Dictionary<string, string> CreateStringForThankYouPage(AbstractDocumentListItem item)
        {
            Dictionary<string, string> extra = new Dictionary<string, string>();
            List<AbstractDocumentListItem> ll = new List<AbstractDocumentListItem>(1);
            ll.Add(item);
            extra.Add(item.tipoDocumento.ToString(), CreateDocumentTypeDescription(ll));
            extra.Add(item.GetIdDocumento(), item.GetOggetto());
            AddKeyForDocumentTYpeDescription(extra, item);
            return extra;
        }

        private static Dictionary<string, string> CreateDescription(int totalRecordCountRejected, int totalRecordCountSigned, int totalRecordCountSignedWithOtp)
        {
            Dictionary<string, string> extra = new Dictionary<string, string>();

            string descriptionCenter = string.Empty;
            if (totalRecordCountSignedWithOtp > 0 && totalRecordCountSigned > 0)
            {
                string descriptionSign = String.Format(LocalizedString.DESCRIPTION_SIGN_COMPLETED.Get(), totalRecordCountSigned);
                string descriptionSignWithOtp = String.Format(LocalizedString.DESCRIPTION_SIGN_COMPLETED_AND_SIGN_WITH_OTP.Get(), totalRecordCountSignedWithOtp);
                descriptionCenter = descriptionSign + descriptionSignWithOtp;
            }
            else if (totalRecordCountSigned > 0)
            {
                descriptionCenter = String.Format(LocalizedString.DESCRIPTION_SIGN_COMPLETED.Get(), totalRecordCountSigned);
            }
            else if (totalRecordCountRejected > 0)
            {
                descriptionCenter = String.Format(LocalizedString.DESCRIPTION_REJECT_COMPLETED.Get(), totalRecordCountRejected);
            }
            else if (totalRecordCountSignedWithOtp > 0)
            {
                descriptionCenter = String.Format(LocalizedString.DESCRIPTION_SIGN_COMPLETED_WITH_OTP.Get(), totalRecordCountSignedWithOtp);
            }

            extra.Add(Constants.DESCRIPTION_THANK_YOU_PAGE, descriptionCenter);

            return extra;
        }

        /// <summary>
        /// Creates the string of number documents.
        /// </summary>
        /// <returns>The string of number documents.</returns>
        /// <param name="totalRecordCountToSignCades">Total record count to sign cades.</param>
        /// <param name="totalRecordCountToSignPades">Total record count to sign pades.</param>
        /// <param name="totalRecordCountToSign">Total record count to sign.</param>
        /// <param name="totalRecordCountToReject">Total record count to reject.</param>
        public static string CreateStringOfNumberDocuments(int totalRecordCount, string message)
        {
            return CreateDescriptionWithNumberDocuments(totalRecordCount, message);
        }

        private static string CreateDescriptionWithNumberDocuments(int totalRecordCount, string message)
        {
            string description = string.Empty;
            bool isOneDocument = false;

            isOneDocument = totalRecordCount == 1 ? true : false;
            description = string.Format(message, totalRecordCount, isOneDocument ? "o" : "i");

            return description;
        }

        private static string CreateDocumentTypeDescription(List<AbstractDocumentListItem> items)
        {
            var doc = items.First();
            if (doc.tipoDocumento == TypeDocument.DOCUMENTO)
            {
                return CreateDocumentDescription(items.Count > 1);
            }
            if (doc.tipoDocumento == TypeDocument.FASCICOLO)
            {
                return CreateFascicoloDescription(items.Count > 1);
            }

            return "";
        }

        private static string CreateDocumentDescription(bool plural)
        {
            return plural ? LocalizedString.DOCUMENTS.Get() : LocalizedString.DOCUMENT.Get(); 
        }
        
        private static string CreateFascicoloDescription(bool plural)
        {
            return plural ? LocalizedString.FILES.Get() : LocalizedString.FILE.Get();
        }
    }
}
