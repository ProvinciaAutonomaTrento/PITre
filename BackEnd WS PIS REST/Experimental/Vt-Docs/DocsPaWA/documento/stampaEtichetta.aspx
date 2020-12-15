<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stampaEtichetta.aspx.cs" Inherits="DocsPAWA.documento.stampaEtichetta" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>stampa</title>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="-1">	
    <OBJECT ID="ctrlPrintPen" 
			CLASSID="CLSID:2860F27F-FC9F-4CDA-B0CB-55A5BD09C52E"
		    CODEBASE="../activex/DocsPa_PrintPen.CAB#version=1,1,0,0" VIEWASTEXT>
    </OBJECT>
<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<script language="javascript">
	    function stampaSegnatura() {
	        var qst = location.search.substr(1);
	        var dati = new Array()
	        dati = qst.split("&");
	        var dato1 = dati[0].substr(dati[0].indexOf("=") + 1);
	        //alert(dato1);
	        var l_segnatura = unescape(dato1);
	        if (l_segnatura != "") {
	            pf_printSignatureWithPenna(l_segnatura);
	        }
	        else {
	            alert('segnatura non assegnata!');
	        }
	    }

	    function pf_printSignatureWithPenna(signature) {
	        try {
	            ctrlPrintPen.UrlFileIni = document.forms[0].hd_UrlIniFileDispositivo.value;
	            ctrlPrintPen.Dispositivo = document.forms[0].hd_dispositivo.value;
	        }
	        catch (e) {
	        }
	        try {
	            ctrlPrintPen.Text = signature;
	            ctrlPrintPen.Amministrazione = unescape(document.forms[0].hd_descrizioneAmministrazione.value);
	            ctrlPrintPen.NumeroDocumento = unescape(document.forms[0].hd_num_doc.value);

	            //carica Campi Estesi Etichetta			
	            ctrlPrintPen.Classifica = unescape(document.forms[0].hd_classifica.value);
	            ctrlPrintPen.Fascicolo = unescape(document.forms[0].hd_fascicolo.value);
	            ctrlPrintPen.Amministrazione_Etichetta = unescape(document.forms[0].hd_amministrazioneEtichetta.value);
	            ctrlPrintPen.CodiceUoProtocollatore = unescape(document.forms[0].hd_coduo_proto.value);
	            ctrlPrintPen.CodiceRegistroProtocollo = unescape(document.forms[0].hd_codreg_proto.value);
	            ctrlPrintPen.TipoProtocollo = unescape(document.forms[0].hd_tipo_proto.value);
	            ctrlPrintPen.NumeroProtocollo = unescape(document.forms[0].hd_num_proto.value);
	            ctrlPrintPen.AnnoProtocollo = unescape(document.forms[0].hd_anno_proto.value);
	            ctrlPrintPen.DataProtocollo = unescape(document.forms[0].hd_data_proto.value);
	            ctrlPrintPen.Stampa();
	            //window.close();
	        }
	        catch (e) {
	            alert("Errore.\n" + e.toString());
	        }
	    }
		
		</script>
	</HEAD>
	<!--body onload="stampaSegnatura();" MS_POSITIONING="GridLayout"-->
	<body onload="" MS_POSITIONING="GridLayout">
		<form id="stampaPrintPen" method="post" runat="server">
		<table id="tbl_contenitore" class="info" align="center" border="0">
				<TR>
					<td class="item_editbox">
						<P class="boxform_item"><asp:label id="Label1" runat="server">Procedura di stampa</asp:label></P>
					</td>
				</TR>
				<tr>
					<td align="middle" height="30">&nbsp;
						<asp:Label id="Label2" runat="server" CssClass="testo_grigio_scuro" Width="272px">Si vuole procedere con la stampa?</asp:Label></td>
				</tr>
				<tr>
					<td align="middle"><asp:label id="lbl_segnatura" runat="server" CssClass="testo_red" Width="40px"></asp:label>
					<asp:button id="btn_ok" runat="server" CssClass="pulsante" Text="OK" OnClick="btn_ok_Click1"></asp:button>
					<asp:button id="btn_chiudi" runat="server" CssClass="pulsante" Text="CHIUDI" OnClick="btn_chiudi_Click1"></asp:button>
                        </td>
				</tr>
				<TR height="0">
					<TD>
                    </TD>
				</TR>
			</table>
            <asp:HiddenField ID="hd_amministrazione" runat="server" />
            <!--x stampa etichette:-->
			<asp:HiddenField id="hd_UrlIniFileDispositivo" runat="server" />
			<asp:HiddenField id="hd_dispositivo" runat="server" /> 
			<asp:HiddenField id="hd_amministrazioneEtichetta" runat="server" /> 
			<asp:HiddenField id="hd_descrizioneAmministrazione" runat="server" /> 
			<asp:HiddenField id="hd_classifica" runat="server" />
			<asp:HiddenField id="hd_fascicolo" runat="server" /> 
			<asp:HiddenField id="hd_num_proto" runat="server" /> 
			<asp:HiddenField id="hd_anno_proto" runat="server" />
			<asp:HiddenField id="hd_data_proto" runat="server" /> 
			<asp:HiddenField id="hd_codreg_proto" runat="server" />
			<asp:HiddenField id="hd_coduo_proto" runat="server" /> 
			<asp:HiddenField id="hd_tipo_proto" runat="server" />
			<asp:HiddenField id="hd_num_doc" runat="server" /> 
			<asp:HiddenField id="hd_numero_allegati" runat="server" />
			<asp:HiddenField id="hd_codiceUoCreatore" runat="server" />
			<asp:HiddenField id="hd_dataCreazione" runat="server" />
			<!--end x stampa etichette:-->
		</form>
	</body>
</html>