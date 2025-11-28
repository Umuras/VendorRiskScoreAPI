using System.Net;
using System.Text.Json;

namespace VendorRiskScoreAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        // Sonraki middleware'e/endpoint'e geçişi temsil eden delege.
        private readonly RequestDelegate _next;
        // Hataları loglamak için .NET'in dahili logger'ı.
        private readonly ILogger<ExceptionMiddleware> _logger;

        // DI (Dependency Injection) ile next ve logger geliyor.
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next; // Bir sonraki middleware'e referans
            _logger = logger;  // Log yazmak için logger
        }

        // Her HTTP isteği için çağrılan giriş noktası.
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Try: İsteği bir sonraki middleware'e/endpoint'e ilet.
                // Burada bir exception fırlarsa catch bloğuna düşecek.
                await _next(context);
            }
            catch (Exception ex)
            {
                // Catch: Pipeline içinde herhangi bir yerde yakalanmayan hata buraya düşer.
                // Hatanın detayını logla (stack trace dahil).
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);

                // İstemciye döneceğimiz standartlaştırılmış JSON hata cevabını hazırla/gönder.
                await HandleExceptionAsync(context, ex);
            }
        }

        // Yakalanan exception'a göre HTTP status code belirler ve JSON response yazar.
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Dönen içeriğin tipi JSON olacak.
            context.Response.ContentType = "application/json";

            /*
             switch expression

            Klasik switch yapısının daha kısa ve fonksiyonel bir versiyonudur.
            Normal switch:

            HttpStatusCode statusCode;

            switch (ex)
            {
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;
            case ArgumentNullException:
                statusCode = HttpStatusCode.BadRequest;
                break;
            default:
                statusCode = HttpStatusCode.InternalServerError;
                break;
}
            Yeni sürümle (switch expression) bunu tek satırda yazabiliyoruz:
            Buradaki ex switch { ... } ifadesi şunu yapıyor:

            ex (Exception) nesnesinin türüne bakıyor,

            eşleşen tür bulunursa karşısındaki değeri döndürüyor,

            _ (underscore) ise default (diğer tüm durumlar) anlamına geliyor.

            Yani aslında bu bir değer döndüren switch yapısı.
             */


            // Hata tipine göre uygun HTTP status code seçimi.
            // İstersen burayı kendi özel exception tiplerinle genişletebilirsin.
            HttpStatusCode statusCode = ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound, //404
                ArgumentNullException => HttpStatusCode.BadRequest, //400
                InvalidOperationException => HttpStatusCode.Conflict, //409
                _ => HttpStatusCode.InternalServerError //500(genel hata)
            };


            /*
             Anonymous Object Initializer (anonim nesne oluşturma)

             Bu kısımda ise sınıf tanımlamadan hızlıca bir JSON benzeri nesne oluşturuyoruz.

             Bu kod, runtime’da şu şekil bir anonim tip oluşturuyor:
             new { int statusCode; string message; string errorType; }

             Bu tipin adı yok (anonymous type), ama JsonSerializer.Serialize() kullanarak doğrudan JSON’a çevirebiliyoruz.
             Bu da C#’ta API’lerden sade ve geçici JSON response dönmek için çok sık kullanılan bir yöntemdir.

             // anonim tip yerine normal bir model sınıfı tanımlamamız gerekirdi:
              var response = new ErrorResponse
              {
                StatusCode = (int)statusCode,
                Message = ex.Message,
                ErrorType = ex.GetType().Name
              };

             Ve tabii bu durumda yukarıda bir model tanımlamamız gerekir:
            public class ErrorResponse
            {
                public int StatusCode { get; set; }
                public string Message { get; set; }
                public string ErrorType { get; set; }
            }


             */
            // İstemciye göndereceğimiz gövde (payload).
            var response = new
            {
                statusCode = (int)statusCode, // Sayısal durum kodu (örn. 404)
                message = ex.Message, // Hata mesajı
                errorType = ex.GetType().Name //Exception sınıf adı(örn. KeyNotFoundException)
            };

            // HTTP durum kodunu yanıt üzerine yaz.
            context.Response.StatusCode = (int)statusCode;
            // Nesneyi JSON'a serileştir.
            var json = JsonSerializer.Serialize(response);
            // JSON'ı HTTP response body'ye yaz.
            await context.Response.WriteAsync(json);
        }
    }
}
