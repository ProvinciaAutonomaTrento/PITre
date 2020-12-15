<%@ Page language="c#" Codebehind="UOSmistamento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.AdminTool.Gestione_Registri.UOSmistamento" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
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
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<SCRIPT language="JavaScript">		
			function CheckAllDataGridCheckBoxes(aspCheckBoxID, checkVal) 
			{
				re = new RegExp(':' + aspCheckBoxID + '$')  
				for(i = 0; i < document.forms[0].elements.length; i++) 
				{
					elm = document.forms[0].elements[i]
					if (elm.type == 'checkbox') 
					{
						if (re.test(elm.name)) 
						{
							elm.checked = checkVal
						}
					}
				}
			}			
			function apriPopup() {
				window.open('../help.aspx?from=UOSM','','width=450,height=500,scrollbars=YES');
			}	
		</SCRIPT>
		<script language="javascript" id="btn_modifica_click" event="onclick()" for="btn_modifica">
		window.document.body.style.cursor='wait';
		</script>
		<base target="_self">
	</HEAD>
	<body bottomMargin="5" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Primo smistamento" />
			<input type="hidden" name="hd_idRegistro" id="hd_idRegistro" runat="server">
			<table cellSpacing="0" cellPadding="0" align="center" border="0" width="100%">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="apriPopup();" href="#">Help</A>&nbsp;&nbsp;|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" align="right" height="10">&nbsp;</td>
				</tr>
			</table>
			<table cellSpacing="0" cellPadding="0" align="center" border="0" width="100%">
				<tr>
					<td class="testo_grigio_scuro">Visualizza UO:&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:RadioButton ID="rb_attivi" AutoPostBack="True" TextAlign="Left" GroupName="visualizza" Text="Attive"
							Runat="server" CssClass="testo_grigio_scuro" Checked="True"></asp:RadioButton>&nbsp;&nbsp;&nbsp;
						<asp:RadioButton ID="rb_nonAttivi" AutoPostBack="True" TextAlign="Left" GroupName="visualizza" Text="Non attive"
							Runat="server" CssClass="testo_grigio_scuro"></asp:RadioButton>&nbsp;&nbsp;&nbsp;
						<asp:RadioButton ID="rb_tutti" AutoPostBack="True" TextAlign="Left" GroupName="visualizza" Text="Tutte"
							Runat="server" CssClass="testo_grigio_scuro"></asp:RadioButton>
					</td>
				</tr>
				<tr>
					<td height="20">
						<asp:label id="lbl_tit" CssClass="titolo" Runat="server"></asp:label></td>
				</tr>
				<tr>
					<td align="right">
						<asp:Button id="btn_selDeselTutti" width="120" runat="server" CssClass="testo_btn" Text="Seleziona tutti"></asp:Button>
					</td>
				<tr>
				<tr><td>&nbsp;</td></tr>
				    <td>								
						<DIV id="DivDGList" style="OVERFLOW: auto; HEIGHT: 395px">
							<asp:datagrid id="dg_AbilitaUO" runat="server" BorderWidth="1px" CellPadding="1" BorderColor="Gray"
								AutoGenerateColumns="False" Width="100%">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="idCorrGlob" HeaderText="idCorrGlob" Visible="False"></asp:BoundColumn>
									<asp:BoundColumn DataField="livello" ReadOnly="True" HeaderText="Liv">
										<HeaderStyle Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="descrizione" HeaderText="Unita' organizzative associate al registro">
										<HeaderStyle Width="85%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Primo smistamento">
										<HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
										<ItemTemplate>
											<asp:CheckBox ID="Chk" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivo")) %>' runat="server" />
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:datagrid>
						</DIV>
					</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td align="center"><asp:button id="btn_modifica" runat="server" CssClass="testo_btn" Text="Modifica"></asp:button>&nbsp;&nbsp;</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
