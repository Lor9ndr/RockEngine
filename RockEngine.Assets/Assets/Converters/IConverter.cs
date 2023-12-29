﻿namespace RockEngine.Assets.Assets.Converters
{
    public interface IConverter<T>
    {
        public void Write(T data, BinaryWriter writer);

        public T Read(BinaryReader reader);
    }
}
