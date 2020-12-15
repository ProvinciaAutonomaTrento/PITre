<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="avvisoConfermaConnessioneMail.aspx.cs" Inherits="DocsPAWA.popup.avvisoConfermaConnessioneMail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<base target="_self">
	 <script type="text/javascript" language="javascript" id="SI">
        function SI()
        {
            window.close();
            return true;
        }
    </script>
    
    <style type="text/css">
        #form1
        {
            height: 142px;
            width: 298px;
            margin-bottom: 2px;
        }
        .style1
        {
            height: 46px;
            width: 356px;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 64px;
            width: 356px;
        }
        .style3
        {
            FONT-SIZE: 10px;
            font-weight: bold;
            TEXT-TRANSFORM: uppercase;
            COLOR: #810d06;
            TEXT-INDENT: 0px;
            FONT-FAMILY: Verdana;
            width: 356px;
        }
    </style>
    
</head>
<body>
    <form id="form1" runat="server">
    <TABLE class="contenitore" id="tbl_avviso" height="140" width="300" align="center" border="0">
		<TR vAlign="middle" height="20">
			<td class="style3" align="center">ATTENZIONE<TR vAlign="top">
			<td class="style2" align="justify">
                <asp:Label ID="conferma" runat="server" Text="conferma"></asp:Label>
			</td>
		</TR>
		<TR>
			<TD align="center" class="style1">
                <asp:button id="btn_si" runat="server" Text="OK" 
                    CssClass="PULSANTE" OnClientClick="window.close();" Width="57px" 
                    onclick="btn_si_Click"></asp:button>&nbsp;
				</TD>
		</TR>
	</TABLE>
    </form>
</body></html>
