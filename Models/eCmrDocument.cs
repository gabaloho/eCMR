using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HerbavitaCMR.Models
{
    public class CmrDocument
    {

       
        public class Address
        {
            public int id { get; set; }
            public string externalId { get; set; }
            public string additionalLine { get; set; }
            public string streetAddress { get; set; }
            public string postalcode { get; set; }
            public string city { get; set; }
            public string countryCode { get; set; }
            public string country { get; set; }
            public int longitude { get; set; }
            public int latitude { get; set; }
            public string text { get; set; }
        }

       

       
        public class Carrier
        {
            public int id { get; set; }
            public string externalId { get; set; }
        }

        public class CollaborationSettings
        {
            public bool collaborate { get; set; }
            public string emailAddress { get; set; }
            public string contactName { get; set; }
            public string message { get; set; }
            public int collaborationScope { get; set; }
        }

        public class Company
        {
            public int id { get; set; }
            public string externalId { get; set; }
        }

        public class Consignee
        {
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class Consignor
        {
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class Contact
        {
            public Contact contact { get; set; }
            public string role { get; set; }
        }

        public class Contact2
        {
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class ContactId
        {
            public int id { get; set; }
            public string externalId { get; set; }
        }

        public class DangerousGood
        {
            public string code { get; set; }
            public string labelOrClassificationCode { get; set; }
            public string packagingDangerLevelCode { get; set; }
            public int points { get; set; }
            public int quantity { get; set; }
            public string quantityUnit { get; set; }
            public string technicalName { get; set; }
            public string transportCategory { get; set; }
            public string tunnelRestrictionCodes { get; set; }
            public string environmentallyHazardous { get; set; }
            public string hazardInitiators { get; set; }
            public string additionalDescription { get; set; }
        }

        public class DangerousGoodsDescription
        {
            public string language { get; set; }
            public Description description { get; set; }
        }

        public class DangerousGoodsInfo
        {
            public string code { get; set; }
            public string labelOrClassificationCode { get; set; }
            public string packagingDangerLevelCode { get; set; }
            public int points { get; set; }
            public int quantity { get; set; }
            public string quantityUnit { get; set; }
            public string technicalName { get; set; }
            public string transportCategory { get; set; }
            public string tunnelRestrictionCodes { get; set; }
            public string environmentallyHazardous { get; set; }
            public string hazardInitiators { get; set; }
            public string additionalDescription { get; set; }
        }

        public class DangerousGoodSummary
        {
            public string remarks { get; set; }
            public string mostRestrictiveTunnelCodes { get; set; }
        }

        public class Delivery
        {
            public DateTime datetime { get; set; }
            public DateTime arrival { get; set; }
            public DateTime departure { get; set; }
            public string reference { get; set; }
            public string remarks { get; set; }
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class Description
        {
            public string adrDescription { get; set; }
            public string environmentallyHazardous { get; set; }
            public string specialProvisions { get; set; }
            public string specialProvisionsForCarriage { get; set; }
            public string carriageUnderDerogation { get; set; }
        }

        public class Document
        {
            public int documentTypeId { get; set; }
            public int documentNumber { get; set; }
            public string referenceNumber { get; set; }
            public string countryLayout { get; set; }
            public int pages { get; set; }
            public string printer { get; set; }
        }

        public class Driver
        {
            public Carrier carrier { get; set; }
            public string name { get; set; }
            public string carrierLicense { get; set; }
            public int id { get; set; }
            public string externalId { get; set; }
        }

        public class Empty
        {
            public int id { get; set; }
            public string externalId { get; set; }
            public string code { get; set; }
            public string description { get; set; }
            public int location { get; set; }
            public int quantityDelivered { get; set; }
            public int quantityReturned { get; set; }
        }

        public class EquipmentId
        {
            public string scheme { get; set; }
            public string value { get; set; }
            public string uri { get; set; }
        }

        public class Event
        {
            public string name { get; set; }
            public int executionOrder { get; set; }
        }

        public class ExternalAttributes
        {
        }

        public class ExternalId
        {
            public string scheme { get; set; }
            public string value { get; set; }
            public string uri { get; set; }
        }

        public class ExternalReference
        {
            public string name { get; set; }
            public string id { get; set; }
        }

        public class ExtraInformation
        {
        }

        public class ExtraProperties
        {
        }

        public class Good
        {
            public string packagingDescription { get; set; }
            public string sealNumber { get; set; }
            public List<RemarkCode> remarkCodes { get; set; }
            public List<Weight> weights { get; set; }
            public string serviceType { get; set; }
            public List<DangerousGoodsDescription> dangerousGoodsDescriptions { get; set; }
            public int id { get; set; }
            public string externalId { get; set; }
            public string sku { get; set; }
            public string code { get; set; }
            public string lotNumber { get; set; }
            public string description { get; set; }
            public string reference { get; set; }
            public int quantity { get; set; }
            public int quantityRefused { get; set; }
            public int unitPrice { get; set; }
            public int volume { get; set; }
            public string volumeUnit { get; set; }
            public int grossWeight { get; set; }
            public string grossWeightUnit { get; set; }
            public int netWeight { get; set; }
            public string netWeightUnit { get; set; }
            public List<PackagingDetail> packagingDetails { get; set; }
            public bool isReturnable { get; set; }
            public List<DangerousGoodsInfo> dangerousGoodsInfo { get; set; }
            public int productState { get; set; }
            public WasteInfo wasteInfo { get; set; }
            public ExternalAttributes externalAttributes { get; set; }
            public string deliveryRemarks { get; set; }
        }

        public class GrossWeight
        {
            public int value { get; set; }
            public string unit { get; set; }
        }

        public class PackagingDetail
        {
            public int id { get; set; }
            public string code { get; set; }
            public string description { get; set; }
            public int productState { get; set; }
            public int quantity { get; set; }
            public int quantityRefused { get; set; }
            public ExternalAttributes externalAttributes { get; set; }
            public int type { get; set; }
            public List<PackagingDetail> packagingDetails { get; set; }
            public GrossWeight grossWeight { get; set; }
            public string additionalDescription { get; set; }
            public List<Weight> weights { get; set; }
            public string serviceType { get; set; }
            public string externalId { get; set; }
        }

        public class Pickup
        {
            public DateTime datetime { get; set; }
            public DateTime arrival { get; set; }
            public DateTime departure { get; set; }
            public string reference { get; set; }
            public string remarks { get; set; }
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class PrimaryCarrier
        {
            public bool isSuccessiveCarrier { get; set; }
            public string licensePlate { get; set; }
            public string trailerLicensePlate { get; set; }
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class Properties
        {
            public int documentTypeId { get; set; }
            public int documentNumber { get; set; }
            public string referenceNumber { get; set; }
            public string countryLayout { get; set; }
            public int pages { get; set; }
            public string printer { get; set; }
        }

        public class RegistrationNumbers
        {
        }

        public class Reimbursement
        {
            public int amount { get; set; }
            public string currency { get; set; }
        }

        public class RemarkCode
        {
            public int id { get; set; }
            public int remarkcodeid { get; set; }
            public int userid { get; set; }
            public int location { get; set; }
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Remarkcodesitem
        {
            public int id { get; set; }
            public int remarkcodeid { get; set; }
            public int userid { get; set; }
            public int location { get; set; }
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Reservation
        {
            public bool fromCarrier { get; set; }
            public int id { get; set; }
            public string externalId { get; set; }
            public string text { get; set; }
            public int creationState { get; set; }
            public ContactId contactId { get; set; }
            public string role { get; set; }
            public string code { get; set; }
        }

        public class ResponsibleContact
        {
            public int id { get; set; }
            public string emailAddress { get; set; }
            public string faxNumber { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string telephoneNumber { get; set; }
        }

        public class Root
        {
            public ExtraProperties extraProperties { get; set; }
            public WasteIdentification wasteIdentification { get; set; }
            public List<string> transitCountries { get; set; }
            public List<TransportEquipment> transportEquipment { get; set; }
            public DangerousGoodSummary dangerousGoodSummary { get; set; }
            public List<Remarkcodesitem> remarkcodesitem { get; set; }
            public string orderState { get; set; }
            public Properties properties { get; set; }
            public List<Document> documents { get; set; }
            public int id { get; set; }
            public Company company { get; set; }
            public string externalId { get; set; }
            public string cmrNumber { get; set; }
            public string orderNumber { get; set; }
            public string dossierNumber { get; set; }
            public List<ExternalReference> externalReferences { get; set; }
            public DateTime issuedDate { get; set; }
            public string issuedPlace { get; set; }
            public Consignor consignor { get; set; }
            public string consignorDocuments { get; set; }
            public string specialAgreements { get; set; }
            public string consignorInstructions { get; set; }
            public Consignee consignee { get; set; }
            public Pickup pickup { get; set; }
            public Delivery delivery { get; set; }
            public PrimaryCarrier primaryCarrier { get; set; }
            public List<SuccessiveCarrier> successiveCarriers { get; set; }
            public string goodsText { get; set; }
            public string goodsInformation { get; set; }
            public string carrierReservationInformation { get; set; }
            public List<Good> goods { get; set; }
            public List<DangerousGood> dangerousGoods { get; set; }
            public string transportationCosts { get; set; }
            public bool carriagePaid { get; set; }
            public bool carriageForward { get; set; }
            public Reimbursement reimbursement { get; set; }
            public int state { get; set; }
            public int transportOrderState { get; set; }
            public TransportType transportType { get; set; }
            
            public List<Empty> empties { get; set; }
            public List<Contact> contacts { get; set; }
            public List<Reservation> reservations { get; set; }
            public List<TransportExecution> transportExecutions { get; set; }
            public bool eCmr { get; set; }
            public string incoterms { get; set; }
        }

        public class SuccessiveCarrier
        {
            public bool isSuccessiveCarrier { get; set; }
            public string licensePlate { get; set; }
            public string trailerLicensePlate { get; set; }
            public RegistrationNumbers registrationNumbers { get; set; }
            public ResponsibleContact responsibleContact { get; set; }
            public ExtraInformation extraInformation { get; set; }
            public string branchNumber { get; set; }
            public Address address { get; set; }
            public CollaborationSettings collaborationSettings { get; set; }
            public string emailAddress { get; set; }
            public string externalId { get; set; }
            public string faxNumber { get; set; }
            public int id { get; set; }
            public string license { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string organizationNumber { get; set; }
            public DateTime SignatureDate { get; set; }
            public string signatureImage { get; set; }
            public string signatureName { get; set; }
            public string signatureText { get; set; }
            public string telephoneNumber { get; set; }
            public string text { get; set; }
        }

        public class Trailer
        {
            public string externalId { get; set; }
            public int id { get; set; }
            public string licensePlate { get; set; }
            public string name { get; set; }
            public TruckloadCapacity truckloadCapacity { get; set; }
            public TruckloadVolume truckloadVolume { get; set; }
        }

        public class TransportEquipment
        {
   
            public List<EquipmentId> equipmentIds { get; set; }
            public bool isReturnable { get; set; }
            public string type { get; set; }
            public string associationType { get; set; }
            public List<ExternalId> externalIds { get; set; }
            public string id { get; set; }
        }

        public class TransportExecution
        {
            public Driver driver { get; set; }
            public Truck truck { get; set; }
            public Trailer trailer { get; set; }
            public bool isCompleted { get; set; }
            public bool isSuccessiveCarrier { get; set; }
            public int executionSequence { get; set; }
            public List<Event> events { get; set; }
        }

        public class TransportType
        {
            public int id { get; set; }
            public string code { get; set; }
            public string description { get; set; }
        }

        public class Truck
        {
            public string externalId { get; set; }
            public int id { get; set; }
            public string licensePlate { get; set; }
            public string name { get; set; }
            public TruckloadCapacity truckloadCapacity { get; set; }
            public TruckloadVolume truckloadVolume { get; set; }
        }

        public class TruckloadCapacity
        {
            public int value { get; set; }
            public string unit { get; set; }
        }

        public class TruckloadVolume
        {
            public int value { get; set; }
            public string unit { get; set; }
        }

        public class WasteIdentification
        {
        }

        public class WasteInfo
        {
            public ExternalAttributes externalAttributes { get; set; }
            public string processingNature { get; set; }
            public string processingNatureDescription { get; set; }
            public string stateOfAggregation { get; set; }
            public string treatmentTechnique { get; set; }
            public string chemicalComposition { get; set; }
        }

        public class Weight
        {
            public DateTime dateTime { get; set; }
            public string type { get; set; }
            public string measurementType { get; set; }
            public string location { get; set; }
            public int value { get; set; }
            public string unit { get; set; }
        }


    }
}
