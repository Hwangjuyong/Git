using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ContosoUniversity.Controllers
{
    public class FilesController : Controller
    {
        
            private readonly ApplicationDbContext _context;
            

            public FilesController(ApplicationDbContext context)
            {
                _context = context;                
            }

            // GET: /File/
            public ActionResult Index(int id)
            {
                var fileToRetrieve = _context.Files.Find(id);
                return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
            }
        
    }
}