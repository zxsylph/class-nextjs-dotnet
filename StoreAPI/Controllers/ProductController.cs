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

    // Get product list
    // GET: /api/Product/filter
    [HttpGet("filter")]
    public ActionResult<product> GetProductsWithFilter()
    {
        var products = context.products.Where(p => p.product_name.Contains("i")).ToList();

        return Ok(products);
    }

    // Get product list
    // GET: /api/Product/join
    [HttpGet("join")]
    public ActionResult<product> GetProductsWithJoin()
    {
        // LINQ สำหรับการดึงข้อมูลจากตาราง Products ทั้งหมด
        // var products = context.products.ToList();

        // แบบอ่านที่มีเงื่อนไข
        // var products = context.products.Where(p => p.unit_price > 45000).ToList();

        // แบบเชื่อมกับตารางอื่น products เชื่อมกับ categories
        var products = context.products
            .Join(
                context.categories,
                p => p.category_id,
                c => c.category_id,
                (p, c) => new
                {
                    p.product_id,
                    p.product_name,
                    p.unit_price,
                    p.unit_in_stock,
                    c.category_name
                }
            ).ToList();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(products);
    }

    // ฟังก์ชันสำหรับการดึงข้อมูลสินค้าตาม id
    // GET: /api/Product/{id}
    [HttpGet("{id}")]
    public ActionResult<product> GetProduct(int id)
    {
        // LINQ สำหรับการดึงข้อมูลจากตาราง Products ตาม id
        var product = context.products.FirstOrDefault(p => p.product_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (product == null)
        {
            return NotFound();
        }

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(product);
    }

    // ฟังก์ชันสำหรับการเพิ่มข้อมูลสินค้า
    // POST: /api/Product
    [HttpPost]
    public ActionResult<product> CreateProduct(product product)
    {
        // เพิ่มข้อมูลลงในตาราง Products
        context.products.Add(product);
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(product);
    }

    // ฟังก์ชันสำหรับการแก้ไขข้อมูลสินค้า
    // PUT: /api/Product/{id}
    [HttpPut("{id}")]
    public ActionResult<product> UpdateProduct(int id, product product)
    {
        // ดึงข้อมูลสินค้าตาม id
        var existingProduct = context.products.FirstOrDefault(p => p.product_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (existingProduct == null)
        {
            return NotFound();
        }

        // แก้ไขข้อมูลสินค้า
        existingProduct.product_name = product.product_name;
        existingProduct.unit_price = product.unit_price;
        existingProduct.unit_in_stock = product.unit_in_stock;
        existingProduct.category_id = product.category_id;

        // บันทึกข้อมูล
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(existingProduct);
    }

    // ฟังก์ชันสำหรับการลบข้อมูลสินค้า
    // DELETE: /api/Product/{id}
    [HttpDelete("{id}")]
    public ActionResult<product> DeleteProduct(int id)
    {
        // ดึงข้อมูลสินค้าตาม id
        var product = context.products.FirstOrDefault(p => p.product_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (product == null)
        {
            return NotFound();
        }

        // ลบข้อมูล
        context.products.Remove(product);
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(product);
    }
}