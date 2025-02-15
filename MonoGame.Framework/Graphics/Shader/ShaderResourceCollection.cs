// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    internal struct ResourceBinding
    {
        public ShaderResource resource;
#if OPENGL
            public int bindingSlot;
            public int bindingSlotForCounter; // in OpenGL structured buffers with append/consume/counter functionality are emulated using a separate counter buffer
#endif
    }

    public sealed partial class ShaderResourceCollection
    {
        private readonly ResourceBinding[] _readonlyResources;
        private readonly ResourceBinding[] _writeableResources;

        private int _readonlyValid;
        private int _writeableValid;

        private ShaderStage _stage;

        public int MaxReadableResources { get { return _readonlyResources.Length; } }
        public int MaxWriteableResources { get { return _writeableResources.Length; } }

        internal ShaderResourceCollection(ShaderStage stage, int maxReadableResources, int maxWriteableResources)
		{
            _stage = stage;

            _readonlyResources = new ResourceBinding[maxReadableResources];
            _writeableResources = new ResourceBinding[maxWriteableResources];
        }

        internal void SetResourceForBindingSlot(ShaderResource resource, ref ShaderResourceInfo resourceInfo)
        {
            bool writeAccess = resourceInfo.writeAccess;
            int bindingSlot = resourceInfo.bindingSlot;

            if (writeAccess && _stage != ShaderStage.Compute)
                throw new ArgumentException("Only a compute shader can use RWStructuredBuffer currently. Uae a regular StructuredBuffer instead and assign it the same buffer.");

            var resources = writeAccess ? _writeableResources : _readonlyResources;

#if OPENGL
            // DX uses u-registers in shaders for writeable buffers and textures, and t-registers for readonly buffers and textures.
            // OpenGL doesn't separate register types like this. If a shader resource is assigned to register u0, and another resource is assigned to register t0,
            // things are fine in DX, but in GL we have a binding slot conflict. To resolve this u-registers have been shifted by 16, if set explicitly (see ShaderConductor shiftAllUABuffersBindings option in MGFXC).
            // Unshift those binding slots now, to avoid an array index overflow.
            resources[bindingSlot % GraphicsDevice.MaxResourceSlotsPerShaderStage] = new ResourceBinding
            {
                resource = resource,
                bindingSlot = bindingSlot,
                bindingSlotForCounter = resourceInfo.bindingSlotForCounter,
            };
#else
            resources[bindingSlot] = new ResourceBinding
            {
                resource = resource,
            };
#endif

            if (writeAccess)
                _writeableValid |= 1 << bindingSlot;
            else
                _readonlyValid |= 1 << bindingSlot;
        }

        internal void Clear()
        {
            for (var i = 0; i < _readonlyResources.Length; i++)
                _readonlyResources[i] = new ResourceBinding();

            for (var i = 0; i < _writeableResources.Length; i++)
                _writeableResources[i] = new ResourceBinding();

            _readonlyValid = 0;
            _writeableValid = 0;
        }

#if WEB
        internal void ApplyAllResourcesToDevice(GraphicsDevice device, int shaderProgram)
#elif OPENGL
        internal void ApplyAllResourcesToDevice(GraphicsDevice device, ShaderProgram shaderProgram)
#else
        internal void ApplyAllResourcesToDevice(GraphicsDevice device)
#endif
        {
            if (_readonlyValid != 0) // If there are no readable resources then skip it.
            {
                int valid = _readonlyValid;

                for (var i = 0; i < _readonlyResources.Length; i++)
                {
                    var resourceInfo = _readonlyResources[i];
                    var resource = resourceInfo.resource;
                    if (resource != null && !resource.IsDisposed)
                    {
#if OPENGL || WEB
                        resource.PlatformApply(device, shaderProgram, ref resourceInfo, false);
#else
                        resource.PlatformApply(device, _stage, i, false);
#endif
                    }

                    // Early out if this is the last one.
                    valid &= ~(1 << i);
                    if (valid == 0)
                        break;
                }
            }

            if (_writeableValid != 0) // If there are no writeable resources then skip it.
            {
                int valid = _writeableValid;

                for (var i = 0; i < _writeableResources.Length; i++)
                {
                    var resourceInfo = _writeableResources[i];
                    var resource = resourceInfo.resource;
                    if (resource != null && !resource.IsDisposed)
                    {
#if OPENGL || WEB
                        resource.PlatformApply(device, shaderProgram, ref resourceInfo, true);
#else
                        resource.PlatformApply(device, _stage, i, true);
#endif
                    }

                    // Early out if this is the last one.
                    valid &= ~(1 << i);
                    if (valid == 0)
                        break;
                }
            }
        }
    }
}
