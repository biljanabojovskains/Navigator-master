<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Insert.aspx.cs" Inherits="Navigator.Admin.UsersPanel.Insert" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div>
        <p>&nbsp;</p>
        <asp:Label ID="ErrorLabel" runat="server" Text="" CssClass="alert alert-dismissable alert-danger" Visible="false"></asp:Label>

        <fieldset class="form-horizontal">
            <legend><%: DbRes.T("KreirajKorisnik", "Resources") %></legend>
            <asp:ValidationSummary runat="server" CssClass="alert alert-danger" />

            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="DdlRoles" CssClass="col-md-2 control-label" Text="<%$ Resources:Uloga %>"></asp:Label>
                <div class="col-md-10">
                    <asp:DropDownList ID="DdlRoles" runat="server" ItemType="Navigator.Models.Abstract.IRole" DataTextField="RoleNameMk" DataValueField="RoleId" CssClass="form-control">
                    </asp:DropDownList><br />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtUsername" CssClass="col-md-2 control-label" Text="<%$ Resources:KorisnickoIme %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtUsername"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:ReqUsername %>" />
                    <asp:RegularExpressionValidator runat="server" ValidationExpression="[A-Za-z][A-Za-z0-9._]{5,19}" ControlToValidate="TxtUsername" ErrorMessage="<%$ Resources:ReqUsername2 %>"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPassword" CssClass="col-md-2 control-label" Text="<%$ Resources:Lozinka %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtPassword" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPassword"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:ReqPass %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtFullname" CssClass="col-md-2 control-label" Text="<%$ Resources:ImePrezime %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtFullname" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtFullname"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:ReqFull %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPhone" CssClass="col-md-2 control-label" Text="<%$ Resources:TelBroj %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPhone"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:ReqTel %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtEmail" CssClass="col-md-2 control-label" Text="<%$ Resources:Email %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtEmail"
                                CssClass="text-danger" ErrorMessage="<%$ Resources:ReqEmail %>" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <asp:Button runat="server" ID="BtnInsert" Text="<%$ Resources:Vnesi %>" CssClass="btn btn-primary" OnClick="BtnInsert_Click" />
                    <asp:Button runat="server" ID="BtnCancel" Text="<%$ Resources:Ponisti %>" CausesValidation="false" CssClass="btn btn-default" OnClick="BtnCancel_Click" />
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
