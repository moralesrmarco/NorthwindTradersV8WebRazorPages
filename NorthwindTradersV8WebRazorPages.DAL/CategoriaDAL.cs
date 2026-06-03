using Microsoft.Data.SqlClient;
using NorthwindTradersV8WebRazorPages.Entities;

namespace NorthwindTradersV8WebRazorPages.DAL
{
    public class CategoriaDAL
    {
        private readonly string connectionString;
        public CategoriaDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
