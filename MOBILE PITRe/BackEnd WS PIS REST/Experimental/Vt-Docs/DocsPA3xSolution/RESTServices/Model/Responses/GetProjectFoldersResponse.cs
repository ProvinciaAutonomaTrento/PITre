using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetProjectFoldersResponse
    {
        public string IdProject { get; set; }

        public string ProjectDescription { get; set; }

        public Domain.Folder[] Folders { get; set; }

        public string ErrorMessage { get; set; }
        public GetProjectFoldersResponseCode Code { get; set; }
    }
    public enum GetProjectFoldersResponseCode { OK, SYSTEM_ERROR }
}