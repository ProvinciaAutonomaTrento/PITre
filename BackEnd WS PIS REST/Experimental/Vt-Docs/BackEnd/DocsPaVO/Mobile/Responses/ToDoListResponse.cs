using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class ToDoListResponse
    {
        public List<ToDoListElement> Elements
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get; 
            set;
        }

        public ToDoListResponseCode Code
        {
            get; 
            set;
        }

        public static ToDoListResponse ErrorResponse
        {
            get
            {
                ToDoListResponse resp = new ToDoListResponse();
                resp.Code = ToDoListResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum ToDoListResponseCode
    {
        OK,SYSTEM_ERROR
    }
}
