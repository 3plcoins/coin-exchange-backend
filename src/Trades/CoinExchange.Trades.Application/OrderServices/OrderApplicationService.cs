﻿using System;
using System.IO;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Trades.Application.OrderServices.Commands;
using CoinExchange.Trades.Application.OrderServices.Representation;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.Services;
using Spring.Context;
using Spring.Context.Support;

namespace CoinExchange.Trades.Application.OrderServices
{
    /// <summary>
    /// Real implementation of order application service
    /// </summary>
    public class OrderApplicationService : IOrderApplicationService
    {
        private ICancelOrderCommandValidation _commandValidationService;
        
        public OrderApplicationService(ICancelOrderCommandValidation cancelOrderCommandValidation)
        {
            _commandValidationService = cancelOrderCommandValidation;
        }

        public CancelOrderResponse CancelOrder(CancelOrderCommand cancelOrderCommand)
        {
            try
            {
                // Verify cancel order command
                if (_commandValidationService.ValidateCancelOrderCommand(cancelOrderCommand))
                {
                    string currencyPair = _commandValidationService.GetCurrencyPair(cancelOrderCommand.OrderId);
                    OrderCancellation cancellation = new OrderCancellation(cancelOrderCommand.OrderId,
                                                                           cancelOrderCommand.TraderId,currencyPair);
                    InputDisruptorPublisher.Publish(InputPayload.CreatePayload(cancellation));
                    return new CancelOrderResponse(true, "Cancel Request Accepted");
                }
                return new CancelOrderResponse(false, new InvalidDataException("Invalid orderid").ToString());
            }
            catch (Exception exception)
            {
                return new CancelOrderResponse(false, exception.Message);
            }
        }

        public NewOrderRepresentation CreateOrder(CreateOrderCommand orderCommand)
        {
            IOrderIdGenerator orderIdGenerator = ContextRegistry.GetContext()["OrderIdGenerator"] as IOrderIdGenerator;
            // ToDo: Inject the FundsConfirmationService residing in Infrastructure Services here and confirm that the 
            // user has enough balance present to send the order. Proceed if enough balance present
            Order order = OrderFactory.CreateOrder(orderCommand.TraderId, orderCommand.Pair,
                orderCommand.Type, orderCommand.Side, orderCommand.Volume, orderCommand.Price, orderIdGenerator);
            
            InputDisruptorPublisher.Publish(InputPayload.CreatePayload(order));
            return new NewOrderRepresentation(order);
        }
    }
}
