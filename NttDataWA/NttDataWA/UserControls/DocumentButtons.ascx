<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentButtons.ascx.cs"
    Inherits="NttDataWA.UserControls.DocumentButtons"  %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutPanel" Src="../CheckInOut/CheckInOutPanel.ascx" %>
    <uc:ajaxpopup2 Id="Timestamp" runat="server" Url="../popup/Timestamp.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="600" Height="600" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
     <uc:ajaxpopup2 Id="InformationFile" runat="server" Url="../popup/InformationFile.aspx" IsFullScreen="false"
        PermitClose="false" PermitScroll="true" Width="800" Height="500" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
<asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional" ClientIDMode="Static">
    <ContentTemplate>
    <div style="float: left;">
        <cc1:CustomImageButton runat="server" ID="DocumentImgUploadFile" ImageUrl="../Images/Icons/upload_file.png"
            OnMouseOutImage="../Images/Icons/upload_file.png" OnMouseOverImage="../Images/Icons/upload_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/upload_file_disabled.png"
            OnClick="DocumentImgUploadFile_Click" Enabled="false" />
        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/img_space.png")%>" alt="">
        <cc1:CustomImageButton runat="server" ID="DocumentImgViewFile" ImageUrl="../Images/Icons/view_file.png"
            OnMouseOutImage="../Images/Icons/view_file.png" OnMouseOverImage="../Images/Icons/view_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/view_file_disabled.png" OnClick="DocumentImgViewFile_Click" 
            Enabled="false" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgZoomFile" ImageUrl="../Images/Icons/zoom.png"
            OnMouseOutImage="../Images/Icons/zoom.png" OnMouseOverImage="../Images/Icons/zoom_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/zoom_disabled.png" Enabled="false" OnClick="DocumentImgZoomFile_Click"/>
        <cc1:CustomImageButton runat="server" ID="DocumentImgSignaturePosition" ImageUrl="../Images/Icons/signature_position.png"
            OnMouseOutImage="../Images/Icons/signature_position.png" OnMouseOverImage="../Images/Icons/signature_position_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/signature_position_disabled.png"
            OnClick="DocumentImgSignaturePosition_Click" Enabled="false" />
        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/img_space.png")%>" alt="">
        <cc1:CustomImageButton runat="server" ID="DocumentImgSignature" ImageUrl="../Images/Icons/signature.png"
            OnMouseOutImage="../Images/Icons/signature.png" OnMouseOverImage="../Images/Icons/signature_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/signature_disabled.png"
            Enabled="false" OnClick="DocumentImgSignature_Click" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgCoSignature" ImageUrl="../Images/Icons/co_signature.png"
            OnMouseOutImage="../Images/Icons/co_signature.png" OnMouseOverImage="../Images/Icons/co_signature_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/co_signature_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupDigitalCosignSelector();" visible="false"/>
        <cc1:CustomImageButton runat="server" ID="DocumentImgVisureSign" ImageUrl="../Images/Icons/LibroFirma/doc_verified.png"
            OnMouseOutImage="../Images/Icons/LibroFirma/doc_verified.png" OnMouseOverImage="../Images/Icons/LibroFirma/doc_verified_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/doc_verified_disabled.png"
            Enabled="false" OnClick="DocumentImgVisureSign_Click" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgSignatureDetails" ImageUrl="../Images/Icons/signature_details.png"
            OnMouseOutImage="../Images/Icons/signature_details.png" OnMouseOverImage="../Images/Icons/signature_details_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/signature_details_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupDigitalSignDetails();" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgSignatureHSM" ImageUrl="../Images/Icons/signature_hsm.png"
            OnMouseOutImage="../Images/Icons/signature_hsm.png" OnMouseOverImage="../Images/Icons/signature_hsm_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/signature_hsm_disabled.png"
            Enabled="false" OnClick="DocumentImgSignatureHSM_Click" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgVerifyCrlSignature" ImageUrl="../Images/Icons/flag_ok.png"
            OnMouseOutImage="../Images/Icons/flag_ok.png" OnMouseOverImage="../Images/Icons/flag_ok_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/flag_ok_disabled.png"
            Enabled="false" visible="false" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgTimestamp" ImageUrl="../Images/Icons/timestamp.png"
            OnMouseOutImage="../Images/Icons/timestamp.png" OnMouseOverImage="../Images/Icons/timestamp_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/timestamp_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupTimestamp();" />
         <cc1:CustomImageButton runat="server" ID="DocumentImgStartProcessSignature" ImageUrl="../Images/Icons/LibroFirma/Start_Process.png"
            OnMouseOutImage="../Images/Icons/LibroFirma/Start_Process.png" OnMouseOverImage="../Images/Icons/LibroFirma/Start_Process_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/Start_Process_disabled.png"
            Enabled="true" OnClick="DocumentImgStartProcessSignature_Click" />
          <cc1:CustomImageButton runat="server" ID="DocumentImgProcessState" ImageUrl="../Images/Icons/LibroFirma/Process_State.png"
            OnMouseOutImage="../Images/Icons/LibroFirma/Process_State.png" OnMouseOverImage="../Images/Icons/LibroFirma/Process_State_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/LibroFirma/Process_State_disabled.png"
            Enabled="true" OnClientClick="return ajaxModalPopupDetailsLFAutomaticMode();" />
         <asp:Image ID="Image2" runat="server" ImageUrl="~/Images/Icons/img_space.png" alt="" />
    </div>
    
    <div id="appletSelector" style="float: left; <% if (componentType!=NttDataWA.Utils.Constans.TYPE_APPLET && componentType!=NttDataWA.Utils.Constans.TYPE_SOCKET) {%>display: none;<%} %>">
        <cc1:CustomImageButton runat="server" ID="DocumentImgSaveLocalFile" ImageUrl="../Images/Icons/save_local_file.png"
            OnMouseOutImage="../Images/Icons/save_local_file.png" OnMouseOverImage="../Images/Icons/save_local_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/save_local_file_disabled.png"
            Enabled="false"  OnClick="DocumentImgSaveLocalFile_Click" /> <!--OnClientClick="return ajaxModalPopupSaveDialog();"/>-->
        <asp:Image runat="server" ID="DocumentImgConsolidateStep1" CssClass="clickable" Visible="false"
            ImageUrl="~/Images/Icons/consolidate_step1.png" OnClientClick="return ajaxModalPopupCheckOutDocument();"/>
        <cc1:CustomImageButton runat="server" ID="DocumentImgLock" ImageUrl="../Images/Icons/lock.png"
            OnMouseOutImage="../Images/Icons/lock.png" OnMouseOverImage="../Images/Icons/lock_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/lock_disabled.png" 
            Enabled="false" OnClick="DocumentImgLock_Click" /> <!--OnClientClick="return ajaxModalPopupCheckOutDocument();"/>-->
        <cc1:CustomImageButton runat="server" ID="DocumentImgUnLock" ImageUrl="../Images/Icons/unlock.png"
            OnMouseOutImage="../Images/Icons/unlock.png" OnMouseOverImage="../Images/Icons/unlock_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/unlock_disabled.png" 
            Enabled="false" OnClick="DocumentImgUnLock_Click" /> <!--OnClientClick="return ajaxModalPopupCheckInDocument();"/>-->
        <cc1:CustomImageButton runat="server" ID="DocumentImgUnlockWithoutSave" ImageUrl="../Images/Icons/lock_no_save.png"
            OnMouseOutImage="../Images/Icons/lock_no_save.png" OnMouseOverImage="../Images/Icons/lock_no_save_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/lock_no_save_disabled.png"
            Enabled="false" OnClick="DocumentImgUnlockWithoutSave_Click" /> <!--OnClientClick="return ajaxModalPopupUndoCheckOut();"/>-->
        <cc1:CustomImageButton runat="server" ID="DocumentImgOpenFile" ImageUrl="../Images/Icons/open_file.png"
            OnMouseOutImage="../Images/Icons/open_file.png" OnMouseOverImage="../Images/Icons/open_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/open_file_disabled.png"
            Enabled="false" OnClick="DocumentImgOpenFile_Click" /> <!--onclientclick="return ajaxModalPopupOpenLocalCheckOutFile();"/>-->
        <cc1:CustomImageButton runat="server" ID="CheckOutShowStatus" ImageUrl="../Images/Icons/View_lock_status.png"
            OnMouseOutImage="../Images/Icons/View_lock_status.png" OnMouseOverImage="../Images/Icons/View_lock_status_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/View_lock_status_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupShowCheckOutStatus();"/>
        </div>
            <uc1:CheckInOutPanel ID="checkInOutPanel" runat="server" RelativeFolderPath="~/CheckInOut/" Visible="false"></uc1:CheckInOutPanel>
        <div style="float: left;">
        
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/Icons/img_space.png" alt="" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgOpenModelApplet" ImageUrl="../Images/Icons/model_file.png"
            OnMouseOutImage="../Images/Icons/model_file.png" OnMouseOverImage="../Images/Icons/model_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/model_file_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupCheckOutModelApplet();" Visible="false"/>
        <cc1:CustomImageButton runat="server" ID="DocumentImgOpenModelActiveX" ImageUrl="../Images/Icons/model_file.png"
            OnMouseOutImage="../Images/Icons/model_file.png" OnMouseOverImage="../Images/Icons/model_file_hover.png"
            CssClass="clickable" ImageUrlDisabled="../Images/Icons/model_file_disabled.png"
            Enabled="false" OnClientClick="return ajaxModalPopupCheckOutModelActiveX();" Visible="false"/>

        <asp:Image ID="imgSeparator6" runat="server" ImageUrl="~/Images/Icons/img_space.png" alt="" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgConvertPdf" ImageUrl="../Images/Icons/ico_pdf2.png"
            OnMouseOutImage="../Images/Icons/ico_pdf2.png" OnMouseOverImage="../Images/Icons/ico_pdf2_hover.png"
            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/ico_pdf2_disabled.png" OnClick="DocumentImgConvertPdf_Click" />
        <cc1:CustomImageButton runat="server" ID="DocumentImgInvoicePreviewPdf" ImageUrl="../Images/Icons/ico_pdf2.png"
            OnMouseOutImage="../Images/Icons/ico_pdf2.png" OnMouseOverImage="../Images/Icons/ico_pdf2_hover.png"
            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/ico_pdf2_disabled.png" OnClick="DocumentImgInvoicePreviewPdf_Click" />
    </div>
    </ContentTemplate>
</asp:UpdatePanel>
