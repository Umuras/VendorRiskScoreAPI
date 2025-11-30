namespace VendorRiskScoreAPI.Domain.ValueObjects
{
    public class RiskScores
    {
        public decimal Financial { get; set; }
        public decimal Operational { get; set; }
        public decimal Security { get; set; }
        public decimal FinalScore { get; set; }
    }
}
