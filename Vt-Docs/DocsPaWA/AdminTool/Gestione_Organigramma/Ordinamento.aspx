<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Ordinamento.aspx.cs" Inherits="Amministrazione.Gestione_Organigramma.Ordinamento" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title></title>
        <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
        <base target=_self></base>
    </head>
    <body bottomMargin="5" leftMargin="5" topMargin="5" rightMargin="5">
        <form id="form1" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Organigramma > Ordinamento" />    
            <TABLE width="100%">
			    <TR>
				    <TD class="titolo_pnl">UO selezionata: <asp:Label ID="lbl_desc_uo" runat=server></asp:Label></TD>						  
			    </TR>			    													
		        <TR>
		            <TD>    
		                <DIV style="OVERFLOW: auto; HEIGHT: 370px">
		                <table border=0 cellpadding=0 cellspacing=0 width=100%>
		                    <tr>
		                        <td>                                                           
                                    <asp:DataGrid id="dg_ord_ruoli" runat="server" AutoGenerateColumns="False"
					                    CellPadding="1" BorderWidth="1px" BorderColor="Gray" Width=100% 
                                        onitemcommand="dg_ord_ruoli_ItemCommand">
					                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
					                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
					                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
					                    <Columns>
						                    <asp:BoundColumn Visible="False" DataField="idCorrGlobale" ReadOnly="True" HeaderText="idCorrGlobale"></asp:BoundColumn>																			
						                    <asp:BoundColumn Visible="False" DataField="idPeso" ReadOnly="True" HeaderText="idPeso"></asp:BoundColumn>
						                    <asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Ruoli">
							                    <ItemStyle Width="90%"></ItemStyle>
						                    </asp:BoundColumn>
                                            <asp:ButtonColumn Text="&lt;img src=../Images/up_trasp.gif border=0 alt='Dettaglio'&gt;" CommandName="Up">
	                                            <HeaderStyle Width="5%"></HeaderStyle>
	                                            <ItemStyle HorizontalAlign="Center" VerticalAlign=Middle></ItemStyle>
                                            </asp:ButtonColumn>    
                                            <asp:ButtonColumn Text="&lt;img src=../Images/down_trasp.gif border=0 alt='Dettaglio'&gt;" CommandName="Down">
	                                            <HeaderStyle Width="5%"></HeaderStyle>
	                                            <ItemStyle HorizontalAlign="Center" VerticalAlign=Middle></ItemStyle>
                                            </asp:ButtonColumn>                													    																							
					                    </Columns>
				                    </asp:DataGrid>   
				                </td>                                                            
		                    </tr>
		                    <tr><td>&nbsp;</td></tr>
		                    <tr>
		                        <td>                                                                
                                    <asp:DataGrid id="dg_ord_uo" runat="server" AutoGenerateColumns="False"
					                    CellPadding="1" BorderWidth="1px" BorderColor="Gray" Width=100% 
                                        onitemcommand="dg_ord_uo_ItemCommand">
					                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
					                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
					                    <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
					                    <Columns>
						                    <asp:BoundColumn Visible="False" DataField="idCorrGlobale" ReadOnly="True" HeaderText="idCorrGlobale"></asp:BoundColumn>																			
						                    <asp:BoundColumn Visible="False" DataField="idPeso" ReadOnly="True" HeaderText="idPeso"></asp:BoundColumn>
						                    <asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="UO inferiori">
							                    <ItemStyle Width="90%"></ItemStyle>
						                    </asp:BoundColumn>
                                            <asp:ButtonColumn Text="&lt;img src=../Images/up_trasp.gif border=0 alt='Dettaglio'&gt;" CommandName="Up">
	                                            <HeaderStyle Width="5%"></HeaderStyle>
	                                            <ItemStyle HorizontalAlign="Center" VerticalAlign=Middle></ItemStyle>
                                            </asp:ButtonColumn>    
                                            <asp:ButtonColumn Text="&lt;img src=../Images/down_trasp.gif border=0 alt='Dettaglio'&gt;" CommandName="Down">
	                                            <HeaderStyle Width="5%"></HeaderStyle>
	                                            <ItemStyle HorizontalAlign="Center" VerticalAlign=Middle></ItemStyle>
                                            </asp:ButtonColumn>                													    																							
					                    </Columns>
				                    </asp:DataGrid>   
				                </td>
				            </tr>
				        </table> 
				        </DIV>                                                           
		            </TD>																    
		        </TR>
		        <TR>
		            <TD align=center>
		                &nbsp;
		            </TD>
		        </TR>	
		        <TR>
		            <TD align=center>
		                <asp:Button ID="btn_chiudi" runat=server CssClass="testo_btn_org" Text="Chiudi" 
                            ToolTip="Chiudi" onclick="btn_chiudi_Click" />
		            </TD>
		        </TR>	
		        <TR>
		            <TD align=center>
		                &nbsp;
		            </TD>
		        </TR>												 													
		    </TABLE> 
        </form>
    </body>
</html>
