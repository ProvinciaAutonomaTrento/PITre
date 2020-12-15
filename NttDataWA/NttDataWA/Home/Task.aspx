<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Task.aspx.cs" Inherits="NttDataWA.Home.Task"
    MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Src="../UserControls/HomeTabs.ascx" TagPrefix="uc1" TagName="HomeTabs" %>
<%@ Register Src="../UserControls/HeaderHome.ascx" TagPrefix="uc2" TagName="HeaderHome" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .gridViewResult
        {
            min-width: 100%;
        }
        .gridViewResult th
        {
            text-align: center;
        }
        .gridViewResult td
        {
            text-align: center;
            padding: 5px;
        }
        #gridViewResult tr.selectedrow
        {
            background: #f3edc6;
            color: #333333;
        }
        .noLink a:link
        {
            text-decoration: none;
            color: #333333;
            font-weight: bold;
        }
    </style>
    <script type="text/javascript">
        function CombineRowsHover() {
            $(".tbl_rounded tr.NormalRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("NormalRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("NormalRowHover");
            });

            $(".tbl_rounded tr.AltRow td").hover(function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).addClass("AltRowHover");
            }, function () {
                var row = $(this).parent().parent().children().index($(this).parent());
                var index = row + 1;
                if (row % 2 == 0) index = row - 1;
                $($(".tbl_rounded tr").get(index)).removeClass("AltRowHover");
            });
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AnswerChooseRecipient" runat="server" Url="../popup/AnswerChooseRecipient.aspx"
        Width="800" Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="CompleteTask" runat="server" Url="../popup/CompleteTask.aspx"
        PermitClose="false" PermitScroll="false" Width="500" Height="350" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="ReopenTask" runat="server" Url="../popup/CompleteTask.aspx?from=ReopenTask"
        PermitClose="false" PermitScroll="false" Width="500" Height="400" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="CompleteTaskFromDay" runat="server" Url="../popup/CompleteTask.aspx?from=CompleteTaskFromDay"
        PermitClose="false" PermitScroll="false" Width="500" Height="350" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupCompleteTaskFromDay');}" />
    <uc:ajaxpopup2 Id="DayInTimetable" runat="server" Url="../popup/DayInTimetable.aspx"
        PermitClose="false" PermitScroll="false" Width="700" Height="450" CloseFunction="function (event, ui) {__doPostBack('UpPnlButtons', 'closePopupDayInTimetable');}" />
    <div id="containerTop">
        <div id="containerTabHome">
            <asp:UpdatePanel runat="server" ID="UpHeaderHome" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
                    <div id="containerStandardTop">
                        <div id="containerStandardTopCxHome">
                            <img src="../Images/Common/griff.png" alt="" title="" />
                        </div>
                        <div id="containerHomeHeader">
                            <div id="containerHeaderHomeSx">
                                <strong>
                                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                                </strong>
                            </div>
                            <div id="containerHeaderHomeDx">
                                <div class="styled-select_full">
                                    <asp:DropDownList ID="ddlRolesUser" runat="server" CssClass="chzn-select-deselect"
                                        AutoPostBack="true" Width="700px">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
                ClientIDMode="Static">
                <ContentTemplate>
                    <div id="containerTabIndex">
                        <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                                <uc1:HomeTabs runat="server" PageCaller="TASK" ID="HomeTabs"></uc1:HomeTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpdatePanelFilterSx">
                        <ContentTemplate>
                            <asp:PlaceHolder runat="server" ID="PnlTypeTask">
                                <div id="containerAdlHome">
                                    <div class="weight">
                                        <asp:RadioButtonList runat="server" ID="RblTypeTask" RepeatDirection="Horizontal"
                                            RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="RblTypeTask_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div class="containerViewMode">
                        <asp:UpdatePanel runat="server" ID="UpnlViewMode" ClientIDMode="Static" UpdateMode="Conditional">
                            <ContentTemplate>
                                <cc1:CustomImageButton ID="ImageViewGrid" runat="server" ImageUrl="../Images/Icons/Griglia.png"
                                    CssClass="clickableLeftN" OnMouseOutImage="../Images/Icons/Griglia.png" OnMouseOverImage="../Images/Icons/Griglia_hover.png"
                                    ImageUrlDisabled="../Images/Icons/Griglia_disabled.png" OnClick="ImageViewGrid_Click" />
                                <cc1:CustomImageButton ID="ImageViewCalendar" runat="server" ImageUrl="../Images/Icons/Calendario.png"
                                    OnMouseOutImage="../Images/Icons/Calendario.png" OnMouseOverImage="../Images/Icons/Calendario_hover.png"
                                    CssClass="clickableLeftN" ImageUrlDisabled="../Images/Icons/Calendario_disabled.png"
                                    OnClick="ImageViewCalendar_Click" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="containerViewMode" style="margin-top: 5px;">
                        <asp:UpdatePanel ID="UpCbxTaskChiusi" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel ID="pnlCbxTaskChiusi" runat="server" Visible="true">
                                    <asp:CheckBox ID="cbxTaskChiusi" runat="server" Checked="false" AutoPostBack="true"
                                        OnCheckedChanged="CbxTaskChiusi_CheckedChanged" />
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div id="containerNotificationCenter">
                <div id="containerNotificationCenterAdlHome">
                    <asp:UpdatePanel runat="server" ID="UpnlCalendar" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:Calendar ID="calendar" runat="server" ShowGridLines="true" CssClass="calendar"
                                Visible="False" OnPreRender="Calendar_PreRender" OnDayRender="Calendar_DayRender"
                                SelectedDayStyle-BackColor="#e6ffe6" SelectedDayStyle-ForeColor="#01497B" DayStyle-ForeColor="#01497B"
                                DayStyle-VerticalAlign="Top" DayStyle-Height="45px" OnSelectionChanged="Calendar_SelectionChanged"
                                DayStyle-Wrap="false" NextMonthText="<img src='../Images/Icons/icon_next.png' onmouseover=this.src='../Images/Icons/icon_next_hover.png' onmouseout=this.src='../Images/Icons/icon_next.png' border=0/>"
                                PrevMonthText="<img src='../Images/Icons/icon_prev.png' onmouseover=this.src='../Images/Icons/icon_prev_hover.png' onmouseout=this.src='../Images/Icons/icon_prev.png' border=0 />">
                                <TitleStyle CssClass="calendar_TitleStyle" />
                                <DayHeaderStyle CssClass="calendar_DayHeaderStyle" />
                                <NextPrevStyle CssClass="calendar_NextPrevStyle" />
                                <OtherMonthDayStyle CssClass="calendar_OtherMonthDayStyle" />
                                <DayStyle CssClass="calendar_DayStyle" />
                            </asp:Calendar>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                        <ContentTemplate>
                            <asp:GridView ID="gridViewResult" runat="server" AllowSorting="true" AllowPaging="false"
                                AutoGenerateColumns="false" CssClass="gridViewResult tbl_rounded round_onlyextreme"
                                HorizontalAlign="Center" AllowCustomPaging="true" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                OnRowDataBound="gridViewResult_RowDataBound" OnPreRender="gridViewResult_PreRender"
                                OnRowCreated="gridViewResult_ItemCreated" OnRowCommand="GridView_RowCommand">
                                <Columns>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idTask" Text='<%# Bind("ID_TASK") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="contributoObbligatorio" Text='<%# Bind("CONTRIBUTO_OBBLIGATORIO") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idStatoTask" Text='<%# Bind("STATO_TASK.ID_STATO_TASK") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idProfile" Text='<%# Bind("ID_PROFILE") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idProject" Text='<%# Bind("ID_PROJECT") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idTipoAtto" Text='<%# Bind("ID_TIPO_ATTO") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idTrasmSingola" Text='<%# Bind("ID_TRASM_SINGOLA") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idProfileReview" Text='<%# Bind("ID_PROFILE_REVIEW") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idRagione" Text='<%# Bind("ID_RAGIONE_TRASM") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idGruppoMittente" Text='<%# Bind("RUOLO_MITTENTE.idGruppo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="False">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="idPeopleMittente" Text='<%# Bind("UTENTE_MITTENTE.idPeople") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-Width="20%" HeaderStyle-Wrap="false" Visible="false"
                                        HeaderText='<%$ localizeByText:TaskAssegnatario%>'>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="assegnatario" Text='<%#this.GetAssegnatario((NttDataWA.DocsPaWR.Task) Container.DataItem) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-Width="20%" HeaderStyle-Wrap="false" Visible="false"
                                        HeaderText='<%$ localizeByText:TaskDestinatario%>'>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="destinatario" Text='<%#this.GetDestinatario((NttDataWA.DocsPaWR.Task) Container.DataItem) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TransmissionLitReasonExtended%>'
                                        HeaderStyle-Width="15%">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="descRagioneTrasm" Text='<%# Bind("DESC_RAGIONE_TRASM") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TaskDataAccettazione%>' HeaderStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="dataAccettazione" Text='<%# Bind("STATO_TASK.DATA_APERTURA") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TaskNoteApertura%>' HeaderStyle-Width="20%"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="noteApertura" Text='<%# Bind("STATO_TASK.NOTE_RIAPERTURA") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-Width="10%" HeaderText='<%$ localizeByText:TaskIdDocCodFasc%>'
                                        HeaderStyle-Wrap="false">
                                        <ItemTemplate>
                                            <span class="noLink">
                                                <asp:LinkButton ID="idDocCodFasc" runat="server" Text='<%#this.GetIdDocCodFasc((NttDataWA.DocsPaWR.Task) Container.DataItem) %>'
                                                    CommandName="ViewObjectTask"></asp:LinkButton></span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-Width="20%" HeaderText='<%$ localizeByText:TaskObjectDescription%>'
                                        HeaderStyle-Wrap="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="objectDescription" Text='<%# Bind("DESCRIPTION_OBJECT") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TaskDataScadenza%>' HeaderStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="dataScadenza" Text='<%#this.GetDataScadenza((NttDataWA.DocsPaWR.Task) Container.DataItem) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TaskDataChiusura%>' HeaderStyle-Width="5%"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="dataLavorazione" Text='<%# Bind("STATO_TASK.DATA_LAVORAZIONE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:TaskNoteLavorazione%>' HeaderStyle-Width="20%"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="noteLavorazione" Text='<%# Bind("STATO_TASK.NOTE_LAVORAZIONE") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <cc1:CustomImageButton ID="ImgViewDocument" CommandName="ViewObjectTask" runat="server"
                                                ImageUrl="../Images/Icons/view_doc_grid.png" OnMouseOutImage="../Images/Icons/view_doc_grid.png"
                                                OnMouseOverImage="../Images/Icons/view_doc_grid_hover.png" ImageUrlDisabled="../Images/Icons/view_doc_grid_disabled.png"
                                                CssClass="clickableLeft" ToolTip='<%$ localizeByText:IndexDetailsDocTooltip%>' />
                                            <cc1:CustomImageButton ID="ImageViewProject" CommandName="ViewObjectTask" runat="server"
                                                ImageUrl="../Images/Icons/ricerca-fasc-1.png" OnMouseOutImage="../Images/Icons/ricerca-fasc-1.png"
                                                OnMouseOverImage="../Images/Icons/ricerca-fasc-1_hover.png" ImageUrlDisabled="../Images/Icons/ricerca-fasc-1_disabled.png"
                                                CssClass="clickableLeft" ToolTip='<%$ localizeByText:IndexDetailsProjTooltip%>' />
                                            <cc1:CustomImageButton ID="ImgCreaContributo" CommandName="CreaContributo" runat="server"
                                                ImageUrl="../Images/Icons/create_doc_in_project.png" OnMouseOutImage="../Images/Icons/create_doc_in_project.png"
                                                ToolTip='<%$ localizeByText:TaskCreaContributoTooltip%>' OnMouseOverImage="../Images/Icons/create_doc_in_project_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/create_doc_in_project_disabled.png" />
                                            <cc1:CustomImageButton ID="ImgViewContributo" CommandName="ViewContributo" runat="server"
                                                ImageUrl="../Images/Icons/view_details_review.png" OnMouseOutImage="../Images/Icons/view_details_review.png"
                                                ToolTip='<%$ localizeByText:TaskViewContributoTooltip%>' OnMouseOverImage="../Images/Icons/view_details_review_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/view_details_review_disabled.png" />
                                            <cc1:CustomImageButton ID="ImgCloseTask" CommandName="CloseTask" runat="server" ImageUrl="../Images/Icons/close_task.png"
                                                OnMouseOutImage="../Images/Icons/close_task.png" ToolTip='<%$ localizeByText:TaskCloseTaskTooltip%>'
                                                OnMouseOverImage="../Images/Icons/close_task_hover.png" CssClass="clickableLeft"
                                                ImageUrlDisabled="../Images/Icons/close_task_disabled.png" />
                                            <cc1:CustomImageButton ID="ImgBlockTask" CommandName="AnnullaTask" runat="server"
                                                ImageUrl="../Images/Icons/block_task.png" OnMouseOutImage="../Images/Icons/block_task.png"
                                                ToolTip='<%$ localizeByText:TaskCancelTaskTooltip%>' OnMouseOverImage="../Images/Icons/block_task_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/block_task_disabled.png" />
                                            <cc1:CustomImageButton ID="ImgRiapriLavorazione" CommandName="RiapriLavorazione"
                                                runat="server" ImageUrl="../Images/Icons/Reopen_task.png" OnMouseOutImage="../Images/Icons/Reopen_task.png"
                                                ToolTip='<%$ localizeByText:TaskRiapriLavorazioneTooltip%>' OnMouseOverImage="../Images/Icons/Reopen_task_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/Reopen_task_disabled.png" />
                                            <cc1:CustomImageButton ID="ImgRemoveTask" CommandName="RemoveTask" runat="server"
                                                ImageUrl="../Images/Icons/chiudiTask.png" OnMouseOutImage="../Images/Icons/chiudiTask.png"
                                                ToolTip='<%$ localizeByText:TaskRemoveTaskTooltip%>' OnMouseOverImage="../Images/Icons/chiudiTask_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/chiudiTask_disabled.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <asp:PlaceHolder ID="plcNavigator" runat="server" />
                            <asp:UpdatePanel ID="upPnlGridIndexes" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:HiddenField ID="HiddenRemoveTask" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="HiddenCancelTask" runat="server" ClientIDMode="Static" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hiddenValueTask" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenDataScadenza" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        //alert($(".chzn-select-deselect").tipsy);
        $(".chzn-select-deselect").tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
        $(".chzn-select").tipsy({ gravity: 's', fade: false, opacity: 1, delayIn: 0, delayOut: 0 });
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
