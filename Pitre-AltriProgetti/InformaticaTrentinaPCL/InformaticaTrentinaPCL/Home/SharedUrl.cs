using InformaticaTrentinaPCL.Network;

namespace InformaticaTrentinaPCL.Home
{
    public class SharedUrl
    {
        public readonly string ACTION = NetworkConstants.APP_BASE_SHARE_URL;
        
        public string baseUrl { get; }
        public string sharedKey { get; }
        public bool isValid { get; }
        
        public SharedUrl(string sharedUrl)
        {
            
            var urlSplit = (sharedUrl ?? "").Split('/');

            int baseUrlPosition = getBaseUrlPosition(urlSplit);
            
            isValid = baseUrlPosition > 0 && //ho un url valido
                      urlSplit.Length == baseUrlPosition + 3 && //ho tutti i segmenti
                      NetworkConstants.APP_URL_ACTION.Equals(urlSplit[baseUrlPosition + 1]); //ho il segmento share

            if (isValid)
            {
                baseUrl = urlSplit[baseUrlPosition];
                sharedKey = urlSplit[baseUrlPosition+2].Replace("%20","%2B");
            }
        }

        private int getBaseUrlPosition(string[] urlSplit)
        {
            int baseUrlPosition = 0;
            foreach (var segment in urlSplit)
            {
                if (NetworkConstants.APP_BASE_URL.Equals(segment))
                {
                    return baseUrlPosition;
                }
                baseUrlPosition++;
            }
            return -1;
        }
        
    }
}