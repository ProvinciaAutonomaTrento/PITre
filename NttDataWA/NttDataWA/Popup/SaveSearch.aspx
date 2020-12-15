<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="SaveSearch.aspx.cs" Inherits="NttDataWA.Popup.SaveSearch" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="col">
                <p>
                    <span class="weight">
                        <asp:Literal ID="litTitle" runat="server" />
                    </span>
                </p>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <cc1:CustomTextArea ID="txtTitle" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" Width="450px"  />
            </div>
        </div>
        <div class="row">
            <div class="col">
                <p>
                    <span class="weight">
                        <asp:Literal ID="litMakeAvailable" runat="server" /></span></p>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <asp:UpdatePanel ID="UpPnlMakeAvailable" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:RadioButtonList ID="rbl_share" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rbl_share_SelectedIndexChanged">
                            <asp:ListItem id="optUsr" Value="usr" />
                            <asp:ListItem id="optGrp" Value="grp" />
                        </asp:RadioButtonList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <asp:UpdatePanel ID="UpPnlGrids" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder ID="PlcGrids" runat="server" Visible="false">
                    <div class="row">
                        <div class="col">
                            <p>
                                <span class="weight">
                                    <asp:Literal ID="litGrids" runat="server" /></span></p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <asp:DropDownList ID="ddl_ric_griglie" runat="server" CssClass="chzn-select-deselect"
                                Width="350px" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="upPnlButtons" runat="server">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSave_Click" OnClientClick="disallowOp('')" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnClose_Click" OnClientClick="disallowOp('')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
