namespace Proyect_InvOperativa.Services
{
    public class VentasService
    {
        public async Task<string> getMensajePrueba (string mensajePrueba1)
        {
            string mensajesPrueba2 = $"Hola {mensajePrueba1} Mundo";
            return mensajesPrueba2;
        }
    }
}
