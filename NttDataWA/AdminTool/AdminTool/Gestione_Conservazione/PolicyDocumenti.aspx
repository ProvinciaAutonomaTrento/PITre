<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PolicyDocumenti.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_Conservazione.PolicyDocumenti" %>

<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuConservazione" Src="../UserControl/MenuConservazione.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Conservazione - Policy Documenti</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <script language="JavaScript" type="text/javascript">

        var cambiapass;
        var hlp;

        function apriPopup() {
            hlp = window.open('../help.aspx?from=CON', '', 'width=450,height=500,scrollbars=YES');
        }
        function cambiaPwd() {
            cambiapass = window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
        }
        function ClosePopUp() {
            if (typeof (cambiapass) != 'undefined') {
                if (!cambiapass.closed)
                    cambiapass.close();
            }
            if (typeof (hlp) != 'undefined') {
                if (!hlp.closed)
                    hlp.close();
            }
        }
        // L'URL della finestra per creare una nuova policy
        var _url_new_policy = '<%=UrlNewPolicy %>';

        var _url_period_policy = '<%=UrlPeriodPolicy %>';

        var _url_view_policy = '<%=UrlViewPolicy %>';

        function NewPolicyDocumenti() {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);

            var retval = window.showModalDialog(_url_new_policy, 'NewPolicyDocumenti', 'dialogWidth:700px;dialogHeight:780px;status:no;resizable:no;scroll:yes;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                window.document.getElementById('<%= Page.Form.ClientID %>').submit();
            }
        }

        function OpenPeriodDetails(ret) {
            var newLeft = (screen.availWidth / 2 - 225);
            var newTop = (screen.availHeight - 704);
            var retval = window.showModalDialog(_url_period_policy + "?id=" + ret, 'OpenPeriodDetails', 'dialogWidth:700px;dialogHeight:750px;status:no;resizable:no;scroll:yes;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
            if (retval != null) {
                window.document.getElementById('<%= Page.Form.ClientID %>').submit();
            }
        }

        function Refresh() 
        {
           window.document.getElementById('<%= Page.Form.ClientID %>').submit();
       }

       function OpenPolicyDetails(id) {
           var newLeft = (screen.availWidth / 2 - 225);
           var newTop = (screen.availHeight - 704);
           var retval = window.showModalDialog(_url_view_policy + "?id=" + id, 'OpenPolicyDetails', 'dialogWidth:700px;dialogHeight:780px;status:no;resizable:no;scroll:yes;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
           if (retval != null) {
               window.document.getElementById('<%= Page.Form.ClientID %>').submit();
           }
       }

    </script>
    <style type="text/css">
        .tab_sx
        {
            color: #333333;
            text-align: left;
        }
        .tab_sx2
        {
            color: #333333;
            text-align: center;
        }
        .tab_format
        {
            border-collapse: collapse;
            margin: 0px;
            padding: 0px;
            width: 95%;
        }
        .tab_format td
        {
            margin: 0px;
            padding: 0px;
            padding: 2px;
            font-size: 11px;
            border: 1px solid #cccccc;
        }
        .head_tab
        {
            background-color: #810101;
            color: #ffffff;
            font-size: 12px;
            text-align: center;
        }
        .tab_format img
        {
            border: 0px;
        }
        #button_new
        {
            text-align: center;
            margin: 0px;
            padding: 0px;
            margin-top: 20px;
            margin-bottom: 10px;
        }
        .cbtn
        {
            font-family: Verdana;
            font-size: 11px;
            margin: 0px;
            padding: 0px;
            padding: 2px;
            width: 120px;
            height: 25px;
            color: #ffffff;
            border: 1px solid #ffffff;
            background-image: url('../images/bg_button.gif');
        }
    </style>
</head>
<body onunload="ClosePopUp()" style="margin: 0px; padding: 0px;">
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Conservazione > Policy Documenti" />
    <!-- Gestione del menu a tendina -->
    <uc3:MenuTendina ID="MenuTendina" runat="server"></uc3:MenuTendina>
    <table height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <!-- STRISCIA SOTTO IL MENU -->
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- STRISCIA DEL TITOLO DELLA PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Conservazione -> Policy Documenti
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                <table height="100" cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tr>
                        <td width="120" height="100%">
                            <uc2:MenuConservazione ID="MenuConservazione" runat="server"></uc2:MenuConservazione>
                        </td>
                        <td width="100%" height="100%" valign="middle" align="center">
                            <div id="DivSel" style="overflow: auto; height: 551px" class="testo_grigio_scuro">
                                <div id="button_new">
                                    <asp:Button ID="btn_new_policy" runat="server" CssClass="cbtn" Text="Nuova Policy" />
                                </div>
                                <asp:UpdatePanel ID="box_policy" runat="server" UpdateMode="Conditional">
                                    <contenttemplate>
                                <div align="center">
                                    <asp:DataGrid ID="grvPolicy" runat="server" AllowSorting="false" AutoGenerateColumns="false"
                                        AllowPaging="False" CssClass="tab_format" OnItemCreated="DataGrid_ItemCreated" OnItemDataBound="ImageCreatedRender">
                                        <AlternatingItemStyle BackColor="#f2f2f2" />
                                        <ItemStyle BackColor="#d9d9d9" ForeColor="#333333" />
                                        <Columns>
                                            <asp:TemplateColumn Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetPolicyID((SAAdminTool.DocsPaWR.Policy)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="AUTOMATICO" runat="server" Text='<%# this.GetAutomaticPeriod((SAAdminTool.DocsPaWR.Policy)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn ItemStyle-Width="74%" HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab"
                                                HeaderText="NOME">
                                                <ItemTemplate>
                                                    <asp:Label ID="NAMEPOLICY" runat="server" Text='<%# this.GetPolicyName((SAAdminTool.DocsPaWR.Policy)Container.DataItem) %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="PERIODICITA'" ItemStyle-CssClass="tab_sx2"
                                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                <ItemTemplate>
                                                 <asp:ImageButton ID="btn_periodo" runat="server" CssClass="testo_grigio" CommandName="Automatica"
                                                    AlternateText="Vai al dettaglio del periodo" ImageUrl='<%# this.GetImagePeriod((SAAdminTool.DocsPaWR.Policy)Container.DataItem) %>'
                                                    ToolTip="Vai al dettaglio del periodo" OnClick="ViewPeriodDetails" ></asp:ImageButton>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="DETTAGLI" ItemStyle-CssClass="tab_sx2"
                                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                <ItemTemplate>
                                                <asp:ImageButton ID="btn_dettagli" runat="server" CssClass="testo_grigio" CommandName="Dettaglio"
                                                    AlternateText="Vedi il dettaglio" ImageUrl="../../images/proto/dett_lente_doc.gif"
                                                    ToolTip="Vedi il dettaglio" OnClick="ViewDetails"></asp:ImageButton>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:TemplateColumn ItemStyle-Width="8%" HeaderText="ELIMINA" ItemStyle-CssClass="tab_sx2"
                                                HeaderStyle-HorizontalAlign="left" HeaderStyle-CssClass="head_tab">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btn_Rimuovi" runat="server" CssClass="testo_grigio" CommandName="Rimuovi"
                                                        AlternateText="Rimuovi" ImageUrl="../../images/ricerca/cancella_griglia.gif"
                                                        ToolTip="Rimuovi" OnClick="DeletePolicy"></asp:ImageButton>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                        </Columns>
                                    </asp:DataGrid>
                                </div>
                                </contenttemplate>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
                <!-- FINE CORPO CENTRALE -->
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
