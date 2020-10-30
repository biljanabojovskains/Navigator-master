<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="PublicNavigator.Account.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <div class="navbar-center-register">
        <p>&nbsp;</p>
        <asp:Label ID="ErrorLabel" runat="server" Text="" CssClass="alert alert-dismissable alert-danger" Visible="false"></asp:Label>

        <fieldset class="form-horizontal">
            <legend>Регистрирај се</legend>
            <asp:ValidationSummary runat="server" CssClass="alert alert-danger" />

            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtUsername" CssClass="col-md-2 control-label" Text="Корисничко име"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtUsername"
                                CssClass="text-danger" ErrorMessage="Корисничкото име е задолжително." />
                    <asp:RegularExpressionValidator runat="server" ValidationExpression="[A-Za-z][A-Za-z0-9._]{5,19}" ControlToValidate="TxtUsername" ErrorMessage="Корисничкото име мора да биде минимум 6 и максимум 20 карактери. Дозволени карактери се A-Z . и _"></asp:RegularExpressionValidator>
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPassword" CssClass="col-md-2 control-label" Text="Лозинка"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPassword"
                                CssClass="text-danger" ErrorMessage="Лозинката е задолжителна." />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPassword" CssClass="col-md-2 control-label" Text="Внесете ја уште еднаш лозинката"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="ConfirmPass" runat="server" TextMode="Password"  CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPassword"
                                CssClass="text-danger" ErrorMessage="Лозинката е задолжителна." />
                    <asp:CompareValidator runat="server" ID="Compare" ControlToValidate="ConfirmPass" ControlToCompare="TxtPassword" ErrorMessage="Лозинките не се совпаѓаат" Font-Size="11px" ForeColor="Red"  />
                </div> 
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtFullname" CssClass="col-md-2 control-label" Text="Име Презиме"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtFullname" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtFullname"
                                CssClass="text-danger" ErrorMessage="Името и презимето се задолжително." />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtPhone" CssClass="col-md-2 control-label" Text="Тел. број"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtPhone" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtPhone"
                                CssClass="text-danger" ErrorMessage="Телефонскиот број е задолжителен." />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="TxtEmail" CssClass="col-md-2 control-label" Text="Email"></asp:Label>
                <div class="col-md-10">
                    <asp:TextBox ID="TxtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="TxtEmail"
                                CssClass="text-danger" ErrorMessage="Email е задолжителен." />
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-2 col-sm-10">
                    <asp:Button runat="server" ID="BtnInsert" Text="Внеси" CssClass="btn btn-primary" OnClick="BtnInsert_Click" />
                    <asp:Button runat="server" ID="BtnCancel" Text="Поништи" CausesValidation="false" CssClass="btn btn-default" OnClick="BtnCancel_Click" />
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>