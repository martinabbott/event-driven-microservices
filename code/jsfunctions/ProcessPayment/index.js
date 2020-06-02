var uuid = require('uuid').v4;
var msRestAzure = require('ms-rest-azure');
var eventGrid = require("azure-eventgrid");
const url = require('url');

module.exports = async function (context, eventGridEvent) {
  
  context.log.warn('ProcessPayment - Processing Payment event');

  currentDate = new Date();
    
  context.bindings.outputEvent = {
    id: uuid(),
    subject: 'payment/paymentProcess',
    dataVersion: '2.0',
    eventType: 'orderAccepted',
    data: {
      orderId : eventGridEvent.data.orderId,
      totalValue: eventGridEvent.data.totalValue,
      orderStatus: "accepted",
      docId: eventGridEvent.data.docId
    },
    eventTime: currentDate
  };

  context.log.warn('ProcessPayment - Sending Order status update');
  
  context.done();

};