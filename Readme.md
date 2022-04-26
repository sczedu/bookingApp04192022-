# Booking Application - ASP.NET Core Web API Serverless Application

This project shows how to run an ASP.NET Core Web API project as an AWS Lambda exposed through Amazon API Gateway. The NuGet package [Amazon.Lambda.AspNetCoreServer](https://www.nuget.org/packages/Amazon.Lambda.AspNetCoreServer) contains a Lambda function that is used to translate requests from API Gateway into the ASP.NET Core framework and then the responses from ASP.NET Core back to API Gateway.


### Project Files ###

* serverless.template - an AWS CloudFormation Serverless Application template file configurated for Serverless functions and other AWS resources like DynamoDB/ApiGateway/...

## Here are some steps to follow from Visual Studio:

To deploy this application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to application.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "bookingApp04192022/test/Application.Tests"
    dotnet test
    cd "bookingApp04192022/test/Web.Tests"
    dotnet test
```

Deploy application
```
    cd "bookingApp04192022/src/Web"
    dotnet lambda deploy-serverless
```

## DynamoDB configuration from the command line at aws cloudshell:

After tables created, at first time, needs to setup configuration table using:
```
    aws dynamodb put-item --table-name BookingService_Configuration --item '{"Id": {"S": "CONFIG"},"ReservationStartsAt": {"S": "00:00:00"},"ReservationEndsAt": {"S": "23:59:59"},"MaximumReservationDays": {"N": "3"},"MaximumEndReservationDays": {"N": "30"}}'
```

## Appplication API:

This application have swagger to facilitate, can be access by ```...swagger/index.html```

Method names and the corresponding routes:

| Service Method Name                                   | HTTP Method | Route                      |
| ----------------------------------------------------- | ----------- | -------------------------- |
| GetReservation(string id)                             | GET         | /reservation/{id}          |
| AvailabilityCheck(DateTime Starts, Datetime Ends)     | GET         | /reservation/availability  |
| CreateReservation(ReservationRequest reservation)     | POST        | /reservation               |
| EditReservation(ModifyReservationRequest reservation) | PUT         | /reservation               |
| ReservationCancel(string id)                          | DELETE      | /reservation/{id}          |


- **Get**: '**/reservation/{id}**' Get information about specific **id** reservation.
- **Get**: '**/reservation/availability**' Get information about availability.
- **Post**: '**/reservation**' Place reservation.
- **Put**: '**/reservation**' Modify dates of reservation.
- **Delete**: '**/reservation/{id}**' Cancel a specific reservation.
