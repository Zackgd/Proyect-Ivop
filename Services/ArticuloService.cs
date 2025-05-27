using Proyect_InvOperativa.Repository;

using Proyect_InvOperativa.Models;

namespace Proyect_InvOperativa.Services
{
    public class ArticuloService
    {
        private readonly ArticuloRepository _articuloRepository;
        

        public ArticuloService(ArticuloRepository articuloRepository)
        {
            _articuloRepository = articuloRepository;
        }
        public async Task CreateArticulo(long id,string descripcion)
        {
        

            var articulo =  new Articulo
            {
                idArticulo = id,
                descripcion = descripcion,
                stockArticulos = null,
                listaArticulos = null,
                masterArticulo = null

            };
            await _articuloRepository.AddAsync(articulo);
        }
    }

}
