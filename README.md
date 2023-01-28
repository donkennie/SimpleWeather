# SimpleWeather
Create a simple weather RESTful API using .NET6 including authentication and authorization

###  Overview
> - An API that gets all your essential weather data for a specific location with an authenticated user 

> - The current weather endpoint can be accessed by an authenticated user with a verified bearer token.

> ### Features implemented:

> - Using CQRs and MediatR Pattern 

> ### Technlogies and packages used:

> - MediatR.Extensions.Microsoft.DependencyInjection
> - Microsoft.EntityFrameworkCore
> - MediatR
> - MySqlServer
> - Newtonsoft.Json
> - Microsoft.EntityFrameworkCore.Tools

> ### Pictoral Explanation

> - The three endpoints to fetch the current weather and login & register

![Screenshot (38)](https://user-images.githubusercontent.com/88739172/215126744-75c3009a-c041-439f-b40e-6c738e076a63.png)

![Screenshot (36)](https://user-images.githubusercontent.com/88739172/215126801-db67edcd-7de9-48c0-bdc0-8a666b72339c.png)

> - Swagger API documentation for the project 

> - placement of beareer token at the top of the header

![Screenshot (40)](https://user-images.githubusercontent.com/88739172/215234351-0e82121d-4588-456e-aeb6-44704835921e.png)

> - Consuming Web API return type through exposing the API capabilities by defining it as a filter that specifies the type of the value and status code returned by the action.
![Screenshot (37)](https://user-images.githubusercontent.com/88739172/215126783-b9a1de98-509c-4147-ab4d-66a04d4db303.png)
