# Project Update & Notes 4/12

## Domain Model

Here are some changes and additions that have been made to the domain model.

### Additions

- **SetSize** - This class represents the number of items that come in a set of OrderItems.
- **ColorFamily** - This class will be useed to group colors into larger groups.l These groups will be navigable via the search by color feature.
- **CustomerProductCatalogController**, <br /> **CustomerCatalogVM**, <br /> **CustomerProductCatalog/SearchByColor**,<br /> **ViewAllProducts**
These are the classes and files being used by Michael to impliment the  search by color and view all products features.

### Changes

- **Seed Data** has been updated and split into separate modules. This will allow us to easily add new data as we need it individually.
- **Repositories** The name of the **HipRepo** will be changing to **AdminRepo**. This is to signify and reflect that each of us may be using our own repository's during project development. Common repository methods will be combines at a later stage of the project.

If you don't see some changes in your local branch after integrating main into your working branch or you experience errors during the process, please contact Devin for details on how to update your local branch while avoiding merge conflicts.
