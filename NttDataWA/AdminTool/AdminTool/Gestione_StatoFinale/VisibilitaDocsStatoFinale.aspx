<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisibilitaDocsStatoFinale.aspx.cs" Inherits="SAAdminTool.popup.VisibilitaDocsStatoFinale" %>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="HEAD1" runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		 <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet"/>
         
		<base target="_self" />
		<%Response.Expires=-1;%>
       
</head>
<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" 
    style="height: 379px; width: 526px;">
 <script type="text/javascript">
     function Closewindow(refrsh) {


         if (refrsh == 'true') {

             window.returnValue = '1';
           
         }
         else {
             window.returnValue = '';
         }
         window.close();



     }



 
 
 </script>
       
		<form id="visibilitaDocumento" method="post" runat="server">
       
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Visibilità documento" />
    <div style="width: 525px">
    <table id="Table1" class="info"border="0" >
				<tr>
					<td style="height:10px; width: 140px;" class="pulsanti" colspan="3" >
                    <asp:Label ID="lblmodDoc" runat="server"  CssClass="testo_grigio_scuro" 
                            Width="287px"></asp:Label></td>
                    <td  align="right" style="height:10px;" class="pulsanti">
                   <asp:Button id="btn_conferma" runat="server" Text="Conferma" 
                            ToolTip="Conferma lo sblocco del documento per ID ruoli selezionati" 
                            CssClass="testo_btn" Width="91px" onclick="btn_conferma_Click"/>
                     <asp:Button id="btn_Chiudi" runat="server" Text="Chiudi" 
                            ToolTip="Chiude la finestra annullando le modifiche non confermate" CssClass="testo_btn" 
                            Width="91px" OnClientClick="Closewindow('false');"/>
                            
                    </td>
				</tr>
                <tr>
                <td valign="top" colspan="4">
					    <div class="div_Visibilita" id="divVis">
						<asp:datagrid id="dg_Visibilita" Width="99%"  runat="server" 
                                BorderColor="Gray" BorderWidth="1px"
							CellPadding="1" HorizontalAlign="Center" AllowPaging="True" AutoGenerateColumns="False" 
                                onpageindexchanged="dg_Visibilita_PageIndexChanged" >
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></HeaderStyle>
							<Columns>
                                <asp:TemplateColumn Visible="false">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
                                        <asp:Label ID="lblIdRuolo" runat="server"
                                            Text = '<%#((SAAdminTool.DocsPaWR.DocumentoDiritto) Container.DataItem).personorgroup%>'>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                         </asp:Label>
									</ItemTemplate>									
								</asp:TemplateColumn>									
                                <asp:TemplateColumn HeaderText="Ruolo" ItemStyle-HorizontalAlign="Left">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
                                        <asp:Label ID="lblCodiceRuolo" runat="server" Font-Bold="true" Text = '<%#((SAAdminTool.DocsPaWR.DocumentoDiritto) Container.DataItem).soggetto.codiceRubrica%>'></asp:Label>
                                        <br />
                                        <asp:Label ID="lblRuolo" runat="server" Text = '<%#((SAAdminTool.DocsPaWR.DocumentoDiritto) Container.DataItem).soggetto.descrizione%>'></asp:Label>
									</ItemTemplate>									

<ItemStyle HorizontalAlign="Left"></ItemStyle>
								</asp:TemplateColumn>									
								<asp:TemplateColumn HeaderText="">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:CheckBox ID="cb_Abilita" runat="server" Checked='<%#this.IsUnLocked((SAAdminTool.DocsPaWR.DocumentoDiritto) Container.DataItem)%>'  AutoPostBack="False" />
									</ItemTemplate>
								</asp:TemplateColumn>							
							</Columns>
							<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" PageButtonCount="20" CssClass="bg_grigioN"
                                        Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						</div>
					</td>
                </tr>
   </table>
    </div>
    </form>
</body>
</html>
