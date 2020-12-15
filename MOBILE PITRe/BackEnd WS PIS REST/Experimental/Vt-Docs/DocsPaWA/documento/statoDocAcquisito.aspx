    <%@ Page Language="c#" CodeBehind="statoDocAcquisito.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.documento.statoDocAcquisito" %>

<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    
    <script type="text/javascript">
        function visualizzaInterno() 
        {
            top.principale.iFrame_dx.document.location="tabdoc.aspx?opt=visualizzaInterno";
        }
    </script>
    
</head>
<body ms_positioning="GridLayout">
    <form id="statoDocAcquisito" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Stato Documento Acquisito" />
    <table border="0" width="100%" height="100%">
        <tr height="10%">
            <td align="left" valign="top">
                <br>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="center" align="middle" class="testo_msg_grigio_grande">
                <!--font face="Comic Sans Ms" size="8">Per visualizzare
							<br>
							il documento<br>
							clicca su<br>
							'Visualizza'</font -->
                Per visualizzare il documento<br />
                clicca su '<a href="<%= this.Link %>" id="link" onclick="visualizzaInterno();return false;" title="Clicca per visualizzare il documento. Trascina in un documento o mail per consentire l'accesso al documento dall'esterno">Visualizza</a>'.
            </td>
        </tr>
        <tr height="10%">
            <td align="left" valign="top">
                <br />
                &nbsp;
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
