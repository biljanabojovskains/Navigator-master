<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Uslovi.aspx.cs" Inherits="Navigator.Admin.Uslovi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script>
        function CheckAll(checkid) {
            var updateButtons = $('input:checkbox');
            if ($(checkid).children().is(':checked')) {
                updateButtons.each(function () {
                    if ($(this).attr("id") != $(checkid).children().attr("id")) {
                        $(this).prop("checked", true);
                    }
                });
            }
            else {
                updateButtons.each(function () {
                    if ($(this).attr("id") != $(checkid).children().attr("id")) {
                        $(this).prop("checked", false);
                    }
                });
            }
        };

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Општи и посебни услови</h2>
    <hr />
    <p>
        Одбери проект
        <asp:DropDownList ID="DdlProekti" runat="server" ItemType="Navigator.Models.Abstract.IOpfat" DataTextField="Ime" DataValueField="Id" AutoPostBack="True" OnSelectedIndexChanged="DdlProekti_OnSelectedIndexChanged"></asp:DropDownList>
    </p>
    <asp:Panel ID="PnlUslovi" runat="server" Visible="False">
        <h3>Општи услови</h3>
        <table class="table table-striped">
            <tr runat="server" id="TrOpstiUslovi">
                <td>
                    <asp:FileUpload ID="FuOpsti" runat="server" CssClass="btn btn-default" /></td>
                <td>
                    <asp:Button ID="BtnOpsti" runat="server" Text="Прикачи" CssClass="btn btn-primary" OnClientClick="this.disabled='true';" UseSubmitBehavior="false" OnClick="BtnOpsti_OnClick" ValidationGroup="FuOpsti" /></td>
                <td>
                    <asp:Button ID="BtnOpstiPrevzemi" runat="server" Text="Преземи општи услови" class="btn btn-link" OnClick="BtnOpstiPrevzemi_OnClick"></asp:Button>
                </td>
            </tr>
        </table>

        <asp:RequiredFieldValidator ID="RequiredFieldFuOpsti" runat="server"
            SetFocusOnError="true" ForeColor="Red" ErrorMessage="Внесете фајл" ControlToValidate="FuOpsti"
            ValidationGroup="FuOpsti"></asp:RequiredFieldValidator>
<%--        <asp:RegularExpressionValidator ID="RegularExpressionFuOpsti"
            runat="server" ControlToValidate="FuOpsti"
            ErrorMessage="Само ворд фајлови (docx) се дозволени." ForeColor="Red"
            ValidationExpression="(.*?)\.(docx)$" ValidationGroup="FuOpsti"
            SetFocusOnError="true"></asp:RegularExpressionValidator>--%>

        <h3>Посебни услови</h3>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:ListView runat="server" ID="ListPosebniUslovi" DataKeyNames="Id" ItemType="Navigator.Models.Abstract.IGradParceli">
                    <EmptyDataTemplate>
                        Нема податоци
                    </EmptyDataTemplate>
                    <LayoutTemplate>
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th>
                                        <asp:CheckBox ID="CheckAll" runat="server" AutoPostBack="false" onchange="CheckAll(this);" Text="" /></th>
                                    <th>Број
                                    </th>
                                    <th>Класа на намена
                                                                <asp:DropDownList ID="DdlKlasa" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlKlasaKatnost_OnSelectedIndexChanged"></asp:DropDownList>
                                    </th>
                                    <th>Катност
                                                                <asp:DropDownList ID="DdlKatnost" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlKlasaKatnost_OnSelectedIndexChanged"></asp:DropDownList>
                                    </th>
                                    <th>&nbsp;</th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr runat="server" id="itemPlaceholder" />
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="CbAdd" runat="server" />
                            </td>
                            <td>
                                <asp:Label ID="Label5" runat="server" Text="<%# Item.Broj %>" />
                            </td>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text='<%# Item.KlasaNamena %>' />
                            </td>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text='<%# Item.Katnost %>' />
                            </td>
                            <td>
                                <asp:Button ID="BtnPosebniPrevzemi" runat="server" Text="Преземи посебни услови" class="btn btn-link" CommandArgument='<%#: Item.PosebniUsloviId %>' OnCommand="BtnPosebniPrevzemi_OnCommand"></asp:Button>
                            </td>
                             <td>
                                <asp:Button ID="BtnNumerickiPrevzemi" runat="server" Text="Преземи нумерички показатели" class="btn btn-link" CommandArgument='<%#: Item.NumerickiPokazateliId %>' OnCommand="BtnNumerickiPrevzemi_OnCommand"></asp:Button>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>

              
            </ContentTemplate>
           
        </asp:UpdatePanel>
        <div class="row">
            <asp:FileUpload ID="FuPosebni" runat="server" class="btn btn-default col-md-6" />
            <asp:Button ID="BtnPosebni" runat="server" Text="Прикачи посебни услови" OnClientClick="this.disabled='true';" UseSubmitBehavior="false" class="btn btn-primary" OnClick="BtnPosebni_OnClick" ValidationGroup="FuPosebni" /><br /><br />
            
            <asp:FileUpload ID="FuNumericki" runat="server" class="btn btn-default col-md-6" />
            <asp:Button ID="BtnNumericki" runat="server" Text="Прикачи нумерички показатели" OnClientClick="this.disabled='true';" UseSubmitBehavior="false" class="btn btn-primary" OnClick="BtnNumericki_OnClick" ValidationGroup="FuNumericki" />
            <asp:RequiredFieldValidator ID="RequiredFieldFuPosebni" runat="server"
                SetFocusOnError="true" ForeColor="Red" ErrorMessage="Внесете фајл" ControlToValidate="FuPosebni"
                ValidationGroup="FuPosebni"></asp:RequiredFieldValidator>
<%--            <asp:RegularExpressionValidator ID="RegularExpressionFuPosebni"
                runat="server" ControlToValidate="FuPosebni"
                ErrorMessage="Само ворд фајлови се дозволени." ForeColor="Red"
                ValidationExpression="(.*?)\.(doc|docx)$" ValidationGroup="FuPosebni"
                SetFocusOnError="true"></asp:RegularExpressionValidator>--%>
        </div>
    </asp:Panel>
</asp:Content>
