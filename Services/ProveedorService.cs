using Proyect_InvOperativa.Dtos.Articulo;
using Proyect_InvOperativa.Dtos.Proveedor;
using Proyect_InvOperativa.Models;
using Proyect_InvOperativa.Repository;

namespace Proyect_InvOperativa.Services
{
    public class ProveedorService
    {
        private readonly ProveedoresRepository _proveedoresRepository;
        private readonly ProveedorArticuloRepository _proveedorArticuloRepository;
        private readonly EstadoProveedoresRepository _estProveedorRepository;
        private readonly ArticuloRepository _articuloRepository;
        private readonly MaestroArticulosRepository _maestroArticuloRepository;
        private readonly OrdenCompraRepository _ordenCompraRepository;
        private readonly ProveedorEstadoRepository _proveedorEstadoRepository;

        public ProveedorService(ProveedorEstadoRepository proveedorEstadoRepository, OrdenCompraRepository ordenCompraRepository, EstadoProveedoresRepository estProveedorRepository, ArticuloRepository articuloRepository, ProveedoresRepository proveedoresRepository, ProveedorArticuloRepository proveedorArticulo, MaestroArticulosRepository maestroArticulosRepository)
        {
            _proveedorEstadoRepository = proveedorEstadoRepository;
            _articuloRepository = articuloRepository;
            _proveedoresRepository = proveedoresRepository;
            _ordenCompraRepository = ordenCompraRepository;
            _estProveedorRepository = estProveedorRepository;
            _proveedorArticuloRepository = proveedorArticulo;
            _maestroArticuloRepository = maestroArticulosRepository;
        }


        #region ABM Proveedores

        #region CREAR PROVEEDOR
        public async Task<Proveedor> CreateProveedor(ProveedorDto ProveedorDto)
        {
            var maestro = await _maestroArticuloRepository.GetByIdAsync(1);
            var EstProv = await _proveedorEstadoRepository.GetByIdAsync(1);//revisar
            var proveedor_n = new Proveedor()
            {
                nombreProveedor = ProveedorDto.nombreProveedor,
                direccion = ProveedorDto.direccion,
                mail = ProveedorDto.mail,
                telefono = ProveedorDto.telefono,
                masterArticulo = maestro
            };
            var estadoProveedor = new EstadoProveedores()
            {
                nEstado = 1,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null,
                proveedor = proveedor_n,
                proveedorEstado = EstProv
            };

            var newProveedor = await _proveedoresRepository.AddAsync(proveedor_n);
            var newEstProveedor = await _estProveedorRepository.AddAsync(estadoProveedor);

            return newProveedor;
        }
        #endregion

        #region ACTUALIZAR PROVEEDOR 
        public async Task UpdateProveedor(long idProveedor, ProveedorDto updateProveedorDto)
        {
            var proveedorModificado = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (proveedorModificado is null)
            {
                throw new Exception($"Proveedor con id: {idProveedor} no encontrado. ");
            }

            proveedorModificado.nombreProveedor = updateProveedorDto.nombreProveedor;
            proveedorModificado.mail = updateProveedorDto.mail;
            proveedorModificado.direccion = updateProveedorDto.direccion;
            proveedorModificado.telefono = updateProveedorDto.telefono;
            await _proveedoresRepository.UpdateAsync(proveedorModificado);

        }
        #endregion

        #region ACTUALIZAR ProveedorArtiulo
        public async Task UpdateProveedorArticulo(ProveedorArticuloDto paDto)
        {
            var proveedorArticulo = await _proveedorArticuloRepository.GetByIdAsync(paDto.idProveedor);
            if (proveedorArticulo is null)
            {
                throw new Exception($"Proveedor Articulo con id: {paDto.idProveedor} no encontrado. ");
            }
            // Validar que el proveedor exista
            var proveedor = await _proveedoresRepository.GetByIdAsync(proveedorArticulo.proveedor!.idProveedor);
            if (proveedor == null)
            {
                throw new Exception($"Proveedor con id: {proveedorArticulo.proveedor.idProveedor} no encontrado. ");
            }
            // Validar que el articulo exista
            var articulo = await _articuloRepository.GetByIdAsync(paDto.idArticulo);
            if (articulo == null)
            {
                throw new Exception($"Artículo con id: {paDto.idArticulo} no encontrado. ");
            }

            proveedorArticulo.predeterminado = paDto.predeterminado;
            proveedorArticulo.fechaFinProveedorArticulo = paDto.fechaFinProveedorArticulo;
            proveedorArticulo.precioUnitario = paDto.precioUnitario;
            proveedorArticulo.tiempoEntregaDias = paDto.tiempoEntregaDias;
            proveedorArticulo.costoPedido = paDto.costoPedido;
            await _proveedorArticuloRepository.UpdateAsync(proveedorArticulo);
        }
        #endregion

        #region ELIMINAR PROVEEDOR
        public async Task DeleteProveedor(long idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (proveedor is null) throw new Exception($"Proveedor con ID {idProveedor} no encontrado.");

            // obtener historico de estados 
            var historialEstados = await _estProveedorRepository.GetHistorialByProveedorId(idProveedor);
            var estadoActual = historialEstados.FirstOrDefault(e => e.fechaFEstadoProveedor == null);

            if (estadoActual == null) throw new Exception("no se encontro estado vigente para este proveedor ");

            // cerrar estado actual
            estadoActual.fechaFEstadoProveedor = DateTime.UtcNow;
            await _estProveedorRepository.UpdateAsync(estadoActual);

            // validar que no sea proveedor predeterminado de ningun articulo
            var aProvArt = await _proveedorArticuloRepository.GetAllByProveedorIdAsync(idProveedor);
            if (aProvArt.Any(pPred => pPred.predeterminado)) throw new Exception("no se puede eliminar el proveedor porque es predeterminado de uno o más artículos ");

            // validar que no tenga ordenes de compra pendientes o en proceso
            var ordenesProveedor = await _ordenCompraRepository.GetAllByProveedorIdAsync(idProveedor);
            var estadosInvalidos = new[] { "Pendiente", "En proceso" };

            if (ordenesProveedor.Any(ordComp => ordComp.ordenEstado != null && estadosInvalidos.Contains(ordComp.ordenEstado.nombreEstadoOrden, StringComparer.OrdinalIgnoreCase))) throw new Exception("el proveedor tiene ordenes de compra pendientes o en proceso, no se puede dar de baja ");

            // obtener estado 'eliminado' 
            var estadoEliminado = await _proveedorEstadoRepository.GetByIdAsync(3);
            if (estadoEliminado == null) throw new Exception("no se encontro el estado 'eliminado' ");

            // registrar nuevo estado
            var nuevoEstado = new EstadoProveedores
            {
                proveedor = proveedor,
                proveedorEstado = estadoEliminado,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null
            };
            await _estProveedorRepository.AddAsync(nuevoEstado);

            // cerrar todas las relaciones proveedorArtiulo activas
            foreach (var relProvArt in aProvArt)
            {
                relProvArt.fechaFinProveedorArticulo = DateTime.UtcNow;
                await _proveedorArticuloRepository.UpdateAsync(relProvArt);
            }
            await _proveedoresRepository.UpdateAsync(proveedor);
        }
        #endregion

        #region SUSPENDER PROVEEDOR
        public async Task SuspenderProveedor(long idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (proveedor == null) throw new Exception($"proveedor con id: {idProveedor} no encontrado ");

            // obtener estado actual vigente
            var historial = await _estProveedorRepository.GetHistorialByProveedorId(idProveedor);
            var estadoActual = historial.FirstOrDefault(e => e.fechaFEstadoProveedor == null);
            if (estadoActual == null) throw new Exception("no se encontro un estado actual para el proveedor ");

            // verificar que no este previamente suspendido
            if (estadoActual.proveedorEstado?.idEstadoProveedor == 2) throw new Exception("el proveedor ya se encuentra suspendido ");

            // cerrar estado anterior
            estadoActual.fechaFEstadoProveedor = DateTime.UtcNow;
            await _estProveedorRepository.UpdateAsync(estadoActual);

            // obtener estado 'Suspendido'
            var estadoSuspendido = await _proveedorEstadoRepository.GetByIdAsync(2);
            if (estadoSuspendido == null) throw new Exception("estado 'Suspendido'  no encontrado");

            // crear nuevo estado
            var nuevoEstado = new EstadoProveedores
            {
                proveedor = proveedor,
                proveedorEstado = estadoSuspendido,
                fechaIEstadoProveedor = DateTime.UtcNow
            };
            await _estProveedorRepository.AddAsync(nuevoEstado);
            await _proveedoresRepository.UpdateAsync(proveedor);
        }
        #endregion

        #region RESTAURAR PROVEEDOR
        public async Task RestaurarProveedor(long idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(idProveedor);
            if (proveedor is null)
            {
                throw new Exception($"proveedor con id: {idProveedor} no encontrado ");
            }

            // obtener historico de estados del proveedor
            var historialEstados = await _estProveedorRepository.GetHistorialByProveedorId(idProveedor);
            var estadoActual = historialEstados.FirstOrDefault(e => e.fechaFEstadoProveedor == null);

            if (estadoActual == null)
            {
                throw new Exception("no se encontro estado vigente para este proveedor ");
            }

            // verificar si el estado actual es 'suspendido'
            if (estadoActual.proveedorEstado == null || estadoActual.proveedorEstado.idEstadoProveedor != 2)
            {
                throw new Exception("el proveedor no se encuentra en estado 'Suspendido' ");
            }

            // cerrar estado actual
            estadoActual.fechaFEstadoProveedor = DateTime.UtcNow;
            await _estProveedorRepository.UpdateAsync(estadoActual);

            // obtener estado 'activo' 
            var estadoActivo = await _proveedorEstadoRepository.GetByIdAsync(1);
            if (estadoActivo == null)
            {
                throw new Exception("no se encontro el estado 'Activo' ");
            }
            // crear nuevo estado de proveedor
            var nuevoEstado = new EstadoProveedores
            {
                proveedor = proveedor,
                proveedorEstado = estadoActivo,
                fechaIEstadoProveedor = DateTime.UtcNow,
                fechaFEstadoProveedor = null
            };

            await _estProveedorRepository.AddAsync(nuevoEstado);
        }
        #endregion

        #region ALLPROVEEDORES
        public async Task<IEnumerable<Proveedor>> GetAllProveedores()
        {
            var proveedores = await _proveedoresRepository.GetAllAsync();

            return proveedores;
        }
        #endregion

        #region PROVEEDORBYID
        public async Task<Proveedor> GetProveedorById(long idProveedor)
        {
            var proveedor = await _proveedoresRepository.GetByIdAsync(idProveedor);

            return proveedor;
        }
        #endregion
    }
}


