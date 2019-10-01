<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderDocument.ascx.cs"
    Inherits="NttDataWA.UserControls.HeaderDocument" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<uc:ajaxpopup2 Id="HistoryPreserved" runat="server" Url="../popup/HistoryPreserved.aspx?typeObject=D"
    PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpContainer', '');}" />
<uc:ajaxpopup2 Id="HistoryVers" runat="server" Url="../popup/HistoryVers.aspx" PermitClose="false"
    PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpContainer', '');}" />
<uc:ajaxpopup2 Id="InfoSignatureProcessesStarted" runat="server" Url="../popup/InfoSignatureProcessesStarted.aspx" PermitClose="false"
    PermitScroll="false" Width="600" Height="300" CloseFunction="function (event, ui) {__doPostBack('UpContainer', '');}" />
<%--<uc:ajaxpopup2 Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx" PermitClose="true"
    PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />--%>
<div id="containerDocumentTop">
    <asp:UpdatePanel ID="UpLetterTypeProtocol" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="PnlcontainerDocumentTopSx" runat="server">
                <p runat="server" id="PProtocolType">
                </p>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTopCx" UpdateMode="Conditional"
        ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerDocumentTopCxOrange" runat="server" clientidmode="Static">
                <div id="containerDocumentTopCxOrangeSx">
                    <div class="rowTop2">
                        <div class="colTop">
                            <asp:PlaceHolder runat="server" ID="phNeutro" Visible="false">
                                <cc1:CustomImageButton runat="server" ID="cimgbttIsCopy" ImageUrl="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                    OnMouseOutImage="../Images/Common/DepositoCopiaDocumentoSmall.png" OnMouseOverImage="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                    CssClass="clickable" ToolTip="Documento Copiato in Deposito" ImageUrlDisabled="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                    Enabled="false" />
                            </asp:PlaceHolder>
                        </div>
                        <div class="colTop">
                            <strong>
                                <asp:Label ID="DocumentLblDocumentId" runat="server" Visible="false"></asp:Label></strong>
                            <span class="anchorType"><a href="#anchorTop">
                                <asp:Label ID="LblIdDocument" runat="server" Visible="false"></asp:Label></a></span>
                        </div>
                        <div class="colTop">
                            <strong>
                                <asp:Label ID="DocumentLblReferenceCodeLabel" runat="server" Visible="false"></asp:Label></strong>
                            <span class="redWeight"><span class="anchorTypeRed"><a href="#anchorTop">
                                <asp:Label ID="LblReferenceCode" runat="server" Visible="false"></asp:Label></a></span></span>
                        </div>
                        <div class="colTop">
                            <strong>
                                <asp:Label ID="DocumentLblRepertoryCodeLabel" runat="server" Visible="false"></asp:Label></strong>
                            <asp:Label ID="LblRepertory" runat="server" Visible="false"></asp:Label>
                        </div>
                    </div>
                    <div class="rowTop3">
                        <div class="colTop">
                            <strong>
                                <asp:Literal ID="DocumentLitRepertory" runat="server" Visible="false"></asp:Literal></strong>
                            <span class="redWeight">
                                <asp:Label ID="DocumentLitRepertoryValue" runat="server" Visible="false"></asp:Label></span>
                        </div>
                        <div class="colTop">
                            <strong>
                                <asp:Literal runat="server" ID="DocumentLitTypeDocumentHead" Visible="false"></asp:Literal>
                            </strong><span class="anchorType"><a href="#anchorType">
                                <asp:Literal runat="server" ID="DocumentLitTypeDocumentValue" Visible="false"></asp:Literal></a></span>
                        </div>
                    </div>
                </div>
                <div id="containerDocumentTopCxOrangeDx">
                    <div id="containerDocumentTopCxOrangeDxSx">
                        <p>
                            <asp:PlaceHolder runat="server" ID="PlcDocumentPrintLabel">
                                <asp:Label runat="server" ID="DocumentLblPrintLabel"></asp:Label>&nbsp;
                                <cc1:CustomTextArea ID="TxtPrintLabel" Text="" runat="server" CssClass="txtLabelPrint"
                                    CssClassReadOnly="txtLabelPrintReadonly" Enabled="false" ClientIDMode="static">
                                </cc1:CustomTextArea>
                            </asp:PlaceHolder>
                        </p>
                    </div>
                    <div id="containerDocumentTopCxOrangeDxDx">
                        <asp:UpdatePanel runat="server" ID="UpPnlButtonsHeader" UpdateMode="Conditional"
                            ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:HiddenField ID="ConsolidateMetadata" runat="server" ClientIDMode="Static" />
                                <ul>
                                    <li>
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgPrintLabel" ImageUrl="../Images/Icons/print_label.png"
                                            OnMouseOutImage="../Images/Icons/print_label.png" OnMouseOverImage="../Images/Icons/print_label_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/print_label_disabled.png"
                                            Enabled="false" OnClick="DocumentImgPrintLabel_Click" />
                                    </li>
                                    <li>
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgPrintA4" ImageUrl="../Images/Icons/print_a4.png"
                                            OnClick="DocumentImgPrintA4_Click" OnMouseOutImage="../Images/Icons/print_a4.png"
                                            OnMouseOverImage="../Images/Icons/print_a4_hover.png" CssClass="clickable" ImageUrlDisabled="../Images/Icons/print_a4_disabled.png"
                                            Enabled="false" />
                                    </li>
                                    <li>
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgPrintReceipt" ImageUrl="../Images/Icons/print_receipt.png"
                                            OnMouseOutImage="../Images/Icons/print_receipt.png" OnClick="DocumentImgPrintReceipt_Click"
                                            OnMouseOverImage="../Images/Icons/print_receipt_hover.png" CssClass="clickable"
                                            ImageUrlDisabled="../Images/Icons/print_receipt_disabled.png" Visible="false" />
                                    </li>
                                    <li>
                                        <asp:Image ID="imgSeparator6" runat="server" ImageUrl="~/Images/Icons/separatore.png"
                                            alt="" />
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgSendReceipt" ImageUrl="../Images/Icons/send_receipt.png"
                                            OnMouseOutImage="../Images/Icons/send_receipt.png" OnMouseOverImage="../Images/Icons/send_receipt_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/send_receipt_disabled.png"
                                            OnClick="DocumentImgSendReceipt_Click" Enabled="false" />
                                    </li>
                                    <li>
                                        <asp:Image ID="imgSeparator7" runat="server" ImageUrl="~/Images/Icons/separatore.png"
                                            alt="" />
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgPreservation" ImageUrl="../Images/Icons/preservation_story.png"
                                            OnMouseOutImage="../Images/Icons/preservation_story.png" OnMouseOverImage="../Images/Icons/preservation_story_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/preservation_story_disabled.png"
                                            Enabled="false" OnClientClick="return ajaxModalPopupHistoryPreserved();" />
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgRecupero" ImageUrl="../Images/Icons/preservation_story.png"
                                            OnMouseOutImage="../Images/Icons/preservation_story.png" OnMouseOverImage="../Images/Icons/preservation_story_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/preservation_story_disabled.png"
                                            Enabled="false" OnClientClick="return ajaxModalPopupHistoryVers();" />
                                    </li>
                                    <li>
                                        <asp:Image ID="imgSeparator8" runat="server" ImageUrl="~/Images/Icons/separatore.png"
                                            alt="" />
                                        <cc1:CustomImageButton runat="server" ID="DocumentImgInfoProcessiAvviati" ImageUrl="../Images/Icons/LibroFirma/InfoProcessiAvviati.png"
                                            OnMouseOutImage="../Images/Icons/LibroFirma/InfoProcessiAvviati.png" OnMouseOverImage="../Images/Icons/LibroFirma/InfoProcessiAvviati_hover.png"
                                            CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/InfoProcessiAvviati_disabled.png"
                                            Enabled="false" OnClientClick="return ajaxModalPopupInfoSignatureProcessesStarted();" />
                                    </li>
                                </ul>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UpcontainerDocumentTopDx" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="containerDocumentTopDx" runat="server" ClientIDMode="Static">
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<script type="text/javascript">
    function PrintReceipt() {
        var filePath;
        var exportUrl;
        var http;
        var applName;
        var fso;
        var folder;
        var path;
        try {
            fso = appletFso();
            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
            folder = fso.GetSpecialFolder(2);
            alert('folder:' + folder);
            path = folder.Path;
            alert('path:' + path);
            filePath = path + "\\export.doc";
            applName = "Microsoft Word";
            //exportUrl= "..\\exportDati\\exportDatiPage.aspx";				
            http = CreateObject("MSXML2.XMLHTTP");
            http.Open("POST", exportUrl, false);
            http.send();


            var content = http.responseBody;
            alert('content:' + content);
            if (content != null) {
                AdoStreamWrapper_SaveBinaryData(filePath, content);

                ShellWrappers_Execute(filePath);
            }
        }
        catch (ex) {
            alert(ex.message.toString());
        }

        return false;
    }
</script>
