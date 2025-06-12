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
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly ListaArticuloRepository _listaArticuloRepository;
        private readonly BaseRepository<StockArticulos> _stockArticulosRepository;

        public MaestroArticulosService(ArticuloRepository articuloRepository, MaestroArticulosRepository maestroArticulosRepository, ListaArticuloRepository listaArticulo, BaseRepository<StockArticulos> stockArticulosRepository)
        {
            _articuloRepository = articuloRepository;
            _maestroArticuloRepository = maestroArticulosRepository;
            _listaArticuloRepository = listaArticulo;
            _stockArticulosRepository = stockArticulosRepository;
        }
        #region AB Maestro Articulo
        public async Task<MaestroArticulo> CreateMaestroArticulo(CreateMaestroArticuloDto createMaestroArticuloDto)
        {
            var maestro = new MaestroArticulo()
            {
                nombreMaestro = createMaestroArticuloDto.nombreMaestroArticulo,
            };

            var newMaestro = await _maestroArticuloRepository.AddAsync(maestro);

            return newMaestro;

        }

        public async Task DeleteMaestroArticulo(long idMaestroArticulo)
        {
            var maestroArticulo = await _maestroArticuloRepository.GetByIdAsync(idMaestroArticulo);

            if (maestroArticulo is null)
            {
                throw new KeyNotFoundException($"Artículo con id: {idMaestroArticulo} no encontrado. ");
            }

            await _maestroArticuloRepository.DeleteIdAsync(idMaestroArticulo);
        }
        #endregion

        #region ABM Articulo
        

        public async Task<Articulo> CreateArticulo(ArticuloDto articuloDto, StockArticulosDto stockArticulosDto)
        {
            var articulo = new Articulo()
            {
                nombreArticulo = articuloDto.nombreArticulo,
                descripcion = articuloDto.descripcion,
                idMaestroArticulo = articuloDto.idMaestroArticulo,
                idListaArticulos = articuloDto.idListaArticulo
            };

            var newArticulo = await _articuloRepository.AddAsync(articulo);

            var stockArticulos = new StockArticulos()
            {
                stockSeguridad = stockArticulosDto.stockSeguridad,
                stockActual = stockArticulosDto.stockActual,
                fechaStockInicio = stockArticulosDto.fechaStockInicio,
                fechaStockFin = stockArticulosDto.fechaStockFin,
                articulo = articulo
            };

            var newStockArticulos = await _stockArticulosRepository.AddAsync(stockArticulos);

            return newArticulo;
        }

        public async Task UpdateArticulo(long idArticulo, ArticuloDto updateArticuloDto)
        {
            var articuloModificado = await _articuloRepository.GetByIdAsync(idArticulo);
            if (articuloModificado is null)
            {
                throw new KeyNotFoundException($"Artículo con id: {idArticulo} no encontrado. ");
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
                throw new KeyNotFoundException($"Artículo con id: {idArticulo} no encontrado. ");
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

        #region Métodos Stock Artículo

        public async Task UpdateStockArticulosAsync(long idStockArticulo, StockArticulosDto stockDto)
        {
            if (!stockDto.idArticulo.HasValue || stockDto.idArticulo.Value <= 0)
            {
                throw new ArgumentException("El Stock de artículos debe tener un ID de artículo válido.");
            }

            var articulo = await _articuloRepository.GetByIdAsync(stockDto.idArticulo.Value);

            if (articulo is null)
            {
                throw new KeyNotFoundException($"No se encontró el artículo con ID {stockDto.idArticulo.Value}");
            }

            var existingStock = await _stockArticulosRepository.GetByIdAsync(idStockArticulo);

            existingStock.stockSeguridad = stockDto.stockSeguridad;
            existingStock.stockActual = stockDto.stockActual;
            existingStock.fechaStockInicio = stockDto.fechaStockInicio;
            existingStock.fechaStockFin = stockDto.fechaStockFin;
            existingStock.articulo = articulo;

            await _stockArticulosRepository.UpdateAsync(existingStock);
        }

        public async Task<StockArticulos> GetStockArticulosByArticuloId(long idArticulo)
        {

            var articulo = await _articuloRepository.GetByIdAsync(idArticulo);
            if (articulo is null)
            {
                throw new KeyNotFoundException($"No se encontró el artículo con ID {idArticulo}. ");
            }
            if (articulo.StockArticulos is null)
            {
                throw new Exception($"El artículo ID {idArticulo} no tiene stock disponible. ");
            }

            return articulo.StockArticulos;
        }

        #endregion

        //Metodos para el calculo de Modelo de Inventario
        public void CalculoLoteFijo(){} 
        public void CalculoIntervaloFijo(){}
        public void CalculoCGI(){}
    }
}
