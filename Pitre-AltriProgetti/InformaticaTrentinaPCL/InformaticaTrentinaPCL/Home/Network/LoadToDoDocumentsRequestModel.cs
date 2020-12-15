using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.Network
{
    public class LoadToDoDocumentsRequestModel : BaseRequestModel
    {
        public Body body;

        public LoadToDoDocumentsRequestModel(Body body, string token)
        {
            this.body = body;
            this.token = token;
        }

        public class Body
        {
            public int page { get; set; }
            public int pageSize { get; set; }

            public Body(int page, int pageSize)
            {
                this.page = page;
                this.pageSize = pageSize;
            }
        }
    }

    public enum SectionType
    {
        ADL,
        SIGN,
        TODO,
        SWITCH,
        SEARCH
    }
}