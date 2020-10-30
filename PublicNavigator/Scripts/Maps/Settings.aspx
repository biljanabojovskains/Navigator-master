<%@ Page Language="C#" AutoEventWireup="false"  ContentType="text/javascript" %>
<%@ Import Namespace="System.Globalization" %>
var server = '<%=ConfigurationManager.AppSettings["server"] %>';
var resolutions = <%=ConfigurationManager.AppSettings["resolutions"] %>;
var port = <%=ConfigurationManager.AppSettings["port"] %>;
var serverCache = '<%=ConfigurationManager.AppSettings["server_cache"] %>';
var portCache = <%=ConfigurationManager.AppSettings["port_cache"] %>;
var ms4wApp = '<%=ConfigurationManager.AppSettings["ms4w_app"] %>';
var minX = <%= double.Parse(ConfigurationManager.AppSettings["minX"], new CultureInfo("en-US")) %>;
var minY = <%=double.Parse(ConfigurationManager.AppSettings["minY"], new CultureInfo("en-US")) %>;
var maxX = <%=double.Parse(ConfigurationManager.AppSettings["maxX"], new CultureInfo("en-US")) %>;
var maxY = <%=double.Parse(ConfigurationManager.AppSettings["maxY"], new CultureInfo("en-US")) %>;
var centerX = <%=double.Parse(ConfigurationManager.AppSettings["centerX"], new CultureInfo("en-US")) %>;
var centerY = <%=double.Parse(ConfigurationManager.AppSettings["centerY"], new CultureInfo("en-US")) %>;
var maxZoom = <%=int.Parse(ConfigurationManager.AppSettings["maxZoom"]) %>;
var minZoom = <%=int.Parse(ConfigurationManager.AppSettings["minZoom"]) %>;
var generalLayers = '<%=ConfigurationManager.AppSettings["generalLayers"] %>';
var generalInactive = '<%=ConfigurationManager.AppSettings["generalInactive"] %>';
var notifikaciiLayers = '<%=ConfigurationManager.AppSettings["notifikaciiLayers"] %>';
var notifikaciiInactive = '<%=ConfigurationManager.AppSettings["notifikaciiInactive"] %>';