using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class DelegaListRequestModel : BaseRequestModel
    {
        public string statoDelega = NetworkConstants.DEFAULT_STATO_DELEGA;
            

        public DelegaListRequestModel() { }

        public DelegaListRequestModel(string token)
        {
            this.token = token;
        }
    }
}
