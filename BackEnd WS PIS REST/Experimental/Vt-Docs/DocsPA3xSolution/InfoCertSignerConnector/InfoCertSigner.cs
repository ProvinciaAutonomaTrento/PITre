using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoCertSignerConnector
{
    public class InfoCertSigner
    {

        public static int CADESSingleSigner
        (
            String alias,
            String dominio,
            String pin,
            String tsaURL,
            String tsaUser,
            String tsaPassword,
            String userApplicativa,
            String passwordApplicativa,
            String pathFileDaFirmare,
            String pathFileFirmato,
            String serverURL
        )
        {
            int retVal = 0;

            InfoCertSignerConnector.InfoCertSignerService.SignerClient client = new InfoCertSignerService.SignerClient();
            
            retVal = client.CADESSingleSigner(alias, dominio, pin, tsaURL, tsaUser, tsaPassword, userApplicativa, passwordApplicativa, pathFileDaFirmare, pathFileFirmato, serverURL);

            return retVal;
        }

    }
}
