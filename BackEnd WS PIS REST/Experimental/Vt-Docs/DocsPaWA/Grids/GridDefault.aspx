<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GridDefault.aspx.cs" Inherits="DocsPAWA.Grids.GridDefault" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<base target="_self" />
    <title>Ritorna alla griglia iniziale</title>
    <link type="text/css" href="../CSS/docspa_30.css" rel="Stylesheet" />
    <link href="../CSS/StyleSheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
      #container_default{
         background-color: #fafafa;
         border: 1px solid #cccccc;
         padding:10px;
         margin:10px;
      }

      #button_center{
        text-align:center;
        margin-top:20px;
      }

      #container_default h2{
        text-align:center;
        font-size:13px;
      }
    </style>
    <script type="text/javascript" language="javascript">
        function close_and_save() {
            var ret = document.getElementById("hid_tab_est").value;
            window.returnValue = ret;
            window.close();
        }
    </script>
</head>
<body style="background-color:#eaeaea;">
    <form id="frmBackDefault" runat="server">
    <asp:HiddenField ID="hid_tab_est" runat="server"/>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Torna alla griglia iniziale" />
    <div id="container_default">
        <h2 class="titolo_scheda">Attenzione!</h2>
        <p class="testo_grigio">Cliccando su continua verrà re-impostata la griglia salvata come predefinita. Si desidera continuare?</p>
        <div id="button_center">
            <asp:Button CssClass="pulsante_mini_3" ID="btnOk" runat="server" Text="Continua"
             OnClick="btnDefaultGridSettings_Click" OnClientClick="close_and_save();"/>
             &nbsp;
            <asp:Button CssClass="pulsante_mini_3" ID="btnClose" runat="server" Text="Chiudi"
             OnClientClick="window.close();"/>
       </div>
    </div>
    </form>
</body>
</html>
