using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PruebaNetCore_AccesoDatos.Datos;
using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using PruebaNetCore_Modelos.ViewModels;
using PruebaNetCore_Utilidades;
using System.Data;
using System.IO;
using System.Linq; 

namespace PruebaNetCore.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductoController : Controller
    {
        private readonly IProductoRepositorio _productoRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IProductoRepositorio productoRepo, IWebHostEnvironment webHostEnvironment)
        {
            _productoRepo = productoRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //IEnumerable<Producto> lista = _db.Producto.Include(c => c.Categoria).Include(t => t.TipoAplicacion);
            IEnumerable<Producto> lista = _productoRepo.ObtenerTodos(incluirPropiedades : "Categoria,TipoAplicacion");
            return View(lista);
        }
        //GET
        public IActionResult Upsert(int? Id)
        {
            //IEnumerable<SelectListItem> categoriaDropDown = _db.Categoria.Select(c=> new SelectListItem { 
            //  Text = c.NombreCategoria,
            //  Value = Id.ToString()
            //});

            //ViewBag.categoriaDropDown = categoriaDropDown;

            //Producto producto = new Producto();

            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                //CategoriaLista = _db.Categoria.Select(c => new SelectListItem
                //{
                //    Text = c.NombreCategoria,
                //    Value = c.Id.ToString()
                //}),
                //TipoAplicacionLista = _db.TipoAplicacion.Select(c => new SelectListItem
                //{
                //    Text = c.Nombre,
                //    Value = c.Id.ToString()
                //})
                CategoriaLista = _productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre),
                TipoAplicacionLista = _productoRepo.ObtenerTodosDropdownList(WC.TipoAplicacionNombre)
            };


            if (Id==null)
            {
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = _productoRepo.Obtener(Id.GetValueOrDefault());
                if(productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPatch = _webHostEnvironment.WebRootPath;
                if (productoVM.Producto.Id == 0)
                {
                    //Crear
                    string upload = webRootPatch + WC.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString(); //Es para que se le asigne un Id a la imagen
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);//fileStream permite guardar nuestra  imagen
                    } //Todo lo anterior para guardar la imagen
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    _productoRepo.Agregar(productoVM.Producto);
                }
                else
                {
                    //Actualizar
                    var objProducto = _productoRepo.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking:false);
                    if (files.Count >0) //Se carga una imagen
                    {
                        string upload = webRootPatch + WC.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString(); //Es para que se le asigne un Id a la imagen
                        string extension = Path.GetExtension(files[0].FileName);

                        //borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        //fin borrar imagen anterior
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);//fileStream permite guardar nuestra  imagen
                        }

                        productoVM.Producto.ImagenUrl = fileName + extension;
                        //Caso contrario si no se carga una nueva imagen
                    }
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    //Actualiza la base de datos
                    _productoRepo.Actualizar(productoVM.Producto);
                }
                _productoRepo.Grabar();
                return RedirectToAction("Index");
                
            }// If ModeIsValid
             //Se llenan nuevamente las listas si algo falla
             //productoVM.CategoriaLista = _db.Categoria.Select(c => new SelectListItem
             //{
             //    Text = c.NombreCategoria,
             //    Value = c.Id.ToString()
             //});
             //productoVM.TipoAplicacionLista = _db.TipoAplicacion.Select(c => new SelectListItem
             //{
             //    Text = c.Nombre,
             //    Value = c.Id.ToString()
             //});
            productoVM.CategoriaLista = _productoRepo.ObtenerTodosDropdownList(WC.CategoriaNombre);
            productoVM.TipoAplicacionLista = _productoRepo.ObtenerTodosDropdownList(WC.TipoAplicacionNombre);
            return View(productoVM);
        }
        //Get
        public IActionResult Eliminar(int? Id)
        {
            if (Id==null || Id==0)
            {
                return NotFound();
            }
            Producto producto = _productoRepo.ObtenerPrimero(p=> p.Id==Id, incluirPropiedades: "Categoria,TipoAplicacion");
            if(producto== null)
            {
                return NotFound();
            }
            return View(producto);
        }
        // Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(Producto producto)
        {
            if(producto == null)
            {
                return NotFound();
            }
            //Eliminar la imagen
            string upload = _webHostEnvironment.WebRootPath + WC.ImagenRuta;
            

            //borrar la imagen anterior
            var anteriorFile = Path.Combine(upload, producto.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            //fin Borrar imagen anterior

            _productoRepo.Remover(producto);
            _productoRepo.Grabar();
            return RedirectToAction(nameof(Index));
        }



    }
}
