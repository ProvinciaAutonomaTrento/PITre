using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace HystoryBrowser
{
    public class history
    {
        public IDictionary saveHistoryBrowser(string urlVisited)
        {
            //Create a new collection
            IDictionary history = (IDictionary)System.Web.HttpContext.Current.Session["history"];

            //Initialize the Session if is null
            if (history == null)
            {
                history = new Dictionary<int, string>();
                System.Web.HttpContext.Current.Session["history"] = history;
            }

            if (urlVisited != null)
            {
                add_to_history(urlVisited, history.Count + 1);
            }

            return history;
        }

        public void add_to_history(string item, int value)
        {
            //Method to add the url visited in the collection/session
            IDictionary history = (IDictionary)System.Web.HttpContext.Current.Session["history"];
            history.Add(value, item);
        }
    }
}