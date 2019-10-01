<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorOwnerFilter.ascx.cs"
    Inherits="SAAdminTool.UserControls.AuthorOwnerFilter" %>
<script src="../LIBRERIE/DocsPA_Func.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">

    // Gestione visualizzazione maschera rubrica
    function ShowDialogAddressBook(corrType, control) {
        var w_width = screen.availWidth - 40;
        var w_height = screen.availHeight - 35;

        var navapp = navigator.appVersion.toUpperCase();
        if ((navapp.indexOf("WIN") != -1) && (navapp.indexOf("NT 5.1") != -1))
            w_height = w_height + 20;

        var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:no;scroll:no;status:no;unadorned:yes";

        var params = "calltype=" + Rubrica.prototype.CALLTYPE_OWNER_AUTHOR + "&tipo_corr=" + corrType + "&control=" + control;

        var urlRubrica = "../popup/rubrica/Rubrica.aspx";
        var res = window.showModalDialog(urlRubrica + "?" + params, window, opts);
    }

    function OpenAddressBook(radioButtonListId, control) {
        var radioButtonList = document.getElementById(radioButtonListId);
        var radioButtons = document.getElementsByName(radioButtonList.childNodes[0].name);

        // Se radio buttuns è undefined, significa che si sta utilizzando il controllo in configurazione
        // proprietario e quindi il primo option busson è disabilitato. Di conseguenza, il primo figlio
        // di radioButtonList è uno span, quindi bisogna prendere il secondo
        if (radioButtonList.childNodes[0].name == undefined)
            radioButtons = document.getElementsByName(radioButtonList.childNodes[1].name);

        var selectedRadioButton;

        for (var x = 0; x < radioButtons.length; x++) {
            if (radioButtons[x].checked) {
                selectedRadioButton = radioButtons[x];
            }
        }

        ShowDialogAddressBook(selectedRadioButton.value, control);
        
    }
		
</script>
<style type="text/css">
    .background-style
    {
        border-right: #7d7d7d 1px solid;
        border-top: #7d7d7d 1px solid;
        border-left: #7d7d7d 1px solid;
        border-bottom: #7d7d7d 1px solid;
        background-color: #fafafa;
    }
</style>
<div class="background-style" id="divContainer" style="text-align: center; border-width: 1px;
    width: 97%;">
    <div class="testo_grigio">
        <div style="float: left; padding-top: 5px;" class="titolo_scheda">
            &nbsp;&nbsp;<asp:Label ID="lblLabel" runat="server">Proprietario:</asp:Label>
        </div>
        <div class="titolo_scheda" style="float: left;">
            <asp:RadioButtonList ID="rblCorrType" runat="server" CssClass="titolo_scheda" RepeatLayout="Flow"
                RepeatDirection="Horizontal" AutoPostBack="True" 
                onselectedindexchanged="rblCorrType_SelectedIndexChanged">
                <asp:ListItem Value="U">UO</asp:ListItem>
                <asp:ListItem Value="R" Selected="True">Ruolo</asp:ListItem>
                <asp:ListItem Value="P">Persona</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div style="float: right; margin-right: 5px;" class="titolo_scheda">
            <asp:ImageButton ID="btnShowAddressBook" runat="server" ImageUrl="~/images/proto/rubrica.gif"
                Height="19px" Width="29px" onclick="btnShowAddressBook_Click" />
        </div>
        <div style="clear: both;" />
        <div style="float: left; vertical-align: top;" class="titolo_scheda">
            &nbsp;
            <asp:TextBox ID="txtCorrCode" runat="server" Width="80px" CssClass="testo_grigio"
                AutoPostBack="True" OnTextChanged="txtCodiceUtenteCreatore_TextChanged"></asp:TextBox>&nbsp;
            <asp:TextBox ID="txtCorrDescription" runat="server" Width="280px" CssClass="testo_grigio"></asp:TextBox>
        </div>
        <div style="clear: both;" />
        <div style="float: right; margin-right: 3px;">
            <asp:CheckBox CssClass="titolo_scheda" ID="chkExtendToHistoricized" Checked="true" runat="server" Text="Estendi a storicizzati" ToolTip="Estendi ricerca a ruoli storicizzati" />
        </div>
    </div>
</div>
