const uuid = require('uuid').v4;
const msRestAzure = require('ms-rest-azure');
const eventGrid = require("azure-eventgrid");
const url = require('url');

module.exports = async function (context, documents) {

    if (!!documents && documents.length > 0) {
        context.log.warn('FulfilOrder - Processing Order Change Feed');

        if (documents[0].orderStatus == "accepted")
        {
            currentDate = new Date();

            context.bindings.outputEvent = {               
                id: uuid(),
                subject: 'order/item/lowstock',
                dataVersion: '2.0',
                eventType: 'backorder',
                data: {
                orderId : documents[0].orderId,
                },
                eventTime: currentDate
            };

            context.log.warn('FulfilOrder - Sending Backorder event');
            
            context.done();
            
        }
    }
}
