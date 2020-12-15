<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="tabRisultatiRicDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.tabRisulatiRicDoc" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register  TagPrefix="uc2" TagName="IconeRicDoc" Src="../UserControls/IconeRicerca.ascx"%>
<%@ Register TagName="MassiveOperation" TagPrefix="MassiveOperation" Src="~/UserControls/MassiveOperationButtons.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title>DOCSPA > RicercaDoc_cn</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
		<link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">		
			function ApriModalDialog()
			{					
				var val;
				val=window.confirm("Vuoi inserire il documento nell'area di Lavoro ?");
				
				if(val)
				{
					window.document.forms[0].hd1.value='Yes';				
				}
				else
				{
					window.document.forms[0].hd1.value='No';
				}			
			}
			function ApriModalDialogNewADC()
			{
	            if(window.confirm("Si vuole creare una nuova istanza di conservazione?"))
	            {
	                window.document.forms[0].hd2.value='Yes';
	            }
	            else
	            {
	                window.document.forms[0].hd2.value='No';
	            }
			}
		    function ApriModalDialogDeleteCons()
		    {
		       alert("Impossibile rimuovere il documento dall'area di conservazione");
		       return false;
		    }
		     function ApriModalDialogNewADL()
			{					
				var val;
				val=window.confirm("Vuoi eliminare il documento dall'area di Lavoro ?");
				
				if(val)
				{
					window.document.forms[0].hd1.value='Yes';
					
	
				}
				else
				{
					window.document.forms[0].hd1.value='No';
				}			
			}
			function StampaRisultatoRicerca()
			{				
				var args=new Object;
				args.window=window;
				window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=doc",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}
			
    var abilita = 'false';
    
    function wait()
    {
        if (abilita == 'false')
        {
            document.getElementById("divWait").style.display="none";
        }
        else
        {
            document.getElementById("divWait").style.display="block";
        }
    }

    function go()
    {
        document.getElementById("divWait").style.display="none";
    }
		</script>
		<script language="javascript">
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";
				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
		</script>
	</HEAD>
	<body text="#660066" vLink="#ff3366" aLink="#cc0066" link="#660066" MS_POSITIONING="GridLayout" onload="go();">
		<form id="tabRisultatiRicDoc" method="post" runat="server" onsubmit="wait();">
			<table cellSpacing="0" cellPadding="0" width="99%" align="center">
				<tr>
					<td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="90%" runat="server">
								 <asp:Label ID="msgADL" runat="server" Text="AREA DI LAVORO - " CssClass="titolo_real" Font-Bold=true Visible=false></asp:Label>
								<asp:label id="titolo" CssClass="titolo_real" Runat="server"></asp:label></td>
								<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%"></td>
								<td vAlign="middle" align="center" width="5%">
								</td>
								<td valign="middle" align="center" width="5%">
								<asp:imagebutton id="insertAllCons" Runat="server" ImageUrl="../images/proto/conservazione_d.gif" ToolTip="Inserisci tutti i documenti trovati in 'Area Conservazione'" 
                                        Width="18px" Height="17px"  OnClientClick="abilita='true';"></asp:imagebutton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
                <tr>
                    <td>
                        <MassiveOperation:MassiveOperation ID="moButtons" runat="server" 
                            CheckBoxControlId="chkSelected" DataGridId="DataGrid1" EnableViewState="true" 
                            HiddenFieldControlId="hfIdProfile" ObjType="D" Visible="true" />

                    </td>
                </tr>
				<tr id="trBody" runat="server">
					<td vAlign="top"><asp:datagrid SkinID="datagrid" id="DataGrid1" runat="server" BorderWidth="1px" Width="100%" AllowSorting="True"
							BorderStyle="Inset" AutoGenerateColumns="False" CellPadding="1" BorderColor="Gray" HorizontalAlign="Center"
							AllowPaging="True" PageSize="20" AllowCustomPaging="True" OnItemCreated="DataGrid1_ItemCreated">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
                                <asp:TemplateColumn>
                                    <HeaderStyle Wrap="false" HorizontalAlign="Center" />
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelected" runat="server" />
                                        <asp:HiddenField ID="hfIdProfile" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Doc.">
									<HeaderStyle Wrap="False" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
<%--										<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrDoc") %>'>
										</asp:Label>--%>
									<asp:LinkButton Text='<%# DataBinder.Eval(Container, "DataItem.descrDoc") %>' runat="server" ID="link_dettaglio" CommandName="Select" ToolTip="Vai alla scheda del documento" ></asp:LinkButton>
									</ItemTemplate>
									<%--<EditItemTemplate>
										<asp:TextBox id=Textbox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrDoc") %>'>
										</asp:TextBox>
									</EditItemTemplate>--%>
								</asp:TemplateColumn>
								
								 <asp:TemplateColumn HeaderText="contatore">
                                    <ItemTemplate>
                                        <asp:Label ID="Label13" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.contatore") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="ID Profilo">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								
								<asp:TemplateColumn Visible="False" HeaderText="Num Prot">
									<HeaderStyle Wrap="False"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label7 runat="server" Text='<%# getNumProt((int) DataBinder.Eval(Container, "DataItem.numProt")) %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox7 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.numProt") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Segnatura">
									<ItemTemplate>
										<asp:Label id=Label8 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox8 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Data">
									<ItemTemplate>
										<asp:Label id=Label9 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox9 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>

                                <asp:TemplateColumn Visible="False" HeaderText="Esito Pubblicazione">
									<ItemTemplate>
										<asp:Label id="l_esitoPubblicazione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.errorePubblicazione") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="txt_esitoPubblicazione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.errorePubblicazione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>

								<asp:TemplateColumn HeaderText="Registro">
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label10 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox10 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>								
								<asp:TemplateColumn HeaderText="Tipo">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label11 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProt") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox11 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProt") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Oggetto">
									<HeaderStyle HorizontalAlign="Center" Width="40%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label12 runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=Textbox12 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="mitt/dest">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label5 runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.mittdest")) %>'></asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittdest") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
<%--								<asp:TemplateColumn HeaderText="DETT">
									<HeaderStyle HorizontalAlign="Center" ></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="Label1" runat="server" CssClass="menu_1_bianco_dg">Dett.</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
                                    <asp:ImageButton id=ImageButton1 runat="server" ImageUrl="../images/proto/dettaglio.gif" BorderWidth="1px" BorderColor="Gray" CommandName="Select" AlternateText='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>' ToolTip="Vai alla scheda del documento" >
										</asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>--%>
<%--                                 <asp:TemplateColumn HeaderText="VIS">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="lbl_Vis" runat="server" CssClass="menu_1_bianco_dg">Vis.</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id=IMG_VIS runat="server" CommandName="VisDoc" ToolTip="Visualizza immagine documento">
										</asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>--%>
								<asp:TemplateColumn Visible="False"  HeaderText="chiave">
									<ItemTemplate>
										<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
<%--								<asp:TemplateColumn HeaderText="ADL">
									<HeaderStyle Wrap="False" HorizontalAlign="Center" Width="5px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<cc1:imagebutton id="Imagebutton2" Runat="server" Width="18px" Height="17" ImageUrl="../images/proto/ins_area.gif"
											CommandName="Area" AlternateText="Inserisci questo documento in 'Area di lavoro'" Tipologia="DO_GET_ADL"
											></cc1:imagebutton>
									</ItemTemplate>
								</asp:TemplateColumn>--%>
								<asp:BoundColumn Visible="False" DataField="dta_annulla"></asp:BoundColumn>
								<%--<asp:BoundColumn Visible="False" DataField="cha_img"></asp:BoundColumn>--%>
<%--								<asp:TemplateColumn Visible="False" HeaderText="in adl" >
								    <HeaderStyle Wrap="False"></HeaderStyle>
								    <ItemTemplate>
								        <asp:Label ID="inADL" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.inADL") %>'>
										</asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>--%>
<%--								<asp:TemplateColumn HeaderText="Area cons.">
								    <HeaderStyle Wrap="false" HorizontalAlign="center" Width="5px"/>
								    <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"/>
								    <ItemTemplate>
								        <cc1:ImageButton ID="img_area_lavoro" runat="server" Width="18px" Height="17px" ImageUrl="../images/proto/conservazione_d.gif" 
								        CommandName="AreaConservazione" AlternateText="Inserisci questo documento in 'Area di conservazione'" Tipologia="DO_GET_ADL" /> 
								    </ItemTemplate>
								</asp:TemplateColumn>--%>
	<%--							<asp:TemplateColumn Visible="false" HeaderText="in conservazione">
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="inConservazione" runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.inConservazione") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>--%>
	<%--							<asp:TemplateColumn Visible="false" HeaderText="firmato">
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="firmato" runat="server" Text='<%#DataBinder.Eval(Container, "DataItem.cha_firmato") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>--%>
								<asp:TemplateColumn Visible="true">
								<HeaderStyle Wrap="false" Width="5px" />
								<ItemTemplate>
							        <uc2:IconeRicDoc ID="ricerca" runat="server" ACQUISITA_IMG='<%#DataBinder.Eval(Container, "DataItem.cha_img") %>' FIRMATO='<%#DataBinder.Eval(Container, "DataItem.cha_firmato") %>' IN_ADL='<%# DataBinder.Eval(Container, "DataItem.inADL") %>' IN_CONSERVAZIONE='<%#DataBinder.Eval(Container, "DataItem.inConservazione") %>' ID_PROFILE='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>' DOC_NUMBER='<%# DataBinder.Eval(Container, "DataItem.docNumber") %>' PAGINA_CHIAMANTE="tabRisultatiRicDoc" />
								</ItemTemplate>
								</asp:TemplateColumn>
							   
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td vAlign="middle" align="center"><input id="hd1" type="hidden" value="null" name="hd1" runat="server"></td>
					<td><input id="hd2" type="hidden" value="null" name="hd2" runat="server" /></td>
				</tr>
			</table>
		</form>
    <div id="divWait" style="display:none; position:absolute; top:0; left:0; width:100%; height:600px">
        <div id="waitTrans"></div>            
        <div id="waitImg"></div>
    </div>
	</body>
</HTML>
