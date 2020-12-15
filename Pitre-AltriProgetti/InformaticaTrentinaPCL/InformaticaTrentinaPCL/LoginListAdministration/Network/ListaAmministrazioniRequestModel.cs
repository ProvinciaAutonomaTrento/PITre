using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Login.Network
{
    public class ListaAmministrazioniRequestModel : BaseRequestModel
    {
        public string userId { get; set; }

        public ListaAmministrazioniRequestModel(string userId)
        {
            this.userId = userId;
        }
    }
}
