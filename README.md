# Event Driven Microservices with Azure Functions, Event Grid and Cosmos DB

This repository contains the code delivered as a demonstration during my presentation at a few conferences in 2020, including Microsoft Ignite the Tour Sydney, Global Integration Bootcamp and Integrate 2020 Remote.

The code uses Azure Functions, Event Grid and Cosmos DB to demonstrate event-driven design. The code is nothing more than scaffold, there is no exception handling, failure handling or any logging, auditing, security or tracing that would be built in to a final solution, and as such is offered as is.

The demonstration follows a very basic order processing scenario that uses mulitple langauges and data stores. Whilst the implementation is split by language, a real implementation would be split by domain objects and data stores.
[IMAGE OF SCENARIO]
![alt text](https://github.com/martinabbott/event-driven-microservices/blob/master/images/demo-scenario.png "Demo Scenario")

## Prerequisites
### Azure Subscription Setup
You will need to create the following artefacts in a Microsoft Azure subscription.
![alt text](https://github.com/martinabbott/event-driven-microservices/blob/master/images/azure-subscription.png "Azure Subscription Setup")

You will need to change the names of the artefacts to ensure there is no collision with globally available names.

In the MongoDB Cosmos DB instance, create a database called *store* with collections called *cart* and *leases*.

In the SQL API Cosmos DB instance, create a database called *orderdb* with collections called *orders* and *leases*

## Visual Studio Code

Make sure you have the Visual Studio Code Azure Functions extension installed and in order to run the Python function you will need to install Python and configure Azure Functions so that it knows where to find your executable.

Once you've cloned the repository, just go to each language folder and open that folder in Visual Studio Code and start the Azure Functions runtime.

## ngrok
The demonstration uses [ngrok](https://ngrok.com/) to allow routing from Azure Event Grid to the Azure Functions running locally. There need to be 3 instances of ngrok running bound to ports 7071, 7072 and 7073.

To create the Event Grid subscriptions in the Azure portal, use the following when defining the web hooks for each subscription within the Azure Portal.

### cartCheckout Event Grid Topic
`[ngrok HTTPS forwarding address for port 7071]/runtime/webhooks/EventGrid?functionName=OrderSubmitted`

### orderStatus Event Grid Topic
`[ngrok HTTPS forwarding address for port 7071]/runtime/webhooks/EventGrid?functionName=OrderProcess`

### paymentProcess Event Grid Topic
`[ngrok HTTPS forwarding address for port 7072]/runtime/webhooks/EventGrid?functionName=ProcessPayment`

### backorderProcess Event Grid Topic
`[ngrok HTTPS forwarding address for port 7073]/runtime/webhooks/EventGrid?functionName=ProcessInventory`

The forwarding address is obtained from the following.
![alt text](https://github.com/martinabbott/event-driven-microservices/blob/master/images/ngrok.png "ngrok forwarding address")

NOTE: In order to create the Event Grid subscriptions you will need to have the 3 instances of ngrok running and the 3 instances of the Azure Functions runtime running locally (one for C#, one for JavaScript and one for Python) as the webhook endpoints need to be available when creating the subscriptions in the Azure Portal.

## Running the demo
In the postman folder are two collections, one for publishing data to the cart and one for checking data in the order collection.

To run the demo, first use the InsertCart request. This creates a cart record in the Mongo DB Cosmos DB cart collection.

Next, use the CheckoutCart request. This updates the cart record and creates an Event Grid event.

Once the initial event is created, all other events such as Event Grid events or Cosmos DB Change Feed events just cascade and the code can be debugged normally.
