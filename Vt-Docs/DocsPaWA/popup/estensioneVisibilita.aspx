<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="estensioneVisibilita.aspx.cs" Inherits="DocsPAWA.popup.estensioneVisibilita" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<base target="_self">
	
	<script type="text/javascript" language="javascript" id="NO">
        function NO()
        {
            window.returnValue = 'NO'; 
            window.close();
            return false;
        }
    </script>
    <script type="text/javascript" language="javascript" id="SI">
        function SI()
        {
            window.returnValue = 'SI'; 
            window.close();
            return true;
        }
    </script>
    
</head>
<body>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Avviso Estensione Visibilità" />
    <form id="AvvisoDocPrivato" runat="server">
	<TABLE class="contenitore" id="tbl_avviso" height="140" width="300" align="center" border="0">
		<TR vAlign="middle" height="20">
			<td class="menu_1_rosso" align="center">ATTENZIONE</td>
		</TR>
		<TR vAlign="top" height="90">
			<td class="testo_grigio_scuro" align="center">
			    <br />
                si sta trasmettendo un documento o fascicolo privato con una ragione che prevede 
	            l&#39;estensione gerarchica della visibilità.
	            <br />Vuoi bloccare l&#39;estensione gerarchica della visibilità? <br />
	            <br />
			</td>
		</TR>
		<TR height="30">
			<TD align="center" height="30"><asp:button id="btn_si" runat="server" Text="SI" 
                    CssClass="PULSANTE" OnClientClick="SI()" Width="30px"></asp:button>&nbsp;
				<asp:button id="btn_no" runat="server" Text="NO" CssClass="PULSANTE" 
                    OnClientClick="NO()" Width="30px"></asp:button></TD>
		</TR>
	</TABLE>
    </form>
</body>
</html>
