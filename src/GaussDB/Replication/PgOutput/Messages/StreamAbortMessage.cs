﻿using GaussDBTypes;
using System;

namespace GaussDB.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol stream abort message
/// </summary>
public sealed class StreamAbortMessage : TransactionControlMessage
{
    /// <summary>
    /// Xid of the subtransaction (will be same as xid of the transaction for top-level transactions).
    /// </summary>
    public uint SubtransactionXid { get; private set; }

    internal StreamAbortMessage() {}

    internal StreamAbortMessage Populate(GaussDBLogSequenceNumber walStart, GaussDBLogSequenceNumber walEnd, DateTime serverClock,
        uint transactionXid, uint subtransactionXid)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);
        SubtransactionXid = subtransactionXid;
        return this;
    }
}