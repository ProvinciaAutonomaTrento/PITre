<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Testata.ascx.cs" Inherits="SAAdminTool.AdminTool.Gestione_VociMenu.Testata" %>
<script type="text/javascript" language="JavaScript">
    function keepAliveSession(imgName) {
        myImg = document.getElementById(imgName);
        if (myImg) myImg.src = myImg.src.replace(/\\?.*$/, '?' + Math.random());
    } 
</script>

<script type="text/javascript" language="javascript">
			function OpenHelp(from) 
			{		
				var pageHeight= 600;
				var pageWidth = 800;
				//alert(from);
				var posTop = (screen.availHeight-pageHeight)/2;
				var posLeft = (screen.availWidth-pageWidth)/2;
				
				var newwin = window.showModalDialog('../Gestione_Help/Manuale.aspx?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');
								
			}							

</script>
<%--<img id="imgKA" width="1" height="1" src="../Images/sessionrefresch.gif?" alt="." />--%>

<table border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr>
		<td height="48" align="left"><asp:Image ID="logoAmm" runat="server" ImageUrl="../Images/logo.gif" border="0" height="48" width="218" /></td>
		<td valign="top" align="right" class="testo_grigio_scuro_grande" width="100%">
			|&nbsp;&nbsp;&nbsp;<asp:Label ID="lbl_help" runat="server" Text="Help"></asp:Label>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href="#" onClick="cambiaPwd();">Cambia 
				password</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<a href="../Exit.aspx">Chiudi</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;
		</td>
	</tr>
	<tr>
		<td height="5" colspan=2></td>
	</tr>
</table>
<table border="0" cellpadding="0" cellspacing="1" width="100%">
	<tr>
	    <!--  TASTO : HOME  -->
	    <td id=td_home runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
	        <asp:hyperlink  CssClass="menu" 
	                        id="Hyperlink_home" 
	                        runat="server" 
	                        Target="_parent" 
	                        NavigateUrl="../Gestione_Homepage/Home.aspx?from=HP"	
	                        ToolTip="Homepage">Home</asp:hyperlink>
	    </td>
	    <!--  TASTO : RUOLI  -->
	    <td id=td_ruoli runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
	        <asp:hyperlink  CssClass="menu" 
	                        id="Hyperlink_ruoli" 
	                        runat="server" 
	                        Target="_parent" 
	                        NavigateUrl="../Gestione_Ruoli/Ruoli.aspx?from=RU" 
	                        ToolTip="Tipi ruolo">Tipi ruolo</asp:hyperlink>
	    </td>
	    <!--  TASTO : UTENTI  -->
		<td id=td_utenti runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_utenti" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Utenti/GestUtenti.aspx?from=UT" 
		                    ToolTip="Utenti">Utenti</asp:hyperlink>
		</td>
		<!--  TASTO : REGISTRI  -->
		<td id=td_registri runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_registri" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Registri/Registri.aspx?from=RG"	
		                    ToolTip="Registri">Registri</asp:hyperlink>
		</td>
		<!--  TASTO : FUNZIONI  -->
		<td id="td_funzioni" runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_funzioni" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Funzioni/TipiFunzione.aspx?from=FU"	
		                    ToolTip="Funzioni">Funzioni</asp:hyperlink>
		</td>
		<!--  TASTO : RAGIONI DI TRASMISSIONE  -->
		<td id=td_ragioni runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_ragTrasm" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_RagioniTrasm/RagioneTrasm.aspx?from=RT"	
		                    ToolTip="Ragioni di Trasmissione">Rag. Trasm.</asp:hyperlink>
		</td>
		<!--  TASTO : ORGANIGRAMMA  -->
		<td id=td_organigramma runat=server width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_organigramma" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Organigramma/Organigramma.aspx?from=OR"	
		                    ToolTip="Organigramma">Organigramma</asp:hyperlink>
		</td>
		<!--  TASTO : TITOLARIO  -->
		<td id="td_titolario" runat="server" width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_titolario" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Titolario/Titolario.aspx?from=TI" 
		                    ToolTip="Titolario">Titolario</asp:hyperlink>
		</td>
        <!--  TASTO : CONSERVAZIONE  -->
		<td id="td_conservazione" runat="server" width="95" height="25" class="testo_bianco" align="center" background="../Images/tasto.jpg" runat="server">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_conservazione" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Conservazione/Conservazione.aspx?from=CON" 
		                    ToolTip="Conservazione">Conservazione</asp:hyperlink>
		</td>
		<!--  TASTO : PUBBLICAZIONI  -->
		<td id="td_pubblicazioni" runat="server" width="95" height="25"
                 class="testo_bianco" align="center" background="../Images/tasto.jpg">
		    <asp:hyperlink  CssClass="menu" 
		                    id="Hyperlink_pubblicazioni" 
		                    runat="server" 
		                    Target="_parent" 
		                    NavigateUrl="../Gestione_Pubblicazioni/Pubblicazioni.aspx?from=PU" 
		                    ToolTip="Pubblicazioni">Pubblicazioni</asp:hyperlink>
		</td>		
		<td height="25" width="25" bgcolor="#810d06">
		    <% if (Session["AMMDATASET"] != null) { %><script language="JavaScript">mmLoadMenus();</script><a href="javaScript:void(0)" onMouseOver="MM_showMenu(window.mm_menu,-1,26,null,'img_other')"
				onMouseOut="MM_startTimeout();"><img id="img1" name="img_other" src="../Images/tasto_other.gif" border="0" width="25"
					height="25" title="Altre opzioni" alt="Altre opzioni"></a><% } else { %><img id="img1" name="img_other" src="../Images/tasto_other.gif" border="0" width="25"
					height="25" title="Altre opzioni" alt="Altre opzioni"><% } %></td>		    	
		<td height="25" bgcolor="#810d06">&nbsp;</td>
	</tr>
</table>
<script language="JavaScript">
//    window.setInterval("keepAliveSession('imgKA')", 18000); 
</script>