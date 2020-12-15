<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MenuImportPregressi.ascx.cs" Inherits="DocsPAWA.AdminTool.UserControl.MenuImportPregressi" %>
<table border="0" cellpadding="0" cellspacing="1" width="120" bgcolor="#c0c0c0" height="100%">
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <tr>
        <%
            if (Request.QueryString["menuImportPregressi"] == "nuovoImport" || Request.QueryString["menuImportPregressi"]==null || string.IsNullOrEmpty(Request.QueryString["menuImportPregressi"].ToString()))
            { %>
        <!--  TASTO : Nuovo import  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left:5px;">
           Nuovo Import
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left:5px;">
            <asp:HyperLink CssClass="menu" ID="linkNuovoImport" runat="server" Target="_parent" NavigateUrl="../Gestione_Import/GestImport.aspx?from=IMP&amp;menuImportPregressi=nuovoImport"
                ToolTip="Policy Documenti">Nuovo Import</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <tr>
        <%
            if (Request.QueryString["menuImportPregressi"] == "vediImport")
            { %>
        <!--  TASTO : Stato import  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left:5px;">
            Stato Import
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left:5px;">
            <asp:HyperLink CssClass="menu" ID="linkStatoImport" runat="server" Target="_parent" NavigateUrl="../Gestione_Import/ReportPregressi.aspx?from=IMP&amp;menuImportPregressi=vediImport"
                ToolTip="Stato Import">Stato Import</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <tr>
        <td width="120" height="100%">
            &nbsp;
        </td>
    </tr>
</table>
