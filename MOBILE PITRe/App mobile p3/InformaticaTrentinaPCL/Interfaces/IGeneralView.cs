using System;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Interfaces
{
    public interface IGeneralView
    {
        void ShowError(string e, bool isLight = false);
		void OnUpdateLoader(bool isShow);
    }
}
