﻿using OpenTK.Graphics.OpenGL4;

namespace RockEngine.OpenGL.Settings
{
    internal record FrameBufferSettings : ISettings
    {
        public FramebufferTarget FramebufferTarget { get; set; }

        public FrameBufferSettings(FramebufferTarget framebufferTarget)
        {
            FramebufferTarget = framebufferTarget;
        }
    }
    internal sealed record FrameBufferRenderBufferSettings : FrameBufferSettings
    {
        public FramebufferAttachment RenderBufferAttachment { get; set; }

        public FrameBufferRenderBufferSettings(FramebufferAttachment renderBufferAttachment, FramebufferTarget framebufferTarget)
            : base(framebufferTarget)
        {
            RenderBufferAttachment = renderBufferAttachment;
        }
    }

    internal sealed record RenderBufferSettings : ISettings
    {
        public RenderbufferStorage RenderbufferStorage { get; set; }
        public RenderbufferTarget RenderbufferTarget { get; set; }
        public bool IsMultiSample { get; set; }
        public int SampleCount { get; set; }

        public RenderBufferSettings(RenderbufferStorage renderbufferStorage, RenderbufferTarget renderbufferTarget, bool isMultiSample = false, int sampleCount = 0)
        {
            RenderbufferStorage = renderbufferStorage;
            RenderbufferTarget = renderbufferTarget;
            IsMultiSample = isMultiSample;
            SampleCount = sampleCount;
        }
    }
}