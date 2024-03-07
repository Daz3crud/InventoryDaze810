using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventario_2.Models;

namespace Inventario_2.Controllers
{
    public class AsientoContablesController : Controller
    {
        private readonly InventarioContext _context;

        public AsientoContablesController(InventarioContext context)
        {
            _context = context;
        }

        // GET: AsientoContables
        public async Task<IActionResult> Index(string buscar)
        {
            var vAsientoContables = from asientoContable in _context.AsientoContables
                                    join cuentaContableDB in _context.CuentaContables on asientoContable.CuentaDb equals cuentaContableDB.IdCuentaContable
                                    join cuentaContableCr in _context.CuentaContables on asientoContable.CuentaCr equals cuentaContableCr.IdCuentaContable
                                    select new AsientoContable
                                    {
                                        IdMovimiento = asientoContable.IdMovimiento,
                                        Descripcion = asientoContable.Descripcion,
                                        Auxiliar = asientoContable.Auxiliar,
                                        CuentaDb = asientoContable.CuentaDb,
                                        CuentaDbDesc = cuentaContableDB.Descripcion,
                                        CuentaCr = asientoContable.CuentaCr,
                                        CuentaCrDesc = cuentaContableCr.Descripcion,
                                        Monto = asientoContable.Monto
                                    };

            var vAsientoContablesList = await vAsientoContables.ToListAsync();

            if (!string.IsNullOrEmpty(buscar))
            {
                vAsientoContablesList = vAsientoContablesList.Where(s =>
                    s.Descripcion.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    s.CuentaDbDesc.Equals(buscar, StringComparison.OrdinalIgnoreCase) ||
                    s.CuentaCrDesc.Equals(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return View(vAsientoContablesList);
        }

        // GET: AsientoContables/Contabilizar/5
        public async Task<IActionResult> Contabilizar(int? id)
        {
            if (id == null || _context.AsientoContables == null)
            {
                return NotFound();
            }

            var asientoContable = await _context.AsientoContables.FirstOrDefaultAsync(m => m.IdMovimiento == id);

            if (asientoContable == null)
            {
                return NotFound();
            }

            return View(asientoContable);
        }

        // POST: AsientoContables/Contabilizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contabilizar([Bind("IdMovimiento,Descripcion,Auxiliar,CuentaDb,CuentaCr,Monto")] AsientoContable asientoContable)
        {
            try
            {
                // Simulación de agregar datos ficticios al contexto
                if (ModelState.IsValid)
                {
                    // Aquí puedes agregar lógica para validar y procesar los datos según tus requisitos
                    _context.AsientoContables.Add(asientoContable);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred. Please try again later. Error: {ex.Message}");
            }

            return View(asientoContable);
        }
    }
}
