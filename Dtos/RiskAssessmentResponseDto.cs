namespace VendorRiskScoreAPI.Dtos
{
    public class RiskAssessmentResponseDto
    {
        public double RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public string Reason { get; set; }
    }
}
