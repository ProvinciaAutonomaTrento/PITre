<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalvaLogDialog.aspx.cs" Inherits="ConservazioneWA.PopUp.SalvaLogDialog" %>

<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Salva log</title>
    <base target="_self" />
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        #header
        {
            background: url(../Img/bg_header.png) repeat-x scroll;
        }
        
        #menutop
        {
            background: url(../Img/bg_menutop.png) repeat-x scroll;
        }
        
        .altro a:link
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:visited
        {
            background-image: url('../Img/bg_menutop_no_hover.png');
        }
        
        .altro a:hover
        {
            background-image: url('../Img/bg_menutop_hover.png');
        }
        
        .sonoqui a:link
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:visited
        {
            background-image: url('../Img/sono_qui.png');
            font-weight: bold;
        }
        
        .sonoqui a:hover
        {
            background-image: url('../Img/sono_qui_hover.png');
        }
        
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
            width: 70px;
            
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
            width: 70px;
        }
                
        .tab_istanze_header
        {
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }
        
        #content
        {
            background-image: url('../Img/bg_content.jpg');
        }
        
        .menu_pager_grigio
        {
            background-image: url('../Img/bg_pager_table.jpg');
            background-repeat: repeat-x;
        }
        
        TD.pulsanti
        {
            background-color: #4885a4;
            color: #ffffff;
            font-size: 11px;
            width: 95%;
            padding: 5px;
            font-weight: bold;
            text-align: center;
            margin-top: 5px;
            background-image: url('../Img/bg_tab_header.jpg');
            background-repeat: repeat-x;
        }

    </style>
    <script type="text/javascript">

        function PerformSelectFolder() {

            var folder = ShellWrappers_BrowseForFolder("Salva documento");

            if (folder != "") {
                var fso = FsoWrapper_CreateFsoObject();
                if (fso.FolderExists(folder)) {
                    frmSaveDialog.txtFolderPath.value = folder;
                    frmSaveDialog.txtFirstInvalidControlID.value = "txtFileName";
                }
            }
        }

        function GetPath() {
            if (GetSelectedFilePath() == true) {
                var selectedPath = frmSaveDialog.txtSelectedPath.value;
                CloseWindow(selectedPath);
            }
            else {
                alert("Inserire un percorso valido.");
            }

        }

        function GetSelectedFilePath() {

            var selectedPath = frmSaveDialog.txtFolderPath.value;
            var retValue = false;

            if (selectedPath != "") {
                var fso = FsoWrapper_CreateFsoObject();

                var folder;
                try {
                    folder = fso.GetFolder(selectedPath);
                }
                catch (ex) {
                    folder = null;
                }

                if (folder != null) {
                    selectedPath = folder.Path;

                    if (selectedPath.charAt(selectedPath.length - 1) != "\\")
                        selectedPath = selectedPath + "\\";

                    selectedPath = selectedPath + GetFileName();
                    frmSaveDialog.txtSelectedPath.value = selectedPath;
                    retValue = true;
                }

            }

            return retValue;

        }

        function GetFileName() {

            var fileName = "";
            var ext = "pdf"

            fileName = frmSaveDialog.txtFileName.value;
            if (!(fileName.slice(-4) == '.pdf')) {
                fileName = fileName + "." + ext;
            }

            return fileName;
        }

        function CloseWindow(retValue) {
            window.returnValue = retValue;
            window.close();
        }

    </script>
</head>
<body style="background-color: #f2f2f2">
    <form id="frmSaveDialog" runat="server">
    <uc2:ShellWrapper ID="shellWrapper" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    <input type="hidden" id="txtFirstInvalidControlID" runat="server" />
    <input type="hidden" id="txtSelectedPath" runat="server" />
    <div>
    <table id="Table1" cellspacing="0" cellpadding="2" width="370" align="center" border="0" runat="server">
        <tr>
            <td align="left" colspan="2">
                <asp:Label ID="lblFileName" runat="server" CssClass="titolo_scheda">Nome file: </asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" width="240">
                <asp:TextBox ID="txtFileName" runat="server" style="height:15px;" CssClass="testo_grigio" Width="300px"></asp:TextBox>
                <asp:Label ID="lblFileExt" runat="server" CssClass="titolo_scheda">.pdf</asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="2">
                <asp:Label ID="lblPathName" runat="server" CssClass="titolo_scheda">Cartella di destinazione: </asp:Label>
            </td>
        </tr>
        <tr>
            <td align="left" valign="middle" colspan="2">
                <asp:TextBox ID="txtFolderPath" runat="server" style="height:15px;" CssClass="testo_grigio" Width="300px"></asp:TextBox>&nbsp;
                <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." CssClass="pulsante">
                </asp:Button>
            </td>        
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:Button ID="btnOk" runat="server" CssClass="cbtn" Text="Ok" />&nbsp;
                <asp:Button ID="btnAnnulla" runat="server" CssClass="cbtn" Text="Annulla" />
            </td>
        </tr>
    </table>
    
    </div>
    </form>
</body>
</html>