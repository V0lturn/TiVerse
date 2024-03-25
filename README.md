# TiVerse - buy tickets for almost any type of transport
* A website for purchasing tickets for different modes of transport
___

## Features
* Search for a route and display transport for a given route
* The main page displays the most popular routes
* View bus/rail/air routes between cities/countries
* Viewing routes with specified filter parameters
* A user profile in which you can add information, view planned trips, trip history and top up your account balance
* Adding and deleting routes
* Localization for Ukrainian and English
* Notify all users when a new route is added

___

The project was built based on clean architecture. 
It is based on an asp.net mvc application.
An Entity Framework was used to work with the database.
Authentication, authorization and registration were done using Duende identity.
Notification when adding a new route was done using SignalR.
An API has also been added to which only an authorized user can access. Verified using access token.

