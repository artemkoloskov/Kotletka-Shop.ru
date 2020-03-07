using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    public class CollectionsController : Controller
    {
        private readonly StoreContext _context;

        public CollectionsController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Инициализация главного пердставления списка коллекций товаров
        /// </summary>
        /// <returns>The index.</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Collections
                .Include(c => c.Image)
                .ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретной коллекции
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // загрузка коллекции из БД по идентификтору
            Collection collection = await _context.Collections
                .Include(c => c.Image)
                .SingleOrDefaultAsync(m => m.CollectionID == id);

            if (collection == null)
            {
                return NotFound();
            }

            collection.Products = await collection.GetCollectionProducts(_context);

            return View(collection);
        }

        // GET: Collections/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Collections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Handle,Title,Body,Published,CompareSubject1," +
                "Condition1,CompareTo1,CompareSubject2,Condition2," +
                "CompareTo2,CompareSubject3,Condition3,CompareTo3," +
                "MatchAllConditions,ImageID,SortByImage")] Collection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(collection);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Не удалось сохранить изменения. " +
                    "Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.");
            }

            return View(collection);
        }
    }
}
