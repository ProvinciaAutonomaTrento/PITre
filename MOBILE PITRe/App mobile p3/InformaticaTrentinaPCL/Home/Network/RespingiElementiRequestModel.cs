using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class RespingiElementiRequestModel : BaseRequestModel
    {
        public Body body;

        public RespingiElementiRequestModel(Body body) 
        {
            this.body = body;    
        }

        public class Body
        {
            
        }

    }
}
