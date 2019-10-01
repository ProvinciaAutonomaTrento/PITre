<%@ Page language="c#" Codebehind="Acquisizione.aspx.cs" AutoEventWireup="false" Inherits="ProtocollazioneIngresso.Scansione.Acquisizione" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title><% = GetMaskTitle()%></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript">

			function InviaScansione() 
			{
				var retValue = document.Acquisizione.ctrlOptAcq.SavedFile;		
				var error = document.Acquisizione.ctrlOptAcq.ErrNumber;
				window.returnValue = retValue;
				window.close();	
			}

			function scan(stampaSegnaturaAbilitata, segnatura) {
			    try {
			        // A partire dalla versione 3.5.15, DocsPa_AcquisisciDoc.ocx
			        // gestisce l'impostazione del parametro per la stampa della
			        // segnatura direttamente tramite gli scanner REI.
			        // E' gestita un'eccezione nel caso in cui la versione del componente
			        // è precedente alla suddetta e non gestisce la stringa di segnatura
			        document.Acquisizione.ctrlOptAcq.PrintSegnatureEnabled = stampaSegnaturaAbilitata;
			        document.Acquisizione.ctrlOptAcq.PrintSegnature = segnatura;			        
			    }
			    catch (e) {
			    }

			    document.Acquisizione.ctrlOptAcq.ScannerStart();
			    return false;
			}
		</script>
		<script id="clientEventHandlersJS" language="javascript">
        <!--

        function resizeObject()
        {	
	        var l_newHeight=Math.abs(parseInt(window.document.body.clientHeight)-parseInt(acquisizione.ctrlOptAcq.clientTop)*2);
	        var l_newWidth=Math.abs(parseInt(window.document.body.clientWidth)-(parseInt(acquisizione.ctrlOptAcq.clientLeft*2))-10);
        	
	        acquisizione.ctrlOptAcq.width=l_newWidth;	
	        acquisizione.ctrlOptAcq.height=l_newHeight;
        }

        function window_onresize() 
        {
	        //resizeObject();
        }

        function window_onload() 
        {
            //resizeObject();
            if ('<%=this.IsEnabledAutomaticScan %>' == 1)
                return scan('<%=this.StampaSegnaturaAbilitata%>', '<%=this.SegnaturaProtocollo%>');
        }

        //-->
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" language="javascript" onresize="return window_onresize()"
		onload="return window_onload()">
		<form name="Acquisizione">
			<table align="center" width="100%" height="100%" border="0">
				<tr height="20" bgcolor="#d9d9d9">
					<td align="center"><input type="image" src="Images/ico_scan_avvia.gif" onclick="return scan('<%=this.StampaSegnaturaAbilitata%>', '<%=this.SegnaturaProtocollo%>');"
							alt="Avvia scansione"></td>
					<td align="center"><input type="image" src="Images/ico_scan_elimina.gif" onclick="document.Acquisizione.ctrlOptAcq.ImgDelete(); return false;"
							alt="Elimina pagina"></td>
					<td align="center"><input type="image" src="Images/ico_scan_precedente.gif" onclick="document.Acquisizione.ctrlOptAcq.ImgPrev(); return false;"
							alt="Pagina precedente"></td>
					<td align="center"><input type="image" src="Images/ico_scan_successiva.gif" onclick="document.Acquisizione.ctrlOptAcq.ImgNext(); return false;"
							alt="Pagina successiva"></td>
					<td align="center"><input type="image" src="Images/ico_scan_ruota.gif" onclick="document.Acquisizione.ctrlOptAcq.ImgRotation(); return false;"
							alt="Ruota immagine"></td>
					<td align="center"><input type="image" src="Images/ico_scan_salva.gif" onclick="document.Acquisizione.ctrlOptAcq.SaveScanner();InviaScansione(); return false;"
							alt="Invia File"></td>
				</tr>
				<tr valign="middle">
					<td width="100%" height="100%" colspan="6">
						<OBJECT id="ctrlOptAcq" codeBase="../../activex/DocsPa_AcquisisciDoc.CAB#version=1,0,0,0" height="100%"
							width="100%" classid="CLSID:00E3AC1D-2F39-465E-BD2C-24A4E1F1E83D" VIEWASTEXT>
							<PARAM NAME="_ExtentX" VALUE="20452">
							<PARAM NAME="_ExtentY" VALUE="13732">
						</OBJECT>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
