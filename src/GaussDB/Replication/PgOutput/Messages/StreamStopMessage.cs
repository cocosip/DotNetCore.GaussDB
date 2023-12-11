﻿using GaussDBTypes;
using System;

namespace GaussDB.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol stream stop message
/// </summary>
public sealed class StreamStopMessage : PgOutputReplicationMessage
{
    internal StreamStopMessage() {}

    internal new StreamStopMessage Populate(GaussDBLogSequenceNumber walStart, GaussDBLogSequenceNumber walEnd, DateTime serverClock)
    {
        base.Populate(walStart, walEnd, serverClock);
        return this;
    }
}