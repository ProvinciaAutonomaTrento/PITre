<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="Signature.aspx.cs" Inherits="NttDataWA.Popup.Signature" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        $(function () {
            $("#TxtX").focus();
        });
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="containerSignature">

        <div class="contentSignatureTopSx">

            <div class="contentDocPreviewAndPosition"> 
                <asp:Label CssClass="NormalBold" ID="SignatureLblPositionInformation" runat="server" ></asp:Label><br />
                <div class="contentDocPreview">
                    <asp:UpdatePanel ID="UpdatePanelcontentSegnatureBtn" runat="server" UpdateMode="Conditional" ClientIDMode="Static" style=" width:100%; height:100%">
                         <ContentTemplate>
                            <div class="contentSegnatureBtn" style=" width:100%; height:99%">
                                <div class="contentSegnatureBtnTop">
                                    <cc1:CustomImageButton runat="server" ID="BtnPosLeft" style="float:left; width:35px" CssClass="clickable" OnClick="BtnSignaturePosition_Click"/>
                                    <cc1:CustomImageButton ID="BtnPosRight" style="float:right; width:35px" runat="server" CssClass="clickable" OnClick="BtnSignaturePosition_Click"/>                               
                                </div>
                                <div class="contentSegnatureBtnDown" style=" clear: left; margin:0px; padding-top:120%">
                                    <cc1:CustomImageButton ID="BtnDownSx" style="float:left;width:35px" runat="server" CssClass="clickable" OnClick="BtnSignaturePosition_Click"/>
                                    <cc1:CustomImageButton ID="BtnDownDx" style="float:right;width:35px" runat="server" CssClass="clickable" OnClick="BtnSignaturePosition_Click"/>
                                </div>
                            </div>
                         </ContentTemplate>
                    </asp:UpdatePanel>  
                </div>  
                <div class="contentDimPosition">
                    <br /><asp:Label ID="SignatureLblDocumentDim" Font-Size="0.88em" runat="server" ></asp:Label>
                    <p>
                         <asp:Label ID="SignatureLblDocumentDimAltezza" Font-Size="0.88em" runat="server" ></asp:Label>
                         <asp:textbox id="docHeight" runat="server" ReadOnly="True" Width="30px"   BorderStyle="None" BackColor="Transparent" Font-Size="0.88em"></asp:textbox><br />
                         <asp:Label ID="SignatureLblDocumentDimLarghezza" Font-Size="0.88em" runat="server" ></asp:Label>
                         <asp:textbox id="docWidth" runat="server"  ReadOnly="True" Width="30px" BorderStyle="None"  BackColor="Transparent" Font-Size="0.88em"></asp:textbox>
                    </p>
                     <p>
                        <asp:Label ID="SignatureLblDocumentCustomLocation" Font-Size="0.88em" runat="server" ></asp:Label>
                    </p>
                    <asp:UpdatePanel ID="UpdatePanelSignaturePosizionePesonalizzata" runat="server" UpdateMode="Conditional" ClientIDMode="Static"  style=" width:100%; height:100%;">
                         <ContentTemplate>
                            <div class="row">
                                <asp:Label ID="SignatureLblDocumentPosizionePesonalizzataX"  Text="X:" Font-Size="0.88em" runat="server" ></asp:Label>
                                <cc1:CustomTextArea id="TxtX" runat="server" Width="30px" AutoPostBack="true" OnTextChanged="tbxPos_TextChanged" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                                <asp:Label ID="SignatureLblDocumentPosizionePesonalizzataY"  Text="Y:" Font-Size="0.88em" runat="server" ></asp:Label>
                                <cc1:CustomTextArea id="TxtY" runat="server" Width="30px" AutoPostBack="true" OnTextChanged="tbxPos_TextChanged"
                                     CssClass="txt_addressBookLeft" CssClassReadOnly="txt_addressBookLeft_disabled"></cc1:CustomTextArea>
                            </div>
                    </ContentTemplate>
                    </asp:UpdatePanel>  
                </div>
            </div>
        </div>
        <div class="contentSignatureDx">
            <asp:UpdatePanel ID="UpdatePanelFrameSignature" runat="server" UpdateMode="Conditional" ClientIDMode="Static" style=" width:100%; height:100%;">
                <ContentTemplate>
                    <div class="contenteViewer" style=" width:98%; height:98%; margin:5px;">
                        <iframe width="100%"  height="100%" frameborder="0" marginheight="0" marginwidth="0" id="frameSignature"
                            runat="server" clientidmode="Static" style="z-index: 0;" ></iframe>
                    </div>  
                 </ContentTemplate>
             </asp:UpdatePanel>          
        </div>
        <div class="contentSignatureBottomSx">
                <asp:Label CssClass="NormalBold" ID="SignatureLblCaracteristic" runat="server" ></asp:Label>
                <div class="contentCaracterist"  style="  margin-top:7px; ">
                    <div class="contentCharacter" style=" float:left; margin-bottom:15px;">
                         <asp:Label ID="SignatureLblCharacter" Font-Size="0.88em" runat="server" ></asp:Label>
                    </div>
                    <div class="contentCharacterList" style=" float: right; width:80%">
                        <asp:UpdatePanel ID="upanelCharacter" runat="server" class="row" UpdateMode="Conditional" ClientIdMode="static">
                            <ContentTemplate>
                                <asp:DropDownList ID="DdlCharacter" CssClass="chzn-select-deselect" runat="server" AutoPostBack="True" Width="200px"  OnSelectedIndexChanged="CharacterList_SelectedIndexChanged">
                                <asp:ListItem Text="" Enabled="False"></asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        </div>
                    <div class="contentColor" style=" clear: left;">
                        <asp:Label ID="SignatureLblColor" Font-Size="0.88em" runat="server" ></asp:Label>  
                    </div>        
                    <div class="contentColorList" style=" float: right; margin-top:-15px; width:80%; >
                        <asp:UpdatePanel ID="upanelColor" runat="server" class="row" UpdateMode="Conditional" ClientIdMode="static">
                            <ContentTemplate>
                                <asp:DropDownList ID="DdlColour" CssClass="chzn-select-deselect"  style="background-color:Red" runat="server" AutoPostBack="True" Width="200px" OnSelectedIndexChanged="ColorList_SelectedIndexChanged">
                                    <asp:ListItem Text="" Enabled="False"></asp:ListItem>
                                    <asp:ListItem id="SignatureColorRed" Selected="True" style="color:Red" ></asp:ListItem>  
                                    <asp:ListItem id="SignatureColorBlack"></asp:ListItem>                                          
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                     <asp:UpdatePanel ID="UpdatePanelRblOrientation" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                            <ContentTemplate>
                             <div class="row">
                                <asp:RadioButtonList ID="RblOrientation" RepeatColumns="2" CellSpacing="1" runat="server" AutoPostBack="True" onselectedindexchanged="RblOrientation_SelectedIndexChanged" RepeatDirection="Horizontal" Font-Size="0.88em">
                                    <asp:ListItem Value="verticale" id="SignatureRdlStampVertical"></asp:ListItem>
                                    <asp:ListItem Value="orizzontale" id="SignatureRdlStampHorizontal"></asp:ListItem>
                                    <asp:ListItem Value="signature" id="SignatureRdlSignature"></asp:ListItem>
                                    <asp:ListItem Value="noSignatureStamp" id="NoSignatureStamp"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <div>
                        <asp:UpdatePanel ID="UpdatePanelRblDigitalSignaturePos" runat="server" AutoPostBack="True" UpdateMode="Conditional" ClientIDMode="Static" style=" width:100%; height:95%; ">
                             <ContentTemplate>
                                <div class="weight">
                                    <asp:CheckBox ID="ChkDigitalSignatureInfo" runat="server" AutoPostBack="True" Font-Size="0.88em" OnCheckedChanged="ChkSegnatureInfo_CheckedChanged"/> 
                                </div>
                                <asp:RadioButtonList ID="RblDigitalSignaturePos" CellSpacing="1" runat="server" AutoPostBack="True" onselectedindexchanged="RblDigitalSignaturePos_Click" RepeatDirection="Horizontal" Font-Size="0.88em">
                                    <asp:ListItem Value="printOnFirstPage" Selected="True" id="SignatureRblPrintOnFirstPage"></asp:ListItem>
                                    <asp:ListItem Value="printOnLastPage" id="SignatureRblPrintOnLastPage"></asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:RadioButtonList ID="RblFormatSign" CellSpacing="1" runat="server" AutoPostBack="True" onselectedindexchanged="RblDigitalSignaturePos_Click" RepeatDirection="Horizontal" Font-Size="0.88em">
                                    <asp:ListItem Value="digitalCompleted" id="SignatureRblDigitalCompleted" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="digitalSynthesis" id="SignatureRblDigitalSynthesis"></asp:ListItem>
                                </asp:RadioButtonList>
                            </ContentTemplate>
                    </asp:UpdatePanel>
                    </div>
            </div>     
        </div>    
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="SignatureBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" Onclick="BtnConfirm_Click" Visible="False"/>
    <cc1:CustomButton ID="SignatureBtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" Onclick="BtnConfirm_Click" />
    <cc1:CustomButton ID="SignatureBtnClose" runat="server" CssClass="btnEnable" OnClick="SenderBtnClose_Click" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
     <script type="text/javascript">
         $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
         $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>

