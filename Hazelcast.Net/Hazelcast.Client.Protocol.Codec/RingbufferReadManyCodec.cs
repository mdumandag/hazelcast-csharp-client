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
    /// Reads a batch of items from the Ringbuffer. If the number of available items after the first read item is smaller
    /// than the maxCount, these items are returned. So it could be the number of items read is smaller than the maxCount.
    /// If there are less items available than minCount, then this call blacks. Reading a batch of items is likely to
    /// perform better because less overhead is involved. A filter can be provided to only select items that need to be read.
    /// If the filter is null, all items are read. If the filter is not null, only items where the filter function returns
    /// true are returned. Using filters is a good way to prevent getting items that are of no value to the receiver.
    /// This reduces the amount of IO and the number of operations being executed, and can result in a significant performance improvement.
    ///</summary>
    internal static class RingbufferReadManyCodec
    {
        //hex: 0x170900
        public const int RequestMessageType = 1509632;
        //hex: 0x170901
        public const int ResponseMessageType = 1509633;
        private const int RequestStartSequenceFieldOffset = PartitionIdFieldOffset + IntSizeInBytes;
        private const int RequestMinCountFieldOffset = RequestStartSequenceFieldOffset + LongSizeInBytes;
        private const int RequestMaxCountFieldOffset = RequestMinCountFieldOffset + IntSizeInBytes;
        private const int RequestInitialFrameSize = RequestMaxCountFieldOffset + IntSizeInBytes;
        private const int ResponseReadCountFieldOffset = ResponseBackupAcksFieldOffset + ByteSizeInBytes;
        private const int ResponseNextSeqFieldOffset = ResponseReadCountFieldOffset + IntSizeInBytes;
        private const int ResponseInitialFrameSize = ResponseNextSeqFieldOffset + LongSizeInBytes;

        public static ClientMessage EncodeRequest(string name, long startSequence, int minCount, int maxCount, IData filter)
        {
            var clientMessage = CreateForEncode();
            clientMessage.IsRetryable = true;
            clientMessage.OperationName = "Ringbuffer.ReadMany";
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], UnfragmentedMessage);
            EncodeInt(initialFrame.Content, TypeFieldOffset, RequestMessageType);
            EncodeInt(initialFrame.Content, PartitionIdFieldOffset, -1);
            EncodeLong(initialFrame.Content, RequestStartSequenceFieldOffset, startSequence);
            EncodeInt(initialFrame.Content, RequestMinCountFieldOffset, minCount);
            EncodeInt(initialFrame.Content, RequestMaxCountFieldOffset, maxCount);
            clientMessage.Add(initialFrame);
            StringCodec.Encode(clientMessage, name);
            CodecUtil.EncodeNullable(clientMessage, filter, DataCodec.Encode);
            return clientMessage;
        }

        public class ResponseParameters
        {

            /// <summary>
            /// Number of items that have been read before filtering.
            ///</summary>
            public int ReadCount;

            /// <summary>
            /// List of items that have beee read.
            ///</summary>
            public IList<IData> Items;

            /// <summary>
            /// List of sequence numbers for the items that have been read.
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