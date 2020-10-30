<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PublicNavigator._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <%--<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/openlayers/4.0.1/ol-debug.js"></script>--%>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/openlayers/4.0.1/ol-debug.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/Settings.aspx")%>"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher.js")%>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/layers.js") %>"></script>
    <link href="Content/measure.css" rel="stylesheet" />
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.css")%>" type="text/css">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel">Резултат</h4>
                </div>
                <div class="modal-body">
                    <div id="divInfo"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Затвори</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="myLoadingModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="myModalLabel">Ве молиме причекајте</h4>
                </div>
                <div class="modal-body">
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Затвори</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="vnesModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnClose"><span aria-hidden="true">&times;</span></button>
                    <h4>Внесете ги точките од интерес</h4>
                </div>
                <div class="modal-body">
                    <div id="divVnes" class="tocki-interes">
                        <br />
                        <strong class="control-label">Тема од интерес</strong>
                        <div>
                            <select required="true" id="selectTema" class="js-example-placeholder-single form-control" style="width: 100%"></select>
                        </div>
                        <br />
                        <strong class="control-label">Подтема од интерес</strong>
                        <div>
                            <select required="true" id="selectPodTema" class="js-example-placeholder-single form-control" style="width: 100%"></select>
                        </div>
                        <br />
                    </div>
                    <div>
                        Напомена:Еден корисник може да избере најмногу 3 точки од интерес.
                    </div>
                    <div>
                        <asp:HyperLink runat="server" NavigateUrl="Modules/Notifikacii" class="btn btn-link" Text="Приказ на досегашните точки од интерес" />
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal" id="btnCloseModel">Затвори</button>
                    <button type="button" class="btn btn-success" data-dismiss="modal" id="btnSaveModel">Зачувај</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="forbiddenModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnForbiddeClose"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h4>За користење на оваа опција потребно е да се најавите!</h4>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal" id="btnCloseForbidden">Затвори</button>
                </div>
            </div>
        </div>
    </div>
    <div id="mapG">
        <div class="edit_map_controlls">
            <a href="#" id="btnInfo" class="btn btn-sm btn-danger" title="Инфо"><span class="glyphicon glyphicon-info-sign" aria-hidden="true"></span></a>
            <a href="#" runat="server" id="btnInfoBiznis" class="btn btn-sm btn-danger" title="Бизнис Инфо"><span class="glyphicon glyphicon-eye-open" aria-hidden="true"></span></a>
            <a href="#" id="btnSearch" class="btn btn-sm btn-danger" title="Пребарај"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>
            <a href="#" runat="server" id="btnDownload" class="btn btn-sm btn-danger" title="Превземи DXF"><span class="glyphicon glyphicon-sort-by-attributes-alt" aria-hidden="true"></span></a>          
            <a href="#" runat="server" id="btnTocki" class="btn btn-sm btn-danger" title="Точки од интерес"><span class="glyphicon glyphicon-pushpin" aria-hidden="true"></span></a>
            <a href="#" id="btnRubberZoom" class="btn btn-sm btn-danger" title="Зумирај дел"><span class="glyphicon glyphicon-screenshot" aria-hidden="true"></span></a>
            <a href="#" id="btnExtent" class="btn btn-sm btn-danger" title="Последен размер" onclick="goToLastExtent()"><span class="glyphicon glyphicon-sort" aria-hidden="true"></span></a>
            <a href="#" runat="server" id="btnPolygon" class="btn btn-sm btn-danger" title="Пресметување на површина"><span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span></a>
            <a href="#" runat="server" id="btnLine" class="btn btn-sm btn-danger" title="Пресметување на должина"><span class="glyphicon glyphicon-resize-horizontal" aria-hidden="true"></span></a>
             <div id="divPopup" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <ul class="nav nav-tabs" id="tabovi">
                                <li class="active"><a id="aBrzoP" href="#brzoP" data-toggle="tab">Сите слоеви</a></li>
                                <li class="" runat="server" id="kp" ><a id="aDolgoP" href="#dolgoP" data-toggle="tab">Катастарски парцели</a></li>
                                <li class="" style="visibility:hidden" runat="server" id="adresi" ><a id="aAdresiP" href="#adresiP" data-toggle="tab">Адреси</a></li>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="brzoP">
                                    <div class="input-group">
                                        <input id="txtSearch" type="text" class="form-control" />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearch" class="btn btn-link" title="Пребарај"><span class="glyphicon glyphicon-search"></span></a>
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
                                            <a href="#" id="btnSubmitSearchDolgo" class="btn btn-link" title="Пребарај"><span class="glyphicon glyphicon-search"></span></a>
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
                                <div class="tab-pane fade" id="adresiP" >
                                    <div class="input-group">   
                                        <div>
                                             <select  required="true" id="selectUlica" class="js-example-placeholder-single form-control" style="width: 100%" runat="server"></select>
                                        </div>
                                        <br />               
                                        <div>
                                            <select required="true" id="selectBroj" class="js-example-placeholder-single form-control" style="width: 100% " ></select>
                                        </div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchAdresi" class="btn btn-link" title="Пребарај"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Height="220px">
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
        </div>
    </div>
    <p class="text-left col-md-4" id="mouse-position"></p>
    <div id="data"></div>
    <div id="dataSearch"></div>
    <div id="dataSearchK"></div>
    <div id="dataSearchA"></div>
    <asp:HiddenField ID="coordinate" runat="server" Value="" />
    <asp:HiddenField ID="userCheck" runat="server" Value="" />
    <asp:HiddenField ID="selectedNum" runat="server" Value="" />
    <p class="bl" id="razmer"></p>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/general.js") %>"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script src="Scripts/select2.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>
    <script type="text/javascript">      
        $(document).ready(function () {
            $('#<%= selectUlica.ClientID%>').select2({ placeholder: 'Изберете улица', allowClear: true });
            $('#<%= selectUlica.ClientID%>').val("");
            $('#<%= selectUlica.ClientID%>').trigger("change.select2");
        });
        $('#<%= selectUlica.ClientID%>').on("change", function (e) {
            $("#selectBroj").html('').select2({ data: [{ ulica: '', text: '' }] }).empty();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Default.aspx/FillNumbers")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{ulica:"' + $('#<%= selectUlica.ClientID%>').val() + '"}',
                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        $('#selectBroj').select2({ placeholder: 'Изберете број', allowClear: true, data: jsonResult });
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
        function showLoading() {
            $('#myLoadingModal').modal('show');
        };
        function hideLoading() {
            $('#myLoadingModal').modal('hide');
        };
        function showVnesModal() {
            $('#vnesModal').modal('show');
        };
        function showForbiddenModal() {
            $('#forbiddenModal').modal('show');
        };
       <%-- function checkAuth() {
           
            $.ajax({
                     type: "POST",
                     url:  "<%= Page.ResolveUrl("~/Default.aspx/CheckUser")%>",
                     async: true,
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     data: '{}',
                     success: function (result) {
                         if (result.d == 'False')
                         {
                             return "ne";
                         }
                         else if (result.d == 'True'){
                             return "da";
                         }
                         
                }
            });
        };--%>

        var infoTool = false;
        var infoBiznisTool = false;
        var dxfTool = false;
        var searchTool = false;
        var tockiTool = false;
        var rubberZoomTool = false;
        var polygonTool = false;
        var lineTool = false;
        var typeSelect = 'None';
        var draw; // global so we can remove it later
        var source = new ol.source.Vector({ wrapX: false });

        $('#btnInfo').click(function () {
            dxfTool = false;
            searchTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            polygonTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (infoTool) {
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
            } else {
                $('#btnInfo').addClass("btn-default");
                $('#btnInfo').removeClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            infoTool = !infoTool;
        });
        $("#<%= btnInfoBiznis.ClientID %>").click(function () {
            dxfTool = false;
            searchTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoTool = false;
            polygonTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (infoBiznisTool) {
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
            } else {
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            infoBiznisTool = !infoBiznisTool;
        });
        $("#<%= btnDownload.ClientID %>").click(function () {
            infoTool = false;
            searchTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            polygonTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (dxfTool) {
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
            } else {
                $("#<%= btnDownload.ClientID %>").addClass("btn-default");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            dxfTool = !dxfTool;
        });
        $('#btnSearch').click(function () {
            infoTool = false;
            dxfTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            $('#divPopup').toggle();
            if (searchTool) {
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
            } else {
                $('#btnSearch').addClass("btn-default");
                $('#btnSearch').removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            searchTool = !searchTool;
        });
        $("#<%= btnTocki.ClientID %>").click(function () {
            infoTool = false;
            dxfTool = false;
            searchTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            polygonTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (tockiTool) {
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
            } else {
                $("#<%= btnTocki.ClientID %>").addClass("btn-default");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = "None";
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
                alert("Кликнете на мапата над тоа место од кое сакате да добивате инфомации за дополнителни измени или анкети");
            }
            tockiTool = !tockiTool;
        });
        $('#btnRubberZoom').click(function () {
            infoTool = false;
            dxfTool = false;
            searchTool = false;
            tockiTool = false;
            infoBiznisTool = false;
            polygonTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (rubberZoomTool) {
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $('#btnRubberZoom').addClass("btn-default");
                $('#btnRubberZoom').removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'Draw';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            rubberZoomTool = !rubberZoomTool;
        });
        $("#<%= btnPolygon.ClientID %>").click(function () {
            infoTool = false;
            searchTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            dxfTool = false;
            lineTool = false;
            $('#divPopup').hide();
            if (polygonTool) {
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $("#<%= btnPolygon.ClientID %>").addClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
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
            searchTool = false;
            tockiTool = false;
            rubberZoomTool = false;
            infoBiznisTool = false;
            dxfTool = false;
            polygonTool = false;
            $('#divPopup').hide();
            if (lineTool) {
                $("#<%= btnLine.ClientID %>").removeClass("btn-default");
                $("#<%= btnLine.ClientID %>").addClass("btn-danger");
                document.body.style.cursor = 'default';
                typeSelect = 'None';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            } else {
                $("#<%= btnLine.ClientID %>").addClass("btn-default");
                $("#<%= btnLine.ClientID %>").removeClass("btn-danger");
                $('#btnInfo').removeClass("btn-default");
                $('#btnInfo').addClass("btn-danger");
                $('#btnSearch').removeClass("btn-default");
                $('#btnSearch').addClass("btn-danger");
                $("#<%= btnInfoBiznis.ClientID %>").removeClass("btn-default");
                $("#<%= btnInfoBiznis.ClientID %>").addClass("btn-danger");
                $("#<%= btnTocki.ClientID %>").removeClass("btn-default");
                $("#<%= btnTocki.ClientID %>").addClass("btn-danger");
                $("#<%= btnDownload.ClientID %>").removeClass("btn-default");
                $("#<%= btnDownload.ClientID %>").addClass("btn-danger");
                $('#btnRubberZoom').removeClass("btn-default");
                $('#btnRubberZoom').addClass("btn-danger");
                $("#<%= btnPolygon.ClientID %>").removeClass("btn-default");
                $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                //za rubber zoom
                document.body.style.cursor = 'default';
                typeSelect = 'length';
                clearMeasurments();
                map.removeInteraction(draw);
                addInteraction();
            }
            lineTool = !lineTool;
        });
        map.on('singleclick', function (evt) {
            if (infoTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Default.aspx/InfoTool")%>",
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
                                        $('#divInfo').append('<h4>Опфат</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Име на план</th><th>Плански период</th><th>Технички број</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td>' + jsonResult1[item].PlanskiPeriod + '</td><td>' + jsonResult1[item].TehnickiBroj + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                        tableString = '<table class="table table-striped table-hover"><tr><th>Број на одлука</th><th>Законска регулатива</th><th>Изработува</th><th>Површина</th></tr>';
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
                                        $('#divInfo').append('<h4>Блок</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Име</th><th>Намена</th><th>Површина</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + jsonResult1[item].Ime + '</td><td>' + jsonResult1[item].Namena + '</td><td>' + jsonResult1[item].Povrshina + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                    }
                                }

                                if (key === "ListGradParceli") {
                                    var jsonResult1 = jsonResult[key];
                                    $('#data').removeData();
                                    if (jsonResult1 !== null && jsonResult1.length > 0) {
                                        $('#divInfo').append('<h4>Градежни парцели</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Број</th><th>Класа на намена</th><th>Компатибилна класа на намена</th><th>Површина</th><th>Површина за градење</th><th>Бруто површина</th></tr>';
                                        for (var item in jsonResult1) {
                                            //if (jsonResult1[item].OpstiUsloviId != null && jsonResult1[item].PosebniUsloviId !=null)
                                            //{
                                            //    $('#divInfo').append('<a href="#" class="btn btn-link" onclick="opstiUslovi(\'' + jsonResult1[item].OpstiUsloviId + '\');return false;">Превземи општи услови</a>');
                                            //    $('#divInfo').append('<a href="#" class="btn btn-link" onclick="posebniUslovi(\'' + jsonResult1[item].PosebniUsloviId + '\');return false;">Превземи посебни услови</a>');
                                            //}
                                            $('#data').data('' + jsonResult1[item].Id, jsonResult1[item].GeoJson);
                                            tableString += '<tr><td>' + nullToEmptyString(jsonResult1[item].Broj) + '</td><td>' + nullToEmptyString(jsonResult1[item].KlasaNamena) + '</td><td>' + nullToEmptyString(jsonResult1[item].KompKlasaNamena) + '</td><td>' + replaceCodes(jsonResult1[item].Povrshina) + '</td><td>' + replaceCodes(jsonResult1[item].PovrshinaGradenje) + '</td><td>' + replaceCodes(jsonResult1[item].BrutoPovrshina) + '</td></tr>';
                                        }
                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);
                                        tableString = '<table class="table table-striped table-hover"><tr><th>Максимална висина</th><th>Катност</th><th>Паркинг места</th><th>Процент на изграденост</th><th>Коефициент на искористеност</th></tr>';
                                        for (var item in jsonResult1) {
                                            tableString += '<tr><td>' + nullToEmptyString(jsonResult1[item].MaxVisina) + '</td><td>' + nullToEmptyString(jsonResult1[item].Katnost) + '</td><td>' + nullToEmptyString(jsonResult1[item].ParkingMesta) + '</td><td>' + nullToEmptyString(jsonResult1[item].ProcentIzgradenost) + '</td><td>' + nullToEmptyString(jsonResult1[item].KoeficientIskoristenost) + '</td></tr>';
                                        }

                                        tableString += '</table>';
                                        $('#divInfo').append(tableString);

                                        for (var item in jsonResult1) {
                                            if (jsonResult1[item].OpstiUsloviId != null && jsonResult1[item].PosebniUsloviId != null) {
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="opstiUslovi(\'' + jsonResult1[item].OpstiUsloviId + '\');return false;">Превземи општи услови</a>');
                                                $('#divInfo').append('<a href="#" class="btn btn-link" onclick="posebniUslovi(\'' + jsonResult1[item].PosebniUsloviId + '\');return false;">Превземи посебни услови</a>');
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
            } if (infoBiznisTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Default.aspx/InfoBiznisTool")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{coordinates:"' + evt.coordinate + '"}',
                    success: function (result) {
                        $('#divInfo').empty();

                        var jsonResult = JSON.parse(result.d);
                        if (jQuery.isEmptyObject(jsonResult)) {
                            $('#divInfo').append('<h4>Нема податоци</h4>');
                        } else {
                            var tableString = '<table class="table table-striped table-hover"><tr><th>Име</th><th>Контакт лице</th><th>Телефон</th><th>Е-пошта</th><th>Адреса</th><th>Веб сајт</th></tr>';
                            for (var key in jsonResult) {
                                if (jsonResult.hasOwnProperty(key)) {
                                    tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].Ime) + '</td><td>' + nullToEmptyString(jsonResult[key].KontaktLice) + '</td><td>' + nullToEmptyString(jsonResult[key].Telefon) + '</td><td>' + replaceCodes(jsonResult[key].Email) + '</td><td>' + replaceCodes(jsonResult[key].Adresa) + '</td><td><a href="' + jsonResult[key].Web + '" class="btn btn-link" target="_blank">   ' + replaceCodes(jsonResult[key].Web) + '</a></td></tr>';
                                }

                            }
                            tableString += '</table>';
                            $('#divInfo').append(tableString);
                        }

                        showModal();
                    },
                    error: function (p1, p2, p3) {
                        alert(p1.status);
                        alert(p3);
                    }
                });
            } else if (dxfTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Default.aspx/CheckUser")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{}',
                    success: function (result) {
                        if (result.d == 'True') {
                            showLoading();
                            $.ajax({
                                type: "POST",
                                url: "<%= Page.ResolveUrl("~/Default.aspx/GenerateDxfFile")%>",
                                async: true,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: '{coordinates:"' + evt.coordinate + '"}',
                                success: function (result) {
                                    hideLoading();
                                    if (result.d == 'ne') {
                                        alert("Неможете повеќе да симнувате dxf");
                                    }
                                    else {
                                        
                                        $.fileDownload('Dxf/' + result.d);
                                    }
                                },
                                error: function (p1, p2, p3) {
                                    hideLoading();
                                    alert(p1.status);
                                    alert(p3);
                                }
                            });
                        } else if (result.d == "False") {
                            showForbiddenModal();
                        }
                    }
                });
            } else if (tockiTool) {
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Default.aspx/CheckUser")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{}',
                    success: function (result) {

                        if (result.d == 'True') {
                            $("#<%= coordinate.ClientID %>").val(evt.coordinate);
                            $("#selectTema").html('').select2({ data: [{ id: '', text: '' }] }).empty();
                            $.ajax({
                                type: "POST",
                                url: "<%= Page.ResolveUrl("~/Default.aspx/FillThemes")%>",
                                async: true,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                data: {},
                                success: function (result) {
                                    if (result.d.length > 0) {
                                        var jsonResult = JSON.parse(result.d);
                                        $('#selectTema').select2({ placeholder: 'Изберете тема', allowClear: true, data: jsonResult });
                                        $('#selectTema').val("");
                                        $('#selectTema').trigger("change.select2");
                                    }
                                },
                                error: function (p1, p2, p3) {
                                    hideLoading();
                                    alert(p3);
                                }
                            });
                            showVnesModal();
                            $('#selectTema').on("change", function (e) {
                                $("#selectPodTema").html('').select2({ data: [{ id: '', text: '' }] }).empty();
                                $.ajax({
                                    type: "POST",
                                    url: "<%= Page.ResolveUrl("~/Default.aspx/FillSubThemes")%>",
                                    async: true,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    data: '{id:' + $('#selectTema').val() + '}',
                                    success: function (result) {
                                        if (result.d.length > 0) {
                                            var jsonResult = JSON.parse(result.d);
                                            $('#selectPodTema').select2({ placeholder: 'Изберете под тема', allowClear: true, data: jsonResult });
                                            $('#selectPodTema').val("");
                                            $('#selectPodTema').trigger("change.select2");
                                        }
                                    },
                                    error: function (p1, p2, p3) {
                                        hideLoading();
                                        alert(p1.status);
                                        alert(p3);
                                    }
                                });
                            });

                        } else if (result.d == "False") {
                            showForbiddenModal();
                        }
                    }
                });
            }
        });

$('#btnSaveModel').click(function () {
    var temaId = $('#selectTema').val();
    var podtemaId = $('#selectPodTema').val();
    var coordinates = $("#<%= coordinate.ClientID %>").val();
    var koordinata = JSON.stringify(coordinates);
    $('#progressDolgo').show();
    $.ajax({
        type: "POST",
        url: "<%= Page.ResolveUrl("~/Default.aspx/Save")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: "{'tema':'" + temaId + "','podtema':'" + podtemaId + "','koordinati':'" + coordinates + "'}",
                success: function (result) {
                    if (result.d)
                        alert("Успешно внесовте точка од интерес");
                    else
                        alert("Неуспешен внес на точка од интерес. Максимален број на точки од интерес е 3.");
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p3);
                    $('#progressDolgo').hide();
                }
            });
});


        $('#btnSubmitSearch').click(function () {
            if ($('#txtSearch').val().length != 0) {
                $('#progress').show();
                var myString = $('#txtSearch').val();
                var searchTxt = myString.replace(/"/gi, "quotes");
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Default.aspx/SearchOut")%>",
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
                                        $('#divSearchResult').append('<h4>Опфати</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Име</th><th>Зум</th></tr>';
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
                                        $('#divSearchResult').append('<h4>Блокови</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Име</th><th>Зум</th></tr>';
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
                                        $('#divSearchResult').append('<h4>Градежни парцели</h4>');
                                        var tableString = '<table class="table table-striped table-hover"><tr><th>Број</th><th>Зум</th></tr>';
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
                            $('#divSearchResult').append('<h4>Нема податоци</h4>');
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
                    url: "<%= Page.ResolveUrl("~/Default.aspx/SearchOutK")%>",
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
                                        $('#divSearchResultDolgo').append('<h4>Катастарски парцели</h4>');
                                        var tableString2 = '<table class="table table-striped table-hover"><tr><th>Број</th><th>Локација</th><th>Зум</th></tr>';

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
                            $('#divSearchResultDolgo').append('<h4>Нема податоци</h4>');
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
                url: "<%= Page.ResolveUrl("~/Default.aspx/SearchOutA")%>",
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

        function downloadDoc(path) {
            // Get file name from url.
            if (path == null || path == '') {
                //console.log('NEMA');
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
                url: "<%= Page.ResolveUrl("~/Default.aspx/GenerateOpstiUslovi")%>",
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
                url: "<%= Page.ResolveUrl("~/Default.aspx/GeneratePosebniUslovi")%>",
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
        $('#selectBroj').on("change", function (e) {
            document.getElementById('<%= selectedNum.ClientID %>').value = "";
            var selectNum = $('#selectBroj').val();

            $("#<%= selectedNum.ClientID %>").val(selectNum);
        });
    </script>
</asp:Content>
