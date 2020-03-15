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
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Customer> customers = from s in _context.Customers
                                                 select s;

            if (!string.IsNullOrEmpty(searchString))
            {

                customers = int.TryParse(searchString, out int searchNumber)
                    ? customers.Where(c => c.FirstName.Contains(searchString)
                                                   || c.LastName.Contains(searchString)
                                                   || c.MiddleName.Contains(searchString)
                                                   || c.City.Contains(searchString)
                                                   || c.Country.Contains(searchString)
                                                   || c.District.Contains(searchString)
                                                   || c.Email.Contains(searchString)
                                                   || c.Note.Contains(searchString)
                                                   || c.PhoneNumber.Contains(searchString)
                                                   || c.Province.Contains(searchString)
                                                   || c.Street.Contains(searchString)
                                                   || c.Tags.Contains(searchString)
                                                   || c.ZipCode == searchNumber
                                                )
                    : customers.Where(c => c.FirstName.Contains(searchString)
                                                   || c.LastName.Contains(searchString)
                                                   || c.MiddleName.Contains(searchString)
                                                   || c.City.Contains(searchString)
                                                   || c.Country.Contains(searchString)
                                                   || c.District.Contains(searchString)
                                                   || c.Email.Contains(searchString)
                                                   || c.Note.Contains(searchString)
                                                   || c.PhoneNumber.Contains(searchString)
                                                   || c.Province.Contains(searchString)
                                                   || c.Street.Contains(searchString)
                                                   || c.Tags.Contains(searchString)
                                                );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(s => s.LastName);
                    break;
                default:
                    customers = customers.OrderBy(s => s.LastName);
                    break;
            }

            return View(await customers
                        .Include(c => c.Orders)
                        .Include(c => c.Payments)
                        .Include(c => c.Image)
                        .AsNoTracking()
                        .ToListAsync());
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

        // GET: Admin/Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customerToUpdate = await _context.Customers.FirstOrDefaultAsync(s => s.CustomerID == id);

            if (await TryUpdateModelAsync<Customer>(
                customerToUpdate,
                "",
                c => c.FirstName, c => c.AcceptsMarketing, c => c.Apartment,
                c => c.Building, c => c.City, c => c.Country, c => c.District,
                c => c.Email, c => c.FirstName, c => c.ImageID, c => c.LastName,
                c => c.MiddleName, c => c.Note, c => c.PhoneNumber,
                c => c.Province, c => c.Street, c => c.Tags, c => c.ZipCode))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Не удалось сохранить изменения. " +
                    "Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.");
                }
            }

            return View(customerToUpdate);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Удаление не удалось. Попробуйте еще раз, если проблема повторяется " +
                    "Свяжитесь с вашим системным администратором.";
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
