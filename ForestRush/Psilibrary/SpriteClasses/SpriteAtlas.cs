using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psilibrary.SpriteClasses
{
    public class SpriteAtlas : ICloneable
    {
        private Dictionary<string, SpriteFrame> _animations = new Dictionary<string, SpriteFrame>();
        private readonly Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        [ContentSerializer]
        public Dictionary<string, SpriteFrame> Animations
        {
            get { return _animations; }
            private set { _animations = value; }
        }

        public SpriteAtlas()
        {

        }

        public Texture2D Texture2D(string name)
        {
            if (_textures.ContainsKey(name))
            {
                return _textures[name];
            }

            return null;
        }

        public void LoadTextures(ContentManager content, string folder)
        {
            foreach (var v in _animations.Values)
            {
                if (!_textures.ContainsKey(v.TextureName))
                {
                    Texture2D texture = content.Load<Texture2D>(folder + "\\" + v.TextureName);
                    _textures.Add(v.TextureName, texture);
                }
            }
        }

        public object Clone()
        {
            SpriteAtlas atlas = new SpriteAtlas();

            foreach (string s in Animations.Keys)
            {
                atlas.Animations.Add(s, (SpriteFrame)Animations[s].Clone());
                _textures.Add(s, _textures[s]);
            }

            return atlas;
        }
    }
}
