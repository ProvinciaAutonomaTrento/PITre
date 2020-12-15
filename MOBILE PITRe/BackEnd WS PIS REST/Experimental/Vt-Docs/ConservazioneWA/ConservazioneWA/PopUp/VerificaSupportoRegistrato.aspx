<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificaSupportoRegistrato.aspx.cs"
    Inherits="ConservazioneWA.PopUp.VerificaSupportoRegistrato" %>

<%@ Register Src="~/UserControl/Calendar.ascx" TagName="Calendario" TagPrefix="uc1" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="~/SmartClient/VerificaSupporto.ascx" TagName="VerificaSupportoCtrl"
    TagPrefix="uc4" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Inserisci note</title>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <style type="text/css">
        #txt_note
        {
            height: 101px;
            width: 272px;
        }
        .style1
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 76px;
        }
        #TextArea1
        {
            height: 80px;
            width: 200px;
        }
        
        .cbtn
        {
            background-image: url('../Img/bg_button.jpg');
        }
        
        .cbtnHover
        {
            background-image: url('../Img/bg_button_hover.jpg');
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
        
         #testoNote
        {
             background-image: url('../Img/bg_tab_header.jpg');
             background-repeat: repeat-x;
        }
    </style>
    </style>
    <script language="javascript" type="text/javascript">

        function performSelectFolder() {
            var folder = ShellWrappers_BrowseForFolder("Selezionare la cartella relativa all'istanza di conservazione");

            if (folder != null && folder != "") {
                var txtPathSupporto = document.getElementById("txtPathSupporto");

                txtPathSupporto.value = folder;
            }
        }

        function execute() {
            var txtPathSupporto = document.getElementById("txtPathSupporto");
            var idIstanza = "<%=this.IdIstanza%>";
            var idDocumento = "<%=this.IdDocumento%>";
            var percentuale = 100;

            if (idDocumento == "") {
                var txtPercentuale = document.getElementById("txtPercentuale");
                percentuale = txtPercentuale.value;
            }

            verificaSupporto_execute(txtPathSupporto.value, percentuale, idIstanza, idDocumento);
        }

    </script>
</head>
<body style="background-color: #ffffff">
    <form id="form1" runat="server">
    <uc2:ShellWrapper ID="shellWrapper" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    <div align="center">
        <div id="testoNote">
            <asp:Label runat="server" ID="lb_intestazione" Text="Verifica supporto registrato"></asp:Label>
        </div>
        <div style="width: 600px; text-align: center">
            <table width="100%">
                <tr id="trPathSupporto" runat="server">
                    <td style="width: 50%; text-align: right">
                        <asp:Label runat="server" ID="lblPathSupporto" Text="Percorso supporto: *" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <asp:TextBox runat="server" ID="txtPathSupporto" CssClass="testo_grigio3" Width="70%"></asp:TextBox>
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." OnClientClick="performSelectFolder()"  CssClass="testo_grigio3"/>
                    </td>
                </tr>
                <tr id="trPercentuale" runat="server">
                    <td style="width: 40%; text-align: right">
                        <asp:Label runat="server" ID="lblPercentuale" Text="Percentuale %: *" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <asp:TextBox runat="server" ID="txtPercentuale" CssClass="testo_grigio3" Text="100"></asp:TextBox>
                    </td>
                </tr>
                <tr id="trDataProssimaVerifica" runat="server">
                    <td style="width: 40%; text-align: right">
                        <asp:Label runat="server" ID="lblDataProssimaVerifica" Text="Prossima verifica il: *" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <uc1:Calendario runat="server" ID="txtDataProssimaVerifica" PAGINA_CHIAMANTE="VerificaSupporto" />
                    </td>
                </tr>
                <tr id="trNoteVerifica" runat="server">
                    <td style="width: 40%; text-align: right">
                        <asp:Label runat="server" ID="lblNoteDiVerifica" Text="Note di verifica:" CssClass="testo_grigio3"></asp:Label>
                    </td>
                    <td style="width: 60%; text-align: left">
                        <asp:TextBox runat="server" ID="txtNoteDiVerifica" CssClass="testo_grigio3" Width="70%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center">
                        <asp:Button ID="btnVerifica" runat="server" Text="Avvia" CssClass="cbtn" OnClientClick="execute();"
                            OnClick="btnVerifica_Click" />
                        <asp:Button ID="btnChiudi" runat="server" Text="Chiudi" CssClass="cbtn" OnClientClick="window.returnValue=false; window.close();" />
                    </td>
                </tr>
            </table>
        </div>
        <uc4:VerificaSupportoCtrl ID="verificaSupportoSmartClient" runat="server" />
    </div>
    </form>
</body>
</html>
