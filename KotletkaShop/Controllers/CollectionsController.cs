using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static KotletkaShop.Models.CollectionCompareSubjects;
using static KotletkaShop.Models.CollectionConditions;
using static KotletkaShop.Models.SortingOrders;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            return View(await _context.Collections.Include(c => c.Image).ToListAsync());
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
    }
}
