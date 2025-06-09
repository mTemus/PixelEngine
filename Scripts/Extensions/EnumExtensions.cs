namespace PixelEngine.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this System.Enum value)
        {
            return System.Convert.ToInt32(value);
        }
    }
}
