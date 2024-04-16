using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")] // GET : api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(100);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("servererror")] // GET : api/buggy/servererror
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);
            var productToReturn = product.ToString();

            return Ok(productToReturn);
        }

        [HttpGet("badrequest")] // GET : api/buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("badrequest/{id}")] // GET : api/buggy/badrequest/{id}
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }


    }
}
