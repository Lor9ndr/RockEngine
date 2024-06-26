﻿using System.Diagnostics.CodeAnalysis;

namespace RockEngine.Rendering.OpenGL.Buffers.UBOBuffers
{
    public interface IUBOData<T> where T : struct
    {
        internal static UBO<T> UBO { get; set; }
        public int BindingPoint { get; }
        public string Name { get; }
        public void SendData(IRenderingContext context);
        public void SendData<TSub>(IRenderingContext context,[DisallowNull, NotNull] TSub data, nint offset, int size);
    }
}
