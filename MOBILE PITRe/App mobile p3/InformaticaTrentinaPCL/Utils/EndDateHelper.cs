using System;
using InformaticaTrentinaPCL.Home;

namespace InformaticaTrentinaPCL.Utils
{
    public static class EndDateHelper
    {
        public static bool CheckVisibilityEndDate(DelegaDocumentModel documentDelegaModel)
        {
            return documentDelegaModel.dataScadenzaDelega == LocalizedString.UNDEFINED_END_DATE_MANDATE.Get();
        }
    }
}
