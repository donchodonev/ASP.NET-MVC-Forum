# ASP.NET-MVC-Forum

* An ASP.NET(5) MVC Forum project
* Deployed at https://donchodonev.com but still in active development

# Main Technologies used

* ASP.NET 5
* MS SQL Server
* EF Core ORM
* HTML Sanitizer by Michael Ganss - https://github.com/mganss/HtmlSanitizer
* Automapper - Jimmy Bogard and others - https://automapper.org/
* HTML/CSS/JS
* Bootstrap
* IIS - for online deployment

# Functionalities implemented so far

* Registration - ASP.NET Identity
* Login - ASP.NET Identity
* Custom site user linked to Identity User (no Identity User pollution)

* Post Creation (via TinyMCE to make it look good)
* Post Reading
* Post Updating (by post author)
* Post Deleting (by post author)

* Validation for all possible user input
* User-friendly feedback with alert messages where needed

# TODO

* Admin Area
* Allow admin to delete and edit posts not created by himself
* Refactor existing code
* Post Votes
* Post comments