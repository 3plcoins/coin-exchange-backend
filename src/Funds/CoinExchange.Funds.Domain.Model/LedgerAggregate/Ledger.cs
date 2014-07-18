﻿using System;
using CoinExchange.Common.Domain.Model;
using CoinExchange.Funds.Domain.Model.CurrencyAggregate;
using CoinExchange.Funds.Domain.Model.DepositAggregate;

namespace CoinExchange.Funds.Domain.Model.LedgerAggregate
{
    /// <summary>
    /// Ledger VO
    /// </summary>
    public class Ledger
    {
        /// <summary>
        /// Database primary key
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Ledger ID
        /// </summary>
        public string LedgerId { get; private set; }

        /// <summary>
        /// Datetime
        /// </summary>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Type of Ledger
        /// </summary>
        public LedgerType LedgerType { get; private set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; private set; }

        /// <summary>
        /// Amount
        /// </summary>
        public double Amount { get; private set; }

        /// <summary>
        /// Fee
        /// </summary>
        public double Fee { get; private set; }

        /// <summary>
        /// Balance
        /// </summary>
        public double Balance { get; private set; }

        /// <summary>
        /// TradeId
        /// </summary>
        public string TradeId { get; private set; }

        /// <summary>
        /// Order ID
        /// </summary>
        public string OrderId { get; private set; }

        /// <summary>
        /// Withdraw ID
        /// </summary>
        public string WithdrawId { get; private set; }
        
        /// <summary>
        /// Deposit ID
        /// </summary>
        public string DepositId { get; private set; }

        /// <summary>
        /// Account ID
        /// </summary>
        public AccountId AccountId { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ledger()
        {
            
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="ledgerId"></param>
        /// <param name="dateTime"></param>
        /// <param name="ledgerType"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="balance"></param>
        /// <param name="tradeId"></param>
        /// <param name="orderId"></param>
        /// <param name="withdrawId"></param>
        /// <param name="depositId"></param>
        /// <param name="accountId"></param>
        public Ledger(string ledgerId, DateTime dateTime, LedgerType ledgerType, Currency currency, double amount, double fee, double balance, string tradeId, string orderId, string withdrawId, string depositId, AccountId accountId)
        {
            LedgerId = ledgerId;
            DateTime = dateTime;
            LedgerType = ledgerType;
            Currency = currency;
            Amount = amount;
            Fee = fee;
            Balance = balance;
            TradeId = tradeId;
            OrderId = orderId;
            WithdrawId = withdrawId;
            DepositId = depositId;
            AccountId = accountId;
        }
    }
}
