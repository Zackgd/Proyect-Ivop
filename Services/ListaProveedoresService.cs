using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Dtos;
using Proyect_InvOperativa.Dtos.ListaProveedores;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class ListaProveedoresService
    {
        public ListaProveedoresRepository _listaPrepository;
        public ListaProveedoresService(ListaProveedoresRepository listaP)
        {
            _listaPrepository = listaP;
        }

        public void CreateListaProveedores(ListaPdatosDTO listaDto)
        {
            var listaP = new ListaProveedores()
            {
                idListaProveedores = listaDto.idListaProveedores,
                fechaInicioLProveedores = listaDto.fechaInicioLProveedores,
                fechaFinLProveedores = null
            };
        }
        public async Task DeleteListaProveedores(ListaPdatosDTO listaDto)
        {
            var listitaP = await _listaPrepository.GetByIdAsync(listaDto.idListaProveedores);
            
            if(listitaP == null)
            {
                throw new Exception("No existe Lista con ese id");
            }
            listitaP.fechaFinLProveedores = DateTime.UtcNow;
            await _listaPrepository.UpdateAsync(listitaP);
        }
    }
}
