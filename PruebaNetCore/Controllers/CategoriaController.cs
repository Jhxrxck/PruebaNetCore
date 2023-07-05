using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaNetCore_AccesoDatos.Datos;
using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using PruebaNetCore_Utilidades;
using System.Data;

namespace PruebaNetCore.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepositorio _catRepo;
        public CategoriaController(ICategoriaRepositorio catRepo)
        {
            _catRepo = catRepo;   
        }
        public IActionResult Index()
        {
            IEnumerable<Categoria> lista = _catRepo.ObtenerTodos();
            return View(lista);
        }

        //Get
        public IActionResult Crear()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if(ModelState.IsValid)
            {
                _catRepo.Agregar(categoria);
                _catRepo.Grabar();
                TempData[WC.Exitosa] = "Categoria creada Exitosamente";
                return RedirectToAction(nameof(Index));
            }
            TempData[WC.Error] = "Error al crear nueva Categoria";
            return View(categoria);
            
        }
        //Get Editar
        public IActionResult Editar(int? Id)
        {
            if (Id==null || Id==0)
            {
                return NotFound();
            }
            var obj = _catRepo.Obtener(Id.GetValueOrDefault());
            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Actualizar(categoria);
                _catRepo.Grabar();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);

        }

        //Get Eliminar
        public IActionResult Eliminar(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Obtener(Id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Categoria categoria)
        {
            if (categoria == null)
            {
                return NotFound();
            }
            _catRepo.Remover(categoria);
            _catRepo.Grabar();
            return RedirectToAction(nameof(Index));

        }

    }
}
