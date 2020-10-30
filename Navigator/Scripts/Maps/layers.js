function get(name) {
    if (name = (new RegExp('[?&]' + encodeURIComponent(name) + '=([^&]*)')).exec(location.search))
        return decodeURIComponent(name[1]);
}

// DEFINE THE PROJECTION AS OPENLAYERS IS SILLY LIKE THAT
proj4.defs("EPSG:6316", "+proj=tmerc +lat_0=0 +lon_0=21 +k=0.9999 +x_0=7500000 +y_0=0 +ellps=bessel +towgs84=551.7,162.9,467.9,6.04,1.96,-11.38,-4.82 +units=m +no_defs");
var projection = ol.proj.get('EPSG:6316');
var projection3857 = ol.proj.get('EPSG:3857');

var segment_str = window.location.pathname;
var segment_array = segment_str.split('/');
var last_segment = segment_array.pop();

var myTileGrid = new ol.tilegrid.TileGrid({
    extent: [minX, minY, maxX, maxY],
    //resolutions: [280.0005600011201, 140.00028000056005, 70.00014000028003, 35.00007000014001, 17.920035840071687, 8.960017920035844, 4.480008960017922, 2.240004480008961, 1.1200022400044805, 0.5600011200022402, 0.2800005600011201, 0.14000028000056006] - site
    resolutions: resolutions
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
var layerRPBeli = new ol.layer.Tile({
    projection: projection,
    title: "Регулациски планови - бели",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/rp_beli.map',
        params: {
            'LAYERS': 'beli',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerUrbanaOprema = new ol.layer.Tile({
projection: projection,
    title: 'Урбана опрема',
source: new ol.source.TileWMS({
    url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
    params: {
        'LAYERS': 'UrbanaOprema',
        'VERSION': '1.3.0',
        'FORMAT': 'image/png'
    },
    serverType: 'mapserver',
    tileGrid: myTileGrid
})
});
var layerVodovod = new ol.layer.Tile({
    projection: projection,
    title: 'Водовод',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
        params: {
            'LAYERS': 'Vodovod',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerDefekt = new ol.layer.Tile({
    projection: projection,
    title: 'Дефект',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
        params: {
            'LAYERS': 'Defekt',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerJavnaPovrsina = new ol.layer.Tile({
    projection: projection,
    title: 'Јавна површина',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
        params: {
            'LAYERS': 'javna_povrsina',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOprema = new ol.layer.Tile({
    projection: projection,
    title: 'Опрема',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
        params: {
            'LAYERS': 'oprema',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOsvetluvanje = new ol.layer.Tile({
    projection: projection,
    title: 'Осветлување',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbanaoprema.map',
        params: {
            'LAYERS': 'osvetluvanje',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var urlTemplate = 'https://services.arcgisonline.com/arcgis/rest/services/ESRI_Imagery_World_2D/MapServer/tile/{z}/{y}/{x}';
var layerWorldStreetMap = new ol.layer.Tile({
    title: 'World Street Map',
    projection: 'EPSG:4326',
    source: new ol.source.XYZ({
        attributions: 'Tiles © <a href="https://services.arcgisonline.com/ArcGIS/' +
            'rest/services/World_Street_Map/MapServer">ArcGIS</a>',
        url: 'https://server.arcgisonline.com/ArcGIS/rest/services/' +
            'World_Street_Map/MapServer/tile/{z}/{y}/{x}'
    })
});
var layerWorldTopoMap = new ol.layer.Tile({
    title: 'World Topo Map',
    projection: 'EPSG:4326',
    source: new ol.source.XYZ({
        attributions: 'Tiles © <a href="https://services.arcgisonline.com/ArcGIS/' +
            'rest/services/World_Topo_Map/MapServer">ArcGIS</a>',
        url: 'https://server.arcgisonline.com/ArcGIS/rest/services/' +
            'World_Topo_Map/MapServer/tile/{z}/{y}/{x}'
    })
});
var layerWorldImagery = new ol.layer.Tile({
    title: 'World Imagery',
    projection: 'EPSG:4326',
    source: new ol.source.XYZ({
        attributions: 'Tiles © <a href="https://services.arcgisonline.com/ArcGIS/' +
            'rest/services/World_Imagery/MapServer">ArcGIS</a>',
        url: 'https://server.arcgisonline.com/ArcGIS/rest/services/' +
            'World_Imagery/MapServer/tile/{z}/{y}/{x}'
    })
}); 


var layerOrtofotoCache = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Ortofoto,
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
var layerSatelitska = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Satelitska,
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/satelitska.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Satelitska',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSatelitskaCache = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Satelitska,
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
var layerSintezen = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Sintezen,
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/sintezen.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Sintezen',
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
var layerSintezenCache = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Sintezen,
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
var layerSoobrakajniciCache = new ol.layer.Tile({
    projection: projection,
    title: "Сообраќајници",
    source: new ol.source.TileWMS({
        url: 'http://' + serverCache + ':' + portCache + '/service?',
        params: {
            'LAYERS': 'Soobrakajnici',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSoobrakajnici = new ol.layer.Tile({
    projection: projection,
    title: "Сообраќајници",
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/soobrakajnici.map',
        params: {
            'LAYERS': 'Soobrakajnici',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerSoobrakaen = new ol.layer.Tile({
    projection: projection,
    title: global.resources.Soobrakaen,
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
    title: global.resources.Infrastrukturen,
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
    title: global.resources.GUPGradskiCetvrt,
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
    title: global.resources.GUP,
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
var layerUlici = new ol.layer.Tile({
    projection: projection,
    title: 'Улици',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/ulici.map',
        params: {
            'LAYERS': 'ulici',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerOpstAkt = new ol.layer.Tile({
    projection: projection,
    title: global.resources.OpstAkt,
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
    title: global.resources.Dup,
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
var layerDupOld = new ol.layer.Tile({
    projection: projection,
    title: 'ДУП СТАРО',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni_old.map',
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
    title: global.resources.Lupd,
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
var layerLupd_old = new ol.layer.Tile({
    projection: projection,
    title: 'ЛУПД СТАРО',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni_old.map',
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
    title: global.resources.UpvnmUps,
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
var layerUps_old = new ol.layer.Tile({
    projection: projection,
    title: 'УПВНМ СТАРО',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni_old.map',
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
    title: global.resources.Aup,
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
    title: global.resources.InfrastrukturenProekt,
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
    title: global.resources.DupdUpd,
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
var layerDupd_old = new ol.layer.Tile({
    projection: projection,
    title: 'ДУПД СТАРО',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/mikro_tekovni_old.map',
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
    title: global.resources.DupFazaNaDonesuvanje,
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
    title: global.resources.KatParceli,
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
    title: global.resources.KatObjekti,
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
    title: global.resources.KatOpstini,
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
    title: global.resources.Ortofoto,
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
        })})

});
var layerOdobrenia = new ol.layer.Tile({
    projection: projection,
    title: 'Одобренија',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/odobrenia.map',
        params: {
            'LAYERS': 'odobrenia',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerElektroenergetski = new ol.layer.Tile({
    projection: projection,
    title: 'Електроенергетски',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/elektroenergetski.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Elektoroenegetski',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerHidrotehnicki = new ol.layer.Tile({
    projection: projection,
    title: 'Хидротехнички',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/hidrotehnicki.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Hidrotehnicki',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerVodovoden = new ol.layer.Tile({
    projection: projection,
    title: 'Водоводен',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/vodovoden.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Vodovoden',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGasovoden = new ol.layer.Tile({
    projection: projection,
    title: 'Гасоводен',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/gasovoden.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Gasovoden',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerFekalen = new ol.layer.Tile({
    projection: projection,
    title: 'Фекален',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/fekalen_kanalizaciski.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Fekalen_kalizaciski',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerTelefonskaMreza = new ol.layer.Tile({
    projection: projection,
    title: 'Телефонска мрежа',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/telefonska_mreza.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'Telefonska_mreza',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGUPSintezen = new ol.layer.Tile({
    projection: projection,
    title: 'ГУП синтезен',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/gup.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'gup',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerZastiteniPodracja = new ol.layer.Tile({
    projection: projection,
    title: 'Заштитени подрачја',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/zastiteni_podracja.map',
        //url: 'http://localhost:8888/service?',
        params: {
            'LAYERS': 'zastiteni_podracja',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerLegalizacijaVoTek = new ol.layer.Tile({
    projection: projection,
    title: 'Градби во тек',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/legalizacija.map',
        params: {
            'LAYERS': 'gradbitek',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerLegalizacijaZavrsena = new ol.layer.Tile({
    projection: projection,
    title: 'Градби завршени',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/legalizacija.map',
        params: {
            'LAYERS': 'gradbizavrseni',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerAdresi = new ol.layer.Tile({
    projection: projection,
    title: 'Адреси',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/adresi.map',
        params: {
            'LAYERS': 'adresi',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerOsnovniUcilista = new ol.layer.Tile({
    projection: projection,
    title: 'Основни училишта',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/osnovni_ucilista.map',
        params: {
            'LAYERS': 'osnovni_ucilista',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerJavniPovrsini = new ol.layer.Tile({
    projection: projection,
    title: 'Јавни површини',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/javni_povrsini.map',
        params: {
            'LAYERS': 'javni_povrsini',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerTrotoari = new ol.layer.Tile({
    projection: projection,
    title: 'Тротоари',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/trotoari.map',
        params: {
            'LAYERS': 'trotoari',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerUrbanaOprema = new ol.layer.Tile({
    projection: projection,
    title: 'Урбана опрема',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/urbana_oprema.map',
        params: {
            'LAYERS': 'urbana_oprema',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerErozivniPodracja = new ol.layer.Tile({
    projection: projection,
    title: 'Ерозивни подрачја',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/erozivni_podracja.map',
        params: {
            'LAYERS': 'erozivni_podracja',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerZagrozeniErozija = new ol.layer.Tile({
    projection: projection,
    title: 'Подрачја загрозени од ерозија',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/erozivni_podracja.map',
        params: {
            'LAYERS': 'zagrozeni_erozivni_podracja',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerSegmentiUlici = new ol.layer.Tile({
    projection: projection,
    title: 'Сегменти од улица',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/segmenti_ulici.map',
        params: {
            'LAYERS': 'segmenti_ulici',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerInfrastrukturniProekti = new ol.layer.Tile({
    projection: projection,
    title: 'Проекти за инфраструктура',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/soobrakajnici.map',
        params: {
            'LAYERS': 'Soobrakajnici',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
//InfrastrukturaAerodrom
var layerTrotoari = new ol.layer.Tile({
    projection: projection,
    title: 'Тротоари',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'trotoari',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerVelosipedskaPateka = new ol.layer.Tile({
    projection: projection,
    title: 'Велосипедска патека',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'velosipedska_pateka',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerZelenilo = new ol.layer.Tile({
    projection: projection,
    title: 'Зеленило',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'zelenilo',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerAtmKanPl = new ol.layer.Tile({
    projection: projection,
    title: 'Атмосферска канализација планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'atmosferska_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerAtmKanPo = new ol.layer.Tile({
    projection: projection,
    title: 'Атмосферска канализација постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'atmosferska_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerVodovodnaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Водоводна инфраструктура планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'vodovodna_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerVodovodnaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Водоводна инфраструктура постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'vodovodna_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGasovodnaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Гасоводна инфраструктура постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'gasovodna_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerGasovodnaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Гасоводна инфраструктура планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'gasovodna_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerTelekomunikaciskaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Телекомуникациска инфраструктура постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'telekomunikaciska_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerTelekomunikaciskaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Телекомуникациска инфраструктура планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'telekomunikaciska_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerElektrikaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Електрика постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'elektrika_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerElektrikaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Електрика планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'elektrika_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerFekalnaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Фекална инфраструктура постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'fekalna_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerFekalnaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Фекална инфраструктура планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'fekalna_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerToplifikacijaPo = new ol.layer.Tile({
    projection: projection,
    title: 'Топлификациска инфраструктура постојна',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'toplifikacija_postojna',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerToplifikacijaPl = new ol.layer.Tile({
    projection: projection,
    title: 'Топлификациска инфраструктура планирана',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'toplifikacija_planirana',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerPesackaPateka= new ol.layer.Tile({
    projection: projection,
    title: 'Пешачка патека',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'pesacka_pateka',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerParking = new ol.layer.Tile({
    projection: projection,
    title: 'Паркинг',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/infrastruktura.map',
        params: {
            'LAYERS': 'parking',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerZelenKatastar = new ol.layer.Tile({
    projection: projection,
    title: 'Зеленило',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/zelen_katastar.map',
        params: {
            'LAYERS': 'zelenilo',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerDrva = new ol.layer.Tile({
    projection: projection,
    title: 'Дрва и грмушки',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/zelen_katastar.map',
        params: {
            'LAYERS': 'drva',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var layerCvekje = new ol.layer.Tile({
    projection: projection,
    title: 'Цвеќиња',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/zelen_katastar.map',
        params: {
            'LAYERS': 'cvekje',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerZeleniPovrsini = new ol.layer.Tile({
    projection: projection,
    title: 'Зелени површини',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/zelen_katastar.map',
        params: {
            'LAYERS': 'zeleni_povrsini',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});
var layerPodlogi = new ol.layer.Tile({
    projection: projection,
    title: 'Подлоги',
    source: new ol.source.TileWMS({
        url: 'http://' + server + ':' + port + '/cgi-bin/mapserv.exe?map=../../apps/' + ms4wApp + '/htdocs/podlogi.map',
        params: {
            'LAYERS': 'podlogi',
            'VERSION': '1.3.0',
            'FORMAT': 'image/png'
        },
        serverType: 'mapserver',
        tileGrid: myTileGrid
    })
});

var vectorSource = new ol.source.Vector({
});
var vectorSourceKP = new ol.source.Vector({
});
var searchLayer = new ol.layer.Vector({
    style: myStyle,
    source: vectorSource,
    projection: projection
    //title: "Пребарување"
});
var searchLayerKP = new ol.layer.Vector({
    style: myStyleKP,
    source: vectorSourceKP,
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
    title: global.resources.PogresenPodatok,
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
