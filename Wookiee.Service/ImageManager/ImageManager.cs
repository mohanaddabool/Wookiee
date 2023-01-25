using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Wookiee.Model.Entities;

namespace Wookiee.Service.ImageManager;

#region interface

public interface IAmImageManager
{
    Task<Image> UploadImage(IFormFile image);
    Task<string> LoadImageAsString(Guid imageId, string fileExtension);
    Task<byte[]> LoadImageAsBytes(Guid imageId);
    void DeleteImage(Guid imageId);
}

#endregion

#region implementation

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

    public async Task<string> LoadImageAsString(Guid imageId, string fileExtension)
    {
        var imagePath = LoadImagePath(imageId);
        var imageAsBytes = await File.ReadAllBytesAsync(imagePath);
        var imageAsBase64 = Convert.ToBase64String(imageAsBytes);
        return $"data:image/{fileExtension};base64,{imageAsBase64}";
    }

    public async Task<byte[]> LoadImageAsBytes(Guid imageId)
    {
        var imagePath = LoadImagePath(imageId);
        return await File.ReadAllBytesAsync(imagePath);
    }

    public void DeleteImage(Guid imageId)
    {
        var imagePath = LoadImagePath(imageId);
        File.Delete(imagePath);
    }

    #endregion
    
    #region private methods

    private string LoadImagePath(Guid imageId)
    {
        var directoryPath = Path.Combine(_environment.ContentRootPath + Directory);
        var imagePath = Path.Combine(directoryPath, imageId.ToString());
        return imagePath;
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
        return MapToImageModel(fileName, fileExtension.ToLower());
    }

    private static Image MapToImageModel(Guid fileName, string fileExtension) => new() { ImageId = fileName, Extension = fileExtension };

    private static void EnsureCreated(string directory) =>
        System.IO.Directory.CreateDirectory(directory);

    #endregion

}