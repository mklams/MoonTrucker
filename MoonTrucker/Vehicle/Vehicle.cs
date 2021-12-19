using System;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Genbox.VelcroPhysics.Factories;
using Genbox.VelcroPhysics.Utilities;
using MoonTrucker.Core;

namespace MoonTrucker.Vehicle
{
    public abstract class Vehicle
    {
        private Texture2D _sprite { get; }
        private Texture2D _light { get; }

        protected Body _body { get; }
        protected SpriteBatch _batch;
        protected bool _isBraking = false;
        protected bool _inDrive = false;
        protected bool _isTurning = false;

        protected World _world;
        protected TextureManager _textureManager;

        public float Height { get; }
        public float Width { get; }
        public Vehicle(float width, float height, Vector2 position, World world, TextureManager manager, SpriteBatch batch)
        {
            Height = height;
            Width = width;

            _world = world;
            _textureManager = manager;
            _body = BodyFactory.CreateRectangle(world, height, width, 1f, position, 0, BodyType.Dynamic);

            //from https://box2d.org/documentation/md__d_1__git_hub_box2d_docs_dynamics.html
            _body.LinearDamping = 0f; //makes car appear "floaty" if > 0
            _body.AngularDamping = .01f;


            _body.Restitution = .5f; //how bouncy (not bouncy) 0 - 1(super bouncy) 
            _body.Friction = 1f;    //friction between other bodies (none) 0 - 1 (frictiony)
            _body.Inertia = 1f;
            //_body.Mass = 1f;

            _sprite = manager.TextureFromShape(_body.FixtureList[0].Shape, Color.Transparent, Color.Salmon);
            _light = new Texture2D(manager.graphicsDevice, 3, (int)ConvertUnits.ToDisplayUnits(width));
            Color[] colors = new Color[(3 * (int)ConvertUnits.ToDisplayUnits(width))];
            for (int i = 0; i < (3 * (int)ConvertUnits.ToDisplayUnits(width)); i++)
            {
                colors[i] = Color.White;
            }
            _light.SetData(colors);
            _batch = batch;
        }

        //I tried not having to re-create the car but this never worked. 
        // public void ResetToPosition(Vector2 startPos)
        // {
        //     disableCollision();
        //     _body.Position = startPos;
        //     _body.Rotation = 0;
        //     enableCollision();
        // }

        // private void disableCollision()
        // {
        //     _body.FixtureList.ForEach(fixture => fixture.Body.Enabled = false);
        //     _body.Enabled = false;
        // }

        // private void enableCollision()
        // {
        //     _body.FixtureList.ForEach(fixture => fixture.Body.Enabled = true);
        //     _body.Enabled = true;
        // }

        public void Destroy()
        {
            _body.FixtureList.ForEach(fix => fix.Body.RemoveFromWorld());
            _body.RemoveFromWorld();
        }

        public Vector2 GetPosition()
        {
            return _body.Position;
        }

        public virtual void Draw()
        {
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            _batch.Draw(_sprite, ConvertUnits.ToDisplayUnits(_body.Position), null, Color.White, _body.Rotation, origin, 1f, SpriteEffects.None, 0f);
            this.drawTailLights(origin);
        }

        private void drawTailLights(Vector2 carOrigin)
        {
            Color tailLightColor;
            if (VectorHelpers.IsStopped(_body))
            {
                tailLightColor = Color.DarkRed;
            }
            else //in motion
            {
                if (_isBraking)
                {
                    tailLightColor = Color.Red;
                }
                else if (_inDrive)
                {
                    tailLightColor = Color.DarkRed;
                }
                else
                {
                    tailLightColor = Color.White;
                }
            }
            _batch.Draw(_light, ConvertUnits.ToDisplayUnits(_body.Position), null, tailLightColor, _body.Rotation, carOrigin, 1f, SpriteEffects.None, 0f);

        }

        public void UpdateVehicle(KeyboardState keyboardState, GameTime gameTime)
        {
            _isBraking = false;
            _isTurning = false;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                this.handleUpKey(gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                this.handleDownKey(gameTime);
            }

            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                _isTurning = true;
                this.handleLeftKey(gameTime);
            }

            if (((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            && !(keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)))) //if holding both left and right, favor left.
            {
                _isTurning = true;
                this.handleRightKey(gameTime);
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                this.handleSpaceBar(gameTime);
            }
            this.snapVelocityToZero();
            this.applyFriction();
            this.restorativeTurn(gameTime);
        }
        protected abstract void restorativeTurn(GameTime gameTime);

        protected abstract void handleUpKey(GameTime gameTime);
        protected abstract void handleDownKey(GameTime gameTime);
        protected abstract void handleLeftKey(GameTime gameTime);
        protected abstract void handleRightKey(GameTime gameTime);
        protected abstract void snapVelocityToZero();
        protected abstract void applyFriction();

        protected abstract void handleSpaceBar(GameTime gameTime);


    }
}
