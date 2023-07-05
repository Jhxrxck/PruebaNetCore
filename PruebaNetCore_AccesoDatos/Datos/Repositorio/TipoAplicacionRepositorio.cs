using PruebaNetCore_AccesoDatos.Datos.Repositorio.IRepositorio;
using PruebaNetCore_Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaNetCore_AccesoDatos.Datos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        public readonly ApplicationDbContext _db;
        public CategoriaRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Actualizar(Categoria categoria)
        {
            var catAnterior = _db.Categoria.FirstOrDefault(c => c.Id == categoria.Id);
            if (catAnterior != null)
            {
                catAnterior.NombreCategoria = categoria.NombreCategoria;
                catAnterior.MostrarOrden = categoria.MostrarOrden;
            }
        }
    }
}
