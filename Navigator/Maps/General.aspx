<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="General.aspx.cs" Inherits="Navigator.Maps.General" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">

    
  <%--<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.0/css/bootstrap.min.css">.--%>
  <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>--%>
  <%--<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.0/js/bootstrap.min.js"></script>--%>


    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/openlayers/4.0.1/ol-debug.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/Settings.aspx")%>"></script>
    <%--<script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher.js")%>"></script>--%>
    <%--<link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">--%>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher-advances.js")%>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher-advanced.css")%>" type="text/css">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/layers.js") %>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.css")%>" type="text/css">
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.min.css")%>" type="text/css" />
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/measure.css")%>" type="text/css">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel"><%: DbRes.T("Rezultat", "Resources") %></h4>
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
                        <asp:FileUpload ID="file_upload" runat="server" AllowMultiple="true" />
                        <asp:Label ID="lblUploadStatus" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnCloseDocModel"><%: DbRes.T("Zatvori", "Resources") %></button>
                    <asp:Button ID="btnFileUpload" runat="server" Text="<%$ Resources:Zacuvaj %>" OnClick="btnFileUpload_Click" CssClass="btn btn-info" />
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

    <div class="modal fade" id="vnesInvestitorModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <b><%: DbRes.T("ImeSopstvenikInvestitor", "Resources") %></b>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnInvestitoClose"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div>
                        <input runat="server" type="text" id="vnesInvestitor" value="" class="form-control" />
                        <asp:RequiredFieldValidator ID="rfvInvestitor" ValidationGroup='valGroupInvestitor' runat="server" ControlToValidate="vnesInvestitor" CssClass="text-danger" ErrorMessage="Немате внесено инвеститор." />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnCloseInvestitorModel"><%: DbRes.T("Zatvori", "Resources") %></button>
                    <asp:Button ValidationGroup='valGroupInvestitor' ID="btnInvestitor" runat="server" Text="<%$ Resources:Zacuvaj %>" OnClick="btnInvestitor_Click" CssClass="btn btn-info" />
                </div>
            </div>
        </div>
    </div>
    <div id="mapG">
        <div class="edit_map_controlls">
            <a href="#" id="btnInfo" class="btn btn-sm btn-info" title="<%: DbRes.T("Info", "Resources") %>"><span class="glyphicon glyphicon-info-sign" aria-hidden="true"></span></a>
            <a href="#" id="btnExtent" class="btn btn-sm btn-info" title="<%: DbRes.T("PosledenRazmer", "Resources") %>" onclick="goToLastExtent()"><span class="glyphicon glyphicon-sort" aria-hidden="true"></span></a>
            <a href="#" id="btnDownload" class="btn btn-sm btn-info" title="<%: DbRes.T("IzvodOdPlan", "Resources") %>"><span class="glyphicon glyphicon-sort-by-attributes-alt" aria-hidden="true"></span></a>
            <a href="#" id="btnSearch" class="btn btn-sm btn-info" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>
            <a href="#" id="btnRubberZoom" class="btn btn-sm btn-info" title="<%: DbRes.T("ZumirajDel", "Resources") %>"><span class="glyphicon glyphicon-screenshot" aria-hidden="true"></span></a>
            <a href="#" runat="server" style="visibility: hidden" id="btnPolygon" class="btn btn-sm btn-info" title="Пресметување на површина"><span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span></a>
            <a href="#" runat="server" style="visibility: hidden" id="btnLine" class="btn btn-sm btn-info" title="Пресметување на должина"><span class="glyphicon glyphicon-resize-horizontal" aria-hidden="true"></span></a>
             <a href="#" runat="server" style="visibility: hidden" id="btnStreetCut" class="btn btn-sm btn-info" title="Извод од улица"><span class="glyphicon glyphicon-tag" aria-hidden="true"></span></a> 
            <a href="#" runat="server" style="visibility: hidden" id="btnDownloadDxf" class="btn btn-sm btn-info" title="<%$ Resources:PrevzemiDxf %>"><span class="glyphicon glyphicon-download-alt" aria-hidden="true"></span></a> 
            
       
            <a href="#" runat="server" style="visibility: hidden" id="btnVnesInvestitor" class="btn btn-sm btn-info" title="<%$ Resources:VnesiSopstvenik %>"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></a>
            <a href="#" runat="server" style="visibility: hidden" id="btnInsertDoc" class="btn btn-sm btn-info" title="<%$ Resources:VnesiDokument %>"><span class="glyphicon glyphicon-paperclip" aria-hidden="true"></span></a>
            <a href="#" runat="server" style="visibility: hidden" id="btnInfoDoc" class="btn btn-sm btn-info" title="<%$ Resources:InfoDokumenti %>"><span class="glyphicon glyphicon-list" aria-hidden="true"></span></a>
          
           

            <div id="divPopup" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content" style="width: 560px; right: 60px">
                        <div class="modal-body">
                            <ul class="nav nav-tabs" id="tabovi">
                                <li class="active"><a id="aBrzoP" href="#brzoP" data-toggle="tab"><%: DbRes.T("SiteSloevi", "Resources") %></a></li>
                                <li class="" runat="server" id="gp"><a id="gParceli" href="#gradParceli" data-toggle="tab">Градежни парцели</a></li>
                                <li class="" style="visibility: hidden" runat="server" id="kp"><a id="aDolgoP" href="#dolgoP" data-toggle="tab"><%: DbRes.T("KatParceli", "Resources") %></a></li>
                                <li class="" style="visibility: hidden" runat="server" id="adresi"><a id="aAdresiP" href="#adresiP" data-toggle="tab"><%: DbRes.T("Adresi", "Resources") %></a></li>
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
                                <div class="tab-pane fade" id="gradParceli">
                                    <div class="input-group" style="width: 440px;">
                                        <div>
                                            <select id="selectOpfat" class="js-example-placeholder-single form-control" style="width: 440px;" runat="server"></select>
                                        </div>
                                        <br />
                                        <div>
                                            <select id="selectGParceli" class="js-example-placeholder-single form-control" style="max-width: 100%;"></select>
                                        </div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchOpfati" class="btn btn-link" title="Зум"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel4" runat="server" Height="220px">
                                        <div id="progressParceli" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultPaceli"></div>
                                    </asp:Panel>
                                </div>
                                <div class="tab-pane fade" id="adresiP">
                                    <div class="input-group" style="width: 310px;">
                                        <div>
                                            <select id="selectUlica" class="js-example-placeholder-single form-control" style="width: 100%" runat="server"></select>
                                        </div>
                                        <br />
                                        <div>
                                            <select id="selectBroj" class="js-example-placeholder-single form-control" style="width: 100%"></select>
                                        </div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchAdresi" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel2" runat="server" Height="220px">
                                        <div id="progressAdresi" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultAdresi"></div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="divPopupStreet" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content" style="width: 560px; right: 60px">
                        <div class="modal-body">
                            <ul class="nav nav-tabs" id="taboviAdresi">

                                <li class="active" runat="server" id="adresiTab"><a id="aAdresiTabP" href="#adresiTabP" data-toggle="tab"><%: DbRes.T("Adresi", "Resources") %></a></li>
                            </ul>
                            <div class="tab-content">


                                <div class="tab-pane fade active in" id="adresiTabP">
                                    <div class="input-group" style="width: 310px;">
                                        <div>
                                            <select id="selectUlicaN" class="js-example-placeholder-single form-control" style="width: 100%"></select>
                                        </div>
                                        <br />
                                        <%-- <div>
                                            <select id="selectBrojN" class="js-example-placeholder-single form-control" style="width: 100%"></select>
                                        </div>--%>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnDownloadIzvodUlica" class="btn btn-link" title="Извод од улица"><span class="glyphicon glyphicon-download"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel8" runat="server" Height="220px">
                                        <div id="progressAdresiN" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultAdresiN"></div>
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
    <div id="dataSearchA"></div>
    <div id="dataSearchKParceli"></div>
    <asp:HiddenField ID="selectedNum" runat="server" Value="" />
    <asp:HiddenField ID="fkDocParcel" runat="server" Value="" />
    <asp:HiddenField ID="coordinate" runat="server" Value="" />
    <asp:HiddenField ID="coordinates" runat="server" Value="" />
    <asp:HiddenField ID="polygon" runat="server" Value="" />
    <asp:HiddenField ID="coordinateX" runat="server" Value="" />
    <asp:HiddenField ID="coordinateY" runat="server" Value="" />
    <p class="bl" id="razmer"></p>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/general.js") %>"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script type="text/javascript">
        function formatDate(datum) {
            var monthNames = ["Јануари", "Февруари", "Март", "Април", "Мај", "Јуни", "Јули", "Август", "Септември", "Октомври", "Ноември", "Декември"];
            var d = new Date(datum);
            var month = (d.getMonth() + 1);
            if (d.getFullYear() != 9999) {
                if (d.getDate() < 10) {
                    return '0' + d.getDate() + ' ' + monthNames[d.getMonth()] + ' ' + d.getFullYear();
                }
                else {
                    return d.getDate() + ' ' + monthNames[d.getMonth()] + ' ' + d.getFullYear();
                }
            }
            else return '/'
        };
        $(document).ready(function () {
            $('#<%= selectUlica.ClientID%>').select2({ placeholder: '<%: DbRes.T("IzbereteUlica", "Resources") %>', allowClear: true });
            $('#<%= selectUlica.ClientID%>').val("");
            $('#<%= selectUlica.ClientID%>').trigger("change.select2");



            $('#<%= selectOpfat.ClientID%>').select2({ placeholder: 'Изберете опфат', allowClear: true });
            $('#<%= selectOpfat.ClientID%>').val("");
            $('#<%= selectOpfat.ClientID%>').trigger("change.select2");
        });
        $('#<%= selectUlica.ClientID%>').on("change", function (e) {
            $("#selectBroj").html('').select2({ data: [{ ulica: '', text: '' }] }).empty();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/FillNumbers")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{ulica:"' + $('#<%= selectUlica.ClientID%>').val() + '"}',
                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        $('#selectBroj').select2({ placeholder: '<%: DbRes.T("IzbereteBroj", "Resources") %>', allowClear: true, data: jsonResult });
                        $('#selectBroj').val("");
                        $('#selectBroj').trigger("change.select2");
                    }
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });

        });
      <%--  $('#<%= selectUlicaN.ClientID%>').on("change", function (e) {
            $("#selectBrojN").html('').select2({ data: [{ ulica: '', text: '' }] }).empty();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/FillNumbers")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{ulica:"' + $('#<%= selectUlicaN.ClientID%>').val() + '"}',
                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        $('#selectBrojN').select2({ placeholder: '<%: DbRes.T("IzbereteBroj", "Resources") %>', allowClear: true, data: jsonResult });
                        $('#selectBrojN').val("");
                        $('#selectBrojN').trigger("change.select2");
                    }
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });

        });--%>



        $('#<%= selectOpfat.ClientID%>').on("change", function (e) {
            $("#selectGParceli").html('').select2({ data: [{ id: '', text: '' }] }).empty();
            console.log($('#<%= selectOpfat.ClientID%>').val());
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/FillParceli")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{opfat:"' + $('#<%= selectOpfat.ClientID%>').val() + '"}',
                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        console.log(jsonResult);
                        $('#selectGParceli').select2({ placeholder: 'Избери парцела', allowClear: true, data: jsonResult });
                        $('#selectGParceli').val("");
                        $('#selectGParceli').trigger("change.select2");
                    }
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });

        });







        //forsiraj enter kaj prebaruvanje
        $(document).keypress(function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });
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
        $("#txtSearchAdresi").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btnSubmitSearchAdresi").click();
            }
        });


        function showModal() {
            $('#infoModal').modal('show');
        };
        function showInfoDocModal() {
            $('#infoDocModal').modal('show');
        };
        function showVnesInvestitorModal() {
            $('#vnesInvestitorModal').modal('show');
        };
        function showLoading() {
            $('#myLoadingModal').modal('show');
        };
        function hideLoading() {
            $('#myLoadingModal').modal('hide');
        };
        function showVnesDocModal() {
            $('#vnesDocModal').modal('show');
        };
        function CloseDocModel() {
            $('#vnesDocModal').modal('hide');
        };
        function CloseInvestitorModel() {
            $('#vnesInvestitorModal').modal('hide');
        };
        $('#btnCloseDocModel').click(function () {

            document.getElementById('<%= file_upload.ClientID %>').value = "";
            document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
            CloseDocModel();
        });
        $('#btnDocClose').click(function () {

            document.getElementById('<%= file_upload.ClientID %>').value = "";
            document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
            CloseDocModel();
        });
        $('#btnCloseInvestitorModel').click(function () {

            document.getElementById('<%= vnesInvestitor.ClientID %>').value = "";
              document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
              CloseInvestitorModel();
          });
          $('#btnInvestitoClose').click(function () {

              document.getElementById('<%= vnesInvestitor.ClientID %>').value = "";
            document.getElementById('<%= fkDocParcel.ClientID %>').value = "";
            CloseInvestitorModel();
        });
        var infoTool = false;
        var izvodTool = false;
        var searchTool = false;
        var rubberZoomTool = false;
        var dxfTool = false;
        var insertDocTool = false;
        var infoDocTool = false;
        var investitorTool = false;
        var polygonTool = false;
        var lineTool = false;
        var typeSelect = 'None';
        var draw; // global so we can remove it later
        var delUlicaTool = false;
        var source = new ol.source.Vector({ wrapX: false });

        $('#btnInfo').click(function () {
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (infoTool) {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
            } else {
                $('#btnInfo').addClass("btn-danger");
                $('#btnInfo').removeClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            infoTool = !infoTool;
        });
        $('#btnDownload').click(function () {
            infoTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (izvodTool) {
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
            } else {
                $('#btnDownload').addClass("btn-danger");
                $('#btnDownload').removeClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            izvodTool = !izvodTool;
        });
        $('#btnSearch').click(function () {
            infoTool = false;
            izvodTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopupStreet').hide();
            $('#divPopup').toggle();
            if (searchTool) {
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
            } else {
                $('#btnSearch').addClass("btn-danger");
                $('#btnSearch').removeClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            searchTool = !searchTool;
        });
        $('#btnRubberZoom').click(function () {
            dxfTool = false;
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
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
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'Draw';
                map.removeInteraction(draw);
                addInteraction();
            }
            rubberZoomTool = !rubberZoomTool;
        });
        $("#<%= btnDownloadDxf.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (dxfTool) {
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
            } else {
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();

            }
            dxfTool = !dxfTool;
        });
        $("#<%= btnInsertDoc.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (insertDocTool) {
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            insertDocTool = !insertDocTool;
        });
        $("#<%= btnInfoDoc.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (insertDocTool) {
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-info");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            infoDocTool = !infoDocTool;
        });
        $("#<%= btnVnesInvestitor.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            polygonTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (investitorTool) {
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
            } else {
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnVnes').removeClass("btn-danger");
                $('#btnVnes').addClass("btn-info");
                $('#btnView').removeClass("btn-danger");
                $('#btnView').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $('#btnInfoDoc').removeClass("btn-danger");
                $('#btnInfoDoc').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                map.removeInteraction(draw);
                addInteraction();
            }
            investitorTool = !investitorTool;
        });
        $("#<%= btnPolygon.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            lineTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (polygonTool) {
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'area';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            polygonTool = !polygonTool;
        });

        $("#<%= btnLine.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            delUlicaTool = false;
            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (lineTool) {
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'length';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            lineTool = !lineTool;
        });


        $("#<%= btnStreetCut.ClientID %>").click(function () {
            infoTool = false;
            izvodTool = false;
            searchTool = false;
            rubberZoomTool = false;
            dxfTool = false;
            insertDocTool = false;
            infoDocTool = false;
            investitorTool = false;
            polygonTool = false;
            lineTool = false;

            $('#divPopup').hide();
            $('#divPopupStreet').hide();
            if (delUlicaTool) {
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-info");
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                map.removeInteraction(draw);
                addInteractionSoobTool();
            } else {
                $("#<%= btnStreetCut.ClientID %>").addClass("btn-danger");
                $("#<%= btnStreetCut.ClientID %>").removeClass("btn-info");
                $('#btnDownload').removeClass("btn-danger");
                $('#btnDownload').addClass("btn-info");
                $("#<%= btnDownloadDxf.ClientID %>").removeClass("btn-danger");
                $("#<%= btnDownloadDxf.ClientID %>").addClass("btn-info");
                $('#btnInfo').removeClass("btn-danger");
                $('#btnInfo').addClass("btn-info");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnSearch').addClass("btn-info");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnRubberZoom').addClass("btn-info");
                $("#<%= btnInsertDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInsertDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnInfoDoc.ClientID %>").removeClass("btn-danger");
                $("#<%= btnInfoDoc.ClientID %>").addClass("btn-info");
                $("#<%= btnVnesInvestitor.ClientID %>").removeClass("btn-danger");
                $("#<%= btnVnesInvestitor.ClientID %>").addClass("btn-info");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $("#<%= btnLine.ClientID %>").addClass("btn-info");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'Draw';
                map.removeInteraction(draw);
                addInteractionSoobTool();
            }

            delUlicaTool = !delUlicaTool;
        });


        map.on('singleclick', function (evt) {
            if (infoTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/InfoTool")%>",
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
                                        var opstina = '<%=ConfigurationManager.AppSettings["opstina"].ToString() %>';
                                        if (opstina == "ЦЕНТАР") {
                                            tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("BrojNaOdluka", "Resources") %></th><th>Датум</th><th><%: DbRes.T("ZakonskaRegulativa", "Resources") %></th><th><%: DbRes.T("Izrabotuva", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th></tr>';
                                            for (var item in jsonResult1) {
                                                tableString += '<tr><td>' + jsonResult1[item].BrOdluka + '</td><td>' + formatDate(jsonResult1[item].DatumNaDonesuvanje) + '</td><td>' + jsonResult1[item].ZakonskaRegulativa + '</td><td>' + jsonResult1[item].Izrabotuva + '</td><td>' + jsonResult1[item].Povrshina + '</td></tr>';
                                            }
                                            tableString += '</table>';
                                            $('#divInfo').append(tableString);
                                        } else {
                                            tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("BrojNaOdluka", "Resources") %></th><th><%: DbRes.T("ZakonskaRegulativa", "Resources") %></th><th><%: DbRes.T("Izrabotuva", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th></tr>';
                                            for (var item in jsonResult1) {
                                                tableString += '<tr><td>' + jsonResult1[item].BrOdluka + '</td><td>' + jsonResult1[item].ZakonskaRegulativa + '</td><td>' + jsonResult1[item].Izrabotuva + '</td><td>' + jsonResult1[item].Povrshina + '</td></tr>';
                                            }
                                            tableString += '</table>';
                                            $('#divInfo').append(tableString);
                                        }

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
                                        tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("MaksimalnaVisina", "Resources") %></th><th><%: DbRes.T("Katnost", "Resources") %></th><th><%: DbRes.T("ParkingMesta", "Resources") %></th><th><%: DbRes.T("ProcentNaIzgradenost", "Resources") %></th><th><%: DbRes.T("KoeficientNaIskoristenost", "Resources") %></th><th><%: DbRes.T("Investitor", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + nullToEmptyString(jsonResult1[item].MaxVisina) + '</td><td>' + nullToEmptyString(jsonResult1[item].Katnost) + '</td><td>' + nullToEmptyString(jsonResult1[item].ParkingMesta) + '</td><td>' + nullToEmptyString(jsonResult1[item].ProcentIzgradenostOpisno) + '</td><td>' + nullToEmptyString(jsonResult1[item].KoeficientIskoristenostOpisno) + '</td><td>' + nullToEmptyString(jsonResult1[item].Investitor) + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                        for (var item in jsonResult1) {
                                            if (jsonResult1[item].OpstiUsloviId != null && jsonResult1[item].PosebniUsloviId != null) {
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="opstiUslovi(\'' + jsonResult1[item].OpstiUsloviId + '\');return false;"><%: DbRes.T("PrevzemiOpstiUslovi", "Resources") %></a>');
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="posebniUslovi(\'' + jsonResult1[item].PosebniUsloviId + '\');return false;"><%: DbRes.T("PrevzemiPosebniUslovi", "Resources") %></a>');
                                                //$('#divInfo').append('<a href="#" class="btn btn-link" onclick="tehnickiIspravki(\'' + jsonResult1[item].TehnickiIspravkiId + '\');return false;">Превземи технички исправки</a>');
                                                
                                            }
                                             if (jsonResult1[item].TehnickiIspravkiId != null) {
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="tehnickiIspravki(\'' + jsonResult1[item].TehnickiIspravkiId + '\');return false;">Превземи технички исправки</a>');
                                            }
                                            if (jsonResult1[item].NumerickiPokazateliId != null) {
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="numerickiPokazateli(\'' + jsonResult1[item].NumerickiPokazateliId + '\');return false;">Превземи нумерички показатели</a>');
                                            }
                                        }
                                        goToGeojsonExtent();
                                    }
                                }

                                if (key === "ListAvtobuska") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Автобуска станица</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Ime", "Resources") %></th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Ime + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListOsvetluvanje") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Осветлување</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Поставеност</th><th>Состојба</th><th>Тип на осветлување</th><th>Тип на сијалица</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Postavenost + '</td><td>' + jsonResult1[item].Sostojba + '</td><td>' + jsonResult1[item].TipOsvetluvanje + '</td><td>' + jsonResult1[item].TipSijalica + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListDefekt") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Дефекти</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Текст</th><th>Тип</th><th>Слика</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Text + '</td><td>' + jsonResult1[item].Tip + '</td><td><img style="max-width:200px" src="../Uploads/' + jsonResult1[item].Slika + '"></td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListJavnaPovrsina") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Јавна површина</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Коментар</th><th>Тип</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Komentar + '</td><td>' + jsonResult1[item].Tip + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListOprema") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Опрема</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Состојба</th><th>Тип</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Sostojba + '</td><td>' + jsonResult1[item].Tip + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListVodovod") {
                                    var jsonResult1 = jsonResult[key];
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Водовод</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Тип</th><th>Тип</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Sifra + '</td><td>' + jsonResult1[item].Tip + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
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
            } else if (dxfTool) {
                showLoading();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/GenerateDxfFile")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        hideLoading();
                        $.fileDownload('../Dxf/' + result.d);
                        console.log(result.d)
                    },
                    error: function (p1, p2, p3) {
                        hideLoading();
                        alert(p1.status);
                        alert(p3);
                    }
                });
            } else if (insertDocTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/InfoTool")%>",
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
            } else if (infoDocTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/InfoDocTool")%>",
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
                                    tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].Dup) + '</td><td><a href="#" class="btn btn-link" onclick="downloadGeneralDoc(\'' + jsonResult[key].Path + '\');return false;"><span class="glyphicon glyphicon-paperclip" aria-hidden="true"></span></a></td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult[key].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-zoom-in" aria-hidden="true"></span></a></td></tr>';
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
        } else if (investitorTool) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/InfoTool")%>",
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
                            showVnesInvestitorModal();
                        }
                    },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                }
            });
            } else if (izvodTool) {
                showLoading();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/IzvodTool")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        hideLoading();
                        console.log(result.d);
                        $.fileDownload('../Izvodi/' + result.d);
                    },
                    error: function (p1, p2, p3) {
                        hideLoading();
                        alert(p1.status);
                        alert(p3);
                    }
                });
            }

<%--            else if (izvodUlicaTool) {
                showLoading();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/IzvodUlica")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        hideLoading();
                        console.log(result.d);
                        $.fileDownload('../Izvodi/' + result.d);
                    },
                    error: function (p1, p2, p3) {
                        hideLoading();
                        alert(p1.status);
                        alert(p3);
                    }
                });
            }--%>


        });


        $('#btnSubmitSearch').click(function () {
            if ($('#txtSearch').val().length != 0) {
                $('#progress').show();
                var myString = $('#txtSearch').val();
                var searchTxt = myString.replace(/"/gi, "quotes");
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/SearchOut")%>",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{searchString:"' + searchTxt + '"}',
            success: function (result) {
                $('#divSearchResult').empty();
                $('#dataSearch').removeData();
                var jsonResult = JSON.parse(result.d);
                for (var key in jsonResult) {
                    if (jsonResult.hasOwnProperty(key)) {
                        if (key === "ListOpfat") {
                            var jsonResult1 = jsonResult[key];
                            if (jsonResult1 !== null && jsonResult1.length > 0) {
                                $('#divSearchResult').append('<h4><%: DbRes.T("Opfat", "Resources") %></h4>');
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
                                $('#divSearchResult').append('<h4><%: DbRes.T("Blok", "Resources") %></h4>');
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
        } else {
            alert("Немате внесено текст за пребарување");
        }
});
    $('#btnSubmitSearchDolgo').click(function () {
        if ($('#txtSearchDolgo').val().length != 0) {
            $('#progressDolgo').show();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/SearchOutK")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{searchString:"' + $('#txtSearchDolgo').val() + '"}',
                success: function (result) {
                    $('#divSearchResultDolgo').empty();

                    $('#dataSearchKParceli').removeData();
                    var jsonResult = JSON.parse(result.d);
                    for (var key in jsonResult) {
                        if (jsonResult.hasOwnProperty(key)) {
                            if (key === "ListKatastarskaParcela") {
                                var jsonResult1 = jsonResult[key];
                                if (jsonResult1 !== null && jsonResult1.length > 0) {
                                    $('#divSearchResultDolgo').append('<h4><%: DbRes.T("KatParceli", "Resources") %></h4>');
                                    var tableString2 = '<table class="table table-striped table-hover"><tr><th><%: DbRes.T("Broj", "Resources") %></th><th><%: DbRes.T("Lokacija", "Resources") %></th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';

                                    for (var item in jsonResult1) {
                                        $('#dataSearchKParceli').data('kparceli' + jsonResult1[item].Id, jsonResult1[item].GeoJson);

                                        tableString2 += '<tr><td>' + jsonResult1[item].Name + '</td><td>' + jsonResult1[item].Location + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtentKParceli(\'' + 'kparceli' + jsonResult1[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';

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
        } else {
            alert("Немате внесено текст за пребарување");
        }
    });

    $('#btnSubmitSearchAdresi').click(function () {
        $('#progressAdresi').show();
        $.ajax({
            type: "POST",
            url: "<%= Page.ResolveUrl("~/Maps/General.aspx/SearchOutA")%>",
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: '{ulica:"' + $('#<%= selectUlica.ClientID%>').val() + '", broj:"' + $('#selectBroj').val() + '"}',
                success: function (result) {
                    $('#divSearchResultAdresi').empty();
                    $('#dataSearchA').removeData();
                    var jsonResult = JSON.parse(result.d);
                    $('#dataSearchA').data(jsonResult.Id.toString(), jsonResult.GeoJson);
                    goToGeojsonExtent5(jsonResult.Id.toString());
                    //for (var key in jsonResult) {
                    //    if (jsonResult.hasOwnProperty(key)) {
                    //        if (key === "ListKatastarskaParcela") {
                    //            var jsonResult1 = jsonResult[key];
                    //            if (jsonResult1 !== null && jsonResult1.length > 0) {
                    //                $('#divSearchResultAdresi').append('<h4>Катастарски парцели</h4>');
                    //                var tableString2 = '<table class="table table-striped table-hover"><tr><th>Број</th><th>Локација</th><th>Зум</th></tr>';

                    //                for (var item in jsonResult1) {
                    //                    $('#dataSearchA').data(jsonResult1[item].Id.toString(), jsonResult1[item].GeoJson);
                    //                    goToGeojsonExtent();
                    //                    console.log(jsonResult);
                    //                    tableString2 += '<tr><td>' + jsonResult1[item].Name + '</td><td>' + jsonResult1[item].Location + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + jsonResult1[item].Id.toString() + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';

                    //                }
                    //                tableString2 += '</table>';
                    //                $('#divSearchResultDolgo').append(tableString2);
                    //            }
                    //        }
                    //    }
                    //}

                    $('#progressAdresi').hide();
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                    $('#progressDolgo').hide();
                }
        });
    });


        $('#btnSubmitSearchUlici').click(function () {
            $('#progressAdresiN').show();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/SearchOutA")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{ulica:"' + $('#selectUlicaN').val() + '", broj:"' + $('#selectBrojN').val() + '"}',


                success: function (result) {
                    $('#divSearchResultAdresiN').empty();
                    $('#dataSearchA').removeData();
                    var jsonResult = JSON.parse(result.d);
                    $('#dataSearchA').data(jsonResult.Id.toString(), jsonResult.GeoJson);
                    goToGeojsonExtent5(jsonResult.Id.toString());

                    $('#progressAdresiN').hide();
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                    $('#progressDolgo').hide();
                }
            });
        });


        $('#btnDownloadIzvodUlica').click(function () {

            var poligon = polygon;
            var ulica_id = $('#selectUlicaN').val();

           
                showLoading();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Maps/General.aspx/IzvodUlica")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{poligon:"' + poligon + '", ulica_id:"' + ulica_id + '"}',

                    success: function (result) {
                        hideLoading();
                        console.log(result.d);
                        $.fileDownload('../Izvodi/' + result.d);
                    },
                    error: function (p1, p2, p3) {
                        alert(p1.status);
                        alert(p3);

                    }
                });
            
        });



        $('#btnSubmitSearchOpfati').click(function () {
            $('#progressParceli').show();

            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/SearchOutGP")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:"' + $('#selectGParceli').val() + '"}',
                success: function (result) {

                    $('#data').removeData();
                    var jsonResult = JSON.parse(result.d);
                    console.log(result.d);
                    console.log(jsonResult.Id);
                    console.log(jsonResult.GeoJson);
                    $('#data').data('' + jsonResult.Id, jsonResult.GeoJson);
                    goToGeojsonExtent();


                    $('#progressParceli').hide();
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                    $('#progressDolgo').hide();
                }
            });
        });

        //function addInteraction() {
        //    if (typeSelect !== 'None') {
        //        value = 'LineString';
        //        maxPoints = 2;
        //        geometryFunction = function (coordinates, geometry) {
        //            if (!geometry) {
        //                geometry = new ol.geom.Polygon(null);
        //            }
        //            var start = coordinates[0];
        //            var end = coordinates[1];
        //            geometry.setCoordinates([
        //              [start, [start[0], end[1]], end, [end[0], start[1]], start]
        //            ]);
        //            return geometry;
        //        };

        //        draw = new ol.interaction.Draw({
        //            source: source,
        //            type: /** @type {ol.geom.GeometryType} */ (value),
        //            geometryFunction: geometryFunction,
        //            maxPoints: maxPoints
        //        });

        //        //zumiranje to toa sto e nacrtano
        //        draw.on('drawend', function (e) {

        //            // koordinatite od nactaniot pravoagolnik
        //            //  alert(e.feature.getGeometry().getExtent());
        //            var extent = e.feature.getGeometry().getExtent();
        //            map.getView().fit(extent, map.getSize());

        //        });
        //        map.addInteraction(draw);
        //    }
        //}

        function nullToEmptyString(obj) {
            if (obj) {
                return obj;
            }
            return "/";

        }

        function replaceCodes(obj) {
            if (obj) {
                if (obj == -1) return "Постојна состојба";
                if (obj == -2) return "со Архитектонско-урбанистички проект";
                if (obj == -3) return "според АУП";
                return obj;
            }
            return "/";

        }
        function downloadGeneralDoc(path) {
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

       

        function numerickiPokazateli(numerickiId) {
            $.ajax({

                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/GenerateNumerickiPokazateli")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{numerickiId:' + numerickiId + '}',
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
         function tehnickiIspravki(tehnickiIspravkiId) {
            $.ajax({

                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/OdobrenieZaGradba/OdobrenieGradba.aspx/GenerateTehnickiIspravki")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{tehnickiIspravkiId:' + tehnickiIspravkiId + '}',
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

        $('#selectBroj').on("change", function (e) {
            document.getElementById('<%= selectedNum.ClientID %>').value = "";
            var selectNum = $('#selectBroj').val();

            $("#<%= selectedNum.ClientID %>").val(selectNum);
        });





        var globalData = "";
        var clearMeasurments = function () {
            clearSourceMeasure();
            if (measureTooltipElement) {
                $(".tooltip").remove();
            }
        }

        var wgs84Sphere = new ol.Sphere(6378137);

        /**
       * Currently drawn feature.
       * @type {ol.Feature}
       */
        var sketch;


        /**
         * The measure tooltip element.
         * @type {Element}
         */
        var measureTooltipElement;


        /**
         * Overlay to show the measurement.
         * @type {ol.Overlay}
         */
        var measureTooltip;


        /**
         * Handle pointer move.
         * @param {ol.MapBrowserEvent} evt The event.
         */
        pointerMoveHandler = function (evt) {
            if (evt.dragging) {
                return;
            }
        };

        var geodesicCheckbox = true;

        map.on('pointermove', pointerMoveHandler);

        /**
         * Format length output.
         * @param {ol.geom.LineString} line The line.
         * @return {string} The formatted length.
         */
        var formatLength = function (line) {
            var length;
            if (geodesicCheckbox) {
                var coordinates = line.getCoordinates();
                length = 0;
                var sourceProj = map.getView().getProjection();
                for (var i = 0, ii = coordinates.length - 1; i < ii; ++i) {
                    var c1 = ol.proj.transform(coordinates[i], sourceProj, 'EPSG:4326');
                    var c2 = ol.proj.transform(coordinates[i + 1], sourceProj, 'EPSG:4326');
                    length += wgs84Sphere.haversineDistance(c1, c2);
                }
            } else {
                length = Math.round(line.getLength() * 100) / 100;
            }
            var output;
            if (length > 100) {
                output = (Math.round(length / 1000 * 100) / 100) +
                    ' ' + 'km';
            } else {
                output = (Math.round(length * 100) / 100) +
                    ' ' + 'm';
            }
            return output;
        };


        /**
         * Format area output.
         * @param {ol.geom.Polygon} polygon The polygon.
         * @return {string} Formatted area.
         */
        var formatArea = function (polygon) {
            var area;
            if (geodesicCheckbox) {
                var sourceProj = map.getView().getProjection();
                var geom = /** @type {ol.geom.Polygon} */(polygon.clone().transform(
                    sourceProj, 'EPSG:4326'));
                var coordinates = geom.getLinearRing(0).getCoordinates();
                area = Math.abs(wgs84Sphere.geodesicArea(coordinates));
            } else {
                area = polygon.getArea();
            }
            var output;
            if (area > 10000) {
                output = (Math.round(area / 1000000 * 100) / 100) +
                    ' ' + 'km<sup>2</sup>';
            } else {
                output = (Math.round(area * 100) / 100) +
                    ' ' + 'm<sup>2</sup>';
            }
            return output;
        };
        function addInteraction() {

            if (typeSelect !== 'None') {
                if (typeSelect == 'Draw') {
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
                else {
                    var type = (typeSelect == 'area' ? 'Polygon' : 'LineString');
                    draw = new ol.interaction.Draw({
                        source: source,
                        type: /** @type {ol.geom.GeometryType} */ (type),
                        style: new ol.style.Style({
                            fill: new ol.style.Fill({
                                color: 'rgba(255, 255, 255, 0.2)'
                            }),
                            stroke: new ol.style.Stroke({
                                color: 'rgba(0, 0, 0, 0.5)',
                                lineDash: [10, 10],
                                width: 2
                            }),
                            image: new ol.style.Circle({
                                radius: 5,
                                stroke: new ol.style.Stroke({
                                    color: 'rgba(0, 0, 0, 0.7)'
                                }),
                                fill: new ol.style.Fill({
                                    color: 'rgba(255, 255, 255, 0.2)'
                                })
                            })
                        })
                    });
                    map.addInteraction(draw);

                    createMeasureTooltip();

                    var listener;
                    draw.on('drawstart',
                        function (evt) {
                            // set sketch
                            sketch = evt.feature;

                            /** @type {ol.Coordinate|undefined} */
                            var tooltipCoord = evt.coordinate;

                            listener = sketch.getGeometry().on('change', function (evt) {
                                var geom = evt.target;
                                var output;
                                if (geom instanceof ol.geom.Polygon) {
                                    output = formatArea(geom);
                                    tooltipCoord = geom.getInteriorPoint().getCoordinates();
                                } else if (geom instanceof ol.geom.LineString) {
                                    output = formatLength(geom);
                                    tooltipCoord = geom.getLastCoordinate();
                                }
                                measureTooltipElement.innerHTML = output;
                                measureTooltip.setPosition(tooltipCoord);
                            });
                        }, this);

                    draw.on('drawend',
                        function () {
                            measureTooltipElement.className = 'tooltip tooltip-static';
                            measureTooltip.setOffset([0, -7]);
                            // unset sketch
                            sketch = null;
                            // unset tooltip so that a new one can be created
                            measureTooltipElement = null;
                            createMeasureTooltip();
                            ol.Observable.unByKey(listener);
                        }, this);
                }
            }
            createMeasureTooltip();
        }

        /**
        * Creates a new measure tooltip
        */
        function createMeasureTooltip() {
            //if (measureTooltipElement) {
            //    measureTooltipElement.parentNode.removeChild(measureTooltipElement);
            //}
            measureTooltipElement = document.createElement('div');
            measureTooltipElement.className = 'tooltip tooltip-measure';
            measureTooltip = new ol.Overlay({
                element: measureTooltipElement,
                offset: [0, -15],
                positioning: 'bottom-center'
            });
            map.addOverlay(measureTooltip);
        }

        addInteraction();


     <%--   function addInteractionSoobTool() {
            var value = 'Polygon';
            if (value !== 'None') {
                draw = new ol.interaction.Draw({
                    source: sourceDraw,
                    type: ('Polygon')
                });
                map.addInteraction(draw);

                draw.on('drawend', function (e) {
                    setTimeout(function () { draw.setActive(false); }, 100);
                    //console.log(e.feature.getGeometry());
                    //SerializeMapData(e.feature);
                    var writer = new ol.format.GeoJSON();
                    geojsonStr = writer.writeGeometry(e.feature.getGeometry());
                    $("#<%= coordinates.ClientID %>").val(geojsonStr);
                    console.log(geojsonStr);
                });

            }
        }--%>


        function addInteractionSoobTool() {

            if (typeSelect !== 'None') {
                if (typeSelect == 'Draw') {
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
                    //draw.on('drawend', function (e) {

                    //    // koordinatite od nactaniot pravoagolnik
                    //    //  alert(e.feature.getGeometry().getExtecentnt());
                    //    var extent = e.feature.getGeometry().getExtent();
                    //    map.getView().fit(extent, map.getSize());

                    //});

                    draw.on('drawend', function (e) {
                        setTimeout(function () { draw.setActive(false); }, 100);
                        //console.log(e.feature.getGeometry());
                        //SerializeMapData(e.feature);
                        var writer = new ol.format.GeoJSON();
                        geojsonStr = writer.writeGeometry(e.feature.getGeometry());
                      <%--  $("#<%= coordinates.ClientID %>").val(geojsonStr);
                        var poligon = document.getElementById('<%= coordinates.ClientID %>').value;--%>

                        var geojson = JSON.parse(geojsonStr);
                        var coordinatesArr = geojson.coordinates;
                        var poligonKoordinati = JSON.stringify(coordinatesArr);
                        //console.log("sojson"+JSON.stringify(coordinatesArr));
                        //console.log("poligonKoordinatin" + poligonKoordinati);

                        $("#<%= polygon.ClientID %>").val(poligonKoordinati);
                        polygon = JSON.stringify(coordinatesArr);
                        //console.log(polygon);
                        coordinateX = polygon[1];
                        //console.log(coordinateX);
                        getAdresi(poligonKoordinati);
                        $('#divPopupStreet').toggle();


                    });

                    map.addInteraction(draw);
                }
                else {
                    var type = (typeSelect == 'area' ? 'Polygon' : 'LineString');
                    draw = new ol.interaction.Draw({
                        source: source,
                        type: /** @type {ol.geom.GeometryType} */ (type),
                        style: new ol.style.Style({
                            fill: new ol.style.Fill({
                                color: 'rgba(255, 255, 255, 0.2)'
                            }),
                            stroke: new ol.style.Stroke({
                                color: 'rgba(0, 0, 0, 0.5)',
                                lineDash: [10, 10],
                                width: 2
                            }),
                            image: new ol.style.Circle({
                                radius: 5,
                                stroke: new ol.style.Stroke({
                                    color: 'rgba(0, 0, 0, 0.7)'
                                }),
                                fill: new ol.style.Fill({
                                    color: 'rgba(255, 255, 255, 0.2)'
                                })
                            })
                        })
                    });
                    map.addInteraction(draw);

                    createMeasureTooltip();

                    var listener;
                    draw.on('drawstart',
                        function (evt) {
                            // set sketch
                            sketch = evt.feature;

                            /** @type {ol.Coordinate|undefined} */
                            var tooltipCoord = evt.coordinate;

                            listener = sketch.getGeometry().on('change', function (evt) {
                                var geom = evt.target;
                                var output;
                                if (geom instanceof ol.geom.Polygon) {
                                    output = formatArea(geom);
                                    tooltipCoord = geom.getInteriorPoint().getCoordinates();
                                } else if (geom instanceof ol.geom.LineString) {
                                    output = formatLength(geom);
                                    tooltipCoord = geom.getLastCoordinate();
                                }
                                measureTooltipElement.innerHTML = output;
                                measureTooltip.setPosition(tooltipCoord);
                            });
                        }, this);

                    draw.on('drawend',
                        function () {
                            measureTooltipElement.className = 'tooltip tooltip-static';
                            measureTooltip.setOffset([0, -7]);
                            // unset sketch
                            sketch = null;
                            // unset tooltip so that a new one can be created
                            measureTooltipElement = null;
                            createMeasureTooltip();
                            ol.Observable.unByKey(listener);
                        }, this);
                }
            }
            createMeasureTooltip();

        }

        function getAdresi(poligonKoordinati) {
            var poligon = poligonKoordinati.toString();

            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Maps/General.aspx/GetListAdresi")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{poligon:"' + poligon + '"}',

                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        $('#selectUlicaN').empty();
                        $('#selectUlicaN').select2({ placeholder: '', allowClear: true, data: jsonResult });
                        $('#selectUlicaN').val("");
                        $('#selectUlicaN').trigger("change.select2");
                    }

                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                }
            });
        }

    </script>


    <script>
        $("label.collapsible").on("click", function () {
            var icon = $(this).next("span.myarrow");
            var ul = icon.next("ul");
            if (ul.hasClass("hidden")) {
                icon.removeClass("glyphicon-arrow-right");
                icon.addClass("glyphicon-arrow-down");
                ul.removeClass("hidden");
                $(this).parent("li").removeClass("collapsed");
                
            }
            else {
                icon.removeClass("glyphicon-arrow-down");
                icon.addClass("glyphicon-arrow-right");
                ul.addClass("hidden");
                $(this).parent("li").addClass("collapsed");
                
            }
        });
    </script>

</asp:Content>
