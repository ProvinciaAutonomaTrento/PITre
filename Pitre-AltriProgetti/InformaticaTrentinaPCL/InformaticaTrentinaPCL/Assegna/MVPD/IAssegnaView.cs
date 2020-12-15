using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssegnaView : IGeneralView
    {
        void EnableButton(bool enabled);
        void OnAssegnaOk(Dictionary<string, string> extra);
        void ShowListaRagioni(List<Ragione> ragioni);
        void ShowSelectRagione(bool visible);
    }
}
