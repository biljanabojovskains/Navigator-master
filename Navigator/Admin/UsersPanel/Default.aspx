<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Navigator.Admin.UsersPanel.Default" %>
<%@ Import Namespace="Microsoft.AspNet.FriendlyUrls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <h2><%: DbRes.T("ListaNaKorisnici", "Resources") %></h2>
    <p>
        <asp:HyperLink runat="server" NavigateUrl="Insert" Text="<%$ Resources:KreirajNov %>" /> <br />
        <asp:HyperLink runat="server" NavigateUrl="Logs" Text="<%$ Resources:Logovi %>" />
    </p>
    <div>
        <asp:ListView runat="server"
            DataKeyNames="UserId"
            ItemType="Navigator.Models.Abstract.IUser"
            OnPagePropertiesChanging="ListUsers_OnPagePropertiesChanging"
            ID="ListUsers">
            <EmptyDataTemplate>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:NemaKorisnici %>"></asp:Literal>
            </EmptyDataTemplate>
            <LayoutTemplate>
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:KorisnickoIme %>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:CelosnoIme %>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Telefon %>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Email %>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Aktiven %>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Uloga %>"></asp:Literal>
                            </th>
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
                        <asp:DynamicControl runat="server" DataField="UserName" ID="UserName" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="FullName" ID="FullName" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="Phone" ID="Phone" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="Email" ID="Email" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="Active" ID="DynamicControl1" Mode="ReadOnly" />
                    </td>
                    <td>
                        <%#: Item.UserRole.RoleNameMk %>
                    </td>
                    <td>
                        <asp:HyperLink runat="server" NavigateUrl='<%# FriendlyUrl.Href("~/Admin/UsersPanel/Edit", Item.UserId) %>' Text="<%$ Resources:Promeni %>" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <asp:DataPager ID="DataPager1" runat="server" PageSize="25" PagedControlID="ListUsers">
            <Fields>
                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" FirstPageText="<%$ Resources:Prva %>" PreviousPageText="<%$ Resources:Predhodna %>" />
                <asp:NumericPagerField />
                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowPreviousPageButton="False" NextPageText="<%$ Resources:Sledna %>" LastPageText="<%$ Resources:Posledna %>" />
            </Fields>
        </asp:DataPager>
    </div>
</asp:Content>
