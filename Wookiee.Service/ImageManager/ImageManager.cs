using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Wookiee.Model.Entities;

namespace Wookiee.Service.ImageManager;

public interface IAmImageManager
{
    Task<Image> UploadImage(IFormFile image);
    Task<string> LoadImage(Guid imageId, string fileExtension);
}

public class ImageManager : IAmImageManager
{

    private readonly IWebHostEnvironment _environment;
    private const string Directory = "\\images";

    public ImageManager(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<Image> UploadImage(IFormFile image)
    {
        var result = await UploadImageToPath(image);
        return result;
    }

    public async Task<string> LoadImage(Guid imageId, string fileExtension)
    {
        var directoryPath = Path.Combine(_environment.ContentRootPath + Directory);
        var imagePath = Path.Combine(directoryPath, imageId.ToString());
        var imageAsBytes = await File.ReadAllBytesAsync(imagePath);
        var imageAsBase64 =  Convert.ToBase64String(imageAsBytes);
        return $"data:image/{fileExtension};base64,{imageAsBase64}>";
    }

    private async Task<Image> UploadImageToPath(IFormFile image)
    {
        
        var fileName = Guid.NewGuid();
        var fullPath = Path.Combine(_environment.ContentRootPath + Directory);
        if (!System.IO.Directory.Exists(fullPath))
            EnsureCreated(fullPath);

        var fullFileExtension = Path.GetExtension(image.FileName);
        var fileExtension = fullFileExtension.Substring(1);
        var filePath = Path.Combine(fullPath, fileName.ToString());
        await using Stream fileStream = new FileStream(filePath, FileMode.Create);
        await image.CopyToAsync(fileStream);
        return MapToImageModel(fileName, fileExtension);
    }

    private static Image MapToImageModel(Guid fileName, string fileExtension) => new() { ImageId = fileName, Extension = fileExtension };

    private static void EnsureCreated(string directory) => 
        System.IO.Directory.CreateDirectory(directory);
}