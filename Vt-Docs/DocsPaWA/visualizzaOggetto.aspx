<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="visualizzaOggetto.aspx.cs"
    Inherits="DocsPAWA.visualizzaOggetto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DOCSPA > visualizzaOggetto</title>
    <link href="CSS/docspa.css" type="text/css" rel="stylesheet" />
</head>
<body>

    <script type="text/javascript">

        function redirectToPage(url, resize) {
            var popup = window.open(
                            url, 
                            'principale', 
                            //'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');
                            'location=0,resizable=yes');

            if (resize == 'true') {
                popup.moveTo(0, 0);
                popup.resizeTo(screen.availWidth, screen.availHeight);
            }
            
            if (popup != self) {
                window.opener = null;
                self.close();
            }

        }

    </script>

    <form id="visualizzaOggetto" method="post" runat="server">
    <div style="z-index: 101; left: 71px; position: absolute; top: 111px">
        <asp:BulletedList ID="blMessage" runat="server">
        </asp:BulletedList>
    </div>
    </form>
</body>
</html>
