using beaconinteriorsapi.Models;
using static beaconinteriorsapi.Exceptions.ExceptionHelpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace beaconinteriorsapi.Services
{
    public interface IFileService {
         Task<List<Image>> UploadFiles(List<IFormFile> files);
        Task<bool> DeleteFiles(List<string> files);
    }
    public class UploadFileParams : ImageUploadParams
    {
        public UploadFileParams(string fileName,Stream file)
        {
            UploadPreset = "beaconinteriors";
            File = new FileDescription(fileName,file);
        }
    }

    public class FileService:IFileService
    {
        private readonly Cloudinary _cloudinary;
       
        public FileService()
        {
            string? name = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
            string? apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");
            string? apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");

            if (string.IsNullOrEmpty(name)| string.IsNullOrEmpty(apiKey)| string.IsNullOrEmpty(apiSecret)) throw new Exception("cloudinary url is not set in env");
            var account=new Account(name, apiKey, apiSecret );
            _cloudinary = new Cloudinary(account);
            }

        public async Task<List<Image>> UploadFiles(List<IFormFile> files) {
            try
            {
                List<Image> result = new();
                foreach (IFormFile file in files)
                {
                    using Stream stream = file.OpenReadStream();
                    var uploadFileParams = new UploadFileParams(file.FileName, stream);

                    ImageUploadResult uploadedImage = await _cloudinary.UploadAsync(uploadFileParams);
                    if (uploadedImage.Error==null && !string.IsNullOrEmpty(uploadedImage.PublicId))
                    {
                        result.Add(new Image(uploadedImage.SecureUrl.ToString(),uploadedImage.PublicId));
                    }
                    else
                    {
                        // Optionally handle failed uploads here and roll back successful uploads if needed.
                        throw new Exception();
                    }
                }
                return result;
            }
            catch (Exception)
            {
                ThrowServerError("unable to upload files at the moment");
                throw;
            }
            
            
        }
        public async Task<bool> DeleteFiles(List<string> publicIds)
        {
            foreach (string publicId in publicIds)
            {
                var deleteParams = new DeletionParams(publicId);

                DeletionResult result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result != "ok")
                {
                    // handle failed deletes here if needed.
                    ThrowServerError($"unable to delete some images at the moment try again later.result:{result.Result}");
                }
            }
            return true;
        }
    }
}
