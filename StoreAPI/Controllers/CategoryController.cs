using Microsoft.AspNetCore.Mvc;
using StoreAPI.Data;
using StoreAPI.Models;

namespace StoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext context;

    private readonly IWebHostEnvironment env;

    public CategoryController(ApplicationDbContext _context, IWebHostEnvironment _env)
    {
        context = _context;
        env = _env;
    }

    // Test db connection
    // GET: /api/Category/testconnectdb
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

    // Get category list
    // GET: /api/Category
    [HttpGet]
    public ActionResult<category> GetCategorys()
    {
        var categories = context.categories.ToList();

        return Ok(categories);
    }

    // Get category list
    // GET: /api/Category/filter
    [HttpGet("filter")]
    public ActionResult<category> GetCategorysWithFilter()
    {
        var categories = context.categories.Where(p => p.category_name.Contains("i")).ToList();

        return Ok(categories);
    }

    // Get category list
    // GET: /api/Category/join
    [HttpGet("join")]
    public ActionResult<category> GetCategorysWithJoin()
    {
        // LINQ สำหรับการดึงข้อมูลจากตาราง Categorys ทั้งหมด
        // var categories = context.categories.ToList();

        // แบบอ่านที่มีเงื่อนไข
        // var categories = context.categories.Where(p => p.unit_price > 45000).ToList();

        // แบบเชื่อมกับตารางอื่น categories เชื่อมกับ categories
        var categories = context.categories
            .Join(
                context.categories,
                p => p.category_id,
                c => c.category_id,
                (p, c) => new
                {
                    p.category_id,
                    p.category_name,
                }
            ).ToList();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(categories);
    }

    // ฟังก์ชันสำหรับการดึงข้อมูลสินค้าตาม id
    // GET: /api/Category/{id}
    [HttpGet("{id}")]
    public ActionResult<category> GetCategory(int id)
    {
        // LINQ สำหรับการดึงข้อมูลจากตาราง Categorys ตาม id
        var category = context.categories.FirstOrDefault(p => p.category_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (category == null)
        {
            return NotFound();
        }

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(category);
    }

    // ฟังก์ชันสำหรับการเพิ่มข้อมูลสินค้า
    // POST: /api/Category
    [HttpPost]
    public ActionResult<category> CreateCategory(category category)
    {
        // เพิ่มข้อมูลลงในตาราง Categorys
        context.categories.Add(category);
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(category);
    }

    // ฟังก์ชันสำหรับการเพิ่มข้อมูลสินค้า
    // POST: /api/Category
    [HttpPost("upload")]
    public async Task<ActionResult> UploadImage(IFormFile imageFile)
    {
        if (imageFile != null)
        {
            // create image filename
            string fileName = Guid.NewGuid().ToString() + "." + Path.GetExtension(imageFile.FileName);

            // create image path
            string uploadPath = Path.Combine(env.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath = Path.Combine(uploadPath, fileName);

            // save image file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
        }
        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return NoContent();
    }

    // ฟังก์ชันสำหรับการแก้ไขข้อมูลสินค้า
    // PUT: /api/Category/{id}
    [HttpPut("{id}")]
    public ActionResult<category> UpdateCategory(int id, category category)
    {
        // ดึงข้อมูลสินค้าตาม id
        var existingCategory = context.categories.FirstOrDefault(p => p.category_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (existingCategory == null)
        {
            return NotFound();
        }

        // แก้ไขข้อมูลสินค้า
        existingCategory.category_name = category.category_name;

        // บันทึกข้อมูล
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(existingCategory);
    }

    // ฟังก์ชันสำหรับการลบข้อมูลสินค้า
    // DELETE: /api/Category/{id}
    [HttpDelete("{id}")]
    public ActionResult<category> DeleteCategory(int id)
    {
        // ดึงข้อมูลสินค้าตาม id
        var category = context.categories.FirstOrDefault(p => p.category_id == id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความ Not Found
        if (category == null)
        {
            return NotFound();
        }

        // ลบข้อมูล
        context.categories.Remove(category);
        context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้
        return Ok(category);
    }
}