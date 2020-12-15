<%@ Page Language="c#" CodeBehind="index.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.index" %>

<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="cc3" TagName="TimerDisservizio" Src="~/UserControls/TimerDisservizi/TimerDisservizio.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <meta name="GENERATOR" content="Microsoft Visual Studio 7.0">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
    <!--link href="CSS/docspa.css" type="text/css" rel="stylesheet"-->

    <!--script language="javascript" src="librerie/docspa_func.js"></script-->

    <script language="javascript" src="librerie/progressMsg.js"></script>

    <!--script language="javascript" src="/waiting/waiting.js"></script-->

    <!--script language="javascript" src="/waiting/waitingManager.js"></script-->
    <script type="text/javascript">

        var myclose = false;

        function ConfirmClose() {
            //alert('X:' + event.clientX);
            //alert('Y:' + event.clientY);
            if (event.clientY < 0 && event.clientX >= document.body.clientWidth - 35) {
                setTimeout('myclose=false', 100);
                myclose = true;
            }
        }

        function HandleClose() {
            if (myclose == true) {
                //alert('X:' + event.clientX);
                //alert('Y:' + event.clientY);
                //alert('Entrato');
                PageMethods.AbandonSession();
            }
        }
    </script>
    <script language="javascript">	
		function body_onload()
        {
	        //GENERAZIONE DEL 'WAITING MANAGER' 
	       // var baseUrl='<%=this.Request.Url.AbsoluteUri.Replace(this.Request.Url.AbsolutePath.Replace(this.Request.ApplicationPath,"")+this.Request.Url.Query,"")%>';
	        //waitingServerJS_createWaitingManager(baseUrl);
        }	
			function body_onunload()
			{
				document.location="Exit.aspx";				
			} 

			function resizeMe()
			{
				
				var maxWidth=screen.width;
				var maxHeight=screen.availHeight;
				window.resizeTo(maxWidth,maxHeight);
				
				var newLeft=0;
				var newTop=0;
				window.moveTo(newLeft,newTop);		
			}
    </script>

</head>
<body ms_positioning="GridLayout" rightmargin="0" topmargin="0"
    leftmargin="0" bottommargin="0" scroll="no"  onbeforeunload="ConfirmClose();" onunload="HandleClose();">
    <form id="index" method="post" runat="server">
    <div style="display: none">
    <asp:scriptmanager id="ScriptManager1" runat="server" enablepagemethods="true" AsyncPostBackTimeout="360000" />
    <cc3:TimerDisservizio ID="timerDiss" runat="server" />
    </div>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione Documentale" />
    <table border="0" cellpadding="0" cellspacing="0" width="100%" height="100%">
        <tr>
            <td height="80">
                <cf1:IFrameWebControl ID="superiore" runat="server" Marginwidth="0" Marginheight="0"
                    iWidth="100%" iHeight="80" Frameborder="0" Scrolling="no" noResize></cf1:IFrameWebControl>
            </td>
        </tr>
        <tr>
            <td height="90%">
                <cf1:IFrameWebControl ID="principale" runat="server" Marginwidth="0" Marginheight="0"
                    iWidth="100%" iHeight="100%" Frameborder="0" Scrolling="no" noResize></cf1:IFrameWebControl>
            </td>
        </tr>
        <tr>
            <td height="17">
                <cf1:IFrameWebControl ID="inferiore" runat="server" Marginwidth="0" Marginheight="0"
                    iWidth="100%" iHeight="17" Frameborder="0" Scrolling="no" noResize></cf1:IFrameWebControl>
            </td>
        </tr>
        <tr>
            <td height="0">
                <cf1:IFrameWebControl ID="progress" runat="server" Marginwidth="0" Marginheight="0"
                    iWidth="100%" iHeight="0" Frameborder="0" Scrolling="no" NavigateTo="progress.aspx"
                    noResize></cf1:IFrameWebControl>
            </td>
        </tr>
       
    </table>
        
    </form>
</body>
 
</html>
