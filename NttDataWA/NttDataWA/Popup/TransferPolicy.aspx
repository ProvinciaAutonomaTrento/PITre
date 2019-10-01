<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="TransferPolicy.aspx.cs" Inherits="NttDataWA.Popup.TransferPolicy" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ACT" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.collapsed" });
        });

        function creatorePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoCreatore');
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

        function creatoreSelected(sender, e) {
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

            var searchText = $get('<%=TxtUODesc.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtUODesc.ClientID%>").focus();
            document.getElementById("<%=this.TxtUODesc.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.FieldUO.ClientID%>").value = codice;
            document.getElementById("<%=TxtUODesc.ClientID%>").value = descrizione;


            __doPostBack('<%=this.FieldUO.ClientID%>', '');
        }

        function DatePicker2() {
            $('.date-picker').datepicker({

                changeYear: true,
                showButtonPanel: false,
                dateFormat: 'yy',
                buttonImage: '/NttDataWA/Images/Icons/calendar.png',
                showOn: "button",
                buttonImageOnly: true,
                onClose: function (dateText, inst) {
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, 1));
                }
            });
       }

       function resizeTable() {
           if ($('.GrdPolicy').length > 0 && $('.GrdPolicy')[0].offsetTop) {
               var height = document.documentElement.clientHeight;
//               height -= $('.GrdPolicy')[0].offsetTop; // not sure how to get this dynamically

//               height -= 80; /* whatever you set your body bottom margin/padding to be */
//               $('.GrdPolicy')[0].style.height = height + "px";
//               $('.GrdPolicy')[0].rows[0].style.height = "35px";

//               var rowHeight = (height - 35) / ($('.GrdPolicy')[0].rows.length - 1);
               var rowHeight = (height) / ($('.GrdPolicy')[0].rows.length );
               for (var i = 1; i < $('.GrdPolicy')[0].rows.length; i++)
                   $('.GrdPolicy')[0].rows[i].style.height = rowHeight + 'px';

               window.onresize = resizeIframe
           }
       };

    </script>
    <style type="text/css">
        .tbl_rounded_custom tr.SelectedRow td
        {
            background: #f3edc6;
            color: #333333;
        }
        .tbl_rounded_custom tr
        {
            cursor: pointer;
        }
        .ui-datepicker-calendar, .ui-datepicker-month, .ui-datepicker .ui-datepicker-prev, .ui-datepicker .ui-datepicker-next 
        {
            display: none;
        }
        
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="OpenTitolario" runat="server" Url="../Popup/ClassificationScheme.aspx?popup=1"
        IsFullScreen="true" Width="600" Height="400" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlRapidCollation', ''); }" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitScroll="false"
        IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <%--DA MODIFICARE--%>
    <uc:ajaxpopup2 Id="TransferPolicyImpact" runat="server" Url="../Popup/TransferPolicyImpact.aspx"
        IsFullScreen="true" PermitScroll="false" CloseFunction="function (event, ui) {$('#btnTransferPolicyImpactPostback').click();}" />
    <div id="contentAddressBook">
        <div id="topContentPopupSearch">
            <ul>
                <li class="addressTab" id="liAddressBookLinkList" runat="server">
                    <asp:LinkButton runat="server" ID="AddressBookLinkList">
                        <asp:Literal ID="LitTitoloPolicy" runat="server"></asp:Literal></asp:LinkButton>
                </li>
            </ul>
        </div>
        <div id="centerContentAddressbook">
            <div id="contentTab">
                <div id="centerContentAddressbookSx">
                    <asp:UpdatePanel ID="UpPnlPolicyMid" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:PlaceHolder ID="PlcPolicyTop" runat="server">
                                <fieldset class="basic3">
                                    <!-- riga ID VERSAMENTO-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitIdVersamento" runat="server"></asp:Literal></strong></div>
                                        <div class="col">
                                            <NttDL:CustomTextArea ID="TxtIdVersamento" runat="server" CssClass="txt_addressBookLeft"
                                                CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="False" Width="20%" ></NttDL:CustomTextArea>
                                        </div>
                                    </div>
                                    <!-- riga DESCR VERSAMENTO-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitDescVers" runat="server"></asp:Literal></strong></div>
                                        <div class="colHalf_arch">
                                            <NttDL:CustomTextArea ID="TxtDescVers" runat="server" CssClass="txt_input_half" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                Enabled="False" ></NttDL:CustomTextArea>
                                        </div>
                                        <div class="col">
                                        </div>
                                    </div>
                                    <!-- riga ID POLICY-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="litPolicyId" runat="server" /></strong></div>
                                        <div class="col">
                                            <NttDL:CustomTextArea ID="TxtPolicyId" runat="server" CssClass="txt_addressBookLeft"
                                                CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="false" Width="20%" />
                                        </div>
                                    </div>
                                    <!-- riga DESCR POLICY-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitDescPolicy" runat="server" /></strong></div>
                                        <div class="colHalf_arch">
                                            <NttDL:CustomTextArea ID="TxtDescPolicy" runat="server" CssClass="txt_addressBookLeft"
                                                CssClassReadOnly="txt_addressBookLeft_disabled"  />
                                        </div>
                                        <div class="col">
                                        </div>
                                    </div>
                                    <!-- riga DOC/FASC-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitDocFasc" runat="server" /></strong></div>
                                        <div class="colHalf_arch">
                                            <asp:DropDownList ID="ddlDocFasc" runat="server" CssClass="chzn-select-deselect"
                                                OnSelectedIndexChanged="ddlDocFasc_SelectedIndexChanged" AutoPostBack="true" Width="100%">
                                                <asp:ListItem Text='Documenti' Value='1'></asp:ListItem>
                                                <asp:ListItem Text='Fascicoli' Value='2'></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col">
                                        </div>
                                    </div>
                                    <!-- riga  AOO REV G-->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitAOO" runat="server" /></strong></div>
                                        <div class="colHalf_arch">
                                            <asp:DropDownList ID="ddlPolicyRegistri" runat="server" Width="100%" CssClass="chzn-select-deselect"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlPolicyRegistri_SelectedIndexChanged"
                                                 />
                                        </div>
                                        <div class="colHalf4_arch">
                                            <div class="colHalf3">
                                                <NttDL:CustomTextArea ID="TxtAOODesc" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Width="90%" />
                                            </div>
                                        </div>
                                    </div>
                                    <!-- riga  UO REV G-->
                                    <div class="row">
                                        <asp:HiddenField ID="IdProject" runat="server" />
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitUO" runat="server" /></strong></div>
                                        <div class="colHalf_arch">
                                            <asp:DropDownList ID="ddlPolicyUo" runat="server" Width="100%" CssClass="chzn-select-deselect"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlPolicyUO_SelectedIndexChanged"/>
                                        </div>
                                        <div class="colHalf4_arch">
                                            <div class="colHalf3">
                                                <asp:HiddenField id="FieldUO" runat="server" OnValueChanged="FieldUO_onValueChenge"/>
                                                <%--<NttDL:CustomTextArea ID="TxtUO" runat="server"  CssClassReadOnly="txt_addressBookLeft_disabled"
                                                     CssClass="hidden" AutoPostBack="true" OnTextChanged="TxtUO_onTextChenge" />--%>
                                                <NttDL:CustomTextArea ID="TxtUODesc" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Enabled="true" Width="90%" />
                                            </div>
                                        </div>
                                        <ACT:AutoCompleteExtender runat="server" ID="RapidCreatore" TargetControlID="TxtUODesc"
                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                            MinimumPrefixLength="2" CompletionInterval="500" EnableCaching="true" CompletionSetCount="20"
                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                            UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                            OnClientPopulated="creatorePopulated ">
                                        </ACT:AutoCompleteExtender>
                                    </div>
                                    <!-- riga checkbox UO -->
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpPnlIncUO" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="colHalf">
                                                    &nbsp;
                                                </div>
                                                <div class="col">
                                                </div>
                                                <div class="colHalf1">
                                                    <asp:CheckBox ID="chkIncUO" runat="server" OnCheckedChanged="chkIncUO_CheckedChanged"
                                                        AutoPostBack="true" />
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- riga checkbox UO -->
                                    <asp:PlaceHolder ID="PlcPolicyTip" runat="server">
                                        <asp:UpdatePanel ID="UpPnlTip" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <strong>
                                                            <asp:Literal ID="LitPolicyTip" runat="server" Text="" />
                                                        </strong>
                                                    </div>
                                                    <div class="col">
                                                        <asp:CheckBox ID="chkArrivo" runat="server" OnCheckedChanged="chkArrivo_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                    <div class='col'>
                                                        <asp:CheckBox ID="chkPartenza" runat="server" OnCheckedChanged="chkPartenza_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                    <div class='col'>
                                                        <asp:CheckBox ID="chkInt" runat="server" OnCheckedChanged="chkInt_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="colHalf">
                                                        &nbsp;
                                                    </div>
                                                    <div class='colHalf'>
                                                        <asp:CheckBox ID="chkNonProt" runat="server" OnCheckedChanged="chkNonProt_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                    <div class='col'>
                                                        <asp:CheckBox ID="chkStampaRegProt" runat="server" OnCheckedChanged="chkStampaRegProt_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                    <div class='col'>
                                                        <asp:CheckBox ID="chkStampaRep" runat="server" OnCheckedChanged="chkStampaRep_CheckedChanged"
                                                            AutoPostBack="true" />
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                    <!-- riga TIPOLOGIA -->
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpPnlPolicyTipologia" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="plcPolicyTipologia" runat="server">
                                                    <div class="row">
                                                        <div class="colHalf">
                                                            <strong>
                                                                <asp:Literal ID="LitPolicyTipologia" runat="server" /></strong></div>
                                                        <div class="colHalf_arch">
                                                            <asp:DropDownList ID="ddlPolicyTipologia" runat="server" Width="100%" CssClass="chzn-select-deselect"
                                                                AutoPostBack="true" OnSelectedIndexChanged="ddlPolicyTipologia_SelectedIndexChanged"
                                                                 />
                                                        </div>
                                                        <div class="colHalf4">
                                                            <div class="colHalf3">
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- riga TITOLARIO -->
                                    <div class="row">
                                        <asp:UpdatePanel ID="UpPnlPolicyTitolario" runat="server" UpdateMode="Always">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="PlcPolicyTitolatio" runat="server">
                                                    <div class="row">
                                                        <div class="colHalf">
                                                            <strong>
                                                                <asp:Literal ID="LitPolicyTitolario" runat="server" /></strong></div>
                                                        <div class="colHalf_arch">
                                                            <asp:DropDownList ID="ddlPolicyTitolatio" runat="server" Width="100%" CssClass="chzn-select-deselect"
                                                                OnSelectedIndexChanged="ddlPolicyTitolatio_SelectedIndexChanged" AutoPostBack="true"
                                                                 />
                                                        </div>
                                                        <div class="colHalf4_arch">
                                                            <div class="colHalf3">
                                                                <asp:UpdatePanel ID="UpPnlRapidCollation" runat="server" UpdateMode="Conditional"
                                                                    ClientIDMode="Static">
                                                                    <ContentTemplate>
                                                                        <NttDL:CustomImageButton ID="btnclassificationschema" ImageUrl="../Images/Icons/classification_scheme.png"
                                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                                            OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"
                                                                            OnClientClick="return ajaxModalPopupOpenTitolario();" />
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- riga CLASSI -->
                                    <div class="row">
                                        <div class="colHalf">
                                            <strong>
                                                <asp:Literal ID="LitPolicyClassi" runat="server" /></strong></div>
                                        <div class="colHalf_arch">
                                            <NttDL:CustomTextArea ID="txtPolicyClassi" runat="server" CssClass="txt_addressBookLeft"
                                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                Width="100%" OnTextChanged="txtPolicyClassi_OnTextChanged">
                                            </NttDL:CustomTextArea>
                                        </div>
                                        <div class="colHalf4_arch">
                                            <div class="colHalf3">
                                                <NttDL:CustomTextArea ID="txtPolicyClassiDesc" runat="server" CssClass="txt_addressBookLeft"
                                                    CssClassReadOnly="txt_addressBookLeft_disabled" Width="90%">
                                                </NttDL:CustomTextArea>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- riga CHK INCLUDI SOTTOCLASSI -->
                                    <asp:UpdatePanel ID="UpPnlIncludeClass" runat="server" UpdateMode="Conditional" class="row">
                                        <ContentTemplate>
                                            <div class="colHalf">
                                                &nbsp;
                                            </div>
                                            <div class="col">
                                            </div>
                                            <div class="colHalf1">
                                                <asp:CheckBox ID="ChkIncludeClass" runat="server" OnCheckedChanged="chkIncUO_CheckedChanged"
                                                    AutoPostBack="true" />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <!-- riga INTERVALLI -->
                                    <asp:UpdatePanel ID="upPnlIntervals" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <!-- data Creazione -->
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight"><span class="black">
                                                        <asp:Literal ID="LitAnnoCreazione" runat="server" /></span></span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="black">
                                                        <asp:DropDownList ID="ddl_dtaCreate" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_dtaCreate_SelectedIndexChanged"
                                                            CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaCreate_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaCreate_opt1" Value="1" />
                                                        </asp:DropDownList>
                                                    </span>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lbl_dtaCreateFrom" runat="server"></asp:Label></div>
                                                <div class="col2">
                                                    <span class="black">
                                                        <NttDL:CustomTextArea ID="dtaCreate_TxtFrom" runat="server" Width="40%" CssClass="txt_textdata date-picker"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" ClientIDMode="Static"></NttDL:CustomTextArea>
                                                    </span>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lbl_dtaCreateTo" runat="server" Visible="False"></asp:Label>
                                                </div>
                                                <div class="col2">
                                                    <span class="black">
                                                        <NttDL:CustomTextArea ID="dtaCreate_TxtTo" runat="server" CssClass="txt_textdata date-picker"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" Width="40%" Visible="false" ClientIDMode="Static"></NttDL:CustomTextArea>
                                                    </span>
                                                </div>
                                            </div>
                                            <!-- data Protocollo -->
                                            <div class="row">
                                                <div class="col">
                                                    <span class="weight"><span class="black">
                                                        <asp:Literal ID="LitAnnoProtocollazione" runat="server" /></span></span>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="col">
                                                    <span class="black">
                                                        <asp:DropDownList ID="ddl_dtaProt" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_dtaProt_SelectedIndexChanged"
                                                            CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="dtaProt_opt0" Selected="True" Value="0" />
                                                            <asp:ListItem id="dtaProt_opt1" Value="1" />
                                                        </asp:DropDownList>
                                                    </span>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lbl_dtaProtFrom" runat="server"></asp:Label></div>
                                                <div class="col2">
                                                    <span class="black">
                                                        <NttDL:CustomTextArea ID="dtaProt_TxtFrom" runat="server" CssClass="txt_addressBookLeft date-picker"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled"  Width="40%" ClientIDMode="Static"></NttDL:CustomTextArea>
                                                    </span>
                                                </div>
                                                <div class="col2">
                                                    <asp:Label ID="lbl_dtaProtTo" runat="server" Visible="False"></asp:Label>
                                                </div>
                                                <div class="col2">
                                                    <span class="black">
                                                        <NttDL:CustomTextArea ID="dtaProt_TxtTo" runat="server" CssClass="txt_addressBookLeft date-picker"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" Width="40%" Visible="false" ClientIDMode="Static"></NttDL:CustomTextArea>
                                                    </span>
                                                </div>
                                                <br />
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div id="centerContentAddressbookDx">
                    <asp:UpdatePanel ID="UpPnlGridResult" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <%--<div class='row'>
                                <div class='col'>
                                    <asp:Literal ID="LitGridPolicyVuota" runat="server" />
                                </div>
                            </div>--%>
                            <div class='row'>
                                <div class='col_full'>
                                    <asp:GridView ID="GrdPolicy" runat="server" CssClass="tbl_rounded_custom round_onlyextreme GrdPolicy" 
                                        Width="100%"  AutoGenerateColumns="False" AllowPaging="true" PageSize="9" DataKeyNames="Id_policy"
                                        BorderWidth="0" OnPreRender="GrdPolicy_PreRender" OnSelectedIndexChanged="GrdPolicy_SelectedIndexChanged"
                                        OnRowDataBound="GrdPolicy_RowDataBound" OnPageIndexChanging="GrdPolicy_PageIndexChanging" >
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <SelectedRowStyle CssClass="SelectedRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <%-- fine colonna di selezione per la modifica della policy.--%>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="cb_selectall" runat="server"
                                                            AutoPostBack="true" OnCheckedChanged="addAll_Click"  />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="ckbIncludiEscludi" runat="server" AutoPostBack="true" OnCheckedChanged="ckbIncludiEscludi_CheckedChanged"
                                                        ToolTip="Seleziona la policy per la ricerca e l'analisi" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblSystem_id_PolicylabelHeader" Text='ID' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblSystem_id_Policy" runat="server" Text='<%# Bind("Id_policy") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDescription_PolicylabelHeader" Text='Descrizione' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblDescription_Policy" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="30%" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblNumeroDoc_PolicylabelHeader" Text='N.Doc.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblNumeroDocumenti" runat="server" Text='<%# Bind("Totale_documenti") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblNumeroFasc_PolicylabelHeader" Text='N.Fasc.' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblNumeroFascicoli" runat="server" Text='<%# Bind("Totale_fascicoli") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblNEffettivi_PolicylabelHeader" Text='Effettivi' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblNEffettivi" runat="server" Text='<%# Bind("Num_documenti_trasferiti") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblNCopiati_PolicylabelHeader" Text='Copiati' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblNCopiati" runat="server" Text='<%# Bind("Num_documenti_copiati") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblStato_PolicylabelHeader" Text='Stato' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="LblStato" runat="server" Text='<%# Bind("Stato") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="20%"/>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:Label ID="LblDettaglio_PolicylabelHeader" Text='Dettaglio' runat="server"></asp:Label>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <NttDL:CustomImageButton ID="cImgBtnDettaglioPolicy" ImageUrl="../Images/Icons/ico_view_document.png"
                                                        runat="server" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                                        OnMouseOutImage="../Images/Icons/ico_view_document.png" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png"
                                                        CssClass="clickableLeft" Enabled="false" ToolTip='Visualizza in dettaglio la policy'
                                                        OnClick="cImgBtnDettaglioPolicy_Click" Visible="false"/>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="5%" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
    <asp:UpdatePanel ID="upPnlHiddeConfirmDeleteTransferPolicy" UpdateMode="Conditional"
        runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <asp:HiddenField ID="HiddeConfirmDeleteTransferPolicy" ClientIDMode="Static" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPnlHiddeConfirmUpdateTransferPolicy" UpdateMode="Conditional"
        runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <asp:HiddenField ID="HiddeConfirmUpdateTransferPolicy" ClientIDMode="Static" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnPolicyNuovo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyNuovo_Click" />
            <NttDL:CustomButton ID="btnPolicyAvviaRicerca" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnEseguiRicerca_Click" />
            <NttDL:CustomButton ID="btnPolicyAnalizza" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyAnalizza_Click" />
            <NttDL:CustomButton ID="btnPolicyRimuoviFiltro" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="btnPolicyRimuoviFiltro_Click"
                ClientIDMode="Static" />
            <NttDL:CustomButton ID="btnPolicyModifica" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyModifica_Click" />
            <NttDL:CustomButton ID="btnPolicyElimina" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyElimina_Click" />
            <NttDL:CustomButton ID="btnPolicyChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnPolicyChiudi_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect:
true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
