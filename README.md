# ASP.NET-MVC-Forum

* An ASP.NET(5) MVC Forum project
* Deployed at https://donchodonev.com but still in active development

# Main Technologies used

* ASP.NET 5
* MS SQL Server
* EF Core ORM
* HTML Sanitizer by Michael Ganss - https://github.com/mganss/HtmlSanitizer
* Automapper - Jimmy Bogard and others - https://automapper.org/
* Profanity Detector - Stephen Haunts -  https://github.com/stephenhaunts/ProfanityDetector
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
* Post reporting
* Post auto-reporting on-creation when profane words are contained in post title or content

* Admin area
* Post report resolving and restoring (admin has the option to visit Post from the report card and/or view report text content and how much time has passed since it's creation)
* Marking post report as resolved while marking the post itself as deleted automatically

* User banning and unbanning
* User banning leads to inability to login
* User banning leads to immediate (banned)user log-out

* User promoting and demoting
* User promoting/demoting leads to immediate (banned)user log-out to reset claims

* Validation for all possible user input
* User-friendly feedback with alert messages where needed

* Per-category post filtration

* In-Memory caching for the site's home page regenerated every 60 seconds

# TODO

* Refactor existing code