<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="ChoiceTypeDelivery.aspx.cs" Inherits="NttDataWA.Popup.ChoiceTypeDelivery" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        p
        {
            text-align: left;
        }
        em
        {
            font-style: normal;
            font-weight: bold;
            color: #9D9D9D;
        }
    </style>
    <script type="text/javascript">
        function setFocusOnTop() {
            disallowOp('Content2');
            $(".container").scrollTop(0);
            reallowOp();
        }

    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <asp:UpdatePanel ID="UpPnlContainer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="container" class="container">
                <div class="row">
                    <asp:UpdatePanel ID="UpPnlDll" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <div class="col">
                                <asp:Literal ID="lblAll" runat="server" /></div>
                            <div class="col">
                                <asp:DropDownList ID="ddlAll" runat="server" CssClass="chzn-select-deselect" Width="200" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="row">
                    <asp:UpdatePanel ID="UpGrid" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:GridView ID="gvList" runat="server" Width="98%" CssClass="tbl" AutoGenerateColumns="False"
                                AllowPaging="false" PageSize="100" OnRowDataBound="gvList_RowDataBound" OnPageIndexChanging="gridViewResult_PageIndexChanging">
                                <RowStyle CssClass="NormalRow" />
                                <AlternatingRowStyle CssClass="AltRow" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescrizione" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden">
                                        <ItemTemplate>
                                            <asp:Label ID="lblId" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlM" runat="server" CssClass="chzn-select-deselect" Width="200" />
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
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpButton" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnOk_Click" OnClientClick="disallowOp('Content2')" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2')" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
