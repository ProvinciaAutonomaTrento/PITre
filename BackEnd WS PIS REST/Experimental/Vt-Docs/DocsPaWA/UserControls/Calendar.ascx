<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendar.ascx.cs" Inherits="DocsPAWA.UserControls.Calendar" %>

<script language="javascript" type="text/javascript">
//function ShowCalendar(clientId,fromUrl)
function ShowCalendar(clientIdDate, clientIdHours, clientIdMinutes, clientIdSeconds, fromUrl)
{
    var retValue = false;
    var selectedDate = window.showModalDialog(fromUrl,'','dialogWidth:230px;dialogHeight:195px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
    var textDataCtl = document.getElementById(clientIdDate);
    
    var textHoursCtl = document.getElementById(clientIdHours);
    var textMinutesCtl = document.getElementById(clientIdMinutes);
    var textSecondsCtl = document.getElementById(clientIdSeconds);


    //var d = new Date;

    if (selectedDate != "" && selectedDate != null) {
        if (textDataCtl != null)
            textDataCtl.value = selectedDate;

        if (textHoursCtl != null)
            textHoursCtl.value = "09";
        //textHoursCtl.value = d.getHours();

        if (textMinutesCtl != null)
            textMinutesCtl.value = "00";
        //textMinutesCtl.value = d.getMinutes();

        if (textSecondsCtl != null)
            textSecondsCtl.value = "00";
        //textSecondsCtl.value = d.getSeconds();

        retValue = true;
    }

    return retValue;    
}
</script>


<cc2:datemask id="txt_Data" runat="server" Width="75px" CssClass="testo_grigio"></cc2:datemask>
<%--<asp:Label ID="lbl_hours" runat="server" Text=":" CssClass="titolo_scheda" Visible="false"></asp:Label>--%>
<asp:TextBox ID="txt_hours" runat="server" MaxLength="2" CssClass="testo_grigio" Columns="2" onKeyPress="ValidateNumKey();" Visible="false"></asp:TextBox>
<asp:Label ID="lbl_minutes" runat="server" Text=":" CssClass="titolo_scheda" Visible="false"></asp:Label>
<asp:TextBox ID="txt_minutes" runat="server" MaxLength="2" CssClass="testo_grigio" Columns="2" onKeyPress="ValidateNumKey();" Visible="false"></asp:TextBox>
<asp:Label ID="lbl_seconds" runat="server" Text=":" CssClass="titolo_scheda" Visible="false"></asp:Label>
<asp:TextBox ID="txt_seconds" runat="server" MaxLength="2" CssClass="testo_grigio" Columns="2" onKeyPress="ValidateNumKey();" Visible="false"></asp:TextBox>
<asp:ImageButton ID="btn_Cal" Width="16px" runat="server" AlternateText="Calendario" ImageUrl="../images/Calendario.gif" Height="16px" />
