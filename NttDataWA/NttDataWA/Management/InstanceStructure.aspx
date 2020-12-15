<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstanceStructure.aspx.cs"
    MasterPageFile="~/MasterPages/Base.Master" Inherits="NttDataWA.Management.InstanceStructure" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/InstanceTabs.ascx" TagPrefix="uc2" TagName="InstanceTabs" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc" TagName="ViewDocument" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        #tree
        {
            overflow: auto;
        }
        #tree a
        {
            text-overflow: ellipsis;
        }
        [rel=directory] > a > .jstree-checkbox { display:none }
    </style>
    <script src="../Scripts/jquery.jstree.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerProjectTop">
                                <div id="containerInstanceTopSx">
                                    <p>
                                        <strong>
                                            <asp:Label runat="server" ID="projectLblCodice" Text="Istanza n° "></asp:Label></strong><span
                                                class="weight"><asp:Label runat="server" ID="projectLblCodiceGenerato"></asp:Label></span></p>
                                </div>
                                <div id="containerInstanceTopCx">
                                </div>
                                <div id="containerInstanceTopDx">
                                </div>
                            </div>
                            <div id="containerInstanceBottom">
                                <div id="containerProjectCxBottom">
                                    <div id="containerProjectCxBottomSx">
                                    </div>
                                    <div id="containerProjectCxBottomDx">
                                        <div id="containerProjectTopCxOrangeDxDx">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerInstanceTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc2:InstanceTabs ID="InstanceTabs" runat="server" PageCaller="STRUCTURE" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerInstanceTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerInstance">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <!-- tree -->
                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="upnlStruttura" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <fieldset>
                                            <div class="row">
                                                <div class="col">
                                                    <div class="linkTree">
                                                        <a href="#expand" id="expand_all">
                                                            <asp:Literal ID="litTreeExpandAll" runat="server" /></a> <a href="#collapse" id="collapse_all">
                                                                <asp:Literal ID="litTreeCollapseAll" runat="server" /></a>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div id="tree">
                                                </div>
                                                <p id="log2">
                                                </p>
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
                                <asp:UpdatePanel ID="unPnlViewDocument" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:UpdatePanel ID="unPnlVD" runat="server" UpdateMode="Conditional" Visible="false">
                                            <ContentTemplate>
                                                <uc:ViewDocument ID="ViewDocument" runat="server" PageCaller="STRUCTURE_INSTANCE"></uc:ViewDocument>
                                              
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
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="InstanceDetailsDelete" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return displaySelected();" />
            <asp:Button ID="BtnChangeSelectedDocument" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="BtnChangeSelectedDocument_Click" />
            <div style="display: none">
                <asp:Button ID="BtnCheckedDocuments" runat="server" />
                <asp:Button ID="BtnContextMenu" runat="server" ClientIDMode="Static" />
            </div>
             <asp:HiddenField ID="treenode_clickDelAll" runat="server" ClientIDMode="static" />
            <asp:HiddenField ID="treenode_clickDel" runat="server" ClientIDMode="static" />
            <asp:HiddenField ID="treenode_sel" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="treenode_checked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="treenode_deleted" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function displaySelected() {
            var checked_ids = [];
            $('.jstree-checked').each(function(){
                var rawCheckedID = $(this).attr('id');
                checked_ids.push(rawCheckedID);
            });
         
            $('#treenode_checked').val(checked_ids.join(","));
         
           // $('#BtnCheckedDocuments').click();
             document.getElementById("<%=this.treenode_clickDelAll.ClientID%>").value = "up";
            __doPostBack('upPnlButtons', '');
            return true;
        }

        function JsTree() {
            $(function () {
                //$("#tree").empty();

                $("#tree")
                .jstree({
                    "core": {
                        "initially_open" : [ <asp:Literal id="jstree_initially_open" runat="server" /> ]
                        , "strings": {
                            loading: "Caricamento in corso...",
                            new_node: "Nuovo fascicolo"
                        }
                    },
                    "html_data": {
                        "ajax": {
                            "url": "InstanceStructure_getFolders.aspx?t=d" + new Date().getTime(),
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
                            "disabled" : { 
                                  "check_node" : false, 
                                  "uncheck_node" : false 
                                },
                             "directory" : {
                               "check_node" : false,
                               "uncheck_node" : false
                            } ,
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
                             "xml": {
                                "icon": {
                                    "image": "../Images/Icons/small_xml.png"
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
//                    "contextmenu": {
//                        "items": function ($node) {
//                            return {
//                                remove: {
//                                    "label": "Elimina",
//                                    "action": function (obj) {
//                                        $(obj).remove();
//                                        $('#treenode_deleted').val(obj.attr("id"));
//                                      
//                                        document.getElementById("<%=this.treenode_clickDel.ClientID%>").value = "up";
//                                          __doPostBack('upPnlButtons', '');
//                                        //$('#BtnContextMenu').click();
//                                         //document.getElementById("<%=BtnContextMenu.ClientID%>").click();

//                                    }
//                                }
//                            };
//                        }
//                    },
                  // "plugins": ["themes", "html_data", "types", "ui", "checkbox", "contextmenu"],
                  "plugins": ["themes", "html_data", "types", "ui", "checkbox"],
                    "checkbox": { "two_state" : true}
                    
                })
                .bind("select_node.jstree", function (event, data) {
                    if ($.jstree._focused().get_selected().attr('rel') != undefined) {
                        if ($.jstree._focused().get_selected().attr('id') != $('#treenode_sel').val()) {
                            $('#treenode_sel').val($.jstree._focused().get_selected().attr('id'));
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
        JsTree();
    </script>
</asp:Content>
