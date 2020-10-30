function get(name) {
    if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
        return decodeURIComponent(name[1]);
}

// DEFINE THE PROJECTION AS OPENLAYERS IS SILLY LIKE THAT
proj4.defs("EPSG:6316", "+proj=tmerc +lat_0=0 +lon_0=21 +k=0.9999 +x_0=7500000 +y_0=0 +ellps=bessel +towgs84=551.7,162.9,467.9,6.04,1.96,-11.38,-4.82 +units=m +no_defs");
var projection = ol.proj.get('EPSG:6316');

var segment_str = window.location.pathname;
var segment_array = segment_str.split('/');
var last_segment = segment_array.pop();

var myTileGrid = new ol.tilegrid.TileGrid({
    extent: [minX, minY, maxX, maxY],
    //resolutions: [280.0005600011201, 140.00028000056005, 70.00014000028003, 35.00007000014001, 17.920035840071687, 8.960017920035844, 4.480008960017922, 2.240004480008961, 1.1200022400044805, 0.5600011200022402, 0.2800005600011201, 0.14000028000056006] - site
    resolutions: resolutions
});
var layerOSM = new ol.layer.Tile({
    projection: 'EPSG:6316',
    title: "Подлога",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/osm.map',
        //ova e za kavadarci
        //url: 'http://178.77.71.87:8888/service?',
        params: {
            'LAYERS': 'Osm',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGranica = new ol.layer.Tile({
    projection: projection,
    title: "Граница",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/opstina_granica.map',
        params: {
            'LAYERS': 'granica',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOsmLocal = new ol.layer.Tile({
    projection: projection,
    title: "OSM",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/osm/basemaps/osm-google.map',
        params: {
            'LAYERS': 'default',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSatelitska = new ol.layer.Tile({
    projection: projection,
    title: "Сателитска",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/satelitska.map',
        params: {
            'LAYERS': 'satelitska',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSatelitskaCache = new ol.layer.Tile({
    projection: projection,
    title: "Сателитска",
    source: new ol.source.TileWMS({
        url: 'http://' + serverCache + ':' + portCache + '/service?',
        params: {
            'LAYERS': 'Satelitska',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOsmLocalCache = new ol.layer.Tile({
    projection: projection,
    title: "OSM",
    source: new ol.source.TileWMS({
        url: 'http://' + serverCache + ':' + portCache + '/service?',
        params: {
            'LAYERS': 'default',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSintezenCache = new ol.layer.Tile({
    projection: projection,
    title: "Синтезен",
    source: new ol.source.TileWMS({
        url: 'http://' + serverCache + ':' + portCache + '/service?',
        params: {
            'LAYERS': 'Sintezen',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSintezen = new ol.layer.Tile({
    projection: projection,
    title: "Синтезен",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/sintezen.map',
        params: {
            'LAYERS': 'sintezen',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSoobrakaen = new ol.layer.Tile({
    projection: projection,
    title: "Сообраќаен",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/soobrakaen.map',
        params: {
            'LAYERS': 'soobrakaen',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerInfrastrukturen = new ol.layer.Tile({
    projection: projection,
    title: "Инфраструктурен",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastrukturen.map',
        params: {
            'LAYERS': 'infrastrukturen',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerGupGc = new ol.layer.Tile({
    projection: projection,
    title: "ГУП со градски четврти",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/makro_tekovni.map',
        params: {
            'LAYERS': 'makro',
            'VERSION': '1.3.0',
            'ID': 1,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGup = new ol.layer.Tile({
    projection: projection,
    title: "ГУП",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/makro_tekovni.map',
        params: {
            'LAYERS': 'makro',
            'VERSION': '1.3.0',
            'ID': 2,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOpstAkt = new ol.layer.Tile({
    projection: projection,
    title: "Општ акт",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/makro_tekovni.map',
        params: {
            'LAYERS': 'makro',
            'VERSION': '1.3.0',
            'ID': 3,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerDup = new ol.layer.Tile({
    projection: projection,
    title: "ДУП",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 4,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerLupd = new ol.layer.Tile({
    projection: projection,
    title: "ЛУПД",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 5,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerUps = new ol.layer.Tile({
    projection: projection,
    title: "УПВНМ/УПС",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 6,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerAup = new ol.layer.Tile({
    projection: projection,
    title: "АУП",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 7,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerInfra = new ol.layer.Tile({
    projection: projection,
    title: "Инфраструктурен проект",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 8,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerDupd = new ol.layer.Tile({
    projection: projection,
    title: "ДУПД/УПД",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 9,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

//idni
var layerDupIdni = new ol.layer.Tile({
    projection: projection,
    title: "ДУП во фаза на донесување",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_idni.map',
        params: {
            'LAYERS': 'mikro',
            'VERSION': '1.3.0',
            'ID': 4,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerKP = new ol.layer.Tile({
    projection: projection,
    title: "Катастарски парцели",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/pz.map',
        params: {
            'LAYERS': 'parceli',
            'VERSION': '1.1.1',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerKZ = new ol.layer.Tile({
    projection: projection,
    title: "Катастарски објекти",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/pz.map',
        params: {
            'LAYERS': 'zgradi',
            'VERSION': '1.1.1',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerKO = new ol.layer.Tile({
    projection: projection,
    title: "Катастарски општини",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/goce.map',
        params: {
            'LAYERS': 'goce2',
            'VERSION': '1.1.1',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerWmsOrtofoto = new ol.layer.Tile({
    projection: projection,
    title: "Ортофото",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/goce.map',
        params: {
            'LAYERS': 'ortofoto',
            'VERSION': '1.1.1',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOrtofotoCache = new ol.layer.Tile({
    projection: projection,
    title: "Ортофото",
    source: new ol.source.TileWMS({
        url: 'http://' + serverCache + ':' + portCache + '/service?',
        params: {
            'LAYERS': 'Ortofoto',
            'VERSION': '1.1.1',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
//poi
var layerPOI = new ol.layer.Tile({
    projection: projection,
    title: "Точки од интерес",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/poi_aerodrom.map',
        params: {
            'LAYERS': 'poi',
            'VERSION': '1.3.0',
            'ID': 10,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerBiznisPOI = new ol.layer.Tile({
    projection: projection,
    title: "Бизнис точки",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/biznis_poi_aerodrom.map',
        params: {
            'LAYERS': 'biznispoi',
            'VERSION': '1.3.0',
            'ID': 11,
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerRPTransparent = new ol.layer.Tile({
    projection: projection,
    title: "Регулациски планови - транспарентни",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/rp_transparent.map',
        params: {
            'LAYERS': 'transparent',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

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
var vectorSource = new ol.source.Vector({

});
var searchLayer = new ol.layer.Vector({
    style: myStyle,
    source: vectorSource,
    projection: projection
    //title: "Пребарување"
});

var jsonFileName = get("geoJson");
var vectorSource2 = new ol.source.Vector({
    url: '../../jsonuploads/' + jsonFileName,
    format: new ol.format.GeoJSON({

        defaultDataProjection: projection,
        projection: projection

    })
});
var errorLayer = new ol.layer.Vector({
    title: "Погрешен податок",
    source: vectorSource2,
    style: myStyle
});

var source = new ol.source.Vector();

var vectorMeasure = new ol.layer.Vector({
    source: source,
    style: new ol.style.Style({
        fill: new ol.style.Fill({
            color: 'rgba(255, 255, 255, 0.2)'
        }),
        stroke: new ol.style.Stroke({
            color: '#ffcc33',
            width: 2
        }),
        image: new ol.style.Circle({
            radius: 7,
            fill: new ol.style.Fill({
                color: '#ffcc33'
            })
        })
    })
});

function clearSourceMeasure() {
    source.clear();
}

//var layerIgea = new ol.layer.Tile({
//    extent: [7454799.05, 4523470.28, 7669725.37, 4693022.89],
//    projection: projection2,
//    title: "WMS Parceli",
//    source: new ol.source.TileWMS({
//        url: 'http://10.177.162.73/ows-m/wms',
//        params: {
//            'sessionId': '3OI1VD31P18TJ7JZ8YF2AESO3RETGM',
//            'LAYERS': 'arec_table:KIS.CD_PARPARTS',
//            'STYLES': 'ekat_kis_parcels',
//            'VERSION': '1.1.1',
//            'TRANSPARENT': 'TRUE',
//            'FORMAT': 'image/png',
//            'REQUEST': 'GetMap'
//        },
//        serverType: 'geoserver'
//    })
//});

//var layerKatastar = new ol.layer.Tile({
//    extent: [7454799.05, 4523470.28, 7669725.37, 4693022.89],
//    projection: projection,
//    title: "Катастар",
//    source: new ol.source.TileWMS({
//        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/kv/htdocs/kv.map',
//        params: {
//            'LAYERS': 'KV_merge,Raster_Pribacevo,Raster_Bucin,Krusevo_full,7i10_2,7i10_3,7i11_92',
//            'VERSION': '1.3.0',
//            'FORMAT': 'image/png',
//            'TILED': true
//        },
//        serverType: 'mapserver'
//    })
//});

//var layerSintezen = new ol.layer.Tile({
//    extent: [7454799.05, 4523470.28, 7669725.37, 4693022.89],
//    projection: projection,
//    title: "Синтезен",
//    source: new ol.source.TileWMS({
//        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/kv/htdocs/jaka.map',
//        params: {
//            'LAYERS': 'Jaka',
//            'VERSION': '1.3.0',
//            'FORMAT': 'image/png',
//            'TILED': true
//        },
//        serverType: 'mapserver'
//    })
//});


var sourceDraw = new ol.source.Vector({ wrapX: false });

var vectorDraw = new ol.layer.Vector({
    source: sourceDraw
});

function clearSourceDraw() {
    sourceDraw.clear();
}
