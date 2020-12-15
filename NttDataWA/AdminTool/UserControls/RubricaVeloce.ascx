<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RubricaVeloce.ascx.cs"
    Inherits="SAAdminTool.UserControls.RubricaVeloce" %>
<%@ Register Assembly="AjaxControlToolkit, Version=3.0.30930.28736, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e"
    Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<style type="text/css">
    .autocomplete_completionListElement
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
        padding-left: 5px;
        background-color: #ffffff;
        font-family: Verdana, Arial, sans-serif;
        z-index : 1004;
    }
    
    .single_item
    {
        border-bottom: 1px dashed #cccccc;
        padding-top: 2px;
        padding-bottom: 2px;
    }
    
    .single_item_hover
    {
        border-bottom: 1px dashed #cccccc;
        background-color: #9d9e9c;
        color: #000000;
        padding-top: 2px;
        padding-bottom: 2px;
    }

    .selectedWord{
        font-weight:bold;
        color:#000000;
    }

</style>
<asp:Panel ID="pnl_rubr_veloce_ingresso" Visible="false" runat="server">
    <tr>
        <td style="text-align: center; padding-left: 7px;" colspan="2" height="24">
            <script type="text/javascript">
                function acePopulated(sender, e) {
                    var behavior = $find('AutoCompleteExIngresso');
                    var target = behavior.get_completionList();
                    if (behavior._currentPrefix != null) {
                        var prefix = behavior._currentPrefix.toLowerCase();
                        var i;
                        for (i = 0; i < target.childNodes.length; i++) {
                            var sValue = target.childNodes[i].innerHTML.toLowerCase();
                            if (sValue.indexOf(prefix) != -1) {
                                var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                            }
                        }
                    }
                }

                function aceSelected(sender, e) {
                    var value = e.get_value();

                    if (!value) {

                        if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                            value = e._item.parentElement.attributes["_value"].value;

                        else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                            value = e._item.parentElement.parentElement.attributes["_value"].value;

                        else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                            value = e._item.parentNode._value;

                        else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                            value = e._item.parentNode.parentNode._value;

                        else value = "";

                    }

                    var searchText = $get('<%=txtAutoComplete_rubr.ClientID %>').value;
                    searchText = searchText.replace('null', '');
                    var testo = value;
                    var indiceFineCodice = testo.lastIndexOf(')');
                    document.getElementById("<%=this.txtAutoComplete_rubr.ClientID%>").focus();
                    document.getElementById("<%=this.txtAutoComplete_rubr.ClientID%>").value = "";
                    var indiceDescrizione = testo.lastIndexOf('(');
                    var descrizione = testo.substr(0, indiceDescrizione - 1);
                    var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                    document.getElementById('txt_CodMit_P').value = codice;
                    document.getElementById('txt_DescMit_P').value = descrizione;
                    setTimeout('__doPostBack(\'txt_CodMit_P\',\'\')', 0);

                } 


            </script>
            <asp:TextBox ID="txtAutoComplete_rubr" Visible="true" Width="365px" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="autoComplete2" TargetControlID="txtAutoComplete_rubr"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngresso"
                OnClientPopulated="acePopulated">
            </cc2:AutoCompleteExtender>
        </td>
    </tr>
</asp:Panel>
<asp:Panel ID="pnl_rubr_veloce_uscita" Visible="false" runat="server">
    <tr>
        <td style="text-align: center; padding-left: 7px;" colspan="2" height="24">
            <script type="text/javascript">
                function ItemSelectedDestinatario(sender, e) {
                    var value = e.get_value();

                    if (!value) {

                        if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                            value = e._item.parentElement.attributes["_value"].value;

                        else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                            value = e._item.parentElement.parentElement.attributes["_value"].value;

                        else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                            value = e._item.parentNode._value;

                        else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                            value = e._item.parentNode.parentNode._value;

                        else value = "";

                    }

                    var searchText = $get('<%=txtAutoComplete_rubr_dest.ClientID %>').value;
                    searchText = searchText.replace('null', '');
                    var testo = value;
                    var indiceFineCodice = testo.lastIndexOf(')');
                    document.getElementById("<%=this.txtAutoComplete_rubr_dest.ClientID%>").focus();
                    document.getElementById("<%=this.txtAutoComplete_rubr_dest.ClientID%>").value = "";
                    var indiceDescrizione = testo.lastIndexOf('(');
                    var descrizione = testo.substr(0, indiceDescrizione - 1);
                    var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                    document.getElementById('txt_CodDest_P').value = codice;
                    document.getElementById('txt_DescDest_P').value = descrizione;
                    document.all("btn_aggiungiDest_P").click();
                }

                function acePopulatedDest(sender, e) {
                    var behavior = $find('AutoCompleteExDestinatari');
                    var target = behavior.get_completionList();
                    if (behavior._currentPrefix != null) {
                        var prefix = behavior._currentPrefix.toLowerCase();
                        var i;
                        for (i = 0; i < target.childNodes.length; i++) {
                            var sValue = target.childNodes[i].innerHTML.toLowerCase();
                            if (sValue.indexOf(prefix) != -1) {
                                var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                            }
                        }
                    }
                }
            </script>
            <asp:TextBox ID="txtAutoComplete_rubr_dest" Visible="true" Width="365px" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="autoComplete3" TargetControlID="txtAutoComplete_rubr_dest"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="ItemSelectedDestinatario" BehaviorID="AutoCompleteExDestinatari" OnClientPopulated="acePopulatedDest">
            </cc2:AutoCompleteExtender>
        </td>
    </tr>
</asp:Panel>

<asp:Panel ID="pnl_mittente_semplificato_ingresso" Visible="false" runat="server">
<tr class="box_item">
    <td class="titolo_scheda" width="14%">&nbsp;</td>
                 <script type="text/javascript">
                     function acePopulated(sender, e) {
                         var behavior = $find('AutoCompleteExIngresso');
                         var target = behavior.get_completionList();
                         if (behavior._currentPrefix != null) {
                             var prefix = behavior._currentPrefix.toLowerCase();
                             var i;
                             for (i = 0; i < target.childNodes.length; i++) {
                                 var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                 if (sValue.indexOf(prefix) != -1) {
                                     var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                     var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                     var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                     target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                 }
                             }
                         }
                     }

                     function aceSelected(sender, e) {
                         var value = e.get_value();

                         if (!value) {

                             if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                 value = e._item.parentElement.attributes["_value"].value;

                             else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                 value = e._item.parentElement.parentElement.attributes["_value"].value;

                             else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                 value = e._item.parentNode._value;

                             else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                 value = e._item.parentNode.parentNode._value;

                             else value = "";

                         }

                         var searchText = $get('<%=txt_autoComplete_rubr_sempl_ingresso.ClientID %>').value;
                         searchText = searchText.replace('null', '');
                         var testo = value;
                         var indiceFineCodice = testo.lastIndexOf(')');
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_ingresso.ClientID%>").focus();
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_ingresso.ClientID%>").value = "";
                         var indiceDescrizione = testo.lastIndexOf('(');
                         var descrizione = testo.substr(0, indiceDescrizione - 1);
                         var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                         document.getElementById('txtCodMittente').value = codice;
                         document.getElementById('txtDescrMittente').value = descrizione;
                         setTimeout('__doPostBack(\'txtCodMittente\',\'\')', 0);

                     } 


            </script>
    <td colspan="7">
     <asp:TextBox ID="txt_autoComplete_rubr_sempl_ingresso" Visible="true" Width="100%" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="autocomplete4" TargetControlID="txt_autoComplete_rubr_sempl_ingresso"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngresso" OnClientPopulated="acePopulated">
            </cc2:AutoCompleteExtender>
    </td>
    <td>&nbsp;</td>
</tr>
</asp:Panel>

<asp:Panel ID="pnl_autoComplete_rubr_sempl_uscita_mitt" Visible="false" runat="server">
<tr class="box_item">
    <td class="titolo_scheda" width="14%">&nbsp;</td>
                 <script type="text/javascript">
                     function acePopulated(sender, e) {
                         var behavior = $find('AutoCompleteExIngresso');
                         var target = behavior.get_completionList();
                         if (behavior._currentPrefix != null) {
                             var prefix = behavior._currentPrefix.toLowerCase();
                             var i;
                             for (i = 0; i < target.childNodes.length; i++) {
                                 var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                 if (sValue.indexOf(prefix) != -1) {
                                     var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                     var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                     var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                     target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                 }
                             }
                         }
                     }

                     function aceSelected(sender, e) {
                         var value = e.get_value();

                         if (!value) {

                             if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                 value = e._item.parentElement.attributes["_value"].value;

                             else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                 value = e._item.parentElement.parentElement.attributes["_value"].value;

                             else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                 value = e._item.parentNode._value;

                             else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                 value = e._item.parentNode.parentNode._value;

                             else value = "";

                         }

                         var searchText = $get('<%=txt_autoComplete_rubr_sempl_uscita_mitt.ClientID %>').value;
                         searchText = searchText.replace('null', '');
                         var testo = value;
                         var indiceFineCodice = testo.lastIndexOf(')');
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_uscita_mitt.ClientID%>").focus();
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_uscita_mitt.ClientID%>").value = "";
                         var indiceDescrizione = testo.lastIndexOf('(');
                         var descrizione = testo.substr(0, indiceDescrizione - 1);
                         var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                         document.getElementById('txtCodMittUsc').value = codice;
                         document.getElementById('txtDescMittUsc').value = descrizione;
                         setTimeout('__doPostBack(\'txtCodMittUsc\',\'\')', 0);

                     } 


            </script>
    <td colspan="7">
     <asp:TextBox ID="txt_autoComplete_rubr_sempl_uscita_mitt" Visible="true" Width="100%" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica il mittente" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="autocomplete5" TargetControlID="txt_autoComplete_rubr_sempl_uscita_mitt"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngresso" OnClientPopulated="acePopulated">
            </cc2:AutoCompleteExtender>
    </td>
    <td>&nbsp;</td>
</tr>
</asp:Panel>


<asp:Panel ID="pnl_rubrica_veloce_destinatario_sempl" Visible="false" runat="server">
<tr class="box_item">
    <td class="titolo_scheda" width="14%">&nbsp;</td>
            <script type="text/javascript">
                function ItemSelectedDestinatario(sender, e) {
                    var value = e.get_value();

                    if (!value) {

                        if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                            value = e._item.parentElement.attributes["_value"].value;

                        else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                            value = e._item.parentElement.parentElement.attributes["_value"].value;

                        else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                            value = e._item.parentNode._value;

                        else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                            value = e._item.parentNode.parentNode._value;

                        else value = "";

                    }

                    var searchText = $get('<%=txt_rubrica_veloce_destinatario_sempl.ClientID %>').value;
                    searchText = searchText.replace('null', '');
                    var testo = value;
                    var indiceFineCodice = testo.lastIndexOf(')');
                    document.getElementById("<%=this.txt_rubrica_veloce_destinatario_sempl.ClientID%>").focus();
                    document.getElementById("<%=this.txt_rubrica_veloce_destinatario_sempl.ClientID%>").value = "";
                    var indiceDescrizione = testo.lastIndexOf('(');
                    var descrizione = testo.substr(0, indiceDescrizione - 1);
                    var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                    document.getElementById('txtCodDest').value = codice;
                    document.getElementById('txtDescrDest').value = descrizione;
                    document.all("btn_aggiungiDest_P").click();
                }

                function acePopulatedDest(sender, e) {
                    var behavior = $find('AutoCompleteExDestinatari');
                    var target = behavior.get_completionList();
                    if (behavior._currentPrefix != null) {
                        var prefix = behavior._currentPrefix.toLowerCase();
                        var i;
                        for (i = 0; i < target.childNodes.length; i++) {
                            var sValue = target.childNodes[i].innerHTML.toLowerCase();
                            if (sValue.indexOf(prefix) != -1) {
                                var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                            }
                        }
                    }
                }
            </script>
        <td colspan="7">
            <asp:TextBox ID="txt_rubrica_veloce_destinatario_sempl" Visible="true" Width="100%" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica i destinatari" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="autocomplete6" TargetControlID="txt_rubrica_veloce_destinatario_sempl"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="ItemSelectedDestinatario" BehaviorID="AutoCompleteExDestinatari" OnClientPopulated="acePopulatedDest">
            </cc2:AutoCompleteExtender>
        </td>
        <td>&nbsp;</td>
</tr>
</asp:Panel>

<asp:Panel ID="pnl_rubr_veloce_ingr_multiplo" Visible="false" runat="server">
    <tr>
        <td style="text-align: center; padding-left: 7px;" colspan="4" height="24">
            <script type="text/javascript">
                function acePopulatedMultiplo(sender, e) {
                    var behavior = $find('AutoCompleteExIngressoMultiplo');
                    var target = behavior.get_completionList();
                    if (behavior._currentPrefix != null) {
                        var prefix = behavior._currentPrefix.toLowerCase();
                        var i;
                        for (i = 0; i < target.childNodes.length; i++) {
                            var sValue = target.childNodes[i].innerHTML.toLowerCase();
                            if (sValue.indexOf(prefix) != -1) {
                                var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                            }
                        }
                    }
                }

                function aceSelectedMultiplo(sender, e) {
                    var value = e.get_value();

                    if (!value) {

                        if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                            value = e._item.parentElement.attributes["_value"].value;

                        else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                            value = e._item.parentElement.parentElement.attributes["_value"].value;

                        else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                            value = e._item.parentNode._value;

                        else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                            value = e._item.parentNode.parentNode._value;

                        else value = "";

                    }

                    var searchText = $get('<%=txtAutoComplete_rubr_mitt_multiplo.ClientID %>').value;
                    searchText = searchText.replace('null', '');
                    var testo = value;
                    var indiceFineCodice = testo.lastIndexOf(')');
                    document.getElementById("<%=this.txtAutoComplete_rubr_mitt_multiplo.ClientID%>").focus();
                    document.getElementById("<%=this.txtAutoComplete_rubr_mitt_multiplo.ClientID%>").value = "";
                    var indiceDescrizione = testo.lastIndexOf('(');
                    var descrizione = testo.substr(0, indiceDescrizione - 1);
                    var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                    document.getElementById('txt_cod_mitt_mult_nascosto').value = codice;
                    document.getElementById('txt_desc_mitt_mult_nascosto').value = descrizione;
                    document.all("btn_nascosto_mitt_multipli").click();

                } 


            </script>
            <asp:TextBox ID="txtAutoComplete_rubr_mitt_multiplo" Visible="false" Width="365px" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica Mittente Multiplo" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="AutoComplete7" TargetControlID="txtAutoComplete_rubr_mitt_multiplo"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="aceSelectedMultiplo" BehaviorID="AutoCompleteExIngressoMultiplo"
                OnClientPopulated="acePopulatedMultiplo">
            </cc2:AutoCompleteExtender>
        </td>
    </tr>
</asp:Panel>

<asp:Panel ID="pnl_rubr_veloce_mitt_sempl_multipli" Visible="false" runat="server">
<tr class="box_item">
    <td class="titolo_scheda" width="14%">&nbsp;</td>
                 <script type="text/javascript">
                     function acePopulatedSempl(sender, e) {
                         var behavior = $find('AutoCompleteExIngressoSempl');
                         var target = behavior.get_completionList();
                         if (behavior._currentPrefix != null) {
                             var prefix = behavior._currentPrefix.toLowerCase();
                             var i;
                             for (i = 0; i < target.childNodes.length; i++) {
                                 var sValue = target.childNodes[i].innerHTML.toLowerCase();
                                 if (sValue.indexOf(prefix) != -1) {
                                     var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                                     var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                                     var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                                     target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                                 }
                             }
                         }
                     }

                     function aceSelectedSempl(sender, e) {
                         var value = e.get_value();

                         if (!value) {

                             if (e._item.parentElement && e._item.parentElement.tagName == "LI")

                                 value = e._item.parentElement.attributes["_value"].value;

                             else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI")

                                 value = e._item.parentElement.parentElement.attributes["_value"].value;

                             else if (e._item.parentNode && e._item.parentNode.tagName == "LI")

                                 value = e._item.parentNode._value;

                             else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI")

                                 value = e._item.parentNode.parentNode._value;

                             else value = "";

                         }

                         var searchText = $get('<%=txt_autoComplete_rubr_sempl_ingresso_multi.ClientID %>').value;
                         searchText = searchText.replace('null', '');
                         var testo = value;
                         var indiceFineCodice = testo.lastIndexOf(')');
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_ingresso_multi.ClientID%>").focus();
                         document.getElementById("<%=txt_autoComplete_rubr_sempl_ingresso_multi.ClientID%>").value = "";
                         var indiceDescrizione = testo.lastIndexOf('(');
                         var descrizione = testo.substr(0, indiceDescrizione - 1);
                         var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                         document.getElementById('txt_cod_mitt_mult_nascosto').value = codice;
                         document.getElementById('txt_desc_mitt_mult_nascosto').value = descrizione;
                         document.all("btn_nascosto_mitt_multipli").click();

                     } 


            </script>
    <td colspan="7">
     <asp:TextBox ID="txt_autoComplete_rubr_sempl_ingresso_multi" Visible="false" Width="100%" CssClass="testo_grigio"
                ToolTip="Cerca in rubrica Mitt Multiplo" runat="server"></asp:TextBox>
            <cc2:AutoCompleteExtender runat="server" ID="AutoComplete8" TargetControlID="txt_autoComplete_rubr_sempl_ingresso_multi"
                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                UseContextKey="true" OnClientItemSelected="aceSelectedSempl" BehaviorID="AutoCompleteExIngressoSempl" OnClientPopulated="acePopulatedSempl">
            </cc2:AutoCompleteExtender>
    </td>
    <td>&nbsp;</td>
</tr>
</asp:Panel>