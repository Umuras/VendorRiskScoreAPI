namespace VendorRiskScoreAPI.Dtos
{
    public class VendorProfileRiskScoreResponseDto
    {
        public string Vendor { get; set; }
        public double Financial { get; set; }
        public double Operational { get; set; }
        public double Security { get; set; }
        public double FinalScore { get; set; }
    }
}
