using System;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.iOS.Helper
{
    public class IosReachabilityManager:IReachability
    {
        static private IosReachabilityManager instance;

        public static IosReachabilityManager Instance()
        {
            if (instance == null)
            {
                instance = new IosReachabilityManager();
            }

            return instance;
        }

        private IosReachabilityManager()
        {
            
        }

        public MyStatusNetwork CheckNetwork()
        {
            var test  = Reachability.Reachability.InternetConnectionStatus();

            if (test == Reachability.NetworkStatus.NotReachable)
                return MyStatusNetwork.Disconnect;
            if  (test == Reachability.NetworkStatus.ReachableViaCarrierDataNetwork || test == Reachability.NetworkStatus.ReachableViaWiFiNetwork)
            {
                Reachability.Reachability.HostName = "www.google.com";
                var testHost = Reachability.Reachability.RemoteHostStatus();
                if (testHost == Reachability.NetworkStatus.NotReachable)
                    return MyStatusNetwork.HostNotReachable;
                else
                    return MyStatusNetwork.Connected;

            }

            return MyStatusNetwork.Disconnect;
        }
    }
}
