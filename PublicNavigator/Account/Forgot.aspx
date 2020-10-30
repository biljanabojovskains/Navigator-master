<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Forgot.aspx.cs" Inherits="PublicNavigator.Account.Forgot" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row navbar-forgot">
        <br/>
        <div class="col-md-6 col-centered">
            <asp:PlaceHolder ID="loginForm" runat="server">
                <div class="form-horizontal">
                    <h4>Внесете го корисничкото име или е-пошта</h4>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <div class="col-md-10">
                            <asp:Label runat="server" AssociatedControlID="Email" CssClass="control-label">Корисничко име или е-пошта</asp:Label><br />
                        </div>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="SingleLine" /><br />
                            <asp:Button runat="server" OnClick="Forgot_Click" Text="Внеси" CssClass="btn btn-primary" />&nbsp;&nbsp;
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" CssClass="text-danger" ErrorMessage="Полето е задолжително" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="DisplayEmail" Visible="false">
                <p class="text-info">
                    Проверете ја е-поштата за да ја ресетирате лозинката.
                </p>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
