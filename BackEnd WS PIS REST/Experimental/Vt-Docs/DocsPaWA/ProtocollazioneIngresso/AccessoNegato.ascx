<%@ Control Language="c#" AutoEventWireup="false" Codebehind="AccessoNegato.ascx.cs" Inherits="ProtocollazioneIngresso.AccessoNegato" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
<TABLE id="tblRegistriNonDisponibili" width="400" align="center" class="info_grigio" runat="server"
	cellSpacing="0" cellPadding="0" border="0">
	<TR>
		<TD align="center" class="tabDoc">
			<asp:Label id="lblMessage" runat="server"></asp:Label>
		</TD>
	</TR>
	<TR>
		<TD align="center" class="tabDoc">
			<asp:button id="btnClose" CausesValidation="False" Text="Chiudi" CssClass="pulsante" Width="100px"
				runat="server"></asp:button></TD>
	</TR>
</TABLE>
