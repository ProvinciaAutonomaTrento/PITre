<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CampiProfilati.aspx.cs"
    Inherits="ConservazioneWA.PopUp.CampiProfilati" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Seleziona campi profilati per ricerca</title>
    <base target="_self" />
    <link href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet"/>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet"/>
    <script language="javascript" type="text/javascript">
        function chiudiFinestra() {
            window.close();
        }
        function close_and_save(ret) {
            window.returnValue = ret;
            window.close();
        }
        function clearSelezioneEsclusiva(id, numeroDiScelte) {
            numeroDiScelte--;
            while (numeroDiScelte >= 0) {
                var elemento = id + "_" + numeroDiScelte;
                document.getElementById(elemento).checked = false;
                numeroDiScelte--;
            }
        }
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
            background-color: #fafafa;
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
            background-color: #ffffff;
  
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
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
            color: #ffffff;
            font-size: 11px;
            text-align: center;
            padding: 2px;
            font-weight: bold;
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
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server">
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
                        <asp:Button ID="btn_ConfermaProfilazione" runat="server" Text="Conferma" CssClass="cbtn"
                            OnClick="BtnSaveDocumentFormat_Click"></asp:Button>
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
