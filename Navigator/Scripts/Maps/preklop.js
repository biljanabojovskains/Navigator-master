var scaleLineControl = new ol.control.ScaleLine({
    geodesic: true
});
var keyMap = new ol.control.OverviewMap();

scaleLineControl.setUnits('metric');
// LIST THE LAYERS THAT NEED TO BE DRAWN
var layersToAdd = preklopLayers.split(',');
var layersOff = preklopInactive.split(',');
var layers = [];
for (var layer in layersToAdd) {
    switch (layersToAdd[layer]) {
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
        case 'layerKZ':
            layers.push(layerKZ);
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
    }
}

for (var layer in layersOff) {
    switch (layersOff[layer]) {
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
        case 'layerKZ':
            layerKZ.setVisible(false);
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
        
            
    }
}

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
    target: 'map',
    controls: ol.control.defaults()
    .extend([scaleLineControl, keyMap]),
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
    tipLabel: global.resources.Legenda// Optional label for button
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
    } else {
        layerKP.disableInSwitcher(false);
        layerKZ.disableInSwitcher(false);
        layerSintezen.disableInSwitcher(false);
        layerSoobrakaen.disableInSwitcher(false);
        layerInfrastrukturen.disableInSwitcher(false);
    }
}

map.on('moveend', onMoveEnd);
map.on('moveend', handleZoom);

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
    var data = $('#data').data(id);
    vectorSource.clear();
    var myFeature = (new ol.format.GeoJSON()).readFeature("{ \"type\": \"Feature\",\"geometry\":" + data + "}");
    vectorSource.addFeature(myFeature);
    map.getView().fit(vectorSource.getExtent(), map.getSize());
}

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