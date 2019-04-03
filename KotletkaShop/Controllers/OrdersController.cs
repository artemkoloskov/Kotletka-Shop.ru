using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KotletkaShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly StoreContext _context;

        public OrdersController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        /// <summary>
        /// Иницализация главного представления списка заказов для админки.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // загрузка из контекста списка закозов. НЕ ЧЕРНОВИКИ
            List<Order> orders = await _context.Orders.Include(o => o.Customer).Where(o => o.Draft == false).AsNoTracking().ToListAsync();

            return View(orders);
        }

        /// <summary>
        /// Инициализация представления деталей конкретного заказа
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await _context.Orders
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Orders)
                .Include(o => o.OrderProducts)
                    .ThenInclude(p => p.Product)
                .Include(o => o.OrderDiscounts)
                    .ThenInclude(d => d.Discount)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
