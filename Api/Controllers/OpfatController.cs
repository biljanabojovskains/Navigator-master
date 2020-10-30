using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Api.Bll.Abstract;
using Api.ViewModels.Concrete;

namespace Api.Controllers
{
    public class OpfatController : ApiController
    {
        private readonly IOpfatRepository _repository;

        public OpfatController(IOpfatRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all opfats
        /// </summary>
        /// <returns>List of Opfat objects</returns>
        [ResponseType(typeof (List<Opfat>))]
        public IHttpActionResult Get()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = _repository.GetAll();
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
    }
}