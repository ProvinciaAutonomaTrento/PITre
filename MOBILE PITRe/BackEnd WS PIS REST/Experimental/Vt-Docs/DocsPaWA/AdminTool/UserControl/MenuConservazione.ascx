<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MenuConservazione.ascx.cs"
    Inherits="DocsPAWA.AdminTool.UserControl.MenuConservazione" %>
<table border="0" cellpadding="0" cellspacing="1" width="120" bgcolor="#c0c0c0" height="100%">
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <!-- INTEGRAZIONE PITRE-PARER -->
    <!-- Se FE_WA_CONSERVAZIONE=0 visualizzo il menu conservazione classico -->
    <% if (!this.DisplayMenuPARER())
       { %>
    <tr>
        <%
            if (Request.QueryString["menuConservazione"] == "policyDocumenti")
            { %>
        <!--  TASTO : Policy Documenti  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Policy Documenti
        </td>
        <% }
            else
            { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkPolicyDocumenti" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/PolicyDocumenti.aspx?from=CON&amp;menuConservazione=policyDocumenti"
                ToolTip="Policy Documenti">Policy Documenti</asp:HyperLink>
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
            if (Request.QueryString["menuConservazione"] == "policyFascicoli")
            { %>
        <!--  TASTO : Policy fascicoli  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Policy Fascicoli
        </td>
        <% }
            else
            { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkPolicyFascicoli" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/PolicyFascicoli.aspx?from=CON&amp;menuConservazione=policyFascicoli"
                ToolTip="Policy Fascicoli">Policy Fascicoli</asp:HyperLink>
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
            if (Request.QueryString["menuConservazione"] == "policyStampe")
            { %>
        <!--  TASTO : Policy Stampe  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Policy Stampe
        </td>
        <% }
            else
            { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkPolicyStampe" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/PolicyStampe.aspx?from=CON&amp;menuConservazione=policyStampe"
                ToolTip="Policy Stampe">Policy Stampe</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <!-- MEV CONS 1.3 - GM Voci di menu abilitate/disabilitate -->
    <% if (!this.DisableAmmGestCons())
       { %>
    <tr>
        <%
        if (Request.QueryString["menuConservazione"] == "controlliAutomatici")
        { %>
        <!--  TASTO : Controlli Automatici  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Controlli Automatici
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="HyperLink1" runat="server" Target="_parent" NavigateUrl="../Gestione_Conservazione/ControlliAutomatici.aspx?from=CON&amp;menuConservazione=controlliAutomatici"
                ToolTip="Controlli Automatici">Controlli Automatici</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <% } %>
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <% if (!this.DisableAmmGestCons())
       { %>
    <tr>
        <%
        if (Request.QueryString["menuConservazione"] == "dimensioneIstanze")
        { %>
        <!--  TASTO : Dimensione Istanze  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Dimensioni Istanze
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="HyperLink2" runat="server" Target="_parent" NavigateUrl="../Gestione_Conservazione/DimensioniIstanze.aspx?from=CON&amp;menuConservazione=dimensioneIstanze"
                ToolTip="Dimensione Istanze">Dimensioni Istanze</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <% } %>
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <% if (!this.DisableAmmGestCons()) 
       { %>
    <tr>
        <%
        if (Request.QueryString["menuConservazione"] == "stampaRegistro")
        { %>
        <!--  TASTO : Stampa Registro -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Stampa Registro
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkStampaRegistro" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/StampaRegistro.aspx?from=CON&amp;menuConservazione=stampaRegistro"
                ToolTip="Stampa Registro">Stampa Registro</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <% } %>
    <tr>
        <td width="120" height="20">
            &nbsp;
        </td>
    </tr>
    <!-- MEV CONS 1.5 - Alert Conservazione -->
    <% if (!this.DisableAmmGestCons()) 
       { %>
    <tr>
    <%
        if (Request.QueryString["menuConservazione"] == "gestioneAlert")
        { %>
        <!--  TASTO : Gestione Alert -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Gestione Alert
        </td>
        <% }
        else
        { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkGestioneAlert" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/GestioneAlert.aspx?from=CON&amp;menuConservazione=gestioneAlert"
                ToolTip="Gestione Alert">Gestione Alert</asp:HyperLink>
        </td>
        <% } %>
    </tr>
    <% } %>
    <% } %>
    <% else { %>
    <!-- INTEGRAZIONE PITRE-PARER -->
    <!-- Menu conservazione PARER -->
    <tr>
        <%
            if (Request.QueryString["menuConservazione"] == "policyDocumentiPARER")
            { %>
        <!--  TASTO : Policy Documenti  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Policy Documenti
        </td>
        <% }
            else
            { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="HyperLink3" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/PolicyDocumentiPARER.aspx?from=CON&amp;menuConservazione=policyDocumentiPARER"
                ToolTip="Policy Documenti">Policy Documenti</asp:HyperLink>
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
            if (Request.QueryString["menuConservazione"] == "policyStampePARER")
            { %>
        <!--  TASTO : Policy Stampe  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Policy Stampe
        </td>
        <% }
            else
            { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="HyperLink4" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/PolicyStampePARER.aspx?from=CON&amp;menuConservazione=policyStampePARER"
                ToolTip="Policy Stampe">Policy Stampe</asp:HyperLink>
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
            if (Request.QueryString["menuConservazione"] == "respConservazione")
            { %>
        <!--  TASTO : Responsabile della conservazione  -->
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Resp. Conservazione
        </td>
        <% }
            else
            { %>
        <% if(this.GestioneMenuUserAdmin()) 
           {%>
        <td width="120" height="25" bgcolor="#ad736b" class="testo_bianco" align="left" style="padding-left: 5px;">
            Resp. Conservazione
        </td>
        <% }
           else { %>
        <td width="120" height="25" bgcolor="#800000" align="left" class="testo_bianco" style="padding-left: 5px;">
            <asp:HyperLink CssClass="menu" ID="linkRespCons" runat="server" Target="_parent"
                NavigateUrl="../Gestione_Conservazione/RespConservazione.aspx?from=CON&amp;menuConservazione=respConservazione"
                ToolTip="Responsabile della conservazione">Resp. Conservazione</asp:HyperLink>
        </td>
        <% } %>
        <% } %>
    </tr>
    <% } %>
    <tr>
        <td width="120" height="100%">
            &nbsp;
        </td>
    </tr>
</table>
