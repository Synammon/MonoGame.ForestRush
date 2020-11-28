using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Psilibrary.SpriteClasses
{
    public class SpriteFrame : ICloneable
    {
        private string _textureName;
        private List<Rectangle> _frames;
        private int _frameCount;
        private int _frameRate;
        private int _frameWidth;
        private int _frameHeight;

        [ContentSerializer]
        public string TextureName
        {
            get { return _textureName; }
            set { _textureName = value; }
        }

        [ContentSerializer]
        public List<Rectangle> Frames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        [ContentSerializer]
        public int FrameCount
        {
            get { return _frameCount; }
            set { _frameCount = value; }
        }

        [ContentSerializer]
        public int FrameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        [ContentSerializer]
        public int FrameWidth
        {
            get { return _frameWidth; }
            set { _frameWidth = value; }
        }

        [ContentSerializer]
        public int FrameHeight
        {
            get { return _frameHeight; }
            set { _frameHeight = value; }
        }

        public SpriteFrame()
        {
        }

        public SpriteFrame(string textureName, int textureWidth, int textureHeight, int frameWidth, int frameHeight, int frameCount, int frameRate = 8)
        {
            this._textureName = textureName;
            this._frameWidth = frameWidth;
            this._frameHeight = frameHeight;
            this._frameCount = frameCount;
            this._frameRate = frameRate;

            _frames = new List<Rectangle>();

            int xOffset = 0;
            int yOffset = 0;

            for (int i = 0; i < frameCount; i++)
            {
                Rectangle r = new Rectangle(xOffset, yOffset, frameWidth, frameHeight);
                _frames.Add(r);
                xOffset += frameWidth;
                if (xOffset >= textureWidth)
                {
                    xOffset = 0;
                    yOffset += frameHeight;
                }
            }
        }

        public object Clone()
        {
            SpriteFrame frame = new SpriteFrame()
            {
                _frames = new List<Rectangle>(),
                _textureName = _textureName,
                _frameCount = _frameCount,
                _frameWidth = _frameWidth,
                _frameHeight = _frameHeight,
                _frameRate = _frameRate,
            };

            foreach (Rectangle r in _frames)
            {
                frame._frames.Add(r);
            }
            return frame;
        }
    }
}
