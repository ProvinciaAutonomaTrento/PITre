<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignatureProcesses.aspx.cs"
    Inherits="NttDataWA.Management.SignatureProcesses" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tbl_rounded td {
            vertical-align: top;
        }

        .tbl_rounded tr.nopointer td {
            cursor: default;
        }

        .tbl_rounded tr {
            cursor: pointer;
        }

        .tbl_rounded th {
            white-space: nowrap;
        }

        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 tr, .recordNavigator2 td {
            background-color: #EEEEEE;
        }

            .recordNavigator2 td {
                border: 0;
            }

        .TreeSignatureProcess {
            padding: 0;
        }

            .TreeSignatureProcess td, .TreeSignatureProcess th, .TreeSignatureProcess tr {
                border: 0;
                padding: 0;
                margin: 0;
                height: 20px;
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
            }

            .TreeSignatureProcess table {
                padding: 0;
                margin: 0;
                height: 0;
                border: 0;
            }

            .TreeSignatureProcess img {
                width: 20px;
                height: 20px;
            }

        .TreeSignatureProcess_node a:link, .TreeSignatureProcess_node a:visited, .TreeSignatureProcess_node a:hover {
            padding: 0 5px;
        }

        .TreeSignatureProcess_selected {
            background-color: #477FAF;
        }

            .TreeSignatureProcess_selected a:link, .TreeSignatureProcess_selected a:visited, .TreeSignatureProcess_selected a:hover {
                padding: 0 5px;
                background-color: transparent;
                color: #fff;
            }

        .textInvaledeted fieldset {
            border: 1px solid #FF4500;
            margin-top: 5px;
            margin-bottom: 10px;
            width: 96%;
            border-radius: 8px;
            -ms-border-radius: 8px; /* ie */
            -moz-border-radius: 8px; /* firefox */
            -webkit-border-radius: 8px; /* safari, chrome */
            -o-border-radius: 8px; /* opera */
        }
    </style>
    <script type="text/javascript">
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

            var searchText = $get('<%=TxtDescriptionRole.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeRole.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRole.ClientID%>").value = descrizione;

            document.getElementById("<%=btnRecipient.ClientID%>").click();
            //__doPostBack('UpPnlRecipient', '');
        }

        function enableRoleOrTypeRole() {
            var pnlRole = $('#<%=PnlRole.ClientID%>');
            var pnlTypeRole = $('#<%=PnlTypeRole.ClientID%>');
            var pnlUenteCoinvolto = $('#<%=PnlUenteCoinvolto.ClientID%>');
            var element = $('#optRole').find('input').get(0);
            if (element.checked == true) {
                pnlRole.show();
                pnlUenteCoinvolto.show();
                pnlTypeRole.hide();
            }
            else {
                pnlUenteCoinvolto.hide();
                pnlRole.hide();
                pnlTypeRole.show();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="VisibilitySignatureProcess" runat="server" Url="../popup/VisibilitySignatureProcess.aspx"
        Width="700" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpContainerSignatureProcessesTab', '');}" />
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('UpContainerSignatureProcessesTab', 'closePopupAddressBook');}" />
    <uc:ajaxpopup2 Id="StatisticsSignatureProcess" runat="server" Url="../Popup/StatisticsSignatureProcess.aspx"
        PermitClose="false" Width="1200" Height="800" PermitScroll="false" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddFilterSignatureProcesses" runat="server" Url="../Popup/AddFilterSignatureProcesses.aspx"
        PermitClose="false" Width="600" Height="450" PermitScroll="false" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="AddNewProcess" runat="server" Url="../Popup/AddNewProcess.aspx"
        PermitClose="false" Width="400" Height="300" PermitScroll="false" CloseFunction="function (event, ui){__doPostBack('UpPnlButtons', '');}" />
    <div id="containerTop">
        <div id="containerDocumentTop">
            <div id="containerStandardTop">
                <div id="containerStandardTopSx">
                </div>
                <div id="containerStandardTopCx">
                    <p>
                        <asp:Literal ID="ManagementSignatureProcesses" runat="server"></asp:Literal>
                    </p>
                </div>
                <div id="containerStandardTopDx">
                </div>
            </div>
            <div id="containerStandardBottom">
                <div id="containerProjectCxBottom">
                </div>
            </div>
        </div>
        <asp:UpdatePanel runat="server" ID="UpContainerSignatureProcessesTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerSignatureProcessesTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <ul>
                                <li id="liSignatureProcesses" runat="server" class="mngIAmSignatureProcesses">
                                    <asp:LinkButton ID="linkSignatureProcesses" runat="server"></asp:LinkButton></li>
                            </ul>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpdPlnProcessName" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlProcessName" runat="server" Visible="false">
                                        <div class="row" style="margin-top: 10px">
                                            <div class="col-marginSx2" style="margin-left: 50px;">
                                                <strong>
                                                    <asp:Literal ID="lblNameSignatureProcesses" runat="server" />*</strong>
                                            </div>
                                            <div class="colHalf12">
                                                <cc1:CustomTextArea ID="txtNameSignatureProcesses" runat="server" CssClass="txt_projectRight"
                                                    CssClassReadOnly="txt_ProjectRight_disabled">
                                                </cc1:CustomTextArea>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <asp:Panel ID="pnlFilter" runat="server">
                            <div id="containerNotificationFilter">
                                <asp:UpdatePanel runat="server" ID="UpPnlBAction" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="col11" style="padding-left: 10px">
                                            <cc1:CustomImageButton runat="server" ID="IndexImgAddFilter" ImageUrl="../Images/Icons/home_add_filters.png"
                                                OnMouseOutImage="../Images/Icons/home_add_filters.png" OnMouseOverImage="../Images/Icons/home_add_filters_hover.png"
                                                CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/home_add_filters_disabled.png"
                                                OnClientClick="return ajaxModalPopupAddFilterSignatureProcesses();" />
                                            <cc1:CustomImageButton runat="server" ID="IndexImgRemoveFilter" ImageUrl="../Images/Icons/home_delete_filters.png"
                                                OnMouseOutImage="../Images/Icons/home_delete_filters.png" OnMouseOverImage="../Images/Icons/home_delete_filters_hover.png"
                                                CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/home_delete_filters_disabled.png" OnClick="IndexImgRemoveFilter_Click"
                                                Enabled="false" />
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </asp:Panel>
                        <div class="box_inside" style="margin-top: 30px">
                            <asp:UpdatePanel ID="upPnlTreeSignatureProcess" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:TreeView ID="TreeSignatureProcess" runat="server" ExpandLevel="10" ShowLines="true"
                                        NodeStyle-CssClass="TreeSignatureProcess_node" SelectedNodeStyle-CssClass="TreeSignatureProcess_selected"
                                        OnSelectedNodeChanged="TreeSignatureProcess_SelectedNodeChanged" OnTreeNodeCollapsed="TreeSignatureProcess_Collapsed"
                                        OnTreeNodeExpanded="TreeSignatureProcess_Collapsed" CssClass="TreeSignatureProcess" />
                                    <asp:HiddenField ID="HiddenRemoveSignatureProcess" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="HiddenRemoveStepSignatureProcess" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="HiddenEventWithoutWait" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div class="box_inside">
                            <div id="contentDxTopProject" style="margin-left: 50px; margin-top: 10px; width: 90%">
                                <asp:UpdatePanel ID="UpdPnlStep" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlStep" runat="server" Visible="false">
                                            <fieldset class="basic5" style="min-height: 10px">
                                                <legend style="padding-bottom: 15px">
                                                    <div style="float: left">
                                                        <strong>
                                                            <asp:Label ID="lblSectionDocument" runat="server"></asp:Label></strong>
                                                    </div>
                                                    <div class="col-right">
                                                        <asp:UpdatePanel ID="UplAddStep" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:Panel ID="PnlBtnAddStep" runat="server" Visible="false">
                                                                    <cc1:CustomImageButton ID="btnAddStep" runat="server" CssClass="clickableRight" Visible="true"
                                                                        OnClick="BtnAddStep_Click" ImageUrl="../Images/Icons/add_version.png" OnMouseOutImage="../Images/Icons/add_version.png"
                                                                        OnMouseOverImage="../Images/Icons/add_version_hover.png" ImageUrlDisabled="../Images/Icons/add_version_disabled.png" />
                                                                </asp:Panel>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </div>
                                                </legend>
                                                <asp:UpdatePanel ID="UpdPnlDetailsStep" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                                                    <ContentTemplate>
                                                        <asp:Panel ID="pnlDetailsSteps" runat="server" Visible="false">
                                                            <%-- WARNING STEP INVALIDATED --%>
                                                            <asp:UpdatePanel ID="UplWarningRoleUserDisabled" runat="server" ClientIDMode="Static"
                                                                UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <asp:Panel ID="PnlWarningRoleUserDisabled" runat="server" Visible="false">
                                                                        <%--<uc:messager ID="msgTextInvalidated" runat="server" />--%>
                                                                        <div id="messager" class="messager" runat="server" align="center">
                                                                            <div class="messager_c1">
                                                                                <img src="<%=Page.ResolveClientUrl("~/Images/Common/messager_warning.png") %>" alt="" />
                                                                            </div>
                                                                            <div class="messager_c2">
                                                                                <span>
                                                                                    <asp:Literal runat="server" ID="MessangerWarning"></asp:Literal></span><asp:Literal
                                                                                        ID="msgTextInvalidated" runat="server" />
                                                                            </div>
                                                                            <div class="messager_c3">
                                                                                <img src="<%=Page.ResolveClientUrl("~/Images/Common/messager_warning.png") %>" alt="" />
                                                                            </div>
                                                                        </div>
                                                                    </asp:Panel>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                            <%-- NUMERO DI SEQUENZA --%>
                                                            <div class="row">
                                                                <asp:UpdatePanel ID="UplNrStep" runat="server">
                                                                    <ContentTemplate>
                                                                        <div class="colHalf" style="width: 30%;">
                                                                            <strong>
                                                                                <asp:Literal ID="ltlNr" runat="server" /></strong>
                                                                            <cc1:CustomTextArea ID="txtNr" runat="server" CssClass="txt_input_full onlynumbers"
                                                                                Style="width: 20px; text-align: center" CssClassReadOnly="txt_input_full_disabled">
                                                                            </cc1:CustomTextArea>
                                                                        </div>
                                                                    </ContentTemplate>
                                                                </asp:UpdatePanel>
                                                            </div>
                                                            <div class="row" style="padding-top: 10px">
                                                                <asp:PlaceHolder ID="TypeStep" runat="server">
                                                                    <div class="colHalf14">
                                                                        <strong>
                                                                            <asp:Literal ID="LtlTypeStep" runat="server" /></strong>
                                                                    </div>
                                                                    <div class="colHalf12">
                                                                        <asp:RadioButtonList ID="RblTypeStep" CssClass="rblHorizontal" runat="server" RepeatLayout="UnorderedList"
                                                                            OnSelectedIndexChanged="RblTypeStep_SelectedIndexChanged" AutoPostBack="true">
                                                                            <asp:ListItem id="optSign" Value="F" Selected="True"></asp:ListItem>
                                                                            <asp:ListItem id="optWait" Value="W"> </asp:ListItem>
                                                                            <asp:ListItem id="optEvent" Value="E"> </asp:ListItem>
                                                                        </asp:RadioButtonList>
                                                                    </div>
                                                                </asp:PlaceHolder>
                                                            </div>
                                                            <asp:UpdatePanel ID="UpTypeStep" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                                                <ContentTemplate>
                                                                    <asp:Panel ID="PnlTypeStep" runat="server">
                                                                        <%-- TIPO FIRMA --%>
                                                                        <div class="row" style="padding-top: 10px">
                                                                            <asp:UpdatePanel ID="UpdPnlTypeSignature" UpdateMode="Conditional" runat="server">
                                                                                <ContentTemplate>
                                                                                    <asp:Panel ID="pnlSign" runat="server">
                                                                                        <div class="colHalf14">
                                                                                            <strong>
                                                                                                <asp:Literal ID="ltlSignatureProcessesTypeSignature" runat="server" /></strong>
                                                                                        </div>
                                                                                        <div class="colHalf" style="width: 35%;">
                                                                                            <asp:RadioButtonList ID="rblTypeSignature" CssClass="rblHorizontal" runat="server"
                                                                                                RepeatLayout="UnorderedList" OnSelectedIndexChanged="RblTypeSignature_SelectedIndexChanged"
                                                                                                AutoPostBack="true">
                                                                                            </asp:RadioButtonList>
                                                                                        </div>
                                                                                        <asp:PlaceHolder ID="plcTypeSignatureD" runat="server">
                                                                                            <div class="colHalf" style="width: 25%;">
                                                                                                <strong>
                                                                                                    <asp:Literal ID="ltlSignatureProcessesTypeSignatureD" runat="server" /></strong>
                                                                                            </div>
                                                                                            <div class="colHalf" style="width: 25%;">
                                                                                                <asp:RadioButtonList ID="rblTypeSignatureD" runat="server" CssClass="rblVertical"
                                                                                                    RepeatLayout="UnorderedList">
                                                                                                </asp:RadioButtonList>
                                                                                            </div>
                                                                                        </asp:PlaceHolder>
                                                                                        <asp:PlaceHolder ID="plcTypeSignatureE" runat="server">
                                                                                            <div class="colHalf" style="width: 25%;">
                                                                                                <strong>
                                                                                                    <asp:Literal ID="ltlSignatureProcessesTypeSignatureE" runat="server" /></strong>
                                                                                            </div>
                                                                                            <div class="colHalf" style="width: 25%;">
                                                                                                <asp:RadioButtonList ID="rblTypeSignatureE" runat="server" CssClass="rblVertical"
                                                                                                    RepeatLayout="UnorderedList">
                                                                                                </asp:RadioButtonList>
                                                                                            </div>
                                                                                        </asp:PlaceHolder>
                                                                                    </asp:Panel>
                                                                                </ContentTemplate>
                                                                            </asp:UpdatePanel>
                                                                        </div>
                                                                        <%-- TIPO EVENTO --%>
                                                                        <div class="row" style="padding-top: 10px">
                                                                            <asp:UpdatePanel ID="UpTypeEvent" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                                                                <ContentTemplate>
                                                                                    <asp:Panel ID="PnlTypeEvent" runat="server">
                                                                                        <div class="colHalf14">
                                                                                            <span class="weight">
                                                                                                <asp:Literal ID="LtlTypeEvent" runat="server"></asp:Literal></span>
                                                                                        </div>
                                                                                        <div class="colHalf12">
                                                                                            <asp:DropDownList ID="DdlTypeEvent" OnSelectedIndexChanged="DdlTypeEvent_SelectedIndexChanged"
                                                                                                Width="98%" runat="server" CssClass="chzn-select-deselect" AutoPostBack="true">
                                                                                            </asp:DropDownList>
                                                                                        </div>
                                                                                        <div class="colHalf13">
                                                                                            <asp:CheckBox ID="cbx_automatico" OnCheckedChanged="cbx_automatico_CheckedChanged"
                                                                                                AutoPostBack="true" Text="Automatico" Visible="false" runat="server" />
                                                                                        </div>
                                                                                    </asp:Panel>
                                                                                </ContentTemplate>
                                                                            </asp:UpdatePanel>
                                                                        </div>
                                                                        <asp:UpdatePanel ID="UpdPnlRoleOrTypeRole" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:Panel ID="PnlRoleOrTypeRole" runat="server" Visible="False">
                                                                                    <asp:RadioButtonList ID="RblRoleOrTypeRole" CssClass="rblHorizontal" runat="server" RepeatLayout="UnorderedList"
                                                                                        AutoPostBack="true" OnSelectedIndexChanged="RblRoleOrTypeRole_SelectedIndexChanged">
                                                                                        <asp:ListItem id="optRole" Value="R" Selected="True"></asp:ListItem>
                                                                                        <asp:ListItem id="optTypeRole" Value="TR"> </asp:ListItem>
                                                                                    </asp:RadioButtonList>
                                                                                </asp:Panel>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                        <%-- TIPO RUOLO --%>
                                                                        <asp:UpdatePanel ID="UpdPnlTypeRole" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:Panel ID="PnlTypeRole" runat="server">
                                                                                    <div class="row" style="padding-top: 10px; width: 100%">
                                                                                        <div class="col" style="margin-right: 15px">
                                                                                            <span class="weight">
                                                                                                <asp:Literal ID="LtlTypeRole" runat="server"></asp:Literal></span>
                                                                                        </div>
                                                                                        <div class="colHalf" style="width: 80%">
                                                                                            <asp:DropDownList ID="DdlTypeRole" Width="250px" runat="server"
                                                                                                CssClass="chzn-select-deselect">
                                                                                            </asp:DropDownList>
                                                                                        </div>
                                                                                    </div>
                                                                                </asp:Panel>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                        <%-- RUOLO COINVOLTO --%>
                                                                        <asp:UpdatePanel ID="UpdPnlRole" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:Panel ID="PnlRole" runat="server">
                                                                                    <div class="row">
                                                                                        <div class="col">
                                                                                            <p>
                                                                                                <span class="weight">
                                                                                                    <asp:Literal ID="LitSignatureProcessesRole" runat="server" /></strong>
                                                                                            </p>
                                                                                        </div>
                                                                                        <div class="col-right-no-margin">
                                                                                            <cc1:CustomImageButton runat="server" ID="DocumentImgSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                                                                OnClick="BtnAddressBook_Click" />
                                                                                        </div>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <asp:HiddenField ID="idRuolo" runat="server" />
                                                                                        <div class="colHalf">
                                                                                            <cc1:CustomTextArea ID="TxtCodeRole" runat="server" CssClass="txt_addressBookLeft"
                                                                                                autocomplete="off" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                                                                AutoCompleteType="Disabled" OnTextChanged="TxtCode_OnTextChanged">
                                                                                            </cc1:CustomTextArea>
                                                                                        </div>
                                                                                        <div class="colHalf2">
                                                                                            <div class="colHalf3">
                                                                                                <cc1:CustomTextArea ID="TxtDescriptionRole" runat="server" CssClass="txt_addressBookRight"
                                                                                                    CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off" AutoCompleteType="Disabled"></cc1:CustomTextArea>
                                                                                            </div>
                                                                                        </div>
                                                                                    </div>
                                                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidRole" TargetControlID="TxtDescriptionRole"
                                                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                                                                                        MinimumPrefixLength="3" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                                                        UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                                                        OnClientPopulated="acePopulated">
                                                                                    </uc1:AutoCompleteExtender>
                                                                                    <asp:Button ID="btnRecipient" runat="server" Text="vai" Style="display: none;" />
                                                                                </asp:Panel>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                        <%-- UTENTE COINVOLTO --%>
                                                                        <asp:UpdatePanel ID="UpdPnlUtenteCoinvolto" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:Panel ID="PnlUenteCoinvolto" runat="server">
                                                                                    <div class="row">
                                                                                        <div class="col">
                                                                                            <p>
                                                                                                <span class="weight">
                                                                                                    <asp:Literal ID="ltlUtenteCoinvolto" runat="server"></asp:Literal></span>
                                                                                            </p>
                                                                                        </div>
                                                                                        <div class="row">
                                                                                            <asp:DropDownList ID="ddlUtenteCoinvolto" Enabled="false" Width="50%" runat="server"
                                                                                                CssClass="chzn-select-deselect">
                                                                                            </asp:DropDownList>
                                                                                        </div>
                                                                                    </div>
                                                                                </asp:Panel>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                        <%-- CAMPI PASSO AUTOMATICO --%>
                                                                        <asp:UpdatePanel ID="UpPnlCampiPassoAutomatco" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <asp:Panel ID="PnlCampiPassoAutomatco" runat="server" Visible="false">
                                                                                    <%-- AOO --%>
                                                                                    <div class="colHalf15">
                                                                                        <div class="row">
                                                                                            <div class="col">
                                                                                                <p>
                                                                                                    <span class="weight">
                                                                                                        <asp:Literal ID="ltlRegistroAOO" runat="server" /></span>
                                                                                                </p>
                                                                                            </div>
                                                                                        </div>
                                                                                        <div class="row">
                                                                                            <asp:DropDownList ID="DdlRegistroAOO" runat="server" Width="100%" AutoPostBack="true"
                                                                                                CssClass="chzn-select-deselect" OnSelectedIndexChanged="DdlRegistroAOO_SelectedIndexChanged">
                                                                                            </asp:DropDownList>
                                                                                        </div>
                                                                                    </div>
                                                                                    <%-- RF --%>
                                                                                    <div class="colHalf16">
                                                                                        <asp:UpdatePanel ID="UpPnlRegistroRF" UpdateMode="Conditional" runat="server">
                                                                                            <ContentTemplate>
                                                                                                <div class="row">
                                                                                                    <div class="col">
                                                                                                        <p>
                                                                                                            <span class="weight">
                                                                                                                <asp:Literal ID="ltlRegistroRF" runat="server" /></span>
                                                                                                        </p>
                                                                                                    </div>
                                                                                                </div>
                                                                                                <div class="row">
                                                                                                    <asp:DropDownList ID="DdlRegistroRF" runat="server" Width="100%" AutoPostBack ="true"
                                                                                                        CssClass="chzn-select-deselect" OnSelectedIndexChanged ="DdlRegistroRF_SelectedIndexChanged">
                                                                                                    </asp:DropDownList>
                                                                                                </div>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </div>
                                                                                    <%-- ELENCO CASELLE --%>
                                                                                    <div class="colHalf16">
                                                                                        <asp:UpdatePanel ID="UpPnlElencoCaselle" runat="server" UpdateMode="Conditional">
                                                                                            <ContentTemplate>
                                                                                                <asp:Panel ID="PnlElencoCaselle" runat="server" Visible="false">
                                                                                                    <%-- ELENCO CASELLE --%>
                                                                                                    <div class="row">
                                                                                                        <div class="col">
                                                                                                            <p>
                                                                                                                <span class="weight">
                                                                                                                    <asp:Literal ID="ltlElencoCaselle" runat="server" /></span>
                                                                                                            </p>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                    <div class="row">
                                                                                                        <asp:DropDownList ID="DdlElencoCaselle" Width="100%" runat="server"
                                                                                                            CssClass="chzn-select-deselect">
                                                                                                        </asp:DropDownList>
                                                                                                    </div>
                                                                                                </asp:Panel>
                                                                                            </ContentTemplate>
                                                                                        </asp:UpdatePanel>
                                                                                    </div>
                                                                                </asp:Panel>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                        <%-- NOTE --%>
                                                                        <div class="row" style="padding-top: 10px">
                                                                            <div class="col" style="width: 90%">
                                                                                <asp:UpdatePanel ID="UpdPnlNotes" UpdateMode="Conditional" runat="server">
                                                                                    <ContentTemplate>
                                                                                        <asp:Panel ID="pnlNotes" runat="server">
                                                                                            <div class="row">
                                                                                                <div class="col" style="width: 5%">
                                                                                                    <span class="weight">
                                                                                                        <asp:Literal ID="ltlNotes" runat="server"></asp:Literal>
                                                                                                    </span>
                                                                                                </div>
                                                                                                <div class="colHalf" style="width: 85%">
                                                                                                    <div>
                                                                                                        <cc1:CustomTextArea ID="txtNotes" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                                                                            CssClassReadOnly="txt_textarea_disabled" ClientIDMode="Static">
                                                                                                        </cc1:CustomTextArea>
                                                                                                    </div>
                                                                                                    <div class="row">
                                                                                                        <div class="col-right-no-margin">
                                                                                                            <span class="charactersAvailable">
                                                                                                                <asp:Literal ID="ltrNotes" runat="server" ClientIDMode="Static"> </asp:Literal>
                                                                                                                <span id="txtNotes_chars" clientidmode="Static" runat="server"></span></span>
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:Panel>
                                                                                    </ContentTemplate>
                                                                                </asp:UpdatePanel>
                                                                            </div>
                                                                        </div>
                                                                        <%-- OPZIONI DI NOTIFICA --%>
                                                                        <asp:UpdatePanel ID="UpdPnlNotififyOption" UpdateMode="Conditional" runat="server">
                                                                            <ContentTemplate>
                                                                                <div class="row" style="width: 100%">
                                                                                    <div class="col">
                                                                                        <span class="weight">
                                                                                            <asp:Literal ID="ltlOptionNotify" runat="server"></asp:Literal>
                                                                                        </span>
                                                                                    </div>
                                                                                    <div class="row">
                                                                                        <div class="col">
                                                                                            <asp:CheckBoxList ID="cbxOptionNotify" runat="server" RepeatDirection="Vertical">
                                                                                            </asp:CheckBoxList>
                                                                                        </div>
                                                                                    </div>
                                                                                </div>
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </asp:Panel>
                                                                    <asp:UpdatePanel ID="updPnlAddModifyStep" UpdateMode="Conditional" runat="server">
                                                                        <ContentTemplate>
                                                                            <div class="row">
                                                                                <div class="col-right-no-margin2">
                                                                                    <cc1:CustomImageButton ID="BtnConfirmAddStep" runat="server" CssClass="clickable"
                                                                                        OnClick="BtnConfirmAddStep_Click" ImageUrl="../Images/Icons/save_grid.png" OnMouseOutImage="../Images/Icons/save_grid.png"
                                                                                        OnMouseOverImage="../Images/Icons/save_grid_hover.png" ImageUrlDisabled="../Images/Icons/save_grid_disabled.png" />
                                                                                    <cc1:CustomImageButton ID="BtnDeleteStep" runat="server" CssClass="clickable" Width="20px"
                                                                                        OnClick="BtnDeleteStep_Click" ImageUrl="../Images/Icons/delete3.png" OnMouseOutImage="../Images/Icons/delete3.png"
                                                                                        OnMouseOverImage="../Images/Icons/delete3_hover.png" ImageUrlDisabled="../Images/Icons/delete3_disabled.png" />
                                                                                </div>
                                                                            </div>
                                                                        </ContentTemplate>
                                                                    </asp:UpdatePanel>
                                                                </ContentTemplate>
                                                            </asp:UpdatePanel>
                                                        </asp:Panel>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="SignatureProcessesBtnNew" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SignatureProcessesBtnNew_Click" />
            <cc1:CustomButton ID="SignatureProcessesBtnSave" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SignatureProcessesBtnSave_Click" />
            <cc1:CustomButton ID="SignatureProcessesBtnVisibility" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SignatureProcessesBtnVisibility_Click" />
            <cc1:CustomButton ID="SignatureProcessesStatistics" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SignatureProcessesStatistics_Click" />
            <cc1:CustomButton ID="SignatureProcessesBtnRemove" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="SignatureProcessesBtnRemove_Click" />
            <cc1:CustomButton ID="SignatureProcessesBtnDuplica" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClientClick="return ajaxModalPopupAddNewProcess();" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">        $(".chzn-select-deselect").chosen({
            allow_single_deselect:
            true, no_results_text: "Nessun risultato trovato"
        }); $(".chzn-select").chosen({
            no_results_text: "Nessun risultato trovato"
        }); </script>
</asp:Content>
