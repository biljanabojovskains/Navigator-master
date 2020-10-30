<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="Navigator.Export" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
     <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery.fileDownload/1.4.2/jquery.fileDownload.min.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol-debug.js"></script>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ol3/3.19.1/ol.css" type="text/css">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/proj4js/2.3.15/proj4.js"></script>
</head>
<body style="background-color: white;">
    <form id="form1" runat="server">
        <div>
            <div id="title-div" style="text-align: center; font-weight: bold; font-size: xx-large; padding-top: 30px">
                <label runat="server" id="Label2"></label>
                <h1 align="center">Зелен катастар - извештај за  <%=tipIme%></h1>
            </div>
            <div id="mapExport" style="position: relative;">
            </div>
            <div>
                <%--<label runat="server" style="font-size: large" id="Label1">wertt</label>
                <label runat="server" style="margin-left: 50px; font-size: large" id="Label2">t4t4</label>--%>
           

                <h3>Вкупен број : <%=broj%></h3><br/>
                 <h3>Број на зимзелени : <%=zimzeleni%></h3><br/>
                 <h3> Број на листопадни : <%=listopadni%></h3><br/>
                 <h3>Број на болни : <%=bolni%></h3><br/>
                 <h3>Број на здрави : <%=zdravi%></h3><br/><br/>

            </div>
            <div>
             <h4>Датум :<%=dateTime%></h4>
            </div>
        </div>
      
        <script type="text/javascript">
         
          
        </script>
    </form>
</body>
</html>
