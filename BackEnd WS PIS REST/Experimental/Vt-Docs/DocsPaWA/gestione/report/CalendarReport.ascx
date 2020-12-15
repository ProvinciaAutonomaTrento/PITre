<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CalendarReport.ascx.cs" Inherits="DocsPAWA.gestione.report.CalendarReport" %>

<script language="javascript" type="text/javascript">
function ShowCalendar(clientId,fromUrl)
{
	var selectedDate = window.showModalDialog(fromUrl,
								'',
								'dialogWidth:230px;dialogHeight:195px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
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


		    <cc2:datemask id="txt_Data" runat="server" Width="75px" CssClass="testo_grigio"></cc2:datemask>
		    <asp:ImageButton ID="btn_Cal" Width="16px" runat="server" AlternateText="Calendario" ImageUrl="../../images/Calendario.gif" Height="16px" />
