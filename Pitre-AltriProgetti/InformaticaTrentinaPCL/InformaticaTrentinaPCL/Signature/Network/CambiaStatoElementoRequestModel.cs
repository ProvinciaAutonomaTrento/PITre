using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class CambiaStatoElementoRequestModel: BaseRequestModel
    {
        public List<SignDocumentNewStateModel> Elementi;
    }
}
