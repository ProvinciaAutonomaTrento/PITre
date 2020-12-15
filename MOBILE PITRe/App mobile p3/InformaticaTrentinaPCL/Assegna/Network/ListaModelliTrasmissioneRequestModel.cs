using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.Network
{
    public class ListaModelliTrasmissioneRequestModel : BaseRequestModel
    {
        public bool fascicoli;

        public ListaModelliTrasmissioneRequestModel(bool fascicoli, string token)
        {
            this.fascicoli = fascicoli;
            this.token = token;
        }
    }
}
