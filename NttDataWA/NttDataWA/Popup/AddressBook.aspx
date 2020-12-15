<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddressBook.aspx.cs" Inherits="NttDataWA.Popup.AddressBook"
    MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/jquery.ui.multidraggable-1.8.8.js" type="text/javascript"></script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .ui-multidraggable
        {
            font-weight: bold;
            color: #0B5588;
        }
        .tbl_rounded_custom tr td
        {
            padding: 5px;
            min-height: 20px;
        }
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #eee;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
    </style>
    <script type="text/javascript">
    function resizeTable() {
        var w = $(document).width() * 0.45;
        $('.tbl_rounded_custom').width(w);
        $('.tbl_rounded_custom tr th:nth-child(3)').width(w*0.33);
        $('.tbl_rounded_custom tr td:nth-child(3)').width(w*0.33);
        $('.tbl_rounded_custom tr td:nth-child(3) div').width(w*0.33);
        $('.tbl_rounded_custom tr th:nth-child(4)').width(w*0.18);
        $('.tbl_rounded_custom tr td:nth-child(4)').width(w*0.18);
        $('.tbl_rounded_custom tr td:nth-child(4) div').width(w*0.18);
        $('.tbl_rounded_custom tr th:nth-child(5)').width(w*0.13);
        $('.tbl_rounded_custom tr td:nth-child(5)').width(w*0.13);
        $('.tbl_rounded_custom tr td:nth-child(5) div').width(w*0.13);
        $('.recordNavigator2 td').width('auto');
    }

    function InitDragNDrop() {
        if (<%=this.multipleSelection.ToString().ToLower() %>) {
            $("#<%=this.GrdAddressBookResult.ClientID%> span.draggable").multidraggable({
                revert: function() {
                    return $('.hoverClass').length==0;
                }
                , zIndex: '2500', helper: 'clone', appendTo: 'body'
            });
            $("#PnlTreeDx span.draggable").multidraggable({
                revert: function() {
                    return $('.hoverClass').length==0;
                }
                , zIndex: '2500', helper: 'clone', appendTo: 'body'
            });
        }
        else {
            $("#<%=this.GrdAddressBookResult.ClientID%> span.draggable").draggable({ revert: 'invalid', zIndex: '2500', helper: 'clone', appendTo: 'body' });
            $("#PnlTreeDx span.draggable").draggable({ revert: 'invalid', zIndex: '2500', helper: 'clone', appendTo: 'body' });
        }

        $("#<%=this.GrdAtSelection.ClientID %>").droppable({
            tolerance: 'pointer',
            over: function(event, ui) {
                $('.ui-draggable-dragging').addClass('hoverClass');
            },
            out: function(event, ui) {
                $('.ui-draggable-dragging').removeClass('hoverClass');
            },
            drop: function (event, ui) {
                if (<%=this.multipleSelection.ToString().ToLower() %> && $('.ui-multidraggable').length>0) {
                    var cns = $("#<%=this.hdnMultipleAtCorrespondents.ClientID%>").val();

                    $('.ui-multidraggable').each(function(index, obj) {
                        var cn = obj.className;
                        cn = cn.substring(cn.indexOf("SystemID_"));
                        cn = cn.substring(0, cn.indexOf(" "));
                        
                        if (cns.indexOf(cn)<0) {
                            if (cns!='') cns += '|';
                            cns += cn;
                        }

                        if (obj.className.indexOf('treeview')>=0)
                            $("#<%=this.hdnCallingFromTreeview.ClientID%>").val('true');
                    });

                    if ($("#<%=this.hdnMultipleAtCorrespondents.ClientID%>").val().length==0)
                        setTimeout(function(){$("#<%=this.btnInsertMultipleAtCorrespondents.ClientID%>").click();}, 300);
                    $("#<%=this.hdnMultipleAtCorrespondents.ClientID%>").val(cns);
                }
                else {
                    var cn = ui.draggable[0].className;
                    cn = cn.substring(cn.indexOf("SystemID_"));
                    cn = cn.substring(0, cn.indexOf(" "));

                    if (ui.draggable[0].className.indexOf('treeview')<0) {
                        $("#<%=this.hdnMultipleAtCorrespondents.ClientID%>").val(cn);
                        $("#<%=this.btnInsertMultipleAtCorrespondents.ClientID%>").click();
                    }
                    else {
                        $(".TreeView_"+cn.replace('SystemID_', '')).get(0).click();
                    }
                }
            }
        });

        $("#<%=this.GrdCctSelection.ClientID %>").droppable({
            tolerance: 'pointer',
            over: function(event, ui) {
                $('.ui-draggable-dragging').addClass('hoverClass');
            },
            out: function(event, ui) {
                $('.ui-draggable-dragging').removeClass('hoverClass');
            },
            drop: function (event, ui) {
                if (<%=this.multipleSelection.ToString().ToLower() %> && $('.ui-multidraggable').length>0) {
                    var cns = $("#<%=this.hdnMultipleCcCorrespondents.ClientID%>").val();

                    $('.ui-multidraggable').each(function(index, obj) {
                        var cn = obj.className;
                        cn = cn.substring(cn.indexOf("SystemID_"));
                        cn = cn.substring(0, cn.indexOf(" "));
                        
                        if (cns.indexOf(cn)<0) {
                            if (cns!='') cns += '|';
                            cns += cn;
                        }

                        if (obj.className.indexOf('treeview')>=0)
                            $("#<%=this.hdnCallingFromTreeview.ClientID%>").val('true');
                    });
                    if ($("#<%=this.hdnMultipleCcCorrespondents.ClientID%>").val().length==0)
                        setTimeout(function(){$("#<%=this.btnInsertMultipleCcCorrespondents.ClientID%>").click();}, 300);
                    $("#<%=this.hdnMultipleCcCorrespondents.ClientID%>").val(cns);
                }
                else {
                    var cn = ui.draggable[0].className;
                    cn = cn.substring(cn.indexOf("SystemID_"));
                    cn = cn.substring(0, cn.indexOf(" "));

                    if (ui.draggable[0].className.indexOf('treeview')<0) {
                        $("#<%=this.hdnMultipleCcCorrespondents.ClientID%>").val(cn);
                        $("#<%=this.btnInsertMultipleCcCorrespondents.ClientID%>").click();
                    }
                    else {
                        $(".TreeViewCC_"+cn.replace('SystemID_', '')).click();
                    }
                }
            }
        });
    };

    $(function() {
        $('.defaultAction').keypress(function(e) {
            if(e.which == 13) {
                e.preventDefault();
                $('#AddressBookSearch').click();
            }
        });
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook_New" runat="server" Url="../Popup/AddressBook_New.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="680" Height="700"
        CloseFunction="function (event, ui) {$('#BtnHidden').click();}" />
    <uc:ajaxpopup2 Id="AddressBook_Details" runat="server" Url="../Popup/AddressBook_Details.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="680" Height="700"
        CloseFunction="function (event, ui) {$('#BtnHidden').click();}" />
    <uc:ajaxpopup2 Id="ExportDati" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=rubrica"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" />
    <uc:ajaxpopup2 Id="ExportSearch" runat="server" Url="../ExportDati/exportDatiSelection.aspx?export=searchAddressBook"
        Width="650" Height="500" PermitClose="true" PermitScroll="true" />
    <uc:ajaxpopup2 Id="ImportDati" runat="server" Url="AddressBook_import.aspx" Width="550"
        Height="400" PermitClose="true" PermitScroll="true" />
    <div id="containerAddressBook">
        <asp:Label CssClass="NormalBold" ID="lbldebug_CallType" runat="server" Visible="false"></asp:Label>
        <asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="UpdatePanelTop">
            <ContentTemplate>
                <div class="boxFilterAddressBookType">
                    <div class="boxFilterAddressBookTypeSx">
                        <fieldset class="filterAddressbook">
                            <div class="colTitle">
                                <p>
                                    <asp:Label CssClass="NormalBold" ID="AddressBookTitleType" runat="server"></asp:Label><br />
                                </p>
                            </div>
                            <div class="col">
                                <asp:CheckBoxList runat="server" ID="ChkListType" RepeatDirection="Horizontal" CssClass="rightBorderAddressBook">
                                    <asp:ListItem id="AddressBookChkOfficies" runat="server" Value="U"></asp:ListItem>
                                    <asp:ListItem id="AddressBookChkRoles" Value="R" runat="server"></asp:ListItem>
                                    <asp:ListItem id="AddressBookChkUsers" Value="P" runat="server"></asp:ListItem>
                                    <asp:ListItem id="AddressBookChkLists" Value="L" runat="server"></asp:ListItem>
                                    <asp:ListItem id="AddressBookChkRF" Value="F" runat="server"></asp:ListItem>
                                </asp:CheckBoxList>
                            </div>
                            <div class="col">
                                <asp:UpdatePanel ID="UpdatePanelTypeCorrespondent" runat="server">
                                    <ContentTemplate>
                                        <asp:RadioButtonList runat="server" ID="RblTypeCorrespondent" RepeatDirection="Horizontal"
                                            CssClass="rightBorderAddressBook" OnSelectedIndexChanged="refreshSelectedCorrType"
                                            AutoPostBack="true">
                                            <asp:ListItem runat="server" id="AddressBookRadioTypeAll" Value="IE"></asp:ListItem>
                                            <asp:ListItem runat="server" id="AddressBookRadioExternal" Value="E"></asp:ListItem>
                                            <asp:ListItem runat="server" id="AddressBookRadioInternal" Value="I"></asp:ListItem>
                                        </asp:RadioButtonList>
                                        <asp:HiddenField ID="hdnOldSelectedCorrType" runat="server" Value="" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="col3">
                                <asp:CheckBox runat="server" ID="AddressBookChkCommonAddressBook" CssClass="defaultAction"/>
                            </div>
                        </fieldset>
                    </div>
                    <div class="boxFilterAddressbookTypeDxDx">
                        <asp:UpdatePanel ID="UpdPnlBtnInitialize" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cc1:CustomImageButton runat="server" ID="AddressBookBtnInitialize" ImageUrl="../Images/Icons/icon_refresh.png"
                                    OnMouseOutImage="../Images/Icons/icon_refresh.png" OnMouseOverImage="../Images/Icons/icon_refresh_hover.png"
                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/icon_refresh_disabled.png"
                                    OnClick="AddressBookBtnInitialize_Click" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div class="boxFilterAddressBookType">
                    <fieldset class="filterAddressbook">
                        <div class="filterAddressbookBox">
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLitAddress" runat="server"></asp:Label>
                                    </p>
                                </div>
                                <div class="colAddDx">
                                    <asp:DropDownList CssClass="chzn-select-deselect defaultAction" ID="AddressBookDDLTypeAOO"
                                        runat="server" Width="130">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblCode" runat="server"></asp:Label></p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtCode" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                        </div>
                        <div class="filterAddressbookBox">
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblDescription" runat="server"></asp:Label></p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtDescription" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblCity" runat="server"></asp:Label></p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtCity" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                        </div>
                        <div class="filterAddressbookBox">
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblCountry" runat="server"></asp:Label>
                                    </p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtCountry" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblMail" runat="server"></asp:Label>
                                    </p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtMail" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                        </div>
                        <div class="filterAddressbookBoxLast">
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblNIN" runat="server"></asp:Label></p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtNIN" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                            <div class="row">
                                <div class="colAddSx">
                                    <p>
                                        <asp:Label CssClass="NormalBold" ID="AddressBookLblPIVA" runat="server"></asp:Label></p>
                                </div>
                                <div class="colAddDx">
                                    <cc1:CustomTextArea runat="server" ID="TxtPIVA" CssClass="txt_addressFilter defaultAction"></cc1:CustomTextArea></div>
                            </div>
                        </div>
                    </fieldset>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="contentAddressBook">
            <div id="topContentAddressBook">
                <asp:UpdatePanel ID="UpTypeResult" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <ul>
                            <li class="addressTab" id="liAddressBookLinkList" runat="server">
                                <asp:LinkButton runat="server" ID="AddressBookLinkList" OnClick="AddressBookLinkList_Click"></asp:LinkButton></li>
                            <li class="otherAddressTab" id="liAddressBookLinkOrg" runat="server">
                                <asp:LinkButton runat="server" ID="AddressBookLinkOrg" OnClick="AddressBookLinkOrg_Click"
                                    OnClientClick="disallowOp('Content2')"></asp:LinkButton></li>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div id="centerContentAddressbook">
                <div id="centerContentAddressbookSx">
                    <div class="marginLeft">
                        <asp:UpdatePanel ID="UpdatePanelDxContent" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:UpdatePanel ID="UpPnlGridResult" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <p>
                                            <asp:Literal runat="server" Text="0 Elementi trovati" ID="LitAddressBookResult"></asp:Literal></p>
                                        <asp:GridView ID="GrdAddressBookResult" runat="server" AutoGenerateColumns="False" Width="90%"
                                            AllowPaging="True" CssClass="tbl_rounded_custom round_onlyextreme" PageSize="9"
                                            OnPageIndexChanging="changPageGrdFound_click" BorderWidth="0" OnRowDataBound="AddressBook_OnRowCreated">
                                            <RowStyle CssClass="NormalRow" />
                                            <AlternatingRowStyle CssClass="AltRow" />
                                            <PagerStyle CssClass="recordNavigator2" />
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgDetail" ImageUrl="../Images/Icons/ico_view_document.png"
                                                            OnMouseOutImage="../Images/Icons/ico_view_document.png" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                                            CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png"
                                                            RowIndex='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' CodiceRubrica='<%# DataBinder.Eval(Container, "DataItem.CodiceRubrica") %>'
                                                            EnableViewState="true" AlternateText='<%# this.GetLabelDetails() %>' ToolTip='<%# this.GetLabelDetails() %>'
                                                            OnClick="imgDetail_Click" OnClientClick="disallowOp('Content2');" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="20" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgType" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.ImgTipo") %>'
                                                            OnMouseOutImage='<%# DataBinder.Eval(Container, "DataItem.ImgTipo") %>' OnMouseOverImage='<%# DataBinder.Eval(Container, "DataItem.ImgTipo") %>'
                                                            CssClass='clickableRight' ImageUrlDisabled='<%# DataBinder.Eval(Container, "DataItem.ImgTipo") %>'
                                                            RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' RowIndex='<%# Container.DisplayIndex %>'
                                                            Enabled='<%# this.GetEnableImg((string)DataBinder.Eval(Container, "DataItem.ImgTipo")) %>'
                                                            AlternateText='<%# this.GetLabelDetailsImgType()%>' ToolTip='<%# this.GetLabelDetailsImgType()%>'
                                                            OnClick="viewInTreeView_Click" OnClientClick="disallowOp('Content2');" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="20" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Panel id="div1" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Descrizione") %>'>
                                                            <span class='clickable SystemID_<%# DataBinder.Eval(Container, "DataItem.SystemID").ToString()+" "+ spanEnable(bool.Parse(DataBinder.Eval(Container, "DataItem.Enabled").ToString())) %> rowindex<%# Container.DataItemIndex %>'>
                                                                <asp:Literal ID="litDescription" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Literal></span>
                                                        </asp:Panel>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Panel id="div2" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'>
                                                            <asp:Literal ID="litCanalPref" runat="server" Text='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'></asp:Literal>
                                                        </asp:Panel>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Panel id="div3" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Rubrica") %>'>
                                                            <asp:Literal ID="litBookType" runat="server" Text='<%# Bind("Rubrica") %>'></asp:Literal>
                                                        </asp:Panel>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgA_All" ImageUrl='<%# this.GetImgDetailsAALL() %>'
                                                            OnMouseOutImage='<%# this.GetImgDetailsAALL() %>' OnMouseOverImage='<%# this.GetImgDetailsAALL() %>'
                                                            CssClass="clickable" ImageUrlDisabled='<%# this.GetImgDetailsAALL() %>' OnClick="addAllAt_Click"
                                                            AlternateText='<%# this.GetLabelDetailsAddAll() %>' ToolTip='<%# this.GetLabelDetailsAddAll() %>'
                                                            OnClientClick="disallowOp('Content2');" />
                                                    </HeaderTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgA" ImageUrl='<%# this.GetImgDetailsAddA((string)DataBinder.Eval(Container.DataItem, "At")) %>'
                                                            OnMouseOutImage='<%# this.GetImgDetailsAddA((string)DataBinder.Eval(Container.DataItem, "At")) %>'
                                                            OnMouseOverImage='<%# this.GetImgDetailsAddAHover((string)DataBinder.Eval(Container.DataItem, "At")) %>'
                                                            CssClass='<%# "clickable SystemID_"+DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                            ImageUrlDisabled="../Images/Icons/addressBook_supA.png" RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                            AlternateText='<%# this.GetLabelDetailsAddA((string)DataBinder.Eval(Container.DataItem, "At")) %>'
                                                            ToolTip='<%# this.GetLabelDetailsAddA((string)DataBinder.Eval(Container.DataItem, "At")) %>'
                                                            RowIndex='<%# Container.DisplayIndex %>' OnClick="addAtCorrespondent_Click"
                                                            OnClientClick="disallowOp('Content2');" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="20" />
                                                </asp:TemplateField>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgCc_All" ImageUrl="../Images/Icons/addressBook_supCC.png"
                                                            OnMouseOutImage="../Images/Icons/addressBook_supCC.png" OnMouseOverImage="../Images/Icons/addressBook_supCC.png"
                                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/addressBook_supCC.png"
                                                            OnClick="addAllCc_Click" AlternateText='<%# this.GetLabelDetailsAddAll() %>'
                                                            ToolTip='<%# this.GetLabelDetailsAddAll() %>' OnClientClick="disallowOp('Content2');" />
                                                    </HeaderTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <cc1:CustomImageButton runat="server" ID="imgCc" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.Cc") %>'
                                                            OnMouseOutImage='<%# DataBinder.Eval(Container, "DataItem.Cc") %>' OnMouseOverImage="../Images/Icons/addressBook_bg_btn_cc_click.png"
                                                            CssClass='clickable' ImageUrlDisabled="../Images/Icons/addressBook_supCC.png"
                                                            RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' RowIndex='<%# Container.DisplayIndex %>'
                                                            AlternateText='<%# this.GetLabelDetailsAddCC((string)DataBinder.Eval(Container.DataItem, "Cc")) %>'
                                                            ToolTip='<%# this.GetLabelDetailsAddCC((string)DataBinder.Eval(Container.DataItem, "Cc")) %>'
                                                            OnClick="addCcCorrespondent_Click" Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>'
                                                            OnClientClick="disallowOp('Content2');" Enabled='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" Width="20" />
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="false">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="trasmId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:UpdatePanel ID="UpPnlTreeResult" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="PnlTreeView" Visible="false">
                                            <div id="PnlTreeTitle">
                                                <h2>
                                                    <asp:Literal runat="server" ID="AddressBookOrgTitle"></asp:Literal>
                                                </h2>
                                            </div>
                                            <div id="PnlTree">
                                                <div id="PnlTreeSx">
                                                    <asp:GridView ID="grdAtCcTreeNode" runat="server" Width="100%" AutoGenerateColumns="False"
                                                        AllowPaging="false" CssClass="tableOrgLeftImg" BorderWidth="0">
                                                        <RowStyle CssClass="NormalRow" />
                                                        <AlternatingRowStyle CssClass="AltRow" />
                                                        <PagerStyle CssClass="recordNavigator2" />
                                                        <Columns>
                                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <cc1:CustomImageButton runat="server" ID="imgA" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.At") %>'
                                                                        OnMouseOutImage='<%# DataBinder.Eval(Container, "DataItem.At") %>' OnMouseOverImage="../Images/Icons/addressBook_bg_btn_a_click.png"
                                                                        CssClass='<%# "clickable SystemID_"+DataBinder.Eval(Container, "DataItem.SystemID")+" TreeView_"+DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                                        ImageUrlDisabled="../Images/Icons/addressBook_supA.png" RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                                        RowIndex='<%# Container.DisplayIndex %>' OnClick="addAtNodeCorrespondent_Click"
                                                                        Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' OnClientClick="disallowOp('Content2');"
                                                                        Enabled='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" />
                                                                <ControlStyle Width="20px" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <cc1:CustomImageButton runat="server" ID="imgCc" ImageUrl='<%# DataBinder.Eval(Container, "DataItem.Cc") %>'
                                                                        OnMouseOutImage='<%# DataBinder.Eval(Container, "DataItem.Cc") %>' OnMouseOverImage="../Images/Icons/addressBook_bg_btn_cc_click.png"
                                                                        CssClass='<%# "clickable SystemID_"+DataBinder.Eval(Container, "DataItem.SystemID")+" TreeViewCC_"+DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                                        ImageUrlDisabled="../Images/Icons/addressBook_supCC.png" RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                                        RowIndex='<%# Container.DisplayIndex %>' OnClick="addCcNodeCorrespondent_Click"
                                                                        Visible='<%# DataBinder.Eval(Container, "DataItem.Enabled") %>' OnClientClick="disallowOp('Content2');" />
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" />
                                                                <ControlStyle Width="20px" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="trasmId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <div id="PnlTreeDx">
                                                    <cc1:OrganizationChartTreeView ID="AddressBookTreeView" OnTreeNodeExpanded="ExpandeTreeView"
                                                        OnTreeNodeCollapsed="CollapseTreeView" runat="server" CssClass="TreeAddressBook2" BorderWidth="0">
                                                    </cc1:OrganizationChartTreeView>
                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <asp:Button ID="btnInsertMultipleAtCorrespondents" runat="server" CssClass="hidden"
                                    OnClick="btnInsertMultipleAtCorrespondents_Click" OnClientClick="
                                    ('Content2');" />
                                <asp:HiddenField ID="hdnMultipleAtCorrespondents" runat="server" />
                                <asp:Button ID="btnInsertMultipleCcCorrespondents" runat="server" CssClass="hidden"
                                    OnClick="btnInsertMultipleCcCorrespondents_Click" OnClientClick="disallowOp('Content2');" />
                                <asp:HiddenField ID="hdnMultipleCcCorrespondents" runat="server" />
                                <asp:HiddenField ID="hdnCallingFromTreeview" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
                <div id="centerContentAddressbookDx">
                    <div id="centerContentAddressbookDxContainer">
                        <div id="centerContentAddressbookDxSx">
                            <div id="centerContentAddressbookCxSx">
                                <span class="AddressBookLit">
                                    <asp:Literal runat="server" ID="AddressBookLitA"></asp:Literal></span>
                            </div>
                            <asp:UpdatePanel ID="UpPnlGridAt" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <p>
                                        <asp:Literal runat="server" Text="0 Elementi selezionati" ID="LitAddressBookAt"></asp:Literal></p>
                                    <asp:GridView ID="GrdAtSelection" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                                        CssClass="tbl_rounded_custom round_onlyextreme" PageSize="5" OnPageIndexChanging="changPageGrdAt_click"
                                        BorderWidth="0" EnableViewState="true" Width="90%">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="25px" />
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDetail" ImageUrl="../Images/Icons/ico_view_document.png"
                                                        OnMouseOutImage="../Images/Icons/ico_view_document.png" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png"
                                                        RowIndex='<%# Container.DisplayIndex %>' Visible="false" AlternateText='<%# this.GetLabelDetails() %>'
                                                        ToolTip='<%# this.GetLabelDetails() %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="25px" />
                                                <ItemTemplate>
                                                    <asp:Image runat="server" ID="imgTypeAt" ImageUrl='<%# this.GetImgNoDetails((string)DataBinder.Eval(Container.DataItem, "Tipo")) %>' Visible='<%# this.GetVisibleImgNoDetails((string)DataBinder.Eval(Container.DataItem, "Tipo")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="left">
                                                <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div1" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Descrizione") %>'>
                                                        <asp:Literal ID="litDescription" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="100px" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div2" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'>
                                                        <asp:Literal ID="litCanalPref" runat="server" Text='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="100px" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div3" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Rubrica") %>'>
                                                        <asp:Literal ID="litBookType" runat="server" Text='<%# Bind("Rubrica") %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDeleteAllAt" ImageUrl="../Images/Icons/delete.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete.png" OnClick="imgDeleteAllAt_Click"
                                                        AlternateText='<%# this.GetLabelDetailsRemoveAll() %>' ToolTip='<%# this.GetLabelDetailsRemoveAll() %>'
                                                        OnClientClick="disallowOp('Content2');" />
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="center" Width="40px" />
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDeleteA" ImageUrl="../Images/Icons/delete.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete.png" RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>'
                                                        OnClick="removeAtCorrespondent_Click" Visible="true" AlternateText='<%# this.GetLabelDetailsRemove() %>'
                                                        ToolTip='<%# this.GetLabelDetailsRemove() %>' OnClientClick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="trasmId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="centerContentAddressbookDxDx">
                            <div id="centerContentAddressbookCxDx">
                                <span class="AddressBookLit">
                                    <asp:Literal runat="server" ID="AddressBookLitCc"></asp:Literal>
                                </span>
                            </div>
                            <asp:UpdatePanel ID="UpPnlGridCc" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <p>
                                        <asp:Literal runat="server" Text="0 Elementi selezionati" ID="LitAddressBookCc"></asp:Literal></p>
                                    <asp:GridView ID="GrdCctSelection" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                                        CssClass="tbl_rounded_custom round_onlyextreme" BorderWidth="0" PageSize="5"
                                        OnPageIndexChanging="changPageGrdCc_click" EnableViewState="true" Width="90%">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="25px" />
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDetail" ImageUrl="../Images/Icons/ico_view_document.png"
                                                        OnMouseOutImage="../Images/Icons/ico_view_document.png" OnMouseOverImage="../Images/Icons/ico_view_document_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_view_document_disabled.png"
                                                        RowIndex='<%# Container.DisplayIndex %>' Visible="false" AlternateText='<%# this.GetLabelDetails() %>'
                                                        ToolTip='<%# this.GetLabelDetails() %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="25px" />
                                                <ItemTemplate>
                                                    <asp:Image runat="server" ID="imgTypeCc" ImageUrl='<%# this.GetImgNoDetails((string)DataBinder.Eval(Container.DataItem, "Tipo")) %>' Visible='<%# this.GetVisibleImgNoDetails((string)DataBinder.Eval(Container.DataItem, "Tipo")) %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left">
                                                <ItemStyle HorizontalAlign="Left" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div1" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Descrizione") %>'>
                                                        <asp:Literal ID="litDescription" runat="server" Text='<%# Bind("Descrizione") %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="100px" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div2" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'>
                                                        <asp:Literal ID="litCanalPref" runat="server" Text='<%# this.GetChannelCorrespondent((bool)DataBinder.Eval(Container.DataItem, "isRubricaComune"), (string)DataBinder.Eval(Container.DataItem, "CodiceRubrica"), (string)DataBinder.Eval(Container.DataItem, "Canale")) %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle HorizontalAlign="center" Width="100px" />
                                                <ItemTemplate>
                                                    <asp:Panel id="div3" runat="server" style="overflow: hidden; text-overflow: ellipsis;" cssclass="clickable" ToolTip='<%# Bind("Rubrica") %>'>
                                                        <asp:Literal ID="litBookType" runat="server" Text='<%# Bind("Rubrica") %>'></asp:Literal>
                                                    </asp:Panel>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDeleteAllCc" ImageUrl="../Images/Icons/delete.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/addressBook_supA.png"
                                                        OnClick="imgDeleteAllCc_Click" AlternateText='<%# this.GetLabelDetailsRemoveAll() %>'
                                                        ToolTip='<%# this.GetLabelDetailsRemoveAll() %>' OnClientClick="disallowOp('Content2');" />
                                                </HeaderTemplate>
                                                <ItemStyle HorizontalAlign="center" Width="40px" />
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton runat="server" ID="imgDeleteCc" ImageUrl="../Images/Icons/delete.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/addressBook_supA.png"
                                                        RowValue='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' OnClick="removeCcCorrespondent_Click"
                                                        Visible="true" AlternateText='<%# this.GetLabelDetailsRemove() %>' ToolTip='<%# this.GetLabelDetailsRemove() %>'
                                                        OnClientClick="disallowOp('Content2');" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="trasmId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.SystemID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
        <ContentTemplate>
            <cc1:CustomButton ID="AddressBookSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddressBookSearch_Click"
                OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="AddressBookNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddressBookNew_Click" />
            <cc1:CustomButton ID="AddressBookImport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupImportDati();" />
            <cc1:CustomButton ID="AddressBookExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupExportDati();" />
            <cc1:CustomButton ID="AddressBookDownloadTemplate" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddressBookDownloadTemplate_Click" />
            <cc1:CustomButton ID="AddressBookBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddressBookBtnClose_Click" />
            <cc1:CustomButton ID="AddressBookSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddressBookBtnSave_Click"
                OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="AddressBookExportSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupExportSearch();" />
            <asp:Button ID="BtnHidden" runat="server" ClientIDMode="Static" CssClass="hidden" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
