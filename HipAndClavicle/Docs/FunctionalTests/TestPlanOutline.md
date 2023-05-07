# Hip and Clavicle Functional Tests Outline

This functional test outline is intended to be used as a guide for testing the Hip and Clavicle website. The outline is broken down into sections that represent the different areas of the site. Each section contains a list of test scenarios that should be tested for that section. Each test scenario contains a list of steps that should be followed to test the scenario. The steps are numbered and should be followed in order.

Additionally, There is some preperation that is needed before running the test. The preperation steps are listed below:

1. Run the bash script to refresh the Database. This is necessary because the test data for some of the views will need to be deleted in order to test the functionality accurately. To do this, open a terminal and navigate to the root of the project. Then run the following command:

```bash
sh .Scripts/rf-db.sh
```

2. The tests should be run in a semi specific order. The Register and Login tests should be run first. This is because the tests that follow require a user to be logged in. The tests that require a user to be logged in are marked with a `*` in the outline below. `**` indicates that the test requires an admin user to be logged in.

---

## Test Scenarios

### Registration

Test Registration functionality

1. Check that the user can register to the site using the Register link located in LoginPartial.cshtml
2. Check that a user is able to either register with or without entering an address.

### Login/Logout

1. Check that a registered user can login to the site using the Login link located in LoginPartial.cshtml
2. Check that a logged in user is able to logout of the site using the Logout link located in LoginPartial.cshtml

### User Profile *

1. Check that a logged in user is able to view their profile by clicking on their username in the top right corner of the page.
2. Check that a logged in user is able to edit their profile while viewing the user profile page.
3. Check that a users profile information is successfully changed when the user edits their profile.
4. Check that an administrator is able to navigate to the user profile page from the CurrentOrders Page using the edit link located on the displayed Address.

### Current Orders *

1. Check that the administrator is able to navigate to the Current Orders page using the Current Orders link located in the Admin menu from the LoginPartial.
2. Check that the administrator is able to use the ship link on an order to navigate to the shipment screen using the ship link on an order.
3. check that the selected orders information is applied to the form on the shipping page.
4. check that the pricing and content of the order is accurate IE does the total equal all the prices of the items in the order?.

### Admin Products **

1. Check that the admin is able to navigate to the Add Product page by using the the new product link on the Admin Products page.
2. Check that the admin is able to add a product to the site using the Add Product page.
3. Check that the admin is able to edit a product by using the edit link on a product.
4. Check that the admin is able to delete a product by using the delete link on a product.

### Shopping Cart *

1. A user can navigate to the Shopping Cart to view their added items.
2. A user that is signed in is able to save a shopping cart to come back to later.

### Customer Catalog View *

1. Check if the a user that is either logged in or not is able to navigate to the listings view to see all the available listings.
2. Check that a user is able to select a listing from the catalog view and navigate to the Listing details view.

## TODO

- [ ] Add tax info to an order.
- [ ] Add color editor to Product.
- [ ] fix images on product page.
- [ ] Add a delete button for a product
- [ ] Add image upload on product edit page.
- [ ] Make shopping cart icon only visible to non admins.
- [ ] Add user profile delete.
