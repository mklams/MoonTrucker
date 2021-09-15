using System;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonTrucker
{
    public class GeneratedCity
    {
        private float _worldWidth;
        private float _worldHeight;
        private VehicleWithPhysics _mainVehicle;
        private StaticBodyFactory _bodyFactory;

        public GeneratedCity(StaticBodyFactory bodyFactory, float screenWidth, float screenHeight, VehicleWithPhysics mainVehicle)
        {
            _bodyFactory = bodyFactory;
            _worldWidth = ConvertUnits.ToSimUnits(screenWidth);
            _worldHeight = ConvertUnits.ToSimUnits(screenHeight);
            _mainVehicle = mainVehicle;
        }

        public List<IDrawable> GenerateSquareCity()
        {
            var city = createBoundryWalls();
            city.AddRange(createSquareCityBlock(new Vector2(1f,1f))); // offset by 1m due to wall

            return city;
        }

        private List<IDrawable> createBoundryWalls()
        {
            const float wallWidth = 1f;
            return new List<IDrawable>
            {
                _bodyFactory.CreateRectangleBody(wallWidth, _worldHeight, new Vector2(wallWidth / 2, _worldHeight / 2f)),
                _bodyFactory.CreateRectangleBody(wallWidth, _worldHeight, new Vector2(_worldWidth - wallWidth / 2, _worldHeight / 2f)),
                _bodyFactory.CreateRectangleBody(_worldWidth, wallWidth, new Vector2(_worldWidth / 2, wallWidth / 2)),
                _bodyFactory.CreateRectangleBody(_worldWidth, wallWidth, new Vector2(_worldWidth / 2, _worldHeight - wallWidth /2)),
            };
        }

        /// <summary>
        /// Creates a city block with four square buildings
        /// </summary>
        /// <param name="blockCorner"></param>
        /// <returns></returns>
        private List<IDrawable> createSquareCityBlock(Vector2 blockCorner)
        {
            var buildingLength = _mainVehicle.Height * 3f;
            var roadLaneWidth = _mainVehicle.Height * 1.5f;
            var firstBuilding = new SquareBuilding(buildingLength, roadLaneWidth, blockCorner, _bodyFactory);
            var secondBuilding = new SquareBuilding(buildingLength, roadLaneWidth, firstBuilding.TopRightCorner, _bodyFactory);
            var thirdBuilding = new SquareBuilding(buildingLength, roadLaneWidth, firstBuilding.BottomLeftCorner, _bodyFactory);
            var fourthBuildg = new SquareBuilding(buildingLength, roadLaneWidth, firstBuilding.BottomRightCorner, _bodyFactory);

            return new List<IDrawable>
            {
                firstBuilding,
                secondBuilding,
                thirdBuilding,
                fourthBuildg
            };
        }

    }

    public interface IDrawable
    {
        public void Draw();
    }

    //TODO: Move this to it's own class
    public class SquareBuilding : IDrawable
    {
        private float _length;
        private float _streetWidth;
        private Vector2 _leftCorner;
        private RectangleBody _buildingBody;
        public Vector2 TopRightCorner { get; }
        public Vector2 BottomRightCorner { get; }
        public Vector2 BottomLeftCorner { get; }

        public SquareBuilding(float length, float streeWidth, Vector2 leftCorner, StaticBodyFactory bodyFactory)
        {
            float offsetToOrigin = streeWidth + length / 2; // add street width since streets surround the entire building 

            Vector2 buildingOrigin = Vector2.Add(leftCorner, new Vector2(offsetToOrigin, offsetToOrigin));

            _buildingBody = bodyFactory.CreateRectangleBody(length, length, buildingOrigin);

            float offSetToCorner = streeWidth * 2 + length; // add 2 street width since there is a street on each side of building
            TopRightCorner = Vector2.Add(leftCorner, new Vector2(offSetToCorner, 0));
            BottomRightCorner = Vector2.Add(leftCorner, new Vector2(offSetToCorner, offSetToCorner));
            BottomLeftCorner = Vector2.Add(leftCorner, new Vector2(0, offSetToCorner));
        }

        public void Draw()
        {
            _buildingBody.Draw();
        }
    }
}
