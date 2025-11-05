using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HerbavitaCMR.Models;
using System.Net.Http;

namespace HerbavitaCMR.Services
{
    public class CMRService
    {
        private readonly HttpClient _httpClient;
        public CMRService()
        {
            _httpClient = new HttpClient(new CustomHttpMessageHandler())
            {
                BaseAddress = CustomHttpMessageHandler.BaseAddress
            };
        }
        public async Task<List<Document>> GetCmrDocumentAsync(bool forceRefresh = false)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/Receive/Cmr");
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GET api/Receive/Cmr response: {content}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to fetch items. Status Code: {response.StatusCode}, Error: {errorContent}");
                    return new List<Document>();
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                var results = new List<Document>();
                if (root.TryGetProperty("value", out var valueElem) && valueElem.ValueKind == JsonValueKind.Array)
                {
                    foreach (var valueItem in valueElem.EnumerateArray())
                    {
                        if (valueItem.TryGetProperty("entries", out var entriesElem) && entriesElem.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var entry in entriesElem.EnumerateArray())
                            {
                                if (entry.TryGetProperty("document", out var innerDocElem))
                                {
                                    var document = JsonSerializer.Deserialize<Document>(innerDocElem.GetRawText(), options) ?? new Document();
                                    if (entry.TryGetProperty("palletItems", out var palletItemsElem) && palletItemsElem.ValueKind == JsonValueKind.Array)
                                    {
                                        var palletItems = JsonSerializer.Deserialize<List<Document.PalletItem>>(palletItemsElem.GetRawText(), options) ?? new List<Document.PalletItem>();
                                        document.PalletItems = palletItems;
                                    }
                                    if (entry.TryGetProperty("shippers", out var shippersElem) && shippersElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.Shippers = JsonSerializer.Deserialize<List<Document.Shipper>>(shippersElem.GetRawText(), options) ?? new List<Document.Shipper>();
                                    }
                                    if (entry.TryGetProperty("expeditors", out var expeditorsElem) && expeditorsElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.Expeditors = JsonSerializer.Deserialize<List<Document.Expeditor>>(expeditorsElem.GetRawText(), options) ?? new List<Document.Expeditor>();
                                    }
                                    if (entry.TryGetProperty("consignees", out var consigneesElem) && consigneesElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.Consignees = JsonSerializer.Deserialize<List<Document.Consignee>>(consigneesElem.GetRawText(), options) ?? new List<Document.Consignee>();
                                    }
                                    if (entry.TryGetProperty("itemAdrs", out var itemAdrsElem) && itemAdrsElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.ItemAdrs = JsonSerializer.Deserialize<List<Document.ItemAdr>>(itemAdrsElem.GetRawText(), options) ?? new List<Document.ItemAdr>();
                                    }
                                    // New optionals: Deserialize if present, else defaults
                                    if (entry.TryGetProperty("successiveCarriers", out var successiveElem) && successiveElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.SuccessiveCarriers = JsonSerializer.Deserialize<List<Document.SuccessiveCarrier>>(successiveElem.GetRawText(), options) ?? new List<Document.SuccessiveCarrier>();
                                    }
                                    if (entry.TryGetProperty("transportExecutions", out var execElem) && execElem.ValueKind == JsonValueKind.Array)
                                    {
                                        document.TransportExecutions = JsonSerializer.Deserialize<List<Document.TransportExecution>>(execElem.GetRawText(), options) ?? new List<Document.TransportExecution>();
                                    }
                                    // Strings: Set if present
                                    if (entry.TryGetProperty("transportationCosts", out var costsElem))
                                    {
                                        document.TransportationCosts = costsElem.GetString() ?? string.Empty;
                                    }
                                    if (entry.TryGetProperty("carrierReservationInformation", out var resElem))
                                    {
                                        document.CarrierReservationInformation = resElem.GetString() ?? string.Empty;
                                    }
                                    if (entry.TryGetProperty("consignorDocuments", out var docsElem))
                                    {
                                        document.ConsignorDocuments = docsElem.GetString() ?? string.Empty;
                                    }
                                    if (entry.TryGetProperty("consignorInstructions", out var instElem))
                                    {
                                        document.ConsignorInstructions = instElem.GetString() ?? string.Empty;
                                    }
                                    if (entry.TryGetProperty("eCmr", out var ecrmElem))
                                    {
                                        document.ECmr = ecrmElem.GetBoolean();
                                    }
                                    results.Add(document);
                                }
                            }
                        }
                    }
                }
                return results;
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"JSON error: {jex.Message}");
                return new List<Document>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching documents: {ex.Message}");
                return new List<Document>();
            }
        }
    }
}