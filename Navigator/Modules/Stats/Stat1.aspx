<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Stat1.aspx.cs" Inherits="Navigator.Modules.Stats.Stats1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/ol3-layerswitcher.css")%>" type="text/css">   
    <link rel="stylesheet" href="<%=ResolveClientUrl("~/Content/css/select2.css")%>" type="text/css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <div>
        <fieldset class="form-horizontal">
            
                <br />
                <br />
                <h4><%: DbRes.T("Statistika1", "Resources") %></h4>
                <br />
                <br />
            <div class="form-group">
                <strong class="control-label"><%: DbRes.T("Proekt", "Resources") %></strong>
                <div>
                    <select id="selectProekt" class="js-example-placeholder-single form-control" style="width: 50%" runat="server"></select>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-9">
                    <a href="#" id="btnSubmitSearchDolgo" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span><%: DbRes.T("Prebaraj", "Resources") %></a>
                    <br />                 
                </div>
            </div>
            <div id="progressDolgo" class="progress" hidden="true">
                <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                </div>
            </div>

                <div class="form-group">
                <strong class="control-label"><%: DbRes.T("Namena", "Resources") %></strong>
                <div>
                    <select id="selectNamena" class="js-example-placeholder-single form-control" style="width: 50% " ></select>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-9">
                    <a href="#" id="btnSubmitSearchDolgo2" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span><%: DbRes.T("Prebaraj", "Resources") %></a>
                    <br />
                </div>
            </div>
            <div id="progressDolgo2" class="progress" hidden="true">
                <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                </div>
            </div>

        </fieldset>
    </div>
    
    <%--Select za Proekt--%>
    <%--<p class="text-left col-md-4" id="mouse-position"></p>--%>
    <div id="data"> </div>
    <%--<p class="bl" id="razmer"></p>--%>
    <div id="rezultatPreklop" class="col-md-12"></div>
    
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script>      
        $(document).ready(function () {
            $('#<%= selectProekt.ClientID%>').select2({ placeholder: '<%: DbRes.T("IzbereteProekt", "Resources") %>', allowClear: true });
            $('#<%= selectProekt.ClientID%>').val("");
            $('#<%= selectProekt.ClientID%>').trigger("change.select2");
        });
        $('#<%= selectProekt.ClientID%>').on("change", function (e) {
            $("#selectNamena").html('').select2({ data: [{ id: '', text: '' }] }).empty();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Stats/Stat1.aspx/ChangeProject")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + $('#<%= selectProekt.ClientID%>').val() + '}',
                success: function (result) {
                    if (result.d.length > 0) { 
                        var jsonResult = JSON.parse(result.d);
                        $('#selectNamena').select2({ placeholder: '<%: DbRes.T("IzbereteNamena", "Resources") %>', allowClear: true, data: jsonResult });
                        $('#selectNamena').val("");
                        $('#selectNamena').trigger("change.select2");
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
            $('#rezultatPreklop').empty();
            if (projId !== null) {
                $('#progressDolgo').show();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/Stats/Stat1.aspx/Search")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{projId:' + projId + '}',
                    success: function (result) {
                        $('#progressDolgo').hide();
                        $('#data').removeData();
                        $('#rezultatPreklop').append('<h3><%: DbRes.T("Rezultat", "Resources") %>:</h3>');
                        var jsonResult = JSON.parse(result.d);
                       
                        var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("PovrsinaBruto", "Resources") %>(m<sup>2</sup>)</th><th><%: DbRes.T("Povrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("PovrsinaPresmetana", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("KoeficientNaIskoristenost", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("ZelenaPovrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("ZelenaPresmetanaPovrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("GradeznaPovrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("GradeznaPresmetanaPovrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("OdnosZeleniloSoGP", "Resources") %> (%)</th><th><%: DbRes.T("OdnosZeleniloSoGradeznaPresmetanaPovrsina", "Resources") %> (%)</th></tr>';

                        tableString += '<tr class="bg-success"><td>' + nullToEmptyString(jsonResult.BrutoPovrsina) + '</td><td>' + nullToEmptyString(jsonResult.PovrsinaGradezniParceli) + '</td><td>' + nullToEmptyString(jsonResult.PovrsinaPresmetana) + '</td><td>' + nullToEmptyString(jsonResult.KoeficientIskeristenost) + '</td><td>' + nullToEmptyString(jsonResult.ZelenaPovrsina) + '</td><td>' + nullToEmptyString(jsonResult.ZelenaPovrsinaPresmetana) + '</td><td>' + nullToEmptyString(jsonResult.GradeznaPovrsina) + '</td><td>' + nullToEmptyString(jsonResult.GradeznaPovrsinaPresmetana) + '</td><td>' + nullToEmptyString(jsonResult.Odnos) + '</td><td>' + nullToEmptyString(jsonResult.OdnosPresmetan) + '</td></tr>';

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
                alert('Задолжителни се сите полиња');
            }
        });
    
        $('#btnSubmitSearchDolgo2').on("click", function (e) {
            var projId = $('#<%= selectProekt.ClientID%>').val();
            var namId = $('#selectNamena').val();
             $('#rezultatPreklop').empty();
             if (projId !== null) {
                 $('#progressDolgo2').show();
                 $.ajax({
                     type: "POST",
                     url: "<%= Page.ResolveUrl("~/Modules/Stats/Stat1.aspx/SearchNamena")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{opfatId:' + projId + ',namena:\'' + namId + '\'}',
                    success: function (result) {
                        $('#progressDolgo2').hide();
                        $('#rezultatPreklop').append('<h3><%: DbRes.T("Rezultat", "Resources") %>:</h3>');
                        var jsonResult = JSON.parse(result.d);
                        var tableString = '<table class="table table-hover"><tr><th><%: DbRes.T("PovrsinaBruto", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("Povrsina", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("PovrsinaPresmetana", "Resources") %> (m<sup>2</sup>)</th><th><%: DbRes.T("KoeficientNaIskoristenost", "Resources") %> (m<sup>2</sup>)</th><th></th></tr>';
                        tableString += '<tr class="bg-success"><td>' + nullToEmptyString(jsonResult.BrutoPovrsina) + '</td><td>' + nullToEmptyString(jsonResult.PovrsinaGradezniParceli) + '</td><td>' + nullToEmptyString(jsonResult.PovrsinaPresmetana) + '</td><td>' + nullToEmptyString(jsonResult.KoeficientIskeristenost) + '</td></tr>';
                        tableString += '</table>';
                        $('#rezultatPreklop').append(tableString);
                    },
                    error: function (p1, p2, p3) {
                        $('#progressDolgo2').hide();
                        alert(p1.status);
                        alert(p2);
                        alert(p3);
                    }
                });
            } else {
                alert('Задолжителни се сите полиња');
            }
         });

        function nullToEmptyString(obj) {
            if (obj) {
                return obj;
            }
            return "/";
        };
    </script>
</asp:Content>
