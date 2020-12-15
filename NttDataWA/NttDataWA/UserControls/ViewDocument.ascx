<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewDocument.ascx.cs"
    Inherits="NttDataWA.UserControls.ViewDocument" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:UpdatePanel ID="UpBottomButtons" runat="server" UpdateMode="Conditional" ClientIDMode="static">
    <ContentTemplate>
        <asp:Panel ID="BottomButtons" runat="server">
            <!--Visible="false"-->
            <ul class="ulBottomButtons">
                <li class="liBottomButtons5">
                    <asp:Image ID="imgInfoVersionSelected" runat="server" ImageUrl="~/Images/Icons/info_1.png"
                        onmouseover="src = src.replace('.png', '_hover.png');" onmouseout="src = src.replace('_hover.png', '.png');"
                        AlternateText="" ToolTip="" CssClass="clickable" />
                </li>
                <li class="liBottomButtons5">
                    <cc1:CustomImageButton runat="server" ID="DocumentImgIdentityCard" ImageUrl="../Images/Icons/ico_verificata.png"
                        OnMouseOutImage="../Images/Icons/ico_verificata.png" OnMouseOverImage="../Images/Icons/ico_verificata.png"
                        CssClass="clickable" ImageUrlDisabled="../Images/Icons/ico_spenta.png" Visible="false"
                        OnClientClick="return ajaxModalPopupInformationFile();" /></li>
                <li class="liBottomButtons1"><asp:Literal ID="LitDocumentSize" runat="server"></asp:Literal></li>
                <li class="liBottomButtons2"><asp:Literal ID="LitDocumentSignature" runat="server"></asp:Literal></li>
                <li class="liBottomButtons3"><asp:Literal ID="LitDocumentBlocked" runat="server"></asp:Literal></li>
                <li class="liBottomButtons4"><asp:Literal ID="LitDocumentConversionPDF" runat="server"></asp:Literal></li>
            </ul>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="UpdatePanelPreview" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                <ContentTemplate>  
                    <asp:PlaceHolder ID="PlaceHolderPreview" runat="server" Visible="false">
                        
                            <div class="row2" style="margin: 0;  width: 83%;">
                                <fieldset style="padding-right:0px;">
                                <!--div class="col2" style=" width: 28%; padding-bottom:3px;">
                                    
                                    <%--<asp:HiddenField runat="server" ID="HiddenFieldFromPage" ClientIDMode="Static" />
                                    <asp:HiddenField runat="server" ID="HiddenFieldLastPage" ClientIDMode="Static" />--%>
                                </div-->
                                <div class="col2" style=" width: 100%; padding-bottom:3px;">
                                   
                                            <ul>
                                                <li style="float: left; margin: 1px; width: 30%;">
                                                    <cc1:CustomImageButton runat="server" ID="imgPrevPreview" ImageUrl="../Images/Preview/PreviousPdf.png"
                                                        OnMouseOutImage="../Images/Preview/PreviousPdf.png" OnMouseOverImage="../Images/Preview/PreviousPdf_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Preview/PreviousPdf_disabled.png" Visible="false"
                                                        OnClick="btnPrev_Click" />
                                                    <%--<asp:Literal ID="repDocumentVersions_sep"
                                                            runat="server"> |</asp:Literal>--%>
                                                </li>
                                                <li style="float: left; margin: 1px;">
                                                    <span id="ViewDocumentPreview" class="weight" style="float: left; margin: 1px;" runat="server"></span>
                                                </li>
                                                <li style="float: right; margin: 1px; width: 30%;">
                                                    <cc1:CustomImageButton runat="server" ID="imgNextPreview" ImageUrl="../Images/Preview/NextPdf.png"
                                                        OnMouseOutImage="../Images/Preview/NextPdf.png" OnMouseOverImage="../Images/Preview/NextPdf_hover.png"
                                                        CssClass="clickable" ImageUrlDisabled="../Images/Preview/NextPdf_disabled.png" Visible="false"
                                                        OnClick="btnSucc_Click" />
                                                </li>
                                            </ul>
                                </div>
                                </fieldset>
                            </div>
                        
                    </asp:PlaceHolder>
                  </ContentTemplate>
            </asp:UpdatePanel>
<asp:UpdatePanel runat="server" ID="UpPnlContentDxSx" UpdateMode="Conditional">
    <ContentTemplate>
        <div id="contentDxSx" runat="server" clientidmode="Static" class="contentDxSx">
            <asp:UpdatePanel runat="server" ID="UpPnlDocumentNotAcquired">
                <ContentTemplate>
                    <fieldset id="docNotAcquisition" style="border: 0;">
                        <h4 class="vcenter">
                            <asp:Label ID="ViewDocumentLblNoAcquired" runat="server"></asp:Label></h4>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpPnlDocumentAcquired" Visible="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <fieldset id="docAcquisition" style="border: 0;">
                        <h4>
                            <asp:Label ID="ViewDocumentLblAcquired" runat="server"></asp:Label></h4>
                        <h5>
                            <img id="ViewDocumentImageDocumentAcquired" alt="" runat="server" /></h5>
                        <h6>
                            <asp:LinkButton ID="ViewDocumentLinkFile" runat="server" OnClick="LinkViewFileDocument"
                                Font-Size="1.5em"></asp:LinkButton></h6>
                    </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpPnlDocumentData" runat="server" UpdateMode="Conditional" ClientIDMode="Static"
                Visible="false">
                <ContentTemplate>
                    <div id="divFrame" class="row" runat="server">
                        <fieldset>
                            <div class="row" style="background-color: #fff;">
                                <iframe width="100%" frameborder="0" marginheight="0" marginwidth="0" id="frame"
                                    runat="server" clientidmode="Static" style="z-index: 0;"></iframe>
                            </div>
                        </fieldset>
                    </div>
                    <asp:PlaceHolder ID="PlcVersions" runat="server">
                        <div class="row2" style="margin: 0; padding-left: 10px;">
                            <div class="col2" style="padding-top: 5px">
                                <span id="ViewDocumentTxtVersions" class="weight" style="float: left; margin: 1px;"
                                    runat="server"></span>:
                            </div>
                            <div class="col2" style="padding-top: 5px; width: 500px;">
                                <asp:Repeater ID="repDocumentVersions" runat="server" OnItemDataBound="repDocumentVersions_Binding">
                                    <HeaderTemplate>
                                        <ul>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li style="float: left; margin: 1px;">
                                            <asp:Button ID="btnVersion" runat="server" OnClick="btnVersion_Click" CssClass="btnDisableVersion"
                                                Style="width: 30px; text-align: center;"></asp:Button>
                                            <asp:Literal ID="repDocumentVersions_sep" runat="server"> |</asp:Literal></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div id="divNumberVersion" runat="server">
                                <div id="divButtonVersion" runat="server" class="col-right-no-margin1">
                                    <cc1:CustomImageButton ID="btnAddVersion" runat="server" CssClass="clickableLeft"
                                         OnClick="AddVersion_Click" />
                                    <cc1:CustomImageButton ID="btnModifyVersion" runat="server" CssClass="clickableLeft"
                                        OnClientClick="__doPostBack('UpBottomButtons', 'POPUP_MODIFY_VERSION');return ajaxModalPopupModifyVersion();" />
                                    <cc1:CustomImageButton ID="btnRemoveVersion" runat="server" OnClientClick=" __doPostBack('UpBottomButtons', 'CONFIRM_REMOVE_VERSION'); return false;"
                                        CssClass="clickableLeft" />
                                </div>
                                <div class="row2">
                                    <div class="col2" style="margin: 0; padding-left: 5px">
                                        <span id="ViewDocumentCreatore" class="weight" runat="server"></span>:
                                        <asp:Literal ID="litCreatore" runat="server" />
                                    </div>
                                    <div class="col2" style="margin: 0; padding-left: 15px">
                                        <span id="ViewDocumentTxtDate" class="weight" runat="server"></span>:
                                        <asp:Literal ID="litDocumentDate" runat="server" />
                                    </div>
                                </div>
                                <asp:UpdatePanel runat="server" ID="UpdateInfoFileAquired" Visible="true" ClientIDMode="Static" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row2">
                                            <div class="col2" style="margin: 0; padding-left: 5px">
                                                <span id="ViewDocumentAuthorFile" class="weight" runat="server"></span>:
                                                <asp:Literal ID="litAuthorFile" runat="server" />
                                            </div>
                                            <div class="col2" style="margin: 0; padding-left: 15px">
                                                <span id="ViewDocumentTxtDateAcquired" class="weight" runat="server"></span>:
                                                <asp:Literal ID="litDocumentDateAcqured" runat="server" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="row2">
                                    <div class="col2" style="margin: 0; padding-left: 5px">
                                        <span id="ViewDocumentTxtPapery" class="weight" runat="server"></span>:
                                        <asp:Literal ID="litDocumentPapery" runat="server" />
                                    </div>
                                </div>
                                <asp:UpdatePanel runat="server" ID="UpdateLitDocumentVersion" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row2">
                                            <div class="col2" style="margin: 0; padding-left: 5px">
                                                <span id="ViewDocumentTxtNoteVersion" class="weight" runat="server"></span>:
                                                <asp:Literal ID="litDocumentVersion" runat="server" />
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:HiddenField runat="server" ID="HiddenRemoveVersion" ClientIDMode="Static" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel runat="server" ID="UpPnlContentDxDx" UpdateMode="Conditional" ClientIDMode="Static"
    style="float: right; padding-left:5px;">
    <ContentTemplate>
        <asp:Literal ID="debug" runat="server" />
        <asp:GridView ID="GrdDocumentAttached" runat="server" AutoGenerateColumns="false"
            ShowHeader="false" OnRowCommand="GrdDocumentAttached_RowCommand" CssClass="tabViewDocument"
            Visible="false">
            <SelectedRowStyle CssClass="selectedrowAttachment" VerticalAlign="Bottom" />
            <Columns>
                <asp:TemplateField ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Image ID="btnImageSelected" runat="server" ImageUrl="../Images/Icons/item_selected.png" ImageAlign="Middle"
                            Visible="<%#IsElementoSelezionatoInLf((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"/>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <cc1:CustomImageButton ID="btnVisualizza" name="btnVisualizza" runat="server" Enabled="<%#IsAcquired((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                            ImageUrl="<%#GetVersionImage((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                            CssClass="clickable-no-ie" CommandName="ShowVersion" ImageAlign="Middle" ToolTip="<%# GetLabelTooltip((NttDataWA.DocsPaWR.FileRequest) Container.DataItem) %>"
                            OnMouseOutImage="<%#GetVersionImage((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                            OnMouseOverImage="<%#GetVersionImage((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                            ImageUrlDisabled="<%#GetVersionImage((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Label ID="lblDescrizione" runat="server" Text="<%# GetLabel((NttDataWA.DocsPaWR.FileRequest) Container.DataItem) %>"
                            CssClass="nameDocument">
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="btnVersioneStampabile" NavigateUrl="<%#GetLinkVersionID((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>" Visible="<%#IsAcquired((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>" CssClass="clickableLeft-no-ie download" ToolTip="<%#GetTooltipPrintableVersion()%>" runat="server" ImageUrl="~/Images/Icons/print_label.png" OnClientClick="alert('cass');"></asp:HyperLink>

                        <%-- <cc1:CustomImageButton ID="btnVersioneStampabile" runat="server" Visible="<%#IsAcquired((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>"
                            ImageUrl="~/Images/Icons/print_label.png" ImageAlign="Middle" ToolTip="<%#GetTooltipPrintableVersion()%>"
                            CssClass="clickableLeft-no-ie download" CommandName="ShowPrintableVersion" OnMouseOutImage="../Images/Icons/print_label.png"
                            OnMouseOverImage="../Images/Icons/print_label_hover.png"  />--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HiddenField Value="<%#GetVersionID((NttDataWA.DocsPaWR.FileRequest) Container.DataItem)%>" ID="AttachVersionId" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:GridView ID="GridAttachNoUser" runat="server" ClientIDMode="Static" AutoGenerateColumns="false" ShowHeader="false" CssClass="tabViewDocument"
            OnRowCommand="GridAttachNoUser_RowCommand">
            <SelectedRowStyle CssClass="selectedrowAttachment" VerticalAlign="Bottom" />
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <div>
                            <div>
                                <cc1:CustomImageButton ID="btnAttachNoUser" runat="server" CssClass="clickable" ImageAlign="Middle"
                                    CommandName="ShowAttachedSelected" CommandArgument="<%# ((AttachNouser)Container.DataItem).TypeAttachment%>"
                                    ImageUrl="<%# GetAttachNoUserImage((AttachNouser)Container.DataItem)%>"
                                    ToolTip="<%# GetAttachNoUserTooltip((AttachNouser)Container.DataItem) %>"
                                    AlternateText="<%# GetAttachNoUserTooltip((AttachNouser)Container.DataItem) %>"
                                    OnMouseOutImage="<%# GetAttachNoUserImage((AttachNouser)Container.DataItem)%>"
                                    OnMouseOverImage="<%# GetAttachNoUserImage((AttachNouser)Container.DataItem)%>" />
                            </div>
                            <%-- OnClientClick="<%# ScriptClickAttachNoUser((AttachNouser) Container.DataItem)%>" --%>
                            <div class="countAttachNoUser">
                                <p style="padding: 0px; margin: 0px; text-align: center;">
                                    <%# GetCountAttachNoUser((AttachNouser) Container.DataItem)%>
                                </p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Button ID="BtnAttachExt" runat="server" CssClass="hidden" ClientIDMode="Static" OnClick="BtnAttachExt_Click" />
    </ContentTemplate>
</asp:UpdatePanel>
