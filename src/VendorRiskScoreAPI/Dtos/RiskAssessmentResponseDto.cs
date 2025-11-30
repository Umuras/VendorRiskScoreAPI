namespace VendorRiskScoreAPI.Dtos
{
    public class RiskAssessmentResponseDto
    {
        public decimal RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public string Reason { get; set; }
    }
}
