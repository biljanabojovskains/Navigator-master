using System.Web.Http;
using Api.Bll.Abstract;
using Api.Models;

namespace Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserRepository _repository;

        public UsersController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _repository.Register(model.Username, model.Password, model.Email);
            if (result == null)
            {
                return Conflict();
            }
            return Ok(result);

        }

        /// <summary>
        /// Validates the user against the repository.
        /// </summary>
        /// <param name="credentials">The credentials that are passed through the HTTP POST body</param>
        /// <returns>HttpResponseMessage DTO with HTTP status code that represents whether the user is validated or not</returns>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult ValidateUser([FromBody] string credentials)
        {
            var result = _repository.ValidateUser(credentials);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }
    }
}
