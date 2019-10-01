<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentAcquisitionDlg.aspx.cs" Inherits="NttDataWA.SmartClient.DocumentAcquisitionDlg" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <BASE target="_self">
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
     <script type="text/javascript">
         // Visualizzatore immagini per la scansione
         function ShowImageViewer() {
             try {
                 acquisizione.myControl.ApplyImageCompression = true;
                 acquisizione.myControl.ApplyPdfConvertion = ("<%=this.SmartClientConfigurations.ApplyPdfConvertionOnScan%>" == "True")
                 acquisizione.myControl.AllowScan = true;
                 acquisizione.myControl.AllowSave = true;
                 acquisizione.myControl.ShowRemovePage = true;
                 acquisizione.myControl.ShowPageNavigations = true;
                 acquisizione.myControl.ShowZoomCapabilities = true;
                 acquisizione.myControl.Title = "Acquisisci documento";
                 acquisizione.myControl.SetSizeScreenPercentage(100, 100);
                 acquisizione.myControl.Signature = "";
                 var ret = acquisizione.myControl.ShowImageViewer();
                 
                 var imagePath = "";

                 if (ret) {
                     imagePath = acquisizione.myControl.ImagePath;
                 }

                 self.returnValue = imagePath;
                 self.close();
             }
             catch (ex) {
                 alert("Errore nell'utilizzo dei componenti client necessari per l'acquisizione:\n" + ex.message.toString());

                 self.returnValue = "";
                 self.close();
             }
         }
    </script>
</head>
<body onload="ShowImageViewer()">
	<form name="acquisizione">
        <uc1:FsoWrapper ID="fsoWrapper" runat="server" />	
        <table class="testo_msg_grigio" align="center" width="100%" height="100%">
            <tr align="center" valign="middle">
	            <td>
	                <font size="3">Acquisizione da scanner in corso...</font>
	            </td>
		    </tr>
	    </table>
        <OBJECT id="myControl" height="0" width="0" 
                data="data:application/x-oleobject;base64,IGkzJfkDzxGP0ACqAGhvEzwhRE9DVFlQRSBIVE1MIFBVQkxJQyAiLS8vVzNDLy9EVEQgSFRNTCA0LjAgVHJhbnNpdGlvbmFsLy9FTiI+DQo8SFRNTD48SEVBRD4NCjxNRVRBIGh0dHAtZXF1aXY9Q29udGVudC1UeXBlIGNvbnRlbnQ9InRleHQvaHRtbDsgY2hhcnNldD13aW5kb3dzLTEyNTIiPg0KPE1FVEEgY29udGVudD0iTVNIVE1MIDYuMDAuMjkwMC4yOTEyIiBuYW1lPUdFTkVSQVRPUj48L0hFQUQ+DQo8Qk9EWT4NCjxQPiZuYnNwOzwvUD48L0JPRFk+PC9IVE1MPg0K"
                classid="../SmartClient/Librerie/DPA.Web.dll#DPA.Web.Imaging.ImageViewerContainerControl"
                VIEWASTEXT>			
        </OBJECT>        	
    </form>
</body>
</html>