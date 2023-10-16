namespace RockEngine.OpenGL
{
    [Flags]
    public enum RenderType
    {
        /// <summary>
        /// NEVER SHOULD BE USED
        /// </summary>
        None = 0,

        /// <summary>
        /// Default render without using Indices
        /// </summary>
        Forward = 1,

        /// <summary>
        /// Render with using indices and EBO
        /// </summary>
        Indices = 2,

        /// <summary>
        /// Render with using Instancing
        /// </summary>
        Instanced = 4,

        /// <summary>
        /// Render using Indirect
        /// </summary>
        Indirect = 8,

        /// <summary>
        /// Render with using instancing and indices
        /// </summary>
        IndicesAndInstanced = Indices | Instanced
    }
}