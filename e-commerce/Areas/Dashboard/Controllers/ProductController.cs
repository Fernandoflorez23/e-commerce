using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using e_commerce.Data;
using e_commerce.Models;

namespace e_commerce.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Dashboard/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Dashboard/Product/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if(Image==null)
                {
                    ModelState.AddModelError(nameof(Product.Image), "Image is required");
                    return View(product);
                }

                var imageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);
                
                if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products"));
                }

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                product.Image = $"/img/Products/{imageName}";

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Dashboard/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Dashboard/Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile Image, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldProduct = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                    if (Image != null)
                    {
                        var imageName = Guid.NewGuid() + Path.GetExtension(Image.FileName);

                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products")))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products"));
                        }

                        var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Products", imageName);

                        await using (var stream = new FileStream(savePath, FileMode.Create))
                        {
                            await Image.CopyToAsync(stream);
                        }

                        oldProduct.Image = $"/img/Products/{imageName}";

                    }

                    oldProduct.Price = product.Price;
                    oldProduct.Description = product.Description;
                    oldProduct.Name = product.Name;

                    _context.Update(oldProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Dashboard/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Dashboard/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
