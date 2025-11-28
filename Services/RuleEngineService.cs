using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Enums;

namespace VendorRiskScoreAPI.Services
{
    public class RuleEngineService : IRuleEngineService
    {
        public float CalculateFinancialRisk(int financialHealth)
        {
            if(financialHealth < 50)
            {
                return 0.8f; //High risk score
            }

            if(financialHealth > 80)
            {
                return 0.2f; //Low risk score
            }

            return 0.5f; //Medium risk score
        }

        public float CalculateOperationalRisk(int slaUpTime, int majorIncidents)
        {
            float slaUpTimeScore = 0;
            float majorIncidentsScore = 0;

            if(slaUpTime >= 95)
            {
                slaUpTimeScore = 0.2f;
            }else if(slaUpTime >= 90 && slaUpTime < 95)
            {
                slaUpTimeScore = 0.5f;
            }else if(slaUpTime < 90)
            {
                slaUpTimeScore = 0.8f;
            }

            if(majorIncidents >= 0 &&  majorIncidents <= 1)
            {
                majorIncidentsScore = 0.2f;
            }else if(majorIncidents <= 3)
            {
                majorIncidentsScore = 0.5f;
            }
            else
            {
                majorIncidentsScore = 0.8f;
            }

            float sumScore = slaUpTimeScore + majorIncidentsScore;
            float avgScore = (sumScore / 2);

            return avgScore;
        }

        public float CalculateSecurityComplianceRisk(List<VendorSecurityCert> vendorSecurityCerts, Document document)
        {
            float securityCertRiskScore = 0.0f;
            bool hasISO27001 = false;
            bool hasSOC2 = false;
            bool hasPCI_DSS = false;

            float documentRiskScore = 0.0f;

            if (vendorSecurityCerts.Count > 0)
            {
                vendorSecurityCerts.ForEach(item =>
                {
                    if(item.CertName == SecurityCertificates.ISO27001.ToString())
                    {
                        hasISO27001 = true;
                    }
                    else if(item.CertName == SecurityCertificates.SOC2.ToString())
                    {
                        hasSOC2 = true;
                    }
                    else if(item.CertName == SecurityCertificates.PCI_DSS.ToString())
                    {
                        hasPCI_DSS = true;
                    }
                });
            }

            if(!hasISO27001)
            {
                securityCertRiskScore += 0.2f;
            }
            if(!hasSOC2)
            {
                securityCertRiskScore += 0.2f;
            }
            if(!hasPCI_DSS)
            {
                securityCertRiskScore += 0.2f;
            }

            if(!document.ContractValid)
            {
                documentRiskScore += 0.2f;
            }
            if (!document.PrivacyPolicyValid)
            {
                documentRiskScore += 0.2f;
            }
            if(!document.PentestReportValid)
            {
                documentRiskScore += 0.2f;
            }

            float sumFinalSecurityComplianceScore = securityCertRiskScore + documentRiskScore;

            if(sumFinalSecurityComplianceScore > 1)
            {
                sumFinalSecurityComplianceScore = 1;
            }

            return sumFinalSecurityComplianceScore;
        }

        public Task<string> GetReasons(int slaUpTime, List<VendorSecurityCert> vendorSecurityCerts, Document document)
        {
            List<string> reasons = new();
            string reason = "";

            bool hasISO27001 = false;
            bool hasSOC2 = false;
            bool hasPCI_DSS = false;

            
            if (vendorSecurityCerts.Count > 0)
            {
                vendorSecurityCerts.ForEach(item =>
                {
                    if (item.CertName == SecurityCertificates.ISO27001.ToString())
                    {
                        hasISO27001 = true;
                    }
                    else if (item.CertName == SecurityCertificates.SOC2.ToString())
                    {
                        hasSOC2 = true;
                    }
                    else if (item.CertName == SecurityCertificates.PCI_DSS.ToString())
                    {
                        hasPCI_DSS = true;
                    }
                });
            }

            if (!hasISO27001)
            {
                 reasons.Add($"Missing {SecurityCertificates.ISO27001.ToString()}");
            }
            if (!hasSOC2)
            {
                reasons.Add($"Missing {SecurityCertificates.SOC2.ToString()}");
            }
            if (!hasPCI_DSS)
            {
                reasons.Add($"Missing {SecurityCertificates.PCI_DSS.ToString()}");
            }

            if (!document.ContractValid)
            {
                reasons.Add("Missing Contract");
            }
            if (!document.PrivacyPolicyValid)
            {
                reasons.Add("Privacy Policy expired");
            }
            if (!document.PentestReportValid)
            {
                reasons.Add("Pentest Report Missing");
            }

            if (slaUpTime < 95)
            {
                reasons.Add("SLA < 95%");
            }

            if(!reasons.Any())
            {
                reasons.Add("All compliance requirements met");
                reason = reasons[0];
                return Task.FromResult(reason);
            }

            if (reasons.Count > 0)
            {
                reason = string.Join(" + ", reasons);
            }

            return Task.FromResult(reason);
        }
    }
}
