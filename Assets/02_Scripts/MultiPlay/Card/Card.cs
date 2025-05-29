using Unity.Netcode;
using Unity.Collections;
using System;

public struct Card : INetworkSerializable, IEquatable<Card>
{
    public int Number;
    public FixedString128Bytes CardEffectKey;

    public Card(int number, FixedString128Bytes cardEffectKey)
    {
        Number = number;
        CardEffectKey = cardEffectKey;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Number);
        serializer.SerializeValue(ref CardEffectKey);
    }
    public bool Equals(Card other)
    {
        return Number == other.Number && CardEffectKey.Equals(other.CardEffectKey);
    }
}