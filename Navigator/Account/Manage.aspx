<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Navigator.Account.Manage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <p>&nbsp;</p>
        <div>
            <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
                <p class="text-success"><%: SuccessMessage %></p>
            </asp:PlaceHolder>
        </div>
        <section id="passwordForm">
            <asp:PlaceHolder runat="server" ID="changePasswordHolder" Visible="false">
                <fieldset class="form-horizontal">
                    <legend><%: DbRes.T("PromeniLozinka", "Resources") %></legend>
                    <asp:ValidationSummary runat="server" CssClass="alert alert-danger" />
                    <div class="form-group">
                        <asp:Label runat="server" ID="CurrentPasswordLabel" AssociatedControlID="CurrentPassword" CssClass="col-md-2 control-label" Text="<%$ Resources:SegasnaLozinka %>"></asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="CurrentPassword" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="CurrentPassword"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:VnesiSegasnaLozinka %>"
                                ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="NewPasswordLabel" AssociatedControlID="NewPassword" CssClass="col-md-2 control-label" Text="<%$ Resources:NovaLozinka %>"></asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="NewPassword"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:VnesiNovaLozinka %>"
                                ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" ID="ConfirmNewPasswordLabel" AssociatedControlID="ConfirmNewPassword" CssClass="col-md-2 control-label" Text="<%$ Resources:NovaLozinka2 %>"></asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="ConfirmNewPassword" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmNewPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="<%$ Resources:VnesiNovaLozinka2 %>"
                                ValidationGroup="ChangePassword" />
                            <asp:CompareValidator runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                CssClass="text-danger" Display="Dynamic" ErrorMessage="<%$ Resources:NeSeSovpagaat %>"
                                ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-sm-offset-2 col-sm-10">
                            <asp:Button runat="server" ID="UpdateButton" Text="<%$ Resources:Promeni %>" CssClass="btn btn-primary" OnClick="UpdateButton_OnClick" ValidationGroup="ChangePassword" />
                        </div>
                    </div>
                </fieldset>
            </asp:PlaceHolder>
        </section>
    </div>
</asp:Content>
