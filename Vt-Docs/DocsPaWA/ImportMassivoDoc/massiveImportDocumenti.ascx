<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="massiveImportDocumenti.ascx.cs" Inherits="DocsPAWA.ImportMassivoDoc.massiveImportDocumenti" %>
<script type="text/javascript" language="javascript" src="jDataScript.js" ></script >
<TABLE id="tableLog" width="100%" cellpadding="2" cellspacing="0">
    <TR style="background-color:AntiqueWhite;">
	    <TD align="left" height="90%">
		    <asp:label id="titolo" Runat="server" CssClass="titolo">Log dell'importazione</asp:label>
	    </TD>
	    <TD class="testo_grigio_scuro" vAlign="middle" align="right" height="10%">									    
		    <asp:ImageButton ID="btn_stampa" Visible="true" Runat="server" AlternateText="Esporta il log delle operazioni."
			    ImageUrl="../images/proto/export.gif"  OnClientClick="lanciaVisPdf();" 
                onclick="btn_stampa_Click"></asp:ImageButton>
	    </TD>
    </TR>
<tr>
<td colspan=2>
<div style="overflow:auto; height:206px;">
    <asp:DataGrid ID="dgrLog" runat="server" 
    BorderColor="Gray"  AutoGenerateColumns ="false" 
    BorderWidth="1px"   CellPadding="1"  
    AllowPaging="false" Width="100%">
    <ItemStyle CssClass="bg_grigioN" />
    <SelectedItemStyle CssClass="bg_grigioS" />
    <AlternatingItemStyle CssClass="bg_grigioA" />
    <HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06"/>
     <Columns>
     <asp:BoundColumn datafield="TS"  HeaderText="Data" ItemStyle-Width="20%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
     <asp:BoundColumn datafield="Type"  HeaderText="Tipo" ItemStyle-Width="8%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
     <asp:BoundColumn datafield="Absolutepath"  HeaderText="File" ItemStyle-Width="45%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
      <asp:BoundColumn datafield="ErrorMessage"  HeaderText="Esito" ItemStyle-Width="27%" Readonly="true" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
    </Columns>				        
    </asp:DataGrid>
    </div>
    </td>
</tr>
</TABLE>