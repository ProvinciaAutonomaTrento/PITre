<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PannelloRicercaModelliTrasmissione.ascx.cs"
    Inherits="DocsPAWA.UserControls.PannelloRicercaModelliTrasmissione" %>
<style type="text/css">
    .field_set
    {
        width: 99%;
    }
    
    .content
    {
        background-color: #eaeaea;
        margin-left: 5px;
        margin-top: 5px;
        padding: 5pt;
        border: 1px solid #cccccc;
        width: 98%;
    }
    
    .label
    {
        font-weight: bold;
        font-size: 10px;
        font-family: Verdana;
    }
    
    .filter
    {
        /*background-color: #fafafa;
        border: 1px solid #cccccc;*/
        margin: 1px;
        padding: 5px;
    }
    
    </style>
    <script type="text/javascript" language="javascript">
        function ApriRubricaSelDest() {
            var r = new Rubrica();
            r.CallType = r.CALLTYPE_DEST_FOR_SEARCH_MODELLI;
            var res = r.Apri();
        }

        /*
         * Cancellazione della descrizione di un corrispondente se il codice è vuoto
         */
        function clearCorrData(codTxt, descrTxtId) {
            if (codTxt.value == '')
                document.getElementById(descrTxtId).value = '';
            
        }
    </script>
<fieldset class="field_set">
    <legend class="testo_piccoloB">Ricerca per modello:</legend>
    <asp:Panel ID="pnlNoResult" runat="server" CssClass="content">
        <div class="filter" style="float: left; width: 97%; text-align: right;">
            <asp:Label ID="lblNoResult" runat="server" ForeColor="Red" />
        </div>
    </asp:Panel>
    <div class="content">
        <div class="filter" style="float: left; width: 17%;">
            <span class="testo_piccoloB">Codice</span><br />
            <asp:TextBox CssClass="testo" ID="txtCodice" runat="server" Width="99%" />
        </div>
        <div class="filter" style="float: left; width: 40%;">
            <span class="testo_piccoloB">Modello</span><br />
            <asp:TextBox CssClass="testo" ID="txtModello" runat="server"
                Width="99%" />
        </div>
        <div class="filter" style="float: left; width: 40%">
            <span class="testo_piccoloB">Note</span><br />
            <asp:TextBox CssClass="testo" ID="txtNote" runat="server" Width="99%" />
        </div>
    </div>
    <div class="content">
        <div class="filter" style="float: left; width: 17%">
            <span class="testo_piccoloB">Tipo Trasmissione</span>
            <br />
            <asp:DropDownList ID="ddlTipoTrasmissione" runat="server" CssClass="testo_piccolo"
                Width="99%">
                <asp:ListItem />
                <asp:ListItem Text="Documento" Value="D" />
                <asp:ListItem Text="Fascicolo" Value="F" />
            </asp:DropDownList>
        </div>
        <div style="float: left; width: 40%;" class="filter">
            <span class="testo_piccoloB">Registro</span>
            <br />
            <asp:DropDownList ID="ddlRegistri" runat="server" CssClass="testo_piccolo" Width="99%" />
        </div>
        <div style="float: left; width: 40%;" class="filter">
            <span class="testo_piccoloB">Ragione Trasmissione</span>
            <br />
            <asp:DropDownList ID="ddlRagioniTrasmissione" runat="server" CssClass="testo_piccolo"
                Width="99%" />
        </div>
    </div>
    <div class="content">
        <asp:Panel runat="server" ID="pnlVisibilita" CssClass="filter" Visible="false" Width="48%">
            <span class="testo_piccoloB">Visibilità</span>&nbsp;
            <asp:DropDownList ID="ddlRegistry" runat="server" CssClass="testo"></asp:DropDownList>
            <br />
            <asp:TextBox ID="txtVisibilitaCodice" CssClass="testo" runat="server" Width="15%" />
            &nbsp;<asp:ImageButton ID="imgSrcVisibilita" runat="server" AlternateText="Selezione per codice inserito"
                ImageUrl="~/images/rubrica/b_arrow_right.gif" OnClick="imgSrcVisibilita_Click"
                Style="height: 12px" />
            &nbsp;&nbsp;<asp:TextBox ID="txtVisibilitaDescrizione" CssClass="testo" runat="server"
                ReadOnly="true" Width="60%" />
            <asp:ImageButton ID="imgRubricaVisibilita" runat="server" Visible="True" ImageUrl="~/images/proto/rubrica.gif"
                AlternateText="Seleziona corrispondente per visibilità" OnClick="imgRubricaVisibilita_Click" /><br />
            
        </asp:Panel>
        <asp:Panel ID="pnlDestinatari" runat="server" CssClass="filter" Width="49%">
            <span class="testo_piccoloB">Destinatari</span>
            <br />
            <asp:TextBox ID="txtDestinatariCodice" CssClass="testo" runat="server" Width="15%" />
            &nbsp;<asp:ImageButton ID="imgSrcDestinatari" runat="server" AlternateText="Selezione per codice inserito"
                ImageUrl="~/images/rubrica/b_arrow_right.gif" OnClick="imgSrcDestinatari_Click" />
            &nbsp;&nbsp;<asp:TextBox ID="txtDestinatariDescrizione" CssClass="testo" runat="server"
                ReadOnly="true" Width="60%" />
            <asp:ImageButton ID="imgRubricaDestinatari" runat="server" Visible="True" ImageUrl="~/images/proto/rubrica.gif"
                AlternateText="Seleziona il destinatario dalla rubrica" OnClick="imgRubricaDestinatari_Click" />
        </asp:Panel>
    </div>
    <div class="content">
        <div class="filter" style="float: left; width: 97%">
            <asp:CheckBox ID="chkRuoloDestDisabilitato" CssClass="testo_piccoloB" runat="server"
                Text="Ruolo destinatario disabilitato" />&nbsp;
            <asp:CheckBox ID="chkRuoloDestInibito" CssClass="testo_piccoloB" runat="server" Text="Ruolo destinatario inibito ricezione trasmissioni" />
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlRange" CssClass="content" Visible="false">
        <div class="filter" style="float: left; width: 97%">
            <span class="testo_piccoloB">Cerca:</span><br />
            <asp:RadioButtonList ID="rblSearchScope" runat="server" RepeatDirection="Horizontal"
                CssClass="testo_piccoloB">
                <asp:ListItem Text="Fra i modelli creati utente" Value="OnlyUser" />
                <asp:ListItem Text="Fra i modelli creati dall'amministratore" Value="OnlyAdmin" />
                <asp:ListItem Text="Ovunque" Value="Everywhere" Selected="True" />
            </asp:RadioButtonList>
        </div>
    </asp:Panel>
    <div class="content">
        <div class="filter" style="float: left; width: 97%; text-align: right;">
            <asp:Button Text=" Trova e sost." runat="server" ID="btnFindAndReplace" 
                onclick="btnFindAndReplace_Click" />&nbsp;
            <asp:Button Text="Cerca" runat="server" ID="btnFind" OnClick="btnFind_Click" />&nbsp;
            <asp:Button Text="Esporta" runat="server" ID="btnExport" OnClick="btnExport_Click" />
        </div>
    </div>
</fieldset>
