# Pitney-Bowes Shipping Api

<!-- key: b2FG1uiL5aroYit5mALymdTodRsiH6uL -->
<!-- secret: G1Ny6man30quAyoZ -->
<!-- Developer Id: 887800997 -->
<!-- Default Merchant ID: 3003454811 -->
<!-- Sandbox BaseURL -->

## OAuth authorization

The following request will return a JSON object containing the access token. The token is valid for 10 hours.

curl -X POST https://shipping-api-sandbox.pitneybowes.com/oauth/token
-H "Authorization: Basic YjJGRzF1aUw1YXJvWWl0NW1BTHltZFRvZFJzaUg2dUw6RzFOeTZtYW4zMHF1QXlvWg=="
-H "Content-Type: application/x-www-form-urlencoded"
-d "grant_type=client_credentials"

## Address Verification

Example of address verification and OAuth configuration

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using shippingapi.Api;
using shippingapi.Client;
using shippingapi.Model;

namespace Example
{
    public class VerifyAddressExample
    {
        public static void Main()
        {
            Configuration.Default.BasePath = "https://shipping-api-sandbox.pitneybowes.com/shippingservices";
            // Configure OAuth2 access token for authorization: oAuth2ClientCredentials
            Configuration.Default.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AddressValidationApi(Configuration.Default);
            var address = new Address(); // Address | Address object that needs to be validated.
            var xPBUnifiedErrorStructure = true;  // bool? | Set this to true to use the standard [error object](https://shipping.pitneybowes.com/reference/error-object.html#standard-error-object) if an error occurs. (optional)  (default to true)
            var minimalAddressValidation = true;  // bool? | When set to true, the complete address (delivery line and last line) is validated but only the last line (city, state, and postal code) would be changed by the validation check. (optional) 

            try
            {
                // Address validation
                Address result = apiInstance.VerifyAddress(address, xPBUnifiedErrorStructure, minimalAddressValidation);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling AddressValidationApi.VerifyAddress: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

## Carrier & CarrierRules

Example `GetCarrierServiceRules`