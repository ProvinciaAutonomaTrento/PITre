<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="newParoleChiave.aspx.cs" Inherits="DocsPAWA.popup.newParoleChiave" %>
<%@ Register TagPrefix="uct" TagName="AppTitleProvider" Src="~/UserControls/AppTitleProvider.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Oggetto" Src="~/documento/Oggetto.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc8" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
   <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR"/>
    <meta content="C#" name="CODE_LANGUAGE"/>
    <meta content="JavaScript" name="vs_defaultClientScript"/>
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema"/>

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
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
            text-decoration:underline;
        }
    </style>
    
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet"/>
    <base target="_self" />
</head>
<body ms_positioning="GridLayout" bottommargin="0" leftmargin="5" topmargin="5" rightmargin="5">
    <form id="NewParoleChiave" method="post" runat="server">
    <asp:ScriptManager ID="ScriptManagerNewKeyWord" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Parole chiave" />
    <div>
    <table class="info" style="width: 500px; height: 345px" align="center" border="0" id="principale">
        <tr>
            <td class="item_editbox">
                <p class="boxform_item">
                    <asp:Label ID="lblRicOgg" runat="server">Parole Chiave</asp:Label>
                </p>
            </td>
        </tr>
        <tr>
            <td>
                <table width="500px" border="0" class="infoDotted">
                    <tr align="left">
                        <td valign="top">
                            <asp:label id="lblCombo" class="titolo_scheda" visible="true" runat="Server">
                            Registro</asp:label>
                        </td>
                        <td width="85%">
                            <asp:DropDownList ID="ddlRegRf" runat="Server" Width="180px" 
                                CssClass="testo_grigio_combo" 
                                onselectedindexchanged="ddlRegRf_SelectedIndexChanged" 
                                AutoPostBack="True" />
                        </td>     
                    </tr>
                    <tr>
                        <td valign="bottom" colspan="2">
                            <asp:TextBox ID="txtKeyWord" runat="server" Width="400px" 
                                ontextchanged="txtKeyWord_TextChanged"></asp:TextBox>
                            <cc8:AutoCompleteExtender runat="server" ID="keyWord" TargetControlID="txtKeyWord"
                            CompletionListCssClass="autocomplete_completionListElementbis" CompletionListItemCssClass="single_itembis"
                            CompletionListHighlightedItemCssClass="single_item_hoverbis" ServiceMethod="GetListaParoleChiaveVeloce"
                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS" 
                            OnClientPopulated="acePopulated ">
                            </cc8:AutoCompleteExtender>
                            <script type="text/javascript">

                                function checkWord() {
                                    var key = document.getElementById('txtKeyWord').value;
                                    if (key == null || key == '')
                                        alert('Inserire una parola chiave');
                                    else document.getElementById('hd_newKey').value = key;
                                }

                                function acePopulated(sender, e) {
                                    document.getElementById('btn_aggiungi').disabled = true;
                                    var behavior = $find('AutoCompleteExIngressoBIS');
                                    var target = behavior.get_completionList();
                                    //alert(target.childNodes.length);
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

                                    var searchText = $get('<%=txtKeyWord.ClientID %>').value;
                                    searchText = searchText.replace('null', '');
                                    var testo = value;
                                    //alert(testo);
                                    var indiceFineCodice = testo.lastIndexOf(')');
                                    document.getElementById("<%=this.txtKeyWord.ClientID%>").focus();
                                    document.getElementById("<%=this.txtKeyWord.ClientID%>").value = "";
                                    var indiceDescrizione = testo.lastIndexOf('(');
                                    var descrizione = testo.substr(0, indiceDescrizione - 1);
                                    //alert(descrizione);
                                    var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
                                    //alert(codice);
                                    //document.getElementById('txt_CodMit_P').value = codice;
                                    document.getElementById('txtKeyWord').value = descrizione;
                                    document.getElementById('hd_idKeyWord').value = codice;
                                    setTimeout('__doPostBack(\'txtKeyWord\',\'\')', 0);

                                } 


                        </script>
                        <asp:HiddenField ID="hd_idKeyWord" runat="server" />
                        <asp:HiddenField ID="hd_newKey" runat="server" />
                        </td>
                        <td width="10%" align="left">
                            <cc1:imagebutton id="btn_aggiungi" DisabledUrl="../images/proto/aggiungi.gif" AlternateText="Aggiungi parola chiave"
							ImageUrl="../images/proto/aggiungi.gif" Tipologia="DO_DOC_ADD_PAROLA" Runat="server" Visible="False" OnClientClick="checkWord()"></cc1:imagebutton>
                        </td>
                    </tr>
                    <tr>
                        <td valign="bottom" colspan="2">
                            &nbsp;</td>
                        <td width="10%" align="left">
                            <cc1:imagebutton id="btn_rimuovi" DisabledUrl="~/images/proto/cancella.gif" AlternateText="Elimina dalla Lista"
							ImageUrl="~/images/proto/cancella.gif" Runat="server" Visible="True" onclick="btn_rimuovi_Click"></cc1:imagebutton>
                        </td>
                    </tr>
                  </table> 
            </td> 
        </tr>
        <tr style="display: none">
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td valign="top" align="center" width="500px">
                <div style="overflow: auto; height: 200px">
                    <asp:Label ID="lbl_risultatoRicOgg" TabIndex="4" CssClass="testo_grigio_scuro_grande"
                        runat="server" Visible="False">Nessun oggetto presente.</asp:Label>
                    <asp:listbox id="ListParoleChiave" runat="server" CssClass="testo_grigio" SelectionMode="Multiple"
							Width="480px" Height="200px"></asp:listbox>
                </div>
            </td>
        </tr>
        <tr>
            <td height="2">
            </td>
        </tr>
        <tr>
            <td align="center" height="30">
                <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK" 
                    onclick="btn_ok_Click"></asp:Button>
                &nbsp;
                <input class="PULSANTE" id="btn_chiudi" onclick="javascript:window.close()" type="button"
                    value="CHIUDI">
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
