using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class ProveedorDAL
    {
        private readonly string connectionString;
        public ProveedorDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
