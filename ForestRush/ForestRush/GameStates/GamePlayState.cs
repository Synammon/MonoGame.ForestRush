using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Psilibrary.SpriteClasses;
using Psilibrary.TileEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestRush.GameStates
{
    public enum SpriteState { Idle, Walking, Jumping }
    public class GamePlayState : BaseGameState
    {
        private AnimatedAtlasSprite _sprite;

        private Camera camera;
        private Engine engine;
        private World world;

        private const float Gravity = 10000;
        private float _impulse = 0;
        private TimeSpan _timer;
        private SpriteState _spriteState;
        private float _speed = 480f;
        private KeyboardState xin, lin;
            
        public GamePlayState(Game game) : base(game)
        {
        }

        protected override void LoadContent()
        {
            SpriteFrame frame = new SpriteFrame("spr_kanako_run_32x64_4-frames", 128, 64, 32, 64, 4);
            SpriteAtlas atlas = new SpriteAtlas();

            atlas.Animations.Add("Run", frame);

            frame = new SpriteFrame("spr_kanako_jump_32x64_6-frames", 192, 64, 32, 64, 6);
            
            atlas.Animations.Add("Jump", frame);
            atlas.LoadTextures(Game.Content, "Player");

            _sprite = new AnimatedAtlasSprite(atlas);

            world = new World();

            Tileset tileset = new Tileset(
                Game.Content.Load<Texture2D>(@"Tileset"),
                8,
                6,
                16,
                16);

            MapLayer baseLayer = new MapLayer(new Tile[30, 100]);

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    baseLayer.SetTile(i, j, new Tile(-1, -1));
                }
            }

            for (int i = 0; i < 100; i++)
            {
                baseLayer.SetTile(i, 29, new Tile(1, 0));
            }

            TileMap map = new TileMap("test", tileset, baseLayer, new CollisionLayer(), new PortalLayer());
            map.PortalLayer.Portals.Add("test", new Portal(new Point(0, 0), new Point(0, 28), "test"));

            for (int i = 0; i < 100; i++)
            {
                map.CollisionLayer.CollisionAreas.Add(
                    new Point(i, 29), 
                    CollisionValue.Impassible);
            }

            world.AddMap("test", map);
            world.ChangeMap("test", "test");

            camera = new Camera(new Rectangle(0, 0, 1280, 720));

            engine = new Engine(32, 32);

            _sprite.ChangeAnimation("Run");
            _sprite.Position = new Vector2(0, Engine.TileHeight * 27);
            _sprite.Facing = Facing.Right;

            camera.LockToSprite(map, _sprite);

            base.LoadContent(); 
        }

        public override void Update(GameTime gameTime)
        {
            xin = Keyboard.GetState();
            Vector2 motion = Vector2.Zero;

            motion.Y = 1;

            _sprite.Update(gameTime);

            if (xin.IsKeyDown(Keys.A))
            {
                _sprite.IsAnimating = true;
                _sprite.Facing = Facing.Left;
                if (_spriteState != SpriteState.Jumping)
                {
                    _spriteState = SpriteState.Walking;
                }
                motion.X = -1;

                if (_sprite.CurrentAnimation != "Run" && _spriteState != SpriteState.Jumping)
                    _sprite.ChangeAnimation("Run");
            }
            else if (xin.IsKeyDown(Keys.D))
            {
                _sprite.IsAnimating = true;
                _sprite.Facing = Facing.Right;
                if (_spriteState != SpriteState.Jumping)
                {
                    _spriteState = SpriteState.Walking;
                }
                motion.X = 1;

                if (_sprite.CurrentAnimation != "Run" && _spriteState != SpriteState.Jumping)
                    _sprite.ChangeAnimation("Run");
            }
            else if (xin.IsKeyUp(Keys.A) && xin.IsKeyUp(Keys.D) && _spriteState != SpriteState.Jumping)
            {
                _spriteState = SpriteState.Idle;
                _sprite.IsAnimating = false;
                motion.X = 0;
            }

            if (xin.IsKeyDown(Keys.W) && lin.IsKeyUp(Keys.W) && _spriteState != SpriteState.Jumping)
            {
                motion.Y = -1;
                _timer = TimeSpan.Zero;
                _impulse = 2000f;
                _spriteState = SpriteState.Jumping;
                _sprite.ChangeAnimation("Jump");
                _sprite.IsAnimating = true;
            }

            if (_spriteState == SpriteState.Jumping)
            {
                motion.Y = -1;
                _timer += gameTime.ElapsedGameTime;
            }

            if ((_timer.TotalSeconds >= 1 || _impulse < -6000) && _spriteState == SpriteState.Jumping)
            {
                _impulse = 0;
                motion.Y = 0;
                _spriteState = SpriteState.Idle;
                _sprite.ChangeAnimation("Run");
                _sprite.IsAnimating = false;
            }

            if (motion != Vector2.Zero)
            {
                //motion.Normalize();
                motion.X *=
                    (_speed *
                    (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (_spriteState == SpriteState.Jumping)
                {
                    motion.Y *= _impulse * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _impulse -= Gravity * (float)(_timer.TotalSeconds) / 2;
                }
                else
                {
                    motion.Y *= Gravity * (float)(gameTime.ElapsedGameTime.TotalSeconds);
                }
            }

            _sprite.Position += motion;
            _sprite.LockToMap(
                    new Point(
                        world.CurrentMap.WidthInPixels,
                        world.CurrentMap.HeightInPixels));

            camera.LockToSprite(world.CurrentMap, _sprite);

            base.Update(gameTime);

            lin = xin;
        }

        public override void Draw(GameTime gameTime)
        {

            GameRef.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                null,
                null,
                null,
                camera.Transformation);

            world.CurrentMap.Draw(gameTime, GameRef.SpriteBatch, camera);

            _sprite.Draw(gameTime, GameRef.SpriteBatch);

            GameRef.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
