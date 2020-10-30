<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="Navigator.Account.Forgot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <br/>
        <div class="col-md-6 col-centered">
            <asp:PlaceHolder ID="loginForm" runat="server">
                <div class="form-horizontal">
                    <h4><%: DbRes.T("UserIliMail", "Resources") %></h4>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="col-md-10">
                            <asp:Label runat="server" AssociatedControlID="Email" CssClass="control-label" Text="<%$ Resources:UserIliMail2 %>"></asp:Label><br />
                        </div>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="SingleLine" /><br />
                            <asp:Button runat="server" OnClick="Forgot_Click" Text="<%$ Resources:Vnesi %>" CssClass="btn btn-primary" />&nbsp;&nbsp;
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="<%$ Resources:PoletoEZadolzitelno %>" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="DisplayEmail" Visible="false">
                <p class="text-info">
                    <%: DbRes.T("ProveriMail", "Resources") %>
                </p>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
