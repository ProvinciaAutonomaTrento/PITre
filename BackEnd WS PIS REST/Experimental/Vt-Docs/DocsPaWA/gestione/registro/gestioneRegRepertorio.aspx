<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="gestioneRegRepertorio.aspx.cs" Inherits="DocsPAWA.gestione.registro.gestioneRegRepertorio" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script>
		    function Body_OnMouseOver() {
		        try {

		            if (top.superiore.document != null)
		                if (top.superiore.document.Script != null)
		                    if (top.superiore.document.Script["closeIt"] != null)
		                        top.superiore.document.Script.closeIt();
		        }
		        catch (e) {
		            alert(e.message);
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
		            alert(e.message);
		        }
		    }
		</script>
	</HEAD>
	<body MS_POSITIONING="GridLayout" onmouseover="Body_OnMouseOver()" onload="Body_OnLoad()">
		<form id="gestioneReg" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione Repertorio Registro" />
			<table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
				<tr valign="top" height="50%">
					<td valign="top" style="width:45%;">
						<cf1:IFrameWebControl id="iFrame_elenco" runat="server" Marginwidth="0" 
                            Marginheight="2" iWidth="415"
							iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="regRepertorioElenco.aspx" 
                            Width="415px" >
                        </cf1:IFrameWebControl>
					</td>
					<td width="2%"><img border="0" src="../../images/spaziatore.gif" width="1"></td>
					
					<td valign="top" style="width:50%; padding:0px;">
						<cf1:IFrameWebControl id="iFrame_dettaglio" runat="server" Marginwidth="0" 
                            Marginheight="2" iWidth="100%"
							iHeight="100%" Frameborder="0" Scrolling="no" NavigateTo="regRepertorioDettaglio.aspx" 
                            Width="100%" >
                        </cf1:IFrameWebControl>
					</td>			
				</tr>
			</table>
		</form>
	</body>
</HTML>
