<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PublicNavigator.Modules.Notifikacii.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/Settings.aspx")%>"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/ol3-layerswitcher.js")%>"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/layers.js") %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div class="col-md-12">
        <br />
        <h3>Листа на точки од интерес</h3>
        <div id="divList">
        </div>
    </div>
    <div class="col-md-12">
        <div id="map"></div>
    </div>
    <div id="data"></div>
    <div id="dataZoom"></div>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/notifikacii.js") %>"></script>
    <script>
        $(document).ready(function () {
            getList();
        });
        function getList() {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Notifikacii/Default.aspx/GetList")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                //data: '{coordinates:"' + evt.coordinate + '"}',
                success: function (result) {
                    $('#data').removeData();
                    $('#divList').empty();
                    var jsonResult = JSON.parse(result.d);
                    if (jQuery.isEmptyObject(jsonResult)) {
                        $('#divList').append('<h4>Нема податоци</h4>');
                    } else {
                        var tableString = '<table class="table table-hover"><tr><th>Тема од интерес</th><th>Под тема</th><th>Преглед</th><th>Избриши</th></tr>';
                        for (var key in jsonResult) {
                            if (jsonResult.hasOwnProperty(key)) {
                                $('#dataZoom').data('parceli' + jsonResult[key].Id, jsonResult[key].GeoJson);
                                tableString += '<tr><td>' + jsonResult[key].Tema + '</td><td>' + jsonResult[key].Podtema + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'parceli' + jsonResult[key].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td><td><a href="#" class="btn btn-link" onclick="deleteNot(\'' + jsonResult[key].Id + '\');return false;"><span class="glyphicon glyphicon-remove-sign" aria-hidden="true"></span></a></td></tr>';
                            }
                        }
                        tableString += '</table>';
                        $('#divList').append(tableString);
                    }
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p2);
                    alert(p3);
                }
            });
        }
        function deleteNot(id) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Notifikacii/Default.aspx/Izbrisi")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + id + '}',
                success: function (result) {
                    getList();
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }
    </script>

</asp:Content>
