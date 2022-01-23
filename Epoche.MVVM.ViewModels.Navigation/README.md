# Navigable View Models

All navigation-related viewmodels are descended from `NavigableViewModel`

# Navigation Stack

Navigation is organized into a stack, for example: `AppViewModel -> AccountViewModel -> ChangePasswordViewModel`

This could translate into something like the following in HTML, where @Body is the content of the next ViewModel in the stack:

AppViewModel - `<html><head> ...styles, etc... </head><body>  ..header.. @Body ..footer..  </body>`

AccountViewModel - `<div class='account'> <h1>Account<h1> <div class='account-container'> @Body </div> </div>`

ChangePasswordViewModel - `<div class='change-password'> <h2> Change Password </h2> <form> ..change password form.. </form> </div>`

A `Presenter` might match this with a URL: `/account/password`

# Attributes

## ICanContain<TViewModel>

`ICanContain<TViewModel>` is attached to a viewmodel which can contain other viewmodels.

In our example above (`AppViewModel -> AccountViewModel -> ChangePasswordViewModel`): 
- `AppViewModel` uses `ICanContain<AccountViewModel>`
- `AccountViewModel` uses `ICanContain<ChangePasswordViewModel>`
- `ChangePasswordViewModel` is not tagged with `ICanContain<...>`

A viewmodel can be `ICanContain`ed by only a single type.  This is because when `.NavigateTo<ChangePasswordViewModel>` is called, 
the stack is constructed by looking for a parent with `ICanContain<ChangePasswordViewModel>`, and then for the parent's parent, etc,
until the root is found (nothing can `ICanContain` the root view model).

## IHaveDefaultContent

`IHaveDefaultContent` is attached to a viewmodel which might have default content.

Whenever a viewmodel with `IHaveDefaultContent` is the last in the stack, `IHaveDefaultContent.GetDefaultViewModelContentType()` is
called.  If it returns a viewmodel type, that viewmodel will be added to the end of the stack.

If we have the following:
- AppViewModel: IHaveDefaultContent (returns WelcomeViewModel)
- WelcomeViewModel: IHaveDefaultContent (returns DashboardViewModel)
- DashboardViewModel

By calling `.NavigateTo<AppViewModel>()`, the stack `AppViewModel -> WelcomeViewModel -> DashboardViewModel` would be created.

