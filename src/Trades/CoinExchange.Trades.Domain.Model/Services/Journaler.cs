﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using CoinExchange.Trades.Domain.Model.OrderAggregate;
using CoinExchange.Trades.Domain.Model.OrderMatchingEngine;
using CoinExchange.Trades.Domain.Model.TradeAggregate;
using Disruptor;

namespace CoinExchange.Trades.Domain.Model.Services
{
    /// <summary>
    /// Journaler for saving events
    /// </summary>
    public class Journaler:IEventHandler<InputPayload>,IEventHandler<byte[]>
    {
        private IEventStore _eventStore;
        private InputPayload _receivedPayload;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="eventStore"></param>
        public Journaler(IEventStore eventStore)
        {
            _eventStore = eventStore;
            ExchangeEssentialsSnapshortEvent.ExchangeSnapshot += SnapshotArrived;
        }

        void SnapshotArrived(ExchangeEssentialsList exchangeEssentials)
        {
            _eventStore.SaveSnapshot(exchangeEssentials);
        }

        /// <summary>
        /// OnNext
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sequence"></param>
        /// <param name="endOfBatch"></param>
        public void OnNext(InputPayload data, long sequence, bool endOfBatch)
        {
            _receivedPayload = new InputPayload() { OrderCancellation = new OrderCancellation(), Order = new Order() };
            if (data.IsOrder)
            {
                data.Order.MemberWiseClone(_receivedPayload.Order);
                _receivedPayload.IsOrder = true;
                _eventStore.StoreEvent(_receivedPayload.Order);
            }
            else
            {
                data.OrderCancellation.MemberWiseClone(_receivedPayload.OrderCancellation);
                _receivedPayload.IsOrder = false;
                _eventStore.StoreEvent(_receivedPayload.OrderCancellation);
            }
        }

        /// <summary>
        /// OnNext
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sequence"></param>
        /// <param name="endOfBatch"></param>
        public void OnNext(byte[] data, long sequence, bool endOfBatch)
        {
            object getObject = ByteArrayToObject(data);
            if (getObject is Order)
            {
                _eventStore.StoreEvent(getObject as Order);
            }
            if (getObject is Trade)
            {
                _eventStore.StoreEvent(getObject as Trade);
            }
            if (getObject is LimitOrderBook)
            {
                LimitOrderBook limitOrderBook = getObject as LimitOrderBook;
                // Raise the event for the memory event to catch
                LimitOrderBookEvent.Raise(limitOrderBook);
                // Store the LimitOrderBook in the event store if no last snapshot datetime has been assigned or if the snapshot 
                // was taken yesterday and new day has begun
                /*if ((limitOrderBook.LastSnapshotTaken == DateTime.MinValue || 
                    limitOrderBook.LastSnapshotTaken.Day < DateTime.Today.Day))
                {
                    _eventStore.StoreEvent(limitOrderBook);
                    limitOrderBook.LastSnapshotTaken = DateTime.Now;
                }*/
            }
            if (getObject is Depth)
            {
               DepthEvent.Raise(getObject as Depth);
            }
            if (getObject is BBO)
            {
                BBOEvent.Raise(getObject as BBO);
            }
        }

        /// <summary>
        /// Converts byte array to object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// Gets the orders for rebuilding the LimitOrderBook state by replaying an event stream for orders
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOrdersForReplay(LimitOrderBook limitOrderBook)
        {
            List<Order> replayOrderList = new List<Order>();
            var orders = _eventStore.GetOrdersByCurrencyPair(limitOrderBook.CurrencyPair);
            if (orders != null)
            {
                foreach (Order order in orders)
                {
                    // Only going to choose those orders that were submitted after the last snapshot and are of state accepted 
                    // or Cancelled
                    if ( /*order.DateTime > limitOrderBook.LastSnapshotTaken && */
                        (order.OrderState == OrderState.Accepted || order.OrderState == OrderState.Cancelled))
                    {
                        replayOrderList.Add(order);
                    }
                }
            }
            if (replayOrderList.Count <= 0)
            {
                replayOrderList = null;
            }
            return replayOrderList;
        }

        /// <summary>
        /// Gets all the orders residing in the event store for the currenct running session
        /// </summary>
        /// <returns></returns>
        public List<Order> GetAllOrders()
        {
            return _eventStore.GetAllOrders();
        }

        public void ShutDown()
        {
            ExchangeEssentialsSnapshortEvent.ExchangeSnapshot -= SnapshotArrived;
        }
    }
}
