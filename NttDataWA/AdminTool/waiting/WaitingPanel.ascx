<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WaitingPanel.ascx.cs" Inherits="SAAdminTool.waiting.WaitingPanel" %>
<script type="text/javascript">
    function ShowWaitPanel(waitMessage)
	{
		var w_width = 300;
		var w_height = 100;
		var t = ((screen.height - window.screenTop) / 2) - (w_height / 2);
		var l = ((screen.width - window.screenLeft)/ 2) - (w_width / 2);

        if (waitMessage==null)
            waitMessage="Attendere prego...";
            
		var waitingLabelId = "<% =WaitLabelMessage %>";
		document.getElementById(waitingLabelId).innerText = waitMessage;
		
		var waitingPanelId = "<% =WaitingPanelId %>";
		if (document.getElementById(waitingPanelId))
		{	
		    document.getElementById(waitingPanelId).style.top = t;
		    document.getElementById(waitingPanelId).style.left = l;
		    document.getElementById(waitingPanelId).style.width = w_width;
		    document.getElementById(waitingPanelId).style.height = w_height;				
		    document.getElementById(waitingPanelId).style.display = '';
	    }
    }

    function CloseWait() {
        var waitingDivId = "<% =DivWaitMessageID %>";
        document.getElementById(waitingDivId).style.display = 'none';
    }
</script>
 <div runat="server" id="waitingPanel" runat="server" style="DISPLAY: none; BORDER-RIGHT: #000000 2px solid; BORDER-TOP: #000000 2px solid; BORDER-LEFT: #000000 2px solid; BORDER-BOTTOM: #000000 2px solid; POSITION: absolute; BACKGROUND-COLOR: #d9d9d9">
    <table cellSpacing="0" cellPadding="0" width="350px" border="0">
        <tr>
            <td vAlign="middle" style="FONT-WEIGHT: bold; FONT-SIZE: 12pt; FONT-FAMILY: Verdana" 
                align="center" width="350px" height="90px">
                <asp:Label ID="lblWaitMessage" runat="server">
                    Attendere prego...
                </asp:Label>
            </td>
        </tr>
    </table>
</div>