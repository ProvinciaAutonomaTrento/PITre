using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Config
{
    public interface IConfigServices
    {
        VtDocs.BusinessServices.Entities.Config.GetConfigResponse IsEnableMittentiMultipli(VtDocs.BusinessServices.Entities.Config.GetConfigRequest request);
    }
}
