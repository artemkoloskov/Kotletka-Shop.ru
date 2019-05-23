using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KotletkaShop.Data;
using KotletkaShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KotletkaShop.Controllers
{
    [Area("Admin")]
    public class CustomersController : Controller
    {
        private readonly StoreContext _context;

        public CustomersController(StoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            List<Models.Customer> customers = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Payments)
                .Include(c => c.Image)
                .AsNoTracking()
                .ToListAsync();

            return View(customers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customer = await _context.Customers
                .Include(c => c.Orders)
                .Include(c => c.Payments)
                .Include(c => c.Image)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.CustomerID == id);

            return customer == null ? NotFound() : (IActionResult)View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FirstName,MiddleName,LastName,Country,City,Province," +
                "District,Street,Building,Apartment,ZipCode," +
                "PhoneNumber,Email,Note,AcceptsMarketing,RegisterDate," +
                "ImageID,Tags")] Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(customer);
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

            return View(customer);
        }
    }
}
