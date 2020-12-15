using System;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.ChooseInstance
{
  public class ChooseInstancePresenter : IChooseInstancePresenter
  {
    IChooseModel model;
    IChooseInstanceView view;
    SessionData sessionData;
    IReachability reachability;
    InstanceModel currentPreferredInstance;

    public ChooseInstancePresenter(IChooseInstanceView view, INativeFactory nativeFactory)
    {
      this.view = view;
      this.sessionData = nativeFactory.GetSessionData();
      this.reachability = nativeFactory.GetReachability();
      this.model = new ChooseInstanceModel();
    }

    public void Dispose()
    {
      model.Dispose();
    }

    /// <summary>
    /// Updates the list instance.
    /// </summary>
    public async void UpdateListInstance()
    {
      view.OnUpdateLoader(true);
      ListInstanceResponseModel response = await model.GetListInstance();
      view.OnUpdateLoader(false);
      this.ManageResponse(response);
    }

    /// <summary>
    /// Sets the instance prefered.
    /// </summary>
    /// <param name="currentPreferredInstance">Current prefered instance.</param>
    public void SetInstancePreferred(InstanceModel currentPreferredInstance)
    {
      if (currentPreferredInstance.url.Contains("http://") || currentPreferredInstance.url.Contains("https://"))
      {
        SaveInstanceAndUrl(currentPreferredInstance);
      }
      else
      {
        view.ShowError(LocalizedString.MESSAGE_URL_NOT_VALID.Get());
      }
    }

    /// <summary>
    /// Saves the instance and URL.
    /// </summary>
    /// <param name="currentPreferredInstance">Current prefered instance.</param>
    private void SaveInstanceAndUrl(InstanceModel currentPreferredInstance)
    {
      this.currentPreferredInstance = currentPreferredInstance;
      view.SavePreferredInstance(currentPreferredInstance.descrizione, currentPreferredInstance.url);
    }

    /// <summary>
    /// Opens the view login.
    /// </summary>
    public void OpenViewLogin()
    {
      view.OpenLoginView();
    }

    private void ManageResponse(ListInstanceResponseModel response)
    {
      if (response.IsCancelled || !response.IsSuccessStatusCode)
      {
        return;
      }

      switch (response.Code)
      {
        case 0:
          UpdateView(response);
          break;
        default:
          view.ShowError(LocalizedString.GENERIC_ERROR.Get()+"  "+ response.Code ); 
          break;
      }
    }

    private void UpdateView(ListInstanceResponseModel response)
    {
      view.OnUpdateLoader(false);
      if (response.instances != null && response.instances.Count > 0)
      {
#if DEBUG
        //InstanceModel im = new InstanceModel();
        //im.descrizione = "XXX";
        //im.url = "https://mobile-qual.pitre.tn.it/pat";
        //im.tipo = "TOP";
        //response.instances.Add(im);
#endif
                view.UpdateList(response.instances);
      }
      else
      {
        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
      }
    }
  }
}