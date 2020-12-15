<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MassiveTransmissionAccept.aspx.cs" Inherits="NttDataWA.Popup.MassiveTransmissionAccept" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tbl th {
            font-weight: bold;
        }
    </style>
        <script type="text/javascript">
        function SetItemCheck(obj, id) {

            if (obj.checked) {
                var value = $('#HiddenItemsChecked').val();
                var values = new Array(value);
                if (value.indexOf(',') >= 0) values = value.split(',');

                values.push(id);
                value = values.join(',');
                if (value.substring(0, 1) == ',')
                    value = value.substring(1);
                $('#HiddenItemsChecked').val(value);
            }
            else {
                var value = $('#HiddenItemsChecked').val();
                var values = new Array(value);
                if (value.indexOf(',') >= 0) values = value.split(',');
                var found = false;

                for (var i = 0; i < values.length; i++) {
                    if (values[i] == id) {
                        values.splice(i, 1);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    $(".GridTrasmissioniPendenti th input[type='checkbox']").attr('checked', false);

                    value = $('#HiddenItemsUnchecked').val();
                    values = new Array(value);
                    if (value.indexOf(',') >= 0) values = value.split(',');
                    values.push(id);

                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenItemsUnchecked').val(value);
                }
                else {
                    value = values.join(',');
                    if (value.substring(0, 1) == ',')
                        value = value.substring(1);
                    $('#HiddenItemsChecked').val(value);
                }
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="container">
        <div class="contentMassiveTransmissionAccept">
            <div class="row">
                <asp:UpdatePanel runat="server" ID="UpNoteAccettazione" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="PnlNoteAccettazione" runat="server">
                            <span class="weight">
                                <asp:Label ID="LblNoteAccettazione" runat="server"></asp:Label></span>
                            <cc1:CustomTextArea ID="txt_NoteAccettazione" runat="server" MaxLength="250" TextMode="MultiLine"
                                CssClass="txt_textarea" ClientIDMode="Static" CssClassReadOnly="txt_textarea_disabled" Height="50px" />
                            <div class="col-right-no-margin">
                                <span class="charactersAvailable">
                                    <asp:Literal ID="LitNoteAccettazione" runat="server"></asp:Literal>
                                    <span id="txt_NoteAccettazione_chars" runat="server" clientidmode="Static"></span></span>
                            </div>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="row">
                <asp:UpdatePanel ID="UpGridTrasmissioniPendenti" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="marginTopMassiveTransmissionAccept">
                            <asp:GridView ID="GridTrasmissioniPendenti" runat="server" Width="100%" AutoGenerateColumns="false" PageSize="5"
                                CssClass="tbl" OnRowDataBound="GridTrasmissioniPendenti_RowDataBound" ShowHeaderWhenEmpty="false"
                                OnPreRender="GridTrasmissioniPendenti_PreRender" OnRowCommand="GridTrasmissioniPendenti_RowCommand" OnRowCreated="GridTrasmissioniPendenti_ItemCreated">
                                <RowStyle CssClass="NormalRow" />
                                <AlternatingRowStyle CssClass="AltRow" />
                                <PagerStyle CssClass="recordNavigator2" />
                                <Columns>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="IdTrasmissione" runat="server" Text='<%# Bind("IdTrasmissione") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="IdTrasmissioneSingola" runat="server" Text='<%# Bind("idTrasmSingola") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="cbxSelAll" runat="server" AutoPostBack="true" HeaderStyle-Width="3%" OnCheckedChanged="cbxSelAll_CheckedChanged" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="cbxSel" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:MassiveTransmissionAcceptDataInvio%>' HeaderStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblDataInvio" Text='<%# Bind("DataInvio") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:MassiveTransmissionAcceptMittente%>' HeaderStyle-Width="42%">
                                        <ItemTemplate>
                                            <%# GetSenderName(DataBinder.Eval(Container, "DataItem.Utente").ToString(), DataBinder.Eval(Container, "DataItem.UtenteDelegato").ToString())%><br />
                                            <em>
                                                <%# DataBinder.Eval(Container, "DataItem.Ruolo")%></em>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText='<%$ localizeByText:MassiveTransmissionAcceptNoteGenerali%>' HeaderStyle-Width="45%">
                                        <ItemTemplate>
                                            <%# DataBinder.Eval(Container, "DataItem.NoteGenerali")%>
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
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="MassiveTransmissionAcceptBtnAccept" runat="server" CssClass="btnEnable"
                CssClassDisabled="btnDisable" OnMouseOver="btnHover" ClientIDMode="Static"
                OnClientClick="disallowOp('ContentPlaceHolderContent')" OnClick="MassiveTransmissionAcceptBtnAccept_Click" />
            <cc1:CustomButton ID="MassiveTransmissionAcceptBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('ContentPlaceHolderContent')" OnClick="MassiveTransmissionAcceptBtnCancel_Click" />
            <asp:HiddenField ID="HiddenItemsChecked" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HiddenItemsUnchecked" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
