namespace VendorRiskScoreAPI.Dtos
{
    public class DocumentDto
    {
        public bool ContractValid { get; set; }
        public bool PrivacyPolicyValid { get; set; }
        public bool PentestReportValid { get; set; }
    }
}
