//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

// This file isn't generated, but this comment is necessary to exclude it from StyleCop analysis.
// <auto-generated/>

using System;

namespace Microsoft.Data.Encryption.Cryptography.Serializers
{
    /// <summary>
    /// Contains the methods for serializing and deserializing <see cref="uint"/>? type data objects.
    /// </summary>
    [CLSCompliant(false)]
    internal class NullableUInt32Serializer : Serializer<uint?>
    {
        private static readonly UInt32Serializer serializer = new UInt32Serializer();

        /// <summary>
        /// The <see cref="Identifier"/> uniquely identifies a particular Serializer implementation.
        /// </summary>
        public override string Identifier => "UInt32_Nullable";

        /// <summary>
        /// Deserializes the provided <paramref name="bytes"/>
        /// </summary>
        /// <param name="bytes">The data to be deserialized</param>
        /// <returns>The serialized data</returns>
        /// <exception cref="MicrosoftDataEncryptionException">
        /// The length of <paramref name="bytes"/> is less than 4.
        /// </exception>
        public override uint? Deserialize(byte[] bytes)
        {
            return bytes.IsNull() ? (uint?)null : serializer.Deserialize(bytes);
        }

        /// <summary>
        /// Serializes the provided <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to be serialized</param>
        /// <returns>
        /// An array of bytes with length 4.
        /// </returns>
        public override byte[] Serialize(uint? value)
        {
            return value.IsNull() ? null : serializer.Serialize(value.Value);
        }
    }
}
