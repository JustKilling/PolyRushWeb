using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Helper
{
    //https://stackoverflow.com/questions/64518949/data-annotation-for-iformfile-so-it-only-allows-files-with-png-jpg-or-jpeg-e
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;
            if (file == null) return ValidationResult.Success;
            string? extension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Your image's filetype is not valid.";
        }
    }
}
