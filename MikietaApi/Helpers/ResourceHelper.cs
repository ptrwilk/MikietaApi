namespace MikietaApi.Helpers
{
    public static class ResourceHelper
    {
        public static string ImagesPath
        {
            get
            {
                var resourcesPath =
                    Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "Resources");

                return Path.Combine(resourcesPath, "Images");
            }
        }

        public static byte[]? GetImage(string name)
        {
            var file = Path.Combine(ImagesPath, $"{name}.png");
            
            return File.Exists(file) ? File.ReadAllBytes(file) : null;
        }
    }
}