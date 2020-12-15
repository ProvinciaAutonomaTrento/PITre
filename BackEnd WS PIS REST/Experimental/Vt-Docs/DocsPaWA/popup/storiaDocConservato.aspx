<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="storiaDocConservato.aspx.cs" Inherits="DocsPAWA.popup.storiaDocConservato" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Storia conservazione</title>
</head>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <base target =_self />
<body>
    <form id="form1" runat="server">
    <div>
    
        <table style="width:100%;">
            <tr>
                <td class="pulsanti">
            <asp:Label runat="server" CssClass="testo_grigio_scuro" ID="lbl_intestazione">Storia conservazione documento</asp:Label>
               </td>
            </tr>
            <tr>
                <td>
                    <asp:GridView ID="gv_dettItemsCons" runat="server" Width="100%" 
                        BorderWidth="1px" BorderColor="Gray" AllowPaging="True"
                    AutoGenerateColumns="False" 
                        onpageindexchanging="gv_dettItemsCons_PageIndexChanging" 
                        onprerender="gv_dettItemsCons_PreRender" 
                        onselectedindexchanged="gv_dettItemsCons_SelectedIndexChanged" EmptyDataText="Il Documento non è mai stato conservato"  EmptyDataRowStyle-CssClass="testo_grigio" EmptyDataRowStyle-HorizontalAlign="Center" >                                       
                        <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06" />
                        <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" />
                        <AlternatingRowStyle CssClass="bg_grigioA" />
                        <RowStyle CssClass="bg_grigioN" />               
                        <SelectedRowStyle CssClass="bg_grigioS" />
                        <Columns>
                        <asp:BoundField DataField="id_conservazione" HeaderText="Id istanza cons." />
                        <asp:BoundField DataField="descrizione" HeaderText="Descrizione istanza" />
                        <asp:BoundField DataField="data_riversamento" HeaderText="Data Conservazione" />
                        <asp:BoundField DataField="userId" HeaderText="Utente richiedente" />
                        <asp:BoundField DataField="collFisica" HeaderText="Collocazione fisica" />
                        <asp:TemplateField HeaderText="Tipo cons.">
                         <ItemTemplate>
                                    <asp:Label ID="tipo_cons" runat="server" Text='<%# Bind("tipo_cons") %>'></asp:Label>
                                </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="num_docInFasc" HeaderText="Numero totale documenti" />
                            <asp:TemplateField HeaderText="ID profile trasmissione" Visible="False">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" 
                                        Text='<%# Bind("id_profile_trasm") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lbl_idProfileTrasm" runat="server" Text='<%# Bind("id_profile_trasm") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DETT.">
                                <ItemTemplate>
                                    <asp:ImageButton ID="img_dettaglio" runat="server" ImageUrl="~/images/proto/dettaglio.gif"  CommandName="Select" CausesValidation="false" ToolTip="Dettaglio"/>                                    
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </td>                
            </tr>          
        </table>
    
    </div>
    </form>
</body>
</html>
