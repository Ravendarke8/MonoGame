﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    public sealed partial class TextureCollection
    {
        void PlatformInit()
        {
        }

        internal void ClearTargets(RenderTargetBinding[] targets, SharpDX.Direct3D11.CommonShaderStage shaderStage)
        {
            // NOTE: We make the assumption here that the caller has
            // locked the d3dContext for us to use.

            // Make one pass across all the texture slots.
            for (var i = 0; i < _textures.Length; i++)
            {
                if (_textures[i] == null)
                    continue;

                for (int k = 0; k < targets.Length; k++)
                {
                    if (_textures[i] == targets[k].RenderTarget)
                    {
                        // Immediately clear the texture from the device.
                        _dirty &= ~(1 << i);
                        _textures[i] = null;
                        shaderStage.SetShaderResource(i, null);
                        break;
                    }
                }
            }
        }

        void PlatformClear()
        {
        }

        internal void PlatformSetTextures(GraphicsDevice device, SharpDX.Direct3D11.CommonShaderStage shaderStageDX)
        {
            // Skip out if nothing has changed.
            if (_dirty == 0)
                return;

            // NOTE: We make the assumption here that the caller has
            // locked the d3dContext for us to use.
            for (var i = 0; i < _textures.Length; i++)
            {
                var mask = 1 << i;
                if ((_dirty & mask) == 0)
                    continue;

                var tex = _textures[i];

                if (_textures[i] == null || _textures[i].IsDisposed)
                    shaderStageDX.SetShaderResource(i, null);
                else
                {
                    shaderStageDX.SetShaderResource(i, _textures[i].GetShaderResourceView());
                    unchecked
                    {
                        _graphicsDevice._graphicsMetrics._textureCount++;
                    }
                }
                _dirty &= ~mask;
                if (_dirty == 0)
                    break;
            }

            _dirty = 0;
        }
    }
}
