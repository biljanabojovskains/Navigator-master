<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OdobrenieGradba.aspx.cs" Inherits="Navigator.Modules.OdobrenieZaGradba.OdobrenieGradba" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/Settings.aspx")%>"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher.js")%>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/layers.js") %>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.css")%>" type="text/css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../../Scripts/moment.js"></script>
    <script src="../../Scripts/bootstrap-datetimepicker.min.js"></script>       
    <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divInfo"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="vnesModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <b><%: DbRes.T("VnesetePredmet", "Resources") %></b>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnClose"><span aria-hidden="true">&times;</span></button>          
                </div>
                <div class="modal-body">
                    <div id="divVnes">
                        <table class="table table-striped table-hover">
                            <tr>
                                <th><%: DbRes.T("Broj", "Resources") %></th>
                                <th>
                                    <input runat="server" type="text" id="brPredmet"  value="" class="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="brPredmet" CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelnoVnesiBrojNaPredmet%>"/>
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("TipNaBaranje", "Resources") %></th>
                                <th>
                                    <asp:DropDownList ID="ddlTipBaranje" runat="server" ItemType="Navigator.Models.Abstract.ITipBaranje" DataTextField="TipBaranjeName" DataValueField="TipBaranjeId" onchange="jsFunction(this.value)" CssClass="form-control"></asp:DropDownList><br />                                   
                                    <select id="ddlPodTipBaranje" class="js-example-placeholder-single form-control"></select>
                                    <asp:RequiredFieldValidator  ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="ddlTipBaranje" CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelnoVnesiTipBaranje%>" />
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("ImeNaSluzbenik", "Resources") %></th>
                                <th>
                                    <input runat="server" type="text" id="sluzbenik" value=""  class="form-control" />
                                     <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="sluzbenik" CssClass="text-danger" ErrorMessage="<%$ Resources:ZadolzitelnoVnesiSluzbenik %>" />
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("DatumNaBaranje", "Resources") %></th>
                                <th>
                                    <asp:TextBox runat="server" ID="datumBaranja" CssClass="form-control" TextMode="SingleLine" MaxLength="10" data-date-format="DD/MM/YYYY" />          
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="datumBaranja" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiDatum %>" />
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("DatumNaIzdavanje", "Resources") %></th>
                                <th>
                                    <asp:TextBox runat="server" ID="datumIzdavanja" CssClass="form-control" TextMode="SingleLine" MaxLength="10" data-date-format="DD/MM/YYYY" />  
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="datumIzdavanja" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiDatum %>" />        
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("Pravosilno", "Resources") %></th>
                                <th>
                                    <asp:TextBox runat="server" ID="pravosilno" CssClass="form-control" TextMode="SingleLine" MaxLength="10" data-date-format="DD/MM/YYYY" />    
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="pravosilno" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiDatum %>" />    
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("Investitor", "Resources") %></th>
                                <th><input  runat="server" type="text" id="investitor" value="" class="form-control"  /></th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("BrojNaKp", "Resources") %></th>
                                <th><input runat="server" type="text" id="brKP" value="" class="form-control"  /></th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("Ko", "Resources") %></th>
                                <th>
                                    <asp:DropDownList ID="ddlKO" runat="server" class="form-control" >
                                        <asp:ListItem Enabled="true" Text="" Value="-1"></asp:ListItem>
                                        <%--<asp:ListItem Text="Гази Баба" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Маџари" Value="2"></asp:ListItem>
                                         <asp:ListItem Text="Сингелиќ" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="Сингелиќ 2" Value="4"></asp:ListItem>
                                         <asp:ListItem Text="Сингелиќ 3" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="Инџиково" Value="6"></asp:ListItem>
                                         <asp:ListItem Text="Трубарево" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="Јурумлери" Value="8"></asp:ListItem>--%>
                                        

                                         <asp:ListItem Text="Центар 1" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Центар 2" Value="1"></asp:ListItem>

                                    </asp:DropDownList>
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("Adresa", "Resources") %></th>
                                <th><input runat="server" type="text" id="adresa" value="" class="form-control"  /></th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("ParkingMestaVoParcela", "Resources") %></th>
                                <th><input runat="server" type="text" id="parkMestoPacela" value="" class="form-control" /></th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("ParkingMestaVoKatna", "Resources") %></th>
                                <th><input runat="server" type="text" id="parkMestoGaraza" value="" class="form-control" /></th>
                            </tr>                          
                            <tr>
                                <th><%: DbRes.T("KatnaGaraza", "Resources") %></th>
                                <th>
                                    <asp:DropDownList ID="DdlGarazi" runat="server" ItemType="Navigator.Models.Abstract.IKatniGarazi" DataTextField="KatniGaraziName" DataValueField="KatniGaraziId" CssClass="form-control"></asp:DropDownList><br />
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("IznosNaKomunalii", "Resources") %></th>
                                <th>
                                    <input runat="server" type="text" id="iznosKomunalni" value="" class="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupPredmet' runat="server" ControlToValidate="iznosKomunalni" CssClass="text-danger" Display="Dynamic" ErrorMessage="<%$ Resources:ZadolzitelnoVnesiIznosKomunalii%>" />
                                    <asp:RegularExpressionValidator ID="valGroupPredmet" ControlToValidate="iznosKomunalni" runat="server" CssClass="text-danger"  ErrorMessage="Внесете децимален број со ' . '" ValidationExpression="\d*(\.\d+)?$" Display="Dynamic"> </asp:RegularExpressionValidator>
                                </th>
                            </tr>
                            <tr>
                                <th><%: DbRes.T("Zabeleski", "Resources") %></th>
                                <th><input runat="server" type="text" id="zabeleska" value="" class="form-control" /></th>
                            </tr>

                        </table>                      
                    </div>
                </div>
                <div class="modal-footer">
                     <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnCloseModel"><%: DbRes.T("Zatvori", "Resources") %></button>
                     <%--<asp:Button ValidationGroup='valGroupPredmet' runat="server" CssClass="btn btn-info"  id="btnZacuvaj" OnClick="btnZacuvaj_Click" Text="<%$ Resources:Resources,Zacuvaj %>" />--%>
                     <a href="#" id="btnSave" class="btn btn-info" data-dismiss="modal" title="Save">Save</a>
                
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="vnesDocModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <b><%: DbRes.T("VneseteDokument", "Resources") %></b>
                     <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnDocClose"><span aria-hidden="true">&times;</span></button>          
                </div>
                <div class="modal-body">
                    <div>
                         <%: DbRes.T("PrikaciDokument", "Resources") %>
                         <input type="file" id="File1" name="File1" runat="server" class="btn btn-default form-control" />              
                    </div>
                </div>
                <div class="modal-footer">
                     <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnCloseDocModel"><%: DbRes.T("Zatvori", "Resources") %></button>
                     <asp:Button ID="btnDocInsert" runat="server" value="Upload" CssClass="btn btn-info" Text="<%$ Resources:Resources,Zacuvaj %>" OnClick="btnDocInsert_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="infoDocModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divInfoDoc"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="infoOdobrenieModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" id="btnCloseOdobrenie" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divInfoOdobrenie"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnCloseOdobrenieModal"  class="btn btn-default" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="myLoadingModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel"><%: DbRes.T("VeMolimePricekajte", "Resources") %></h4>
                </div>
                <div class="modal-body">
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>
                </div>
            </div>
        </div>
    </div>
    <div id="mapG">
        <div class="edit_map_controlls">
            <a href="#" id="btnInfo" class="btn btn-sm btn-info" title="<%: DbRes.T("Info", "Resources") %>"><span class="glyphicon glyphicon-info-sign" aria-hidden="true"></span></a>
            <a href="#" id="btnExtent" class="btn btn-sm btn-info" title="<%: DbRes.T("PosledenRazmer", "Resources") %>" onclick="goToLastExtent()"><span class="glyphicon glyphicon-sort" aria-hidden="true"></span></a>
            <a href="#" id="btnVnes" class="btn btn-sm btn-info" title="<%: DbRes.T("Vnes", "Resources") %>"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></a>
            <a href="#" id="btnView" class="btn btn-sm btn-info" title="<%: DbRes.T("InfoOdobrenie", "Resources") %>"><span class="glyphicon glyphicon-eye-open" aria-hidden="true"></span></a>
            <a href="#" id="btnInsertDoc" class="btn btn-sm btn-info" title="<%: DbRes.T("VnesiDokument", "Resources") %>"><span class="glyphicon glyphicon-paperclip" aria-hidden="true"></span></a>
            <a href="#" id="btnInfoDoc" class="btn btn-sm btn-info" title="<%: DbRes.T("InfoDokumenti", "Resources") %>"><span class="glyphicon glyphicon-list" aria-hidden="true"></span></a>
            <a href="#" id="btnSearch" class="btn btn-sm btn-info" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>
            <a href="#" id="btnRubberZoom" class="btn btn-sm btn-info" title="<%: DbRes.T("ZumirajDel", "Resources") %>"><span class="glyphicon glyphicon-screenshot" aria-hidden="true"></span></a>
            <div id="divPopup" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <ul class="nav nav-tabs" id="tabovi">
                                <li class="active"><a id="aBrzoP" href="#brzoP" data-toggle="tab"><%: DbRes.T("SiteSloevi", "Resources") %></a></li>
                                <li class=""><a id="aDolgoP" href="#dolgoP" data-toggle="tab"><%: DbRes.T("KatParceli", "Resources") %></a></li>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="brzoP">
                                    <div class="input-group">
                                        <input id="txtSearch" type="text" class="form-control" />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearch" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="280px">
                                        <div id="progress" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResult"></div>
                                    </asp:Panel>
                                </div>
                                <div class="tab-pane fade" id="dolgoP">
                                    <div class="input-group">
                                        <input id="txtSearchDolgo" type="text" class="form-control" />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchDolgo" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical" Height="220px">
                                        <div id="progressDolgo" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultDolgo"></div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <p class="text-left col-md-4" id="mouse-position"></p>
    <div id="data"></div>
    <div id="dataSearch"></div>
    <div id="dataSearchK"></div>
    <p class="bl" id="razmer"></p>
     <iframe id="my_iframe" style="display:none;"></iframe>
    <asp:HiddenField ID="fkParcel" runat="server" Value="" />
    <asp:HiddenField ID="fkDocParcel" runat="server" Value="" />
    <asp:HiddenField ID="predmetCoordinates" runat="server" Value="" />
    <asp:HiddenField ID="fkPodtipBaranje" runat="server" Value="" />
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/odobrenieGradba.js") %>"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>
    <script type="text/javascript">

        function jsFunction(value) {
            if (value == 17) {
                $("#ddlPodTipBaranje").html('').select2({ data: [{ id: '', text: '' }] }).empty();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/FillSubRequest")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{id:' + value + '}',
                    success: function (result) {
                        if (result.d.length > 0) {
                            var jsonResult = JSON.parse(result.d);
                            $('#ddlPodTipBaranje').select2({ placeholder: '<%: DbRes.T("IzberiPodTipNaBaranje", "Resources") %>', allowClear: true, data: jsonResult });
                        $('#ddlPodTipBaranje').val("");
                        $('#ddlPodTipBaranje').trigger("change.select2");
                    }
                },
                    error: function (p1, p2, p3) {
                        hideLoading();
                        alert(p1.status);
                        alert(p3);
                    }
                });
        }
        else {
            $("#ddlPodTipBaranje").html('').select2({ data: [{ id: '', text: '' }] }).empty();
        }
    }
    $('#ddlPodTipBaranje').on('change', function () {
        -+
            console.log("NNN " + ddlPodTipBaranje.options[ddlPodTipBaranje.selectedIndex].innerHTML);
        console.log($('#<%=fkPodtipBaranje.ClientID%>'));
            $('#<%=fkPodtipBaranje.ClientID%>').val(ddlPodTipBaranje.options[ddlPodTipBaranje.selectedIndex].innerHTML);
            console.log($('#<%=fkPodtipBaranje.ClientID%>').val());
        });
        function downloadDocData(path) {
            // Get file name from url.
            if (path == null || path == '') {
            }
            else {
                var root = location.protocol + '//' + location.host + '/';
                var url = root + path;
                var filename = url.substring(url.lastIndexOf("/") + 1).split("?")[0];
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'blob';
                xhr.onload = function () {
                    var a = document.createElement('a');
                    a.href = window.URL.createObjectURL(xhr.response); // xhr.response is a blob
                    a.download = filename; // Set the file name.
                    a.style.display = 'none';
                    document.body.appendChild(a);
                    a.click();
                    delete a;
                };
                xhr.open('GET', url);
                xhr.send();
            }
        };
        function formatDate(datum) {
            var d = new Date(datum);
            if (d.getDate() < 10 && (d.getMonth() + 1) < 10) {
                return '0' + d.getDate() + '.' + '0' + (d.getMonth() + 1) + '.' + d.getFullYear();
            }
            else if (d.getDate() < 10 && (d.getMonth() + 1) > 9) {
                return '0' + d.getDate() + '.' + (d.getMonth() + 1) + '.' + d.getFullYear();
            }
            else if (d.getDate() > 9 && (d.getMonth() + 1) < 10) {
                return d.getDate() + '.' + '0' + (d.getMonth() + 1) + '.' + d.getFullYear();
            }
            else {
                return d.getDate() + '.' + (d.getMonth() + 1) + '.' + d.getFullYear();
            }
        };
        $("#txtSearch").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btnSubmitSearch").click();
            }
        });
        $("#txtSearchDolgo").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btnSubmitSearchDolgo").click();
            }
        });
        function showModal() {
            $('#infoModal').modal('show');
        };
        function showVnesModal() {
            $('#vnesModal').modal('show');
        };
        function showInfoOdobrenieModal() {
            $('#infoOdobrenieModal').modal('show');
        };
        function closeVnesModal() {
            $('#vnesModal').modal('hide');
        };
        function showLoading() {
            $('#myLoadingModal').modal('show');
        };
        function showVnesDocModal() {
            $('#vnesDocModal').modal('show');
        };
        function showInfoDocModal() {
            $('#infoDocModal').modal('show');
        };
        function CloseDocModel() {
            $('#vnesDocModal').modal('hide');
        };
        function hideLoading() {
            $('#myLoadingModal').modal('hide');
        };
        $('#btnCloseOdobrenieModal').click(function () {
            document.getElementById('<%= predmetCoordinates.ClientID %>').value = "";
        });
            $('#btnCloseOdobrenie').click(function () {
                document.getElementById('<%= predmetCoordinates.ClientID %>').value = "";
        });
        $('#btnCloseModel').click(function () {
            document.getElementById('<%= brPredmet.ClientID %>').value = "";
            document.getElementById('<%= sluzbenik.ClientID %>').value = "";
            document.getElementById('<%= investitor.ClientID %>').value = "";
            document.getElementById('<%= brKP.ClientID %>').value = "";
            document.getElementById('<%= adresa.ClientID %>').value = "";
            document.getElementById('<%= parkMestoPacela.ClientID %>').value = "";
            document.getElementById('<%= parkMestoGaraza.ClientID %>').value = "";
            document.getElementById('<%= iznosKomunalni.ClientID %>').value = "";
            document.getElementById('<%= zabeleska.ClientID %>').value = "";
            document.getElementById('<%= fkParcel.ClientID %>').value = "";
            $("#<%= ddlTipBaranje.ClientID %>").val("");
            $("#<%= ddlKO.ClientID %>").val("-1");
            $("#<%= DdlGarazi.ClientID %>").val("");
            $("#ddlPodTipBaranje").html('').select2({ data: [{ id: '', text: '' }] }).empty();

            document.getElementById('<%= datumBaranja.ClientID %>').value = "";
            document.getElementById('<%= datumIzdavanja.ClientID %>').value = "";
            document.getElementById('<%= pravosilno.ClientID %>').value = "";
            closeVnesModal();
        });
        $('#btnClose').click(function () {
            document.getElementById('<%= brPredmet.ClientID %>').value = "";
            document.getElementById('<%= sluzbenik.ClientID %>').value = "";
            document.getElementById('<%= investitor.ClientID %>').value = "";
            document.getElementById('<%= brKP.ClientID %>').value = "";
            document.getElementById('<%= adresa.ClientID %>').value = "";
            document.getElementById('<%= parkMestoPacela.ClientID %>').value = "";
            document.getElementById('<%= parkMestoGaraza.ClientID %>').value = "";
            document.getElementById('<%= iznosKomunalni.ClientID %>').value = "";
            document.getElementById('<%= zabeleska.ClientID %>').value = "";
            document.getElementById('<%= fkParcel.ClientID %>').value = "";
            $("#<%= ddlKO.ClientID %>").val("-1");
            document.getElementById('<%= datumBaranja.ClientID %>').value = "";
            document.getElementById('<%= datumIzdavanja.ClientID %>').value = "";
            document.getElementById('<%= pravosilno.ClientID %>').value = "";
            $("#<%= ddlTipBaranje.ClientID %>").val("0");
            $("#<%= ddlKO.ClientID %>").val("-1");
            $("#<%= DdlGarazi.ClientID %>").val("");
            $("#ddlPodTipBaranje").html('').select2({ data: [{ id: '', text: '' }] }).empty();
            closeVnesModal();
        });

        $('#btnCloseDocModel').click(function () {

            document.getElementById('<%= File1.ClientID %>').value = "";
            document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
            CloseDocModel();
        });
        $('#btnDocClose').click(function () {

            document.getElementById('<%= File1.ClientID %>').value = "";
            document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
            CloseDocModel();
        });
        var infoTool = false;
        var vnesTool = false;
        var infoOdobrenieTool = false;
        var searchTool = false;
        var rubberZoomTool = false;
        var insertDocTool = false;
        var infoDocTool = false;
        var typeSelect = 'None';
        var draw; // global so we can remove it later
        var source = new ol.source.Vector({ wrapX: false });
        var idParcela;
        var idParcelaDoc;
        $('#btnInfo').click(function () {
            searchTool = false;
            rubberZoomTool = false;
            vnesTool = false;
            infoOdobrenieTool = false;
            infoDocTool = false;
            insertDocTool = false;
            $('#divPopup').hide();
            if (infoTool) {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
            } else {
                $('#btnInfo').addClass("btn-danger");
                $('#btnInfo').removeClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            infoTool = !infoTool;
        });
        $('#btnVnes').click(function () {
            searchTool = false;
            rubberZoomTool = false;
            infoTool = false;
            infoOdobrenieTool = false;
            infoDocTool = false;
            insertDocTool = false;
            $('#divPopup').hide();
            if (vnesTool) {
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').addClass("btn-danger");
                $('#btnVnes').removeClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            vnesTool = !vnesTool;
        });
        $('#btnView').click(function () {
            vnesTool = false;
            searchTool = false;
            rubberZoomTool = false;
            infoTool = false;
            infoDocTool = false;
            insertDocTool = false;
            $('#divPopup').hide();
            if (infoOdobrenieTool) {
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').addClass("btn-danger");
                $('#btnView').removeClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            infoOdobrenieTool = !infoOdobrenieTool;
        });
        $('#btnInsertDoc').click(function () {
            vnesTool = false;
            searchTool = false;
            rubberZoomTool = false;
            infoTool = false;
            infoOdobrenieTool = false;
            infoDocTool = false;
            $('#divPopup').hide();
            if (insertDocTool) {
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInsertDoc').addClass("btn-danger");
                $('#btnInsertDoc').removeClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            insertDocTool = !insertDocTool;
        });
        $('#btnInfoDoc').click(function () {
            vnesTool = false;
            searchTool = false;
            rubberZoomTool = false;
            infoTool = false;
            infoOdobrenieTool = false;
            insertDocTool = false;
            $('#divPopup').hide();
            if (infoDocTool) {
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInfoDoc').addClass("btn-danger");
                $('#btnInfoDoc').removeClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            infoDocTool = !infoDocTool;
        });
        $('#btnSearch').click(function () {
            infoTool = false;
            vnesTool = false;
            infoOdobrenieTool = false;
            rubberZoomTool = false;
            infoDocTool = false;
            insertDocTool = false;
            $('#divPopup').toggle();
            if (searchTool) {
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
            } else {
                $('#btnSearch').addClass("btn-danger");
                $('#btnSearch').removeClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            searchTool = !searchTool;
        });
        $('#btnRubberZoom').click(function () {
            infoTool = false;
            vnesTool = false;
            infoOdobrenieTool = false;
            searchTool = false;
            infoDocTool = false;
            insertDocTool = false;
            $('#divPopup').hide();
            if (rubberZoomTool) {
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $('#btnRubberZoom').addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnInsertDoc').removeClass("btn-danger");
                $('#btnInsertDoc').addClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'Draw';
                map.removeInteraction(draw);
                addInteraction();
            }
            rubberZoomTool = !rubberZoomTool;
        });

        map.on('singleclick', function (evt) {
            if (infoTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoTool")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        $('#divInfo').empty();
                        var jsonResult = JSON.parse(result.d);
                        for (var key in jsonResult) {
                            if (jsonResult.hasOwnProperty(key)) {

                                if (key === "ListOpfat") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4><%: DbRes.T("Opfat", "Resources") %></h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("ImeNaPlan", "Resources") %></th><th><%: DbRes.T("PlanskiPeriod", "Resources") %></th><th><%: DbRes.T("TehnickiBroj", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td>' + jsonResult1[item].PlanskiPeriod + '</td><td>' + jsonResult1[item].TehnickiBroj + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                        tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("BrojNaOdluka", "Resources") %></th><th><%: DbRes.T("ZakonskaRegulativa", "Resources") %></th><th><%: DbRes.T("Izrabotuva", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].BrOdluka + '</td><td>' + jsonResult1[item].ZakonskaRegulativa + '</td><td>' + jsonResult1[item].Izrabotuva + '</td><td>' + jsonResult1[item].Povrshina + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListBlok") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4><%: DbRes.T("Blok", "Resources") %></h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Ime", "Resources") %></th><th><%: DbRes.T("Namena", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td>' + jsonResult1[item].Namena + '</td><td>' + nullToEmptyString(jsonResult1[item].Povrshina) + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListGradParceli") {
                                    var jsonResult1 = jsonResult[key];
                                    $('#data').removeData();
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4><%: DbRes.T("GradezniParceli", "Resources") %></h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Broj", "Resources") %></th><th><%: DbRes.T("KlasaNaNamena", "Resources") %></th><th><%: DbRes.T("KompatibilnaKlasaNaNamena", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th><th><%: DbRes.T("PovrsinaZaGradenje", "Resources") %></th><th><%: DbRes.T("PovrsinaBruto", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            $('#data').data('' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                            tableString += '<tr><td>' + nullToEmptyString(jsonResult1[item].Broj) + '</td><td>' + nullToEmptyString(jsonResult1[item].KlasaNamena) + '</td><td>' + nullToEmptyString(jsonResult1[item].KompKlasaNamena) + '</td><td>' + nullToEmptyString(jsonResult1[item].PovrshinaOpisno) + '</td><td>' + nullToEmptyString(jsonResult1[item].PovrshinaGradenjeOpisno) + '</td><td>' + nullToEmptyString(jsonResult1[item].BrutoPovrshinaOpisno) + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                        tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("MaksimalnaVisina", "Resources") %></th><th><%: DbRes.T("Katnost", "Resources") %></th><th><%: DbRes.T("ParkingMesta", "Resources") %></th><th><%: DbRes.T("ProcentNaIzgradenost", "Resources") %></th><th><%: DbRes.T("KoeficientNaIskoristenost", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + nullToEmptyString(jsonResult1[item].MaxVisina) + '</td><td>' + nullToEmptyString(jsonResult1[item].Katnost) + '</td><td>' + nullToEmptyString(jsonResult1[item].ParkingMesta) + '</td><td>' + nullToEmptyString(jsonResult1[item].ProcentIzgradenostOpisno) + '</td><td>' + nullToEmptyString(jsonResult1[item].KoeficientIskoristenostOpisno) + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);

                                        for (var item in jsonResult1) {
                                            if (jsonResult1[item].OpstiUsloviId != null && jsonResult1[item].PosebniUsloviId != null) {
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="opstiUslovi(\'' + jsonResult1[item].OpstiUsloviId + '\');return false;"><%: DbRes.T("PrevzemiOpstiUslovi", "Resources") %></a>');
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="posebniUslovi(\'' + jsonResult1[item].PosebniUsloviId + '\');return false;"><%: DbRes.T("PrevzemiPosebniUslovi", "Resources") %></a>');
                                            }
                                        }
                                        goToGeojsonExtent();
                                    }
                                }
                            }
                        }
                        showModal();
                    },
                    error: function (p1, p2, p3) {
                        alert(p1.status);
                        alert(p3);
                    }
                });


            } else if (vnesTool) {

                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoTool")%>",
                        async: true,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: '{coordinates:"' + evt.coordinate + '"}',
                        success: function (result) {
                            $('#divInfo').empty();
                            var jsonResult = JSON.parse(result.d);
                            for (var key in jsonResult) {
                                if (jsonResult.hasOwnProperty(key)) {

                                    if (key === "ListGradParceli") {
                                        var jsonResult1 = jsonResult[key];
                                        $('#data').removeData();
                                        if (jsonResult1 !== null && jsonResult1.length > 0) {
                                            for (var item in jsonResult1) {
                                                $('#data').data('' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                                idParcela = jsonResult1[item].Id;
                                            }
                                            goToGeojsonExtent();
                                        }
                                    }
                                }
                            }
                            if (idParcela != undefined) {
                                $("#<%= fkParcel.ClientID %>").val(idParcela);
                                showVnesModal();
                            }
                        },
                        error: function (p1, p2, p3) {
                            alert(p1.status);
                            alert(p3);
                        }
                    });

                } else if (insertDocTool) {
                    $.ajax({
                        type: "POST",
                        url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoTool")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        $('#divInfo').empty();
                        var jsonResult = JSON.parse(result.d);
                        for (var key in jsonResult) {
                            if (jsonResult.hasOwnProperty(key)) {

                                if (key === "ListGradParceli") {
                                    var jsonResult1 = jsonResult[key];
                                    $('#data').removeData();
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        for (var item in jsonResult1) {
                                            $('#data').data('' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                            idParcelaDoc = jsonResult1[item].Id;
                                        }
                                        goToGeojsonExtent();
                                    }
                                }
                            }
                        }

                        if (idParcelaDoc != undefined) {
                            $("#<%= fkDocParcel.ClientID %>").val(idParcelaDoc);
                            showVnesDocModal();
                        }
                    },
                    error: function (p1, p2, p3) {
                        alert(p1.status);
                        alert(p3);
                    }
                });
            }
            else if (infoDocTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoDocTool")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        $('#dataSearchK').removeData();
                        $('#data').removeData();
                        $('#divInfoDoc').empty();
                        var jsonResult = JSON.parse(result.d);
                        if (jQuery.isEmptyObject(jsonResult)) {
                            $('#divInfoDoc').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                        } else {

                            var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("Dup", "Resources") %></th><th><%: DbRes.T("PrevzemiDokument", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                            for (var key in jsonResult) {
                                if (jsonResult.hasOwnProperty(key)) {
                                    $('#dataSearchK').data(jsonResult[key].Id.toString(), jsonResult[key].GeoJson);
                                    if (jsonResult[key].Path == null || jsonResult[key].Path == '') {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].Dup) + '</td><td><%: DbRes.T("NemaDokument", "Resources") %></td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult[key].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span></a></td></tr>';
                                }
                                else {
                                    tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].Dup) + '</td><td><a href="#" class="btn btn-link" onclick="downloadDocData(\'' + jsonResult[key].Path + '\');return false;"><span class="glyphicon glyphicon-paperclip" aria-hidden="true"></span></a></td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult[key].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span></a></td></tr>';
                                }
                            }
                        }

                        tableString += '</table>';
                        $('#divInfoDoc').append(tableString);
                    }
                        showInfoDocModal();
                    },
                    error: function (p1, p2, p3) {
                        alert(p1.status);
                        alert(p2);
                        alert(p3);
                    }
                });
        }
        else if (infoOdobrenieTool) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoOdobrenieTool")%>",
                        async: true,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: '{coordinates:"' + evt.coordinate + '"}',
                        success: function (result) {
                            $("#<%= predmetCoordinates.ClientID %>").val(evt.coordinate);
                            $('#dataSearchK').removeData();
                            $('#data').removeData();
                            $('#divInfoOdobrenie').empty();
                            var jsonResult = JSON.parse(result.d);
                            if (jQuery.isEmptyObject(jsonResult)) {
                                $('#divInfoOdobrenie').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                            } else {
                                var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("Broj", "Resources") %></th><th><%: DbRes.T("TipNaBaranje", "Resources") %></th><th><%: DbRes.T("PodTipNaBaranje", "Resources") %></th><th><%: DbRes.T("ImeNaSluzbenik", "Resources") %></th><th><%: DbRes.T("DatumNaBaranje", "Resources") %></th><th><%: DbRes.T("DatumNaIzdavanje", "Resources") %></th><th><%: DbRes.T("Pravosilno", "Resources") %></th><th><%: DbRes.T("Investitor", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].TipBaranje) + '</td><td>' + nullToEmptyString(jsonResult[key].PodTipBaranje) + '</td><td>' + nullToEmptyString(jsonResult[key].Sluzbenik) + '</td><td>' + formatDate(jsonResult[key].DatumBaranja) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DatumIzdavanja)) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DatumPravosilno)) + '</td><td>' + nullToEmptyString(jsonResult[key].Investitor) + '</td></tr>';
                                    }
                                }
                                tableString += '</table>';
                                $('#divInfoOdobrenie').append(tableString);

                                var tableString2 = '<table class="table table-hover"><tr><th><%: DbRes.T("Dup", "Resources") %></th><th><%: DbRes.T("OdlukaZaDup", "Resources") %></th><th><%: DbRes.T("DatumNaOdlukaZaDup", "Resources") %></th><th><%: DbRes.T("NamenaNaGP", "Resources") %></th><th><%: DbRes.T("BrojNaGP", "Resources") %></th><th><%: DbRes.T("BrojNaKp", "Resources") %></th><th><%: DbRes.T("Ko", "Resources") %></th><th><%: DbRes.T("Adresa", "Resources") %></th><th><%: DbRes.T("ParkingMestaVoParcela", "Resources") %></th><th><%: DbRes.T("ParkingMestaVoKatna", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        tableString2 += '<tr><td>' + nullToEmptyString(jsonResult[key].Dup) + '</td><td>' + nullToEmptyString(jsonResult[key].OdlukaDup) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DonesuvanjeOdlukaDup)) + '</td><td>' + nullToEmptyString(jsonResult[key].Namena) + '</td><td>' + nullToEmptyString(jsonResult[key].BrNamena) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKP) + '</td><td>' + nullToEmptyString(jsonResult[key].KO) + '</td><td>' + nullToEmptyString(jsonResult[key].adresa) + '</td><td>' + nullToEmptyString(jsonResult[key].ParkingMestaPacela) + '</td><td>' + nullToEmptyString(jsonResult[key].ParkingMestaGaraza) + '</td></tr>';
                                    }
                                }

                                tableString2 += '</table>';
                                $('#divInfoOdobrenie').append(tableString2);

                                var tableString3 = '<table class="table table-hover"><tr><th><%: DbRes.T("KatnaGaraza", "Resources") %></th><th><%: DbRes.T("IznosNaKomunalii", "Resources") %></th><th><%: DbRes.T("Zabeleska", "Resources") %></th><th><%: DbRes.T("Izbrisi", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        $('#dataSearchK').data(jsonResult[key].Id.toString(), jsonResult[key].GeoJson);

                                        tableString3 += '<tr><td>' + nullToEmptyString(jsonResult[key].KatnaGaraza) + '</td><td>' + nullToEmptyString(jsonResult[key].IznosKomunalii) + '</td><td>' + nullToEmptyString(jsonResult[key].Zabeleski) + '<td><a href="#" class="btn btn-link" onclick="deleteNot(\'' + jsonResult[key].Id + '\');return false;"><span class="glyphicon glyphicon-remove-sign" aria-hidden="true"></span></a></td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult[key].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span></a></td></tr>';



                                    }
                                }

                                tableString3 += '</table>';
                                $('#divInfoOdobrenie').append(tableString3);
                            }
                            showInfoOdobrenieModal();
                        },
                        error: function (p1, p2, p3) {
                            alert(p1.status);
                            alert(p2);
                            alert(p3);
                        }
                    });
                }
        });

$('#btnSubmitSearch').click(function () {
    $('#progress').show();
    $.ajax({
        type: "POST",
        url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/SearchOut")%>",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{searchString:"' + $('#txtSearch').val() + '"}',
            success: function (result) {
                $('#divSearchResult').empty();
                $('#dataSearch').removeData();
                var jsonResult = JSON.parse(result.d);
                for (var key in jsonResult) {
                    if (jsonResult.hasOwnProperty(key)) {
                        if (key === "ListOpfat") {
                            var jsonResult1 = jsonResult[key];
                            if (jsonResult1 !== null && jsonResult1.length > 0) {
                                $('#divSearchResult').append('<h4><%: DbRes.T("Opfati", "Resources") %></h4>');
                                var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Ime", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                                for (var item in jsonResult1) {
                                    $('#dataSearch').data('opfat' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                    tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'opfat' + jsonResult1[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                                }
                                tableString += '</table>';
                                $('#divSearchResult').append(tableString);
                            }
                        }
                        else if (key === "ListBlok") {
                            var jsonResult1 = jsonResult[key];
                            if (jsonResult1 !== null && jsonResult1.length > 0) {
                                $('#divSearchResult').append('<h4><%: DbRes.T("Blokovi", "Resources") %></h4>');
                                var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Ime", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                                for (var item in jsonResult1) {
                                    $('#dataSearch').data('blokovi' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                    tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'blokovi' + jsonResult1[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                                }
                                tableString += '</table>';
                                $('#divSearchResult').append(tableString);
                            }
                        }
                        else if (key === "ListGradParceli") {
                            var jsonResult1 = jsonResult[key];
                            if (jsonResult1 !== null && jsonResult1.length > 0) {
                                $('#divSearchResult').append('<h4><%: DbRes.T("GradezniParceli", "Resources") %></h4>');
                                var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Broj", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                                for (var item in jsonResult1) {
                                    $('#dataSearch').data('parceli' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                    tableString += '<tr><td>' + jsonResult1[item].Broj + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'parceli' + jsonResult1[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                                }
                                tableString += '</table>';
                                $('#divSearchResult').append(tableString);
                            }
                        }
            }
        }
                if ($('#divSearchResult').is(':empty')) {
                    $('#divSearchResult').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                };
                $('#progress').hide();
            },
            error: function (p1, p2, p3) {
                alert(p1.status);
                alert(p3);
                $('#progress').hide();
            }
        });
    });
    $('#btnSubmitSearchDolgo').click(function () {
        $('#progressDolgo').show();
        $.ajax({
            type: "POST",
            url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/SearchOutK")%>",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{searchString:"' + $('#txtSearchDolgo').val() + '"}',
            success: function (result) {
                $('#divSearchResultDolgo').empty();
                $('#dataSearchK').removeData();
                var jsonResult = JSON.parse(result.d);
                for (var key in jsonResult) {
                    if (jsonResult.hasOwnProperty(key)) {
                        if (key === "ListKatastarskaParcela") {
                            var jsonResult1 = jsonResult[key];
                            if (jsonResult1 !== null && jsonResult1.length > 0) {
                                $('#divSearchResultDolgo').append('<h4><%: DbRes.T("KatParceli", "Resources") %></h4>');
                                    var tableString2 = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Broj", "Resources") %></th><th><%: DbRes.T("Lokacija", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';

                                    for (var item in jsonResult1) {
                                        $('#dataSearchK').data(jsonResult1[item].Id.toString(), jsonResult1[item].GeoJson);
                                        tableString2 += '<tr><td>' + jsonResult1[item].Name + '</td><td>' + jsonResult1[item].Location + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult1[item].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                                    }
                                    tableString2 += '</table>';
                                    $('#divSearchResultDolgo').append(tableString2);
                                }
                            }
                        }
                    }
                    if ($('#divSearchResultDolgo').is(':empty')) {
                        $('#divSearchResultDolgo').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                    };
                    $('#progressDolgo').hide();
                },
            error: function (p1, p2, p3) {
                alert(p1.status);
                alert(p3);
                $('#progressDolgo').hide();
            }
        });
    });
        function addInteraction() {
            if (typeSelect !== 'None') {
                value = 'LineString';
                maxPoints = 2;
                geometryFunction = function (coordinates, geometry) {
                    if (!geometry) {
                        geometry = new ol.geom.Polygon(null);
                    }
                    var start = coordinates[0];
                    var end = coordinates[1];
                    geometry.setCoordinates([
                      [start, [start[0], end[1]], end, [end[0], start[1]], start]
                    ]);
                    return geometry;
                };

                draw = new ol.interaction.Draw({
                    source: source,
                    type: /** @type {ol.geom.GeometryType} */ (value),
                    geometryFunction: geometryFunction,
                    maxPoints: maxPoints
                });

                //zumiranje to toa sto e nacrtano
                draw.on('drawend', function (e) {

                    // koordinatite od nactaniot pravoagolnik
                    //  alert(e.feature.getGeometry().getExtent());
                    var extent = e.feature.getGeometry().getExtent();
                    map.getView().fit(extent, map.getSize());

                });
                map.addInteraction(draw);
            }
        }

        function nullToEmptyString(obj) {
            if (obj) {
                return obj;
            }
            return "/";
        }
        $(function () {
            $('#<%=datumBaranja.ClientID%>').datetimepicker({});
           $('#<%=datumIzdavanja.ClientID%>').datetimepicker({});
           $('#<%=pravosilno.ClientID%>').datetimepicker({});
       });

       function deleteNot(id) {
           $.ajax({
               type: "POST",
               url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/IzbrisiPredmet")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + id + '}',
                success: function (result) {
                    $.ajax({
                        type: "POST",
                        url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/InfoOdobrenieTool")%>",
                        async: true,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: '{coordinates:"' + $("#<%= predmetCoordinates.ClientID %>").val() + '"}',
                        success: function (result) {
                            $('#dataSearchK').removeData();
                            $('#data').removeData();
                            $('#divInfoOdobrenie').empty();
                            var jsonResult = JSON.parse(result.d);
                            if (jQuery.isEmptyObject(jsonResult)) {
                                $('#divInfoOdobrenie').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                            } else {
                                var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("BrojNaPredmet", "Resources") %></th><th><%: DbRes.T("TipNaBaranje", "Resources") %></th><th><%: DbRes.T("ImeNaSluzbenik", "Resources") %></th><th><%: DbRes.T("DatumNaBaranje", "Resources") %></th><th><%: DbRes.T("DatumNaIzdavanje", "Resources") %></th><th><%: DbRes.T("Pravosilno", "Resources") %></th><th><%: DbRes.T("Investitor", "Resources") %></th><th><%: DbRes.T("Dup", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].TipBaranje) + '</td><td>' + nullToEmptyString(jsonResult[key].Sluzbenik) + '</td><td>' + formatDate(jsonResult[key].DatumBaranja) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DatumIzdavanja)) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DatumPravosilno)) + '</td><td>' + nullToEmptyString(jsonResult[key].Investitor) + '</td><td>' + nullToEmptyString(jsonResult[key].Dup) + '</td></tr>';
                                    }
                                }
                                tableString += '</table>';
                                $('#divInfoOdobrenie').append(tableString);

                                var tableString2 = '<table class="table table-hover"><tr><th><%: DbRes.T("OdlukaZaDup", "Resources") %></th><th><%: DbRes.T("DatumNaOdlukaZaDup", "Resources") %></th><th><%: DbRes.T("NamenaNaGP", "Resources") %></th><th><%: DbRes.T("BrojNaGP", "Resources") %></th><th><%: DbRes.T("BrojNaKp", "Resources") %></th><th><%: DbRes.T("Ko", "Resources") %></th><th><%: DbRes.T("Adresa", "Resources") %></th><th><%: DbRes.T("ParkingMestaVoParcela", "Resources") %></th><th><%: DbRes.T("ParkingMestaVoKatna", "Resources") %></th><th><%: DbRes.T("KatnaGaraza", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        tableString2 += '<tr><td>' + nullToEmptyString(jsonResult[key].OdlukaDup) + '</td><td>' + nullToEmptyString(formatDate(jsonResult[key].DonesuvanjeOdlukaDup)) + '</td><td>' + nullToEmptyString(jsonResult[key].Namena) + '</td><td>' + nullToEmptyString(jsonResult[key].BrNamena) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKP) + '</td><td>' + nullToEmptyString(jsonResult[key].KO) + '</td><td>' + nullToEmptyString(jsonResult[key].adresa) + '</td><td>' + nullToEmptyString(jsonResult[key].ParkingMestaPacela) + '</td><td>' + nullToEmptyString(jsonResult[key].ParkingMestaGaraza) + '</td><td>' + nullToEmptyString(jsonResult[key].KatnaGaraza) + '</td></tr>';
                                    }
                                }
                                tableString2 += '</table>';
                                $('#divInfoOdobrenie').append(tableString2);

                                var tableString3 = '<table class="table table-hover"><tr><th><%: DbRes.T("IznosNaKomunalii", "Resources") %></th><th><%: DbRes.T("Zabeleska", "Resources") %></th><th><%: DbRes.T("Izbrisi", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';
                                for (var key in jsonResult) {
                                    if (jsonResult.hasOwnProperty(key)) {
                                        $('#dataSearchK').data(jsonResult[key].Id.toString(), jsonResult[key].GeoJson);

                                        tableString3 += '<tr><td>' + nullToEmptyString(jsonResult[key].IznosKomunalii) + '</td><td>' + nullToEmptyString(jsonResult[key].Zabeleski) + '<td><a href="#" class="btn btn-link" onclick="deleteNot(\'' + jsonResult[key].Id + '\');return false;"><span class="glyphicon glyphicon-remove-sign" aria-hidden="true"></span></a></td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult[key].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span></a></td></tr>';
                                    }
                                }

                                tableString3 += '</table>';
                                $('#divInfoOdobrenie').append(tableString3);
                            }
                            showInfoOdobrenieModal();
                        },
                        error: function (p1, p2, p3) {
                            alert(p1.status);
                            alert(p2);
                            alert(p3);
                        }
                    });
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }
        function downloadDoc(path) {
            // Get file name from url.
            if (path == null || path == '') {
            }
            else {
                var root = location.protocol + '//' + location.host + '/';
                var url = root + "Uslovi/" + path;
                var filename = url.substring(url.lastIndexOf("/") + 1).split("?")[0];
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'blob';
                xhr.onload = function () {
                    var a = document.createElement('a');
                    a.href = window.URL.createObjectURL(xhr.response); // xhr.response is a blob
                    a.download = filename; // Set the file name.
                    a.style.display = 'none';
                    document.body.appendChild(a);
                    a.click();
                    delete a;
                };
                xhr.open('GET', url);
                xhr.send();
            }
        }
        function opstiUslovi(opstiId) {
            $.ajax({

                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/GenerateOpstiUslovi")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{opstiId:' + opstiId + '}',
                success: function (result) {
                    hideLoading();

                    //$.fileDownload('../Uslovi/' + result.d);
                    downloadDoc(result.d);

                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }
        function posebniUslovi(posebniId) {
            $.ajax({

                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/GeneratePosebniUslovi")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{posebniId:' + posebniId + '}',
                success: function (result) {
                    hideLoading();

                    //$.fileDownload('Uslovi/' + result.d);
                    downloadDoc(result.d);

                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }


        $('#btnSave').click(function () {
            var parcela = $('#<%= fkParcel.ClientID%>').val();
            var brPredmet = $('#<%= brPredmet.ClientID%>').val();
            var dllTip = $("#<%= ddlTipBaranje.ClientID %>").val();
            var sluzbenik = $('#<%= sluzbenik.ClientID%>').val();
            var datumBaranja = $('#<%= datumBaranja.ClientID%>').val();
            var datumIzdavanja = $('#<%= datumIzdavanja.ClientID%>').val();
            var pravosilno = $('#<%= pravosilno.ClientID%>').val();
            var investitor = $('#<%= investitor.ClientID%>').val();
            var brKP = $('#<%= brKP.ClientID%>').val();
            var dllKO = $('#<%= ddlKO.ClientID%>').val();
            var adresa = $('#<%= adresa.ClientID%>').val();
            var parkMestoParc = $('#<%= parkMestoPacela.ClientID%>').val();
            var parkMestoGaraza = $('#<%= parkMestoGaraza.ClientID%>').val();
            var ddGarazi = $('#<%= DdlGarazi.ClientID%>').val();
            var iznosKomunalii = $('#<%= iznosKomunalni.ClientID%>').val();
            var zabeleska = $('#<%= zabeleska.ClientID%>').val();

            if (parcela === null) {
                parcela = '';
            }
            if (brPredmet === null) {
                brPredmet = '';
            }
            if (dllTip === null) {
                dllTip = '';
            }
            if (sluzbenik === null) {
                sluzbenik = '';
            }
            if (datumBaranja === null) {
                datumBaranja = '';
            }
            if (datumIzdavanja === null) {
                datumIzdavanja = '';
            }
            if (pravosilno === null) {
                pravosilno = '';
            }
            if (investitor === null) {
                investitor = '';
            }
            if (brKP === null) {
                brKP = '';
            }
            if (dllKO === null) {
                dllKO = '';
            }
            if (adresa === null) {
                adresa = '';
            }
            if (parkMestoParc === null) {
                parkMestoParc = '';
            }
            if (parkMestoGaraza === null) {
                parkMestoGaraza = '';
            }
            if (ddGarazi === null) {
                ddGarazi = '';
            }
            if (iznosKomunalii === null) {
                iznosKomunalii = '';
            }
            if (zabeleska === null) {
                zabeleska = '';
            }

            if (brPredmet && dllTip && sluzbenik && datumBaranja && datumIzdavanja && pravosilno && iznosKomunalii) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/Save")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{fkParcela:"' + $('#<%= fkParcel.ClientID%>').val() + '" , brPredmet:"' + $('#<%= brPredmet.ClientID%>').val() + '", tipBaranje: "' + $('#<%= ddlTipBaranje.ClientID %> option:selected').text() + '" , sluzbenik:"' + $('#<%= sluzbenik.ClientID%>').val() + '", datumBaranja:"' + $('#<%= datumBaranja.ClientID%>').val() + '", datumIzdavanja:"' + $('#<%= datumIzdavanja.ClientID%>').val() + '", datumPravosilno:"' + $('#<%= pravosilno.ClientID%>').val() + '", investitor:"' + $('#<%= investitor.ClientID%>').val() + '" , brKP:"' + $('#<%= brKP.ClientID%>').val() + '" , ko:"' + $('#<%= ddlKO.ClientID%> option:selected').text() + '" , adresa:"' + $('#<%= adresa.ClientID%>').val() + '" ,  parkingMestaParcela:"' + $('#<%= parkMestoPacela.ClientID%>').val() + '",  parkingMestaGaraza:"' + $('#<%= parkMestoGaraza.ClientID%>').val() + '", katnaGaraza:"' + $('#<%= DdlGarazi.ClientID%> option:selected').text() + '", iznosKomunalii:"' + $('#<%= iznosKomunalni.ClientID%>').val() + '", zabeleski:"' + $('#<%= zabeleska.ClientID%>').val() + '",podtipBaranje:"' + $("#ddlPodTipBaranje option:selected").text() + '"}',
                success: function (result) {
                    $('#<%= fkParcel.ClientID%>').val(""),
                    $('#<%= brPredmet.ClientID%>').val(""),
                    $("#<%= ddlTipBaranje.ClientID %>").val(""),
                    $('#<%= sluzbenik.ClientID%>').val(""),
                    $('#<%= datumBaranja.ClientID%>').val(""),
                    $('#<%= datumIzdavanja.ClientID%>').val(""),
                    $('#<%= pravosilno.ClientID%>').val(""),
                    $('#<%= investitor.ClientID%>').val(""),
                    $('#<%= brKP.ClientID%>').val(""),
                    $('#<%= ddlKO.ClientID%>').val(""),
                    $('#<%= adresa.ClientID%>').val(""),
                    $('#<%= parkMestoPacela.ClientID%>').val(""),
                    $('#<%= parkMestoGaraza.ClientID%>').val(""),
                    $('#<%= DdlGarazi.ClientID%>').val(""),
                    $('#<%= iznosKomunalni.ClientID%>').val(""),
                    $('#<%= zabeleska.ClientID%>').val(""),
                    $('#ddlPodTipBaranje').empty();



            },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
            }
            });
        }
        else{
                alert("Не се пополнети задолжителните полиња!");

        }


        })



    </script>
</asp:Content>
