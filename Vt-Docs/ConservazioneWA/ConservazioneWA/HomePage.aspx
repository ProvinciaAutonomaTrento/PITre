<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomePage.aspx.cs" Inherits="ConservazioneWA.HomePage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Conservazione</title>
   <link href="CSS/docspa_30.css" type="text/css" rel="Stylesheet" />
   <link href="CSS/rubrica.css" type="text/css" rel="Stylesheet" />

    <script language="javascript" type="text/javascript">
    function setIFrameUrl()
    {
        //Iframes.location='Ricerca.aspx';
        document.getElementById('img_conservazione').src = "Img/Conservazione_NonAttivo.gif";
        document.getElementById('img_ricerca').src = "Img/Supporti.gif";
        document.all.Iframes.src="Ricerca.aspx";
        return false;
    }
    function setIFrameUrl2()
    {
        document.getElementById('img_conservazione').src = "Img/Conservazione.gif";
        document.getElementById('img_ricerca').src = "Img/Supporti_NonAttivo.gif";
        document.all.Iframes.src="RicercaSupporti.aspx";
        return false; 
      // Iframes.location='RicercaSupporti.aspx';  
    }
    function setIframePage()
    {
        document.getElementById('img_conservazione').src = "Img/Conservazione_NonAttivo.gif";
        document.getElementById('img_ricerca').src = "Img/Supporti.gif";
        document.all.Iframes.src="Ricerca.aspx";
        return false;
    }
    </script>
    
</head>
<body onload="return setIframePage();">
    <form id="form1" runat="server">
    <input type="hidden" id="hd_srcPagina" name="hd_srcPagina" runat="server" />
    <table cellspacing="0" cellpadding="0" width="100%" style="height:100%">
        <tr>
            <td>
                <table cellspacing="0" cellpadding="0">
 				    <TR>
					    <TD id="mnubar0" width="133" height="38"><asp:imagebutton id="img_logo" runat="server" ImageAlign="AbsMiddle" width="133" height="38" AlternateText="Torna alla pagina iniziale" BorderWidth="0"></asp:imagebutton></TD>
					    <TD width="100%" height="38">
						    <table cellspacing="0" cellpadding="0" width="100%" border="0">
							    <tr>
								    <td id="backgroundLogo" runat="server" width="455" height="38" style="background-color:#b1d2c3"><asp:image id="logoEnte" height="38" SkinId="logoEnte" width="455" border="0" runat="server" /></td>
								    <td runat="server" id="backgroundLogoEnte" vAlign="top" align="right" width="100%" 
									    height="38"><asp:Label runat="server" ID="lbl_ruolo" Text="" CssClass="testo_testata"></asp:Label></td>
							    </tr>
						    </table>
					    </TD>
				    </TR>
                </table>
            </td>
        </tr>
        <tr><td height="5">&nbsp;</td></tr>
        <tr>
            <td valign="bottom">
                <table>
                    <tr>
                        <td valign="bottom">
                            <img id="img_conservazione" onclick="return setIFrameUrl();" src="Img/Conservazione.gif" width="140px" height="16px" />
                            
                        </td>
                        <td valign="bottom">
                            <img id="img_ricerca" onclick="return setIFrameUrl2();" src="Img/Supporti_NonAttivo.gif" width="140px" height="16px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr valign="top" align="center">
            <td>            
            <iframe id="Iframes" runat="server" frameborder="0"   scrolling="no"
                    marginwidth="0" marginheight="0" width="100%" height="590px"></iframe>

                </td>
        </tr>
    </table>

    </form>
</body>
</html>
