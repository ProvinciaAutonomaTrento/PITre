<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChannelDetail.ascx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Pubblicazioni.ChannelDetail" %>
<%@ Register TagPrefix="uc1" TagName="Time" Src="TimeControl.ascx" %>
<%@ Register TagPrefix="uc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<table style="height: 100%; width: 100%" cellSpacing="1" cellPadding="0" border="0" align="center" class="contenitore">
	<tr style="height: 20px">
        <td colspan="2" style="height: 5px" class="titolo_pnl">
            Configurazioni sottoscrittore
        </td>
	</tr>    
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right">
            <asp:Label ID="lblSubscriberUrl" runat="server" class="testo_grigio_scuro" Text="Url del servizio:"></asp:Label>
        </td>
        <td style="width: 65%; text-align: left">
            <asp:TextBox ID="txtSubscriberUrl" runat="server" AutoPostBack="true" CssClass="testo" Width="490px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="requiredTxtSubscriberUrl" runat="server" ControlToValidate="txtSubscriberUrl" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
            <asp:Button ID="btnFetchSubscribers" runat="server" CausesValidation="false" Width="120px" CssClass="testo_btn_p" Text="Carica sottoscrittori" OnClick="btnFetchSubscribers_Click"></asp:Button>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right; vertical-align:text-top">
            <asp:Label ID="lblSubscriberName" runat="server" class="testo_grigio_scuro" Text="Nome del sottoscrittore:"></asp:Label>
        </td>
        <td style="width: 65%; text-align: left">
            <asp:DropDownList ID="cboSubscribers" runat="server" CssClass="testo" Width="500px"></asp:DropDownList>
            <asp:RequiredFieldValidator ID="tequiredCboSubscribers" runat="server" ControlToValidate="cboSubscribers" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
        </td>
    </tr>
	<tr style="height: 20px">
        <td colspan="2" style="height: 5px" class="titolo_pnl">
            Modalità di avvio del servizio
        </td>
	</tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:Label ID="lblComputeFrom" runat="server" class="testo_grigio_scuro" Text="Calcola dal:"></asp:Label>
        </td>
        <td style="width: 65%; text-align: left" class="testo">
            <uc2:DateMask ID="txtComputeFrom" runat="server" CssClass="testo" Width="80px"></uc2:DateMask>
            <asp:RequiredFieldValidator ID="requiredTxtComputeFrom" runat="server" ControlToValidate="txtComputeFrom" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
            <uc1:Time id="txtComputeFromTime" runat="server"></uc1:Time>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group1" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true" />
        </td>
        <td style="width: 65%" class="testo">
            ogni 
            <asp:TextBox ID="txtGroup1Number" runat="server" Width="40px"  CssClass="testo"></asp:TextBox>
            <asp:DropDownList ID="cboGroup1ExecutionMode" runat="server"  CssClass="testo">
                <asp:ListItem Value="BySecond" Text="Secondo" Selected="True"></asp:ListItem>
                <asp:ListItem Value="ByMinute" Text="Minuto"></asp:ListItem>
                <asp:ListItem Value="Hourly" Text="Ora"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group2" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true"/>
        </td>
        <td style="width: 65%; text-align: left" class="testo">
            ogni giorno alle 
            <uc1:Time id="timeGroup2" runat="server"></uc1:Time>
        </td>
    </tr>
    <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group3" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true" />
        </td>
        <td style="width: 65%; text-align: left" class="testo">
            ogni <asp:DropDownList ID="cboGroup3Days" runat="server" CssClass="testo">
                <asp:ListItem Value="1" Text="Lunedì" Selected="True"></asp:ListItem>
                <asp:ListItem Value="2" Text="Martedì"></asp:ListItem>
                <asp:ListItem Value="3" Text="Mercoledì"></asp:ListItem>
                <asp:ListItem Value="4" Text="Giovedì"></asp:ListItem>
                <asp:ListItem Value="5" Text="Venerdì"></asp:ListItem>
                <asp:ListItem Value="6" Text="Sabato"></asp:ListItem>
                <asp:ListItem Value="0" Text="Domenica"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="requiredCboGroup3Days" runat="server" ControlToValidate="cboGroup3Days" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
            alle 
            <uc1:Time id="timeGroup3" runat="server"></uc1:Time>
        </td>
    </tr>
        <tr style="height: 25px">
        <td style="width: 35%; text-align: right" class="testo">
            <asp:RadioButton ID="group4" runat="server" GroupName="Frequence" class="testo_grigio_scuro" AutoPostBack="true" />
        </td>
        <td style="width: 65%; text-align: left" class="testo">

            ogni <asp:TextBox ID="txtGroup4Number" runat="server" Width="40px"  CssClass="testo"></asp:TextBox> Minuti
            dalle 
            <uc1:Time id="timeGroup4Start" runat="server"></uc1:Time>
            alle 
            <uc1:Time id="timeGroup4Stop" runat="server"></uc1:Time>
        </td>
    </tr>

</table>
