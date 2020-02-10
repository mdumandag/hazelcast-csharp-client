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
using Hazelcast.Client.Protocol.Util;
using Hazelcast.IO;
using Hazelcast.IO.Serialization;
using static Hazelcast.Client.Protocol.Codec.BuiltIn.FixedSizeTypesCodec;
using static Hazelcast.Client.Protocol.ClientMessage;
using static Hazelcast.IO.Bits;

namespace Hazelcast.Client.Protocol.Codec.Custom
{
    // This file is auto-generated by the Hazelcast Client Protocol Code Generator.
    // To change this file, edit the templates or the protocol
    // definitions on the https://github.com/hazelcast/hazelcast-client-protocol
    // and regenerate it.

    internal static class BitmapIndexOptionsCodec
    {
        private const int UniqueKeyTransformationFieldOffset = 0;
        private const int InitialFrameSize = UniqueKeyTransformationFieldOffset + IntSizeInBytes;

        public static void Encode(ClientMessage clientMessage, Hazelcast.Config.BitmapIndexOptions bitmapIndexOptions)
        {
            clientMessage.Add(BeginFrame.Copy());

            var initialFrame = new Frame(new byte[InitialFrameSize]);
            EncodeInt(initialFrame.Content, UniqueKeyTransformationFieldOffset, bitmapIndexOptions.UniqueKeyTransformation);
            clientMessage.Add(initialFrame);

            StringCodec.Encode(clientMessage, bitmapIndexOptions.UniqueKey);

            clientMessage.Add(EndFrame.Copy());
        }

        public static Hazelcast.Config.BitmapIndexOptions Decode(FrameIterator iterator)
        {
            // begin frame
            iterator.Next();

            var initialFrame = iterator.Next();
            var uniqueKeyTransformation = DecodeInt(initialFrame.Content, UniqueKeyTransformationFieldOffset);

            var uniqueKey = StringCodec.Decode(iterator);

            CodecUtil.FastForwardToEndFrame(iterator);

            return CustomTypeFactory.CreateBitmapIndexOptions(uniqueKey, uniqueKeyTransformation);
        }
    }
}