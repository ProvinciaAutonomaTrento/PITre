using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class DoRevokeResponseModel : BaseResponseModel
    {
        public bool revoked { get; set; } // Il servizio ritorna solo un bool, utilizzo questa per gestire lo stato 

        public DoRevokeResponseModel()
        {
        }
    }
}
