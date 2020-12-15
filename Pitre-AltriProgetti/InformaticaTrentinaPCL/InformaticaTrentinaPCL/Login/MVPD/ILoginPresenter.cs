using System;
using InformaticaTrentinaPCL.ChangePassword;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Login.MVP
{
  public interface ILoginPresenter : IBasePresenter
  {
    void LoginAsync(bool saveLogin);
    void UpdateUsername(string username);
    void UpdatePassword(string pwd);
    void OnViewReady();
    void SetUrlConstant(string url);
    void ChangeServer();
    string GetAppVersion();

    /// <summary>
    ///  Metodo che aggiorna lo stato dell'Amministrazione
    /// </summary>
    /// <param name="amministrazione">L'amministrazione corrente</param>
    /// <param name="state">Lo stato dell'Amministrazione</param>
    /// <param name="isUserAction">true se la modifica è stata generata da un'interazione utente, false se l'update è generato dal sistema (es. a seguito di un restore state di Android)</param>
    void UpdateAdministration(AmministrazioneModel amministrazione, LoginAdministrationState state, bool isUserAction = true);
  }
}