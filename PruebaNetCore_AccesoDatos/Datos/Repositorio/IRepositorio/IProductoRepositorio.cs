using Microsoft.AspNetCore.Mvc.Rendering;
using PruebaNetCore_Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio
{
    public interface IProductoRepositorio :IRepositorio<Producto>
    {
        void Actualizar(Producto producto);
        IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj);
    }
}
