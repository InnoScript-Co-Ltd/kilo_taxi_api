namespace KiloTaxi.API.Helper.FileHelpers
{
    public class FileUploadHelper
    {
        private readonly List<string> _allowedExtensions;
        private readonly List<string> _allowedMimeTypes;
        private readonly long _maxFileSize;
        private readonly IConfiguration _configuration;

        public FileUploadHelper(IConfiguration configuration, List<string> allowedExtensions, List<string> allowedMimeTypes, long maxFileSize)
        {
            _configuration = configuration;
            _allowedExtensions = allowedExtensions;
            _allowedMimeTypes = allowedMimeTypes;
            _maxFileSize = maxFileSize;
        }

        public bool ValidateFile(IFormFile file, bool allowEmptyFile, string directoryName, out string resolvedFilePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Set resolvedFilePath to "directoryName/default.png" if file is null and allowEmptyFile is true
            if (file == null || file.Length == 0)
            {
                if (allowEmptyFile)
                {
                    resolvedFilePath = Path.Combine(directoryName, "default.png").Replace('\\', '/');
                    resolvedFilePath = $"{directoryName}/default.png";
                    return true;
                }
                else
                {
                    errorMessage = "File is empty or not provided.";
                    resolvedFilePath = string.Empty;
                    return false;
                }
            }

            // File is not null, proceed with validation
            resolvedFilePath = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(resolvedFilePath))
            {
                errorMessage = "Only image files (.jpg, .jpeg, .png) are allowed.";
                return false;
            }

            if (!_allowedMimeTypes.Contains(file.ContentType.ToLower()))
            {
                errorMessage = "Only image files (JPEG, PNG) are allowed.";
                return false;
            }

            if (file.Length > _maxFileSize)
            {
                errorMessage = $"File size exceeds the {_maxFileSize / (1024 * 1024)} MB limit.";
                return false;
            }

            return true;
        }


        public async Task<string> SaveFileAsync(IFormFile file, string directoryName, string fileNamePrefix, string fileExtension)
        {
            var uploadDirectory = Path.Combine(_configuration["MediaFilePath"], directoryName).Replace('\\', '/');

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            var filePath = Path.Combine(uploadDirectory, $"{fileNamePrefix}{fileExtension}").Replace('\\', '/');

            // Wrap the FileStream in a using statement to ensure it is disposed
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

    }
}
