<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NumeroCopie.aspx.cs" Inherits="ConservazioneWA.PopUp.NumeroCopie" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
   
   <base target="_self" />

   <style type="text/css">
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
    <script language="javascript" type="text/javascript">
        function controllo() {
            var pattern = /^[a-zA-Z0-9éèàìòù\s\?\,\.\+\-\*\;\:\!\%\&\(\)\[\]\{\}\\\/''""]+$/;
            if (window.form1.TextArea1.value == "") {
                alert('Inserire la ragione oppure annullare l\'operazione');
                return false;
            }
            if (window.form1.TextArea1.value != "") {
                if (!pattern.test(window.form1.TextArea1.value)) {
                    alert('Formato testo della ragione non valido');
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
            <asp:Label runat="server" ID="lb_intestazione" Text="Passaggio in Lavorazione"></asp:Label>
        </div>
            
        <asp:Panel ID="pnlPlocyNonValidata" runat="server">
            </div>
                <div id="Div1">
                <br />
                <asp:Label runat="server" ID="lblPolNonValidata" Text="Policy non validata"></asp:Label>
            </div>

            <div align="center" style=" color:Red; font-size: 12px;">
                <p>Attenzione, la policy non è stata validata. </p>
                <p>Se si vuole proseguire con l'immissione in lavorazione, indicarne la ragione:</p>
            </div>
        
            <div align="center">
                <textarea id="txtMotivoAccettazione" runat="server"></textarea>
            </div>
        </asp:Panel>

        <div align="center" style="margin-top: 10px;">
            <asp:Label ID="lblNumeroSupporti" runat="server" Text="Numero supporti esterni (opzionale):" Width="250px" CssClass="testo_grigio2"></asp:Label>
            <asp:TextBox ID="txtNumeroSupporti" runat="server" Width="36px" CssClass="testo_grigio" Text="0"></asp:TextBox>
        </div>
        <div align="center">
            <asp:Button ID="btn_inserisci" runat="server"  Text="Conferma" CssClass="cbtn" onclick="btn_inserisci_Click" />
            <asp:Button ID="btn_chiudi" runat="server" Text="Annulla" CssClass="cbtn" onclick="btn_chiudi_Click" />
        </div>
    </form>
</body>
</html>
