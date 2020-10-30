<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Mikro.aspx.cs" Inherits="Navigator.Modules.Preklop.Mikro" %>

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
    <script src='<%= ResolveUrl("~/Scripts/jquery.fileDownload.js")%>'></script>
    <div class="col-md-2">
        <fieldset class="form-horizontal">
            <div class="form-group">
                <br />
                <br />
                <h4><%: DbRes.T("PreklopNaPlaniranaSoMomentalna", "Resources") %></h4>
                <br />
                <br />
                <strong class="control-label"><%: DbRes.T("Proekt", "Resources") %></strong>
                <div>
                    <select id="selectProekt" class="js-example-placeholder-single form-control" style="width: 100%" runat="server"></select>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-9">
                    <a href="#" id="btnSubmitSearchDolgo" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span><%: DbRes.T("Prebaraj", "Resources") %></a>
                    <br />
                    <a href="#" id="btnPrevzemi" class="btn btn-link" title="Превземи Excel"><span class="glyphicon glyphicon-download"></span><%: DbRes.T("PrevzemiExcel", "Resources") %></a>
                </div>
            </div>
            <div id="progressDolgo" class="progress" hidden="true">
                <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                </div>
            </div>
        </fieldset>
    </div>
    <div class="col-md-10">
        <div id="map">
        </div>
    </div>
    <%--<p class="text-left col-md-4" id="mouse-position"></p>--%>
    <div id="data"></div>
    <%--<p class="bl" id="razmer"></p>--%>
    <div id="rezultatPreklop" class="col-md-12"></div>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/preklop.js") %>"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script>
        var globalData = "";
        $('#btnPrevzemi').hide();
        $(document).ready(function () {
            $('#<%= selectProekt.ClientID%>').select2({ placeholder: '<%: DbRes.T("IzbereteProekt", "Resources") %>', allowClear: true });
            $('#<%= selectProekt.ClientID%>').val("");
            $('#<%= selectProekt.ClientID%>').trigger("change.select2");
        });
        $('#<%= selectProekt.ClientID%>').on("change", function(e) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Preklop/Mikro.aspx/ChangeProject")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + $('#<%= selectProekt.ClientID%>').val() + '}',
                success: function (result) {
                    if (result.d.length > 0) {
                        $('#data').removeData();
                        var jsonResult = JSON.parse(result.d);
                        $('#data').data(jsonResult.Id.toString(), jsonResult.GeoJson);
                        goToGeojsonExtent();
                    }
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        });


        $('#btnSubmitSearchDolgo').on("click", function (e) {
            var projId = $('#<%= selectProekt.ClientID%>').val();
            globalData = "";
            $('#rezultatPreklop').empty();
            $('#btnPrevzemi').hide();
            if (projId !== null) {
                $('#progressDolgo').show();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/Preklop/Mikro.aspx/Search")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{projId:' + projId + '}',
                    success: function (result) {
                        $('#progressDolgo').hide();
                        $('#data').removeData();

                        $('#rezultatPreklop').append('<h3><%: DbRes.T("Rezultat", "Resources") %>:</h3>');
                        var jsonResult = JSON.parse(result.d);
                        var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("BrojPlanirana", "Resources") %></th><th><%: DbRes.T("BrojPostojna", "Resources") %></th><th><%: DbRes.T("Presek", "Resources") %></th><th><%: DbRes.T("Povrsina", "Resources") %></th><th><%: DbRes.T("PovrsinaZaGradenje", "Resources") %></th><th><%: DbRes.T("PovrsinaBruto", "Resources") %></th><th>KlasaNaNamena</th><th><%: DbRes.T("KompatibilnaKlasaNaNamena", "Resources") %></th><th><%: DbRes.T("MaksimalnaVisina", "Resources") %></th><th><%: DbRes.T("Katnost", "Resources") %></th><th><%: DbRes.T("ParkingMesta", "Resources") %></th><th><%: DbRes.T("ProcentNaIzgradenost", "Resources") %></th><th><%: DbRes.T("KoeficientNaIskoristenost", "Resources") %></th><th></th></tr>';
                        for (var key in jsonResult) {
                            if (jsonResult.hasOwnProperty(key)) {
                                var preklop = jsonResult[key];
                                var nova = preklop.Nova;
                                $('#data').data('' + nova.Id, nova.GeoJson);
                                tableString += '<tr class="bg-success"><td>' + nullToEmptyString(nova.Broj) + '</td><td></td><td></td><td>' + replaceCodes(nova.Povrshina) + '</td><td>' + replaceCodes(nova.PovrshinaGradenje) + '</td><td>' + replaceCodes(nova.BrutoPovrshina) + '</td><td>' + nullToEmptyString(nova.KlasaNamena) + '</td><td>' + nullToEmptyString(nova.KompKlasaNamena) + '</td><td>' + nullToEmptyString(nova.MaxVisina) + '</td><td>' + nullToEmptyString(nova.Katnost) + '</td><td>' + nullToEmptyString(nova.ParkingMesta) + '</td><td>' + nullToEmptyString(nova.ProcentIzgradenost) + '</td><td>' + nullToEmptyString(nova.KoeficientIskoristenost) + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent4(\'' + nova.Id.toString() + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';
                                var stari = preklop.Stari;
                                for (var key2 in stari) {
                                    var stara = stari[key2];
                                    tableString += '<tr class="bg-danger"><td></td><td>' + nullToEmptyString(stara.Broj) + '</td><td><strong>' + nullToEmptyString(stara.Presek) + '</strong></td><td>' + replaceCodes(stara.Povrshina) + '</td><td>' + replaceCodes(stara.PovrshinaGradenje) + '</td><td>' + replaceCodes(stara.BrutoPovrshina) + '</td><td>' + nullToEmptyString(stara.KlasaNamena) + '</td><td>' + nullToEmptyString(stara.KompKlasaNamena) + '</td><td>' + nullToEmptyString(stara.MaxVisina) + '</td><td>' + nullToEmptyString(stara.Katnost) + '</td><td>' + nullToEmptyString(stara.ParkingMesta) + '</td><td>' + nullToEmptyString(stara.ProcentIzgradenost) + '</td><td>' + nullToEmptyString(stara.KoeficientIskoristenost) + '</td><td></td></tr>';
                                }
                            }
                        }
                        tableString += '</table>';
                        $('#rezultatPreklop').append(tableString);
                    },
                    error: function (p1, p2, p3) {
                        $('#progressDolgo').hide();
                        alert(p1.status);
                        alert(p2);
                        alert(p3);
                    }
                });
            } else {
                alert('<%: DbRes.T("ZadolzitelniSeSitePolinja", "Resources") %>');
            }
        });

        function nullToEmptyString(obj) {
            if (obj) {
                return obj;
            }
            return "/";
        };

        function replaceCodes(obj) {
            if (obj) {
                if (obj == -1) return "Постојна состојба";
                if (obj == -2) return "со Архитектонско-урбанистички проект";
                if (obj == -3) return "според АУП";
                return obj;
            }
            return "/";
        };
    </script>
</asp:Content>
