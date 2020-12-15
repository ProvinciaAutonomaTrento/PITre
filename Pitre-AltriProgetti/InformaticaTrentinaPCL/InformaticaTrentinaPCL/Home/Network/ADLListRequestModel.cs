using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class ADLListRequestModel : RicercaRequestModel
    {
        public ADLListRequestModel(Body body, string token) : base(body, token)
        {
        }
    }
}
