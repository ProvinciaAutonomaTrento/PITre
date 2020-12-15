<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderProject.ascx.cs"
    Inherits="NttDataWA.UserControls.HeaderProject" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<uc:ajaxpopup2 Id="HistoryPreserved" runat="server" Url="../popup/HistoryPreserved.aspx?typeObject=F"
    PermitClose="false" PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui) {__doPostBack('UpContainer', '');}" />
<uc:ajaxpopup2 Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx?type=F"
    PermitClose="true" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('UpdPnlObject', '');}" />
<uc:ajaxpopup2 Id="Phases" runat="server" Url="../popup/Phases.aspx"
    PermitClose="true" PermitScroll="false" Width="550" Height="400" CloseFunction="function (event, ui) { __doPostBack('UpHiddenField', '');}" />
<uc:ajaxpopup2 Id="MissingRoles" runat="server" Url="../popup/MissingRoles.aspx"
    IsFullScreen="true" PermitClose="false" PermitScroll="true"  CloseFunction="function (event, ui) { __doPostBack('UpHiddenField', '');}" />
<div id="containerDocumentTop">
    <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="containerProjectTop">
                <div id="containerProjectTopSx">
                    <p>
                        <strong>
                            <asp:Label runat="server" ID="projectLblCodice"></asp:Label></strong><span class="redWeight"><asp:Label
                                runat="server" ID="projectLblCodiceGenerato"></asp:Label></span></p>
                </div>
                <div id="containerProjectTopCx">
                    <ul>
                        <li><strong>
                            <asp:Label runat="server" ID="projectLblTitolario"></asp:Label></strong>
                            <asp:Label runat="server" ID="projectLblTitolarioGenerato"></asp:Label>
                        </li>
                        <li><strong>
                            <asp:Label runat="server" ID="projectlblClassifica"></asp:Label></strong>
                            <asp:Label runat="server" ID="projectLblClassificaGenerato" CssClass="clickable"></asp:Label></li>
                    </ul>
                </div>
                <div id="containerProjectTopDx">
                </div>
            </div>
            <div id="containerProjectBottom" runat="server" clientidmode="Static">
                <div id="containerProjectCxBottom">
                    <div id="containerProjectCxBottomSx">
                        <ul>
                            <li>
                                <asp:PlaceHolder runat="server" ID="phNeutro" Visible="false">
                                    <cc1:CustomImageButton runat="server" ID="cimgbttIsCopy" ImageUrl="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                        OnMouseOutImage="../Images/Common/DepositoCopiaDocumentoSmall.png" OnMouseOverImage="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                        CssClass="clickable" ToolTip="Documento Copiato in Deposito" ImageUrlDisabled="../Images/Common/DepositoCopiaDocumentoSmall.png"
                                        Enabled="false" />
                                </asp:PlaceHolder>
                            </li>
                            <li><strong>
                                <asp:Label runat="server" ID="projectLblId"></asp:Label></strong> <span class="anchorType">
                                    <a href="#anchorTop">
                                        <asp:Label ID="projectLblIdGenerato" runat="server"></asp:Label></a></span></li>
                            <li><strong>
                                <asp:Label runat="server" ID="projectlblStato"></asp:Label></strong><span class="open"><asp:Label
                                    runat="server" ID="projectLblStatoGenerato"></asp:Label></span> </li>
                            <li><strong>
                                <asp:Label runat="server" ID="projectLblRegistroSelezionato"></asp:Label></strong>
                                <asp:Label runat="server" ID="projectLblRegistroGenerato"></asp:Label>
                            </li>
                            <li><strong>
                                <asp:Label runat="server" ID="ProjectLitTypeDocumentHead" Visible="false"></asp:Label></strong>
                                <span class="anchorType"><a href="#anchorType">
                                    <asp:Label runat="server" ID="ProjectLitTypeDocumentValue" Visible="false"></asp:Label></a></span></li>
                        </ul>
                    </div>
                    <div id="containerProjectCxBottomDx">
                        <div id="containerProjectTopCxOrangeDxDx">
                            <ul>
                                <li>
                                    <cc1:CustomImageButton ID="projectImgStampaEtichette" ImageUrl="../Images/Icons/print_label.png"
                                        runat="server" OnMouseOverImage="../Images/Icons/print_label_hover.png" OnMouseOutImage="../Images/Icons/print_label.png"
                                        CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/print_label_disabled.png"
                                        Enabled="false" OnClick="ProjectImgStampaEtichette_Click" />
                                    <li>
                                        <cc1:CustomImageButton ID="projectImgConservazione" ImageUrl="../Images/Icons/preservation_story.png"
                                            runat="server" OnMouseOverImage="../Images/Icons/preservation_story_hover.png"
                                            OnMouseOutImage="../Images/Icons/preservation_story.png" CssClass="clickableLeft"
                                            ImageUrlDisabled="../Images/Icons/preservation_story_disabled.png" Enabled="false"
                                            OnClientClick="return ajaxModalPopupHistoryPreserved();" /></li>
                                </li>
                            </ul>
                        </div>
                    </div>         
                    <div style="float:left">
                        <asp:UpdatePanel ID="UpnlFasiDiagrammaStato" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:Panel ID="pnlFasiDiagrammaStato" runat="server" Visible="false">
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpHiddenField" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                            <ContentTemplate>
                                <asp:HiddenField ID="HiddenFaseDiagramma" ClientIDMode="Static" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                     </div>    
                     <div id="containerProjectTabDxBorder">
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
