# Pitney-Bowes Shipping Api

<!-- key: b2FG1uiL5aroYit5mALymdTodRsiH6uL -->
<!-- secret: G1Ny6man30quAyoZ -->
<!-- Developer Id: 887800997 -->
<!-- Default Merchant ID: 3003454811 -->
<!-- Sandbox BaseURL -->

## OAuth authorization

The following request will return a JSON object containing the access token. The token is valid for 10 hours.

curl -X POST https://shipping-api-sandbox.pitneybowes.com/oauth/token
-H "Authorization: Basic "
-H "Content-Type: application/x-www-form-urlencoded"
-d "grant_type=client_credentials"

## Address Verification

### Address

+ AddressLines : List\<string>?
+ CarrierRoute : string?
+ CityTown : string?
+ Company : string?
+ CountryCode : string
+ DeliveryPoint : string?
+ Email : string?
+ Name : string string?
+ Phone : string?
+ PostalCode : string?
+ Residential : bool?
+ StateProvince : string?
+ Status : string?
+ Taxld : string?
+ TaxIdType : string?

### Parcel

+ Dimension : ParcelDimension
+ Weight : ParcelWeight
+ ValueOfGoods : decimal
+ CurrencyCode : string

### Rate

+ Carrier : Carrier
+ Currency Code : string?
+ DeliveryCommitment : DeliveryCommitment?
+ DestinationZone :decimal?
+ DimensionalWeight : ParcelWeight?
+ Discounts : List\<Discount>?
+ InductionPostalCode : string?
+ ParcelType : ParcelType
+ RateTypeld : string?
+ Serviceld : Services?
+ SpecialServices : List\<SpecialService>?
+ Surcharges :  List\<Surcharge>?
+ TotalCarrierCharge : decimal?
+ TotalTaxAmount : decimal?

### CarrierPayment

+ AccountNumber : string
+ CountryCode : string
+ Party : string
+ PostalCode : string
+ TypeOfCharge : string

### ParcelDimension

+ Length : decimal
+ Height : decimal
+ Width : decimal
+ UnitOfMeasurement : UnitOfDimension
+ IrregularParcelGirth : decimal