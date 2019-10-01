<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneModelliTrasm_Notifiche.aspx.cs" Inherits="DocsPAWA.popup.GestioneModelliTrasm_Notifiche" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
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
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">    
     <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
   <base target="_self" />	 
    <script language=javascript>    
        function SingleSelect(regex,current) 
        { 
            re = new RegExp(regex);
            for(i = 0; i < document.forms[0].elements.length;i++)
            {                
                elm = document.forms[0].elements[i];

                if (elm.type == 'checkbox' && elm != current && re.test(elm.name))                
                {
                    elm.checked = false; 
                } 
            }             
         }
    </script>          
</head>
<body>
    <form id="form1" runat="server">                
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione notifiche trasmissione" />
		    <asp:HiddenField ID="hd_mode" runat=server />
        <table cellSpacing="0" cellPadding="0" width="95%" align="center" border="0">
			    <tr><td>&nbsp;</td></tr>
				<tr>
					<td class="infoDT" align="center" height="20">
					    <asp:label id="titolo" Runat="server" CssClass="titolo_rosso"></asp:label></td>
				</tr>
				<tr>
				    <td align=center>
				        <asp:Label ID="lbl_avviso" runat="server" CssClass="testo_grigio_scuro"></asp:Label><br />
				    </td>
				</tr> 
				<tr>
				    <td align=center>			           
					    <DIV id="DivDGList" style="OVERFLOW: auto; HEIGHT: 360px">
						    <asp:datagrid id="dg_Notifiche" runat="server" SkinID="datagrid" BorderWidth="1px" 
                                CellPadding="1" BorderColor="Gray"
							    AutoGenerateColumns="False" Width="95%" onitemcreated="dg_Notifiche_ItemCreated">
							    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
							    <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
							    <Columns>									    
								    <asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Descrizione">
									    <HeaderStyle Width="80%"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Left" /> 
								    </asp:BoundColumn>
								    <asp:TemplateColumn HeaderText="Notifica">
									    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
									    <ItemTemplate>
										    <asp:CheckBox ID="Chk" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivo")) %>' Enabled='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.disabled")) %>' runat="server" />
									    </ItemTemplate>
								    </asp:TemplateColumn>
								    <asp:BoundColumn Visible="False" DataField="idPeople" HeaderText="idPeople"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="idRuolo" HeaderText="idRuolo"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="tipo" HeaderText="tipo"></asp:BoundColumn>
								    <asp:BoundColumn Visible="False" DataField="idGroup" HeaderText="idRuolo"></asp:BoundColumn>
								    <asp:TemplateColumn HeaderText="Cessione">
									    <HeaderStyle HorizontalAlign="Center" Width="10%"></HeaderStyle>
									    <ItemStyle HorizontalAlign="Center"></ItemStyle>
									    <ItemTemplate>
										    <asp:CheckBox ID="Chk_C" Checked='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.attivoC")) %>' runat="server" Enabled='<%# getCheckBox(DataBinder.Eval(Container, "DataItem.enabledC")) %>' />
									    </ItemTemplate> 
								    </asp:TemplateColumn> 
							    </Columns>
						    </asp:datagrid>
					    </DIV>
				    </td>
				</tr>				
				<tr>
					<td vAlign="middle" align="center">
					    <br />
				        <asp:button id="btn_ok" runat="server" CssClass="pulsante" Text="    SALVA e CHIUDI   " 
                            TabIndex=2 onclick="btn_ok_Click" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:button id="btn_annulla" runat="server" CssClass="pulsante" Text=" CHIUDI " 
                            TabIndex=3 onclick="btn_annulla_Click" /><br />
                        <asp:Label ID="lbl_nota" runat=server CssClass="testo_grigio"></asp:Label>
					</td>	
				</tr>
			</table>
    </form>
</body>
</html>
