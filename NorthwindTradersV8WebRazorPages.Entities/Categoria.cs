namespace NorthwindTradersV8WebRazorPages.Entities
{
    public class Categoria
    {
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
        public byte[]? RowVersion { get; set; }

        public string RowVersionStr
        {
            get
            {
                if (RowVersion == null || RowVersion.Length < 8)
                    return string.Empty;

                return BitConverter.ToInt64(RowVersion, 0).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RowVersion = null;
                    return;
                }

                RowVersion = BitConverter.GetBytes(long.Parse(value));
            }
        }

        // del diagrama entidad-relacion podemos ver que
        // una categoria tiene muchos productos asociados
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
