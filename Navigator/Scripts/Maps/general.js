// CAPTURE THE MOUSE COORDINATES
var mousePositionControl = new ol.control.MousePosition({
    coordinateFormat: ol.coordinate.createStringXY(4),
    projection: projection,
    // comment the following two lines to have the mouse position
    // be placed within the map.
    className: "custom-mouse-position",
    target: document.getElementById("mouse-position"),
    undefinedHTML: ""
});

var scaleLineControl = new ol.control.ScaleLine({
    geodesic: true
});
var keyMap = new ol.control.OverviewMap();

scaleLineControl.setUnits('metric');
// LIST THE LAYERS THAT NEED TO BE DRAWN
var layersToAdd = generalLayers.split(',');
var layersOff = generalInactive.split(',');
var layers = [];

var infrastrukturalayersToAdd = infrastrukturaLayers.split(',');
var infrastrukturalayersOff = infrastrukturaInactive.split(',');
var infrastrukturaGroupLayers = new ol.Collection();
var infrastrukturaGroup = new ol.layer.Group({ layers: infrastrukturaGroupLayers });
infrastrukturaGroup.set("title", 'Инфраструктура');

for (var layer in layersToAdd) {
    switch (layersToAdd[layer]) {
        case 'layerUlici':
            layers.push(layerUlici);
            break;
        case 'layerUrbanaOprema':
            layers.push(layerUrbanaOprema);
            break;
        case 'layerSintezen':
            layers.push(layerSintezen);
            break;
        case 'layerSoobrakaen':
            layers.push(layerSoobrakaen);
            break;
        case 'layerInfrastrukturen':
            layers.push(layerInfrastrukturen);
            break;
        case 'layerGupGc':
            layers.push(layerGupGc);
            break;
        case 'layerGup':
            layers.push(layerGup);
            break;
        case 'layerOpstAkt':
            layers.push(layerOpstAkt);
            break;
        case 'layerDup':
            layers.push(layerDup);
            break;
        case 'layerLupd':
            layers.push(layerLupd);
            break;
        case 'layerUps':
            layers.push(layerUps);
            break;
        case 'layerAup':
            layers.push(layerAup);
            break;
        case 'layerInfra':
            layers.push(layerInfra);
            break;
        case 'layerDupd':
            layers.push(layerDupd);
            break;
        case 'layerDupIdni':
            layers.push(layerDupIdni);
            break;
        case 'layerKP':
            layers.push(layerKP);
            break;
        case 'layerKO':
            layers.push(layerKO);
            break;
        case 'layerWmsOrtofoto':
            layers.push(layerWmsOrtofoto);
            break;
        case 'errorLayer':
            layers.push(errorLayer);
            break;
        case 'searchLayer':
            layers.push(searchLayer);
            break;
        case 'searchLayerKP':
            layers.push(searchLayerKP);
            break;
        case 'layerSatelitska':
            layers.push(layerSatelitska);
            break;
        case 'layerOsmLocal':
            layers.push(layerOsmLocal);
            break;
        case 'layerOsmLocalCache':
            layers.push(layerOsmLocalCache);
            break;
        case 'layerSintezenCache':
            layers.push(layerSintezenCache);
            break;
        case 'layerSatelitskaCache':
            layers.push(layerSatelitskaCache);
            break;
        case 'layerOrtofotoCache':
            layers.push(layerOrtofotoCache);
            break;
        case 'layerKZ':
            layers.push(layerKZ);
            break;
        case 'layerSoobrakajnici':
            layers.push(layerSoobrakajnici);
            break;
        case 'layerSoobrakajniciCache':
            layers.push(layerSoobrakajniciCache);
            break;
        case 'layerWorldStreetMap':
            layers.push(layerWorldStreetMap);
            break;
        case 'layerWorldTopoMap':
            layers.push(layerWorldTopoMap);
            break;
        case 'layerWorldImagery':
            layers.push(layerWorldImagery);
            break;
        case 'layerOsvetluvanje':
            layers.push(layerOsvetluvanje);
            break;
        case 'layerOprema':
            layers.push(layerOprema);
            break;
        case 'layerJavnaPovrsina':
            layers.push(layerJavnaPovrsina);
            break;
        case 'layerDefekt':
            layers.push(layerDefekt);
            break;
        case 'layerVodovod':
            layers.push(layerVodovod);
            break;
        case 'layerRPBeli':
            layers.push(layerRPBeli);
            break;
        case 'layerRPTransparent':
            layers.push(layerRPTransparent);
            break;
        case 'layerGranica':
            layers.push(layerGranica);
            break;
        case 'layerOdobrenia':
            layers.push(layerOdobrenia);
            break;
        case 'layerElektroenergetski':
            layers.push(layerElektroenergetski);
            break;
        case 'layerHidrotehnicki':
            layers.push(layerHidrotehnicki);
            break;
        case 'layerVodovoden':
            layers.push(layerVodovoden);
            break;
        case 'layerGasovoden':
            layers.push(layerGasovoden);
            break;
        case 'layerFekalen':
            layers.push(layerFekalen);
            break;
        case 'layerTelefonskaMreza':
            layers.push(layerTelefonskaMreza);
            break;
        case 'layerGUPSintezen':
            layers.push(layerGUPSintezen);
            break;
        case 'layerZastiteniPodracja':
            layers.push(layerZastiteniPodracja);
            break;
        case 'layerAdresi':
            layers.push(layerAdresi);
            break;
        case 'layerDupOld':
            layers.push(layerDupOld);
            break;
        case 'layerUps_old':
            layers.push(layerUps_old);
            break;
        case 'layerLupd_old':
            layers.push(layerLupd_old);
            break;
        case 'layerDupd_old':
            layers.push(layerDupd_old);
            break; 
        case 'layerOsnovniUcilista':
            layers.push(layerOsnovniUcilista);
            break;
        case 'layerJavniPovrsini':
            layers.push(layerJavniPovrsini);
            break;
        case 'layerTrotoari':
            layers.push(layerTrotoari);
            break;
        case 'layerUrbanaOprema':
            layers.push(layerUrbanaOprema);
            break;
        case 'layerErozivniPodracja':
            layers.push(layerErozivniPodracja);
            break;
        case 'layerZagrozeniErozija':
            layers.push(layerZagrozeniErozija);
            break;
        case 'layerSegmentiUlici':
            layers.push(layerSegmentiUlici);
            break;
        case 'layerInfrastrukturniProekti':
            layers.push(layerInfrastrukturniProekti);
            break;
        case 'layerPodlogi':
            layers.push(layerPodlogi);
            break;
     
    }
}

for (var layer in layersOff) {
    switch (layersOff[layer]) {
        case 'layerUlici':
            layerUlici.setVisible(false);
            break;
        case 'layerUrbanaOprema':
            layerUrbanaOprema.setVisible(false);
            break;
        case 'layerSintezen':
            layerSintezen.setVisible(false);
            break;
        case 'layerSoobrakaen':
            layerSoobrakaen.setVisible(false);
            break;
        case 'layerInfrastrukturen':
            layerInfrastrukturen.setVisible(false);
            break;
        case 'layerGupGc':
            layerGupGc.setVisible(false);
            break;
        case 'layerGup':
            layerGup.setVisible(false);
            break;
        case 'layerOpstAkt':
            layerOpstAkt.setVisible(false);
            break;
        case 'layerDup':
            layerDup.setVisible(false);
            break;
        case 'layerLupd':
            layerLupd.setVisible(false);
            break;
        case 'layerUps':
            layerUps.setVisible(false);
            break;
        case 'layerAup':
            layerAup.setVisible(false);
            break;
        case 'layerInfra':
            layerInfra.setVisible(false);
            break;
        case 'layerDupd':
            layerDupd.setVisible(false);
            break;
        case 'layerDupIdni':
            layerDupIdni.setVisible(false);
            break;
        case 'layerKP':
            layerKP.setVisible(false);
            break;
        case 'layerKO':
            layerKO.setVisible(false);
            break;
        case 'layerWmsOrtofoto':
            layerWmsOrtofoto.setVisible(false);
            break;
        case 'errorLayer':
            errorLayer.setVisible(false);
            break;
        case 'searchLayer':
            searchLayer.setVisible(false);
            break;
        case 'searchLayerKP':
            searchLayerKP.setVisible(false);
            break;
        case 'layerSatelitska':
            layerSatelitska.setVisible(false);
            break;
        case 'layerOsmLocal':
            layerOsmLocal.setVisible(false);
            break;
        case 'layerOsmLocalCache':
            layerOsmLocalCache.setVisible(false);
            break;
        case 'layerSintezenCache':
            layerSintezenCache.setVisible(false);
            break;
        case 'layerSatelitskaCache':
            layerSatelitskaCache.setVisible(false);
            break; 
        case 'layerOrtofotoCache':
            layerOrtofotoCache.setVisible(false);
            break;
        case 'layerKZ':
            layerKZ.setVisible(false);
            break;
        case 'layerSoobrakajnici':
            layerSoobrakajnici.setVisible(false);
            break;
        case 'layerSoobrakajniciCache':
            layerSoobrakajniciCache.setVisible(false);
            break;
        case 'layerWorldStreetMap':
            layerWorldStreetMap.setVisible(false);
            break;
        case 'layerWorldTopoMap':
            layerWorldTopoMap.setVisible(false);
            break;
        case 'layerWorldImagery':
            layerWorldImagery.setVisible(false);
            break;
        case 'layerOsvetluvanje':
            layerOsvetluvanje.setVisible(false);
            break;
        case 'layerOprema':
            layerOprema.setVisible(false);
            break;
        case 'layerJavnaPovrsina':
            layerJavnaPovrsina.setVisible(false);
            break;
        case 'layerDefekt':
            layerDefekt.setVisible(false);
            break;
        case 'layerVodovod':
            layerVodovod.setVisible(false);
            break;
        case 'layerRPBeli':
            layerRPBeli.setVisible(false);
            break;
        case 'layerRPTransparent':
            layerRPTransparent.setVisible(false);
            break;
        case 'layerGranica':
            layerGranica.setVisible(false);
            break;
        case 'layerOdobrenia':
            layerOdobrenia.setVisible(false);
            break;
        case 'layerElektroenergetski':
            layerElektroenergetski.setVisible(false);
            break;
        case 'layerHidrotehnicki':
            layerHidrotehnicki.setVisible(false);
            break;
        case 'layerVodovoden':
            layerVodovoden.setVisible(false);
            break;
        case 'layerGasovoden':
            layerGasovoden.setVisible(false);
            break;
        case 'layerFekalen':
            layerFekalen.setVisible(false);
            break;
        case 'layerTelefonskaMreza':
            layerTelefonskaMreza.setVisible(false);
            break;
        case 'layerGUPSintezen':
            layerGUPSintezen.setVisible(false);
            break;
        case 'layerZastiteniPodracja':
            layerZastiteniPodracja.setVisible(false);
            break;
        case 'layerAdresi':
            layerAdresi.setVisible(false);
            break;
        case 'layerDupOld':
            layerDupOld.setVisible(false);
            break;
        case 'layerUps_old':
            layerUps_old.setVisible(false);
            break;
        case 'layerLupd_old':
            layerLupd_old.setVisible(false);
            break;
        case 'layerDupd_old':
            layerDupd_old.setVisible(false);
            break;
        case 'layerOsnovniUcilista':
            layerOsnovniUcilista.setVisible(false);
            break;
        case 'layerJavniPovrsini':
            layerJavniPovrsini.setVisible(false);
            break;
        case 'layerTrotoari':
            layerTrotoari.setVisible(false);
            break;
        case 'layerUrbanaOprema':
            layerUrbanaOprema.setVisible(false);
            break;
        case 'layerErozivniPodracja':
            layerErozivniPodracja.setVisible(false);
            break;
        case 'layerZagrozeniErozija':
            layerZagrozeniErozija.setVisible(false);
            break;
        case 'layerSegmentiUlici':
            layerSegmentiUlici.setVisible(false);
            break;
        case 'layerInfrastrukturniProekti':
            layerInfrastrukturniProekti.setVisible(false);
            break;
        case 'layerPodlogi':
            layerPodlogi.setVisible(false);
            break;
            
            
            
    }
}

for (var layer in infrastrukturalayersToAdd) {
    switch (infrastrukturalayersToAdd[layer]) {
        case 'layerTrotoari':
            infrastrukturaGroupLayers.push(layerTrotoari);
            break;
        case 'layerVelosipedskaPateka':
            infrastrukturaGroupLayers.push(layerVelosipedskaPateka);
            break;
        case 'layerZelenilo':
            infrastrukturaGroupLayers.push(layerZelenilo);
            break;
        case 'layerAtmKanPl':
            infrastrukturaGroupLayers.push(layerAtmKanPl);
            break;
        case 'layerAtmKanPo':
            infrastrukturaGroupLayers.push(layerAtmKanPo);
            break;
        case 'layerVodovodnaPl':
            infrastrukturaGroupLayers.push(layerVodovodnaPl);
            break;

        case 'layerVodovodnaPo':
            infrastrukturaGroupLayers.push(layerVodovodnaPo);
            break;

        case 'layerGasovodnaPo':
            infrastrukturaGroupLayers.push(layerGasovodnaPo);
            break;
        case 'layerGasovodnaPl':
            infrastrukturaGroupLayers.push(layerGasovodnaPl);
            break;
        case 'layerTelekomunikaciskaPo':
            infrastrukturaGroupLayers.push(layerTelekomunikaciskaPo);
            break;
        case 'layerTelekomunikaciskaPl':
            infrastrukturaGroupLayers.push(layerTelekomunikaciskaPl);
            break;
        case 'layerElektrikaPo':
            infrastrukturaGroupLayers.push(layerElektrikaPo);
            break;
        case 'layerElektrikaPl':
            infrastrukturaGroupLayers.push(layerElektrikaPl);
            break;

        case 'layerFekalnaPo':
            infrastrukturaGroupLayers.push(layerFekalnaPo);
            break;
        case 'layerFekalnaPl':
            infrastrukturaGroupLayers.push(layerFekalnaPl);
            break;
        case 'layerToplifikacijaPo':
            infrastrukturaGroupLayers.push(layerToplifikacijaPo);
            break;

        case 'layerToplifikacijaPl':
            infrastrukturaGroupLayers.push(layerToplifikacijaPl);
            break;
        case 'layerPesackaPateka':
            infrastrukturaGroupLayers.push(layerPesackaPateka);
            break;
        case 'layerParking':
            infrastrukturaGroupLayers.push(layerParking);
            break;




    }
}
for (var layer in infrastrukturalayersOff) {
    switch (infrastrukturalayersOff[layer]) {
        case 'layerTrotoari':
            layerTrotoari.setVisible(false);
            break;
        case 'layerVelosipedskaPateka':
            layerVelosipedskaPateka.setVisible(false);
            break;
        case 'layerZelenilo':
            layerZelenilo.setVisible(false);
            break;
        case 'layerAtmKanPl':
            layerAtmKanPl.setVisible(false);
            break;
        case 'layerAtmKanPo':
            layerAtmKanPo.setVisible(false);
            break;
        case 'layerVodovodnaPl':
            layerVodovodnaPl.setVisible(false);
            break;
        case 'layerVodovodnaPo':
            layerVodovodnaPo.setVisible(false);
            break;
        case 'layerGasovodnaPo':
            layerGasovodnaPo.setVisible(false);
            break;
        case 'layerGasovodnaPl':
            layerGasovodnaPl.setVisible(false);
            break;
        case 'layerTelekomunikaciskaPo':
            layerTelekomunikaciskaPo.setVisible(false);
            break;
        case 'layerTelekomunikaciskaPl':
            layerTelekomunikaciskaPl.setVisible(false);
            break;
        case 'layerElektrikaPo':
            layerElektrikaPo.setVisible(false);
            break;
        case 'layerElektrikaPl':
            layerElektrikaPl.setVisible(false);
            break;
        case 'layerFekalnaPo':
            layerFekalnaPo.setVisible(false);
            break;
        case 'layerFekalnaPl':
            layerFekalnaPl.setVisible(false);
            break;
        case 'layerToplifikacijaPo':
            layerToplifikacijaPo.setVisible(false);
            break;
        case 'layerToplifikacijaPl':
            layerToplifikacijaPl.setVisible(false);
            break;
        case 'layerPesackaPateka':
            layerPesackaPateka.setVisible(false);
            break;
        case 'layerParking':
            layerParking.setVisible(false);
            break;


    }
}


if (infrastrukturaGroupLayers.getLength() > 0)
    layers.push(infrastrukturaGroup);

ol.layer.Layer.prototype.disableInSwitcher = function (disable) {
    var layerName = this.values_.title;
    if (disable) {
        if (!this.disabledInSwitcher) {
            map.getLayers().forEach(function (layer) {
                if (layer.values_.title == layerName) {
                    $('label').each(function () {
                        if ($(this).html() == layerName) {
                            var id = '#' + $(this).attr('for');
                            $(id).attr('disabled', true);
                        }
                    });
                }
            });
            this.disabledInSwitcher = true;
        }
    } else {
        if (this.disabledInSwitcher) {
            map.getLayers().forEach(function (layer) {
                if (layer.values_.title == layerName) {
                    $('label').each(function () {
                        if ($(this).html() == layerName) {
                            var id = '#' + $(this).attr('for');
                            $(id).attr('disabled', false);
                        }
                    });
                }
            });
            this.disabledInSwitcher = false;
        }
    }
};
// CREATE THE MAP
var map = new ol.Map({
    layers: layers,
    loadTilesWhileAnimating: true,
    loadTilesWhileInteracting: true,
    target: 'mapG',
    controls: ol.control.defaults()
    .extend([mousePositionControl, scaleLineControl, keyMap]),
    view: new ol.View({
        projection: projection,
        center: [centerX, centerY],
        zoom: minZoom,
        maxZoom: maxZoom,
        minZoom: minZoom,
        resolutions: resolutions
    })
});

var layerSwitcher = new ol.control.LayerSwitcher({
    tipLabel: global.resources.Legenda // Optional label for button
});

map.addControl(layerSwitcher);
$('.ol-attribution').remove();

var newExtent;
var lastExtent;
var newZoomLevel;
var lastZoomLevel;
function onMoveEnd(evt) {
    var map = evt.map;
    lastExtent = newExtent;
    lastZoomLevel = newZoomLevel;
    newExtent = map.getView().calculateExtent(map.getSize());
    newZoomLevel = map.getView().getZoom();
    $("#razmer").text("1 : " + Math.round(getScaleFromResolution(map.getView().getResolution())));
}

function handleZoom() {
    //disable katastarski parceli
    if (Math.round(getScaleFromResolution(map.getView().getResolution())) > 4000) {
        layerKP.disableInSwitcher(true);
        layerKZ.disableInSwitcher(true);
        layerSintezen.disableInSwitcher(true);
        layerSoobrakaen.disableInSwitcher(true);
        layerInfrastrukturen.disableInSwitcher(true);
        layerSoobrakajnici.disableInSwitcher(true);
        layerSoobrakajniciCache.disableInSwitcher(true);
        layerInfrastrukturniProekti.disableInSwitcher(true);
        layerPodlogi.disableInSwitcher(true);
 
    } else {
        layerKP.disableInSwitcher(false);
        layerKZ.disableInSwitcher(false);
        layerSintezen.disableInSwitcher(false);
        layerSoobrakaen.disableInSwitcher(false);
        layerInfrastrukturen.disableInSwitcher(false);
        layerSoobrakajnici.disableInSwitcher(false);
        layerSoobrakajniciCache.disableInSwitcher(false);
        layerInfrastrukturniProekti.disableInSwitcher(false);
        layerPodlogi.disableInSwitcher(false);
        
    }
}

map.on('moveend', onMoveEnd);
map.on('moveend', handleZoom);

function goToLastExtent() {
    if (typeof lastExtent !== 'undefined') {
        map.getView().fit(lastExtent, map.getSize());
        map.getView().setZoom(lastZoomLevel);
    }
}

function goToExtent(a, b, c, d) {
    map.getView().fit([a, b, c, d], map.getSize());
}

var myStyle = new ol.style.Style({
    stroke: new ol.style.Stroke({
        color: 'blue',
        lineDash: [4],
        width: 3
    }),
    fill: new ol.style.Fill({
        color: 'rgba(0, 0, 255, 0.1)'
    })
});
var myStyleKP = new ol.style.Style({
    image: new ol.style.Circle({
        radius: 7,
        fill: new ol.style.Fill({
            color: 'black'
        })
    })

});
function goToGeojsonExtent2(id) {
    var data = $('#dataSearch').data(id);
    vectorSource.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSource.addFeature(myFeature);
    map.getView().fit(vectorSource.getExtent(), map.getSize());
}

function goToGeojsonExtent() {
    var data = $('#data').data();
    vectorSource.clear();
    $.each(data, function (i, obj) {
        var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + obj + "}");
        vectorSource.addFeature(myFeature);
    });
    map.getView().fit(vectorSource.getExtent(), map.getSize());
}

function goToGeojsonExtent4(id) {
    var data = $('#dataSearchK').data(id);
    vectorSource.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSource.addFeature(myFeature);
    map.getView().fit(vectorSource.getExtent(), map.getSize());
}
function goToGeojsonExtent5(id) {
    var data = $('#dataSearchA').data(id);
    vectorSourceKP.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSourceKP.addFeature(myFeature);
    map.getView().fit(vectorSourceKP.getExtent(), map.getSize());
}
function goToGeojsonExtent6(id) {
    var data = $('#dataSearchGP').data(id);
    vectorSource.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSource.addFeature(myFeature);
    map.getView().fit(vectorSource.getExtent(), map.getSize());
}
function goToGeojsonExtentKParceli(id) {
    //var data = $('#dataSearchKParceli').data(id);
    //console.log(data);
    //vectorSourceKP.clear();
    //var myFeatures = (new ol.format.GeoJSON()).readFeatures("{ \"type\": \"FeatureCollection\", \"features\": [{ \"type\": \"Feature\",\"geometry\":" + data + ", \"properties\":null }]}");
    //vectorSourceKP.addFeatures(myFeatures);
    //map.getView().fit(vectorSourceKP.getExtent(), map.getSize());
    var data = $('#dataSearchKParceli').data(id);
    vectorSourceKP.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSourceKP.addFeature(myFeature);
    map.getView().fit(vectorSourceKP.getExtent(), map.getSize());
}

//function goToGeojsonExtent10(id) {
//    var data = $('#dataSearch').data(id);
//    console.log(data);
//    vectorSource.clear();
//    var myFeatures = (new ol.format.GeoJSON()).readFeatures("{ \"type\": \"FeatureCollection\", \"features\": [{ \"type\": \"Feature\",\"geometry\":" + data + ", \"properties\":null }]}");
//    vectorSource.addFeatures(myFeatures);
//    map.getView().fit(vectorSource.getExtent(), map.getSize());
//}

function restrictExtent(bbox) {
    for (index = layers.length - 1; index >= 0; --index) { layers[index].setExtent(bbox); };
};

function getResolutionFromScale(scale) {
    var units = map.getView().getProjection().getUnits();
    var dpi = 25.4 / 0.28;
    var mpu = ol.proj.METERS_PER_UNIT[units];
    var resolution = scale / (mpu * 39.37 * dpi);
    return resolution;
}

function getScaleFromResolution(resolution) {
    var units = map.getView().getProjection().getUnits();
    var dpi = 25.4 / 0.28;
    var mpu = ol.proj.METERS_PER_UNIT[units];
    var scale = resolution * (mpu * 39.37 * dpi);
    return scale;
}