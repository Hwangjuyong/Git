using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Articles
        public async Task<IActionResult> Index(          
            string currentFilter,
            string searchString,
            int? page)
        {         
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var articles = from s in _context.Article.Include(a => a.Writer )
                           select s                          
                           ;
            if (!String.IsNullOrEmpty(searchString))
            {
                articles = articles.Where(s => s.Title.Contains(searchString) || s.Content.Contains(searchString));
            }

            articles = articles.OrderByDescending(s => s.cDate);
            
            
            int pageSize = 4;
            return View(await PaginatedList<Article>.CreateAsync(articles.AsNoTracking(), page ?? 1, pageSize));

        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }          

            var article = await _context.Article
                .Include(a => a.Writer)               
                .Include(i => i.Replys).ThenInclude(i => i.Writer)                                   
                .SingleOrDefaultAsync(m => m.ID == id);
            
            if (article == null)
            {
                return NotFound();
            }            
            if (article.WriterID == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                ViewData["isWriter"] = true;
            }
            else
            {
                ViewData["isWriter"] = false;
            }

            return View(article);
        }

        // GET: Articles/Create        
        public IActionResult Create()
        {
            ViewData["WriterID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Content,Title")] Article article)
        {
            article.cDate = DateTime.Now;
            article.WriterID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["WriterID"] = new SelectList(_context.Users, "Id", "Id", article.WriterID);
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ID == id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["WriterID"] = new SelectList(_context.Users, "Id", "Id", article.WriterID);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Content,Title,cDate,WriterID")] Article article)
        {
            if (id != article.ID)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["WriterID"] = new SelectList(_context.Users, "Id", "Id", article.WriterID);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Writer)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.ID == id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ID == id);
        }



        // POST: Articles/AddComment        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            var reply = new Reply();


            reply.cDate = DateTime.Now;
            reply.WriterID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            reply.ArticleID = (int)id;
            reply.Content = comment;




            if (ModelState.IsValid)
            {
                _context.Add(reply);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details/" + id);
            }            
            return RedirectToAction("Details/"+id);
        }

        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int articleId)
        {
            var reply = await _context.Reply.SingleOrDefaultAsync(m => m.ID == id);
            
            _context.Reply.Remove(reply);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details/" + articleId);
        }

        private bool IsAuthor(string id)
        {            
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) == id)
            {
                return true;
            }
            return false;
        }
    }
}
