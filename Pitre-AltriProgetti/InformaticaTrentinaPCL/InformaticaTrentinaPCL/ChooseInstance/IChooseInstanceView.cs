using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;

namespace InformaticaTrentinaPCL.ChooseInstance
{
    public interface IChooseInstanceView : IGeneralView
    {
        void UpdateList(List<InstanceModel> list);
        void SavePreferredInstance(string description, string url);
        void OpenLoginView();
    }
}
