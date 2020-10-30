<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Navigator.Admin.UsersPanel.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <p>&nbsp;</p>
        <asp:Label ID="ErrorLabel" runat="server" Text="" CssClass="alert alert-dismissable alert-danger" Visible="false"></asp:Label>
        <fieldset class="form-horizontal">
            <legend><%: DbRes.T("PromeniKorisnik", "Resources") %></legend>
            <asp:ValidationSummary runat="server" CssClass="alert alert-danger" />
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtUsername" CssClass="col-md-2 control-label" Text="<%$ Resources:KorisnickoIme %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                     <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtUsername"
                        CssClass="text-danger" ErrorMessage="<%$ Resources:ZodolzitelnoKorIme %>" />
                </div>
            </div>
              <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="DdlRoles" CssClass="col-md-2 control-label" Text="<%$ Resources:Uloga %>"></asp:Label>
                <div class="col-md-10">
                    <asp:DropDownList ID="DdlRoles" runat="server" ItemType="Navigator.Models.Abstract.IRole" DataTextField="RoleNameMk" DataValueField="RoleId" CssClass="form-control">
                    </asp:DropDownList><br/>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtFullname" CssClass="col-md-2 control-label" Text="<%$ Resources:ImePrezime %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtFullname" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtFullname"
                        CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelenoImePrezime %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPhone" CssClass="col-md-2 control-label" Text="<%$ Resources:TelBroj %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPhone"
                        CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelenTelBroj %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtEmail" CssClass="col-md-2 control-label" Text="<%$ Resources:Email %>"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtEmail"
                        CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelenEmail %>" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="CbActive" CssClass="col-md-2 control-label" Text="<%$ Resources:Aktiven %>"></asp:Label>
                <div class="col-md-10"><asp:CheckBox ID="CbActive" runat="server" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <asp:Button runat="server" ID="UpdateButton" Text="<%$ Resources:Promeni %>" CssClass="btn btn-primary" OnClick="UpdateButton_OnClick" />
                    <asp:Button runat="server" ID="CancelButton" Text="<%$ Resources:Ponisti %>" CausesValidation="false" CssClass="btn btn-default" OnClick="CancelButton_OnClick" />
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
