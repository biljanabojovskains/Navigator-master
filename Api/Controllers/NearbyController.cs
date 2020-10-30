using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Api.Bll.Abstract;
using Api.Results;
using Api.ViewModels.Concrete;

namespace Api.Controllers
{
    public class NearbyController : ApiController
    {
        private readonly IGradezniParceliRepository _repository;

        public NearbyController(IGradezniParceliRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets nearby building parcels
        /// </summary>
        /// <param name="lon">Longitude ex. 21.1111</param>
        /// <param name="lat">Latitude, ex. 41.1111</param>
        /// <returns>List of GradParceli objects</returns>
        [ResponseType(typeof (List<GradParceli>))]
        public IHttpActionResult Get(double lon, double lat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = _repository.GetByBuffer(lon, lat);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Gone,
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(msg);
            }
        }

        /// <summary>
        /// Get all building parcels in current area
        /// </summary>
        /// <param name="id">Id of the opfat</param>
        /// <returns>List of Opfat objects</returns>
        [ResponseType(typeof(List<GradParceli>))]
        public IHttpActionResult GetByOpfat(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = _repository.GetByOpfat(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Gone,
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(msg);
            }
        }

        /// <summary>
        /// Get building parcel
        /// </summary>
        /// <param name="id">Id of the building parcel</param>
        /// <returns>GradParceli object</returns>
        [ResponseType(typeof(GradParceli))]
        public IHttpActionResult GetByGradeznaParcela(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = _repository.GetByParcela(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Gone,
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(msg);
            }
        }

        /// <summary>
        /// Get geography of building parcel
        /// </summary>
        /// <param name="id">Id of the building parcel</param>
        /// <returns>coordinates</returns>
        [ResponseType(typeof(string))]
        public IHttpActionResult GetGeom(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = _repository.GetGeom(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var msg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Gone,
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(msg);
            }
        }

        /// <summary>
        /// Get building parcel
        /// </summary>
        /// <param name="id">Id of the building parcel</param>
        /// <returns>Excel file</returns>
        public string GetByGradeznaParcelaExcel(int id)
        {
            var res = _repository.GetByParcelaExcel(id);
            return res;
            //var fileInfo = new FileInfo(res);
            //return !fileInfo.Exists
            //    ? (IHttpActionResult)NotFound()
            //    : new FileResult(fileInfo.FullName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Get excel with informations about building parcels
        /// </summary>
        /// <param name="ids">list of Ids of the building parcels</param>
        /// <returns>Excel file</returns>
        [HttpPost]
        public string GetByGradeznaParcelaExcelMultiple(List<int> ids)
        {
            var res = _repository.GetByParcelaExcel(ids);
            return res;
            //var fileInfo = new FileInfo(res);
            //return !fileInfo.Exists
            //    ? (IHttpActionResult)NotFound()
            //    : new FileResult(fileInfo.FullName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        /// <summary>
        /// Get image of the building parcel
        /// </summary>
        /// <param name="id">Id of the building parcel</param>
        /// <returns>URL</returns>
        public string GetImage(int id)
        {
            var res = _repository.GetImage(id);
            return res;
        }
    }
}