using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Psilibrary.TileEngine;
using System;

namespace Psilibrary.SpriteClasses
{
    public enum Facing { Left, Right }

    public class AnimatedAtlasSprite : ICloneable
    {
        private SpriteAtlas _atlas;
        private string _currentAnimation;
        private int _currentFrame;
        public Vector2 Position;
        private TimeSpan _frameTimer;
        private TimeSpan _frameLength;

        public SpriteAtlas SpriteAtlas
        {
            get { return _atlas; }
        }

        public string CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        public Vector2 Center
        {
            get { return Origin + Position; }
        }

        public Vector2 Origin
        {
            get 
            { 
                return new Vector2(
                    _atlas.Animations[_currentAnimation].FrameWidth / 2, 
                    _atlas.Animations[_currentAnimation].FrameHeight / 2); 
            }
        }

        public int Width
        {
            get { return _atlas.Animations[_currentAnimation].FrameWidth; }
        }

        public int Height
        {
            get { return _atlas.Animations[_currentAnimation].FrameHeight; }
        }

        public Facing Facing { get; set; }

        private AnimatedAtlasSprite()
        {
            Facing = Facing.Right;
        }

        public AnimatedAtlasSprite(SpriteAtlas atlas)
        {
            this._atlas = atlas;
        }

        public Rectangle Bounds
        {
            get 
            { 
                return new Rectangle(
                    (int)Position.X, 
                    (int)Position.Y, 
                    _atlas.Animations[_currentAnimation].FrameWidth, 
                    _atlas.Animations[_currentAnimation].FrameHeight); 
            }
        }

        public string Name { get; set; }
        public bool IsAnimating { get; set; }

        public void ChangeAnimation(string animation)
        {
            if (_atlas.Animations.ContainsKey(animation))
            {
                _currentAnimation = animation;
                _currentFrame = 0;
                _frameTimer = TimeSpan.Zero;
                _frameLength = TimeSpan.FromSeconds(1 / (double)_atlas.Animations[animation].FrameRate);
            }
        }

        public void Update(GameTime gameTime)
        {
            _frameTimer += gameTime.ElapsedGameTime;

            if (_frameTimer >= _frameLength)
            {
                _frameTimer = TimeSpan.Zero;

                if (string.IsNullOrEmpty(_currentAnimation))
                {
                    _currentAnimation = "Run";
                }

                if (IsAnimating)
                {
                    _currentFrame = (_currentFrame + 1) % _atlas.Animations[_currentAnimation].Frames.Count;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Facing == Facing.Right)
            {
                spriteBatch.Draw(
                    _atlas.Texture2D(_atlas.Animations[_currentAnimation].TextureName),
                    new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        _atlas.Animations[_currentAnimation].FrameWidth,
                        _atlas.Animations[_currentAnimation].FrameHeight),
                    _atlas.Animations[_currentAnimation].Frames[_currentFrame],
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1f);
            }
            else
            {
                spriteBatch.Draw(
                    _atlas.Texture2D(_atlas.Animations[_currentAnimation].TextureName),
                    new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        _atlas.Animations[_currentAnimation].FrameWidth,
                        _atlas.Animations[_currentAnimation].FrameHeight),
                    _atlas.Animations[_currentAnimation].Frames[_currentFrame],
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.FlipHorizontally,
                    1f);
            }
        }

        public void LockToMap(Point mapSize)
        {
            Position.X = MathHelper.Clamp(Position.X, 0, mapSize.X - Width);
            Position.Y = MathHelper.Clamp(Position.Y, 0, mapSize.Y - Height);
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle destinationRect)
        {
            spriteBatch.Draw(
                _atlas.Texture2D(_currentAnimation),
                destinationRect,
                _atlas.Animations[_currentAnimation].Frames[_currentFrame],
                Color.White);
        }

        public object  Clone()
        {
            AnimatedAtlasSprite sprite = new AnimatedAtlasSprite
            {
                _atlas = (SpriteAtlas)_atlas.Clone(),
                _currentFrame = _currentFrame,
                _frameLength = _frameLength,
                _frameTimer = _frameTimer,
                _currentAnimation = _currentAnimation,
                Position = Position
            };

            return sprite;
        }

        public bool Contains(Point p)
        {
            return Bounds.Contains(p);
        }

        public bool Contains(Vector2 p)
        {
            return Bounds.Contains(p);
        }
    }
}
