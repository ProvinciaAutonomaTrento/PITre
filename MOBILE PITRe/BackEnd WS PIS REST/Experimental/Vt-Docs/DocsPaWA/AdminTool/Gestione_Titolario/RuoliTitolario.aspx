<%@ Page language="c#" Codebehind="RuoliTitolario.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Titolario.RuoliTitolario" %>
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
		
			function VisualizzaAttendi() {
			    
				window.document.body.style.cursor='wait';
				window.scroll(0,0);
			}
			
			function confirmDisattivazione()
		    {
		        var result = true;
		        window.scroll(0,0);
		        result = confirm('Si vuole procedere con la modifica della visibilità del titolario per questi ruoli?');
                if(result == true)
                    VisualizzaAttendi();

                document.getElementById("btnOK").style.display = 'none';
                
                return result;
		    }	

			function CloseWindow()
			{
				window.close();
			}
			
			function ShowWaitCursor()
			{
				window.document.frmRuoliTitolario.btnOK.style.cursor="wait";
            }

            function ShowValidationResultDialog() {
                var retVal = window.showModalDialog('../EsitoOperazioneViewerDialog.aspx?maskTitle=Esito modifica',
                            '',
                            'dialogWidth:700px;dialogHeight:500px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no'); 
            }
			
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="frmRuoliTitolario" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Ruoli con visibilità sul titolario" />
 			<TABLE class="tblContainer" cellSpacing="1" cellPadding="0" width="95%" border="0" align="center">
				<tr align="right">
					<td>
						<asp:checkbox id="chkSelectDeselectAll" runat="server" AutoPostBack="True" CssClass="testo_grigio_scuro"
							Text="Seleziona / deseleziona tutti" TextAlign="left" Checked="False" Visible="true"></asp:checkbox>
					    <asp:RadioButtonList ID="rbAttivaDisattivaAll" runat="server" CellSpacing="0" CellPadding="0" AutoPostBack="true" Visible="true" CssClass="testo_grigio_scuro">
					        <asp:ListItem Text="Attiva Tutto" Value="Attiva" Selected="False"></asp:ListItem>
					        <asp:ListItem Text="Disattiva Tutto" Value="Disattiva" Selected="False"></asp:ListItem>
					        <asp:ListItem Text="Annulla Selezione" Value="Annulla" Selected="True"></asp:ListItem>
					    </asp:RadioButtonList>
					</td>
				</tr>
				<tr align="left">
					<td class="pulsanti">
						<asp:Label id="lblItemsCounter" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
					</td>
				</tr>
				<tr height="3">
					<td></td>
				</tr>
                <tr>
		            <td class="testo_grigio_scuro_grande">Ricerca per :
			            <asp:DropDownList id="ddl_ricTipo" Runat="server" CssClass="testo_grigio_scuro" 
                            AutoPostBack="true" Width="22%"
                            onselectedindexchanged="ddl_ricTipo_SelectedIndexChanged">
                            <asp:ListItem Value="T">Tutti</asp:ListItem>
				            <asp:ListItem Value="COD_RUOLO">Codice Ruolo</asp:ListItem>
					        <asp:ListItem Value="DES_RUOLO">Descr. Ruolo</asp:ListItem>				
				        </asp:DropDownList>
			            <asp:textbox id="txt_ricerca"  CssClass="testo_grigio_scuro_grande" Runat="server" Width="45%" ReadOnly="true" BackColor="Gainsboro"></asp:textbox>
                        <asp:button id="btn_find" CssClass="testo_btn" Runat="server" Text="Cerca" OnClick="btn_find_Click"></asp:button>
			        </td>
                </tr>
                <tr>
	        <td colspan="2" style="height:10px; text-align:center;">
	            <asp:label id="lbl_ricercaRuoli" runat="server" CssClass="titolo"></asp:label>
			</td>
	    </tr>
				<TR>
					<TD align="center">
						<DIV id="DivDGList" style="OVERFLOW: auto; height: 450px; WIDTH: 100%">
                            <asp:datagrid id="grdRuoliTitolario" runat="server" Width="100%" 
                                BorderWidth="1px" AutoGenerateColumns="False"
								BorderColor="Gray" CellPadding="1">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
								<Columns>
									<asp:BoundColumn Visible="False" DataField="ID_RUOLO"></asp:BoundColumn>
									<asp:BoundColumn DataField="CODICE_RUOLO" HeaderText="Codice">
										<HeaderStyle Width="25%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:BoundColumn DataField="DESCRIZIONE_RUOLO" HeaderText="Descrizione">
										<HeaderStyle Width="65%"></HeaderStyle>
									</asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Attivo">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:CheckBox id="chkRuoloAttivo" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.GRUPPOASSOCIATO") %>'>
											</asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:ButtonColumn Text="&lt;img src=../Images/ext_down3.gif border=0 alt='Attiva la visibilità su tutti i nodi inferiori'&gt;"
										CommandName="ExtDown" HeaderText="+">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></ItemStyle>
									</asp:ButtonColumn>
									<asp:TemplateColumn HeaderText="Attiva" Visible="false">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
										    <asp:CheckBox ID="checkBox1" runat="server" AutoPostBack="true" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Disattiva" Visible="false">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
										    <asp:CheckBox ID="checkBox2" runat="server" AutoPostBack="true" />
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:ButtonColumn Text="&lt;img src=../Images/ext_down3_barr.gif border=0 alt='Disattiva la visibilità su tutti i nodi inferiori'&gt;"
										CommandName="ExtDownNo" HeaderText="-">
										<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></ItemStyle>
									</asp:ButtonColumn>
								</Columns>
								<PagerStyle Mode="NumericPages"></PagerStyle>
							</asp:datagrid>
                        </DIV>
					</TD>
				</TR>
				<tr height="5">
					<td></td>
				</tr>
				<TR>
					<TD vAlign="middle" align="center">
					    <asp:button id="btnOK" runat="server" CssClass="testo_btn" Text="Modifica"  OnClientClick="return confirmDisattivazione()"></asp:button>&nbsp;
						<asp:button id="btnCancel" runat="server" CssClass="testo_btn" Text="Chiudi"></asp:button>
				    </TD>
				</TR>
			</TABLE>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #ffc37d">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Attendere 
							prego...
						</td>
					</tr>
				</table>
			</DIV>
		</form>
	</body>
</HTML>
