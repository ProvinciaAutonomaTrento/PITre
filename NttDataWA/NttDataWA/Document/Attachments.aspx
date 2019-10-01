<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Attachments.aspx.cs" Inherits="NttDataWA.Document.Attachments" EnableEventValidation="false" %>

<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<%@ Register Src="~/UserControls/ViewDocument.ascx" TagPrefix="uc5" TagName="ViewDocument" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <link href="../Css/bootstrap.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/require.js"></script>
    <style type="text/css">
        .box_inside fieldset:first-child
        {
            position: relative;
            top: 3px;
        }
        
        #ver_editable
        {
            cursor: pointer;
        }
        #ver_editable img
        {
            border: 0;
        }
        #rblFilter_row
        {
            margin: 3px 0;
        }
        #rblFilter_row span
        {
            float: left;
            font-weight: bold;
            margin: 0 10px 0 0;
        }
        #rblFilter
        {
            list-style-type: none;
            display: inline;
        }
        #rblFilter li
        {
            float: left;
            padding: 0;
            margin: 0;
        }
        #row_object
        {
            max-height: 35px;
            overflow: auto;
        }
        
        .tbl_rounded
        {
            margin: 3px 0 0 5px;
        }
        .tbl_rounded td
        {
            background: #fff;
            cursor: pointer;
        }
        .tbl_rounded td.grdAllegati_code
        {
            width: 40px;
            text-align: center;
        }
        .tbl_rounded td.grdAllegati_date
        {
            width: 90px;
        }
        .tbl_rounded td.grdAllegati_pages
        {
            width: 40px;
            text-align: center;
        }
        .tbl_rounded td.grdAllegati_icon
        {
            width: 25px;
        }
        .tbl_rounded td.grdAllegati_description
        {
            width: 90%;
            text-align: left;
        }
        
        #GrdDocumentAttached
        {
            border: 0;
        }
        #GrdDocumentAttached td
        {
            border: 0;
            padding: 0;
        }
        #GrdDocumentAttached tr.selectedrow span
        {
            background: #f3edc6;
            color: #333333;
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
        .customEnableButton {
                        
                        padding: 8px 5px;
                        background-size: cover;
                        background-position-y: center;
                        border-radius: 5px;
                        font-weight: bold;
                        border: 1px solid #3377ff;
                        margin: 0 !important;
                    }

        #AttachmentsBtnUploadAllegati {
            width: 140px;
            background-size: cover;
            background-position-y: -1px;
            border-radius: 3px;
        }
    </style>
    <script type="text/javascript">
        function verEditable() {
            $('#ver_editable').inlineEdit({
                buttons: '<a href="#" class="save"><img src="<%=Page.ResolveClientUrl("~/Images/Icons/icon_yes.jpg") %>" alt="" /></a> <a href="#" class="cancel"><img src="<%=Page.ResolveClientUrl("~/Images/Icons/icon_abort.jpg") %>" alt="" /></a>',
                buttonsTag: 'a',
                save: function (event, data) {
                    $.ajax({
                        url: 'AttachmentInlineSaver.aspx',
                        type: 'POST',
                        data: { 'value': data.value }
                    }).responseText;
                }
            });
        };

        function onClickScambiaAllegato() {
            if (<%=this.EnabledManagingSwapAttachments.ToString().ToLower()%>) {
                ajaxModalPopupAttachmentsSwap();
            }
            else {
                __doPostBack('panelAllegati', 'SWAP');
            }
            return false;
        };

         
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="UplodadFile" runat="server" Url="../popup/UploadFile.aspx?idDoc=<%=GetIdDocumento()%>" Width="600"
        Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="RepositoryView" runat="server" Url="../Repository/RepositoryView.aspx" Width="850"
        Height="600" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
    <uc:ajaxpopup Id="Signature" runat="server" Url="../popup/Signature.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="DocumentViewer" runat="server" Url="../popup/DocumentViewer.aspx"
        ForceDontClose="true" PermitClose="false" PermitScroll="false" IsFullScreen="true"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', 'closeZoom');}" />
    <uc:ajaxpopup Id="ActiveXScann" runat="server" Title="Acquisizione da scanner" Url="../popup/acquisizione.aspx"
        ShowLoading="false" Width="1000" Height="700" PermitClose="true" PermitScroll="false"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="AttachmentsAdd" runat="server" Title="Nuovo allegato" Url="../popup/attachment.aspx"
        Width="400" Height="350" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {$('#grid_rowindex').val('-2'); __doPostBack('panelAllegati','');}" />

    <uc:ajaxpopup Id="AttachmentsUpload" runat="server" Title="Acquisisci Allegati" Url="../popup/AttachmentsUpload.aspx" Width="700" Height="500" PermitClose="true"  CloseFunction="function (event, ui) {__doPostBack('panelAllegati', '');}" />

    <uc:ajaxpopup Id="AttachmentsModify" runat="server" Title="Modifica allegato" Url="../popup/attachment.aspx?t=modify"
        Width="400" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelAllegati', '');}" />
    <uc:ajaxpopup Id="AttachmentsSwap" runat="server" Title="Scambia" Url="../popup/attachment.aspx?t=swap"
        Width="400" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelAllegati', '');}" />
    <uc:ajaxpopup Id="DigitalSignSelector" runat="server" Title="Firma documento" Url="../popup/DigitalSignSelector.aspx?TipoFirma=sign"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="DigitalCosignSelector" runat="server" Title="Firma documento" Url="../popup/DigitalSignSelector.aspx?TipoFirma=cosign&Caller=cosign"
        Width="600" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="DigitalVisureSelector" runat="server" Title="Approva documento" Url="../popup/DigitalVisure.aspx"
        Width="600" Height="300" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />     
    <uc:ajaxpopup Id="DigitalSignDetails" runat="server" Url="../popup/DigitalSignDetails.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="VersionAdd" runat="server" Url="../popup/version_add.aspx" Width="450"
        Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="ModifyVersion" runat="server" Url="../popup/version_add.aspx?modifyVersion=t"
        Width="450" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="SaveDialog" runat="server" Url="../CheckInOutApplet/CheckInOutSaveLocal.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="430"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="CheckOutDocument" runat="server" Url="../CheckInOutApplet/CheckOutDocument.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="250"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="UndoCheckOut" runat="server" Url="../CheckInOutApplet/UndoPendingChange.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="CheckInDocument" runat="server" Url="../CheckInOutApplet/CheckInDocument.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="330"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="OpenLocalCheckOutFile" runat="server" Url="../CheckInOutApplet/OpenLocalCheckOutFile.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="300"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="ShowCheckOutStatus" runat="server" Url="../CheckInOutApplet/ShowCheckOutStatus.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="CheckOutModelApplet" runat="server" Url="../CheckInOutApplet/CheckOutModel.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="CheckOutModelActiveX" runat="server" Url="../CheckInOut/CheckOutModel.aspx"
        IsFullScreen="false" PermitClose="true" PermitScroll="false" Width="500" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="HSMSignature" runat="server" Url="../popup/HSM_Signature.aspx"
        IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="700" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="StartProcessSignature" runat="server" Url="../popup/StartProcessSignature.aspx"
        Width="1200" Height="800" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="DetailsLFAutomaticMode" runat="server" Url="../popup/DetailsLFAutomaticMode.aspx"
        Width="750" Height="400" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', 'SignatureProcessConcluted');}" />
    <uc:ajaxpopup Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="RemotePdfStamp" runat="server" Url="../popup/RemotePdfStamp.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="700" Height="400" CloseFunction="function (event, ui) {__doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="MassiveReportDragAndDrop" runat="server" Url="../popup/MassiveReportDragAndDrop.aspx"
        Width="700" Height="500" PermitClose="true" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('upPnlButtons', 'POPUP_DRAG_AND_DROP');}" />
    <uc:ajaxpopup Id="AddressBookFromPopup" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui){__doPostBack('panelButtons', 'closePopupAddressBook');}" />
    <div id="containerTop">
        <asp:UpdatePanel ID="UpUserControlHeaderDocument" runat="server" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <uc2:HeaderDocument runat="server" ID="HeaderDocument" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpcontainerDocumentTabLeftBorder" runat="server" UpdateMode="Conditional"
            ClientIDMode="static">
            <ContentTemplate>
                <div id="containerDocumentTab" runat="server" clientidmode="Static">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpContainerDocumentTab" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc4:DocumentTabs runat="server" PageCaller="ATTACHMENTS" ID="DocumentTabs"></uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="ATTACHMENTS" />
                        </div>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTabDxBorder" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerDocumentTabDxBorder" runat="server" clientidmode="Static">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="container" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight" id="AttachmentsObject" runat="server"></span>
                                        </div>
                                    </div>
                                    <div id="row_object" class="row">
                                        <asp:Literal ID="litObject" runat="server" /></div>
                                </fieldset>
                            </div>
                            <div class="row" id="RowAttachmentsStd1" runat="server">
                                <fieldset>
                                    <asp:UpdatePanel ID="rblFilter_row" class="row" runat="server">
                                        <ContentTemplate>
                                            <asp:Label ID="lblFilter" runat="server" /><br />
                                            <asp:RadioButtonList ID="rblFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblFilter_SelectedIndexChanged"
                                                RepeatDirection="Horizontal">
                                                <asp:ListItem ID="AttachmentsAll" Value="all" runat="server"></asp:ListItem>
                                                <asp:ListItem ID="AttachmentsUser" Value="user" Selected="True" runat="server"></asp:ListItem>
                                                <asp:ListItem ID="AttachmentsPec" Value="pec" runat="server"></asp:ListItem>
                                                <asp:ListItem ID="AttachmentsPitre" Value="SIMPLIFIEDINTEROPERABILITY" runat="server"></asp:ListItem>
                                                <asp:ListItem ID="AttachmentsExternalSystem" Value="esterni" runat="server"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                            <asp:UpdatePanel ID="panelAllegati" runat="server" class="row" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:GridView ID="grdAllegati" runat="server" Width="98%" AutoGenerateColumns="False"
                                        AllowPaging="True" CssClass="tbl_rounded round_onlyextreme" PageSize="10" BorderWidth="0"
                                        OnRowDataBound="grdAllegati_RowDataBound" OnRowCommand="grdAllegati_RowCommand"
                                        OnPageIndexChanging="grdAllegati_PageIndexChanging">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:BoundField DataField="versionLabel" ItemStyle-CssClass="grdAllegati_code" runat="server"
                                                HeaderText='<%$ localizeByText:AttachmentsCod%>' />
                                            <asp:TemplateField ItemStyle-CssClass="grdAllegati_date" runat="server" HeaderText='<%$ localizeByText:AttachmentsDate%>'>
                                                <ItemTemplate>
                                                    <%# NttDataWA.Utils.dateformat.dateLength(Eval("dataInserimento").ToString())%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="descrizione" ItemStyle-CssClass="grdAllegati_description"
                                                runat="server" HeaderText='<%$ localizeByText:AttachmentsDescription%>' />
                                            <asp:BoundField DataField="numeroPagine" ItemStyle-CssClass="grdAllegati_pages" runat="server"
                                                HeaderText='<%$ localizeByText:AttachmentsPage%>' />
                                            <asp:TemplateField ItemStyle-CssClass="grdAllegati_icon">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="btnNavigateDocument" runat="server" CommandName="NavigateDocument">
                                                    </cc1:CustomImageButton>
                                                    <asp:HiddenField ID="attachId" runat="server" Value='<%#Eval("versionId").ToString() %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="HiddenRemoveAttachment" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row" id="RowModifyAttach1" runat="server">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <span class="weight" id="SpanAttachModify" runat="server">
                                                <asp:Label ID="AttachmentLblModify" runat="server"></asp:Label>
                                            </span><br />
                                            &nbsp;<br />
                                        </div>
                                    </div>
                                    <div id="Div1" class="row">
                                        <p id="rowDescription" runat="server">
                                            <strong><asp:Label ID="AttachmentLblModifyDescription" runat="server"></asp:Label></strong><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                                runat="server" ErrorMessage="<br />Campo obbligatorio" ControlToValidate="AttachmentDescription"
                                                EnableClientScript="false" ForeColor="Red"></asp:RequiredFieldValidator><br />
                                            <cc1:CustomTextArea ID="AttachmentDescription" runat="server" Columns="50" Rows="4"
                                                TextMode="MultiLine" ClientIDMode="Static" CssClass="txt_textarea" MaxLength="2000"></cc1:CustomTextArea><br />
                                            <%--<span class="col-right">Caratteri rimanenti: <span id="AttachmentDescription_chars"></span></span>--%>
                                        </p>
                                        <p id="rowPagesCount" runat="server">
                                            <strong><asp:Label ID="AttachmentsLblModifyPagesNumber" runat="server"></asp:Label></strong><br />
                                            <cc1:CustomTextArea ID="AttachmentPagesCount" MaxLength="5" runat="server" ClientIDMode="Static"
                                                Columns="5" CssClass="txt_textdata"></cc1:CustomTextArea><br />
                                        </p>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <uc5:ViewDocument ID="ViewDocument" runat="server" PageCaller="ATTACHMENTS"></uc5:ViewDocument>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
    <div id="UploadLiveuploads"  runat="server" Visible="false">
        <div class="upload-dialog" id="upload-liveuploads" data-bind="template: { name: 'template-uploads' }"></div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="panelButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="AttachmentsBtnAdd" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Nuovo" ClientIDMode="Static"  OnClick="AttachmentsAdd_Click" />
            <cc1:CustomButton ID="AttachmentsBtnModify" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Modifica" ClientIDMode="Static" OnClick="AttachmentsBtnModify_Click" />
            <%--OnClientClick="return ajaxModalPopupAttachmentsModify();" --%>
            <cc1:CustomButton ID="AttachmentsBtnSwap" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Scambia" OnClick="AttachmentsBtnSwap_Click" ClientIDMode="Static" />
            <cc1:CustomButton ID="AttachmentsBtnRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Rimuovi" ClientIDMode="Static" OnClick="AttachmentsBtnRemove_Click"  />
            <cc1:CustomButton ID="AttachmentsBtnAnnulla" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" Text="Annulla" ClientIDMode="Static"
                OnClick="AttachmentsBtnAnnulla_Click" />
            <!--[if !IE]><!-->
            <cc1:CustomButton ID="AttachmentsBtnUploadAllegati" Enabled="false" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static" Text="Acquisisci Allegati" OnClick="AttachmentsBtnUploadAllegati_Click" />
        <!--<![endif]-->
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/html" id="template-uploads">
            <div class="upload-dialog-title-container" data-bind="visible: inUpload()">
                <div class="upload-dialog-title">
                    <div class="upload-dialog-title-uploaded">
                        <span>File caricati</span>
                    </div>
                </div>
            </div>
            <div data-bind="visible: showTotalProgress()">
                <div>
                    <span data-bind="text: uploadSpeedFormatted()"></span>
                    <span data-bind="text: timeRemainingFormatted()" style="float: right;"></span>
                </div>
                <div class="upload-totalprogress">
                    <div class="upload-totalprogressbar" style="width: 0%;" data-bind="style: { width: totalProgress() + '%' }"></div>
                </div>
            </div>
            <div data-bind="visible: inUpload()" >
                <div class="upload-dialog-list" data-bind="foreach: uploads">
                    <div class="upload-upload">
                        <div class="upload-fileinfo upload-dialog-row-fluid">
                            <div class="upload-dialog-col-md-10" style="margin-top: 8px;">
                                <strong data-bind="text: fileName"></strong>
                                <span data-bind="text: fileSizeFormated"></span>
                                <span class="upload-progresspct" data-bind="visible: uploadProgress() < 100"><span data-bind="    text: uploadSpeedFormatted()"></span></span>
                            </div>
                            <div class="upload-dialog-col-md-2">
                                <div class="upload-uploadcompleted" data-bind="visible: uploadCompleted()">
                                    <div class="upload-uploadsuccessful" data-bind="visible: uploadSuccessful()"></div>
                                    <div class="upload-uploadfailed" data-bind="visible: !uploadSuccessful()"></div>
                                </div>
                            </div>
                        </div>
                        <div class="upload-progress">
                            <div class="upload-progressbar" style="width: 0%;" data-bind="style: { width: uploadProgress() + '%' }, visible: !uploadCompleted()"></div>
                        </div>
                    </div>
                </div>
            </div>
    </script>
    <%--<script type="text/javascript" data-main="<%=Page.ResolveClientUrl("~/Scripts/AttachmentsDragAndDrop.js") %>" src="<%=Page.ResolveClientUrl("~/Scripts/require.js") %>"></script>--%>
    <script type="text/javascript">
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function (s, e) {
            AttachmentsDragAndDropMain();
        });
    </script>
</asp:Content>
