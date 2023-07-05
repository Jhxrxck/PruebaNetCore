using Microsoft.AspNetCore.Mvc.Rendering;
using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using PruebaNetCore_Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaNetCore_AccesoDatos.Datos.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        public readonly ApplicationDbContext _db;
        public ProductoRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Actualizar(Producto producto)
        {
            _db.Update(producto);
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj)
        {
            if(obj == WC.CategoriaNombre)
            {
                return _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                });
                
            }
            if(obj == WC.TipoAplicacionNombre)
            {
                return _db.Categoria.Select(c => new SelectListItem
                {
                    Text = c.NombreCategoria,
                    Value = c.Id.ToString()
                });
            }
            return null;
        }
    }
}
