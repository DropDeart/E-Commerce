using Ljuss.Applications.DTOs;

namespace LjussAPI.Helper
{
    public class ImageHelper
    {
        public void ValidateFileUpload(ImageUploadRequestDto imageUpload)
        {
            var allowedExtension = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtension.Contains(Path.GetExtension(imageUpload.File.FileName)))
            {
                throw new Exception("Desteklenmeyen dosya biçimi !");
            }

            if (imageUpload.File.Length > 10485760)
            {
                throw new Exception("Dosya boyutu 10MB'dan büyük olamaz !");
            }
        }
    }
}
