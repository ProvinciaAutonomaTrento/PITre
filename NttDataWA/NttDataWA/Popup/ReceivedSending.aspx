<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ReceivedSending.aspx.cs" Inherits="NttDataWA.Popup.ReceivedSending" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="popupContainer">
    <asp:UpdatePanel ID="upPlnFilter" runat="server">
        <ContentTemplate>
        <fieldset style="border: 0px;">
            <div>
                <asp:label id="lbl_message" runat="server" Visible="False" CssClass="NormalBold"></asp:label>
            </div>
            <div class="popupContainerSx">
                <asp:CheckBox ID="ReceivedSendingCbxEnDisAll" runat="server"  OnCheckedChanged="ReceivedSendingCbxEnDisAll_CheckedChanged" AutoPostBack="true"/>
            </div>           
            <div class="popupContainerDx">
                <cc1:CustomImageButton runat="server" ID="ReceivedSendingExportReceipts" ImageUrl="../Images/Icons/export excel-pdf.png"
                OnMouseOutImage="../Images/Icons/export excel-pdf.png" OnMouseOverImage="../Images/Icons/export-excel-pdf_hover.png"
                CssClass="clickable" ImageUrlDisabled="../Images/Icons/obj_description_disabled.png" OnClick="ReceivedSendingExportReceipts_Click"
                />
            </div>
            <div class="boxFilterType">
                      <p>
                        <asp:Label CssClass="NormalBold" ID="ReceivedSendingLblTypeReceived" runat="server" ></asp:Label>
                    </p>
                <asp:CheckBoxList ID="ReceivedSendingCbxFilterType" RepeatDirection="Horizontal"  RepeatColumns="4" runat="server">
                        <asp:ListItem Value="accettazione"  id="ReceivedSendingCbxAccettazione"/>
                        <asp:ListItem Value="avvenuta-consegna" id="ReceivedSendingCbxAvvenutaConsegna"/>
                        <asp:ListItem Value="non-accettazione" id="ReceivedSendingCbxNonAccettazione"/>
                        <asp:ListItem Value="errore-consegna" id="ReceivedSendingCbxErroreConsegna" />
                        <asp:ListItem Value="ricevuta" id="ReceivedSendingCbxRicevuta" />
                        <asp:ListItem Value="annulla" id="ReceivedSendingCbxAnnulla" />
                        <asp:ListItem Value="eccezione" id="ReceivedSendingCbxEccezione" />
                        <asp:ListItem Value="errore" id="ReceivedSendingCbxErrore" />
                </asp:CheckBoxList>
            </div>
            </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
            <div class="boxFilterType" style="display:none; margin-top: 10px">
                <%--Panel checkbox filter mail --%>
                <div id="divMail" runat="server">
                    <p>
                        <asp:Label CssClass="NormalBold" ID="ReceivedSendinglblMailDest" runat="server"></asp:Label>
                    </p>
                    <br />
                    <asp:CheckBoxList ID="cbxMail" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal" CellSpacing="3" RepeatColumns="2"></asp:CheckBoxList>
                </div>
                <%--Panel filter Code Corr for IS --%>
                <div id="divCodiceCorr" runat="server" style="display:none;">
                    <p>
                        <asp:Label CssClass="NormalBold" ID="ReceivedSendinglblCodiceCorr" runat="server"></asp:Label>
                    </p>
                    <asp:TextBox id="txtFilterCodiceIS" runat="server" CssClass="testo_grigio" Width="270px" MaxLength="50" ></asp:TextBox>
                </div>    
            </div>
            <asp:UpdatePanel ID="UpdatePanelGridReceivedSending" runat="server">
                <ContentTemplate>
                    <div class="risultatiReceivedSending" runat="server" visible="true">
                         <asp:GridView ID="grdList" runat="server" Width="98%" CssClass="tbl" Visible="True" ShowHeaderWhenEmpty ="true" AutoGenerateColumns="False">
                            <RowStyle CssClass="NormalRow" />
                            <AlternatingRowStyle CssClass="AltRow" />
                            <Columns>
                                <asp:TemplateField HeaderText="Data Notifica" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDataNotifica" runat="server" Text='<%# Bind("DATA") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tipo">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTipo" runat="server" Text='<%# Bind("TIPO") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Destinatario">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDestinatario" runat="server" Text='<%# Bind("DESCRIZIONE") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Dettagli">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDettagli" runat="server" Text='<%# Bind("DETTAGLIO") %>'></asp:Label>
                                    </ItemTemplate>                              
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Chiave" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label ID="lblChiave" runat="server" Text='<%# Bind("CHIAVE") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Wrap="False" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <iframe id="reportContent" runat="server" style="width:0px; height:0px; border: 0px;" />
            </ContentTemplate>           
        </asp:UpdatePanel>
        
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
        <cc1:CustomButton ID="ReceivedSendingBtnApplyFilter" runat="server" OnClick="ReceivedSendingBtnApplyFilter_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
        <cc1:CustomButton ID="ReceivedSendingBtnClose" runat="server" OnClick="ReceivedSendingBtnClose_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />

</asp:Content>

