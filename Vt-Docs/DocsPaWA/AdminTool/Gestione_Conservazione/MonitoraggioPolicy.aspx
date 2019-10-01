<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonitoraggioPolicy.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Conservazione.MonitoraggioPolicy" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="head" runat="server">
    <title>Monitoraggio policy</title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <script src="../../LIBRERIE/rubrica.js" type="text/javascript"></script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            overflow-x: hidden;
            overflow-y: scroll;
        }
        #container
        {
            float: left;
            width: 99%;
            background-color: #ffffff;
            /*height: 900px;*/
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 97%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
        }
        .title 
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 4px;
            padding-bottom: 4px;
            background-color: #810101;
            color: #ffffff;
        }
        #content_field
        {
            width: 100%;
            float: left;
            background-color: #fbfbfb;
            /*height: 845px;*/
        }
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
        }
        
        .contenitore_box_due fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 5px;
        }
        
        .contenitore_box_due legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 5px;
        }
        .tabInput
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            padding: 10px;
            width: 100%;
        }
        table.tabInput td.legend
        {
            width: 25%;
            text-align: left;
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
            padding-bottom: 10px;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
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
<body>
    <form id="Form" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
        <div id="container">
            <div id="content">
                <asp:Label runat="server" ID="titlePage" Text="Monitoraggio policy" CssClass="title"></asp:Label>
                <div id="content_field">
                    <asp:UpdatePanel ID="UpdatePanelSelCrit" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="contenitore_box_due">
                                <fieldset>
                                    <legend>Filtri</legend>
                                    <table class="tabInput">
                                        <tr>
                                            <td class="legend">Amministrazione</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_amministrazione" runat="server" CssClass="testo_grigio" Width="300px"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">Codice policy</td>
                                            <td>
                                                <asp:TextBox ID="txtCodPolicy" runat="server" CssClass="testo_grigio" MaxLength="32"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">Descrizione policy</td>
                                            <td>
                                                <asp:TextBox ID="txtDescPolicy" runat="server" CssClass="testo_grigio" MaxLength="200" Width="300px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="legend">Data esecuzione policy</td>
                                            <td>
                                                <asp:DropDownList ID="ddl_dataEsecuzione" runat="server" CssClass="testo_grigio" AutoPostBack="true" Width="30%" OnSelectedIndexChanged="ddl_dataEsecuzione_SelectedIndexChanged">
                                                    <asp:ListItem Value="S">Valore singolo</asp:ListItem>
                                                    <asp:ListItem Value="R">Intervallo</asp:ListItem>
                                                    <asp:ListItem Value="M">Mese corrente</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:label id="lblDa" runat="server" CssClass="testo_grigio" Width="18px">Da</asp:label>
                                                <uc4:Calendario id="lbl_dataCreazioneDa" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                                <asp:label id="lblA" runat="server" CssClass="testo_grigio" Width="18px">A</asp:label>
                                                <uc4:Calendario id="lbl_dataCreazioneA" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:Panel ID="pnlButtons" runat="server">
                            <table id="Table3" align="center">
                                <tr>
                                    <td align="center">
                                        <asp:Button id="btn_export" runat="server" CssClass="cbtn" Text="Esporta" OnClick="btn_export_Click"></asp:Button>
                                    </td>
                                    <td align="center">
                                        <asp:Button ID="btn_close" runat="server" CssClass="cbtn" Text="Chiudi" OnClientClick="window.close();"></asp:Button>
                                    </td>
                                </tr>
                            </table>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
