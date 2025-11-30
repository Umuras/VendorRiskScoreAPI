# VendorRiskScoreAPI

VendorRiskScoreAPI, tedarikçi risk skorlarını hesaplayan ve yöneten bir ASP.NET Core Web API projesidir.  

---

## Kurulum ve Çalıştırma

1.) Repo’yu klonlayın:  (Github Desktop ya da Sourcetree kullanmanız tavsiye edilir)
```bash
git clone https://github.com/Umuras/VendorRiskScoreAPI.git
cd VendorRiskScoreAPI

2.) NuGet paketlerini yükleyin:
dotnet restore

3.) PostgreSQl Ayarı
appsettings.json içindeki PostgreSQL ayarlarını ayarlayın ve  connection string’i güncelleyin.

4.) Veritabanını oluşturun ve migration’ları uygulayın:
Add-Migration InitialCreate
Update-Database

5.) API’yi çalıştırın:
dotnet run
2.seçenek ise terminal kullanmadan direk kullandığınız ide üzerinden de çalıştırabilirsiniz.

6.) docker-compose.yaml ayağa kaldırmak için gerekli adımlar:
- Terminal üzerinden docker-compose.yaml dosyasının olduğunu konuma gidilir. Kullandığınız IDE'nin terminalini kullanmak konuma erişme
rahatlığı açısından önerilir.
- Docker Compose ile container’ları başlat
    a.) docker-compose up -d
        up → Container’ları başlatır ve gerekirse image’ları çeker.
        -d → Detach mode, terminali meşgul etmeden arka planda çalıştırır.

- Container'ların ayağa kalkıp kalkmadığını görmek için:
    - docker-compose ps 
    Eğer State Up yazıyorsa çalışıyor demektir.

- Logları görmek
    docker-compose logs -f elasticsearch
    docker-compose logs -f kibana
    docker-compose logs -f logstash

- Container’ları durdurmak

- İşin bittiğinde veya yeniden başlatmak istediğinde:

    docker-compose down

- Erişim
    Elasticsearch: http://localhost:9200
    Kibana: http://localhost:5601
    Logstash için pipeline’lar ./logstash/pipeline klasöründen okunuyor.

Örnek Veri / Dataset

VendorProfile eklemek için örnek JSON:
{
  "name": "TechPlus Solutions",
  "financialHealth": 90,
  "slaUptime": 96,
  "majorIncidents": 0,
  "securityCerts": [
    "ISO27001"
  ],
  "documents": {
    "contractValid": true,
    "privacyPolicyValid": true,
    "pentestReportValid": true
  }
}

API Örnek İstekler

Get Vendor Profiles
GET /api/vendor/allprofiles


[
  {
    "id": 1,
    "name": "TechPlus Solutions",
    "financialHealth": 90,
    "slaUptime": 96,
    "majorIncidents": 0,
    "securityCerts": ["ISO27001"],
    "documents": {
      "contractValid": true,
      "privacyPolicyValid": true,
      "pentestReportValid": true
    }
  },
  {
    "id": 2,
    "name": "NovaLog Logistics",
    "financialHealth": 55,
    "slaUptime": 88,
    "majorIncidents": 3,
    "securityCerts": [],
    "documents": {
      "contractValid": true,
      "privacyPolicyValid": true,
      "pentestReportValid": false
    }
  }
]

Get Vendor Profile
GET /api/vendor/{id}

GET /api/vendor/1

  {
    "id": 1,
    "name": "TechPlus Solutions",
    "financialHealth": 90,
    "slaUptime": 96,
    "majorIncidents": 0,
    "securityCerts": ["ISO27001"],
    "documents": {
      "contractValid": true,
      "privacyPolicyValid": true,
      "pentestReportValid": true
    }
  }

PUT Vendor Profile
PUT /api/vendor/1

{
  "name": "",
  "financialHealth": 0,
  "slaUptime": 0,
  "majorIncidents": 0,
  "securityCerts": ["string"],
  "documents": {
    "contractValid": true,
    "privacyPolicyValid": true,
    "pentestReportValid": true
  }
}

Response: VendorProfile updated successfully

POST Vendor Profile
POST /api/vendor

{
  "name": "TechPlus Solutions",
  "financialHealth": 45,
  "slaUptime": 65,
  "majorIncidents": 2,
  "securityCerts": ["SOC2"],
  "documents": {
    "contractValid": true,
    "privacyPolicyValid": false,
    "pentestReportValid": false
  }
}

Response: {
  "riskScore": 0.755,
  "riskLevel": "High",
  "reason": "Missing ISO27001 + Missing PCI_DSS + Privacy Policy expired + Pentest Report Missing + SLA < 95%"
}


DELETE Vendor Profile
DELETE /api/vendor/{id}

DELETE /api/vendor/3

Response: 204

GET Vendor Profile Risk Result 
GET /api/vendor/{vendorId}/risk

GET /api/vendor/3/risk
Response:
{
  "riskScore": 0.755,
  "riskLevel": "High",
  "reason": "Missing ISO27001 + Missing PCI_DSS + Privacy Policy expired + Pentest Report Missing + SLA < 95%"
}

GET Vendor Profile Risk Scores
GET /api/vendor/{vendorId}/riskscores

GET /api/vendor/3/riskscores

Response:
{
  "vendor": "TechPlus Solutions",
  "financial": 0.32,
  "operational": 0.195,
  "security": 0.24,
  "finalScore": 0.755
}

Validasyonlar(VendorProfile için):
- Name propertysi POST ve PUT işleminde boş bırakılamaz; 3 harften az ve 25 harften fazla olamaz.
- FinancialHealth ve SlaUptime POST ve PUT işleminde 0 ile 100 arasında değer girilebilir.
- MajorIncidents POST ve PUT işleminde 0 ile 100 arasında değer girilebilir.
- SecurityCerts POST ve PUT işleminde boş string veya hiçbir değer girilmeden geçilebilir, aynı sertifikadan iki kere girilirse 
sunucu kendisi onu teke düşürür, sadece ISO27001, SOC2, PCI_DSS sertifikaları girilebilir, bunlardan farklı girilirse hata verir, 
en fazla 3 tane sertifika girilebilir.

Ek Bilgiler

- Risk hesaplamaları RiskAssessmentService ve RuleEngineService üzerinden yapılır.
- VendorProfileRiskScore entity’si, finansal, operasyonel ve güvenlik risk skorlarını tutar.
- API, CRUD işlemleri için transaction ve veri bütünlüğü kontrolleri sağlar.
- Response DTO’lar ve Request DTO’lar ayrı tutulur; örneğin VendorProfileResponseDto, request sırasında id gönderilemeyeceği için kullanılır.




