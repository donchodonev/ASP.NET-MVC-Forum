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
* jQuery
* IIS - for online deployment

# Functionalities implemented so far

* Registration - ASP.NET Identity
* Login - ASP.NET Identity
* Custom site user linked to Identity User (no Identity User pollution)

* Post Creation (via TinyMCE to make it look good)
* Post Reading
* Post Updating (by post author)
* Post Deleting (by post author)

* Post commenting and viewing done via AJAX and comments API (with anti-CSRF token) (by authenticated users)
* Post comment time elapsed since each comment is made in seconds/minutes/hours/days
* Post comment count
* Post comment deletion validation
* Post comment deletion option shown only for comment author and admin


* Validation for all possible user input
* User-friendly feedback with alert messages where needed

* Per-category post filtration

* In-Memory caching for the site's home page regenerated every 60 seconds

# TODO

* Admin Area
* Refactor existing code