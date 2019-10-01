<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="AddKeyword.aspx.cs" Inherits="NttDataWA.Popup.AddKeyword" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div id="selectKeyword">
        <fieldset>
            <div class="row">
                <div id="col">
                    <table cellspacing="2" cellpadding="2" border="0">                        
                        <tr>
                            <td>
                                <span class="weight"><asp:Label ID="AddKeywordLblText" runat="server"></asp:Label></span>
                            </td>
                            <td>                           
                            <cc1:CustomTextArea ID="NewKeywordText" Width="240px" runat="server" CssClass="txt_textdata" CssClassReadOnly="txt_textdata_disabled" MaxLength="200" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left">
        <cc1:CustomButton ID="AddKeywordBtnOk" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" OnClick="AddKeywordBtnOk_Click" />
        <cc1:CustomButton ID="AddKeywordBtnChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
            OnMouseOver="btnHover" OnClick="AddKeywordBtnChiudi_Click" />
    </div>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
