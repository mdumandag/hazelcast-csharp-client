// Copyright (c) 2008-2020, Hazelcast, Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using Hazelcast.Client.Protocol;
using Hazelcast.Client.Protocol.Codec.BuiltIn;
using Hazelcast.Client.Protocol.Codec.Custom;
using Hazelcast.Client.Protocol.Util;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using static Hazelcast.Client.Protocol.Codec.BuiltIn.FixedSizeTypesCodec;
using static Hazelcast.Client.Protocol.ClientMessage;
using static Hazelcast.IO.Bits;

namespace Hazelcast.Client.Protocol.Codec
{
    // This file is auto-generated by the Hazelcast Client Protocol Code Generator.
    // To change this file, edit the templates or the protocol
    // definitions on the https://github.com/hazelcast/hazelcast-client-protocol
    // and regenerate it.

    /// <summary>
    /// Reads from the map event journal in batches. You may specify the start sequence,
    /// the minumum required number of items in the response, the maximum number of items
    /// in the response, a predicate that the events should pass and a projection to
    /// apply to the events in the journal.
    /// If the event journal currently contains less events than {@code minSize}, the
    /// call will wait until it has sufficient items.
    /// The predicate, filter and projection may be {@code null} in which case all elements are returned
    /// and no projection is applied.
    ///</summary>
    internal static class MapEventJournalReadCodec
    {
        //hex: 0x014200
        public const int RequestMessageType = 82432;
        //hex: 0x014201
        public const int ResponseMessageType = 82433;
        private const int RequestStartSequenceFieldOffset = PartitionIdFieldOffset + IntSizeInBytes;
        private const int RequestMinSizeFieldOffset = RequestStartSequenceFieldOffset + LongSizeInBytes;
        private const int RequestMaxSizeFieldOffset = RequestMinSizeFieldOffset + IntSizeInBytes;
        private const int RequestInitialFrameSize = RequestMaxSizeFieldOffset + IntSizeInBytes;
        private const int ResponseReadCountFieldOffset = ResponseBackupAcksFieldOffset + ByteSizeInBytes;
        private const int ResponseNextSeqFieldOffset = ResponseReadCountFieldOffset + IntSizeInBytes;
        private const int ResponseInitialFrameSize = ResponseNextSeqFieldOffset + LongSizeInBytes;

        public static ClientMessage EncodeRequest(string name, long startSequence, int minSize, int maxSize, IData predicate, IData projection)
        {
            var clientMessage = CreateForEncode();
            clientMessage.IsRetryable = true;
            clientMessage.OperationName = "Map.EventJournalRead";
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], UnfragmentedMessage);
            EncodeInt(initialFrame.Content, TypeFieldOffset, RequestMessageType);
            EncodeInt(initialFrame.Content, PartitionIdFieldOffset, -1);
            EncodeLong(initialFrame.Content, RequestStartSequenceFieldOffset, startSequence);
            EncodeInt(initialFrame.Content, RequestMinSizeFieldOffset, minSize);
            EncodeInt(initialFrame.Content, RequestMaxSizeFieldOffset, maxSize);
            clientMessage.Add(initialFrame);
            StringCodec.Encode(clientMessage, name);
            CodecUtil.EncodeNullable(clientMessage, predicate, DataCodec.Encode);
            CodecUtil.EncodeNullable(clientMessage, projection, DataCodec.Encode);
            return clientMessage;
        }

        public class ResponseParameters
        {

            /// <summary>
            /// Number of items that have been read.
            ///</summary>
            public int ReadCount;

            /// <summary>
            /// List of items that have been read.
            ///</summary>
            public IList<IData> Items;

            /// <summary>
            /// Sequence numbers of items in the event journal.
            ///</summary>
            public long[] ItemSeqs;

            /// <summary>
            /// Sequence number of the item following the last read item.
            ///</summary>
            public long NextSeq;
        }

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            var iterator = clientMessage.GetIterator();
            var response = new ResponseParameters();
            var initialFrame = iterator.Next();
            response.ReadCount = DecodeInt(initialFrame.Content, ResponseReadCountFieldOffset);
            response.NextSeq = DecodeLong(initialFrame.Content, ResponseNextSeqFieldOffset);
            response.Items = ListMultiFrameCodec.Decode(iterator, DataCodec.Decode);
            response.ItemSeqs = CodecUtil.DecodeNullable(iterator, LongArrayCodec.Decode);
            return response;
        }

    }
}