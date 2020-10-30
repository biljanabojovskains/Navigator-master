<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="Navigator.Admin.UsersPanel.Logs" %>
<%@ Import Namespace="Microsoft.AspNet.FriendlyUrls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            <h2><%: DbRes.T("LogoviNaAktivnosti", "Resources") %></h2>

 <div>
        <asp:ListView runat="server"
            DataKeyNames="LogId"
            ItemType="Navigator.Models.Abstract.IIzvodLogs"
            OnPagePropertiesChanging="ListLogs_OnPagePropertiesChanging"
            ID="ListLogs">
            <EmptyDataTemplate>
                 <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:NemaLogovi%>"></asp:Literal>
            </EmptyDataTemplate>
            <LayoutTemplate>
                <table class="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:KorisnickoIme%>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Opfat%>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:BrojNaParcela%>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Datum%>"></asp:Literal>
                            </th>
                            <th><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Prezemi%>"></asp:Literal>
                            </th>
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
                        <asp:DynamicControl runat="server" DataField="OpfatIme" ID="OpfatIme" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="BrParcela" ID="BrParcela" Mode="ReadOnly" />
                    </td>
                    <td>
                        <asp:DynamicControl runat="server" DataField="Datum" ID="Datum" Mode="ReadOnly" />
                    </td>
                    <td>    
                        <asp:LinkButton ID="BtnPrezemi" runat="server"  class="btn btn-link" CommandArgument='<%#: Item.Path %>' OnCommand="Btnprezemi_OnCommand"><span aria-hidden="true" class="glyphicon glyphicon-download-alt"></span></asp:LinkButton>
                    </td>                  
                </tr>
            </ItemTemplate>
        </asp:ListView>
        <asp:DataPager ID="DataPager1" runat="server" PageSize="25" PagedControlID="ListLogs">
            <Fields>
                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" FirstPageText="<%$ Resources:Prva %>" PreviousPageText="<%$ Resources:Prethodna %>" />
                <asp:NumericPagerField />
                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowPreviousPageButton="False" NextPageText="<%$ Resources:Sledna %>" LastPageText="<%$ Resources:Posledna %>" />
            </Fields>
        </asp:DataPager>
    </div>
  
</asp:Content>