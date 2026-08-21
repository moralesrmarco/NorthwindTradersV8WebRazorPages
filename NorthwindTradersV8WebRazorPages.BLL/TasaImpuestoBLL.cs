using NorthwindTradersV8WebRazorPages.DAL;

namespace NorthwindTradersV8WebRazorPages.BLL
{
    public class TasaImpuestoBLL
    {
        private readonly TasaImpuestoDAL tasaImpuestoDAL;
        public TasaImpuestoBLL(string connectionString)
        {
            this.tasaImpuestoDAL = new TasaImpuestoDAL(connectionString);
        }
        public decimal? ObtenerTasaVigente(DateTime fecha)
        {
            return tasaImpuestoDAL.ObtenerTasaVigente(fecha);
        }
    }
}
