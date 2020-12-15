using Android.App;
using Android.Content;
using Android.Net;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Droid.Utils.Reachability
{
    public class AndroidReachabilityManager:IReachability
    {
        private static AndroidReachabilityManager instance;

        private AndroidReachabilityManager()
        {
        }

        public static AndroidReachabilityManager GetInstance()
        {
            if (instance == null) instance = new AndroidReachabilityManager();
            return instance;
        }

        public MyStatusNetwork CheckNetwork()
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
            return networkInfo != null && networkInfo.IsConnectedOrConnecting
                ? MyStatusNetwork.Connected
                : MyStatusNetwork.Disconnect;
        
            
            /*if( networkInfo != null && networkInfo.IsConnected ){
                    string CheckUrl = "http://google.com";
                    try
                    {
                        HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);
                        iNetRequest.Timeout = 1000;
                        WebResponse iNetResponse = iNetRequest.GetResponse();
                        iNetResponse.Close();
                        return MyStatusNetwork.Connected;
                    }
                    catch (WebException ex)
                    {
                        return MyStatusNetwork.HostNotReachable;
                    }               
            }
            
            return MyStatusNetwork.Disconnect;
            */
        }


    }
}
