# Saga pattern workshop
This repository contains the source code of the Saga pattern workshop from doubleloop.io
To run the application follow these steps:
- install .NET Core framework version 2.1 from https://dotnet.microsoft.com/download
- clone this repository ```git clone git@github.com:doubleloop-io/saga-pattern-workshop.git```
- enter the root folder ```cd saga-pattern-workshop```
- run ```dotnet test``` to build everything and check that it is working
- enter SagaPattern folder ```cd SagaPattern```
- run ```dotnet run``` in order to execute the sample application.
You should see an output like this:
```
10:07:31.8068|3|InventoryMessages+AddSeats
10:07:31.8280|4|InventoryMessages+SeatsAdded
10:07:31.8280|1|InventoryMessages+RemoveSeats
10:07:31.8438|9|InventoryMessages+SeatsRemoved
10:07:31.8438|8|InventoryMessages+MakeSeatsReservation
10:07:31.8609|9|InventoryMessages+SeatsReservationAccepted
10:07:31.8609|1|InventoryMessages+MakeSeatsReservation
10:07:31.8609|5|InventoryMessages+SeatsReservationAccepted
10:07:31.8609|1|InventoryMessages+MakeSeatsReservation
10:07:31.8746|8|InventoryMessages+SeatsReservationRejected
10:07:31.8746|1|InventoryMessages+CommitSeatsReservation
10:07:31.8746|12|InventoryMessages+SeatsReservationCommitted
10:07:31.8912|1|InventoryMessages+CancelSeatsReservation
10:07:31.8912|3|InventoryMessages+SeatsReservationCanceled
10:07:31.9070|1|PaymentMessages+MakePayment
10:07:34.9217|6|PaymentMessages+PaymentAccepted
10:07:34.9721|8|PaymentMessages+MakePayment
10:07:36.9868|8|PaymentMessages+PaymentRejected
10:07:37.0464|8|PaymentMessages+MakePayment
10:07:38.0478|6|PaymentMessages+PaymentRejected
10:07:38.1469|8|PricingMessages+CalculatePrice
10:07:38.1469|10|PricingMessages+PriceCalculated
10:07:38.1567|8|SellingMessages+PlaceOrder
10:07:38.1567|3|SellingMessages+OrderPlaced
10:07:38.1567|8|SellingMessages+BookOrder
10:07:38.1803|5|SellingMessages+OrderBooked
10:07:38.1878|8|SellingMessages+PriceOrder
10:07:38.1878|3|SellingMessages+OrderPriced
10:07:38.1878|8|SellingMessages+SetCustomer
10:07:38.2095|6|SellingMessages+CustomerSet
10:07:38.2095|8|SellingMessages+ConfirmOrder
10:07:38.2287|12|SellingMessages+OrderConfirmed
10:07:38.2287|8|SellingMessages+PlaceOrder
10:07:38.2287|5|SellingMessages+OrderPlaced
10:07:38.2392|8|SellingMessages+CancelOrder
10:07:38.2392|10|SellingMessages+OrderCanceled
Press <ENTER> to exit
```
