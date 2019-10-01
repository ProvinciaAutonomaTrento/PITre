<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DigitalSignWrapper.ascx.cs" Inherits="NttDataWA.SmartClient.DigitalSignWrapper" %>

<form name="frm">
        <OBJECT id="digitalSignServicesControl" height="0" width="0" 
                data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
                classid="../SmartClient/Librerie/DPA.Web.dll#DPA.Web.DigitalSignature.DigitalSignServicesControl"
                VIEWASTEXT>			
        </OBJECT>    
</form>

<script type="text/javascript">
    function CapicomWrapper_GetCertificateList(storeLocation, storeName) {
        try 
        {
            if (storeLocation == 2) {
                storeLocation = 1;
            }
            var retValue = new Array();

            var ctrl = document.getElementById("digitalSignServicesControl");
            ctrl.CheckCertificate = false;

            var list = ctrl.GetCertificateListAsJsonFormat(storeLocation, storeName);
            var jsonList = eval(list);

            for (var i = 0; i < jsonList.length; i++) {
                retValue[i] = jsonList[i];
            }
        
            return retValue;
	    }
	    catch(e)
	    {
	        //alert("Errore nel reperimento della lista dei certificati residenti nello store '" + storeName + "'\n" + e);
	        alert("Errore nel reperimento della lista dei certificati residenti nello store '" + storeName + "'");
	    }
    }

    function pack(bytes) {
        var chars = [];
        for (var i = 0, n = bytes.length; i < n; ) {
            chars.push(((bytes[i++] & 0xff) << 8) | (bytes[i++] & 0xff));
        }
        return String.fromCharCode.apply(null, chars);
    }

    function CapicomWrapper_SignData(content, certIndex, applyCoSign, storeLocation, storeName) {
        var signedData = null;
        certIndex = certIndex - 1;
        try
        {
            var ctrl = document.getElementById("digitalSignServicesControl");
            if (storeLocation == 2) {
                storeLocation = 1;
            }          

            signedData = ctrl.SignData(content, certIndex, applyCoSign, storeLocation, storeName);
	    }
	    catch(e)
	    {
	        alert("Errore: " + e.message.toString());
	    } 
	
	    return signedData;
	}

	function CapicomWrapper_SignHash(content, certIndex, applyCoSign, storeLocation, storeName) {
	    var signedData = null;
	    certIndex = certIndex - 1;
	    try {
	        var ctrl = document.getElementById("digitalSignServicesControl");
	        if (storeLocation == 2) {
	            storeLocation = 1;
	        }

	        signedData = ctrl.SignHash(content, certIndex, applyCoSign, storeLocation, storeName);
	    }
	    catch (e) {
	        alert("Errore: " + e.message.toString());
	    }

	    return signedData;
	}
</script>
