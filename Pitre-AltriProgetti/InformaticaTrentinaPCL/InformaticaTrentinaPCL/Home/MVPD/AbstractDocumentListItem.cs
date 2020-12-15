using System;
using System.Globalization;
using System.Text.RegularExpressions;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home.MVPD
{
    /// <summary>
    /// Identifica il tipo dell'elemento
    /// </summary>
    public enum TypeDocument
    {
        FASCICOLO, //IMMAGINE GIALLA
        DOCUMENTO,  //IMMAGINE VERDE
        ALL,
        NONE
    }

    public abstract class AbstractDocumentListItem
    {
        public Type InstanceType
        {
            get { return GetType(); }
            private set {}
        }

        [JsonProperty(PropertyName = "Tipo")]
        public int tipo { get; set; }

        [JsonIgnore]
        public bool rejectFlag { get; set; } = false;
        [JsonIgnore]
        public bool signFlag { get; set; } = false;

        public abstract string GetData();
        public abstract string GetMittente();
        public abstract string GetOggetto();
        public abstract string GetInfo();
        public abstract string GetEstensione();
 
        public abstract void SetFlags();

        public TypeDocument tipoDocumento
        {
            get
            {
                return this.tipo == 0 ? TypeDocument.DOCUMENTO : TypeDocument.FASCICOLO;

            }
        }

        public abstract string GetIdTrasmissione();
        public abstract string GetIdDocumento();
        public abstract string GetRagione();
        public abstract SignatureInfo getSignatureInfo();
        public abstract string getIdEvento();

        public string ConvertDateFormatIfNeeded(string date)
        {
            if (DateStringShouldBeCOnverted(date))
            {
                return ConvertDateToFormat(date);
            }

            return date;
        }

        private bool DateStringShouldBeCOnverted(string date)
        {
            string regex = "[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}";
            Regex rgx = new Regex(regex);

            return rgx.IsMatch(date);
        }

        /// <summary>
        /// Convert a string from the format yyyy-MM-ddTHH:mm:ss in the format dd/MM/yyyy HH:mm
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string ConvertDateToFormat(string date)
        {
            DateTime d = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

            return d.ToString("dd/MM/yyyy HH:mm");
        }
    }

    public class SignatureInfo
    {
        public string value { get; }
        public bool isSignature { get; }

        private SignatureInfo()
        {
            value = "";
            isSignature = false;
        }
        
        private SignatureInfo(string value, bool isSignature)
        {
            this.value = value;
            this.isSignature = isSignature;
        }

        public static SignatureInfo Create(string signature, string id)
        {
            if (string.IsNullOrEmpty(signature))
            {
                return new SignatureInfo(id, false);
            }
            string clearSignature = Regex.Replace(signature, "<.*?>", String.Empty);
            return new SignatureInfo(clearSignature, true);
        }

        public static SignatureInfo Create()
        {
            return new SignatureInfo();
        }
    }
}
