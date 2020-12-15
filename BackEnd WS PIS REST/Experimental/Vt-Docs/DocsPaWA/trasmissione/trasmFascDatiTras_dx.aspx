<%@ Page language="c#" Codebehind="trasmFascDatiTras_dx.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.trasmissione.trasmDatiFascTras_dx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > trasmDatiFascTrasm_dx</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language='javascript'>
			var w = window.screen.width;
			var h = window.screen.height;
			var new_w = (w-100)/2;
			var new_h = (h-400)/2;
			
			function redirect()
			{
				top.principale.document.location='../fascicolo/gestioneFasc.aspx?tab=trasmissioni';
			}
			
			function apriSalvaModTrasm()
			{
				window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=../popup/salvaModTrasm.aspx','','dialogWidth:490px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);				
			}
			
			function apriSceltaNuovoPropietario(tipo)
			{
			    window.showModalDialog('../popup/sceltaNuovoProprietario.aspx?tipo='+tipo,'NuovoProprietario','dialogWidth:600px;dialogHeight:550px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
			}

			function openChooseTransDoc() {
			    window.showModalDialog('../popup/searchInControlledProject.aspx', '', 'dialogWidth:700px;dialogHeight:600px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);
			    document.forms[0].submit();
			}
		</script>		
	</HEAD>
	<body>
		<form id="trasmDatiFascTras_dx" method="post" name="trasmDatiFascTras_dx" runat="server">
		<input id="azione" type=hidden name="azione" runat=server />
			<INPUT id="flag_save_fasc" style="Z-INDEX: 102; LEFT: 22px; WIDTH: 87px; POSITION: absolute; TOP: 279px; HEIGHT: 22px" type="hidden" size="9" name="flag_save_fasc" runat="server">
			<table width="100%" border="0" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td height="20" align="middle" class="infoDT">
						<asp:Label ID="titolo" CssClass="titolo_rosso" Runat="server"></asp:Label></td>
				</tr>
				<tr>
					<td>
						<asp:table id="tbl_Lista" runat="server" HorizontalAlign="Right" CellSpacing="1" CellPadding="0" Width="100%"></asp:table>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Button id="Button1" runat="server" Text="SALVA" CssClass="PULSANTE" Visible="False"></asp:Button>
					</td>
				</tr>
			</table>
            <div id="please_wait" style="display:none; z-index:1000; border-right: #000000 2px solid; border-top: #000000 2px solid;
                border-left: #000000 2px solid; border-bottom: #000000 2px solid; position: absolute;
                background-color: #d9d9d9">
                <table cellspacing="0" cellpadding="0" width="350px" border="0">
                    <tr>
                        <td valign="middle" style="font-weight: bold; font-size: 12pt; font-family: Verdana"
                            align="center" width="350px" height="90px">
                            Attendere, prego...
                        </td>
                    </tr>
                </table>
            </div>
		</form>
	</body>
</HTML>
