<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuLogAmm.ascx.cs" Inherits="Amministrazione.UserControl.MenuLogAmm" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<table border="0" cellpadding="0" cellspacing="1" width="120" bgcolor="#c0c0c0" height="100%">
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<% 			
			if (Request.QueryString["menuLog"] == "abilLogAmm") { %>
		<!--  TASTO : Abilitazioni  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Attivazione log Amm.
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink1" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/AbLogAmm.aspx?from=GL&amp;menuLog=abilLogAmm"
				ToolTip="Attivazione log">Attivazione log Amm.</asp:hyperlink>
		</td>
		<% } %>
	</tr>
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<%
		if (Request.QueryString["menuLog"] == "queryLogAmm") { %>
		<!--  TASTO : Query Log  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Ricerca log Amm.
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink2" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/QueryLogAmm.aspx?from=GL&amp;menuLog=queryLogAmm"
				ToolTip="Ricerca log">Ricerca log Amm.</asp:hyperlink>
		</td>
		<% } %>
	</tr>
	<tr>
		<td width="120" height="20">&nbsp;</td>
	</tr>
	<tr>
		<%
		if (Request.QueryString["menuLog"] == "archLogAmm") { %>
		<!--  TASTO : Archivia Log  -->
		<td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="center">
			Archivia log Amm.
		</td>
		<% }else { %>
		<td width="120" height="25" bgcolor="#800000" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink3" runat="server" Target="_parent" NavigateUrl="../Gestione_Logs/ArchiviaLogAmm.aspx?from=GL&amp;menuLog=archLogAmm"
				ToolTip="Archivia log">Archivia log Amm.</asp:hyperlink>
		</td>
		<% } %>
	</tr>
	<tr>
		<td width="120" height="100%">&nbsp;</td>
	</tr>
</table>
