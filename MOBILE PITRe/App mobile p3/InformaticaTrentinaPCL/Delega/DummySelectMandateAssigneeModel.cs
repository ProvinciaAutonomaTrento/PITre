using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Delega.Network;
using TestRefit.Network;

namespace InformaticaTrentinaPCL.Delega
{
    public class DummySelectMandateAssigneeModel : WS, ISelectMandateAssigneeModel
    {
        public DummySelectMandateAssigneeModel()
        {
        }

        public async Task<FavoritesResponseModel> GetListFavorites(FavoritesRequestModel request)
        {
            FavoritesResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new FavoritesResponseModel();
                response.Code = 0;
                return response;
            }
            catch (Exception e)
            {
                response = new FavoritesResponseModel();
                ResolveError(response, e);
            }
            return response;
        }

        public async Task<SearchMandateAssigneeResponseModel> SearchAssignee(SearchMandateAssigneeRequestModel request)
        {
            SearchMandateAssigneeResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new SearchMandateAssigneeResponseModel();
                response.Risultati = this.GenerateSearchResults();
                response.Code = 0;
                return response;
            }
            catch (Exception e)
            {
                response = new SearchMandateAssigneeResponseModel();
                ResolveError(response, e);
            }
            return response;

        }

        public List<SearchMandateAssignee> GenerateSearchResults(){

            return new List<SearchMandateAssignee>()
            {
                new SearchMandateAssignee("descrizione 1","dst 1","id amministrazione 1","id people 1",
                                          new List<ChangeRole.RuoloInfo>(){new ChangeRole.RuoloInfo("id ruolo 1","code ruolo 1","code desc 1")},"token 1","user id 1", false),
                new SearchMandateAssignee("descrizione 2", "dst 2", "id amministrazione 2", "id people ", 
				                                     new List<ChangeRole.RuoloInfo>() { new ChangeRole.RuoloInfo("id ruolo 2", "code ruolo ", "code desc 2") }, "token 2", "user id 2", true),
                new SearchMandateAssignee("descrizione 3", "dst 3", "id amministrazione 3", "id people 3",
                                                     new List<ChangeRole.RuoloInfo>() { new ChangeRole.RuoloInfo("id ruolo 3", "code ruolo 3", "code desc 3") }, "token 3", "user id 3",false)
            };
        }

        public async Task<SetFavoriteResponseModel> SetFavorite(SetFavoriteRequestModel request)
        {
            SetFavoriteResponseModel response;
            try
            {
                await Task.Delay(1000);
                response = new SetFavoriteResponseModel();
                response.success = true;
                response.Code = 0;
                return response;
            }
            catch (Exception e)
            {
                response = new SetFavoriteResponseModel();
                ResolveError(response, e);
            }
            return response;
        }
    }
}
