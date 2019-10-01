<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DayInTimetable.aspx.cs"
    Inherits="NttDataWA.Popup.DayInTimetable" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        function closeCompleteTaskPopup() {
            $('#btnCompleteTaskPostback').click();
        }

        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });

            $('#contentRepListTask input, #contentRepListTask textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentRepListTask select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });
        function UpdateExpand() {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });

            $('#contentRepListTask input, #contentRepListTask textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentRepListTask select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        };
    </script>
    <style type="text/css">
        .row
        {
            clear: both;
            min-height: 1px;
            margin: 0 0 1px 0;
            text-align: left;
            vertical-align: top;
        }
        .expand img
        {
            float: right;
            border: 0px;
        }
        .col2
        {
            margin: 0px;
            padding-top: 5px;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
        <uc:ajaxpopup2 Id="ReopenTask" runat="server" Url="../popup/CompleteTask.aspx?from=ReopenTask"
        PermitClose="false" PermitScroll="false" Width="500" Height="400" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <div class="contentRep">
        <asp:UpdatePanel ID="UpRepiterTask" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <asp:Repeater ID="repListTask" runat="server" OnItemDataBound="RepListTask_DataBinding" ClientIDMode="Static"
                    OnItemCommand="RepListTask_ItemCommand">
                    <ItemTemplate>
                        <div class="contentRepListTask">
                            <fieldset>
                                <div class="row">
                                    <h2 class="expand">
                                        <asp:Literal runat="server" ID="taskDescription" Text='<%# Bind("DESCRIPTION_OBJECT") %>'></asp:Literal>
                                        <asp:Image ID="imgStatoTask" runat="server" />
                                    </h2>
                                    <div id="Div2" class="collapse" runat="server">
                                        <div class="col2">
                                            <span class="weight">
                                                <asp:Literal ID="ltl_assegnatario" Text='<%$ localizeByText:TaskAssegnatario%>' runat="server" />
                                            </span>
                                            <asp:Label ID="lbl_assegnatario" Text='<%#this.GetAssegnatario((NttDataWA.DocsPaWR.Task) Container.DataItem) %>'
                                                runat="server" />
                                        </div>
                                        <div class="row">
                                            <div class="col2">
                                                <span class="weight">
                                                    <asp:Literal ID="ltl_ReasonExtender" Text='<%$ localizeByText:TransmissionLitReasonExtended%>'
                                                        runat="server" />
                                                </span>
                                                <asp:Label ID="lbl_ReasonExtender" Text='<%# Bind("DESC_RAGIONE_TRASM") %>' runat="server" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col2">
                                                <span class="weight">
                                                    <asp:Literal ID="ltl_DataAccettazione" Text='<%$ localizeByText:TaskDataAccettazione%>'
                                                        runat="server" />
                                                </span>
                                                <asp:Label ID="lbl_dataAccettazione" Text='<%# Bind("STATO_TASK.DATA_APERTURA") %>' runat="server" />
                                            </div>
                                        </div>
<%--                                        <div class="row">
                                            <div class="col2">
                                                <span class="weight">
                                                    <asp:Literal ID="ltl_idDocCodFasc" Text='<%$ localizeByText:TaskIdDocCodFasc%>' runat="server" />
                                                </span><span class="noLink">
                                                    <asp:LinkButton ID="idDocCodFasc" runat="server" Text='<%#this.GetIdDocCodFasc((NttDataWA.DocsPaWR.Task) Container.DataItem) %>'
                                                        CommandName="ViewObjectTask"></asp:LinkButton></span>
                                            </div>
                                        </div>--%>
                                        <asp:Panel ID="pnlDataCompletamento" runat="server">
                                            <div class="row">
                                                <div class="col2">
                                                    <span class="weight">
                                                        <asp:Literal ID="ltl_dataChiusura" Text='<%$ localizeByText:TaskDataChiusura%>' runat="server" />
                                                    </span>
                                                    <asp:Label ID="lbl_dataChiusura" Text='<%# Bind("STATO_TASK.DATA_LAVORAZIONE") %>' runat="server" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <div class="row" style="padding-top: 10px">
                                            <asp:UpdatePanel ID="UpPnlIconButton" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                                <ContentTemplate>
                                                    <cc1:CustomImageButton ID="ImgViewDocument" CommandName="ViewObjectTask" runat="server"
                                                        ImageUrl="../Images/Icons/view_doc_grid.png" OnMouseOutImage="../Images/Icons/view_doc_grid.png"
                                                        OnMouseOverImage="../Images/Icons/view_doc_grid_hover.png" ImageUrlDisabled="../Images/Icons/view_doc_grid_disabled.png"
                                                        CssClass="clickableRight" ToolTip='<%$ localizeByText:IndexDetailsDocTooltip%>' OnClientClick="disallowOp('ContentPlaceHolderContent')" />
                                                    <cc1:CustomImageButton ID="ImageViewProject" CommandName="ViewObjectTask" runat="server"
                                                        ImageUrl="../Images/Icons/ricerca-fasc-1.png" OnMouseOutImage="../Images/Icons/ricerca-fasc-1.png"
                                                        OnMouseOverImage="../Images/Icons/ricerca-fasc-1_hover.png" ImageUrlDisabled="../Images/Icons/ricerca-fasc-1_disabled.png"
                                                        CssClass="clickableRight" ToolTip='<%$ localizeByText:IndexDetailsProjTooltip%>' OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                    <cc1:CustomImageButton ID="ImgCreaContributo" CommandName="CreaContributo" runat="server"
                                                        ImageUrl="../Images/Icons/create_doc_in_project.png" OnMouseOutImage="../Images/Icons/create_doc_in_project.png"
                                                        ToolTip='<%$ localizeByText:TaskCreaContributoTooltip%>' OnMouseOverImage="../Images/Icons/create_doc_in_project_hover.png"
                                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/create_doc_in_project_disabled.png" OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                    <cc1:CustomImageButton ID="ImgViewContributo" CommandName="ViewContributo" runat="server"
                                                        ImageUrl="../Images/Icons/view_details_review.png" OnMouseOutImage="../Images/Icons/view_details_review.png"
                                                        ToolTip='<%$ localizeByText:TaskViewContributoTooltip%>' OnMouseOverImage="../Images/Icons/view_details_review_hover.png"
                                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/view_details_review_disabled.png" OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                    <cc1:CustomImageButton ID="ImgCloseTask" CommandName="CloseTask" runat="server" ImageUrl="../Images/Icons/close_task.png"
                                                        OnMouseOutImage="../Images/Icons/close_task.png" ToolTip='<%$ localizeByText:TaskCloseTaskTooltip%>'
                                                        OnMouseOverImage="../Images/Icons/close_task_hover.png" CssClass="clickableRight"
                                                        ImageUrlDisabled="../Images/Icons/close_task_disabled.png" OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                    <cc1:CustomImageButton ID="ImgBlockTask" CommandName="AnnullaTask" runat="server"
                                                        ImageUrl="../Images/Icons/block_task.png" OnMouseOutImage="../Images/Icons/block_task.png"
                                                        ToolTip='<%$ localizeByText:TaskCancelTaskTooltip%>' OnMouseOverImage="../Images/Icons/block_task_hover.png"
                                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/block_task_disabled.png" OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                    <cc1:CustomImageButton ID="ImgRiapriLavorazione" CommandName="RiapriLavorazione"
                                                        runat="server" ImageUrl="../Images/Icons/Reopen_task.png" OnMouseOutImage="../Images/Icons/Reopen_task.png"
                                                        ToolTip='<%$ localizeByText:TaskRiapriLavorazioneTooltip%>' OnMouseOverImage="../Images/Icons/Reopen_task_hover.png"
                                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/Reopen_task_disabled.png" />
                                                    <cc1:CustomImageButton ID="ImgRemoveTask" CommandName="RemoveTask" runat="server"
                                                        ImageUrl="../Images/Icons/chiudiTask.png" OnMouseOutImage="../Images/Icons/chiudiTask.png"
                                                        ToolTip='<%$ localizeByText:TaskRemoveTaskTooltip%>' OnMouseOverImage="../Images/Icons/chiudiTask_hover.png"
                                                        CssClass="clickableRight" ImageUrlDisabled="../Images/Icons/chiudiTask_disabled.png" OnClientClick="disallowOp('ContentPlaceHolderContent')"/>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </div>
                        <asp:HiddenField runat="server" ID="IdTask" Value='<%# Bind("ID_TASK") %>' />
                    </ItemTemplate>
                </asp:Repeater>
                <asp:HiddenField ID="HiddenRemoveTask1" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="HiddenCancelTask1" runat="server" ClientIDMode="Static" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="DayInTimetableClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="DayInTimetableClose_Click"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <asp:Button ID="btnCompleteTaskPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnCompleteTaskPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
