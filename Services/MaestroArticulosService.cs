using System.Xml.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.MaestroArticulo;
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
        private readonly ArticuloRepository _articuloRepository;
        private readonly ProveedoresRepository _proveedorRepository;
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly ListaArticuloRepository _listaArticulo;
        public MaestroArticulosService(ArticuloRepository articuloRepository, ProveedoresRepository proveedorRepository, MaestroArticulosRepository maestroArticulosRepository,ListaArticuloRepository listaArticulo)
        {
            _articuloRepository = articuloRepository;
            _proveedorRepository = proveedorRepository;
            _maestroArticuloRepository = maestroArticulosRepository;
            _listaArticulo = listaArticulo;
        }
        #region AB Maestro Articulo
        public async Task<MaestroArticulo> CreateMaestroArticulo(CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = new MaestroArticulo()
            {
                idMaestroArticulo = 1,
                nombreMaestro = createMaestroArticuloDto.nombreMaestroArticulo,
            };

            var newMaestro = await _maestroArticuloRepository.AddAsync(maestro);

            return newMaestro;

        }

        public async Task DeleteMaestroArticulo(long idMaestroArticulo)
        {
            var maestroArticulo = await _articuloRepository.GetByIdAsync(idMaestroArticulo);

            if (maestroArticulo is null)
            {
                throw new Exception($"Artículo con id: {idMaestroArticulo} no encontrado. ");
            }

            await _articuloRepository.DeleteIdAsync(idMaestroArticulo);
        }
        #endregion

        #region ABM Articulo
        //Creacion Articulo, modificacion y Eliminacion.
        //Falta agregar las relaciones al articulo a medida que se creen las demas entidades

        public async Task<Articulo> CreateArticulo(ArticuloDto ArticuloDto)
        {
            var maestro = await _maestroArticuloRepository.GetByIdAsync(1); //debe haber otra forma, es para que funcione, despues lo arreglo 
            var listaArt = await _listaArticulo.GetByIdAsync(ArticuloDto.idArticulo);
            var articulo = new Articulo()
            {
                idArticulo = ArticuloDto.idArticulo,
                nombreArticulo = ArticuloDto.nombreArticulo,
                descripcion = ArticuloDto.descripcion,
                listaArticulos = listaArt,
                masterArticulo = maestro
            };
            
            var newArticulo = await _articuloRepository.AddAsync(articulo);

            return newArticulo;
        }

        public async Task UpdateArticulo(long idArticulo, UpdateArticuloDto updateArticuloDto)
        {
            var articuloModificado = await _articuloRepository.GetByIdAsync(idArticulo);
            if (articuloModificado is null)
            {
                throw new Exception($"Artículo con id: {idArticulo} no encontrado. ");
            }

            articuloModificado.nombreArticulo = updateArticuloDto.nombreArticulo;
            articuloModificado.descripcion = updateArticuloDto.descripcion;

            await _articuloRepository.UpdateAsync(articuloModificado);

        }
        public async Task DeleteArticulo(long idArticulo)
        {
            var artEliminar = await _articuloRepository.GetByIdAsync(idArticulo);

            if (artEliminar is null)
            {
                throw new Exception($"Artículo con id: {idArticulo} no encontrado. ");
            }

            await _articuloRepository.DeleteIdAsync(idArticulo);

        }

        public async Task<IEnumerable<Articulo>> GetAllArticulos()
        {
            var articulos = await _articuloRepository.GetAllAsync();

            return articulos;
        }

        public async Task<Articulo> GetArticuloById(long idArticulo)
        {
            var articulo = await _articuloRepository.GetByIdAsync(idArticulo);

            return articulo;
        }
        #endregion

        #region ABM Proveedor
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
        #endregion

        
        //Metodos para el calculo de Modelo de Inventario
        public void CalculoLoteFijo(){} 
        public void CalculoIntervaloFijo(){}
        public void CalculoCGI(){}
    }
}
