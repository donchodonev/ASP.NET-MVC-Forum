# ASP.NET-MVC-Forum

* An ASP.NET(5) MVC Forum project
* Deployed at https://donchodonev.com but still in active development

# Main Technologies / Nu-Get packages used

* ASP.NET 5
* MS SQL Server
* EF Core ORM
* HTML Sanitizer by Michael Ganss - https://github.com/mganss/HtmlSanitizer
* Automapper - Jimmy Bogard and others - https://automapper.org/
* Profanity Detector - Stephen Haunts -  https://github.com/stephenhaunts/ProfanityDetector
* Image Sharp -  [James Jackson-South](https://github.com/jimbobsquarepants), [Dirk Lemstra](https://github.com/dlemstra), [Anton Firsov](https://github.com/antonfirsov) , [Scott Williams](https://github.com/tocsoft) ,[Brian Popow](https://github.com/brianpopow) - https://github.com/SixLabors/ImageSharp
* SendGrid API
* HTML/CSS/JS
* Bootstrap
* jQuery
* Chart.js
* IIS - for deployment to WWW

# Functionalities implemented so far

* Registration - ASP.NET Identity, with the ability to register/login with Facebook
* Login - ASP.NET Identity
* Custom site base-user linked to Identity User (no Identity User pollution)

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
* Post auto-reporting on-creation or edit, when profane words are contained in post title or content
* Post censoring with Profanity Detector or with Regex pattern-matching
* Post deletion/restoration along with report 
* Post comment deletion/restoration along with report 

* Post up/dpwn voting with AJAX and WebAPI

* Admin area
* Admin area dashboart with charts on the main page
* Chart supplement data (post title, link to post title, count of entities)
* Each chart represents real data and upon selecting a new chart from the dropdown menu - chart data is updated with AJAX and WebAPI, this also triggers the removal of supplement post data surrounding the chart and the chart image download button. Upon a successful chart data update - the html list elements are re-drawn , the same applies for the chart image download button

* Post report resolving and restoring (admin has the option to visit Post from the report card and/or view report text content and how much time has passed since it's creation)
* Marking post report as resolved while marking the post itself as deleted automatically

* User banning and unbanning
* User banning leads to inability to login
* User banning leads to immediate (banned)user log-out
* User promoting and demoting
* User promoting/demoting leads to immediate (banned)user log-out to reset claims
* Users can upload an image which would then be used as their avatar (image is stored locally as a physical file, type-checked, name GUIDified to prevent avatars for-eaching and for better user-image anonimity)
* User image resizing before upload to reduce site traffic and improve page-load time (default is 50x50px)


* Validation for all possible user input
* User-friendly feedback with alert messages where needed

* Per-category post filtration

* In-Memory caching for the site's home page regenerated every 60 seconds

# TODO

* Refactor existing code