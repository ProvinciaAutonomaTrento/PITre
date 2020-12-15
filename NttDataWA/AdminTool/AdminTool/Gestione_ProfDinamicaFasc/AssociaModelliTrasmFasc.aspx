<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssociaModelliTrasmFasc.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamicaFasc.AssociaModelliTrasm" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head runat="server">
    <title></title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
	<meta name="CODE_LANGUAGE" Content="C#">
	<meta name="vs_defaultClientScript" content="JavaScript">
	<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	<script language="javascript" type="text/javascript">
	function isCodiceNumber()
		{
		    if(form1.hd_codice.value=="1")
		    {
		        var pattern =/^[Mm]{1}[Tt1}\_[0-9]+$/; 

		        if(document.getElementById("txt_ricerca").value!="" && document.getElementById("txt_ricerca")!=null)
		        {
		            var codice=document.getElementById("txt_ricerca").value;
		            if(!pattern.test(codice))
		            {
		                alert('attenzione: il formato del codice deve essere mt_<numero modello>');
		                return false;
		            }
		        }
		    }
	        return true;
		}
	</script>
</head>
<body>
    <form id="form1" method="post" runat="server">
    <input type="hidden" name="hd_codice" id="hd_codice" runat="server" value="0" />		    
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Associa modelli" />
		<table width="100%">
			<tr>
				<td class="titolo" align="center" bgColor="#e0e0e0" height="20">
					<asp:Label id="lbl_titolo" runat="server"></asp:Label></td>
				<td align="center" width="10%" bgColor="#e0e0e0">
					<asp:Button id="btn_conferma" runat="server" Text="Conferma" 
                        CssClass="testo_btn_p" onclick="btn_conferma_Click"></asp:Button></td>
			</tr>
			<tr>
		    <td class="testo_piccoloB">Ricerca per:&nbsp;&nbsp;
			    <asp:DropDownList id="ddl_ricTipo" Runat="server" CssClass="testo_grigio_scuro" 
                    AutoPostBack="True" 
                    onselectedindexchanged="ddl_ricTipo_SelectedIndexChanged">
                    <asp:ListItem Value="T">Tutti</asp:ListItem>
				    <asp:ListItem Value="C">Codice</asp:ListItem>
					<asp:ListItem Value="D">Descrizione</asp:ListItem>
					<asp:ListItem Value="S">Selezionati</asp:ListItem>
					<asp:ListItem Value="U">Non selezionati</asp:ListItem>
				</asp:DropDownList>&nbsp;&nbsp;
			    <asp:textbox id="txt_ricerca"  CssClass="testo_grigio_scuro_grande" Runat="server" Width="270px" Enabled=false></asp:textbox>&nbsp;
				</td>
				<td>
				<asp:button id="btn_find"  CssClass="testo_btn_p" Runat="server" Text="Cerca" 
                    onclick="btn_find_Click" onclientclick="return isCodiceNumber();"></asp:button>
			</td>
		</tr>
			<tr>
				<td colspan="2">
				    <DIV id="div_listaModelli" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 310px; width:100%;">
					<asp:DataGrid id="dg_ModelliTrasm" runat="server" AutoGenerateColumns="False" Width="100%">
						<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
						<ItemStyle CssClass="bg_grigioN"></ItemStyle>
						<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
						<Columns>
							<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SystemId"></asp:BoundColumn>
							<asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
								<HeaderStyle Width="60%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Selezione">
								<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:CheckBox id="CheckBox1" runat="server" AutoPostBack="true"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Trasm. Aut.">
								<HeaderStyle HorizontalAlign="Center" Width="20%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:CheckBox id="CheckBox2" runat="server" AutoPostBack="True"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</asp:DataGrid>
					</DIV>
				</td>
			</tr>
		</table>    
    </form>
</body>
</html>
