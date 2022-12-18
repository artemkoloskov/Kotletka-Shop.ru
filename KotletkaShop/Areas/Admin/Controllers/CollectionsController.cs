using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
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
        /// <param name="sortOrder">Параметр сортировки</param>
        /// <param name="searchString">Параметр поиска</param>
        /// <param name="currentFilter">Параметр фильтрации</param>
        /// <param name="pageNumber">Номер страницы списка</param>
        /// <returns>The index.</returns>
        public async Task<IActionResult> Index(
            string sortOrder,
            string searchString,
            string currentFilter,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            // Если нет строки для поиска - открываем список с первой страницы
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var collections = from s in _context.Collections
                              select s;
            // Если строка поиска не пустая - пробуем считать из строки целое число
            // и выполняем поиск по нему, или, если число не удалось счтать,
            // ищем по самой строке
            // Поиск осуществляется по Заголовку, короткому названию и идентификатору
            // коллекции
            if (!string.IsNullOrEmpty(searchString))
            {
                collections = int.TryParse(searchString, out var searchNumber)
                    ? collections.Where(c => c.Title.Contains(searchString)
                                       || c.Handle.Contains(searchString)
                                       || c.CollectionID == searchNumber
                                       )
                    : collections.Where(c => c.Title.Contains(searchString)
                                       || c.Handle.Contains(searchString)
                                       );
            }

            collections = sortOrder switch
            {
                "title_desc" => collections.OrderByDescending(s => s.Title),
                _ => collections.OrderBy(s => s.Title),
            };

            return View(await collections
                .Include(c => c.Image)
                .ToListAsync());
        }

        /// <summary>
        /// Инициализация представления деталей конкретной коллекции
        /// </summary>
        /// <param name="id">Идентификатор коллекции</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // загрузка коллекции из БД по идентификтору
            var collection = await _context.Collections
                .Include(c => c.Image)
                .SingleOrDefaultAsync(m => m.CollectionID == id);

            if (collection == null)
            {
                return NotFound();
            }

            collection.Products = await collection.GetCollectionProducts(_context);

            return View(collection);
        }

        // GET: Admin/Collections/Create
        /// <summary>
        /// Инициализация представления создания новой коллекции
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Collections/Create
        /// <summary>
        /// Инициализация представления создания новой коллекции
        /// </summary>
        /// <returns></returns>
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
                    _ = _context.Add(collection);
                    _ = await _context.SaveChangesAsync();
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

        // GET: Admin/Collections/Edit/5
        /// <summary>
        /// Инициализация представления редактирования коллекции
        /// </summary>
        /// <param name="id">Идентификатор коллекции</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        // POST: Admin/Collections/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionToUpdate = await _context.Collections.FirstOrDefaultAsync(s => s.CollectionID == id);

            if (await TryUpdateModelAsync<Collection>(
                collectionToUpdate,
                "",
                c => c.Handle, c => c.MatchAllConditions, c => c.Published, c => c.Title, c => c.Body, c => c.CompareSubject1,
                c => c.CompareSubject2, c => c.CompareSubject3, c => c.CompareTo1, c => c.CompareTo2, c => c.CompareTo3,
                c => c.Condition1, c => c.Condition2, c => c.Condition3, c => c.ImageID))
            {
                try
                {
                    _ = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Не удалось сохранить изменения. " +
                    "Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.");
                }
            }

            return View(collectionToUpdate);
        }

        // GET: Collections/Delete/5
        /// <summary>
        /// Инициализация представления удаления коллекции
        /// </summary>
        /// <param name="id">Идентификатор коллекции</param>
        /// <param name="saveChangesError">Маркер ошибки удаления</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collection = await _context.Collections
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CollectionID == id);
            if (collection == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Удаление не удалось. Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.";
            }

            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _ = _context.Collections.Remove(collection);
                _ = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), (id, saveChangesError: true));
            }
        }
    }
}
