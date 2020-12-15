<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationCenterItemList.ascx.cs"
    Inherits="DocsPAWA.UserControls.NotificationCenterItemList" %>
<asp:ScriptManager ID="ScriptManager" runat="server">
</asp:ScriptManager>
<div class="listBackgroud">
    <div>
        <span class="title">Notifiche</span>
    </div>
    <asp:UpdatePanel ID="upLeftArrow" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="leftArrow">
                <asp:ImageButton ID="imgPreviousPage" runat="server" ImageUrl="~/images/NotificationCenter/arrow_left.png"
                    OnClick="imgPreviousPage_Click" Enabled="false" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="items" style="float: left;">
        <asp:UpdatePanel ID="upItems" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:DataList ID="dlNotificationCenter" runat="server" RepeatDirection="Horizontal">
                    <ItemTemplate>
                        <div class="itemStyle">
                            <div class="feedRow">
                                <a target="_blank" href='<%# DataBinder.Eval(Container.DataItem, "FeedLink") %>'>
                                    <img class="feedIcon" alt="Feed RSS" src="images/NotificationCenter/Feed-Icon.png" /></a>&nbsp;
                                <asp:ImageButton AlternateText="Elimina" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>'
                                    ID="imgDelete" ImageUrl="~/images/cancella.gif" CssClass="deletedItem" OnClick="imgDelete_Click" />
                            </div>
                            <div>
                                <asp:Label ID="lblItemTitle" CssClass="feedLabel" runat="server"><%# DataBinder.Eval(Container.DataItem, "Title") %></asp:Label>
                            </div>
                            <div>
                                <asp:Label ID="lblText" CssClass="feedText" runat="server">Protocollo
                                    <asp:HyperLink NavigateUrl='<%# this.FormatLink(Container.DataItem) %>' ID="hlGoToDoc"
                                        runat="server" CssClass="feedId" ToolTip='<%# DocsPAWA.DocumentManager.GetDocumentSignatureByProfileId(DataBinder.Eval(Container.DataItem, "MessageId").ToString()) %>'>
                                    <%# DocsPAWA.DocumentManager.GetDocumentSignatureByProfileId(DataBinder.Eval(Container.DataItem, "MessageId").ToString()) %>
                                    </asp:HyperLink>
                                </asp:Label>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:DataList>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="imgNextPage" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="imgPreviousPage" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="upRightArrow" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="rightArrow">
                <asp:ImageButton ID="imgNextPage" runat="server" ImageUrl="~/images/NotificationCenter/arrow_right.png"
                    OnClick="imgNextPage_Click" Enabled="false" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
