<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewSearchIcons.ascx.cs"
    Inherits="SAAdminTool.UserControls.NewSearchIcons" %>
<script type="text/javascript" language="javascript">

    // Questa funzione mostra una dialog di conferma per l'inserimento di un oggetto
    // nell'area di conservazione
    function showDialogInsertInSA(controlId, objectType) {
        var val;
        val = window.confirm("Vuoi inserire il " + objectType + " nell'area di Conservazione ?");

        if (val) {
            this.showIconWait(controlId);
            return true;
        }
        else {
            return false;
        }
    }

    // Questa funzione visualizza la finestra di conferma per l'eliminazione di un oggetto
    // dall'area di conservazione
    function showDialogRemoveFromSA(controlId, objectType) {
        var val;
        val = window.confirm("Vuoi eliminare il " + objectType + " dall'area di Conservazione ?");

        if (val) {
            this.showIconWait(controlId);
            return true;
        }
        else {
            return false;
        }
    }

    // Questa funzione mostra il nuovo visualizzatore
    function openViewer(path) {
        window.open(
            path,
            '',
            "width=800px,height=600px,top=50,left=100,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes");

        return false;
    }

    // Questa finestra mostra una dialog di conferma per l'inserimento di un oggetto nell'area
    // di lavoro
    function insertInWA(controlId, objectType) {
        var val;
        val = window.confirm("Vuoi inserire il " + objectType + " nell'area di Lavoro ?");

        if (val) {
            this.showIconWait(controlId);
            return true;
        }
        else {
            return false;
        }
    }

    // Questa funzione mostra una dialog di conferma per l'eliminazione di un object dall'area
    // di lavoro
    function removeFromWA(controlId, objectType) {
        var val;
        val = window.confirm("Vuoi eliminare il " + objectType + " dall'area di Lavoro ?");

        if (val) {
            this.showIconWait(controlId);
            return true;
        }
        else {
            return false;
        }
    }

    // Questa funzione mostra la finestra con i dettagli sulla firma
    function showDialogSignInformation(documentId) {
        var height = screen.availHeight;
        var width = screen.availWidth;

        height = (height * 90) / 100;
        width = (width * 90) / 100;

        window.showModalDialog(
            '../popup/dettagliFirmaDoc.aspx?documentId=' + documentId,
			'',
			'dialogHeight: ' + height + 'px; dialogWidth: ' + width + 'px; resizable: yes;status:no;scroll:yes;help:no;close:no');

        return false;
    }

    // Questa funzione mostra una dialog di conferma per la rimozione di un documento da un fascicolo
    function showDialogRemoveDocFromPrj(controlId) {
        var val;
        val = window.confirm("Il documento verrà rimosso dal fascicolo. Continuare?");
        if (val) {
            this.showIconWait(controlId);
            return true;
        } else {
            return false;
        }

    }

    // Questa funzione mostra un alert per avvisare l'utente che non è possibile rimuovere
    // il documento dal fascicolo
    function showDialogCannotRemoveDocument() {
        var agree = alert("Il documento non può essere rimosso dal fascicolo.");
        return true;

    }

    // Questa funzione imposta come icona del controllo con id controlId
    // l'icona 
    function showIconWait(contolId) {
        $get(contolId).src = "../images/mini-loading.gif";

    }
</script>
<asp:ScriptManagerProxy ID="ScriptManagerProxy" runat="server">
</asp:ScriptManagerProxy>
<div id="divDetailsAndADL" style="float: left; width: 40px">
    <div id="divDetails" style="float: left;">
        <asp:ImageButton ID="btnShowDetails" runat="server" BorderColor="#404040" AlternateText="Vai al dettaglio del documento"
            ImageUrl="~/images/proto/dettaglio.gif" Visible="true" />
    </div>
    <div id="divADL" style="float: right;">
        <asp:UpdatePanel ID="upInsertInWA" runat="server">
            <ContentTemplate>
                <asp:ImageButton ID="btnWorkingArea" runat="server" AlternateText="Inserisci il documento nell'Area di Lavoro"
                    ImageUrl="~/images/proto/ins_area.gif" Visible="<%# this.ShowWAButton %>" OnClick="btnWorkingArea_Click"
                    OnClientClick="showIconWait('<%# this.GetInsertInWAClientID %>');" />
                <!-- Questo pulsante sarà visibile solo nell'area di lavoro -->
                <asp:ImageButton ID="btnShowDocumentDetails" runat="server" BorderColor="#404040"
                    AlternateText="Vai al dettaglio del documento" ImageUrl="~/images/proto/dettaglio.gif"
                    Visible="<%# this.ShowDocumentDetailsFromToDoList %>" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<div style="clear: both;" />
<div id="divShowDocumentAndSignInformation" style="float: left; width: 40px;">
    <div style="float: left;">
        <asp:UpdatePanel ID="upShowDocumentFile" runat="server">
            <ContentTemplate>
                <asp:ImageButton ID="btnShowDocumentFile" runat="server" ToolTip="Visualizza immagine documento"
                    Visible="<%# this.ShowFileImageButton %>" 
                    onclick="btnShowDocumentFile_Click" style="width: 14px" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="float: right;">
        <asp:UpdatePanel ID="upSignInformation" runat="server">
            <ContentTemplate>
                <asp:ImageButton ID="btnSignInformation" runat="server" ToolTip="Dettaglio firma"
                    ImageUrl="~/images/tabDocImages/icon_p7m.gif" 
                    Visible="<%# this.IsSigned %>" onclick="btnSignInformation_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<div style="clear: both;" />
<div id="divDeleteAndStorage" style="float: left; width: 40px;">
    <div style="float: left;">
        <asp:UpdatePanel ID="upRemoveDocumentFromProject" runat="server">
            <ContentTemplate>
                <asp:ImageButton ID="btnRemoveDocumentFromProject" runat="server" ImageUrl="~/images/proto/cancella.gif"
                    Visible="<%# this.ShowRemoveFromProjectsButton %>" OnClientClick="return showDialogRemoveDocFromPrj();"
                    OnClick="btnRemoveDocumentFromProject_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="float: right;">
        <asp:UpdatePanel ID="upStorageArea" runat="server">
            <ContentTemplate>
                <asp:ImageButton ID="btnStorageArea" runat="server" ImageUrl="~/images/proto/conservazione_d.gif"
                    ToolTip="Inserisci questo documento in Area conservazione" Visible="<%# this.ShowStorageAreaButton %>"
                    OnClick="btnStorageArea_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<div style="clear: both;" />
