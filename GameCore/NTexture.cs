using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class NTexture
    {
        public NTexture(byte[] pmTextureBytes, int pmTextureWidth, int pmTextureHeight, int pmTextureGapX, int pmTextureGapY)
        {
            this.textureBytes = pmTextureBytes;
            this.textureWidth = pmTextureWidth;
            this.textureHeight = pmTextureHeight;
            this.textureGapX = pmTextureGapX-1;
            this.textureGapY = pmTextureGapY-1;
            this.textureD3D9 = null;
        }

        #region declaration
        public byte[] textureBytes;
        public int textureWidth;
        public int textureHeight;
        public int textureGapX;
        public int textureGapY;
        public Texture textureD3D9;
        #endregion
    }
}
