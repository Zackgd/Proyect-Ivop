using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;
using ZstdSharp;

namespace Proyect_InvOperativa.Services
{
    public class ListaArticuloService
    {
        public ListaArticuloRepository _listaArticulo;
        private readonly ProveedoresRepository _proveedoresRepository;
        public ListaArticuloService(ListaArticuloRepository listaA, ProveedoresRepository proveedoresRepository) 
        {
            _listaArticulo = listaA;
            _proveedoresRepository = proveedoresRepository;
        }
        public async Task<ListaArticulos> CreateListaArticulo (ListaArticuloDto listaADto)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(listaADto.idproveedor);

            var listaArticulo = new ListaArticulos()
            {
                idListaArticulos = listaADto.idListaArticulos,
                fechaInicioLista = DateTime.UtcNow,
                fechaFinLista = null,
                proveedor = proveedor
            };
            var listaANew = await _listaArticulo.AddAsync(listaArticulo);
            return listaANew;
        }

        public async Task DeleteListaArticulo (ListaArticuloDto listaAUpdate)
        {
            var listaExistente = await _listaArticulo.GetByIdAsync(listaAUpdate.idListaArticulos);
            if (listaExistente == null)
            {
                throw new Exception("No existe la lista");
            }
            listaExistente.fechaFinLista = DateTime.UtcNow;
            await _listaArticulo.UpdateAsync(listaExistente);
        }

    }
}
