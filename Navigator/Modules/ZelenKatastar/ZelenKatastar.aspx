<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ZelenKatastar.aspx.cs" Inherits="Navigator.Modules.ZelenKatastar.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
     <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/Settings.aspx")%>"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher.js")%>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/layers.js") %>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.css")%>" type="text/css">
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.min.css")%>" type="text/css" />
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/measure.css")%>" type="text/css">
   

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src="<%=ResolveClientUrl("~/Scripts/moment.js")%>"></script>
    <script src="<%=ResolveClientUrl("~/Scripts/bootstrap-datetimepicker.min.js")%>"></script>


     <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divInfo" class="table-responsive"></div>
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
            <a href="#" runat="server" id="btnPolygon" class="btn btn-sm btn-info" title="Пресметување на површина"><span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span></a>
            <a href="#" id="btnSearch" class="btn btn-sm btn-info" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>
            <a href="#" id="btnReport" class="btn btn-sm btn-info" title="Извештај"><span class="glyphicon glyphicon-list" aria-hidden="true"></span></a>

              <div id="divPopup" class="popup" hidden="true">
                  
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content" style="width: 1000px; right: 500px">
                        <div class="modal-body" >
                            <ul class="nav nav-tabs" id="tabovi">
                                <li class="active" runat="server" id="treeshrub"><a id="treeShrubS" href="#treeShrub" data-toggle="tab">Дрва/Грмушки</a></li>
                                <li class="" runat="server" id="streetsSearch"><a id="streetsS" href="#streets" data-toggle="tab">Полигони</a></li>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="treeShrub">
                                    <div class="input-group" style="width: 100%;">
                                        <div>
                                            <select id="dllDrvoGrmushka"  style="width: 100%" ></select>
                                        </div>
                                        <br />
                                        <div>
                                            <select id="dllSeason"  style="width: 100%" ></select>
                                        </div>  
                                        <br />
                                          <div>
                                            <select id="dllName"  style="width: 100%" ></select>
                                        </div>
                                      <div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchDrvaGrmushki" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel2" runat="server" ScrollBars="Vertical" Height="200px" >
                                        <div id="progressDrvaGrmushki" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultDrvaGrmushki" ></div>
                                    </asp:Panel>
                                </div>
                                 
                            </div>

                                 <div class="tab-pane fade " id="streets">
                                    <div class="input-group" style="width: 100%;">
                                        <div>
                                            <select id="dllStreets"  style="width: 100%" ></select>
                                        </div>
                                        <br />
                                       
                                      <div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchStreets" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical" Height="200px">
                                        <div id="progressStreets" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultStreets"></div>
                                    </asp:Panel>
                                </div>
                                 
                            </div>
                        </div>
                    </div>
                </div>
                       
            </div>



        </div>

            <div id="divPopupReport" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content" style="width: 600px; right: 100px">
                        <div class="modal-body" >
                            <ul class="nav nav-tabs" id="tabovi2">
                                <%--<li class="active" runat="server" id="TreeShrubReport"><a id="treeShrub2" href="#treeShrub" data-toggle="tab">Извештај дрва и грмушки</a></li>--%>
                                 <li class="active"><a id="TreeShrubReport" href="#treeShrubReport" data-toggle="tab">Извештај дрва и грмушки</a></li>
                              <%--   <li class=""><a id="sFlowerReport" href="#flowerReport" data-toggle="tab">Извештај цвеќиња</a></li>
                                <li class=""><a id="sGreenSurfaceReport" href="#GreenSurfaceReport" data-toggle="tab">Извештај зелени површини</a></li>  --%>
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="treeShrubReport">
                                    <div class="input-group" style="width: 100%;">
                                        <div>
                                            <select id="dllDrvoGrmushkaReport"  style="width: 100%" ></select>
                                        </div>
                                        <br />
                                      <div>
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchDrvaGrmushkiReport" class="btn btn-info" role="button" title="Извештај">Креирај извештај</a>
                                            <a href="#" id="btnSaveMap" class="btn btn-info" role="button" title="Извештај" style=" margin-left: 10px">Конвертирај во PDF</a>
                                            <br/>
                                             <br/>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="200px">
                                        <div id="progressDrvaGrmushkiReport" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultDrvaGrmushkiReport"></div>
                                    </asp:Panel>
                                </div>
                            </div>
                              <%--   <div class="tab-pane" id="flowerReport">
                                    <div class="input-group" style="width: 100%;">
                                        <div>
                                            <select id="dllDrvoGrmushkaReport3"  style="width: 100%" ></select>
                                        </div>
                                        <br />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchFlowerReport" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel3" runat="server" ScrollBars="Vertical" Height="200px">
                                        <div id="progressFlowerReport" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultFlowerReport"></div>
                                    </asp:Panel>
                                </div>--%>
                                  <%-- <div class="tab-pane" id="GreenSurfaceReport">
                                    <div class="input-group" style="width: 100%;">
                                        <div>
                                            <select id="dllDrvoGrmushkaReport4"  style="width: 100%" ></select>
                                        </div>
                                        <br />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchGreenSurfaceReport" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                       
                                    </div>
                                    <asp:Panel ID="Panel4" runat="server" ScrollBars="Vertical" Height="200px">
                                        <div id="progressGreenSurfaceReport" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultGreenSurfaceReport"></div>
                                    </asp:Panel>
                                </div>--%>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

                <div class="modal fade" id="myLoadingModal" tabindex="-1" role="dialog" aria-labelledby="loadingModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="loadingModalLabel">Ве молиме причекајте</h4>
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


             

        </div>
    </div>
           
    <asp:HiddenField ID="coordinate" runat="server" Value="" />
    <asp:HiddenField ID="coordinates" runat="server" Value="" />
    <asp:HiddenField ID="polygon" runat="server" Value="" />

    <p class="text-left col-md-4" id="mouse-position"></p>
    <div id="data"></div>
    <div id="dataSearch"></div>
    <div id="dataSearchK"></div>
    <div id="dataSearchA"></div>
    <div id="dataSearchDrvaGrmushki"></div>
    <div id="dataSearchKParceli"></div>
    <p class="bl" id="razmer"></p>
    <iframe id="my_iframe" style="display: none;"></iframe>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/zelenKatastar.js") %>"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>
    

     <script type="text/javascript">

       

         function showModal() {
             $('#infoModal').modal('show');
         };
         function closeShowModal() {
             $('#infoModal').modal('hide');
         };
         function showLoading() {
             $('#myLoadingModal').modal('show');
         };
         function hideLoading() {
             $('#myLoadingModal').modal('hide');
         };


         var infoTool = false;
         var rubberZoomTool = false;
         var polygonTool = false;
         var typeSelect = 'None';
         var searchTool = false;
         var reportTool = false;
         var draw; // global so we can remove it later
         var source = new ol.source.Vector({ wrapX: false });


         $('#btnInfo').click(function () {
             rubberZoomTool = false;
             searchTool = false;
             polygonTool = false;
             reportTool = false;
             $('#divPopup').hide();
             $('#divPopupReport').hide();
             if (infoTool) {
                 $('#btnInfo').removeClass("btn-danger");
                 $('#btnInfo').addClass("btn-info");
                 clearSourceDraw();
             } else {
                 $('#btnInfo').addClass("btn-danger");
                 $('#btnInfo').removeClass("btn-info");
                 $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                 $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                 $('#btnSearch').removeClass("btn-danger");
                 $('#btnSearch').addClass("btn-info");
                 $('#btnReport').addClass("btn-info");
                 $('#btnReport').removeClass("btn-danger");
                 //za rubber zoom
                 document.body.style.cursor = 'default';
                 typeSelect = "None";
                 map.removeInteraction(draw);
                 addInteraction();
             }
             infoTool = !infoTool;
         });


         $("#<%= btnPolygon.ClientID %>").click(function () {
             rubberZoomTool = false;
             searchTool = false;
             infoTool = false;
             reportTool = false;
             $('#divPopup').hide();
             $('#divPopupReport').hide();
             if (polygonTool) {
                 $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                 $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                 document.body.style.cursor = 'default';
                 typeSelect = 'None';
                 clearMeasurments();
                 map.removeInteraction(draw);
                 addInteraction();
             } else {
                 $('#btnInfo').removeClass("btn-danger");
                 $('#btnInfo').addClass("btn-info");
                 $("#<%= btnPolygon.ClientID %>").addClass("btn-danger");
                 $("#<%= btnPolygon.ClientID %>").removeClass("btn-info");
                 $('#btnSearch').removeClass("btn-danger");
                 $('#btnSearch').addClass("btn-info");
                 $('#btnReport').addClass("btn-info");
                 $('#btnReport').removeClass("btn-danger");
                 //za rubber zoom
                 document.body.style.cursor = 'default';
                 typeSelect = 'area';
                 clearMeasurments();
                 map.removeInteraction(draw);
                 addInteraction();
             }
             polygonTool = !polygonTool;
         });

         $('#btnSearch').click(function () {
             polygonTool = false;
             infoTool = false;
             reportTool = false;
             $('#divPopup').toggle();
             $('#divPopupReport').hide();
             if (searchTool) {
                 $('#btnSearch').removeClass("btn-danger");
                 $('#btnSearch').addClass("btn-info");
             } else {
                 $('#btnInfo').removeClass("btn-danger");
                 $('#btnInfo').addClass("btn-info");
                 $('#btnSearch').addClass("btn-danger");
                 $('#btnSearch').removeClass("btn-info");
                 $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                 $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                 $('#btnReport').addClass("btn-info");
                 $('#btnReport').removeClass("btn-danger");

                 //za rubber zoom
                 document.body.style.cursor = 'default';
                 typeSelect = "None";
                 map.removeInteraction(draw);
                 addInteraction();
             }
             searchTool = !searchTool;
         });

         $('#btnReport').click(function () {
             infoTool = false;
             vnesTockiTool = false;
             crtanjeTool = false;
             searchTool = false;
             $('#divPopupReport').toggle();
             $('#divPopup').hide();
             if (reportTool) {
                 $('#btnReport').removeClass("btn-danger");
                 $('#btnReport').addClass("btn-info");
                 clearSourceDraw();
             } else {
                 $('#btnReport').addClass("btn-danger");
                 $('#btnReport').removeClass("btn-info");
                 $('#btnInfo').removeClass("btn-danger");
                 $('#btnInfo').addClass("btn-info");
                 $("#<%= btnPolygon.ClientID %>").removeClass("btn-danger");
                 $("#<%= btnPolygon.ClientID %>").addClass("btn-info");
                 $('#btnSearch').removeClass("btn-danger");
                 $('#btnSearch').addClass("btn-info");
                 document.body.style.cursor = 'default';
                 typeSelect = "None";
                 map.removeInteraction(draw);
                 addInteraction(typeSelect);
                 clearSourceDraw();
          
             }
             reportTool = !reportTool;

         });

         <%--$(document).ready(function () {
             $('#<%= selectTreeSeason.ClientID%>').select2({ placeholder: 'Изебрете сезона', allowClear: true });
             $('#<%= selectTreeSeason.ClientID%>').val("");
             $('#<%= selectTreeSeason.ClientID%>').trigger("change.select2");

         });--%>
         <%-- $('#<%= selectTreeSeason.ClientID%>').on("change", function (e) {
             var treeseason = $('#<%= selectTreeSeason.ClientID%>').val();
             console.log(treeseason);
             if (treeseason == 'Зимзелено') {
                 var treeseasonid = 1;
                 console.log(treeseasonid);
             }
             else {
                 treeseasonid = 2;
                 console.log(treeseasonid);
             }
             //$("#selectName").html('').select2({ data: [{ seasontree: '', text: '' }] }).empty();
             $.ajax({
                 type: "POST",
                 url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/FillTreeName")%>",
                 async: true,
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 data: '{treeseasonid:"' + treeseasonid + '"}',
                 success: function (result) {
                     if (result.d.length > 0) {
                         var jsonResult = JSON.parse(result.d);
                         $('#selectName').select2({ placeholder: 'Izberi ime', allowClear: true, data: jsonResult });
                         $('#selectName').val("");
                         $('#selectName').trigger("change.select2");
                     }
                 },
                 error: function (p1, p2, p3) {
                     hideLoading();
                     alert(p1.status);
                     alert(p3);
                 }
             });

         });--%>

         map.on('singleclick', function (evt) {
             if(infoTool){
             $.ajax({
                 type: "POST",
                 url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/InfoZeleniloTool")%>",
                 async: true,
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 data: '{coordinates:"' + evt.coordinate + '"}',
                 success: function (result) {
                     $('#divInfo').empty();
                     var jsonResult = JSON.parse(result.d);

                     for (var key in jsonResult) {
                         if (jsonResult.hasOwnProperty(key)) {

                             if (key === "ListZelenilo") {
                                 var jsonResult1 = jsonResult[key];
                                 if (jQuery.isEmptyObject(jsonResult1)) {
                                     $('#divInfo').append('');
                                 } else {
                                     $('#divInfo').append('<h4>Зеленило полигони</h4>');
                                     var tableString = '<table class="table table-hover"><tr><th>Име</th><th>Површина (m<sup>2</sup>)</th><th>Катастарска парцела</th><th>Катастарска општина</th></tr>';
                                     //console.log(jsonResult );
                                     for (var item in jsonResult1) {
                                         tableString += ' <tr><td>' + jsonResult1[item].Name + '</td><td>' + nullToEmptyString(jsonResult1[item].Povrsina) + '</td><td>' + nullToEmptyString(jsonResult1[item].KP) + '</td><td>' + nullToEmptyString(jsonResult1[item].KO) + '</td></tr> ';
                                     }

                                     tableString += '</table>';
                                     $('#divInfo').append(tableString);
                                 }
                             }

                             if (key === "ListTreeShrub") {
                                 var jsonResult1 = jsonResult[key];

                                 if (jQuery.isEmptyObject(jsonResult1)) {
                                     $('#divInfo').append('');
                                 } else {
                                     $('#divInfo').append('<h4>Дрво/грмушка</h4>');
                                     var tableString = '<table class="table table-hover"><tr><th>Тип</th><th>Број</th><th>Име</th><th>Латинско име</th><th>Висина (m)</th><th>Ширина (m)</th><th>Години</th><th>Состојба</th><th>Полигон</th><th>Интервенција</th> </tr>';
                                     //console.log(jsonResult );
                                     for (var item in jsonResult1) {
                                         if (jsonResult1[item].Intervention == 1) {
                                             tableString += ' <tr><td>' + jsonResult1[item].TreeShrubTypeName + '</td><td>' + nullToEmptyString(jsonResult1[item].IdNumber) + '</td><td>' + nullToEmptyString(jsonResult1[item].TopologyName) + '</td><td>' + nullToEmptyString(jsonResult1[item].LatinTopologyName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Height) + '</td><td>' + nullToEmptyString(jsonResult1[item].CanopyWidth) + '</td><td>' + treeAge(jsonResult1[item].Age) + '</td><td>' + nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName) + '</td><td> Има  </td></tr> ';
                                         }
                                         else {
                                             tableString += ' <tr><td>' + jsonResult1[item].TreeShrubTypeName + '</td><td>' + nullToEmptyString(jsonResult1[item].IdNumber) + '</td><td>' + nullToEmptyString(jsonResult1[item].TopologyName) + '</td><td>' + nullToEmptyString(jsonResult1[item].LatinTopologyName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Height) + '</td><td>' + nullToEmptyString(jsonResult1[item].CanopyWidth) + '</td><td>' + treeAge(jsonResult1[item].Age) + '</td><td>' +  nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName)  + '</td><td>  Нема  </td></tr> ';
                                         }
                                        

                                     }

                                     tableString += '</table>';
                                     $('#divInfo').append(tableString);
                                 }
                             }
                             if (key === "ListCvekjinja") {
                                 var jsonResult1 = jsonResult[key];

                                 if (jQuery.isEmptyObject(jsonResult1)) {
                                     $('#divInfo').append('');
                                 } else {
                                     $('#divInfo').append('<h4>Цвеќе</h4>');
                                     var tableString = '<table class="table table-hover"><tr><th>Тип</th><th>Број</th><th>Име</th><th>Латинско име</th><th>Состојба</th><th>Забелешка</th><th>Полигон</th><th>Сезона</th><th>Интервенција</th> <th>Површина (m<sup>2</sup>)</th></tr>';
                                     //console.log(jsonResult );
                                     for (var item in jsonResult1) {

                                         if (jsonResult1[item].Intervention == 1) {
                                             tableString += ' <tr><td>' + jsonResult1[item].FlowerTypeName + '</td><td>' + nullToEmptyString(jsonResult1[item].IdNumber) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerName) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerLatinName) + '</td><td>' + nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Note) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerSeason) + '</td><td> Има </td><td>' + nullToEmptyString(jsonResult1[item].Surface) + '</td></tr> ';
                                         }
                                         else {
                                             tableString += ' <tr><td>' + jsonResult1[item].FlowerTypeName + '</td><td>' + nullToEmptyString(jsonResult1[item].IdNumber) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerName) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerLatinName) + '</td><td>' + nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Note) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName) + '</td><td>' + nullToEmptyString(jsonResult1[item].FlowerSeason) + '</td><td> Нема </td><td>' + nullToEmptyString(jsonResult1[item].Surface) + '</td></tr> ';
                                         }
                                     }

                                     tableString += '</table>';
                                     $('#divInfo').append(tableString);
                                 }
                             }
                             if (key === "ListZeleniPovrsini") {
                                 var jsonResult1 = jsonResult[key];

                                 if (jQuery.isEmptyObject(jsonResult1)) {
                                     $('#divInfo').append('');
                                 } else {
                                     $('#divInfo').append('<h4>Зелени површини</h4>');
                                     var tableString = '<table class="table table-hover"><tr><th>Име</th><th>Сезона</th><th>Состојба</th><th>Забелешка</th><th>Површина (m<sup>2</sup>)</th><th>Полигон</th><th>Патека</th><th>Интервенција</th> </tr>';
                                     //console.log(jsonResult );
                                     for (var item in jsonResult1) {

                                         if (jsonResult1[item].Intervention == 1) {
                                             tableString += ' <tr><td>Зелена површина</td><td>' + jsonResult1[item].SeasonName + '</td><td>' + nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Note) + '</td><td>' + nullToEmptyString(jsonResult1[item].Surface) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Paths) + '</td><td> Има </td></tr> ';
                                         }
                                         else {
                                             tableString += ' <tr><td>Зелена површина</td><td>' + jsonResult1[item].SeasonName + '</td><td>' + nullToEmptyString(jsonResult1[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Note) + '</td><td>' + nullToEmptyString(jsonResult1[item].Surface) + '</td><td>' + nullToEmptyString(jsonResult1[item].PolygonName) + '</td><td>' + nullToEmptyString(jsonResult1[item].Paths) + '</td><td> Нема </td></tr> ';
                                         }
                                     }

                                     tableString += '</table>';
                                     $('#divInfo').append(tableString);
                                 }
                             }
                         }
                     }
                     if ($('#divInfo').is(':empty')) {
                         $('#divInfo').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                     };
                     showModal();
                 },
                 error: function (p1, p2, p3) {
                     alert(p1.status);
                     alert(p2);
                     alert(p3);
                 }
             });

         }
         });


         function GetDrvoGrmushka() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetDrvoGrmushka")%>";
             $('#dllDrvoGrmushka').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             //type: $("#ddlstakeholder").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Type
                                 };
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Дрво/Грмушка',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,
             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }

         function GetDrvoGrmushkaReport() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetDrvoGrmushka")%>";
             $('#dllDrvoGrmushkaReport').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             //type: $("#ddlstakeholder").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Type
                                 };
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Дрво/Грмушка',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,
             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }



         function GetSeason() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetSeason")%>";
             $('#dllSeason').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             type: $("#dllDrvoGrmushka").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Name
                                 };
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Сезона',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,
             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }
         <%--function GetSeasonReport() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetSeason")%>";
             $('#dllSeasonReport').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             type: $("#dllDrvoGrmushkaReport").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Name
                                 };
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Сезона',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,
             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }--%>


         function GetTreeShrubName() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetTreeShrubName")%>";
             $('#dllName').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             type: $("#dllDrvoGrmushka").val(),
                             id: $("#dllSeason").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Name
                                 };
                        
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Име',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,

             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }


         function GetStreets() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetStreets")%>";
             $('#dllStreets').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             //type: $("#dllDrvoGrmushka").val(),
                             //id: $("#dllSeason").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Name
                                 };

                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Име',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,

             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }

         <%--   function GetTreeShrubNameReport() {
             var reqUrlDocuments = "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/GetTreeShrubName")%>";
             $('#dllNameReport').select2({
                 ajax: {
                     type: "POST",
                     url: reqUrlDocuments,
                     dataType: 'json',
                     contentType: "application/json; charset=utf-8",
                     delay: 250,
                     data: function (params) {
                         var term = "";
                         if (params.term) {
                             term = params.term;
                         }
                         return JSON.stringify({
                             type: $("#dllDrvoGrmushkaReport").val(),
                             id: $("#dllSeasonReport").val(),
                             search: term
                         });
                     },
                     processResults: function (data, params) {
                         var response = JSON.parse(data.d);
                         return {
                             results: $.map(response, function (item) {
                                 return {
                                     id: item.Id,
                                     text: item.Name

                                 };
                             })
                         };
                     },
                     cache: true
                 },

                 width: "100%",
                 allowClear: true,
                 placeholder: 'Име',
                 escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
                 templateResult: formatDocument,

             });
             function formatDocument(e) {
                 var markup = e.text;
                 return markup;
             };
         }--%>

        

       
       
         $('#btnSubmitSearchDrvaGrmushki').click(function () {

             if ($('#dllDrvoGrmushka option:selected').val() != null || $('#dllSeason option:selected').val() != null || $('#dllName option:selected').text() != '') {
                 var drvoGrmushka;
                 var season;
                 var name;

                 if (($('#dllDrvoGrmushka option:selected').val()) == null)
                     drvoGrmushka = 0;
                 else
                     drvoGrmushka = $('#dllDrvoGrmushka option:selected').val();
                 if (($('#dllSeason option:selected').val()) == null)
                     season = 0;
                 else
                     season = $('#dllSeason option:selected').val();

                 if (($('#dllName option:selected').text()) == "")
                     name = "";
                 else
                     name = $('#dllName option:selected').text();

                 $('#progressDrvaGrmushki').show();
                 $.ajax({
                     type: "POST",
                     url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/SearchOutA")%>",
                     async: true,
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     data: '{treeShrub:"' + drvoGrmushka + '", season:"' + season + '", name:"' + name + '"}',
                     success: function (result) {
                         $('#divSearchResultDrvaGrmushki').empty();
                         //$('#dataSearchDrvaGrmushki').removeData();
                         //   $('#dataSearch').removeData();
                         var jsonResult = JSON.parse(result.d);

                         var tableString = '<table class="table table-striped table-hover" ><tr><th>Тип</th><th>Сезона</th><th>Број</th><th>Име</th><th>Латинско име</th><th>Висина (m)</th><th>Ширина (m)</th><th>Години</th><th>Состојба</th><th>Полигон</th><th>Интервенција</th><th>Зум</th></tr>';
                         for (var item in jsonResult) {
                             $('#dataSearch').data(jsonResult[item].Id, JSON.parse(jsonResult[item].GeoJson).coordinates);
                             var x = JSON.parse(jsonResult[item].GeoJson).coordinates[0];
                             var y = JSON.parse(jsonResult[item].GeoJson).coordinates[1];
                             
                             if (jsonResult[item].Intervention == 1) {
                                 tableString += '<tr><td>' + jsonResult[item].TreeShrubTypeName + '</td><td>' + jsonResult[item].SeasonName + '</td><td>' + nullToEmptyString(jsonResult[item].IdNumber) + '</td><td>' + jsonResult[item].TopologyName + '</td><td>' + jsonResult[item].LatinTopologyName + '</td><td>' + nullToEmptyString(jsonResult[item].Height) + '</td><td>' + nullToEmptyString(jsonResult[item].CanopyWidth) + '</td><td>' + treeAge(jsonResult[item].Age) + '</td><td>' + nullToEmptyString(jsonResult[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult[item].PolygonName) + '</td><td>Има</td><td><a href="#" class="btn btn-link" ';
                                 tableString += 'onclick="goToGeojsonExtent2(\'' + jsonResult[item].Id + '\',\'' + x + ', ' + y + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                             }
                             else {
                                 tableString += '<tr><td>' + jsonResult[item].TreeShrubTypeName + '</td><td>' + jsonResult[item].SeasonName + '</td><td>' + nullToEmptyString(jsonResult[item].IdNumber) + '</td><td>' + jsonResult[item].TopologyName + '</td><td>' + jsonResult[item].LatinTopologyName + '</td><td>' + nullToEmptyString(jsonResult[item].Height) + '</td><td>' + nullToEmptyString(jsonResult[item].CanopyWidth) + '</td><td>' + treeAge(jsonResult[item].Age) + '</td><td>' + nullToEmptyString(jsonResult[item].ConditionName) + '</td><td>' + nullToEmptyString(jsonResult[item].PolygonName) + '</td><td>Нема</td><td><a href="#" class="btn btn-link" ';
                                 tableString += 'onclick="goToGeojsonExtent2(\'' + jsonResult[item].Id + '\',\'' + x + ', ' + y + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                             }

                         }
                         tableString += '</table>';
                         $('#divSearchResultDrvaGrmushki').append(tableString);


                         $('#progressDrvaGrmushki').hide();

                     },
                     error: function (p1, p2, p3) {
                         alert(p1.status);
                         alert(p3);
                         $('#progressDolgo').hide();
                     }
                 });
             }
             else {
                 alert('Изберете вредност');
             }
         });

         $('#btnSubmitSearchDrvaGrmushkiReport').click(function () {

             if ($('#dllDrvoGrmushkaReport').val() != null) {
                 $('#progressDrvaGrmushkiReport').show();
                 $.ajax({
                     type: "POST",
                     url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/ReportTreeShrub")%>",
                     async: true,
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     data: '{treeShrub:"' + $('#dllDrvoGrmushkaReport').val() + '"}',
                     success: function (result) {
                         $('#divSearchResultDrvaGrmushkiReport').empty();

                         var jsonResult = JSON.parse(result.d);

                         var tableString = '<table class="table table-striped table-hover" ><tr><th>Број</th><th>Зимзелени</th><th>Листопадни</th><th>Болни</th><th>Здрави</th></tr>';

                         tableString += '<tr><td>' +  nullToEmptyString(jsonResult.CountTreeShrub) + '</td><td>' + nullToEmptyString(jsonResult.CountZimzeleni) + '</td><td>' + nullToEmptyString(jsonResult.CountListopadni) + '</td><td>' + nullToEmptyString(jsonResult.CountZdravi) + '</td><td>' + nullToEmptyString(jsonResult.CountBolni) + '</td></tr>';
                         tableString += '</table>';
                         $('#divSearchResultDrvaGrmushkiReport').append(tableString);


                         $('#progressDrvaGrmushkiReport').hide();

                     },
                     error: function (p1, p2, p3) {
                         alert(p1.status);
                         alert(p3);
                         $('#progressDolgo').hide();
                     }
                 });
             }
             else {
                 alert('Изберете тип');
             }
         });


         
         $('#btnSubmitSearchStreets').click(function () {

             if ($('#dllStreets option:selected').val() != null) {
                 var name = $("#dllStreets option:selected").text();
                 var ulica;
                 if (($('#dllStreets option:selected').text()) == "")
                     ulica = "";
                 else
                     ulica = $('#dllStreets option:selected').text();

                 $('#progressStreets').show();
                 $.ajax({
                     type: "POST",
                     url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/SearchOutUlici")%>",
                     async: true,
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     data: '{searchString:"' + ulica + '"}',
                     success: function (result) {
                         $('#divSearchResultStreets').empty();

                         var jsonResult = JSON.parse(result.d);

                         var tableString = '<table class="table table-striped table-hover" ><tr><th>Име</th><th>Површина</th><th>Катастарска парцела</th><th>Катастарска општина</th><th>Зум</th></tr>';
                         for (var item in jsonResult) {

                             $('#dataSearch').data('opfat' + jsonResult[item].Id, jsonResult[item].GeoJsonUlica);
                             tableString += '<tr><td>' + jsonResult[item].Name + '</td><td>' + jsonResult[item].Povrsina + '</td><td>' + jsonResult[item].KP + '</td><td>' + jsonResult[item].KO + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtentPolygon(\'' + 'opfat' + jsonResult[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                         }
                         tableString += '</table>';
                         $('#divSearchResultStreets').append(tableString);


                         $('#progressStreets').hide();

                     },
                     error: function (p1, p2, p3) {
                         alert(p1.status);
                         alert(p3);
                         $('#progressStreets').hide();
                     }
                 });
             }
             else {
                 alert("Изберете улица");
             }
         });
      

         $("#btnSaveMap").click(function () {
             if ($('#dllDrvoGrmushkaReport').val() != null) {
             showLoading();
             $('#divExportMap').hide();
             var newRequestObj = new Object();

             newRequestObj.tip = $('#dllDrvoGrmushkaReport').val()

             var RequestDataString = JSON.stringify(newRequestObj);
           
                 $.ajax({
                     type: "POST",
                     url: "<%= Page.ResolveUrl("~/Modules/ZelenKatastar/ZelenKatastar.aspx/ExportReport")%>",
                     data: RequestDataString,
                     //async: true,
                     contentType: "application/json; charset=utf-8",
                     dataType: "json",
                     success: function (result) {
                         hideLoading();
                         if (result.d != "") {
                             //console.log(newRequestObj.formatType);
                             var win = window.open('../GetFile.ashx?img=' + result.d + '&folder=ExportMap', '_blank');
                             //if (newRequestObj.formatPaperType == 1) {
                             //    var win = window.open('../ExportMap/' + result.d, '_blank');
                             //} else {
                             //    var win = window.open('../GetFile.ashx?img=' + result.d + '&folder=ExportMap', '_blank');
                             //}
                         }
                     },
                     error: function (p1, p2, p3) {
                         hideLoading();
                     }
                 });
             }
             else {
                 alert('Изберете тип');
             }
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

         $(function () {
             GetDrvoGrmushka();
             GetTreeShrubName();
             GetSeason();
             GetDrvoGrmushkaReport();
             GetStreets();
             //GetFlowerResult()
             //GetSeasonReport();
             //GetTreeShrubNameReport();
            
         });

         function nullToEmptyString(obj) {
             if (obj) {
                 return obj;
             }
             return "/";
         };


          function treeAge(obj) {
            if (obj) {
                if (obj == 1) return "0-5";
                if (obj == 2) return "5-10";
                if (obj == 3) return "10-20";
                if (obj == 4) return "20-30";
                if (obj == 5) return "30-50";
                if (obj == 6) return "над 50";
                return obj;
            }
            return "/";

        }
          
         </script>

</asp:Content>
