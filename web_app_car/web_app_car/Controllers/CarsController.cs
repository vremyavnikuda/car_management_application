using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_app_car.Models;

namespace web_app_car.Controllers;

public class CarsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Cars/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Cars/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarViewModel carViewModel)
    {
        if (ModelState.IsValid)
        {
            var car = new Cars
            {
                Brand = carViewModel.Brand,
                Model = carViewModel.Model,
                YearOfManufacture = carViewModel.YearOfManufacture,
                FuelType = carViewModel.FuelType,
                Price = carViewModel.Price,
                OwnerName = carViewModel.OwnerName,
                LicensePlate = carViewModel.LicensePlate,
                Power = carViewModel.Power,
                Type = carViewModel.Type,
                Transmission = carViewModel.Transmission,
                RepairDates = carViewModel.RepairDates
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(carViewModel);
    }

    // GET: Cars/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // POST: Cars/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cars car)
    {
        if (id != car.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(car);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(car.Id))
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

        return View(car);
    }

    // GET: Cars/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }

    // POST: Cars/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return NotFound();
        }

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Cars/Index
    public async Task<IActionResult> Index(string searchQuery)
    {
        var cars = _context.Cars.AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            cars = cars.Where(c => c.Brand.Contains(searchQuery) || c.Model.Contains(searchQuery));
        }

        var carList = await cars.ToListAsync();

        return View(carList);
    }

    private bool CarExists(int id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }

    // GET: Cars/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var car = await _context.Cars
            .FirstOrDefaultAsync(m => m.Id == id);
        if (car == null)
        {
            return NotFound();
        }

        return View(car);
    }
}
