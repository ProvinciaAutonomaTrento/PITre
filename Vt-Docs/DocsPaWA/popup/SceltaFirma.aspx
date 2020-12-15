<%@ Page language="c#" Codebehind="SceltaFirma.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.SceltaFirma" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
	
function FirmaDoc(f,tipofirma)
{
	var sFeatures="dialogHeight: 320px; dialogWidth: 420px; resizable: Yes ;dialogTop: 10px; dialogLeft:10px;";
	var destForm = "docVisualizzaFrame.aspx";
	var salvaForm = "docSalva.aspx";
	

	var certIndex = parseInt(window.showModalDialog("../popup/listaCertificati.aspx"," ",sFeatures),10);
	if(isNaN(certIndex) || certIndex==0) {
		return false;
	}

	var store = new ActiveXObject("CAPICOM.Store");	
	store.Open(2, "MY", 0);
	var cert = store.certificates(certIndex);

	var signer = new ActiveXObject("CAPICOM.Signer");
	signer.Certificate = cert;

	var http = new ActiveXObject("MSXML2.XMLHTTP")
	http.Open("POST", destForm,false);
	http.send();
	
	var sd = new ActiveXObject("CAPICOM.SignedData");
	sd.content = http.responseBody;

	try {
	var pkcs7 ;
	if(tipofirma=="sign")
			pkcs7 = sd.Sign(signer, false,0);
	else if(tipofirma=="cosign")
			{
				sd.Verify(sd.content,false,CAPICOM_VERIFY_SIGNATURE_AND_CERTIFICATE);
				pkcs7 = sd.CoSign(signer,0);
			}
			
		
		f.hdn_doc.value = pkcs7;
		http.Open("POST",salvaForm,false);
		http.send(pkcs7);
		
		if(http.status!=0 && http.status!=200) {
			alert("Errore durante il POST del documento firmato.\n" + http.statusText + "\n" + http.responseText);
			var w = window.open(salvaForm,"FirmaDocumento","width=270,height=140,toolbar=no,directories=no,menubar=no,resizable=yes,scrollbars=no");
			w.document.write(http.responseText);
		}
					
		f.img_firmato.style.display = "";
		return false; //blocco comunque la postback al server
	}
	catch(e){
		alert("Errore durante la firma del documento.\n" + e.toString());
		return false;
	}
}
		
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="SceltaFirma" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Scelta Firma" />
			<TABLE id="Table1" height="200" width="400" border="0" class="info">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item">TIPO FIRMA</P>
					</td>
				</TR>
				<TR>
					<TD align="middle" style="HEIGHT: 133px">
						<asp:RadioButtonList id="RadioButtonList1" runat="server" CssClass="testo_grigio_scuro" RepeatDirection="Horizontal" AutoPostBack="True">
							<asp:ListItem Value="sign">Firma&nbsp;&nbsp;</asp:ListItem>
							<asp:ListItem Value="cosign">CoFirma</asp:ListItem>
						</asp:RadioButtonList></TD>
				</TR>
				<tr>
					<td height="100%"></td>
				</tr>
				<TR>
					<TD align="middle">
						<asp:Button id="Button1" runat="server" Text="CHIUDI" CssClass="pulsante"></asp:Button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
