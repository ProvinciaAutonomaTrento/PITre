<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="Sender.aspx.cs" Inherits="NttDataWA.Popup.Sender" %>
<%@ Register src="../UserControls/Correspondent.ascx" tagname="Correspondent" tagprefix="uc2" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc3" TagName="ajaxpopup2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    $(function () {
        // --- Using the default options:
        $("h2.expand").toggler({ initShow: "div.collapsed" });
        //$("h2.expand").toggler({ initShow: "div.collapse:first" });
        // --- Other options:
        //$("h2.expand").toggler({method: "toggle", speed: 0});
        //$("h2.expand").toggler({method: "toggle"});
        //$("h2.expand").toggler({speed: "fast"});
        //$("h2.expand").toggler({method: "fadeToggle"});
        //$("h2.expand").toggler({method: "slideFadeToggle"});    
        //        $("#content").expandAll({ trigger: "p.expand", ref: "div.demo", localLinks: "p.top a" });
    });
</script>
<script type="text/javascript">
    function closeAjaxModal(id, retval) { // chiude il popup modale [id] e imposta il valore di ritorno [retval] nel campo hidden
        var p = parent.fra_main;
        if (arguments.length > 2 && arguments[2] != null) {
            p = arguments[2];
        }
        else {
            try {
                var e = p.$('iframe').get(0);

                if (e.id != 'ifrm_' + id) {
                    p = e.contentWindow;
                    e = p.$('iframe').get(0);

                    if (e.id != 'ifrm_' + id) {
                        p = e.contentWindow;
                        e = p.$('iframe').get(0);
                    }
                }
            }
            catch (err) {
                try {
                    p = parent.fra_main;
                }
                catch (err2) {
                    p = parent;
                }
            }
        }

        if (arguments.length > 1) {
            this.$('.retval' + id + ' input').get(0).value = retval;
        }
        this.$('#' + id + '_panel').dialog('close');
    }
    </script>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .messager_c1 {float: left; width: 15%; text-align: right;}
        .messager_c2 {float: left; width: 60%; text-align: center; margin: 0 3%;}
        .messager_c2 span 
        {
            color: #f5ae44;
            font-weight: bold;
	        text-transform: uppercase;
	        padding: 15px 0 0 0;
	        margin: 0;
	        display: block;
        }
        .messager_c3 {float: left; width: 15%; text-align: left;}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<!-- PEC 4 - requisito 5 - storico spedizioni -->
<uc3:ajaxpopup2 Id="SenderObjHistory" runat="server" Url="../popup/SenderObjHistory.aspx"
        IsFullScreen="true" PermitClose="true" PermitScroll="true" CloseFunction="function (event, ui)  {__doPostBack('UpSend','');}" />
<uc3:ajaxpopup2 Id="SelectNextMessage" runat="server" Url="../Popup/SelectNextMessage.aspx" IsFullScreen="false" PermitClose="false" PermitScroll="true" Width="600" Height="400"
        CloseFunction="function (event, ui) {__doPostBack('UpSend','SelectNextMessageClosePopup'); }" />
    
    <uc:messager id="messager" runat="server" />     
    <input id="txtReturnValue" runat="server" type="hidden" value="False" />
    <div id="wrapper"> 
        <div id="content">  
            <div class="demo">
                <div>
                    <div style="float:right; margin: 20px;">
                        <!-- PEC 4 - requisito 5 - storico spedizioni -->
                        <cc1:CustomImageButton runat="server" ID="SenderImgObjectHistory" ImageUrl="../Images/Icons/obj_history_big.png"
                                                    
                                OnMouseOutImage="../Images/Icons/obj_history_big.png" OnMouseOverImage="../Images/Icons/obj_history_big_hover.png"
                                                            CssClass="clickableLeft" 
                                ImageUrlDisabled="../Images/Icons/obj_history_big_disabled.png" 
                                 OnClientClick="return ajaxModalPopupSenderObjHistory();" />
                    </div>
                    <div id="SenderDivRegistri" runat="server">
                        <p>
                            <asp:Label ID="SenderLblRegistriRF" runat="server" CssClass="lblspec" Width="250px"></asp:Label>
                            <asp:Label ID="SenderLblCaselle" runat="server" CssClass="lblspec" Width="300px"></asp:Label>
                            <asp:Label ID="SenderLblTipoRicevuta" runat="server" CssClass="lblspec" Width="150px"></asp:Label>
                        </p>
                        <p>
                        <asp:UpdatePanel ID="UpdPnlRegisterAndMail" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:DropDownList ID="cboRegistriRF" runat="server" CssClass="chzn-select-deselect" Width="250px" onselectedindexchanged="cboRegistriRF_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:DropDownList ID="ddl_caselle" runat="server" CssClass="chzn-select-deselect" Width="300px" Enabled="false" onselectedindexchanged="ddl_caselle_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:DropDownList ID="cboTipoRicevutaPec" runat="server" Enabled="false" CssClass="chzn-select-deselect" Width="150px">
                                <asp:ListItem Value ="C"></asp:ListItem>
                                <asp:ListItem Value ="B"></asp:ListItem>
                                <asp:ListItem Value ="S"></asp:ListItem>                                
                            </asp:DropDownList>
                            </ContentTemplate>
                            </asp:UpdatePanel>
                        </p>
                    </div>
                </div>
               <%--<div class="externalCorrespondent">--%>
                    <h2 class="expand" id="SenderInteroperablePEC" runat="server">
                        <asp:Label runat="server" ID="SenderLblInteroperableRecipientsPEC" CssClass="lblcontainer"></asp:Label>
                    </h2>
                    <div id="pnelDestinatariInteroperanti" class="collapse" runat="server">
                        <asp:UpdatePanel runat="server" ID="UpdateDestinatariInteroperanti" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc2:Correspondent ID="listaDestinatariInteroperanti" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
        
                    <h2 class="expand" id="SenderPITRE" runat="server">
                        <asp:Label runat="server" ID="SenderLblInteroperableRecipientsPITRE" CssClass="lblcontainer"></asp:Label>
                    </h2>
                    <div id="pnelDestinatatiInteropSempl" class="collapse" runat="server">
                        <asp:UpdatePanel runat="server" ID="UpdateDestinatatiInteropSempl" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc2:Correspondent ID="listaDestinatatiInteropSempl" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <h2 class="expand" id="SenderInterni" runat="server">
                        <asp:Label runat="server" ID="SenderLblInternalRecipients" CssClass="lblcontainer"></asp:Label>
                    </h2>
                    <div id="pnelDestinatariInterni" class="collapse" runat="server">
                        <asp:UpdatePanel runat="server" ID="UpdateDestinatariInterni" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc2:Correspondent ID="listaDestinatariInterni" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>

                    <h2 class="expand" id="SenderNoInterop" runat="server">
                        <asp:Label runat="server" ID="SenderLblExternalNotInteroperableRecipients" CssClass="lblcontainer"></asp:Label>
                    </h2>
                    <div id="pnelDestinatariNonInteroperanti" class="collapse" runat="server">
                        <asp:UpdatePanel runat="server" ID="UpdateDestinatariNonInteroperanti" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc2:Correspondent ID="listaDestinatariNonInteroperanti" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                <%--</div>--%>
            </div>
       </div>
    </div>
    <br />
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server" ClientIDMode="Static">
    <div style="float: left">
        <asp:UpdatePanel runat="server" ID="UpSend" UpdateMode="Conditional">
            <ContentTemplate>
                <cc1:CustomButton ID="SenderBtnSend" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" Text="Send" onclick="SenderBtnSend_Click" OnClientClick="disallowOp('Content2');" />
            </ContentTemplate>
        </asp:UpdatePanel>
     </div><div style="float: left">
        <cc1:CustomButton ID="SenderBtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                    OnMouseOver="btnHover" Text="Close" onclick="SenderBtnClose_Click" />
    </div>
</asp:Content>