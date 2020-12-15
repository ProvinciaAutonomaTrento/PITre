using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class CambiaStatoElementiMultipliRequestModel : BaseRequestModel
    {
        public List<SignDocumentNewStateModel> Elementi;
    }

    public class SignDocumentNewStateModel {
        public string NuovoStato;
        public SignDocumentModel Elemento;
    }

}
