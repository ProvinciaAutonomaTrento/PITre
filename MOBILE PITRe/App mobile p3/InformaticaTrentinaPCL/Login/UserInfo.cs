using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.ChangeRole;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Login
{
    //public class UserModel
    //{
    //    public string IDUser { get; set; }
    //    public string username { get; set; }
    //    public string name { get; set; }
    //    public string surname { get; set; }
    //    public string URLImageUser { get; set; }
    //    public string office { get; set; }
    //}

    public class UserInfo
    {
        [JsonProperty(PropertyName = "idPeople")]
        public string idPeople { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "Dst")]
        public string dst { get; set; }

        [JsonProperty(PropertyName = "Token")]
        public string token { get; set; }

        [JsonProperty(PropertyName = "Descrizione")]
        public string descrizione { get; set; }

        [JsonProperty(PropertyName = "Ruoli")]
        public List<RuoloInfo> ruoli { get; set; }

        [JsonProperty(PropertyName = "IdAmministrazione")]
        public string idAmministrazione { get; set; }

        [JsonProperty(PropertyName = "Utente")]
        private Utente utente { get; set; }

        // public InfoUtente delegato; su be c'e' questo per delegante
        [JsonProperty(PropertyName = "delegato")]
        public UserInfo delegato { get; set; }

        public string codiceFiscale
        {
            get
            {
                return utente.codiceFiscale;
            }
        }
        public string URLImageUser { get; set; }

        public List<RuoloInfo> ruoliFiltered
        {
            get
            {
                List<RuoloInfo> copy = new List<RuoloInfo>(ruoli);
                if (null != ruoli && ruoli.Count >= 1)
                {
                    copy.RemoveAt(0);
                    return copy;
                }
                else return ruoli;
            }
        }

        public class Utente
        {

            [JsonProperty(PropertyName = "codfisc")]
            public string codiceFiscale { get; set; }

        }

        public SignConfiguration signConfiguration = new SignConfiguration();
    }
}
