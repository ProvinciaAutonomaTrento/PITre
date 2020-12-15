<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddNewProcess.aspx.cs" Inherits="NttDataWA.Popup.AddNewProcess" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container {
            position: fixed;
            top: 1px;
            left: 0px;
            bottom: 71px;
            right: 0px;
            overflow: auto;
            background: #ffffff;
            text-align: left;
            padding: 10px;
        }

        .txt_textarea {
            width: 100%;
            border: 1px solid #cccccc;
            line-height: 18px;
            font-family: Verdana, Arial, Verdana, sans-serif;
            font-size: 13px;
            height: 50px;
            overflow: auto;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
        .contentListProcess {
            height: 140px;
        }

         .contentListProcess1 {
            height: 140px;
            overflow:auto;
        }

        .contentListProcess fieldset {
            margin: 0px;
            border: 1px solid #cccccc;
            padding: 0px;
            width: 98%;
            border-radius: 5px;
            -ms-border-radius: 5px; /* ie */
            -moz-border-radius: 5px; /* firefox */
            -webkit-border-radius: 5px; /* safari, chrome */
            -o-border-radius: 5px; /* opera */
        }
         </style>

        <script type="text/javascript">

        function closeAddressBookPopup() {
            $('#btnAddressBookPostback').click();
        }

        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoBIS');
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
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function aceSelected(sender, e) {
            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=txtDescrizioneRuolo.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneRuolo.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneRuolo.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceRuolo.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneRuolo.ClientID%>").value = descrizione;

            document.getElementById("<%=btnRuolo.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <asp:Panel ID="PnlDuplicaProcesso" runat="server">
            <p id="rowDescription" runat="server">
                <span class="weight">
                    <asp:Literal ID="ProcessName" runat="server"></asp:Literal></span><br />
                <cc1:CustomTextArea ID="txt_processName" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                    ClientIDMode="Static" TextMode="MultiLine">
                </cc1:CustomTextArea>
                <span class="col-right">
                    <asp:Literal ID="VersionsLitChars" runat="server"></asp:Literal>
                    <span id="VersionDescription_chars"></span></span>
            </p>
        </asp:Panel>
        <asp:Panel ID="PnlCopiaProcesso" runat="server" Visible="false">
            <asp:UpdatePanel ID="UpdPnlRuolo" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="weight">
                                    <asp:Literal ID="ltlRuolo" runat="server" /></span>
                            </p>
                        </div>
                        <div class="col-right-no-margin">
                            <cc1:CustomImageButton runat="server" ID="ImgAddressBookRuolo" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="BtnAddressBook_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:HiddenField ID="idRuolo" runat="server" />
                        <div class="colHalf">
                            <cc1:CustomTextArea ID="txtCodiceRuolo" runat="server" CssClass="txt_addressBookLeft"
                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoComplete="off"
                                OnTextChanged="TxtCode_OnTextChanged" onchange="disallowOp('ContentPlaceHolderContent');">
                            </cc1:CustomTextArea>
                        </div>
                        <div class="colHalf2">
                            <div class="colHalf3">
                                <cc1:CustomTextArea ID="txtDescrizioneRuolo" runat="server" CssClass="txt_addressBookRight"
                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                            </div>
                        </div>
                    </div>
                    <uc1:AutoCompleteExtender runat="server" ID="RapidCorr" TargetControlID="txtDescrizioneRuolo"
                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                        UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                        OnClientPopulated="acePopulated">
                    </uc1:AutoCompleteExtender>
                    <asp:Button ID="btnRuolo" runat="server" Text="vai" Style="display: none;" />
                    <asp:Panel ID="PnlUente" runat="server">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="weight">
                                        <asp:Literal ID="ltlUtente" runat="server"></asp:Literal></span>
                                </p>
                            </div>
                            <div class="row">
                                <asp:DropDownList ID="ddlUtente" Enabled="false" Width="50%" runat="server"
                                    CssClass="chzn-select-deselect">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <asp:CheckBox ID="cbxMantieni" runat="server" />
            </div>
        </asp:Panel>
        <div class="row">
            <asp:CheckBox ID="cbxCopiaVisibilita" runat="server" />
        </div>
        <div class="row" style="margin-top: 25px">
            <asp:UpdatePanel ID="UpnlListaProcessiNonCopiati" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="PnlListaProcessiNonCopiati" runat="server" Visible="false">
                        <span class="weight">
                            <asp:Literal ID="LtlProcessiNonCopiati" runat="server" /></span>
                        <asp:Panel ID="pnlReport" runat="server">
                            <asp:GridView ID="grdReport" runat="server" Width="95%" AutoGenerateColumns="False"
                                CssClass="tbl_rounded_custom round_onlyextreme">
                                <RowStyle CssClass="NormalRow" />
                                <AlternatingRowStyle CssClass="AltRow" />
                                <PagerStyle CssClass="recordNavigator2" />
                                <Columns>
                                    <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReportCopiaProcessi%>' DataField="ObjId">
                                        <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Esito" DataField="Result" ItemStyle-HorizontalAlign="Center">
                                        <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Dettagli" DataField="Details" HtmlEncode="false">
                                        <HeaderStyle HorizontalAlign="Center" Width="30%" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="AddNewProcessSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddNewProcessSave_Click" Enabled="false"
                OnClientClick="disallowOp('Content1')" />
            <cc1:CustomButton ID="AddNewProcessClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content1')"
                OnClick="AddNewProcessClose_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
