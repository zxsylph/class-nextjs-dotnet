using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;

namespace StoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ApplicationDbContext context;

    public ProductController(ApplicationDbContext _context)
    {
        context = _context;
    }

    // Test db connection
    // GET: /api/Product/testconnectdb
    [HttpGet("testconnectdb")]
    public void TestConnection()
    {
        if (context.Database.CanConnect())
        {
            Response.WriteAsync("Connected");
        }
        else
        {
            Response.WriteAsync("Not Connected");
        }
    }

    // Get product list
    // GET: /api/Product
    [HttpGet]
    public ActionResult<product> GetProducts()
    {
        var products = context.products.ToList();

        return Ok(products);
    }
}