using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TestODataModels;

namespace ODataServer.Controllers
{
    public class CitiesController : ODataController
    {
        public CitiesController()
        {
        }

        public ActionResult Post([FromBody] City continent)
        {
            return Created(continent);
        }

    }
}
