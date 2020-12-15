<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopupRifiuto.aspx.cs" Inherits="ConservazioneWA.Esibizione.PopupRifiuto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        <script type="text/javascript">
            function controllo() {
                var pattern = /^[a-zA-Z0-9éèàìòù\s\?\,\.\+\-\*\;\:\!\%\&\(\)\[\]\{\}\\\/''""]+$/;
                if (window.form1.TextArea1.value == "") {
                    alert('Inserire le Note oppure annullare l\'operazione');
                    return false;
                }
                if (window.form1.TextArea1.value != "") {
                    if (!pattern.test(window.form1.TextArea1.value)) {
                        alert('Formato testo delle note non valido');
                        return false;
                    }
                }
                return true;
            }
    </script>


</head>
<body style="background-color: #ffffff">
    <form id="form1" runat="server">
    <div align="center">
        <div id="testoNote">
            <asp:Label runat="server" ID="lb_intestazione" Text="Inserici note rifiuto"></asp:Label>
        </div>
    </div>
    <div align="center" style="margin-top: 10px;">
        <textarea id="TextArea1" runat="server" class="txt_area"></textarea>
    </div>
    <div align="center">
        <asp:Button ID="btn_inserisci" runat="server" Text="Inserisci" CssClass="cbtn" OnClick="btn_inserisci_Click"
            OnClientClick="return controllo();" />
        <asp:Button ID="btn_chiudi" runat="server" Text="Annulla" CssClass="cbtn" OnClick="btn_chiudi_Click" />
    </div>
    </form>
</body>
</html>
