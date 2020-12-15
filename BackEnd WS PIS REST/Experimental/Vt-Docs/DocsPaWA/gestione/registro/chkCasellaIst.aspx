<%@ Page language="c#" Codebehind="chkCasellaIst.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.gestione.registro.chkCasellaIst" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<base target="_self">
		<script language="javascript">
			// Visualizzazione maschera per l'acquisizione dei
			// documenti tramite componenti smartclient
			function ShowMailCheckResultWindow()
			{				
				var args=new Object;
				args.window=window;

				var height=screen.availHeight;
				var width=screen.availWidth;
				
				//height=(height * 90) / 100;
				//width=(width * 90) / 100;
				
				height="670";
				width="900";
				
				window.showModalDialog('../../Interoperabilita/MailCheckResponse.aspx',
						'',
						'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: no;status:no;scroll:yes;help:no;close:no');
						
				window.close();	
			}
			
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" style="CURSOR: wait">
		<form id="chkCasellaIst" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Verifica casella istituzionale" />
			<table width="100%" height="100%" ID="Table1">
				<tr align="center" valign="middle">
					<td class="testo_grigio">
						<label id="lbl_msg" Class="testo_grigio" style="FONT-SIZE: medium;  COLOR: black;  FONT-FAMILY: verdana;  Arial: ; tahoma: ">
							Verifica in corso...</label>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
