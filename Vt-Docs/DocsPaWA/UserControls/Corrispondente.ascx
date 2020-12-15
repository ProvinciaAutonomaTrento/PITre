<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Corrispondente.ascx.cs" Inherits="DocsPAWA.UserControls.Corrispondente" %>
<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc8" %>
<script type="text/javascript" language="javascript" src="<%=ResolveClientUrl("~/LIBRERIE/rubrica.js")%>"></script>
<!--
<script type="text/javascript" language="javascript" src="/LIBRERIE/rubrica.js"></script>
<script type="text/javascript" language="javascript" src="../LIBRERIE/rubrica.js"></script>
<script type="text/javascript" language="javascript" src="../../LIBRERIE/rubrica.js"></script>
<script type="text/javascript" language="javascript" src="../../../LIBRERIE/rubrica.js"></script>
<script type="text/javascript" language="javascript" src="../../../../LIBRERIE/rubrica.js"></script>
-->

<script type="text/javascript">
    function apriRubrica(calltype) 
	{
        var r = new Rubrica();
        switch (calltype) 
		{
            case "CALLTYPE_CORR_INT":
                r.CallType = r.CALLTYPE_CORR_INT;
                var res = r.Apri();
                break;

            case "CALLTYPE_CORR_EST":
                r.CallType = r.CALLTYPE_CORR_EST;
                var res = r.Apri();
                break;

            case "CALLTYPE_CORR_INT_EST":
                r.CallType = r.CALLTYPE_CORR_INT_EST;
                var res = r.Apri();
                break;

            case "CALLTYPE_CORR_NO_FILTRI":
                r.CallType = r.CALLTYPE_CORR_NO_FILTRI;
                var res = r.Apri();
                break;

            case "CALLTYPE_CORR_INT_NO_UO":
                r.CallType = r.CALLTYPE_CORR_INT_NO_UO;
                var res = r.Apri();
                break;

            case "CALLTYPE_RICERCA_CORR_NON_STORICIZZATO":
                r.CallType = r.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO;
                var res = r.Apri();
                break;
        }
    }

    function ApriFinestraMultiCorrispondenti(codice_clientId, desc_clientId, corr_clientId) {
        var rtnValue = window.showModalDialog('../popup/MultiDestinatari.aspx?tipo=M&page=corrispondente&corrId=' + corr_clientId, '', 'dialogWidth:700px;dialogHeight:350px;status:no;center:yes;resizable:no;scroll:yes;help:no;');
        if (rtnValue != undefined) {
            var ar = rtnValue.split("@-@");
            var descrizione = new String(ar[0]);
            if (descrizione.indexOf("^^^") > 0) {
                descrizione = descrizione.replace("^^^", "'");
            }
            document.getElementById(desc_clientId).value = descrizione;
            document.getElementById(codice_clientId).value = ar[1];
        }
    }

</script>
<style type="text/css">
    .autocomplete_completionListElementbis
    {
        height: 280px;
        list-style-type: none;
        margin: 0px;
        padding: 0px;
        font-size: 10px;
        color: #333333;
        line-height: 18px;
        border: 1px solid #333333;
        overflow: auto;
        padding-left: 1px;
        background-color: #ffffff;
        font-family: Verdana, Arial, sans-serif;
        z-index: 1004;
    }
    
    .single_itembis
    {
        border-bottom: 1px dashed #cccccc;
        padding-top: 2px;
        padding-bottom: 2px;
    }
    
    .single_item_hoverbis
    {
        border-bottom: 1px dashed #cccccc;
        background-color: #9d9e9c;
        color: #000000;
        padding-top: 2px;
        padding-bottom: 2px;
    }
    
    .selectedWord
    {
        font-weight: bold;
        color: #000000;
    }
</style>
<asp:Panel ID="pnl_Corrispondente" Width="100%" runat="server">
    <input id="txt_SystemId" type="hidden" runat="server" />
    <asp:TextBox ID="txt_Codice" Width="20%" runat="server" AutoPostBack="True" OnTextChanged="txt_Codice_TextChanged"></asp:TextBox>
    <asp:TextBox ID="txt_Descrizione" Width="50%" runat="server" AutoPostBack="True" OnTextChanged="txt_Descrizione_TextChanged"></asp:TextBox>
    <asp:ImageButton ID="btn_Rubrica" Width="30px" Height="20px" runat="server" ImageUrl="~/images/proto/rubrica.gif" OnClick="btn_Rubrica_Click" AlternateText="Seleziona un corrispondente dalla rubrica" ToolTip="Seleziona un corrispondente dalla rubrica" />
    <asp:CheckBox ID="cbx_storicizzato" runat="server" Text="Storicizzati" Checked="false" Visible="false" OnCheckedChanged="cbx_storicizzato_checkedChanged" />
<cc8:AutoCompleteExtender runat="server" ID="mittente_veloce" TargetControlID="txt_Descrizione"
                CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true">
            </cc8:AutoCompleteExtender>
</asp:Panel>
