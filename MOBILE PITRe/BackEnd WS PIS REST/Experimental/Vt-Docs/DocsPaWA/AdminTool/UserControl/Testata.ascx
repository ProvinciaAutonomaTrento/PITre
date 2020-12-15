<%@ Control Language="c#" AutoEventWireup="false" Codebehind="Testata.ascx.cs" Inherits="Amministrazione.UserControl.Testata" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<meta name="vs_snapToGrid" content="True">
<table border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr>
		<td height="48" align="left"><img src="../Images/logo.gif" border="0" height="48" width="218"></td>
		<td valign="top" align="right" class="testo_grigio_scuro_grande" width="100%">
			|&nbsp;&nbsp;&nbsp;<a href="#" onClick="apriPopup();">Help</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href="#" onClick="cambiaPwd();">Cambia 
				password</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href="../Exit.aspx">Chiudi</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
		</td>
	</tr>
	<tr>
		<td height="5" colspan=2></td>
	</tr>
</table>
<table border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr>
		<% if (Session["Restricted"] != "Y") {
			if (Request.QueryString["from"] == "HP" || Request.QueryString["from"] == null) { %>
		<!--  TASTO : HOME  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Home
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" align="center" class="testo_bianco">
			<asp:hyperlink CssClass="menu" id="Hyperlink1" runat="server" Target="_parent" NavigateUrl="../Gestione_Homepage/Home.aspx?from=HP"
				ToolTip="Homepage">Home</asp:hyperlink>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 
		   if (Request.QueryString["from"] == "RU") { %>
		<!--  TASTO : RUOLI  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Tipi ruolo
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink3" runat="server" Target="_parent" NavigateUrl="../Gestione_Ruoli/Ruoli.aspx?from=RU"
				ToolTip="Tipi ruolo">Tipi ruolo</asp:hyperlink>
			<% } else { %>
			Tipi ruolo
			<% } %>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 		
		   if (Request.QueryString["from"] == "UT") { %>
		<!--  TASTO : UTENTI  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Utenti
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink4" runat="server" Target="_parent" NavigateUrl="../Gestione_Utenti/GestUtenti.aspx?from=UT"
				ToolTip="Utenti">Utenti</asp:hyperlink>
			<% } else { %>
			Utenti
			<% } %>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 
		   if (Request.QueryString["from"] == "RG") { %>
		<!--  TASTO : REGISTRI  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Registri
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink5" runat="server" Target="_parent" NavigateUrl="../Gestione_Registri/Registri.aspx?from=RG"
				ToolTip="Registri">Registri</asp:hyperlink>
			<% } else { %>
			Registri
			<% } %>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 
		   if (Request.QueryString["from"] == "FU") { %>
		<!--  TASTO : FUNZIONI  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Funzioni
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink6" runat="server" Target="_parent" NavigateUrl="../Gestione_Funzioni/TipiFunzione.aspx?from=FU"
				ToolTip="Funzioni">Funzioni</asp:hyperlink>
			<% } else { %>
			Funzioni
			<% } %>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 
		   if (Request.QueryString["from"] == "RT") { %>
		<!--  TASTO : RAGIONI DI TRASMISSIONE  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Rag. Trasm.
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink7" runat="server" Target="_parent" NavigateUrl="../Gestione_RagioniTrasm/RagioneTrasm.aspx?from=RT"
				ToolTip="Ragioni di Trasmissione">Rag. Trasm.</asp:hyperlink>
			<% } else { %>
			Rag. Trasm.
			<% } %>
		</td>
		<% }} if (Session["Restricted"] != "Y") { 
		   if (Request.QueryString["from"] == "OR") { %>
		<!--  TASTO : ORGANIGRAMMA  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Organigramma
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink8" runat="server" Target="_parent" NavigateUrl="../Gestione_Organigramma/Organigramma.aspx?from=OR"
				ToolTip="Organigramma">Organigramma</asp:hyperlink>
			<% } else { %>
			Organigramma
			<% } %>
		</td>
		<% }}
		   if (Request.QueryString["from"] == "TI") { %>
		<!--  TASTO : TITOLARIO  -->
		<td width="95" height="25" background="../Images/tasto_a.gif" class="testo_bianco" align="center">
			Titolario
		</td>
		<% }else { %>
		<td width="95" height="25" background="../Images/tasto.jpg" class="testo_bianco" align="center">
			<% if (Session["AMMDATASET"] != null) { %>
			<asp:hyperlink CssClass="menu" id="Hyperlink9" runat="server" Target="_parent" NavigateUrl="../Gestione_Titolario/Titolario.aspx?from=TI"
				ToolTip="Titolario">Titolario</asp:hyperlink>
			<% } else { %>
			Titolario
			<% } %>
		</td>
		<% } if (Session["Restricted"] != "Y") { %>
		<td height="25" width="25" bgcolor="#810d06"><% if (Session["AMMDATASET"] != null) { %><script language="JavaScript">try { mmLoadMenus(); } catch(e) {}</script><a href="javaScript:void(0)" onMouseOver="MM_showMenu(window.mm_menu,-1,26,null,'img_other')"
				onMouseOut="MM_startTimeout();"><img id="img_other" name="img_other" src="../Images/tasto_other.gif" border="0" width="25"
					height="25" title="Altre opzioni" alt="Altre opzioni"></a><% } else { %>&nbsp;<% } %></td>
		<% } %>
		<td height="25" bgcolor="#810d06">&nbsp;</td>
	</tr>
</table>
