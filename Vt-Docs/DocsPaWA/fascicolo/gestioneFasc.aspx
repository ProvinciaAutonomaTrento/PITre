<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="gestioneFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicolo.gestioneFasc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="AclFascicolo" Src="AclFascicolo.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio 7.0">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script>
		    function Body_OnMouseOver() {
		        try {

		            if (top.superiore.document != null)
		                if (top.superiore.document.Script != null)
		                    if (top.superiore.document.Script["closeIt"] != null)
		                        top.superiore.document.Script.closeIt();
		        }
		        catch (e) {
		            //alert(e.message);
		        }

		    }
		    function Body_OnLoad() {
		        try {

		            if (top.superiore.document != null)
		                if (top.superiore.document.Script != null)
		                    if (top.superiore.document.Script["CheckTestataTastoSel"] != null)
		                        top.superiore.document.Script.CheckTestataTastoSel();
		        }
		        catch (e) {
		            //alert(e.message);
		        }

		    }
		    function ApriFinestraRicercaDocPerClassifica(parameters) {
		        var newLeft = (screen.availWidth - 602);
		        var newTop = (screen.availHeight - 689);
		        var myUrl = "../popup/RicercaDocumentiPerClassifica.aspx?" + parameters;
		        rtnValue = window.showModalDialog(myUrl, '', 'dialogWidth:595px;dialogHeight:643px;status:no;dialogLeft:' + newLeft + '; dialogTop:' + newTop + ';center:no;resizable:yes;scroll:no;help:no;');
		        window.document.gestioneFasc.submit();
		    }


		</script>
	</HEAD>
	<body bottommargin="0" leftmargin="0" rightmargin="0" topmargin="0" onmouseover="Body_OnMouseOver()"
		onload="Body_OnLoad()" MS_POSITIONING="GridLayout">
		<!--<body MS_POSITIONING="GridLayout">-->
		<form id="gestioneFasc" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Fascicoli" />
			<uc1:AclFascicolo ID="aclFascicolo" runat="server"></uc1:AclFascicolo>
            <table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				
                <tr valign="top" height="100%">
					<td width="365" valign="top">
						<cf1:IFrameWebControl id="iFrame_sx" runat="server" Width="415px" Scrolling="no" Frameborder="0" iHeight="100%"
							iWidth="415" Marginheight="0" Marginwidth="0" BorderWidth="1px" BackColor="Red"></cf1:IFrameWebControl>

					</td>
					<td width="1"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<!--TD vAlign="top" width="1" background="../images/tratteggio_bn_v.gif" bgColor="#9e9e9e" rowSpan="2" height="100%"><IMG height="6" src="../images/tratteggio_bn_v.gif" width="1" border="0"></TD-->
					<td width="1"><img border="0" src="../images/spaziatore.gif" width="1"></td>
					<td >
						<cf1:IFrameWebControl id="iFrame_dx" runat="server" Scrolling="auto" Frameborder="0" iHeight="100%" iWidth="100%"
							Marginheight="0" Marginwidth="10"></cf1:IFrameWebControl>

					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>




