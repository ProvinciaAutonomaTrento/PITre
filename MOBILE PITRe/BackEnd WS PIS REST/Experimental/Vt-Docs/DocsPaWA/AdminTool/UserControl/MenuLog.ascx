<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuLog.ascx.cs" Inherits="Amministrazione.UserControl.MenuLog" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellpadding="0" cellspacing="1" width="120" bgcolor="#c0c0c0" height="100%">
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<% 			
			if (Request.QueryString["menuLog"] == "abilLog") { %>
		<!--  TASTO : Abilitazioni  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Attivazione log
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink1" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/AbilitaLog.aspx?from=GL&amp;menuLog=abilLog"
				ToolTip="Attivazione log">Attivazione log</asp:hyperlink>
		</td>
		<% } %>
	</tr>
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<%
		if (Request.QueryString["menuLog"] == "queryLog") { %>
		<!--  TASTO : Query Log  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Ricerca log
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink2" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/QueryLog.aspx?from=GL&amp;menuLog=queryLog"
				ToolTip="Ricerca log">Ricerca log</asp:hyperlink>
		</td>
		<% } %>
	</tr>
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<%
		if (Request.QueryString["menuLog"] == "archLog") { %>
		<!--  TASTO : Archivia Log  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Archivia log
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink3" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/ArchiviaLog.aspx?from=GL&amp;menuLog=archLog"
				ToolTip="Archivia log">Archivia log</asp:hyperlink>
		</td>
		<% } %>
	</tr>
    <asp:Panel ID="pnl_estrazione_log" runat="server" Visible="false">
    <tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
    <tr>
		<%
		if (Request.QueryString["menuLog"] == "estrLog") { %>
		<!--  TASTO : Estrazione Log  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Estrazione log
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink4" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/EstrazioneLog.aspx?from=GL&amp;menuLog=estrLog"
				ToolTip="Estrazione log">Estrazione log</asp:hyperlink>
		</td>
		<% } %>
	</tr>
    </asp:Panel>
   <tr>
		<td width="120" height="100%">&nbsp;</td>
	</tr>
</table>
