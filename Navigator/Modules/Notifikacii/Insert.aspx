<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Insert.aspx.cs" Inherits="Navigator.Modules.Notifikacii.Insert" %>

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

      <div class="col-md-2">
        <fieldset class="form-horizontal">
            <div class="form-group">    
                <h1></h1>          
                <h4><%: DbRes.T("ModulDodavanjePoi", "Resources") %></h4>
                <br />           
                <strong class="control-label"><%: DbRes.T("TemaOdInteres", "Resources") %></strong>
                <div>
                    <select  required="true" id="selectTema" class="js-example-placeholder-single form-control" style="width: 100%" runat="server"></select>
                </div>
                <br />               
                <strong class="control-label"><%: DbRes.T("PodTemaOdInteres", "Resources") %></strong>
                <div>
                    <select required="true" id="selectPodTema" class="js-example-placeholder-single form-control" style="width: 100% " ></select>
                </div>
                <br />
                <div>
                    <asp:TextBox runat="server" ID="datumOd" CssClass="form-control" TextMode="SingleLine" data-date-format="DD/MM/YYYY" placeholder="<%$ Resources:Resources,VaziOd %>"/>  
                    <asp:RequiredFieldValidator ValidationGroup="ValnNotification" runat="server" ControlToValidate="datumOd" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiDatum %>"/> 
                </div>
                <div>
                    <asp:TextBox runat="server" ID="datumDo" CssClass="form-control" TextMode="SingleLine" data-date-format="DD/MM/YYYY" placeholder="<%$ Resources:Resources,VaziDo %>"  />  
                    <asp:RequiredFieldValidator ValidationGroup="ValnNotification"  runat="server" ControlToValidate="datumDo" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiDatum %>"/> 
                </div>
                 <strong class="control-label"><%: DbRes.T("Komentar", "Resources") %></strong>
                <div>
                    <asp:TextBox ID="komentar" runat="server" Rows="5" TextMode="MultiLine" class="js-example-placeholder-single form-control" ></asp:TextBox>
                    <asp:RequiredFieldValidator ValidationGroup="ValnNotification"  runat="server" ControlToValidate="komentar" CssClass="text-danger" ErrorMessage="<%$ Resources:Resources,ZadolzitelnoVnesiKomentar %>"  /> 
                </div>
            </div>
            <div>
                <asp:Button runat="server"  ValidationGroup="ValnNotification"  ID="btnSave" CssClass="btn btn-default" Text="<%$ Resources:Resources,Zacuvaj %>" OnClick="btnSave_Click" />
            </div>
        </fieldset>
    </div>

    <div class="col-md-10"  style="left:45px">
        <div id="map">           
            <div class="edit_map_controlls" style="right:15px" >
               <a href="#" id="btnPolygon" class="btn btn-sm btn-info"  title="<%: DbRes.T("Vnesi", "Resources") %>"><span class="glyphicon glyphicon-pencil" aria-hidden="true"></span></a>  
            </div>
        </div>
    </div>
   
    <div id="data"></div>
    <asp:HiddenField ID="fkPodtema" runat="server" Value="" />
    <asp:HiddenField ID="coordinates" runat="server" Value="" />
    <script type="text/javascript" src="<%=ResolveClientUrl("~/Scripts/Maps/notifikacii.js") %>"></script>

    <script src='<%= ResolveUrl("~/Scripts/select2.js")%>'></script>
    <script>
        var draw;
        var geojsonStr;
        var polygonTool = false;
        document.getElementById('<%= coordinates.ClientID %>').value = "";
        $(document).ready(function () {
            $('#<%= selectTema.ClientID%>').select2({ placeholder: '<%: DbRes.T("IzberiTema", "Resources") %>', allowClear: true });
              $('#<%= selectTema.ClientID%>').val("");
            $('#<%= selectTema.ClientID%>').trigger("change.select2");
          });
          $('#<%= selectTema.ClientID%>').on("change", function (e) {
              $("#selectPodTema").html('').select2({ data: [{ id: '', text: '' }] }).empty();
            $.ajax({
                type: "POST",
                url: "<%= Page.ResolveUrl("~/Modules/Notifikacii/Insert.aspx/FillSubThemes")%>",
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{id:' + $('#<%= selectTema.ClientID%>').val() + '}',
                success: function (result) {
                    if (result.d.length > 0) {
                        var jsonResult = JSON.parse(result.d);
                        $('#selectPodTema').select2({ placeholder: '<%: DbRes.T("IzberiPodTema", "Resources") %>', allowClear: true, data: jsonResult });
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

        $("#<%= btnSave.ClientID %>").click(function ()  {
            var textVal = document.getElementById('<%= coordinates.ClientID %>').value;
            if (textVal == ''){
                alert("<%: DbRes.T("NemaVnesenoPoligon", "Resources") %>");
                document.getElementById('<%= datumOd.ClientID %>').value = "";
                document.getElementById('<%= datumDo.ClientID %>').value = "";
                document.getElementById('<%= komentar.ClientID %>').value = "";
                return false;
            }
            else {
                return true;
            }
        })


        $('#selectPodTema').on("change", function (e) {
            document.getElementById('<%= fkPodtema.ClientID %>').value = "";
            var podtemaId = $('#selectPodTema').val();

            $("#<%= fkPodtema.ClientID %>").val(podtemaId);
          
         });

        $(function () {
           $('#<%=datumOd.ClientID%>').datetimepicker({});     
            $('#<%=datumDo.ClientID%>').datetimepicker({});  
            
        });

        $('#btnPolygon').click(function () {
            
            if (polygonTool) {
                $('#btnPolygon').removeClass("btn-danger");
                $('#btnPolygon').addClass("btn-info");
                
                //map.removeInteraction(draw);
                //console.log('map.removeInteraction(draw);');
                clearSourceDraw();
                map.removeInteraction(draw);

            } else {
                $('#btnPolygon').addClass("btn-danger");
                $('#btnPolygon').removeClass("btn-info");    
               
                addInteraction();
            }
            polygonTool = !polygonTool;
        });

        //function SerializeMapData(w) {
        //    var gJson = new ol.format.GeoJSON();
        //    console.log(gJson.writeFeatures(w.getGeometry, { 'dataProjection': 'EPSG:3264', 'featureProjection': 'EPSG:3264' }));
        //}

        function addInteraction() {            
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
                });

            }            
        }
    </script>
</asp:Content>
