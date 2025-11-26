namespace VendorRiskScoreAPI.Dtos
{
    public class DocumentRequestDto
    {
        public bool ContractValid { get; set; }
        public bool PrivacyPolicyValid { get; set; }
        public bool PentestReportValid { get; set; }
    }
}
