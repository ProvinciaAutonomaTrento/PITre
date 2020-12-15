using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Responses
{
    public class GetFolderResponse
    {
        public Domain.Folder Folder { get; set; }
        public string ErrorMessage { get; set; }
        public GetFolderResponseCode Code { get; set; }
    }
    public enum GetFolderResponseCode { OK, SYSTEM_ERROR}
}