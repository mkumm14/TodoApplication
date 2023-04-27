using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApplication.Data;
using TodoApplication.Models;
using Microsoft.AspNetCore.Identity;


namespace TodoApplication.Controllers
{
    public class TodoController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        public TodoController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db= db;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var todos = _db.ToDoItems.Include(t => t.ApplicationUser)
                                     .Where(t => t.ApplicationUserId == currentUser.Id)
                                     .ToList();

            return View(todos);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItem todoItem)
        {
            if (todoItem.DueDate.HasValue && todoItem.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError("DueDate", "Due date cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                todoItem.ApplicationUserId = currentUser.Id;

                _db.ToDoItems.Add(todoItem);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(todoItem);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var todoItem = await _db.ToDoItems
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }
            else if (todoItem.ApplicationUserId != currentUser.Id)
            {
                return View("PermissionDenied");
            }


            return View(todoItem);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var todoItem = await _db.ToDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            else if (todoItem.ApplicationUserId != currentUser.Id)
            {
                return View("PermissionDenied");
            }


            _db.ToDoItems.Remove(todoItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Todo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var todoItem = await _db.ToDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }
            else if (todoItem.ApplicationUserId != currentUser.Id)
            {
                return View("PermissionDenied");
            }
            return View(todoItem);
        }

        // POST: Todo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToDoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (todoItem.ApplicationUserId != currentUser.Id)
            {
                return View("PermissionDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(todoItem);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.ToDoItems.Any(e => e.Id == todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }


    }
}
