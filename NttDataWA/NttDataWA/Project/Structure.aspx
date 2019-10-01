<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="Structure.aspx.cs" Inherits="NttDataWA.Project.Structure" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/HeaderProject.ascx" TagPrefix="uc1" TagName="HeaderProject" %>
<%@ Register Src="~/UserControls/ProjectTabs.ascx" TagPrefix="uc2" TagName="ProjectTabs" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc" TagName="ViewDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #tree {overflow: auto;}
        #tree a {max-width: 400px; text-overflow: ellipsis;}
    </style>
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
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

            var searchText = $get('<%=TxtDescriptionRecipient.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRecipient.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeRecipient.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRecipient.ClientID%>").value = descrizione;

            __doPostBack('<%=this.TxtCodeRecipient.ClientID%>', '');
        }

        function Tipsy() {
            $(".tipsy").remove();
            $('.tooltip').tipsy();
            $('.clickable').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
            $('.clickableLeft').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.redStrike').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableUnderline').tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.referenceCode').tipsy({ className: 'reference-tip', gravity: 'n', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableLeftN').tipsy({ gravity: 'e', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
            $('.clickableRight').tipsy({ gravity: 'w', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
            $('.clickableNE').tipsy({ gravity: 'ne', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });

            var isIEmin9 = false;
            if ($.browser.msie && $.browser.version < 10) isIEmin9 = true;
            if (!isIEmin9) {
                $('.clickableRight-no-ie').tipsy({ gravity: 'w', fade: false, opacity: 1, delayIn: 0, delayOut: 0, html: true });
            }
        }
    </script>
    <style type="text/css">
    #unPnlVD {width: 80%;}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup2 Id="SearchSubset" runat="server" Url="../popup/ProjectSearchSubset.aspx"
        PermitClose="false" PermitScroll="false" IsFullScreen="false" Width="400" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('upPnlButtons', '');}" />
  <uc:ajaxpopup2 Id="Prints" runat="server" Url="../popup/visualReport_iframe.aspx"
        Width="400" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <uc1:HeaderProject ID="HeaderProject" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerProjectTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:ProjectTabs ID="ProjectTabs" runat="server" PageCaller="STRUCTURE" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerProjectTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerProject">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <!-- tree -->
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="upnlStruttura" UpdateMode="Conditional" ClientIDMode="Static">
                                    <ContentTemplate>
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <div class="linkTree">
                                                        <a href="#expand" id="expand_all"><asp:Literal ID="litTreeExpandAll" runat="server" /></a> <a href="#collapse" id="collapse_all"><asp:Literal ID="litTreeCollapseAll" runat="server" /></a>
                                                    </div>
                                                    <cc1:CustomImageButton ID="ImgFolderSearch" runat="server" ClientIDMode="Static" 
                                                        ImageUrl="../Images/Icons/search_projects.png" CssClass="clickable"
                                                        OnMouseOverImage="../Images/Icons/search_projects_hover.png"
                                                        OnMouseOutImage="../Images/Icons/search_projects.png"
                                                        OnClientClick="return ajaxModalPopupSearchSubset();" />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div id="tree"></div>
                                                <p id="log2"></p>
                                            </div>
                                        </fieldset>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="unPnlFilters" runat="server" UpdateMode="Conditional" class="box_inside">
                            <ContentTemplate>
                                <fieldset>
                                    <div class="row">
                                        <div class="colHalf4"><asp:Literal ID="lbl_dtCreation" runat="server" /></div>
                                        <div class="col"><cc1:CustomTextArea ID="txtDate" runat="server" Columns="10" MaxLength="10" ClientIDMode="Static" CssClass="txt_date datepicker" CssClassReadOnly="txt_date_disabled" /></div>
                                    </div>
                                    <div class="row">
                                        <div class="colHalf4"><asp:Literal ID="lbl_subject" runat="server" /></div>
                                        <div class="colHalf5"><cc1:CustomTextArea ID="txtSubject" runat="server" ClientIDMode="Static" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" /></div>
                                    </div>
                                    <div class="row">
                                        <div class="colHalf4">&nbsp;</div>
                                        <div class="col">
                                            <asp:RadioButtonList ID="rblRecipientType" runat="server" RepeatLayout="UnorderedList"
                                                ClientIDMode="Static" />
                                        </div>
                                        <div class="col-right-no-margin">
                                            <cc1:CustomImageButton runat="server" ID="DocumentImgSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                CssClass="clickable" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                OnClick="DocumentImgSenderAddressBook_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <asp:UpdatePanel runat="server" ID="UpPnlRecipient" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="colHalf4"><asp:Literal ID="lbl_recipient" runat="server" /></div>
                                                <div class="colHalf5">
                                                    <asp:HiddenField ID="IdRecipient" runat="server" />
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="TxtCodeRecipient" runat="server" CssClass="txt_addressBookLeft"
                                                            CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged"
                                                            autocomplete="off" AutoPostBack="true"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf8">
                                                        <cc1:CustomTextArea ID="TxtDescriptionRecipient" runat="server" CssClass="txt_addressBookRight"
                                                            CssClassReadOnly="txt_addressBookRight_disabled" autocomplete="off"></cc1:CustomTextArea>
                                                         <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="TxtDescriptionRecipient"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                            OnClientPopulated="acePopulated" Enabled="false">
                                                        </uc1:AutoCompleteExtender>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                                <br />
                                <asp:UpdatePanel ID="unPnlViewDocument" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:UpdatePanel ID="unPnlVD" runat="server" UpdateMode="Conditional" Visible="false">
                                            <ContentTemplate>
                                                <uc:ViewDocument ID="ViewDocument" runat="server"></uc:ViewDocument>
                                                <p style="clear: both; width: 100%;"><asp:LinkButton ID="LnkViewDocument" runat="server" OnClick="LnkViewDocument_Click" /></p>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="projectBtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="projectBtnSearch_Click" />
            <cc1:CustomButton ID="projectBtnRemoveFilters" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="projectBtnRemoveFilters_Click" />

            <asp:Button ID="BtnChangeSelectedDocument" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="BtnChangeSelectedDocument_Click" />
            <asp:HiddenField ID="treenode_sel" runat="server" ClientIDMode="Static" />

            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
    <script type="text/javascript">
        function JsTree() {
            $(function () {
                //$("#tree").empty();

                $("#tree")
                .bind("before.jstree", function (e, data) {
                    if (data.func == "move_node"
                        && data.args[1] == false
                        && data.plugin == "core"
                        && data.args[0].o != undefined
                    ) {
                        var selfDrop = false;
                        if (data.args[0].r != undefined && data.args[0].o[0].id == data.args[0].r[0].id) selfDrop = true;

                        var permitDrop = true;
                        if (data.args[0].ot._get_type(data.args[0].o) == 'root' && data.args[0].ot._get_type(data.args[0].r) != 'root') permitDrop = false;
                        if (data.args[0].cr===-1) permitDrop = false;
                        if (data.args[0].np[0].id==data.args[0].op[0].id) permitDrop = false;
                        var stateProject = "<%=this.GetStateOfTheProject()%>"
                        if (stateProject == 'C') {
                                alert("Attenzione! L'operazione richiesta non può essere effettuata poichè il fascicolo risulta essere chiuso.");
                                return false;
                            };

                        if (!$(data.args[0].o[0]).hasClass('jstree-unamovable') && !$(data.args[0].r[0]).hasClass('nosons') && !selfDrop && permitDrop) {
                            var confirmMoveFolder = '<asp:Literal id="litConfirmMoveFolder" runat="server" />';
                            var confirmMoveDocuments = '<asp:Literal id="litConfirmMoveDocuments" runat="server" />';
                            var confirmMessage = confirmMoveFolder.replace('##', $(data.args[0].o[0]).attr('data-title').replace('\'', '\\\''));
                            if ($(data.args[0].o[0]).hasClass('nosons')) {
                                confirmMessage = '';
                                for (var i=0; i<data.args[0].o.length; i++) 
                                    confirmMessage = confirmMessage + '\n'+$(data.args[0].o[i]).attr('data-title');
                                confirmMessage = confirmMoveDocuments.replace('##', confirmMessage.replace('\'', '\\\''));
                            }
                            if (!confirm(confirmMessage)) {
                                e.stopImmediatePropagation();
                                return false;
                            }
                        }
                        else {
                            if (!$(data.args[0].o[0]).hasClass('jstree-unamovable')) alert('<asp:Literal id="litTreeAlertOperationNotAllowed" runat="server" />');
                            e.stopImmediatePropagation();
                            return false;
                        }
                    }
                })
                .jstree({
                    "core": {
                        "initially_open" : [ <asp:Literal id="jstree_initially_open" runat="server" /> ]
                        , "strings": {
                            loading: '<asp:Literal id="litTreeLoading" runat="server" />',
                            new_node: "Nuovo fascicolo"
                        }
                    },
                    "html_data": {
                        "ajax": {
                            "url": "Structure_getFolders.aspx?t=d" + new Date().getTime(),
                            "data": function (n) {
                                return { id: n.attr ? n.attr("id") : 0 };
                            }
                        }
                    },
                    "themes": {
                        "theme": "classic",
                        "dots": true,
                        "icons": true
                    },
                    "types": {
                        "valid_children": "all",
                        "types": {
                            "access": {
                                "icon": {
                                    "image": "../Images/Icons/small_access.png"
                                }
                            },
                            "chip": {
                                "icon": {
                                    "image": "../Images/Icons/small_chip.png"
                                }
                            },
                            "doc": {
                                "icon": {
                                    "image": "../Images/Icons/small_doc.png"
                                }
                            },
                            "doc2": {
                                "icon": {
                                    "image": "../Images/Icons/small_doc2.png"
                                }
                            },
                            "doc3": {
                                "icon": {
                                    "image": "../Images/Icons/small_doc3.png"
                                }
                            },
                            "docx": {
                                "icon": {
                                    "image": "../Images/Icons/small_docx.png"
                                }
                            },
                            "docx2": {
                                "icon": {
                                    "image": "../Images/Icons/small_docx2.png"
                                }
                            },
                            "eml": {
                                "icon": {
                                    "image": "../Images/Icons/small_eml.png"
                                }
                            },
                            "gen": {
                                "icon": {
                                    "image": "../Images/Icons/small_gen.png"
                                }
                            },
                            "gif": {
                                "icon": {
                                    "image": "../Images/Icons/small_jpg.png"
                                }
                            },
                            "html": {
                                "icon": {
                                    "image": "../Images/Icons/small_html.png"
                                }
                            },
                            "ie": {
                                "icon": {
                                    "image": "../Images/Icons/small_ie.png"
                                }
                            },
                            "jpg": {
                                "icon": {
                                    "image": "../Images/Icons/small_jpg.png"
                                }
                            },
                            "no_file": {
                                "icon": {
                                    "image": "../Images/Icons/small_no_file.png"
                                }
                            },
                            "odt": {
                                "icon": {
                                    "image": "../Images/Icons/small_odt.png"
                                }
                            },
                            "pdf": {
                                "icon": {
                                    "image": "../Images/Icons/small_pdf.png"
                                }
                            },
                            "png": {
                                "icon": {
                                    "image": "../Images/Icons/small_jpg.png"
                                }
                            },
                            "ppt": {
                                "icon": {
                                    "image": "../Images/Icons/small_ppt.png"
                                }
                            },
                            "pptx": {
                                "icon": {
                                    "image": "../Images/Icons/small_pptx.png"
                                }
                            },
                            "rtf": {
                                "icon": {
                                    "image": "../Images/Icons/small_rtf.png"
                                }
                            },
                            "sxw": {
                                "icon": {
                                    "image": "../Images/Icons/small_sxw.png"
                                }
                            },
                            "tif": {
                                "icon": {
                                    "image": "../Images/Icons/small_tif.png"
                                }
                            },
                            "txt": {
                                "icon": {
                                    "image": "../Images/Icons/small_txt.png"
                                }
                            },
                            "wri": {
                                "icon": {
                                    "image": "../Images/Icons/small_wri.png"
                                }
                            },
                            "wri2": {
                                "icon": {
                                    "image": "../Images/Icons/small_wri2.png"
                                }
                            },
                            "xls": {
                                "icon": {
                                    "image": "../Images/Icons/small_xls.png"
                                }
                            },
                            "xls2": {
                                "icon": {
                                    "image": "../Images/Icons/small_xls2.png"
                                }
                            },
                            "xls3": {
                                "icon": {
                                    "image": "../Images/Icons/small_xls3.png"
                                }
                            },
                            "xlsx": {
                                "icon": {
                                    "image": "../Images/Icons/small_xlsx.png"
                                }
                            },
                            "xlsx2": {
                                "icon": {
                                    "image": "../Images/Icons/small_xlsx2.png"
                                }
                            },
                            "zip": {
                                "icon": {
                                    "image": "../Images/Icons/small_zip.png"
                                }
                            },
                            "default": {
                                "valid_children": ["default", "access", "chip", "doc", "doc2", "doc3", "docx", "docx2", "eml", "gen", "gif", "html", "ie", "jpg", "no_file", "odt", "pdf", "png", "ppt", "pptx", "rtf", "sxw", "tif", "txt", "wri", "wri2", "xls", "xls2", "xls3", "xlsx", "xlsx2", "zip"]
                            }
                        }
                    },
                    "crrm": {
                        "move": {
                            "check_move": function (m) {
                                if (m.o.hasClass('jstree-unamovable')) {
                                    return false;
                                }
                                else if (m.p == "before" || m.p == "after") {
                                    return false;
                                }
                                else {
                                    return true;
                                }
                            }
                        }
                    },
                    "dnd": {
                        "drag_check": function (data) {
                            if (data.r.hasClass("nosons")) {
                                return false;
                            }
                            return {
                                after: false,
                                before: false,
                                inside: true
                            };
                        },
                        "drag_finish": function (data) {
                            if ($('.jstree-draggable-selected').length > 0 && confirm('Confermi operazione?')) {
                                var iDs = "";
                                for (var i = 0; i < $('.jstree-draggable-selected').length; i++) {
                                    if (iDs.length > 0) iDs += ",";
                                    iDs += $('.jstree-draggable-selected')[i].id;
                                }

                                disallowOp('content');
                                $.ajax({
                                    'async': true,
                                    'timeout': 2160000,
                                    'type': 'POST',
                                    'url': "Structure_getFolders.aspx",
                                    'data': {
                                        "operation": "drag_documents",
                                        "ids": iDs,
                                        "id": data.o.id,
                                        "ref": data.r[0].id
                                    },
                                    'success': function (r) {
                                        $('#log2').html(r);
                                    },
                                    'error': function (jqXHR, textStatus, errorThrown) {
                                        $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
                                    },
                                    'complete': function (r) {
                                        $('.jstree-draggable').removeClass('jstree-draggable-selected');
                                        reallowOp();
                                    }
                                });
                            }
                        }
                    },
                    "plugins": ["themes", "html_data", "dnd", "crrm", "types", "ui"]
                })
	            .bind("move_node.jstree", function (e, data) {
	                data.rslt.o.each(function (i) {
	                    /*
	                    data.rslt
	                    .o - the node being moved
	                    .r - the reference node in the move
	                    .ot - the origin tree instance
	                    .rt - the reference tree instance
	                    .p - the position to move to (may be a string - "last", "first", etc)
	                    .cp - the calculated position to move to (always a number)
	                    .np - the new parent
	                    .oc - the original node (if there was a copy)
	                    .cy - boolen indicating if the move was a copy
	                    .cr - same as np, but if a root node is created this is -1
	                    .op - the former parent
	                    .or - the node that was previously in the position of the moved node 
	                    */

	                    var iDs = "";
	                    for (var i = 0; i < data.rslt.o.length; i++) {
	                        if (iDs.length > 0) iDs += ",";
	                        iDs += data.rslt.o[i].id;
	                    }

                        if (data.rslt.cr === -1 || data.rslt.np.attr("id")==data.rslt.op.attr("id")) {
                            e.stopImmediatePropagation();
                            return;
                        }
                        else {
	                        disallowOp('content');
	                        $.ajax({
	                            'async': true,
	                            'type': 'POST',
	                            'timeout': 2160000,
	                            'url': "Structure_getFolders.aspx",
	                            'data': {
	                                "operation": "move_node",
	                                "ids": iDs,
	                                "id": $(this).attr("id"),
	                                "ref": data.rslt.r.attr("id"),
	                                "parent": data.rslt.cr === -1 ? 1 : data.rslt.np.attr("id"),
                                    "oparent": data.rslt.op.attr("id"),
	                                "position": data.rslt.p
	                            },
	                            'success': function (r) {
	                                $('#log2').html(r);
	                            },
	                            'error': function (jqXHR, textStatus, errorThrown) {
	                                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
	                            },
	                            'complete': function () {
	                                reallowOp();
	                            }
	                        });
                        }
	                });
	            })
                .bind("select_node.jstree", function (event, data) {
                    if ($.jstree._focused().get_selected().attr('rel') != undefined) {
                        if ($.jstree._focused().get_selected().attr('id') != $('#treenode_sel').val()) {
                            $('#treenode_sel').val($.jstree._focused().get_selected().attr('id').replace('doc_', ''));
                            $('#tree .jstree-search').removeClass("jstree-search");
                        }
                    }
                    else {
                        //$('#tree .jstree-clicked').removeClass("jstree-clicked");
                        $('#treenode_sel').val('');
                    }
                    $('#BtnChangeSelectedDocument').click();
                })
                .bind("loaded.jstree", function (e, data) {
                    if ($('#treenode_sel').val().length > 0) $("#tree").jstree("select_node", '#' + $('#treenode_sel').val())

                    // assign tooltip and css class to a
                    tooltipTipsy();
                    $('.jstree-leaf').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-last').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-open').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-closed').each(function (i) {
                        $(this).attr('title', '');
                    });
                })
                .bind("open_node.jstree", function (e, data) {
                    // assign tooltip and css class to a
                    tooltipTipsy();
                    $('.jstree-leaf').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-last').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-open').each(function (i) {
                        $(this).attr('title', '');
                    });
                    $('.jstree-closed').each(function (i) {
                        $(this).attr('title', '');
                    });

                    // highlight
                    if (searchTreeQry!=null) {
                        for (var j = 0; j < searchTreeQry.length; j++) {
                            if (searchTreeQry[j].isResult=='true' && !$('#'+searchTreeQry[j].id+' a').hasClass('jstree-search'))
                                $('#'+searchTreeQry[j].id+' a').addClass('jstree-search');
                        }
                    }
                });

                // resolve bug, see https://github.com/vakata/jstree/issues/174
                $("#jstree-marker-line").remove();

                // collapse/expand all noded
                $("#collapse_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('close_all');
                    return false;
                });
                $("#expand_all").click(function (e) {
                    e.stopImmediatePropagation();
                    $('#tree').jstree('open_all');
                    return false;
                });
            }
        )
        };

        var nodes;
        var searchTreeQry = null;
        JsTree();


        function searchTree(qry) {
            // set all nodes color back to normal
            $("#tree").find('.jstree-search').removeClass('jstree-search');
            $("#tree").find('.jstree-clicked').removeClass('jstree-clicked');
            searchTreeQry = null;

            $.getJSON('Structure_getFolders.aspx', { 'q': qry }, function (data) {
                if (data != null) {
                    searchTreeQry = data;
                    // server returns a list of object that is { string id, bool isResult }.  
                    // the results contains any elements in the search and all of the parents of that element.  
                    // all these elements are necessary to expand all the branches. 
                    // only search results return a true value for isResult
                    nodes = data;
                    var i = 0;


                    $('#tree').bind('open_node.jstree', function (e, d) {
                        var index = -1;
                        for (var j = 0; j < data.length; j++) if (data[j].id == d.rslt.obj[0].id) index = j;
                        searchTreeFromIndex(index);
                    });

                    if (data.length == 1 && false) {// delete false condition to autoselect folder when result is one
                        $("#tree").jstree("select_node", "#" + data[0].id).trigger("select_node.jstree");
                    }
                    else {
                        while (i < data.length) {                                       
                            var node = '#' + data[i].id;
                            if (data[i].isResult == 'true') {
                                $(node).find('>a').addClass('jstree-search');
                                i++;
                            }
                            else {
                                $('#tree').jstree('open_node', $(node));
                                i++;
                            }
                        }
                    }
                }
            })
            .error(function (jqXHR, textStatus, errorThrown) {
                $('#log2').html('ERROR: ' + textStatus + '<br />Text: ' + jqXHR.responseText);
            });

            $('.retvalSearchSubset input').get(0).value = '';
        }
                
        function searchTreeDocuments(data) {
            // set all nodes color back to normal
            $("#tree").find('.jstree-search').removeClass('jstree-search');
            $("#tree").find('.jstree-clicked').removeClass('jstree-clicked');

            if (data != null) {

                data = jQuery.parseJSON(data);

                // server returns a list of object that is { string id, bool isResult }.  
                // the results contains any elements in the search and all of the parents of that element.  
                // all these elements are necessary to expand all the branches. 
                // only search results return a true value for isResult
                nodes = data;
                var i = 0;


                $('#tree').bind('open_node.jstree', function (e, d) {
                    var index = -1;
                    for (var j = 0; j < data.length; j++) if (data[j].id == d.rslt.obj[0].id) index = j;
                    //setTimeout('searchTreeFromIndex('+index+')', 500);
                    searchTreeFromIndex(index);
                });

                if (data.length == 1 && false) {// delete false condition to autoselect folder when result is one
                    $("#tree").jstree("select_node", "#" + data[0].id).trigger("select_node.jstree");
                }
                else {
                    while (i < data.length) {                                       
                        var node = '.' + data[i].id;
                        if (data[i].isResult == 'false') {
                            node = '#' + data[i].id;
                            $('#tree').jstree('open_node', $(node));
                        }
                        else {
                            $(node).find('>a').addClass('jstree-search');
                        }
                        i++;
                    }
                }
            };
        }

        function searchTreeFromIndex(index) {
            for (var j = index; j < nodes.length; j++) {
                if (nodes[j]) {
                    var node = '.' + nodes[j].id;
                    if (nodes[j].isResult == 'false') {
                        node = '#' + nodes[j].id;
                        $('#tree').jstree('open_node', $(node));                        
                    }
                    else {
                        $(node).find('>a').addClass('jstree-search');
                    }
                }
            }
        }
    </script>
</asp:Content>
