using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;

namespace HerbavitaCMR.Models
{
    // Represents one item of the "value" array in the API response
    public class Document
    {
        [JsonPropertyName("shipmentDeliveryDate")]
        [Required]
        public DateTime shipmentDeliveryDate { get; set; }
        [JsonPropertyName("documentNumber")]
        [Required]
        public string DocumentNumber { get; set; } = string.Empty;
        [JsonPropertyName("id")]
        [Required]
        public int Id { get; set; }
        [JsonPropertyName("address")]
        [Required]
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("postcode")]
        [Required]
        public string Postcode { get; set; } = string.Empty;
        [JsonPropertyName("city")]
        [Required]
        public string City { get; set; } = string.Empty;
        [JsonPropertyName("country")]
        [Required]
        public string Country { get; set; } = string.Empty;
        [JsonPropertyName("customer")]
        public string Customer { get; set; } = string.Empty;
        [JsonPropertyName("vendor")]
        public string Vendor { get; set; } = string.Empty;
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;
        [JsonPropertyName("shippingType")]
        [Required]
        public string ShippingType { get; set; } = string.Empty;
        [JsonPropertyName("documentLines")]
        [Required]
        public List<DocumentLine> DocumentLines { get; set; } = new List<DocumentLine>();
        [JsonPropertyName("isOutbound")]
        public bool IsOutbound { get; set; }
        [JsonPropertyName("isInbound")]
        public bool IsInbound { get; set; }
        [JsonPropertyName("isShipped")]
        public bool IsShipped { get; set; }
        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
        [JsonPropertyName("addressModified")]
        public bool AddressModified { get; set; }
        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;
        [JsonPropertyName("shipmentDeliveryDateModified")]
        public bool ShipmentDeliveryDateModified { get; set; }
        [JsonPropertyName("shipmentMethodCode")]
        public string ShipmentMethodCode { get; set; } = string.Empty;
        [JsonPropertyName("shipperId")]
        public int ShipperId { get; set; }
        [JsonPropertyName("trackpodOrder")]
        public int? TrackpodOrder { get; set; }
        [JsonPropertyName("isReleased")]
        public bool IsReleased { get; set; }
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("expeditorId")]
        public int ExpeditorId { get; set; }
        // Sibling lists that come with each entry
        [JsonPropertyName("palletItems")]
        public List<PalletItem> PalletItems { get; set; } = new();
        [JsonPropertyName("shippers")]
        public List<Shipper> Shippers { get; set; } = new();
        [JsonPropertyName("expeditors")]
        public List<Expeditor> Expeditors { get; set; } = new();
        [JsonPropertyName("consignees")]
        public List<Consignee> Consignees { get; set; } = new();
        [JsonPropertyName("itemAdrs")]
        public List<ItemAdr> ItemAdrs { get; set; } = new();
        // Convenience properties for primary singles (assume first item)
        [JsonIgnore] // Not from JSON
        public Shipper? PrimaryShipper => Shippers?.FirstOrDefault();
        [JsonIgnore]
        public Expeditor? PrimaryExpeditor => Expeditors?.FirstOrDefault();
        [JsonIgnore]
        public Consignee? PrimaryConsignee => Consignees?.FirstOrDefault();
        // For pallet items (all are "primary" here)
        [JsonIgnore]
        public List<PalletItem> PrimaryPalletItems => PalletItems ?? new List<PalletItem>();
        // Computed summaries
        [JsonIgnore]
        public int PalletCount => PalletItems?.Select(pi => pi.PalletId).Distinct().Count() ?? 0;
        [JsonIgnore]
        public Dictionary<int, int> PalletCountPerItem => ComputePalletCountsPerItem();

        // Made public so callers (views/services) can access per-item pallet counts.
        public Dictionary<int, int> ComputePalletCountsPerItem()
        {
            var counts = new Dictionary<int, int>();
            if (PalletItems == null || !PalletItems.Any()) return counts;
            foreach (var group in PalletItems.GroupBy(pi => pi.ItemId))
            {
                counts[group.Key] = group.Select(pi => pi.PalletId).Distinct().Count();
            }
            return counts;
        }
        [JsonIgnore]
        public decimal PalletTotalWeight => PalletItems?.Sum(pi => (decimal)pi.TotalGrossWeight) ?? 0m;
        // Replace the problematic property with the correct null-coalescing and type usage
[JsonIgnore]
public double TotalWeight => PalletItems?.Sum(pi => pi.TotalGrossWeight) ?? 0.0;
        // For unbound labels (customize as needed)
        [JsonIgnore]
        public string DossierNumber => DocumentNumber; // Assuming "Dossier" means customer name; alt: DocumentNumber or Id.ToString()
        [JsonIgnore]
        public string PickupReference => Reference ?? string.Empty; // Or empty if no ref
        [JsonIgnore]
        public string DeliveryDateTime => shipmentDeliveryDate.ToString("yyyy-MM-dd"); // Date-only for delivery
        // GoodsText: use ADR text for the document (all languages block)
        [JsonIgnore]
        public string GoodsText => AdrTextAllLanguages;
        // New optional fields (from CMR mapping; defaults for blanks)
        [JsonPropertyName("successiveCarriers")]
        public List<SuccessiveCarrier> SuccessiveCarriers { get; set; } = new(); // Optional; empty if none
        [JsonPropertyName("transportationCosts")]
        public string TransportationCosts { get; set; } = string.Empty; // Field 8; blank OK
        [JsonPropertyName("carrierReservationInformation")]
        public string CarrierReservationInformation { get; set; } = string.Empty; // Field 9
        [JsonPropertyName("consignorDocuments")]
        public string ConsignorDocuments { get; set; } = string.Empty; // Field 11
        [JsonPropertyName("consignorInstructions")]
        public string ConsignorInstructions { get; set; } = string.Empty; // Field 13; add empties table if needed later
        [JsonPropertyName("transportExecutions")]
        public List<TransportExecution> TransportExecutions { get; set; } = new(); // Page 7; optional driver/truck
        [JsonPropertyName("eCmr")]
        public bool ECmr { get; set; } = true; // Page 6; default true
        // Convenience props for issue/pickup/delivery (reuse shipmentDeliveryDate)
        [JsonIgnore]
        public string IssuedPlace => PrimaryExpeditor?.City ?? City; // Consignor city as pickup
        [JsonIgnore]
        public string PickupDateTimeFull => shipmentDeliveryDate.ToString("yyyy-MM-ddTHH:mm:ss"); // ISO for JSON
        [JsonIgnore]
        public string DeliveryDateTimeFull => shipmentDeliveryDate.ToString("yyyy-MM-ddTHH:mm:ss"); // Reuse or add separate if needed
        // ADR text for goods — existing preferred-language convenience (keeps current behavior)
        [JsonIgnore]
        public string AdrText => GetPreferredAdrText();
        private string GetPreferredAdrText()
        {
            if (string.IsNullOrEmpty(LanguageCode) || !int.TryParse(LanguageCode, out int preferredLangId)) preferredLangId = 1; // Default to NL=1
            var relevantAdrs = ItemAdrs?.Where(adr => PalletItems?.Any(pi => pi.ItemId == adr.ItemId) == true && adr.LanguageId == preferredLangId).Select(adr => adr.AdrText);
            return relevantAdrs != null && relevantAdrs.Any() ? string.Join("\n", relevantAdrs) : string.Empty;
        }
        // New: return ADR text for all languages found for the document's pallet items.
        [JsonIgnore]
        public string AdrTextAllLanguages => GetAllAdrText();
        private string GetAllAdrText()
        {
            if (ItemAdrs == null || ItemAdrs.Count == 0 || PalletItems == null || PalletItems.Count == 0)
                return string.Empty;
            var relevant = ItemAdrs.Where(adr => PalletItems.Any(pi => pi.ItemId == adr.ItemId)).ToList();
            if (!relevant.Any())
                return string.Empty;
            var grouped = relevant
                .GroupBy(a => a.LanguageId)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var langLabel = GetLanguageLabel(g.Key);
                    var texts = string.Join("\n", g.Select(x => x.AdrText).Where(t => !string.IsNullOrWhiteSpace(t)));
                    return string.IsNullOrWhiteSpace(texts) ? string.Empty : $"{langLabel}: {texts}";
                })
                .Where(s => !string.IsNullOrWhiteSpace(s));
            return string.Join("\n\n", grouped);
        }
        private static string GetLanguageLabel(int languageId) =>
            languageId switch
            {
                1 => "NL",
                2 => "FR",
                3 => "EN",
                4 => "CZ",
                _ => $"Lang {languageId}"
            };
        // NEW: a lightweight view per pallet item that includes ADR texts for that item.
        [JsonIgnore]
        public List<PalletItemView> PrimaryPalletItemViews => BuildPalletItemViews();
        private List<PalletItemView> BuildPalletItemViews()
        {
            var items = PrimaryPalletItems;
            var views = new List<PalletItemView>();
            if (items == null || items.Count == 0)
                return views;
            foreach (var pi in items)
            {
                // collect ADRs for this itemId and group by language
                var relevant = ItemAdrs?.Where(a => a.ItemId == pi.ItemId).ToList() ?? new List<ItemAdr>();
                string adrText = string.Empty;
                if (relevant.Any())
                {
                    var grouped = relevant
                        .GroupBy(a => a.LanguageId)
                        .OrderBy(g => g.Key)
                        .Select(g =>
                        {
                            var langLabel = GetLanguageLabel(g.Key);
                            var texts = string.Join("\n", g.Select(x => x.AdrText).Where(t => !string.IsNullOrWhiteSpace(t)));
                            return string.IsNullOrWhiteSpace(texts) ? string.Empty : $"{langLabel}: {texts}";
                        })
                        .Where(s => !string.IsNullOrWhiteSpace(s));
                    adrText = string.Join("\n\n", grouped);
                }
                views.Add(new PalletItemView
                {
                    ItemNumber = pi.ItemNumber,
                    Description = pi.Description,
                    NetWeight = pi.Quantity,
                    GrossWeight = (double)pi.TotalGrossWeight,
                    UnitOfMeasure = pi.UnitOfMeasure,
                    AdrTextAllLanguages = adrText
                });
            }
            return views;
        }
        // Lightweight per-item view used only for UI presentation.
        public class PalletItemView
        {
            public string ItemNumber { get; init; } = string.Empty;
            public string Description { get; init; } = string.Empty;
            // Quantity retained as net weight (existing consumers)
            public decimal Quantity { get; init; }
            // New: explicit net and gross weight properties
            public decimal NetWeight { get; init; }
            public double GrossWeight { get; init; }
            public string UnitOfMeasure { get; init; } = string.Empty;
            public string AdrTextAllLanguages { get; init; } = string.Empty;
        }
        // Nested types (unique names, no duplicates)
        public class Shipper
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("companyName")]
            public string CompanyName { get; set; } = string.Empty;
            [JsonPropertyName("address")]
            public string Address { get; set; } = string.Empty;
            [JsonPropertyName("postcode")]
            public string Postcode { get; set; } = string.Empty;
            [JsonPropertyName("city")]
            public string City { get; set; } = string.Empty;
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; } = string.Empty;
            [JsonPropertyName("country")]
            public string Country { get; set; } = string.Empty;
            [JsonPropertyName("truckLicence")]
            public string TruckLicence { get; set; } = string.Empty;
            [JsonPropertyName("trailerLicence")]
            public string TrailerLicence { get; set; } = string.Empty;
            [JsonPropertyName("emailAddress")]
            public string EmailAddress { get; set; } = string.Empty;
        }
        public class Expeditor
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("companyName")]
            public string CompanyName { get; set; } = string.Empty;
            [JsonPropertyName("address")]
            public string Address { get; set; } = string.Empty;
            [JsonPropertyName("postcode")]
            public string Postcode { get; set; } = string.Empty;
            [JsonPropertyName("city")]
            public string City { get; set; } = string.Empty;
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; } = string.Empty;
            [JsonPropertyName("country")]
            public string Country { get; set; } = string.Empty;
            [JsonPropertyName("emailAddress")]
            public string EmailAddress { get; set; } = string.Empty;
        }
        public class Consignee
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("companyName")]
            public string CompanyName { get; set; } = string.Empty;
            [JsonPropertyName("address")]
            public string Address { get; set; } = string.Empty;
            [JsonPropertyName("postcode")]
            public string Postcode { get; set; } = string.Empty;
            [JsonPropertyName("city")]
            public string City { get; set; } = string.Empty;
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; } = string.Empty;
            [JsonPropertyName("country")]
            public string Country { get; set; } = string.Empty;
            [JsonPropertyName("emailAddress")]  // Added for consistency
            public string EmailAddress { get; set; } = string.Empty;
        }
        public class PalletItem
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("palletId")]
            public int PalletId { get; set; }
            [JsonPropertyName("itemId")]
            public int ItemId { get; set; }
            [JsonPropertyName("lotNumber")]
            public string LotNumber { get; set; } = string.Empty;
            [JsonPropertyName("quantity")]
            public decimal Quantity { get; set; }
            [JsonPropertyName("quantityConsumed")]
            public decimal QuantityConsumed { get; set; }
            [JsonPropertyName("quantityRemaining")]
            public decimal QuantityRemaining { get; set; }
            [JsonPropertyName("unitOfMeasureId")]
            public int UnitOfMeasureId { get; set; }
            [JsonPropertyName("packageUnitOfMeasureId")]
            public int PackageUnitOfMeasureId { get; set; }
            [JsonPropertyName("creationTimestamp")]
            public DateTime CreationTimestamp { get; set; }
            [JsonPropertyName("updateTimestamp")]
            public DateTime UpdateTimestamp { get; set; }
            [JsonPropertyName("itemNumber")]
            public string ItemNumber { get; set; } = string.Empty;
            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;
            [JsonPropertyName("unitOfMeasure")]
            public string UnitOfMeasure { get; set; } = string.Empty;
            [JsonPropertyName("itemWeightGross ")]
            public double ItemWeightGross { get; set; }
            [JsonPropertyName("totalGrossWeight")]
            public double TotalGrossWeight { get; set; }
        }
        public class ItemAdr
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("itemId")]
            public int ItemId { get; set; }
            [JsonPropertyName("languageId")]
            public int LanguageId { get; set; }
            [JsonPropertyName("adrText")]
            public string AdrText { get; set; } = string.Empty;
            [JsonPropertyName("creationTimestamp")]
            public DateTime CreationTimestamp { get; set; }
        }
        // New: AddressInfo class for nested use (e.g., in SuccessiveCarrier)
        public class AddressInfo
        {
            [JsonPropertyName("externalId")]
            public int? ExternalId { get; set; }
            [JsonPropertyName("additionalLine")]
            public string AdditionalLine { get; set; } = string.Empty;
            [JsonPropertyName("streetAddress")]
            public string StreetAddress { get; set; } = string.Empty;
            [JsonPropertyName("postalcode")]
            public string Postalcode { get; set; } = string.Empty;
            [JsonPropertyName("city")]
            public string City { get; set; } = string.Empty;
            [JsonPropertyName("countryCode")]
            public string CountryCode { get; set; } = string.Empty;
        }
        // New nested classes for CMR (from mapping)
        public class SuccessiveCarrier
        {
            [JsonPropertyName("isSuccessiveCarrier")]
            public bool IsSuccessiveCarrier { get; set; } // true for field 7; false for 6
            [JsonPropertyName("externalId")]
            public int? ExternalId { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("address")]
            public AddressInfo Address { get; set; } = new AddressInfo(); // Now uses AddressInfo
            [JsonPropertyName("license")]
            public string License { get; set; } = string.Empty;
        }
        public class TransportExecution
        {
            [JsonPropertyName("driver")]
            public Driver Driver { get; set; } = new Driver();
            [JsonPropertyName("truck")]
            public Vehicle Truck { get; set; } = new Vehicle();
            [JsonPropertyName("trailer")]
            public Vehicle Trailer { get; set; } = new Vehicle();
            [JsonPropertyName("isCompleted")]
            public bool IsCompleted { get; set; } = false;
        }
        public class Driver
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("externalId")]
            public int ExternalId { get; set; }
        }
        public class Vehicle
        {
            [JsonPropertyName("externalId")]
            public int ExternalId { get; set; }
            [JsonPropertyName("licensePlate")]
            public string LicensePlate { get; set; } = string.Empty;
        }
        public class ApiResponse<T>
        {
            [JsonPropertyName("value")]
            public T? Value { get; set; }
            [JsonPropertyName("message")]
            public string Message { get; set; } = string.Empty;
            [JsonPropertyName("isSuccess")]
            public bool IsSuccess { get; set; }
        }
    }
    public class DocumentLine
    {
        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }
        [JsonPropertyName("itemCode")]
        public string ItemCode { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;
    }
}