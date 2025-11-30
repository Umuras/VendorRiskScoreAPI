using VendorRiskScoreAPI.Domain.Entities;
using VendorRiskScoreAPI.Enums;

namespace VendorRiskScoreAPI.Services
{
    public class RuleEngineService : IRuleEngineService
    {
        public decimal CalculateFinancialRisk(int financialHealth)
        {
            if(financialHealth < 50)
            {
                return 0.8m; //High risk score
            }

            if(financialHealth > 80)
            {
                return 0.2m; //Low risk score
            }

            return 0.5m; //Medium risk score
        }

        public decimal CalculateOperationalRisk(int slaUpTime, int majorIncidents)
        {
            decimal slaUpTimeScore = 0;
            decimal majorIncidentsScore = 0;

            if(slaUpTime >= 95)
            {
                slaUpTimeScore = 0.2m;
            }else if(slaUpTime >= 90 && slaUpTime < 95)
            {
                slaUpTimeScore = 0.5m;
            }else if(slaUpTime < 90)
            {
                slaUpTimeScore = 0.8m;
            }

            if(majorIncidents >= 0 &&  majorIncidents <= 1)
            {
                majorIncidentsScore = 0.2m;
            }else if(majorIncidents <= 3)
            {
                majorIncidentsScore = 0.5m;
            }
            else
            {
                majorIncidentsScore = 0.8m;
            }

            decimal sumScore = slaUpTimeScore + majorIncidentsScore;
            decimal avgScore = (sumScore / 2);

            return avgScore;
        }

        public decimal CalculateSecurityComplianceRisk(List<VendorSecurityCert> vendorSecurityCerts, Document document)
        {
            decimal securityCertRiskScore = 0.0m;
            bool hasISO27001 = false;
            bool hasSOC2 = false;
            bool hasPCI_DSS = false;

            decimal documentRiskScore = 0.0m;

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
                securityCertRiskScore += 0.2m;
            }
            if(!hasSOC2)
            {
                securityCertRiskScore += 0.2m;
            }
            if(!hasPCI_DSS)
            {
                securityCertRiskScore += 0.2m;
            }

            if(!document.ContractValid)
            {
                documentRiskScore += 0.2m;
            }
            if (!document.PrivacyPolicyValid)
            {
                documentRiskScore += 0.2m;
            }
            if(!document.PentestReportValid)
            {
                documentRiskScore += 0.2m;
            }

            decimal sumFinalSecurityComplianceScore = securityCertRiskScore + documentRiskScore;

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
