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
    /// Retrieves, but does not remove, the head of this queue, or returns null if this queue is empty.
    ///</summary>
    internal static class TransactionalQueuePeekCodec
    {
        //hex: 0x120400
        public const int RequestMessageType = 1180672;
        //hex: 0x120401
        public const int ResponseMessageType = 1180673;
        private const int RequestTxnIdFieldOffset = PartitionIdFieldOffset + IntSizeInBytes;
        private const int RequestThreadIdFieldOffset = RequestTxnIdFieldOffset + GuidSizeInBytes;
        private const int RequestTimeoutFieldOffset = RequestThreadIdFieldOffset + LongSizeInBytes;
        private const int RequestInitialFrameSize = RequestTimeoutFieldOffset + LongSizeInBytes;
        private const int ResponseInitialFrameSize = ResponseBackupAcksFieldOffset + ByteSizeInBytes;

        public static ClientMessage EncodeRequest(string name, Guid txnId, long threadId, long timeout)
        {
            var clientMessage = CreateForEncode();
            clientMessage.IsRetryable = false;
            clientMessage.OperationName = "TransactionalQueue.Peek";
            var initialFrame = new Frame(new byte[RequestInitialFrameSize], UnfragmentedMessage);
            EncodeInt(initialFrame.Content, TypeFieldOffset, RequestMessageType);
            EncodeInt(initialFrame.Content, PartitionIdFieldOffset, -1);
            EncodeGuid(initialFrame.Content, RequestTxnIdFieldOffset, txnId);
            EncodeLong(initialFrame.Content, RequestThreadIdFieldOffset, threadId);
            EncodeLong(initialFrame.Content, RequestTimeoutFieldOffset, timeout);
            clientMessage.Add(initialFrame);
            StringCodec.Encode(clientMessage, name);
            return clientMessage;
        }

        public class ResponseParameters
        {

            /// <summary>
            /// The value at the head of the queue.
            ///</summary>
            public IData Response;
        }

        public static ResponseParameters DecodeResponse(ClientMessage clientMessage)
        {
            var iterator = clientMessage.GetIterator();
            var response = new ResponseParameters();
            //empty initial frame
            iterator.Next();
            response.Response = CodecUtil.DecodeNullable(iterator, DataCodec.Decode);
            return response;
        }

    }
}