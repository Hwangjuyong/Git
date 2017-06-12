using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;


        public HomeController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            IQueryable<EnrollmentDateGroup> data =
                from student in _context.Students
                group student by student.EnrollmentDate into dateGroup
                select new EnrollmentDateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };
            ViewData["Path"] = _hostingEnvironment.WebRootPath + "\\";
            ViewData["Path2"] = Path.GetTempFileName();
            return View(await data.AsNoTracking().ToListAsync());
        }

        public IActionResult Contact()
        {
            int recordCount = 0;
            var dbConnection = new SqlConnection();
            dbConnection = new SqlConnection("server=erp.emaxit.net, 24879;uid=hjy;pwd=@h7*o~Jz#ym;database=System8;");

            dbConnection.Open();
            var cmd = new SqlCommand();

            cmd = new SqlCommand("select cnt = count(*) from hra200", dbConnection);
            SqlDataReader dr =cmd.ExecuteReader();
            if (dr.Read())
            {
                recordCount = (int)dr["cnt"];
            }            
            dbConnection.Close();

            ViewData["cnt"] = recordCount;
            return View();
        }
        
        [HttpPost, ActionName("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = _hostingEnvironment.WebRootPath + "\\file\\";
            string file2 ="";

            foreach (var formFile in files)
            {
                var fileName = formFile.FileName;
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath +fileName, FileMode.Create))

                    {
                        file2 = fileName;
                        await formFile.CopyToAsync(stream);                        
                    }
                }
            }

            // process uploaded files
            var file3 = filePath + file2;
            return Ok(new { count = files.Count, size, file3});
        }
    }
}
