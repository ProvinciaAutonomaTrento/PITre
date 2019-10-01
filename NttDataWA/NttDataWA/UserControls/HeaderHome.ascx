<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeaderHome.ascx.cs" Inherits="NttDataWA.UserControls.HeaderHome" %>
    <div id="containerStandardTop">
        <div id="containerHomeHeader">
            <div id="containerHeaderHomeSx">
                <strong>
                    <asp:Label runat="server" ID="headerHomeLblRole"></asp:Label>
                </strong>
            </div>
            <div id="containerHeaderHomeDx">
                <div class="styled-select_full">
                <asp:DropDownList ID="ddlRolesUser" runat="server" OnSelectedIndexChanged="HomeDdlRoles_SelectedIndexChange"
                 CssClass="chzn-select-deselect" AutoPostBack="true" Width="40%">
                 <asp:ListItem Text=""></asp:ListItem>
                 </asp:DropDownList>

                 </div>
            </div>
        </div>
    </div>