<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChiusuraIstanza.aspx.cs" Inherits="ConservazioneWA.PopUp.ChiusuraIstanza" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Chiusura Istanza</title>
    
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
       
    </script>
</head>
<body style="background-color: #ffffff">

    <form id="form1" runat="server">
        <div align="center">
        <div id="testoNote">
            <asp:Label runat="server" ID="lb_intestazione" Text="Chiusura Istanza"></asp:Label>
        </div>
        <br />
            <asp:Label runat="server" id="lbl_messaggio" Text="Verifica Leggibilità effettuata con successo.<br />Caricamento dei file sullo storage remoto."  CssClass="testo_grigio2"></asp:Label>
            <br /><br />
            <asp:HiddenField runat="server" id="hd_stato">
            </asp:HiddenField>
            <asp:Image runat="server" id="img_loading" ImageUrl="../Img/loading.gif" AlternateText="Caricamento dell'istanza sullo storage remoto" >
            </asp:Image>
            <asp:ScriptManager ID="ScriptManager1" runat="server" />
            <asp:Timer runat="server" id="timer1" Interval="1000" Enabled="False" 
                ontick="timer1_Tick" >
            </asp:Timer>
            </asp:ScriptManager>
            <asp:Button runat="server" id="btn_chiudi" Text="OK" CssClass="cbtn" 
                onclick="btn_chiudi_Click" Visible="false" />
            
        </div>
    </form>
</body>
</html>