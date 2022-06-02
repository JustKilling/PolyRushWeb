using System.ComponentModel.DataAnnotations;

namespace PolyRushWeb.Helper
{
    //https://stackoverflow.com/questions/64518949/data-annotation-for-iformfile-so-it-only-allows-files-with-png-jpg-or-jpeg-e
    //this is a data annotation attribute to make sure the filepicker expects given extensions
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        //constructor that injects the dependencies
        public AllowedExtensionsAttribute(string[] extensions )
        {
            _extensions = extensions;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //check if file extention is in the given extensions
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
            string extensions = "";
            foreach (var extension in _extensions)
            {
                extensions += extension + "";
            }
            return $"Your filetype is not valid. ({extensions})";
        }
    }
}
