using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BigDataOrdersDashboard.ViewComponents.CustomerDetailViewComponents
{
    public class _CustomerDetailAIAnalyzeByLastOrdersComponentPartial : ViewComponent
    {
        private readonly BigDataOrdersDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public _CustomerDetailAIAnalyzeByLastOrdersComponentPartial(BigDataOrdersDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            // Hocanın kodundaki test id değeri
            id = 8;

            // 1. ADIM: Veritabanından Müşteri ve Son 20 Sipariş Bilgilerinin Çekilmesi
            var customer = _context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.CustomerId == id)
                .Select(c => new
                {
                    c.CustomerName,
                    c.CustomerSurname,
                    Orders = c.Orders.OrderByDescending(o => o.OrderDate)
                    .Take(20)
                    .Select(o => new
                    {
                        o.OrderDate,
                        Product = o.Product.ProductName,
                        Category = o.Product.Category.CategoryName,
                        o.Quantity,
                        o.Product.UnitPrice,
                        TotalPrice = o.Quantity * o.Product.UnitPrice
                    })
                }).FirstOrDefault();

            var jsonData = JsonSerializer.Serialize(customer);

            // 2. ADIM: Yapay Zekaya Gönderilecek Prompt Şablonu
            string prompt = $@"
⚠️ Çok önemli:
Kesinlikle ``` (backtick) veya kod bloğu verme.
Sadece saf HTML üret. Markdown verme. Kod bloğu verme.

Sen bir veri analisti ve müşteri davranış uzmanısın.
Aşağıdaki veriyi analiz et ve sonucu HTML formatında ver.

Bu başlıkları kullan (sırasını ve isimleri değiştirme):

<h4>👤 Müşteri Profili</h4>
<p><b>Ad:</b> ...</p>
<p><b>Soyad:</b> ...</p>
<p><b>Toplam Sipariş:</b> ...</p>
<p><b>Toplam Harcama:</b> ...</p>

<h4>🛍️ Ürün Tercihleri</h4>
<ul>
  <li>🏠 Ev & Dekorasyon – X sipariş</li>
  <li>💄 Kozmetik – X sipariş</li>
</ul>
<p><b>Öne çıkan ürünler:</b></p>
<ul>
  <li>Ürün adı (adet — fiyat)</li>
</ul>

<h4>⏰ Zaman Bazlı Alışveriş Davranışı</h4>
<p>En yoğun ay: ...</p>
<p>En yoğun gün: ...</p>
<p>Favori saat aralığı: ...</p>

<h4>💰 Ortalama Harcama ve Sıklık</h4>
<p>Aylık ortalama sipariş: ...</p>
<p>Ortalama sepet tutarı: ...</p>
<p>En yüksek sipariş: ...</p>
<p>En düşük sipariş: ...</p>

<h4>🎯 Sadakat ve Tekrar Harcama Eğilimi</h4>
<p>Tekrar alışveriş eğilimi: ...</p>
<p>Marka sadakati: ...</p>
<p>Kategori sadakati: ...</p>

<h4>🚀 Pazarlama Önerileri</h4>
<ul>
  <li>🎁 Kampanya önerisi: ...</li>
  <li>✉️ Hedefli e-posta: ...</li>
  <li>🆕 Yeni ürün tanıtımı önerisi: ...</li>
</ul>

Veri:
{jsonData}";

            // 3. ADIM: OpenRouter API İstemcisinin Hazırlanması
            var httpClient = _httpClientFactory.CreateClient();

            // Kimlik Doğrulama (Token) Tanımı
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Apı_Key");

            // OpenRouter platformu için zorunlu/önerilen başlıklar
            httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost:7057");
            httpClient.DefaultRequestHeaders.Add("X-Title", "Big Data Analyze");

            // Gönderilecek JSON paket gövdesi (Request Body)
            var requestBody = new
            {
                model = "google/gemini-2.5-flash", // OpenRouter üzerindeki güncel ücretsiz model
                messages = new[]
                {
                    new { role = "system", content = "You are a senior marketing data analyzer." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.5,
                max_tokens = 1000
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // 4. ADIM: İsteğin Gönderilmesi ve Yanıtın Alınması (Hatalı/Çift satırlar temizlendi)
            // Temiz, köşeli parantezsiz ve link eklemesi olmayan saf adres satırı:
            var response = await httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // API düzeyinde bir hata olup olmadığının kontrolü
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Analyze = $"API Hatası Oluştu! Durum Kodu: {response.StatusCode} - Detay: {responseString}";
                return View();
            }

            // 5. ADIM: Gelen JSON Yanıtının Parçalanması ve Arayüze (View) Dağıtılması
            var doc = JsonDocument.Parse(responseString);

            if (doc.RootElement.TryGetProperty("choices", out var choicesProp) && choicesProp.GetArrayLength() > 0)
            {
                var completion = choicesProp[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                // Yapay zekadan tek parça gelen saf HTML'i <h4> etiketlerine göre kesiyoruz
                string[] sections = completion.Split("<h4>");

                // Her parçanın başına silinen başlık etiketini geri ekleyip View tarafına ayrı ayrı gönderiyoruz
                ViewBag.AnalysisSection1 = "<h4>" + sections.ElementAtOrDefault(1);
                ViewBag.AnalysisSection2 = "<h4>" + sections.ElementAtOrDefault(2);
                ViewBag.AnalysisSection3 = "<h4>" + sections.ElementAtOrDefault(3);
                ViewBag.AnalysisSection4 = "<h4>" + sections.ElementAtOrDefault(4);
                ViewBag.AnalysisSection5 = "<h4>" + sections.ElementAtOrDefault(5);
                ViewBag.AnalysisSection6 = "<h4>" + sections.ElementAtOrDefault(6);
            }
            else
            {
                ViewBag.Analyze = $"Beklenmeyen JSON Yapısı! Gelen Veri: {responseString}";
            }

            return View();
        }
    }
}