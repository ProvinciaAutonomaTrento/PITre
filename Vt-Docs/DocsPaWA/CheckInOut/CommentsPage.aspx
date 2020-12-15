<%@ Page language="c#" Codebehind="CommentsPage.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.CheckInOut.CommentsPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Note per il rilascio</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript">
		
			function ClosePage(retValue)
			{
				window.returnValue=retValue;
				window.close();
			}
		
		</script>
	</HEAD>
	<body bottomMargin="1" leftMargin="1" topMargin="6" rightMargin="1" MS_POSITIONING="GridLayout">
		<form id="frmCommentsPage" method="post" encType="multipart/form-data" runat="server">
			<table class="info" id="tblContainer" cellSpacing="0" cellPadding="5" width="400" align="center"
				border="0" runat="server">
				<TR>
					<TD class="item_editbox">
						<P class="boxform_item">
							<asp:label id="lblComments" runat="server">NOTE</asp:label>
							<asp:TextBox id="txtComments" runat="server" Width="350px" TextMode="MultiLine" Rows="5"></asp:TextBox>
						</P>
						<br>
						<div align="center">
							<asp:Button id="btnCheckIn" Runat="server" class="PULSANTE" Text="Rilascia"></asp:Button>
							&nbsp;
							<asp:Button id="btnCancel" runat="server" class="PULSANTE" Text="Annulla"></asp:Button>
						</div>
					</TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
