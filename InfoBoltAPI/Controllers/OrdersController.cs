using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InfoBoltAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly infoboltContext _context;

        public OrdersController(infoboltContext infoboltContext)
        {
            _context = infoboltContext;
        }

        // GET: api/<OrdersController>
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _context.Orders;
        }

        // GET api/<OrdersController>/5
        [HttpGet("{userid}")]
        public List<Order>? Get(int userid)
        {
            if (User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                var loggedUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.Parse(loggedUserId) != userid)
                    return null;
                var orders = _context.Orders.Where(x => x.Userid == userid).ToList();
                orders.ForEach(x =>
                {
                    x.Items = _context.Items.Where(i => i.Orderid == x.Id).ToList();
                });
                return orders;
            }
            return null;
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Order o)
        {
            if (User.Identity.IsAuthenticated) //Gets the ClaimsPrincipal from the sent auth cookie.
            {
                o.Items.ToList().ForEach(x => x.Order = o);
                _context.Orders.Add(o);
                if (await _context.SaveChangesAsync() > 0)
                    return StatusCode(201);
                return StatusCode(500);
            }
            return StatusCode(401);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
