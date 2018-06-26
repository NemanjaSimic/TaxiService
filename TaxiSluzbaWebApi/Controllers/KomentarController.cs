using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace TaxiSluzbaWebApi.Controllers
{
    public class KomentarController : ApiController
    {
       [HttpGet]
       [Route("Komentar")]
       public HttpResponseMessage Komentar()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}