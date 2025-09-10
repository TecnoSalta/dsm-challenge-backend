Car Rental
==============================

The car rental system at the airport will allow users to manage rentals efficiently and personally. A customer can check the availability of cars based on the date and desired features.
Register a rental by selecting a start date, specifying the vehicle characteristics they want (such as type and model), and defining the rental period by setting a return date.
Additionally, the system will allow customers to modify their reservation details, such as changing the start date, extending the ret       urn date, or changing the selected vehicle, as well as canceling a previously registered rental if their plans change. In this way, the system fulfills the processes of booking, consultation, modification, and cancellation.
To complete a Rental, the following information must be known about the Customer: ID, full name, address, rental start and end days, and car type. The system will show options according to availability.
After the rental is completed, that car cannot be rented for the next day.
All cars have a service every 2 months that lasts 2 days.
A car can only be assigned to one customer at a time.
On the other hand, management needs to generate a daily list of cars that have scheduled services in the next two weeks. The list should include the model, car type, and service date.

![](https://www.plantuml.com/plantuml/svg/SoWkIImgAStDuL8ZDJ2qpIJCEKbd1oX8IyS_qCi2KjL8BpxI6TvPyQQFLJYn3HeTGyV5HItJ8yUm4qA7yS0)

Directory structure
-------------------------------
PWC.Challenge.Domain/
  Customers/
    Customer.cs
  Rentals/
    Rental.cs
  Cars/
    Car.cs
    Service.cs

