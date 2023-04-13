# Project Update & Notes 4/12

## sprint report meeting

- See if client can attend the sprint meeting or have a discord meeting to provide feedback. **Fri, Sat** or **Sun**
- Sprint Planning meeting tomorrow Fri 4/14.
  - who is the SCRUM master?
  - Burndown chart

---
  
## Reminders

- are you moving your issues on the board as you work on them?

---

## Optional stuff

### <ins>Nehemiah</ins>

IF you want to use these, here are the bootstrap icons for IG and FB. <br />You are by no means required to use them, but i'm putting  them here for easy access if you do.

```html
<i class="bi bi-instagram"></i>
<i class="bi bi-facebook"></i>
```

### <ins>Navigation paths currently in main</ins>

these are asp-action and asp-controller paths that we currently have on gh,

#### Admin Home

currently this is the admins current orders view, but it will be the admin dashboard after development is finished on both pages.

because these should be hidden unless the administrator is logged in, these will be located in a _LoginPartial.cshtml file.

```html
<a asp-action="Index" asp-controller="Admin">Administrator</a>
<a asp-action="Orders" asp-controller="Admin">Admin Home</a>
```

#### Customer

wow someone sure does love typing... :laughing:.

```html
<a asp-action="SearchByColor" asp-controller="CustomerProductCatalogController"></a>
<a asp-action="ViewAllProducts" asp-controller="CustomerProductCatalogController"></a>
```

#### Account

**Steven** if you have tikme before the end of the sprint, you could impliment the LoginPartial.cshtml file and add these links to it. There is an example in the prototype. If you would like to scaffold it in let me know a good time and I can walk you through that process. This could also optionally be done next sprint. 

***(TODO add LoginPartial user story or subissue to the project board.)***

```html
<a asp-action="Login" asp-controller="Account">Login</a>
<a asp-action="Register" asp-controller="Account">Register</a>
<a asp-action="Index" asp-controller="Account">User Profile</a>
```
