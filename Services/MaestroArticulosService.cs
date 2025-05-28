using System.Xml.Linq;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    #region LISTA DE FALTANTES
    /// <summary>
    //Creacion, modificacion y eliminacion de Articulo (LISTO)
    //    Asignar proveedor Predeterminado
    //    Determinacion del modelo de inventario
    //    Calculo del modelo lote fijo
    //    Calculo del modelo Intervalo fijo
    //    Calculo del CGI
    //    Listado de productos a reponer //// Para hacer el pedido
    //    Listado Productos Faltantes
    //    Listado Proveedores x Articulo
    /// </summary>
    #endregion
    public class MaestroArticulosService
    {
        public readonly ArticuloRepository _articuloRepository;
        public readonly ProveedoresRepository _proveedorRepository;
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository)
        {
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedorRepository;
        }

        #region "Descripcion Articulo"
        //Creacion Articulo, modificacion y Eliminacion.
        //Falta agregar las relaciones al articulo a medida que se creen las demas entidades
        #endregion
        public async Task CreateArticulo(long id, string descripcion)
        {


            var articulo = new Articulo
            {
                idArticulo = id,
                descripcion = descripcion,
                stockArticulos = null,
                listaArticulos = null,
                masterArticulo = null

            };
            await _articuloRepository.AddAsync(articulo);
        }
        public async Task ModificarArticulo(long idArticulo,string nombreArticuloM, string descripcionM)
        {
            var articuloModificado = await _articuloRepository.GetByIdAsync(idArticulo);
            if(articuloModificado != null)
            {
                articuloModificado.descripcion = descripcionM;
                articuloModificado.nombreArticulo = nombreArticuloM;
                
            }
            else
            {
                throw new Exception("Articulo no Encontrado");
            }
            await _articuloRepository.UpdateAsync(articuloModificado);
        }
        public async Task DeleteArticulo(long idArticulo)
        {
            await _articuloRepository.DeleteIdAsync(idArticulo);
        }


        //Proveedor
        public async Task CreateProveedor(string nombreP,long idP)
        {
            var proveedor = new Proveedor
            {
                nombreProveedor = nombreP,
                idProveedor = idP,
                listaProveedores = null,
                masterArticulo = null
            };
            await _proveedorRepository.AddAsync(proveedor);
        }

        public async Task DeleteProveedor(long idProveedor)
        {
           await _proveedorRepository.DeleteIdAsync(idProveedor);
        }

        //Metodos para el calculo de Modelo de Inventario
        public void CalculoLoteFijo(){} 
        public void CalculoIntervaloFijo(){}
        public void CalculoCGI(){}
    }
}
