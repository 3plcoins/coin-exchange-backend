﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using CoinExchange.Funds.Domain.Model.VOs;
using CoinExchange.Trades.Domain.Model.Entities;
using CoinExchange.Trades.Infrastructure.Services.Services;

namespace CoinExchange.Trades.Port.Adapter.RestService
{
    /// <summary>
    /// Rest service for serving requests related to Trades
    /// </summary>
    public class TradesRestService : ApiController
    {
        private TradesService _tradesService;

        public TradesRestService()
        {
            _tradesService = new TradesService();
        }

        /// <summary>
        /// Returns orders that have not been executed but those that have been accepted on the server. Exception can be 
        /// provided in the second parameter
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        public List<Order> OpenOrderList(TraderId traderId, bool includeTrades = false, string userRefId = "")
        {
            // ToDo: In the next sprint related to business logic behind RESTful calls, need to split the ledgersIds comma
            // separated list
            return _tradesService.GetOpenOrders();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// Params:
        /// 1. TraderId(ValueObject)[FromBody]: Contains an Id of the trader, used for authentication of the trader
        /// 2. includeTrades(bool): Include trades as well in the response(optional)
        /// 3. userRefId: Restrict results to given user reference id (optional)
        /// </summary>
        /// <returns></returns>
        public List<Order> GetClosedOrders(TraderId traderId, bool includeTrades = false, string userRefId = "",
            string startTime = "", string endTime = "", string offset = "", string closetime = "both")
        {
            return _tradesService.GetClosedOrders();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="offset">Result offset</param>
        /// <param name="type">Type of trade (optional) [all = all types (default), any position = any position (open or closed), closed position = positions that have been closed, closing position = any trade closing all or part of a position, no position = non-positional trades]</param>
        /// <param name="trades">Whether or not to include trades related to position in output (optional.  default = false)</param>
        /// <param name="start">Starting unix timestamp or trade tx id of results (optional.  exclusive)</param>
        /// <param name="end">Ending unix timestamp or trade tx id of results (optional.  inclusive)</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> GetTradeHistory([FromBody]TraderId traderId, string offset = "", string type = "all",
            bool trades = false, string start = "", string end = "")
        {
            return _tradesService.GetTradesHistory();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> FetchQueryTrades([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            return _tradesService.GetTradesHistory();
        }

        /// <summary>
        /// Returns orders of the user that have been filled/executed
        /// <param name="traderId">Trader ID</param>
        /// <param name="txId">Comma separated list of txIds</param>
        /// <param name="includeTrades">Whether or not to include the trades</param>
        /// </summary>
        /// <returns></returns>
        public List<Order> TradeBalance([FromBody]TraderId traderId, string txId = "", bool includeTrades = false)
        {
            return _tradesService.GetTradesHistory();
        }
    }
}
