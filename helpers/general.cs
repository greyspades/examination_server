using Microsoft.AspNetCore.StaticFiles;

namespace Helpers.General;

public class File {
    public static string GetContentType(string filePath)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream"; // Default content type if not found
        }
        return contentType;
}
}
