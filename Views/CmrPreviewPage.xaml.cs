using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using HerbavitaCMR.Models;
using HerbavitaCMR.Services;

namespace HerbavitaCMR.Views
{
    public partial class CmrPreviewPage : ContentPage
    {
        // In-memory token cache for the lifetime of the page/process
        private static string? _accessToken;
        private static DateTime _accessTokenExpiresAt = DateTime.MinValue;

        // Default endpoints from the API documentation - switched to test environment
        private const string DefaultBaseApi = "https://api-test.ewastra.com/api/v2";
        private const string DefaultTokenEndpoint = "https://api-test.ewastra.com/api/v1/oauth2/token";

        // Allow null when not explicitly set
        public string? ApiEndpoint { get; set; }

        public CmrPreviewPage()
        {
            InitializeComponent();
        }

        public CmrPreviewPage(Document doc) : this()
        {
            // Ensure creation timestamp for display
            if (doc.CreationTimestamp == default)
                doc.CreationTimestamp = DateTime.Now;
            // Keep BindingContext for any XAML bindings that use it
            BindingContext = doc;
            // Populate named controls that are not bound in XAML
            PopulateFromDocument(doc);
        }

        void PopulateFromDocument(Document doc)
        {
            // Consignor (PrimaryExpeditor)
            var consignor = doc.PrimaryExpeditor;
            ConsignorCompanyLabel.Text = consignor?.CompanyName ?? doc.Customer ?? string.Empty;
            ConsignorEmailLabel.Text = consignor?.EmailAddress ?? string.Empty;
            ConsignorAddressSpan.Text =
                $"{consignor?.Address ?? doc.Address} {consignor?.Postcode ?? doc.Postcode} {consignor?.City ?? doc.City}, {consignor?.CountryCode ?? doc.CountryCode}";
            // Consignee (PrimaryConsignee)
            var consignee = doc.PrimaryConsignee;
            ConsigneeCompanyLabel.Text = consignee?.CompanyName ?? doc.Customer ?? string.Empty;
            ConsigneeEmailLabel.Text = consignee?.EmailAddress ?? string.Empty; // Now available
            ConsigneeAddressSpan.Text =
                $"{consignee?.Address ?? doc.Address} {consignee?.Postcode ?? doc.Postcode} {consignee?.City ?? doc.City}, {consignee?.CountryCode ?? doc.CountryCode}";
            // Shipper (PrimaryShipper)
            var shipper = doc.PrimaryShipper;
            ShipperCompanyLabel.Text = shipper?.CompanyName ?? string.Empty;
            ShipperEmailLabel.Text = shipper?.EmailAddress ?? string.Empty;
            ShipperAddressLabel.Text =
                $"{shipper?.Address ?? string.Empty} {shipper?.Postcode ?? string.Empty} {shipper?.City ?? string.Empty}, {shipper?.CountryCode ?? string.Empty}";
            ShipperLicenseLabel.Text = $"{shipper?.TruckLicence} {shipper?.TrailerLicence}".Trim();
            ShipperLicenseLabel.IsVisible = !string.IsNullOrWhiteSpace(ShipperLicenseLabel.Text);
            // Pickup / Delivery
            PickupPlaceLabel.Text = consignor?.Address ?? doc.Address;
            PickupDateLabel.Text = doc.shipmentDeliveryDate != default ? $"Pickup: {doc.shipmentDeliveryDate:yyyy-MM-dd HH:mm}" : string.Empty; // Added time
            PickupReferenceLabel.Text = $"Reference: {doc.PickupReference}";
            DeliveryAddressLabel.Text = consignee?.Address ?? doc.Address;
            DeliveryPostcodeLabel.Text = consignee?.Postcode ?? doc.Postcode;
            DeliveryCityLabel.Text = consignee?.City ?? doc.City;
            DeliveryDateLabel.Text = doc.DeliveryDateTime != null ? $"Delivery: {doc.DeliveryDateTime}" : string.Empty;
            // Goods: bind to per-item view list so ADR is available per-item in the template
            // IMPORTANT: use the per-item views that include AdrTextAllLanguages (so ADRs print immediately under their item)
            GoodsCollectionView.ItemsSource = doc.PrimaryPalletItemViews;
            PalletCountLabel.Text = $"Antal palleten: {doc.PalletCount}";
            PalletTotalWeightLabel.Text = $"Totaal gewicht: {doc.PalletTotalWeight:0.##} kg";
            // Clear document-level ADR printing so ADRs appear only inline after each item
            Box11Label.Text = string.Empty;
            Box13Label.Text = string.Empty;
            // New optionals: Bind to new fields (blanks show empty)
            Box8Label.Text = doc.TransportationCosts;
            Box9Label.Text = doc.CarrierReservationInformation;
            Box11Label.Text = doc.ConsignorDocuments;
            Box13Label.Text = doc.ConsignorInstructions;
            // Successive carriers: Filter for display (only successive, not sub)
            SuccessiveCarriersCollectionView.ItemsSource = doc.SuccessiveCarriers?.Where(c => c.IsSuccessiveCarrier).ToList() ?? new List<Document.SuccessiveCarrier>();
            // Footer place/date: Use consignor (pickup)
            FooterPlaceLabel.Text = doc.PrimaryExpeditor?.City ?? doc.Address; // e.g., "Brussels"
            FooterDateLabel.Text = doc.CreationTimestamp.ToString("yyyy-MM-dd");
            // Footer consignor/carrier info
            FooterConsignorName.Text = consignor?.CompanyName ?? doc.Customer ?? string.Empty;
            FooterConsignorAddress.Text = consignor?.Address ?? doc.Address ?? string.Empty;
            FooterCarrierName.Text = shipper?.CompanyName ?? string.Empty;
            FooterCarrierAddress.Text = shipper?.Address ?? string.Empty;
            // Footer pallets label (summary)
            FooterPalletsLabel.Text = $"Pallets: {doc.PalletCount} — Weight: {doc.PalletTotalWeight:0.##} kg";
        }

        // Click handler wired in XAML. If ApiEndpoint is not set, user is prompted to enter it.
        async void OnSendClicked(object sender, EventArgs e)
        {
            var endpoint = ApiEndpoint;
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                endpoint = await DisplayPromptAsync("API endpoint", "Enter POST endpoint (e.g. https://api-test.ewastra.com/api/v2/transportdocuments) or leave empty to use default", accept: "Send", cancel: "Cancel", placeholder: DefaultBaseApi);
                if (string.IsNullOrWhiteSpace(endpoint))
                    endpoint = DefaultBaseApi; // use default base
            }

            // Basic validation
            if (BindingContext is Document doc && string.IsNullOrWhiteSpace(doc.PrimaryExpeditor?.Address))
            {
                await DisplayAlert("Validation Error", "Consignor address required.", "OK");
                return;
            }

            try
            {
                var (success, responseBody) = await SubmitDocumentAsync(endpoint);
                if (success)
                {
                    // Show API response body on success to inspect created resource
                    var summary = string.IsNullOrWhiteSpace(responseBody) ? $"Status: Created" : $"Status: Created\n\n{responseBody}";
                    await DisplayAlert("Created", summary, "OK");
                }
                else
                {
                    await DisplayAlert("Failed", $"Document posting failed.\n\n{responseBody}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to send document: {ex.Message}", "OK");
            }
        }

        // Builds a JSON payload from the bound Document and POSTs it to the provided endpoint.
        // Uses OAuth2 client credentials to obtain a bearer token when required by the API.
        // Returns a tuple with success flag and the response body (for inspection).
        public async Task<(bool Success, string ResponseBody)> SubmitDocumentAsync(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint must be provided.", nameof(endpoint));
            if (BindingContext is not Document doc)
                throw new InvalidOperationException("BindingContext is not a Document.");
            if (string.IsNullOrWhiteSpace(doc.PrimaryExpeditor?.Address))
                throw new InvalidOperationException("Consignor address required.");

            // Ensure endpoint points to transportdocuments resource
            if (!endpoint.Contains("transportdocuments", StringComparison.OrdinalIgnoreCase))
            {
                endpoint = endpoint.TrimEnd('/') + "/transportdocuments";
            }

            // Acquire client credentials from user (for quick testing). In production store securely (SecureStorage).
            var clientId = await DisplayPromptAsync("API credentials", "Enter client_id for ewastra (leave blank to cancel)", accept: "OK", cancel: "Cancel");
            if (string.IsNullOrWhiteSpace(clientId))
                return (false, "client_id not provided");

            // Provide -1 (no limit) instead of null for maxLength
            var clientSecret = await DisplayPromptAsync("API credentials", "Enter client_secret for ewastra", accept: "OK", cancel: "Cancel", placeholder: "", maxLength: -1, keyboard: Keyboard.Text);
            var http = new HttpClient();
            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                // No secret, assume public client
                http.DefaultRequestHeaders.Add("X-Client-Id", clientId);
            }
            else
            {
                // Get bearer token (cached)
                var token = await GetAccessTokenAsync(DefaultTokenEndpoint, clientId, clientSecret);
                if (string.IsNullOrWhiteSpace(token))
                    return (false, "Failed to obtain access token");

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var consignor = doc.PrimaryExpeditor;
            var consignee = doc.PrimaryConsignee;
            var carrier = doc.PrimaryShipper;

            var payload = new
            {
                documents = new { documentTypeId = 1, countryLayout = "BE" }, // CMR, Belgium
                externalId = doc.Id.ToString(), // Or generate GUID: Guid.NewGuid().ToString()
                orderNumber = doc.DocumentNumber,
                dossierNumber = doc.DossierNumber,
                issuedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), 
                issuedPlace = consignor?.City ?? doc.City, 
                consignor = new
                {
                    externalId = consignor?.Id ?? 0,
                    name = consignor?.CompanyName ?? doc.Customer,
                    address = new
                    {
                        externalId = (int?)null,
                        additionalLine = "",
                        streetAddress = consignor?.Address ?? doc.Address,
                        postalcode = consignor?.Postcode ?? doc.Postcode,
                        city = consignor?.City ?? doc.City,
                        countryCode = consignor?.CountryCode ?? doc.CountryCode
                    },
                    emailAddress = consignor?.EmailAddress ?? ""
                },
                consignee = new // Similar mapping
                {
                    externalId = consignee?.Id ?? 0,
                    name = consignee?.CompanyName ?? "",
                    address = new
                    {
                        externalId = (int?)null,
                        additionalLine = "",
                        streetAddress = consignee?.Address ?? "",
                        postalcode = consignee?.Postcode ?? "",
                        city = consignee?.City ?? "",
                        countryCode = consignee?.CountryCode ?? ""
                    },
                    emailAddress = consignee?.EmailAddress ?? ""  // Now available
                },
                pickup = new // Reuse consignor + date/ref
                {
                    datetime = doc.PickupDateTimeFull,
                    reference = doc.PickupReference,
                    externalId = 0, 
                    name = consignor?.CompanyName, 
                    address = new // Copy consignor address
                    {
                        externalId = (int?)null,
                        additionalLine = "",
                        streetAddress = consignor?.Address ?? doc.Address,
                        postalcode = consignor?.Postcode ?? doc.Postcode,
                        city = consignor?.City ?? doc.City,
                        countryCode = consignor?.CountryCode ?? doc.CountryCode
                    },
                    emailAddress = consignor?.EmailAddress ?? ""
                },
                delivery = new // Reuse consignee
                {
                    datetime = doc.DeliveryDateTimeFull,
                    reference = doc.Reference ?? "", // Or separate
                    externalId = 0,
                    name = consignee?.CompanyName,
                    address = new // Copy consignee
                    {
                        externalId = (int?)null,
                        additionalLine = "",
                        streetAddress = consignee?.Address ?? "",
                        postalcode = consignee?.Postcode ?? "",
                        city = consignee?.City ?? "",
                        countryCode = consignee?.CountryCode ?? ""
                    },
                    emailAddress = consignee?.EmailAddress ?? ""
                },
                primaryCarrier = new
                {
                    isSuccessiveCarrier = false,
                    licensePlate = carrier?.TruckLicence ?? "", // Truck
                    externalId = carrier?.Id ?? 0,
                    name = carrier?.CompanyName ?? "",
                    address = new
                    {
                        externalId = (int?)null,
                        additionalLine = "",
                        streetAddress = carrier?.Address ?? "",
                        postalcode = carrier?.Postcode ?? "",
                        city = carrier?.City ?? "",
                        countryCode = carrier?.CountryCode ?? ""
                    },
                    emailAddress = carrier?.EmailAddress ?? "",
                    license = carrier?.TrailerLicence ?? "" // Trailer or company license
                },
                successiveCarriers = doc.SuccessiveCarriers?.Cast<object>().ToList() ?? new List<object>(), // Type-safe cast; empty if none
                goods = doc.PrimaryPalletItems.Select(pi =>
                {
                    // Use per-item pallet counts and keep item quantity as actual quantity
                    doc.PalletCountPerItem.TryGetValue(pi.ItemId, out var palletsForItem);
                    
                    // Get ADR texts for this specific item
                    var itemAdrs = doc.ItemAdrs?.Where(adr => adr.ItemId == pi.ItemId).ToList() ?? new List<Document.ItemAdr>();
                    
                    // Create dangerous goods descriptions with language support
                    var dangerousGoodsDescriptions = itemAdrs.Select(adr => new
                    {
                        language = GetLanguageCode(adr.LanguageId), // Convert language ID to code
                        description = new
                        {
                            adrDescription = adr.AdrText,
                            environmentallyHazardous = "", // Add if available in your data
                            specialProvisions = "", // Add if available
                            specialProvisionsForCarriage = "", // Add if available
                            carriageUnderDerogation = "" // Add if available
                        }
                    }).ToArray();

                    return new
                    {
                        externalId = pi.ItemId.ToString(),
                        code = pi.ItemNumber,
                        description = pi.Description,
                        quantity = pi.Quantity, // item quantity
                        grossWeight = pi.TotalGrossWeight, // Or compute actual weight (assumes kg unit)
                        grossWeightUnit = pi.UnitOfMeasure ?? "kg",
                        netWeight = pi.Quantity,
                        netWeightUnit = pi.UnitOfMeasure ?? "kg",
                        packagingDetails = new List<object>(),
                        
                        // ADR Information - Primary field for dangerous goods descriptions
                        dangerousGoodsDescriptions = dangerousGoodsDescriptions.Length > 0 ? dangerousGoodsDescriptions : null,
                        
                        // Alternative structured dangerous goods info if needed
                        dangerousGoodsInfo = itemAdrs.Any() ? itemAdrs.Select(adr => new
                        {
                            code = "", // UN number if available
                            technicalName = adr.AdrText,
                            additionalDescription = adr.AdrText
                        }).ToArray() : null
                    };
                }).ToArray(),
                transportationCosts = doc.TransportationCosts, // Blank string OK
                carrierReservationInformation = doc.CarrierReservationInformation,
                goodsInformation = doc.AdrTextAllLanguages, // Document-level ADR text
                goodsText = doc.AdrTextAllLanguages, // Document-level ADR text
                consignorDocuments = doc.ConsignorDocuments,
                consignorInstructions = doc.ConsignorInstructions,
                empties = new List<object>(), // Add if returnables needed
                transportExecutions = doc.TransportExecutions?.Cast<object>().ToList() ?? new List<object>(),
                eCmr = doc.ECmr
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // For debugging; set false in prod
            };
            var json = JsonSerializer.Serialize(payload, options);

            // Log json for testing
            Console.WriteLine(json);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PostAsync(endpoint, content);

            // Always read response body for inspection (successful or not)
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"POST failed: {response.StatusCode} - {responseBody}");
                return (false, responseBody);
            }

            // Success: return body so caller can display or parse it.
            Console.WriteLine($"POST succeeded: {response.StatusCode}\n{responseBody}");
            return (true, responseBody);
        }

        // Obtain an access token using OAuth2 client credentials (cached)
        private async Task<string> GetAccessTokenAsync(string tokenEndpoint, string clientId, string clientSecret)
        {
            if (!string.IsNullOrWhiteSpace(_accessToken) && DateTime.UtcNow < _accessTokenExpiresAt - TimeSpan.FromSeconds(30))
                return _accessToken;

            using var http = new HttpClient();
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            };
            using var content = new FormUrlEncodedContent(form);
            using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = content };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token request failed: {response.StatusCode} - {err}");
                return string.Empty;
            }

            var resp = await response.Content.ReadAsStringAsync();
            try
            {
                var doc = JsonSerializer.Deserialize<TokenResponse>(resp, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (doc == null || string.IsNullOrWhiteSpace(doc.AccessToken))
                    return string.Empty;

                _accessToken = doc.AccessToken;
                _accessTokenExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(doc.ExpiresIn > 0 ? doc.ExpiresIn : 3600);
                return _accessToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed parse token response: {ex.Message}");
                return string.Empty;
            }
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }
            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }

        // Helper method to convert language ID to language code
        private static string GetLanguageCode(int languageId)
        {
            return languageId switch
            {
                1 => "en", // Dutch
                2 => "fr", // French  
                3 => "en", // English
                4 => "de", // German
                _ => "nl"  // Default to Dutch
            };
        }
    }
}