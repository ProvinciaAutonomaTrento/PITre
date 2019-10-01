<%@ Page language="c#" Codebehind="Navigazione_Organigramma.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.Navigazione_Organigramma" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript">
			function ApriRisultRic(idAmm) 
			{										
				window.document.body.style.cursor='wait';
				
				if(document.Form1.txt_ricCod.value.length > 0 || document.Form1.txt_ricDesc.value.length > 0)
				{
					var myUrl = "RisultatoRicercaOrg.aspx?idAmm="+idAmm+"&tipo="+document.Form1.ddl_ricTipo.value+"&cod="+document.Form1.txt_ricCod.value+"&desc="+document.Form1.txt_ricDesc.value;
					rtnValue = window.showModalDialog(myUrl,"","dialogWidth:750px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
					Form1.hd_returnValueModal.value = rtnValue;
				}
			}
			function stampa()
			{
				td_ricerca.style.display="None";
				window.print();
				td_ricerca.style.display="";
			}			
			
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
        	<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Organigramma" />
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<input id="hd_tipologia" type="hidden" name="hd_tipologia" runat="server"> <input id="hd_selezione" type="hidden" name="hd_selezione" runat="server">
			<input id="hd_subselezione" type="hidden" name="hd_subselezione" runat="server">
			<input id="hd_readonly" type="hidden" name="hd_readonly" runat="server">
			<TABLE cellSpacing="1" cellPadding="1" width="100%" align="center" border="0">
				<TR>
					<TD class="pulsanti" id="td_ricerca">
						<TABLE cellSpacing="2" cellPadding="0" width="100%" align="center" border="0">
							<TR>
								<TD class="testo_piccoloB" width="15%">Ricerca tra:</TD>
								<TD class="testo_piccoloB" width="15%">Codice:</TD>
								<TD class="testo_piccoloB" id="td_descRicerca" width="30%" runat="server">Nome UO:</TD>
								<TD class="testo_piccoloB" width="10%"></TD>
								<TD class="testo_piccoloB" width="30%"></TD>
							</TR>
							<TR>
								<TD class="testo_piccoloB" width="15%"><asp:dropdownlist id="ddl_ricTipo" Runat="server" AutoPostBack="True" CssClass="testo_grigio_scuro"></asp:dropdownlist></TD>
								<TD class="testo_piccoloB" width="15%"><asp:textbox id="txt_ricCod" tabIndex="1" Runat="server" CssClass="testo_grigio_scuro" Width="80"></asp:textbox></TD>
								<TD class="testo_piccoloB" width="30%"><asp:textbox id="txt_ricDesc" tabIndex="2" Runat="server" CssClass="testo_grigio_scuro" Width="210"></asp:textbox></TD>
								<TD class="testo_piccoloB" width="10%"><asp:button id="btn_find" tabIndex="3" Runat="server" CssClass="testo_btn" Text="Cerca"></asp:button></TD>
								<TD class="testo_piccoloB" align="right" width="30%"><!--<input class="testo_btn" onclick="stampa();" type="button" value="Stampa">--></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD id="treeview" vAlign="top">
						<DIV style="OVERFLOW: auto; HEIGHT: 100%"><IEWC:TREEVIEW id="treeViewUO" runat="server" AutoPostBack="True" width="100%" font="verdana" bordercolor="maroon"
								borderstyle="solid" borderwidth="1px" backcolor="antiquewhite" DefaultStyle="font-weight:normal;font-size:10px;color:black;text-indent:0px;font-family:Verdana;"></IEWC:TREEVIEW></DIV>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
