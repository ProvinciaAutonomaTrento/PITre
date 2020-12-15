using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Interfaces;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Delega.Network
{
    public class DoRevokeRequestModel : BaseRequestModel
    {
        public DelegaDocumentModel body;

        public DoRevokeRequestModel(DelegaDocumentModel body, string token)
        {
            this.body = body;
            this.token = token;
        }
    }
}
