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
    public class UsuarioAplicacionRepositorio : Repositorio<UsuarioAplicacion>, IUsuarioAplicacionRepositorio
    {
        public readonly ApplicationDbContext _db;
        public UsuarioAplicacionRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Actualizar(UsuarioAplicacion usuarioAplicacion)
        {
            _db.Update(usuarioAplicacion);
        }
    }
}
