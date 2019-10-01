<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricaComune.ascx.cs"
    Inherits="DocsPAWA.AdminTool.UserControl.RubricaComune" %>
<div style="border: 1 solid #810d06;">
    <div style="text-align: left; padding: 3;" class="titolo_pnl">
        Rubrica Comune:
    </div>
    <div style="clear: both; padding: 5 0 0 0;" />
    <div class="testo_grigio_scuro" style="float: left; width: 20%; margin-left: 5;">
        Codice:
    </div>
    <div class="testo_grigio_scuro">
        <asp:Label ID="lblCodiceRC" runat="server"></asp:Label>
    </div>
    <div style="clear: both; padding: 5 0 0 0;" />
    <div class="testo_grigio_scuro" style="float: left; width: 20%; margin-left: 5;">
        Descrizione:
    </div>
    <div class="testo" style="float: left;">
        <asp:Label ID="lblDescrizioneRC" runat="server"></asp:Label>
    </div>
    <div style="clear: both; padding: 5 0 0 0;" />
    <div class="testo" style="float: left; width: 20%; margin-left: 5;">
        Creato il:
    </div>
    <div class="testo" style="float: left;">
        <asp:Label ID="lblDataCreazioneRC" runat="server"></asp:Label>
    </div>
    <div style="clear: both; padding: 5 0 0 0;" />
    <div class="testo_grigio_scuro" style="float: left; width: 20%; margin-left: 5;">
        Modificato il:
    </div>
    <div class="testo_grigio_scuro" style="float: left;">
        <asp:Label ID="lblDataUltimaModificaRC" runat="server"></asp:Label>
    </div>
    <div style="clear: both; padding: 5 0 0 0;" />
    <div style="text-align: right; padding: 3;" class="titolo_pnl">
        <asp:Button ID="btnPubblicaInRC" CssClass="testo_btn" runat="server" Text="Pubblica in RC" />
    </div>
</div>
