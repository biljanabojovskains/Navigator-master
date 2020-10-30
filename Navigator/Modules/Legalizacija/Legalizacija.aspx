﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Legalizacija.aspx.cs" Inherits="Navigator.Modules.Legalizacija.Legalizacija" %>
<%--test--%>
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
    <script src="../../Scripts/moment.js"></script>
    <script src="../../Scripts/bootstrap-datetimepicker.min.js"></script>

       
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


    <div class="modal fade" id="legaliziraniModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close"  id="btnCloseReport" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divLegalizirani"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" id="btnCloseReportModal" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="vnesModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" >
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <b>Внесете барање</b>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnClose"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divVnes">
                        <table class="table table-striped table-hover">
                            <tr>
                                <th>Катастарска општина *</th>
                                <th>
                                <select  required="true" id="selectOpstina" class="js-example-placeholder-single form-control" style="width: 50%" runat="server" data-minimum-results-for-search="Infinity"></select>
                                </th>
                            </tr>
                            <tr>
                                <th>Катастарска парцела *</th>
                                <th>
                                    <input runat="server" type="text" id="katParcela" value="" class="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupBaranje' runat="server" ControlToValidate="katParcela" CssClass="text-danger" ErrorMessage="Задолжително внесете катастарска парцела" />
                                </th>
                            </tr>
                            <tr>
                                <th>Број на предмет *</th>
                                <th>
                                    <input runat="server" type="text" id="brPredmet" value="" class="form-control" />
                                    <asp:RequiredFieldValidator ValidationGroup='valGroupBaranje' runat="server" ControlToValidate="brPredmet" CssClass="text-danger" ErrorMessage="Задолжително внесете број" />
                                </th>
                            </tr>
                            <tr>
                                <th>Намена на објект *</th>
                                <th>
                                     <select  required="true" id="namenaNaObjekt" class="js-example-placeholder-single form-control" style="width: 50%" runat="server" data-minimum-results-for-search="Infinity"></select>
                              
                                </th>
                            </tr>
                             <tr>
                                <th>Број  на објект</th>
                                <th>
                                    <input runat="server" type="number" id="brojObjekt" name="brojObjekt" min="0" value="" class="form-control" />
                                  
                                </th>
                            </tr>
                 

                               <tr>
                                <th>Тип легализација</th>
                                <th>
                                    <input runat="server" type="text" id="tipLegalizacija" value="" class="form-control" />
                                    
                                </th>
                            </tr>


                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnCloseModel"><%: DbRes.T("Zatvori", "Resources") %></button>        
                    <input type="button" id="btnZacuvaj" class="btn btn-info" validationgroup='valGroupBaranje' title="Зачувај" value="Зачувај" />

                </div>
            </div>
        </div>

    </div>



    <div class="modal fade" id="successModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="succModalLabel"></h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    Успешно внесено барање!
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal"><%: DbRes.T("Zatvori", "Resources") %></button>

                </div>
            </div>
        </div>
    </div>


    <div class="modal fade" id="vnesTockiModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <b>Внесете координати</b>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close" id="btnPointsClose"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divVnesTocki">
                        <table id="tableVnesTocki" class="table table-striped table-hover">
                            <tr>
                                <td><b>X точка</b></td>
                                <td><b>Y точка</b></td>
                            </tr>

                            <tr class="rowTocka">
                                <td>
                                    <input type="text" id="tocka_x1" value="" class="form-control" />
                                </td>
                                <td>
                                    <input type="text" id="tocka_y1" value="" class="form-control" />
                                </td>

                            </tr>
                            <tr class="rowTocka">
                                <td>
                                    <input type="text" id="tocka_x2" value="" class="form-control" />
                                </td>
                                <td>
                                    <input type="text" id="tocka_y2" value="" class="form-control" />
                                </td>

                            </tr>
                            <tr class="rowTocka">
                                <td>
                                    <input type="text" id="tocka_x3" value="" class="form-control" />
                                </td>
                                <td>
                                    <input type="text" id="tocka_y3" value="" class="form-control" />
                                </td>

                            </tr>
                            <tr class="rowTocka">
                                <td>
                                    <input type="text" id="tocka_x4" value="" class="form-control" />
                                </td>
                                <td>
                                    <input type="text" id="tocka_y4" value="" class="form-control" />
                                </td>

                            </tr>
                        </table>
                        <input type="button" id="dodadiTocka" class="btn btn-info" value="Додади" />
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal" id="btnPointsCloseModel"><%: DbRes.T("Zatvori", "Resources") %></button>
                    <button type="button" class="btn btn-info" id="btnZacuvajTocki">Зачувај точки</button>
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
                    <div id="divDoc" >
                        <div class="row">
                            <div class="col-lg-6">
                                  <%: DbRes.T("PrikaciDokument", "Resources") %><br/>
                                  <input id="document_info" name="document" type="file" multiple  />
                            </div>
                            <div class="col-lg-6">
                                <br />
                                  <button type="button" class="btn btn-info" id="btnZacuvajDoc" >Прикачи документ</button>
                            </div>
                        </div>
                        <div id="divListDokumenti"></div>
                
                     <asp:HiddenField ID="idLegalizacija" runat="server" Value="" />
                    </div>
                    </div>
                    
                <div class="modal-footer">
                </div>
            </div>
        </div>
    </div>


    <div id="mapG">
        <div class="edit_map_controlls">
            <a href="#" id="btnInfo" class="btn btn-sm btn-info" title="<%: DbRes.T("Info", "Resources") %>"><span class="glyphicon glyphicon-info-sign" aria-hidden="true"></span></a>
            <a href="#" id="btnSearch" class="btn btn-sm btn-info" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a>
            <a href="#" id="btnInsertPoints" class="btn btn-sm btn-info" title="Внес на точки"><span class="glyphicon glyphicon-pushpin" aria-hidden="true"></span></a>
            <a href="#" id="btnDrawPoints" class="btn btn-sm btn-info" title="Цртање полигон"><span class="glyphicon glyphicon-unchecked" aria-hidden="true"></span></a>
            <a href="#" id="btnReport" class="btn btn-sm btn-info" title="Извештај"><span class="glyphicon glyphicon-list" aria-hidden="true"></span></a>
            

            <div id="divPopup" class="popup" hidden="true">
                <div class="modal-dialog modal-lg petsto" role="document">
                    <div class="modal-content">
                        <div class="modal-body">
                            <ul class="nav nav-tabs" id="tabovi">
                                <li class="active"><a id="aDolgoP" href="#dolgoP" data-toggle="tab"><%: DbRes.T("KatParceli", "Resources") %></a></li>
                              <li class=""><a id="sBrPredmet" href="#brojPredmet" data-toggle="tab">Број на предмет</a></li>           
                            </ul>
                            <div class="tab-content">
                                <div class="tab-pane fade active in" id="dolgoP">
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
                                <div class="tab-pane fade" id="brojPredmet">
                                    <div class="input-group">
                                        <input id="txtSearchPredmet" type="text" class="form-control" />
                                        <div class="input-group-btn">
                                            <a href="#" id="btnSubmitSearchPredmet" class="btn btn-link" title="<%: DbRes.T("Prebaraj", "Resources") %>"><span class="glyphicon glyphicon-search"></span></a>
                                        </div>
                                    </div>
                                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Vertical" Height="280px">
                                        <div id="progress" class="progress" hidden="true">
                                            <div class="progress-bar progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                                            </div>
                                        </div>
                                        <div id="divSearchResultBrPredmet"></div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="coordinates" runat="server" Value="" />
    <asp:HiddenField ID="polygon" runat="server" Value="" />



    <p class="text-left col-md-4" id="mouse-position"></p>
    <div id="data"></div>
    <div id="dataSearch"></div>
    <div id="dataSearchK"></div>
    <div id="dataSearchA"></div>
    <div id="dataSearchKParceli"></div>
    <p class="bl" id="razmer"></p>
    <iframe id="my_iframe" style="display: none;"></iframe>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/legalizacija.js") %>"></script>
    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>

    <script type="text/javascript">
        $(document).ready(function ()
        {
            $('#<%= selectOpstina.ClientID%>').select2({ placeholder: 'Избери општина', allowClear: true });
            $('#<%= selectOpstina.ClientID%>').val("");
            $('#<%= selectOpstina.ClientID%>').trigger("change.select2");

            $('#<%= namenaNaObjekt.ClientID%>').select2({ placeholder: 'Избери намена', allowClear: true });
            $('#<%= namenaNaObjekt.ClientID%>').val("");
            $('#<%= namenaNaObjekt.ClientID%>').trigger("change.select2");
        });
        //forsiraj enter kaj prebaruvanje
        $(document).keypress(function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
                return false;
            }
        });
        $("#txtSearchPredmet").keyup(function (event) {
            if (event.keyCode == 13) {
                $("#btnSubmitSearchPredmet").click();
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
        function closeShowModal() {
            $('#infoModal').modal('hide');
        };
        function showLegaliziraniModal() {
            $('#legaliziraniModal').modal('show');
        };
        function closeLegaliziraniModal() {
            $('#legaliziraniModal').modal('hide');
        };

        function showVnesModal() {
            $('#vnesModal').modal('show');
        };
        function closeVnesModal() {
            $('#vnesModal').modal('hide');
        };

        function ShowVnesTockiModal() {
            $('#vnesTockiModal').modal('show');
        };
        function closeVnesTockiModal() {
            $('#vnesTockiModal').modal('hide');
        };
        function showDocModal() {
            $('#vnesDocModal').modal('show');
        };
        function closeDocModal() {
            $('#vnesDocModal').modal('hide');
        };


        $('#btnSubmitSearchDolgo').click(function () {
            if ($('#txtSearchDolgo').val().length != 0) {
                $('#progressDolgo').show();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/SearchOutKat")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{searchString:"' + $('#txtSearchDolgo').val() + '"}',
                    success: function (result) {
                        $('#divSearchResultDolgo').empty();

                        $('#dataSearch').removeData();
                        var jsonResult = JSON.parse(result.d);
                        if (jsonResult !== null && jsonResult.length > 0) {
                            $('#divSearchResultDolgo').append('<h4><%: DbRes.T("KatParceli", "Resources") %></h4>');
                            var tableString2 = '<table class="table table-striped table-hover"><tr><th>Катастарска парцела</th><th>Катастарска општина</th><th>Намена</th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';

                            for (var item in jsonResult) {
                                $('#dataSearch').data('opfat' + jsonResult[item].Id, jsonResult[item].GeoJsonParceli);

                                tableString2 += '<tr><td>' + jsonResult[item].BrKatastarskaParcela + '</td><td>' + jsonResult[item].KatastarskaOpstina + '</td><td>' + jsonResult[item].NamenaNaObjekt + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'opfat' + jsonResult[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';

                            }
                            tableString2 += '</table>';
                            $('#divSearchResultDolgo').append(tableString2);


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
        $('#btnSubmitSearchPredmet').click(function () {
            if ($('#txtSearchPredmet').val().length != 0) {
                $('#progress').show();
                $.ajax({
                    type: "POST",
                    url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/SearchOutPredmet")%>",
                    async: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: '{searchString:"' + $('#txtSearchPredmet').val() + '"}',
                    success: function (result) {
                        $('#divSearchResultBrPredmet').empty();

                        $('#dataSearch').removeData();
                        var jsonResult = JSON.parse(result.d);
                        if (jsonResult !== null && jsonResult.length > 0) {
                            $('#divSearchResultBrPredmet').append('<h4>Број на предмет</h4>');
                            var tableString2 = '<table class="table table-striped table-hover"><tr><th>Катастарска парцела</th><th>Катастарска општина</th><th>Број на предмет</th><th><%: DbRes.T("Zum", "Resources") %></th></tr>';

                            for (var item in jsonResult) {
                                $('#dataSearch').data('opfat' + jsonResult[item].Id, jsonResult[item].GeoJsonParceli);

                                tableString2 += '<tr><td>' + jsonResult[item].BrKatastarskaParcela + '</td><td>' + jsonResult[item].KatastarskaOpstina + '</td><td>' + jsonResult[item].BrPredmet + '</td><td><a href="#" class="btn btn-link" onclick="goToGeojsonExtent2(\'' + 'opfat' + jsonResult[item].Id + '\');return false;"><span class="glyphicon glyphicon-search" aria-hidden="true"></span></a></td></tr>';

                            }
                            tableString2 += '</table>';
                            $('#divSearchResultBrPredmet').append(tableString2);


                        }
                        if ($('#divSearchResultBrPredmet').is(':empty')) {
                            $('#divSearchResultBrPredmet').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
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
        $('#btnCloseModel').click(function () {
            $('#<%= namenaNaObjekt.ClientID%>').val(null).trigger('change');
            $('#<%= selectOpstina.ClientID%>').val(null).trigger('change');
            document.getElementById('<%= katParcela.ClientID %>').value = "";
            document.getElementById('<%= brPredmet.ClientID %>').value = "";
            document.getElementById('<%= tipLegalizacija.ClientID %>').value = "";
            document.getElementById('<%= polygon.ClientID %>').value = "";
            document.getElementById('<%= brojObjekt.ClientID %>').value = "";
            tocka_x1.value = "";
            tocka_y1.value = "";
            tocka_x2.value = "";
            tocka_y2.value = "";
            tocka_x3.value = "";
            tocka_y3.value = "";
            tocka_x4.value = "";
            tocka_y4.value = "";
            $(".manuallyAdded").remove();
            closeVnesModal();
        });


        $('#btnCloseReportModal').click(function () {
            closeLegaliziraniModal();
        });

        $('#btnClose').click(function () {


            $('#<%= namenaNaObjekt.ClientID%>').val(null).trigger('change');
            $('#<%= selectOpstina.ClientID%>').val(null).trigger('change');

            document.getElementById('<%= brojObjekt.ClientID %>').value = "";
            document.getElementById('<%= katParcela.ClientID %>').value = "";
            document.getElementById('<%= brPredmet.ClientID %>').value = "";
            document.getElementById('<%= tipLegalizacija.ClientID %>').value = "";
            document.getElementById('<%= polygon.ClientID %>').value = "";
            tocka_x1.value = "";
            tocka_y1.value = "";
            tocka_x2.value = "";
            tocka_y2.value = "";
            tocka_x3.value = "";
            tocka_y3.value = "";
            tocka_x4.value = "";
            tocka_y4.value = "";
            $(".manuallyAdded").remove();
            closeVnesModal();
        });

        $('#btnPointsCloseModel').click(function () {
            tocka_x1.value = "";
            tocka_y1.value = "";
            tocka_x2.value = "";
            tocka_y2.value = "";
            tocka_x3.value = "";
            tocka_y3.value = "";
            tocka_x4.value = "";
            tocka_y4.value = "";
            $(".manuallyAdded").remove();
            closeVnesTockiModal();
        });
        $('#btnDocClose').click(function () {
         
            closeDocModal();
        }); 

        $('#btnPointsClose').click(function () {
            tocka_x1.value = "";
            tocka_y1.value = "";
            tocka_x2.value = "";
            tocka_y2.value = "";
            tocka_x3.value = "";
            tocka_y3.value = "";
            tocka_x4.value = "";
            tocka_y4.value = "";
            $(".manuallyAdded").remove();
            closeVnesTockiModal();
        });

        function showSuccessModal() {
            $('#successModal').modal('show');
        };
        function closeSuccessModal() {
            $('#successModal').modal('hide');
        };

        function showLoading() {
            $('#myLoadingModal').modal('show');
        };
        function hideLoading() {
            $('#myLoadingModal').modal('hide');
        };


        var n = 5;
        var m = 5;
        $('#dodadiTocka').click(function (i) {
            var tableString = '';
            tableString += '<tr class="rowTocka manuallyAdded"> <td><input type="text" id="tocka_x' + n + '" value="" class="form-control" /> </td><td><input type="text"  id="tocka_y' + m + '"" value="" class="form-control" /> </td></tr>';
            m++;
            n++;

            $('#tableVnesTocki').append(tableString);
        });


        function ValidatePoints() {
            var valid = true;
            var alphaExp = /^[a-zA-Z]+$/;

            $("#tableVnesTocki tr.rowTocka").each(function () {

                $(this).find("input[id^='tocka_x']").each(function () {

                    var value = $(this).val();
                    if (value === "" || value === null || value.length === 0 || value.match(alphaExp || value < 7534200 || value > 7553000)) {
                        valid = false;
                        return false;
                    }
                });
                $(this).find("input[id^='tocka_y']").each(function () {

                    var value = $(this).val();
                    if (value === "" || value === null || value.length === 0 || value.match(alphaExp) || value < 4638400 || value > 4666000) {
                        valid = false;
                        return false;
                    }
                });

            });

            return valid;
        }



        $('#btnZacuvajTocki').click(function () {
            var jsonRequestObject = {};
            var tockiNiza = [];
            var tocki = [];

            var pointsValid = ValidatePoints();
            if (pointsValid) {
                $("#tableVnesTocki tr.rowTocka").each(function () {
                    var tocka = [];

                    $(this).find("input[id^='tocka_']").each(function () {
                        var tockaId = this.id.split('_')[1];
                        var value = $('#' + this.id).val();
                        tocka.push(value);
                    }

                    );

                    tockiNiza.push(tocka);
                    console.log(tocka);
                });
                var prvaTocka = tockiNiza[0];
                var poslednaTocka = tockiNiza[tockiNiza.length - 1];
                if (prvaTocka != poslednaTocka)
                    tockiNiza.push(prvaTocka);
                tocki.push(tockiNiza);

                jsonRequestObject["type"] = "Polygon";
                jsonRequestObject["coordinates"] = tocki;

                $("#<%= polygon.ClientID %>").val(JSON.stringify(jsonRequestObject));
                closeVnesTockiModal();
                showVnesModal();
            }
            else {
                alert("Внесете валидни вредности!");

            }
        });


        $('#btnZacuvajDoc').click(function () {
            var length = document.getElementById('document_info').files.length;
            if (length == 0){
                alert("Внесете документ");
            }

            else{
                var id = document.getElementById('<%= idLegalizacija.ClientID %>').value;
                for (var i = 0; i < length; i++) {
                    var file = "";
                    file = document.querySelector('#document_info').files[i];
                    zacuvaj(file, id);
                }

                $('#document_info').val("");
            }
        });

        function formatDate(datum) {
            var monthNames = ["Јануари", "Февруари", "Март", "Април", "Мај", "Јуни", "Јули", "Август", "Септември", "Октомври", "Ноември", "Декември"];
            var d = new Date(datum);
            var month = (d.getMonth() + 1);
            if (d.getDate() < 10) {
                return '0' + d.getDate() + ' ' + monthNames[d.getMonth()] + ' ' + d.getFullYear();
            }
            else {
                return d.getDate() + ' ' + monthNames[d.getMonth()] + ' ' + d.getFullYear();
            }
        };
        function zacuvaj(file, id) {
            if (file != null) {
                var reader = new FileReader();
                reader.readAsDataURL(file);
                reader.onload = (function () {
                    $.ajax({
                        type: "POST",
                        url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/ZacuvajDokumenti")%>",
                        async: true,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: '{ fileBase64:"' + reader.result + '", filename:"' + file.name + '", id:"' + id + '" }',

                        success: function (result) {
                            listDokumenti(id);
                           
                        },
                        error: function (p1, p2, p3) {
                            alert(p1.status);
                            alert(p3);
                        }

                    });

                });

            }
          
        }


        $('#btnZacuvaj').click(function () {
            var opstina = $('#<%= selectOpstina.ClientID%>').val();
            var parcela = $('#<%= katParcela.ClientID%>').val();
            var predmet = $('#<%= brPredmet.ClientID%>').val();
            var namena = $('#<%= namenaNaObjekt.ClientID%>').val();
            var tip = $('#<%= tipLegalizacija.ClientID%>').val();
            var brobjekt = $('#<%= brojObjekt.ClientID%>').val();
            if (opstina === null) {
                opstina = '';
            }
            if (parcela === null) {
                parcela = '';
            }
            if (predmet === null) {
                predmet = '';
            }
            if (namena === null) {
                namena = '';
            }
            if (tip === null) {
                tip = '';
            }
            if (brobjekt === null) {
                brobjekt = '';
            }
            if (opstina && parcela && predmet && namena) {
              
                    $.ajax({
                        type: "POST",
                        url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/ZacuvajBaranje")%>",
                        async: true,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: '{katastarskaOpstina:"' + $('#<%= selectOpstina.ClientID%>').val() + '" , katastarskaParcela:"' + $('#<%= katParcela.ClientID%>').val() + '", broj: "' + $('#<%= brPredmet.ClientID%>').val() + '" , namenaobjekt:"' + $('#<%= namenaNaObjekt.ClientID%>').val() + '" , brojObjekt:"' + brobjekt + '", tipLegalizacija:"' + tip + '" , polygon:' + JSON.stringify($('#<%= polygon.ClientID%>').val()) + '}',
                        
                        success: function (result) {
                            $('#<%= selectOpstina.ClientID%>').val("");
                            $('#<%= katParcela.ClientID%>').val("");
                            $('#<%= brPredmet.ClientID%>').val("");
                            $('#<%= namenaNaObjekt.ClientID%>').val("");
                            $('#<%= brojObjekt.ClientID%>').val("");
                            $('#<%= tipLegalizacija.ClientID%>').val("");
                            $('#<%= polygon.ClientID%>').val("");
                            $('#<%= namenaNaObjekt.ClientID%>').val(null).trigger('change');
                            $('#<%= selectOpstina.ClientID%>').val(null).trigger('change');
                            tocka_x1.value = "";
                            tocka_y1.value = "";
                            tocka_x2.value = "";
                            tocka_y2.value = "";
                            tocka_x3.value = "";
                            tocka_y3.value = "";
                            tocka_x4.value = "";
                            tocka_y4.value = "";
                            $(".manuallyAdded").remove();
                            closeVnesModal();
                            var id = result.d;
                            docShow(id);
                            //showSuccessModal();
                            alert('Успешно внесено барање');
                            
                        },
                        error: function (p1, p2, p3) {
                            alert(p1.status);
                            alert(p3);
                        }
                      
                    });
                
            }
            else {
                alert("Не се пополнети задолжителните полиња!");
            }
        });



    var infoTool = false;
    var vnesTockiTool = false;
    var crtanjeTool = false;
    var searchTool = false;
    var reportTool = false;
    var draw;
    var typeSelect = 'None';
    var geojsonStr;
    var source = new ol.source.Vector({ wrapX: false });


    $('#btnInfo').click(function () {
        vnesTockiTool = false;
        crtanjeTool = false;
        searchTool = false;
        reportTool = false;
        $('#divPopup').hide();
        if (infoTool) {
            $('#btnInfo').removeClass("btn-danger");
            $('#btnInfo').addClass("btn-info");
            clearSourceDraw();
        } else {
            $('#btnInfo').addClass("btn-danger");
            $('#btnInfo').removeClass("btn-info");
            $('#btnInsertPoints').removeClass("btn-danger");
            $('#btnInsertPoints').addClass("btn-info");
            $('#btnDrawPoints').removeClass("btn-danger");
            $('#btnDrawPoints').addClass("btn-info");
            $('#btnSearch').addClass("btn-info");
            $('#btnSearch').removeClass("btn-danger");
            $('#btnReport').addClass("btn-info");
            $('#btnReport').removeClass("btn-danger");
           
            document.body.style.cursor = 'default';
            typeSelect = "None";
            map.removeInteraction(draw);
            addInteraction(typeSelect);
            clearSourceDraw();

        }
        infoTool = !infoTool;
    });

    $('#btnInsertPoints').click(function () {
        infoTool = false;
        crtanjeTool = false;
        searchTool = false;
        reportTool = false;
       
        $('#divPopup').hide();
        if (vnesTockiTool) {
            $('#btnInsertPoints').removeClass("btn-danger");
            $('#btnInsertPoints').addClass("btn-info");
        } else {
            $('#btnInfo').removeClass("btn-danger");
            $('#btnInfo').addClass("btn-info");
            $('#btnInsertPoints').addClass("btn-danger");
            $('#btnInsertPoints').removeClass("btn-info");
            $('#btnDrawPoints').removeClass("btn-danger");
            $('#btnDrawPoints').addClass("btn-info");
            $('#btnSearch').removeClass("btn-danger");
            $('#btnSearch').addClass("btn-info");
            $('#btnReport').addClass("btn-info");
            $('#btnReport').removeClass("btn-danger");
           

            document.body.style.cursor = 'default';
            typeSelect = "None";
            map.removeInteraction(draw);
            addInteraction(typeSelect);
            clearSourceDraw();
            ShowVnesTockiModal();
        }
        vnesTockiTool = !vnesTockiTool;

    });

    $('#btnDrawPoints').click(function () {
        infoTool = false;
        vnesTockiTool = false;
        searchTool = false;
        reportTool = false;
        $('#divPopup').hide();
        if (crtanjeTool) {
            $('#btnDrawPoints').removeClass("btn-danger");
            $('#btnDrawPoints').addClass("btn-info");
            clearSourceDraw();
            map.removeInteraction(draw);
        } else {
            $('#btnInfo').removeClass("btn-danger");
            $('#btnInfo').addClass("btn-info");
            $('#btnInsertPoints').removeClass("btn-danger");
            $('#btnInsertPoints').addClass("btn-info");
            $('#btnDrawPoints').addClass("btn-danger");
            $('#btnDrawPoints').removeClass("btn-info");
            $('#btnSearch').addClass("btn-info");
            $('#btnSearch').removeClass("btn-danger");
            $('#btnReport').addClass("btn-info");
            $('#btnReport').removeClass("btn-danger");
         
            //document.body.style.cursor = 'default';
            typeSelect = "Polygon";
            //map.removeInteraction(draw);
            addInteraction(typeSelect);

        }
        crtanjeTool = !crtanjeTool;
    });

    $('#btnSearch').click(function () {
        infoTool = false;
        vnesTockiTool = false;
        crtanjeTool = false;
        reportTool = false;
        $('#divPopup').toggle();
        if (searchTool) {
            $('#btnSearch').removeClass("btn-danger");
            $('#btnSearch').addClass("btn-info");
        } else {
            $('#btnSearch').addClass("btn-danger");
            $('#btnSearch').removeClass("btn-info");
            $('#btnInfo').removeClass("btn-danger");
            $('#btnInfo').addClass("btn-info");
            $('#btnInsertPoints').removeClass("btn-danger");
            $('#btnInsertPoints').addClass("btn-info");
            $('#btnDrawPoints').removeClass("btn-danger");
            $('#btnDrawPoints').addClass("btn-info");
            $('#btnReport').addClass("btn-info");
            $('#btnReport').removeClass("btn-danger");
         
            document.body.style.cursor = 'default';
            typeSelect = "None";
            map.removeInteraction(draw);
            addInteraction(typeSelect);
            clearSourceDraw();
        }
        searchTool = !searchTool;
    });



    $('#btnReport').click(function () {
        infoTool = false;
        vnesTockiTool = false;
        crtanjeTool = false;
        searchTool = false;

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
            $('#btnDrawPoints').removeClass("btn-danger");
            $('#btnDrawPoints').addClass("btn-info");
            $('#btnSearch').removeClass("btn-danger");
            $('#btnSearch').addClass("btn-info");
            $('#btnInsertPoints').removeClass("btn-danger");
            $('#btnInsertPoints').addClass("btn-info");

            document.body.style.cursor = 'default';
            typeSelect = "None";
            map.removeInteraction(draw);
            addInteraction(typeSelect);
            clearSourceDraw();
            CountLegalizacija();
            //showLegaliziraniModal();
        }
        reportTool = !reportTool;

    });

    map.on('singleclick', function (evt) {

        if (infoTool) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/InfoLegalizacijaTool")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{coordinates:"' + evt.coordinate + '"}',
                success: function (result) {
                    $("#<%= polygon.ClientID %>").val(evt.coordinate);
                            $('#divInfo').empty();
                            var jsonResult = JSON.parse(result.d);
                            if (jQuery.isEmptyObject(jsonResult)) {
                                $('#divInfo').append('<h4><%: DbRes.T("NemaPodatoci", "Resources") %></h4>');
                        } else {
                            var tableString = '<table class="table table-hover"><tr><th>Катастарска општина</th><th>Број на катастарска парцела</th><th>Број на предмет</th><th>Намена на објект</th><th>Број на објект</th><th>Тип на легализација</th><th>Финализирај постапка</th><th>Документи</th><th>Избриши објект</th></tr>';
                            //console.log(jsonResult );
                            for (var key in jsonResult) {

                                if (jsonResult[key].Active) {
                                    if (jsonResult[key].Path == null || jsonResult[key].Path == '') {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].KatastarskaOpstina) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKatastarskaParcela) + '</td><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].NamenaNaObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].BrojObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].TipLegalizacija) + '</td><td><a href="#" class="btn btn-link" onclick="addZavrseni(\'' + jsonResult[key].Id + '\');">Заврши постапка</a></td><td><a href="#" class="btn btn-link" onclick="docShow(\'' + jsonResult[key].Id + '\');return false;">Документи</a></td><td><a href="#" class="btn btn-link" onclick="deleteObj(\'' + jsonResult[key].Id + '\');return false;">Избриши</a></td></tr>';

                                    } else {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].KatastarskaOpstina) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKatastarskaParcela) + '</td><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].NamenaNaObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].BrojObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].TipLegalizacija) + '</td><td><a href="#" class="btn btn-link" onclick="addZavrseni(\'' + jsonResult[key].Id + '\');">Заврши постапка</a></td><td><a href="#" class="btn btn-link" onclick="docShow(\'' + jsonResult[key].Id + '\');return false;">Документи</a></td><td><a href="#" class="btn btn-link" onclick="deleteObj(\'' + jsonResult[key].Id + '\');return false;">Избриши</a></td></tr>';
                                    }
                                }
                                else {
                                    if (jsonResult[key].Path == null || jsonResult[key].Path == '') {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].KatastarskaOpstina) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKatastarskaParcela) + '</td><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].NamenaNaObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].BrojObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].TipLegalizacija) + '</td><td><span class="glyphicon glyphicon-ok"></span></td><td><a href="#" class="btn btn-link" onclick="docShow(\'' + jsonResult[key].Id + '\');return false;">Документи</a></td><td><a href="#" class="btn btn-link" onclick="deleteObj(\'' + jsonResult[key].Id + '\');return false;">Избриши</a></td></tr>';
                                    } else {
                                        tableString += '<tr><td>' + nullToEmptyString(jsonResult[key].KatastarskaOpstina) + '</td><td>' + nullToEmptyString(jsonResult[key].BrKatastarskaParcela) + '</td><td>' + nullToEmptyString(jsonResult[key].BrPredmet) + '</td><td>' + nullToEmptyString(jsonResult[key].NamenaNaObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].BrojObjekt) + '</td><td>' + nullToEmptyString(jsonResult[key].TipLegalizacija) + '</td><td><span class="glyphicon glyphicon-ok"></span></td><td><a href="#" class="btn btn-link" onclick="docShow(\'' + jsonResult[key].Id + '\');return false;">Документи</a></td><td><a href="#" class="btn btn-link" onclick="deleteObj(\'' + jsonResult[key].Id + '\');return false;">Избриши</a></td></tr>';
                                    }
                                }
                            }

                            tableString += '</table>';
                            $('#divInfo').append(tableString);
                        }

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


        function CountLegalizacija() {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/CountLegalizacija")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                //data: '{}',
                success: function (result) {
                    $('#divLegalizirani').empty();
                    var jsonResult = JSON.parse(result.d);
                    var tableString = '<table class="table table-hover"><tr><th>Легализација</th><th>Број на објекти</th></tr>';
                    for (var key in jsonResult) {
                        if (jsonResult[key].Legalizirani == 'tekovni') {
                            tableString += '<tr><td>Тековна легализација:</td><td>' + nullToEmptyString(jsonResult[key].Count) + '</td></tr>';
                        }
                        else {
                            tableString += '<tr><td>Завршена легализација:</td><td>' + nullToEmptyString(jsonResult[key].Count) + '</td></tr>';
                        }

                    }
                    tableString += '</table>';
                    $('#divLegalizirani').append(tableString);
                    showLegaliziraniModal();


                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p2);
                    alert(p3);
                }
            });

        };


        function docShow(id) {
  
            document.getElementById('<%= idLegalizacija.ClientID %>').value = "";
            $("#<%= idLegalizacija.ClientID %>").val(id);
            listDokumenti(id);
            showDocModal();
        }
        function listDokumenti(id) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/ListDokumenti")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:"' + id + '"}',
                success: function (result) {
                    $('#divListDokumenti').empty();
                    var jsonResult = JSON.parse(result.d);
                    if (jQuery.isEmptyObject(jsonResult)) {
                        $('#divListDokumenti').append('<h4>Нема документи</h4>');
                    }
                    else {
                        var tableString = '<br /><table class="table table-hover"><tr><th>Име</th><th>Датум</th><th>Избриши</th></tr>';
                        for (var key in jsonResult) {
                            tableString += '<tr><td><a href="#" class="btn btn-link" onclick="downloadDoc(\'' + jsonResult[key].Filename + '\',\'' + jsonResult[key].Path + '\');">' + jsonResult[key].Filename + '</a></td><td>' + formatDate(jsonResult[key].Datum) + '</td><td><a href="#" class="btn btn-link" onclick="deleteDocument(\'' + jsonResult[key].Id + '\',\'' + id + '\');return false;"><span class="glyphicon glyphicon-remove-sign" aria-hidden="true"></span></a></td></tr>';

                        }
                        tableString += '</table>';
                        $('#divListDokumenti').append(tableString);
                    }
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p2);
                    alert(p3);
                }
            });
        }
        function deleteDocument(id, idLegalizacija) {
            console.log(id + " " + idLegalizacija);
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/IzbrisiDokument")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + id + '}',
                success: function (result) {
                    listDokumenti(idLegalizacija);
                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }
        function deleteObj(id) {
          
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/Izbrisi")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + id + '}',
                success: function (result) {
                    alert('Успешно избришан објект');
                    closeShowModal();

                },
                error: function (p1, p2, p3) {
                    hideLoading();
                    alert(p1.status);
                    alert(p3);
                }
            });
        }

        function addInteraction(typeSelect) {
            var value = 'Polygon';
            if (value == typeSelect) {
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
                    $("#<%= polygon.ClientID %>").val(geojsonStr);
                    showVnesModal();
                });
            }
        }
        function nullToEmptyString(obj) {
            if (obj) {
                return obj;
            }
            return "/";
        }
        function addZavrseni(id) {
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Legalizacija/Legalizacija.aspx/UpdateStatusGradba")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:"' + id + '"}',
                success: function (result) {
                    alert('Успешно финализиравте постапка');
                    closeShowModal();
                },
                error: function (p1, p2, p3) {
                    alert(p1.status);
                    alert(p2);
                    alert(p3);
                }
            });

        }

        function downloadDoc(docname, path) {
            // Get file name from url.
            if (path == null || path == '') {
            }
            else {
                var root = location.protocol + '//' + location.host + '/';
                var url = root + path;
                //console.log(root);
                //console.log(path);
                //console.log(url);
                var filename = url.substring(url.lastIndexOf("/") + 1).split("?")[0];
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'blob';
                xhr.onload = function () {
                    var a = document.createElement('a');
                    a.href = window.URL.createObjectURL(xhr.response); // xhr.response is a blob
                    a.download = docname; // Set the file name.
                    console.log(docname);
                    a.style.display = 'none';
                    document.body.appendChild(a);
                    a.click();
                    delete a;
                };
                xhr.open('GET', url);
                xhr.send();
            }
        };

    </script>

</asp:Content>