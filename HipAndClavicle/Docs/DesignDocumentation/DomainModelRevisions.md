# Domain Model Revisions

## Class after revision

### HipItem

HipItem represents a set of choices selected by the customer for a product they would like to purchase.

**Properties:**

- Product : ItemProduct
- SetSize : Size
- int : UnitPrice
- List\<Color> : Colors
- int : Quantity
- enum : ProductCategory

### ProductListing

This class is a culmination of Product and Listing as the had a majority of their properties in common.

**Properties:**

- string : Title
- List\<images> : Images
- string : Description
- double : SetPrice
- 
- ColorFamily : ColorFamily
- List\<SetSize> : SetSizes
- [NotMapped] IFormFile : TempFile

## Class Replacements

- Product -> ProductListing
- Listing -> ProductListing
- OrderItem -> HipItem
- ShoppingCartItem -> HipItem
- ProductImage -> None
