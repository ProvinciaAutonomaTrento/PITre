<%@ Register Assembly="DocsPaWebCtrlLibrary" Namespace="DocsPaWebCtrlLibrary" TagPrefix="cc2"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="ConservazioneWA.UserControl.Calendar" %>


<script language="javascript" type="text/javascript">
function ShowCalendar(clientId,fromUrl)
{
	var selectedDate = window.showModalDialog(fromUrl,
								'',
								'dialogWidth:250px;dialogHeight:195px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
    var textDataCtl = document.getElementById(clientId);
    if (selectedDate != "" && selectedDate != null)
    {
        if (textDataCtl != null)
            textDataCtl.value = selectedDate;
    }
    else
    {
        if (textDataCtl != null)
            textDataCtl.value = "";
    }								
}

</script>
		    <cc2:datemask id="txt_Data" runat="server" Width="75px" CssClass="calendarBox"></cc2:datemask>
		    <asp:ImageButton ID="btn_Cal" Width="16px" runat="server" AlternateText="Calendario" ImageUrl="~/Img/Calendario.gif" Height="16px" />
		   <%-- <input type="image" id="btn_cal_html" src="../Img/Calendario.gif"  alt="Calendario"  onclick="ShowCalendar();"  runat="server"/>		    --%>