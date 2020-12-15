<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AvvisoRuoloConLF.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Organigramma.AvvisoRuoloConLF" %>

<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <base target="_self">
    <script type="text/javascript">
        function ApriProcessiFirma(idRuoloTitolare, idUtenteTitolare) {
            var myUrl = "ProcessiFirma.aspx?from=RU&idGruppo=" + idRuoloTitolare + "&idUtente=" + idUtenteTitolare;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:1200px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }
        function ApriProcessiFirmaRuolo(idRuoloTitolare) {
            var myUrl = "ProcessiFirma.aspx?from=R&idGruppo=" + idRuoloTitolare;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:1200px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }
        function ApriProcessiFirmaRuoliUtente(idUtenteTitolare) {
            var myUrl = "ProcessiFirma.aspx?from=U&idUtente=" + idUtenteTitolare;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:1200px;dialogHeight:600px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }
    </script>
</head>
<body bottommargin="2" leftmargin="2" topmargin="2" rightmargin="2" ms_positioning="GridLayout">
    <form id="frmAvviso" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Avviso utente convolto in processi/istanze di firma" />
        <div style="padding:10px;">
            <div style="width: 10%; float: left">
                <asp:Image ID="img_alert" ImageUrl="../Images/alert.gif" runat="server"></asp:Image>
            </div>
            <div style="width: 90%">
                <asp:Label ID="lbl_utente" runat="server"></asp:Label>
            </div>
            <div style="text-align:center;padding-top:30px">
                <asp:Button ID="btn_dettaglio" CssClass="testo_btn" runat="server" Text="Apri dettaglio" ToolTip="Visualizza processi di firma o istanze di processo in cui l'utente è coinvolto" ></asp:Button>&nbsp;
                <asp:Button ID="btn_si_senza_interruzione" CssClass="testo_btn" runat="server" Text="Si, senza interrompere" Visible="false"></asp:Button>&nbsp;
                <asp:Button ID="btn_add_user" CssClass="testo_btn" runat="server" Text="Si, sostituisci utente" Visible="false"></asp:Button>&nbsp;
                <asp:Button ID="btn_si" CssClass="testo_btn" runat="server" Text="Si, con interruzione"></asp:Button>&nbsp;
                <asp:Button ID="btn_no" CssClass="testo_btn" runat="server" Text="Annulla"></asp:Button>
            </div>
        </div>
    </form>
</body>
</html>
