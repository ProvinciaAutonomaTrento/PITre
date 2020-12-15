<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowCheckOutStatus.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.ShowCheckOutStatus" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .container {text-align: left; line-height: 2em; width: 100%; height: 80%; top: 20%;}
    .description {color: #521110; font-size: small;}
    .definition {color: #151052; font-size: small;}
    .divisor {border-bottom-style:solid; border-bottom-width: thin; border-bottom-color: Silver;}
</style>
<script type="text/javascript">
    
</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
            <div class="container">
                <div class="divisor">
                    <asp:Label ID="lblUser" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblUserNAme" runat="server" class="definition"></asp:Label>
                </div>
                <div class="divisor">
                    <asp:Label ID="lblRole" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblRoleType" runat="server" class="definition"></asp:Label>
                </div>
                <div class="divisor">
                    <asp:Label ID="lblData" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblDataCheckOut" runat="server" class="definition"></asp:Label>
                </div>
                <div class="divisor">
                    <asp:Label ID="lblPath" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblLocalFilePath" runat="server" class="definition"></asp:Label>
                </div>
                <div class="divisor">
                    <asp:Label ID="lblComputer" runat="server" class="description"></asp:Label>
                    <asp:Label ID="lblComputerName" runat="server" class="definition"></asp:Label>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="CheckInOutCloseButton_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
