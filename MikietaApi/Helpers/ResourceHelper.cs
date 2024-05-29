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

        public static byte[] GetImage(string name)
        {
            return File.ReadAllBytes(Path.Combine(ImagesPath, $"{name}.png"));
        }
    }
}