using System;
namespace InformaticaTrentinaPCL.Interfaces
{
    public enum MyStatusNetwork
    {
        Connected,
        Disconnect,
        HostNotReachable
    }
    
    public interface IReachability
    {
        /// <summary>
        /// ios : Chi implementa questo metodo deve utilizzare per ios la classe Reachability per recuperare le informazioni sulla rete : Connesso , non connesso , 3g , wifi
        /// Android : Chi implementa questo metodo deve utilizzare per ios la classe ConnectivityManager per recuperare le informazioni sulla rete : Connesso , non connesso , 3g , wifi
        /// Android guida : https://developer.xamarin.com/recipes/android/networking/detect-network-connection/
        /// Ios guida : https://developer.xamarin.com/recipes/ios/network/
        /// </summary>
        /// <returns><c>true</c>, if connected was ised, <c>false</c> otherwise.</returns>
        MyStatusNetwork CheckNetwork();
    }
}
