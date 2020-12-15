<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerificaLeggibilita.aspx.cs" Inherits="ConservazioneWA.PopUp.VerificaLeggibilita" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
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
        function showChiusuraIstanza(idConservazione) {
            var returnvalue = window.showModalDialog("ChiusuraIstanza.aspx?idCons=" + idConservazione, "", "dialogWidth:300px;dialogHeight:150px;status:no;resizable:no;scroll:no;center:yes;help:no");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div align="center">
            <div id="testoNote">
                <asp:Label runat="server" ID="lb_intestazione" Text="Verifica Leggibilità"></asp:Label>
            </div>
        </div>
        <div align="center" style=" margin-top:10px;">
        <div style="height:580px; width:95%;">
            <iframe id="iframeDoc" style="width:100%; height:100%;" scrolling="auto" frameborder="0" runat="server" visible="true"></iframe>      
        </div>   </div>
        <div align="left" style="float:left; margin: 10px 0 10px 100px;">
            <asp:HiddenField ID="hd_indice_docs" runat="server" />
            <asp:HiddenField ID="hd_num_docs" runat="server" />
            <asp:HiddenField ID="hd_idCons" runat="server" />
            <asp:HiddenField ID="hd_files" runat="server" />
            <asp:HiddenField ID="hd_docnumber" runat="server" />
            <asp:HiddenField ID="risultatoLeggibilita" runat="server" />
            <asp:HiddenField ID="hd_percent" runat="server"  />
            <asp:HiddenField ID="hd_localstore" runat="server" />
            </asp:HiddenField>
         <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                <asp:ListItem>Documento leggibile</asp:ListItem>
                <asp:ListItem>Documento non leggibile</asp:ListItem>
                
            </asp:RadioButtonList>
        </div>
        <div align="center" style="float:right; margin:10px 100px 10px 0;">
            

           <asp:Button ID="btn_avanti" runat="server" Text="Avanti" CssClass="cbtn" 
                onclick="btn_avanti_Click" />
           <asp:Button ID="btn_chiudi" runat="server" Text="Chiudi" CssClass="cbtn" 
                Visible="false" onclick="btn_chiudi_Click" />
        </div>

    </div>
    </form>
</body>
</html>
