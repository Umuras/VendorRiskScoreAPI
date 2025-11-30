namespace VendorRiskScoreAPI.Exceptions
{
    public class DuplicateVendorProfileNameException : Exception
    {
        public DuplicateVendorProfileNameException(string name) : base($"A VendorProfile with the name '{name}' already exists.")
        {

        }
    }
}
