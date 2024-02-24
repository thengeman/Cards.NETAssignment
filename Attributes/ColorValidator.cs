using System.ComponentModel.DataAnnotations;

namespace Cards.Attributes
{
    public class ColorValidator : ValidationAttribute
    {
        public ColorValidator()
            :base("Color must start with '#' and length of 6")
        { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var strValue = value as string;

            if (!string.IsNullOrEmpty(strValue))
            {
                if (!strValue.StartsWith("#") || strValue.Length != 6) 
                {
                    return new ValidationResult("Color must start with '#' and length of 6");
                }
            }

            return ValidationResult.Success;

        }
    }
}
