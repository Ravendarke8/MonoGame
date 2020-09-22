// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using MonoGame.OpenGL;
using ExtTextureFilterAnisotropic = MonoGame.OpenGL.TextureParameterName;

namespace Microsoft.Xna.Framework.Graphics
{
    public partial class SamplerState
    {
        internal int _glSampler = -1;

        private readonly float[] _openGLBorderColor = new float[4];

        internal const TextureParameterName TextureParameterNameTextureMaxAnisotropy = (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt;
        internal const TextureParameterName TextureParameterNameTextureMaxLevel = TextureParameterName.TextureMaxLevel;

        internal int GetGLSampler(GraphicsDevice device, bool useMipmaps = false)
        {
            if (GraphicsDevice == null)
            {
                // We're now bound to a device... no one should
                // be changing the state of this object now!
                GraphicsDevice = device;
            }
            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

            if (_glSampler == -1)
            {
                if (GL.GenSamplers == null)
                    return -1;

                GL.GenSamplers(1, out _glSampler);
                GraphicsExtensions.CheckGLError();
            }

            switch (Filter)
            {
                case TextureFilter.Point:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    break;
                case TextureFilter.Linear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureFilter.Anisotropic:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, MathHelper.Clamp(this.MaxAnisotropy, 1.0f, GraphicsDevice.GraphicsCapabilities.MaxTextureAnisotropy));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureFilter.PointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    break;
                case TextureFilter.LinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureFilter.MinLinearMagPointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    break;
                case TextureFilter.MinLinearMagPointMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    break;
                case TextureFilter.MinPointMagLinearMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                case TextureFilter.MinPointMagLinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                        SamplerParameter(_glSampler, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                    SamplerParameter(_glSampler, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    SamplerParameter(_glSampler, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Set up texture addressing.
            SamplerParameter(_glSampler, TextureParameterName.TextureWrapS, (int)GetWrapMode(AddressU));
            SamplerParameter(_glSampler, TextureParameterName.TextureWrapT, (int)GetWrapMode(AddressV));

            _openGLBorderColor[0] = BorderColor.R / 255.0f;
            _openGLBorderColor[1] = BorderColor.G / 255.0f;
            _openGLBorderColor[2] = BorderColor.B / 255.0f;
            _openGLBorderColor[3] = BorderColor.A / 255.0f;
            SamplerParameter(_glSampler, TextureParameterName.TextureBorderColor, _openGLBorderColor);
            SamplerParameter(_glSampler, TextureParameterName.TextureLodBias, MipMapLevelOfDetailBias);

            switch (FilterMode)
            {
                case TextureFilterMode.Comparison:
                    SamplerParameter(_glSampler, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
                    SamplerParameter(_glSampler, TextureParameterName.TextureCompareFunc, (int)ComparisonFunction.GetDepthFunction());
                    break;
                case TextureFilterMode.Default:
                    SamplerParameter(_glSampler, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
                    break;
                default:
                    throw new InvalidOperationException("Invalid filter mode!");
            }

            if (this.MaxMipLevel > 0)
                SamplerParameter(_glSampler, TextureParameterName.TextureMaxLOD, this.MaxMipLevel);
            else
                SamplerParameter(_glSampler, TextureParameterName.TextureMaxLOD, 1000);

            return _glSampler;
        }

        private void SamplerParameter(int sampler, TextureParameterName name, int value)
        {
            GL.SamplerParameteri(sampler, name, value);
            GraphicsExtensions.CheckGLError();
        }

        private void SamplerParameter(int sampler, TextureParameterName name, float value)
        {
            GL.SamplerParameterf(sampler, name, value);
            GraphicsExtensions.CheckGLError();
        }

        private unsafe void SamplerParameter(int sampler, TextureParameterName name, float[] values)
        {
            fixed (float* ptr = &values[0])
            {
                GL.SamplerParameterfv(sampler, name, ptr);
                GraphicsExtensions.CheckGLError();
            }
        }

        internal void Activate(GraphicsDevice device, TextureTarget target, bool useMipmaps = false)
        {
            if (GraphicsDevice == null)
            {
                // We're now bound to a device... no one should
                // be changing the state of this object now!
                GraphicsDevice = device;
            }
            Debug.Assert(GraphicsDevice == device, "The state was created for a different device!");

            switch (Filter)
            {
                case TextureFilter.Point:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Linear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.Anisotropic:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, MathHelper.Clamp(this.MaxAnisotropy, 1.0f, GraphicsDevice.GraphicsCapabilities.MaxTextureAnisotropy));
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.PointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.LinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinLinearMagPointMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.Linear));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipLinear:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapLinear : TextureMinFilter.Nearest));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilter.MinPointMagLinearMipPoint:
                    if (GraphicsDevice.GraphicsCapabilities.SupportsTextureFilterAnisotropic)
                    {
                        GL.TexParameter(target, TextureParameterNameTextureMaxAnisotropy, 1.0f);
                        GraphicsExtensions.CheckGLError();
                    }
                    GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)(useMipmaps ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.Nearest));
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GraphicsExtensions.CheckGLError();
                    break;
                default:
                    throw new NotSupportedException();
            }

            // Set up texture addressing.
            GL.TexParameter(target, TextureParameterName.TextureWrapS, (int)GetWrapMode(AddressU));
            GraphicsExtensions.CheckGLError();
            GL.TexParameter(target, TextureParameterName.TextureWrapT, (int)GetWrapMode(AddressV));
            GraphicsExtensions.CheckGLError();
#if !GLES
            // Border color is not supported by glTexParameter in OpenGL ES 2.0
            _openGLBorderColor[0] = BorderColor.R / 255.0f;
            _openGLBorderColor[1] = BorderColor.G / 255.0f;
            _openGLBorderColor[2] = BorderColor.B / 255.0f;
            _openGLBorderColor[3] = BorderColor.A / 255.0f;
            GL.TexParameter(target, TextureParameterName.TextureBorderColor, _openGLBorderColor);
            GraphicsExtensions.CheckGLError();
            // LOD bias is not supported by glTexParameter in OpenGL ES 2.0
            GL.TexParameter(target, TextureParameterName.TextureLodBias, MipMapLevelOfDetailBias);
            GraphicsExtensions.CheckGLError();
            // Comparison samplers are not supported in OpenGL ES 2.0 (without an extension, anyway)
            switch (FilterMode)
            {
                case TextureFilterMode.Comparison:
                    GL.TexParameter(target, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
                    GraphicsExtensions.CheckGLError();
                    GL.TexParameter(target, TextureParameterName.TextureCompareFunc, (int)ComparisonFunction.GetDepthFunction());
                    GraphicsExtensions.CheckGLError();
                    break;
                case TextureFilterMode.Default:
                    GL.TexParameter(target, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.None);
                    GraphicsExtensions.CheckGLError();
                    break;
                default:
                    throw new InvalidOperationException("Invalid filter mode!");
            }
#endif
            if (GraphicsDevice.GraphicsCapabilities.SupportsTextureMaxLevel)
            {
                if (this.MaxMipLevel > 0)
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterNameTextureMaxLevel, this.MaxMipLevel);
                }
                else
                {
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterNameTextureMaxLevel, 1000);
                }
                GraphicsExtensions.CheckGLError();
            }
        }

        private int GetWrapMode(TextureAddressMode textureAddressMode)
        {
            switch (textureAddressMode)
            {
                case TextureAddressMode.Clamp:
                    return (int)TextureWrapMode.ClampToEdge;
                case TextureAddressMode.Wrap:
                    return (int)TextureWrapMode.Repeat;
                case TextureAddressMode.Mirror:
                    return (int)TextureWrapMode.MirroredRepeat;
#if !GLES
                case TextureAddressMode.Border:
                    return (int)TextureWrapMode.ClampToBorder;
#endif
                default:
                    throw new ArgumentException("No support for " + textureAddressMode);
            }
        }

        partial void PlatformDispose()
        {
            if (_glSampler > 0)
            {
                unsafe
                {
                    fixed (int* ptr = &_glSampler)
                        GL.DeleteSamplers(1, ptr);
                }
                _glSampler = -1;
            }
        }
    }
}

