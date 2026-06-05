namespace NorthwindTradersV8WebRazorPages.DAL.Helpers
{
    public static class PhotoHelper
    {
        public static byte[]? StripOleHeader(byte[] oleBytes, int employeeId)
        {
            if (employeeId <= 9 && oleBytes != null && oleBytes.Length > 78)
            {
                // Quita los primeros 78 bytes
                var newBytes = new byte[oleBytes.Length - 78];
                Buffer.BlockCopy(oleBytes, 78, newBytes, 0, newBytes.Length);
                return newBytes;
            }
            return oleBytes;
        }
    }
}
