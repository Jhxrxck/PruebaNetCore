using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PruebaNetCore_AccesoDatos.Datos;
using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using PruebaNetCore_Modelos.ViewModels;
using PruebaNetCore_Utilidades;
using System.Security.Claims;
using System.Text;

namespace PruebaNetCore.Controllers
{
    //El authorize nos impide si un usuario no esta registrado no le va a permitir comprar y lo va a enviar a que se regitre
    [Authorize]
    public class CarroController : Controller
    {
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IProductoRepositorio _productoRepo;
        private readonly IUsuarioAplicacionRepositorio _usuarioRepo;
        private readonly IOrdenRepositorio _ordenRepo;
        private readonly IOrdenDetalleRepositorio _ordenDetalleRepo;

        [BindProperty] //Se coloca este Atributo para que esta propiedad podamos utilizar en todo el controlador y no se pierdan sus valores
        public ProductoUsuarioVM productoUsuarioVM { get; set; }
        //CONSTRUCTOR
        public CarroController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender,
                               IProductoRepositorio productoRepo, IUsuarioAplicacionRepositorio usuarioRepo,
                               IOrdenRepositorio ordenRepo, IOrdenDetalleRepositorio ordenDetalleRepo)
        {
            // lo inicializamos en nuestro contructor
            _productoRepo = productoRepo;
            _usuarioRepo = usuarioRepo;
            _ordenRepo = ordenRepo;
            _ordenDetalleRepo = ordenDetalleRepo;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }
        //GET
        public IActionResult Index()
        {
            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroCompraList.Select(i => i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = _productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            return View(prodList);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]//Se agregan los atributos
        [ActionName("Index")]
        public IActionResult IndexPost()//Lo que hace el indexpost es redireccionar al metodo RESUMEN 
        {
            return RedirectToAction(nameof(Resumen));
        }

        //Metodo resumen
        public IActionResult Resumen()
        {
            //Traer el usuario conectado
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            List<int> prodEnCarro = carroCompraList.Select(i => i.ProductoId).ToList();
            //IEnumerable<Producto> prodList = _db.Producto.Where(p => prodEnCarro.Contains(p.Id));
            IEnumerable<Producto> prodList = _productoRepo.ObtenerTodos(p => prodEnCarro.Contains(p.Id));
            productoUsuarioVM = new ProductoUsuarioVM()
            {
                //UsuarioAplicacion = _db.UsuarioAplicacion.FirstOrDefault(u => u.Id == claim.Value),
                UsuarioAplicacion = _usuarioRepo.ObtenerPrimero(u => u.Id == claim.Value),
                ProductoLista = prodList.ToList()
            };
            return View(productoUsuarioVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Resumen")] //async es asincrono
        public async Task<IActionResult> ResumenPost(ProductoUsuarioVM productoUsuarioVM)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            var rutaTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                                 + "templates"+ Path.DirectorySeparatorChar.ToString()
                                 + "PlantillaOrden.html";
            var subject = "Nueva Orden";
            string HtmlBody = "";







            using(StreamReader sr = System.IO.File.OpenText(rutaTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productoListaSB = new StringBuilder();

            foreach(var prod in productoUsuarioVM.ProductoLista)
            {//El Append nos permite escribir HTML dentro de una variable
                productoListaSB.Append($" - Nombre:  {prod.NombreProducto}<span style='font-size:14px;'>(ID:{prod.Id})</span><br/>");
            }
            //Lo siguiente es el cuerpo del mensaje
            string messageBosy = string.Format(HtmlBody,
                productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                productoUsuarioVM.UsuarioAplicacion.Email,
                productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                productoListaSB.ToString());
            //invocaremos al email sender por medio del await y llamamos al metodo
            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBosy);

            //Grabar la Orden y Detalle en la BD
            Orden orden = new Orden()
            {
                UsuarioAplicacionId = claim.Value,
                NombreCompleto = productoUsuarioVM.UsuarioAplicacion.NombreCompleto,
                Email = productoUsuarioVM.UsuarioAplicacion.Email,
                Telefono = productoUsuarioVM.UsuarioAplicacion.PhoneNumber,
                FechaOrden = DateTime.Now
            };
            _ordenRepo.Agregar(orden);
            _ordenRepo.Grabar();

            foreach (var prod in productoUsuarioVM.ProductoLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    OrdenId = orden.Id,
                    ProductoId = prod.Id
                };
                _ordenDetalleRepo.Agregar(ordenDetalle);
            }
            _ordenDetalleRepo.Grabar();

                return RedirectToAction(nameof(Confirmacion));
        }

        public IActionResult Confirmacion()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Remover(int Id)
        {
            List<CarroCompra> carroCompraList = new List<CarroCompra>();
            if (HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras) != null &&
                HttpContext.Session.Get<IEnumerable<CarroCompra>>(WC.SessionCarroCompras).Count() > 0)
            {
                carroCompraList = HttpContext.Session.Get<List<CarroCompra>>(WC.SessionCarroCompras);
            }

            carroCompraList.Remove(carroCompraList.FirstOrDefault(p=>p.ProductoId==Id));
            HttpContext.Session.Set(WC.SessionCarroCompras, carroCompraList);
            return RedirectToAction(nameof(Index));
        }  
    }
}
