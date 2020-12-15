<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CapicomWrapper.ascx.cs" Inherits="DocsPAWA.ActivexWrappers.CapicomWrapper" %>
<% if (this.UseActivexWrapper && !this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "CapicomWrapperObject")) 
   { %> 
<OBJECT CLASSID="CLSID:1A9DD38C-5584-4E46-88B1-41D70ECC2CCD" CODEBASE="../activex/DocsPa_ActivexWrappers.CAB#version=3,6,0,0" height="0" width="0"></OBJECT> 
<% 
    this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CapicomWrapperObject", string.Empty);
   } %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "capicomWrapperControlScript"))
   { %>
<script id="capicomWrapperControlScript" type="text/javascript">
function CapicomWrapper_GetCertificateList(storeLocation, storeName)
{
    try
    {
        var retValue = new Array();
        
        if ("<%=UseActivexWrapper%>" == "True")
        {
            var capicomWrapper = new ActiveXObject("DocsPa_ActivexWrappers.CapicomWrapper");

            var list = null;

            try {
                // Reperimento dei soli certificati di tipo non ripudio
                list = capicomWrapper.GetCertificateList2(storeLocation, storeName);
            }
            catch (ex) {
                // In caso di errore (ovvero se la nuova versione dell'activex non risulta installata)
                // viene richiamato il vecchio metodo che reperisce tutti i certicati installati nello store locale
                list = capicomWrapper.GetCertificateList(storeLocation, storeName);
            }

            var e = new Enumerator(list);
            var i = 0;
            for (; !e.atEnd(); e.moveNext())	
            {
                retValue[i] = e.item();
                i++;
            }   
            
            capicomWrapper = null;         
        }
	    else
	    {
	        var store = new ActiveXObject("CAPICOM.Store");	
	        store.Open(storeLocation, storeName, 0);

	        for(i=1; i<=store.Certificates.Count; i++)
		        retValue[i] = store.Certificates(i);
		        
	        store = null;
        }
        
        return retValue;
	}
	catch(e)
	{
	    alert("Errore nel reperimento della lista dei certificati residenti nello store '" + storeName + "'");
	}
}

function CapicomWrapper_SignData(content, certIndex, applyCoSign, storeLocation, storeName)
{
    var signedData = null;
    
    try
    {
        if ("<%=UseActivexWrapper%>" == "True")
        {
            var capicomWrapper=new ActiveXObject("DocsPa_ActivexWrappers.CapicomWrapper");
            signedData=capicomWrapper.SignData(content, certIndex, applyCoSign, storeLocation, storeName);
            capicomWrapper=null;
        }
	    else
	    {
	        var store=new ActiveXObject("CAPICOM.Store");	
            store.Open(storeLocation, storeName, 0);
			var cert=store.certificates(certIndex);

			var signer=new ActiveXObject("CAPICOM.Signer");
			signer.Certificate=cert;

			var sd=new ActiveXObject("CAPICOM.SignedData");
			sd.content = content;

		    if (!applyCoSign)
			{
			    signedData=sd.Sign(signer, false, 0);
			}
			else
			{
			    sd.Verify(sd.content, false, 1);
				signedData=sd.CoSign(signer, 0);
		    }
	    }
	}
	catch(e)
	{
	    alert(e.message.toString());
	} 
	
	return signedData;
}
</script>
<% 
      this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "capicomWrapperControlScript", string.Empty); 
  } %>