<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistraSupporto.aspx.cs"
    Inherits="ConservazioneWA.PopUp.RegistraSupporto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Registra il supporto</title>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <base target="_self" />
    <style type="text/css">
        #txt_note
        {
            height: 101px;
            width: 272px;
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
</head>
<body>
    <form id="form1" runat="server" style="background-color: #ffffff;">
    <div align="center">
        <div id="testoNote">
            <asp:Label runat="server" ID="lb_intestazione" Text="Registrazione supporto esterno"></asp:Label>
        </div>
        <table width="600">
            <tr>
                <td>
                    <asp:Label runat="server" ID="lblCollocazione" Text="Collocazione: *" CssClass="testo_grigio3" ></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtCollocazione" Width="80%" CssClass="testo_grigio2"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label2" Text="Note:" CssClass="testo_grigio3"></asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtNote" Width="80%" CssClass="testo_grigio2"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div align="center">
            <asp:Button ID="btnRegistra" runat="server" Text="Registra" CssClass="cbtn" OnClick="btnRegistra_Click" />
            <asp:Button ID="btnChiudi" runat="server" Text="Chiudi" CssClass="cbtn" OnClientClick="window.returnValue=false; window.close();" />
        </div>
    </div>
    </form>
</body>
</html>
