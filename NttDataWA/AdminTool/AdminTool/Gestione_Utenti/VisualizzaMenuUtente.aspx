<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizzaMenuUtente.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Utenti.VisualizzaMenuUtente" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<HTML>
	<HEAD runat = "server">
        <title></title>	
		<script language="C#" runat="server">
			public bool getCheckBox(object abilita)
			{			
				string abil = abilita.ToString();
				if(abil == "true")
				{
					return true;
				}
				else
				{
					return false;
				}
			}      
		</script>
		<script language=javascript>
		    function ApriFinestraRegistri(idCorrGlob,idAmm) 
			{			
				var myUrl = "VisualizzaRegistriUtente.aspx?idCorrGlob="+idCorrGlob+"&idAmm="+idAmm;				
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:460px;dialogHeight:213px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				Form1.hd_returnValueModal.value = rtnValue;								
				window.document.Form1.submit();	
			}	  
		</script>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione Utente / Menu" />
		    <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<TABLE class="contenitore" width="100%">
				<TR>
					<TD>
						<TABLE cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD class="titolo_pnl" vAlign="middle" align="center" width="5%"><IMG src="../Images/userAdmin_menu.gif" border="0">
								</TD>
								<TD class="titolo_pnl" vAlign="middle"><asp:label id="titolo" Runat="server" tabIndex="3"></asp:label></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD>
						<DIV style="OVERFLOW: auto"><asp:datagrid id="dg_menu" runat="server" Height="100%" AutoGenerateColumns="False" CellPadding="1"
								BorderWidth="1px" BorderColor="Gray" tabIndex="4" OnItemCreated="dg_menu_ItemCreated" OnSelectedIndexChanged="dg_menu_SelectedIndexChanged" >
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="IDMenu" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
									<asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice" Visible="False">
										<ItemStyle Width="0%"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">	
										<ItemStyle Width="100%"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm"></asp:BoundColumn>
									<asp:BoundColumn Visible="False" DataField="IDCorrGlob" ReadOnly="True" HeaderText="IDCorrGlob"></asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Btn">
										<HeaderStyle HorizontalAlign="Center" Width="50px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<HeaderTemplate>
											<asp:Label id="Label2" runat="server" CssClass="menu_1_bianco_dg">Dett</asp:Label>
										</HeaderTemplate>
										<ItemTemplate>
											<asp:ImageButton ID="dg_btn_registri" runat="server"  BorderColor="#404040" BorderWidth="0px" AlternateText="Registri"
												ImageUrl="../images/registri.gif"  Visible=false></asp:ImageButton>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Sel" >
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate  >
											<asp:CheckBox ID="Chk_menu" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.sel")) %>' runat="server" AutoPostBack=true OnCheckedChanged="Chk_menu_CheckedChanged" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:datagrid></DIV>
					</TD>
				</TR>
				<TR>
					<TD class="titolo_pnl">
						<TABLE cellSpacing="0" cellPadding="0" border="0" width="100%">
							<TR>
								<TD class="titolo_pnl" align="left">&nbsp;&nbsp;&nbsp;<asp:button id="btn_chiudi" tabIndex="2" runat="server" Text="Chiudi" CssClass="testo_btn_org" OnClick="btn_chiudi_Click"></asp:button></TD>
								<TD class="titolo_pnl" align="right"><asp:button id="btn_mod_menu" tabIndex="1" runat="server" Text="Modifica" CssClass="testo_btn_org" OnClick="btn_mod_menu_Click"></asp:button>&nbsp;&nbsp;&nbsp;</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
