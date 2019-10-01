<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CampiProfilati.aspx.cs"
    Inherits="SAAdminTool.AdminTool.Gestione_Conservazione.CampiProfilati" %>

<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Seleziona campi profilati per ricerca</title>
    <base target="_self" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <script language="JavaScript" src="../../CSS/ETcalendar.js"></script>
    <LINK href="../../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
    <LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">		
    <script language="javascript">
        function chiudiFinestra() {
            window.close();
        }
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }
    </script>
    <script>
        function clearSelezioneEsclusiva(id, numeroDiScelte) {
            numeroDiScelte--;
            while (numeroDiScelte >= 0) {
                var elemento = id + "_" + numeroDiScelte;
                document.getElementById(elemento).checked = false;
                numeroDiScelte--;
            }
        }
    </script>
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
    </script>
    <style type="text/css">
        body
        {
            font-family: Verdana;
            background-color:#fafafa;
        }
        #container
        {
            float: left;
            width: 100%;
            height: 590px;
            overflow-y: scroll;
        }
        #content
        {
            border: 1px solid #cccccc;
            text-align: center;
            width: 98%;
            float: left;
            margin-left: 6px;
            margin-top: 5px;
            background-color:#ffffff;
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
            height: 305px;
            padding-top: 10px;
            overflow-y: scroll;
        }
        .contenitore_box
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
        }
        .contenitore_box fieldset
        {
            border: 1px solid #eaeaea;
            margin: 5px;
            padding: 0px;
        }
        .contenitore_box legend
        {
            font-size: 10px;
            font-weight: normal;
            color: #4b4b4b;
            font-family: Verdana;
            margin-left: 15px;
            font-weight: bold;
        }
        .contenitore_box_due
        {
            text-align: left;
            font-family: Tahoma, Arial, sans-serif;
            margin: 5px;
            margin-bottom: 10px;
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
        .titolo_scheda
        {
            font-size: 11px;
            color: #666666;
            font-weight: bold;
        }
        .testo_grigio
        {
            font-size: 11px;
            font-family: Verdana;
            color: #333333;
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
            font-size: 10px;
            border: 1px solid #cccccc;
        }
        .head_tab
        {
            background-color: #810101;
            color: #ffffff;
            font-size: 11px;
            text-align: center;
            padding:2px;
            font-weight:bold;
        }
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
        #button
        {
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
     <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Anteprima" />
    <asp:HiddenField ID="hid_tab_est" runat="server" />
    <div id="container">
        <div id="content">
            <div align="center">
                <table width="100%" border="0">
                    <tr>
                        <td class="head_tab">
                            <asp:Label ID="lbl_NomeModello" runat="server" Text="Cerca nei campi profilati"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="Label_Avviso" runat="server" Font-Bold="True" Font-Size="12px" ForeColor="Red"
                                Visible="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_profDinamica" align="left">
                            <asp:Panel ID="panel_Contenuto" runat="server">
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="Table3" align="center">
                <tr>
                    <td>
                        <asp:Button ID="btn_ConfermaProfilazione" runat="server" Text="Conferma" CssClass="cbtn" OnClick="BtnSaveDocumentFormat_Click">
                        </asp:Button>
                    </td>
                    <td>
                        <asp:Button ID="btn_resettaPopUp" runat="server" Text="Resetta" CssClass="cbtn" OnClick="btn_resettaPopUp_Click">
                        </asp:Button>
                    </td>
                    <td>
                        <asp:Button ID="btn_Chiudi" runat="server" Text="Chiudi" CssClass="cbtn" OnClick="BtnCloseNotSave_Click">
                        </asp:Button>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
