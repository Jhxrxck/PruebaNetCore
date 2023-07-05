using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using PruebaNetCore_Modelos.ViewModels;
using PruebaNetCore_Utilidades;

namespace PruebaNetCore.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class OrdenController : Controller
    {
        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;
        [BindProperty]
        public OrdenVM OrdenVM { get; set; }
        public OrdenController(IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
            _ordenRepo = ordenRepo;
            _ordenDetalleRepo = ordenDetalleRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detalle(int id)
        {
            OrdenVM = new OrdenVM()
            {
                Orden = _ordenRepo.ObtenerPrimero(o=>o.Id==id),
                OrdenDetalle = _ordenDetalleRepo.ObtenerTodos(d=>d.OrdenId==id, incluirPropiedades: "Producto"),
            };
            return View(OrdenVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalle()
        {
            List<CarroCompra> carroCompraLista = new List<CarroCompra>();
            OrdenVM.OrdenDetalle = _ordenDetalleRepo.ObtenerTodos(d => d.OrdenId == OrdenVM.Orden.Id);
            foreach(var detalle in OrdenVM.OrdenDetalle)
            {
                CarroCompra carroCompra = new CarroCompra()
                {
                    ProductoId = detalle.ProductoId
                };
                carroCompraLista.Add(carroCompra);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraLista);
            HttpContext.Session.Set(WC.SessionOrdenId, OrdenVM.Orden.Id);
            return RedirectToAction("Index","Carro");


        }
        [HttpPost]
        public IActionResult Eliminar()
        {
            Orden orden = _ordenRepo.ObtenerPrimero(o => o.Id == OrdenVM.Orden.Id);
            IEnumerable<OrdenDetalle> ordenDetalle = _ordenDetalleRepo.ObtenerTodos(d => d.OrdenId == OrdenVM.Orden.Id);



            _ordenDetalleRepo.RemoverRango(ordenDetalle);
            _ordenRepo.Remover(orden);
            _ordenRepo.Grabar();
            return RedirectToAction(nameof(Index));
        }
        #region APIs
        [HttpGet]
        public IActionResult ObtenerListaOrdenes()
        {
            return Json(new { data = _ordenRepo.ObtenerTodos() });
        }
        #endregion
    }
}
