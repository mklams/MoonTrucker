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
        private SpriteBatch _spriteBatch;
        private float _worldWidth;
        private float _worldHeight;
        private World _world;
        private VehicleWithPhysics _mainVehicle;
        private TextureManager _manager;

        public GeneratedCity(SpriteBatch spriteBatch, float screenWidth, float screenHeight, World world, VehicleWithPhysics mainVehicle, TextureManager manager)
        {
            _spriteBatch = spriteBatch;
            _worldWidth = ConvertUnits.ToSimUnits(screenWidth);
            _worldHeight = ConvertUnits.ToSimUnits(screenHeight);
            _world = world;
            _mainVehicle = mainVehicle;
            _manager = manager;
        }

        public List<RectangleBody> GenerateSquareCity()
        {
            var city = createBoundryWalls();
            city.Add(createSquare(_mainVehicle.Width, new Vector2(5f, 5f)));
            //city.AddRange(createSquareCityBlock(new Vector2(0,0)));

            return city;
        }

        private List<RectangleBody> createBoundryWalls()
        {
            const float wallWidth = 1f;
            return new List<RectangleBody>
            {
                new RectangleBody(wallWidth, _worldHeight, new Vector2(wallWidth / 2, _worldHeight / 2f), _world, _manager, _spriteBatch),
                new RectangleBody(wallWidth, _worldHeight, new Vector2(_worldWidth - wallWidth / 2, _worldHeight / 2f), _world, _manager, _spriteBatch),
                new RectangleBody(_worldWidth, wallWidth, new Vector2(_worldWidth / 2, wallWidth / 2), _world, _manager, _spriteBatch),
                new RectangleBody(_worldWidth, wallWidth, new Vector2(_worldWidth / 2, _worldHeight - wallWidth /2), _world, _manager, _spriteBatch),
            };
        }

        private RectangleBody createSquare(float sideLength, Vector2 position)
        {
            return new RectangleBody(sideLength, sideLength, position, _world, _manager, _spriteBatch);
        }

        /// <summary>
        /// Creates a city block with four square buildings
        /// </summary>
        /// <param name="blockCorner"></param>
        /// <returns></returns>
        private List<RectangleBody> createSquareCityBlock(Vector2 blockCorner)
        {
            float vehicleHeight = _mainVehicle.Height;
            var buildingLength = _mainVehicle.Width * 3f;
            var roadLaneWidth = vehicleHeight;
            var firstBuildingCorner = createVectorRelativeToOrgin(blockCorner, vehicleHeight, vehicleHeight);
            var secondBuildingCorner = createVectorRelativeToOrgin(blockCorner, vehicleHeight, 6 * vehicleHeight);
            var thirdBuildingCorner = createVectorRelativeToOrgin(blockCorner, 6 * vehicleHeight, vehicleHeight);
            var fourthBuildgCorner = createVectorRelativeToOrgin(blockCorner, 6 * vehicleHeight, 6 * vehicleHeight);

            return new List<RectangleBody>
            {
                createRectangleBuildingAtPoint(firstBuildingCorner, buildingLength, buildingLength),
                createRectangleBuildingAtPoint(secondBuildingCorner, buildingLength, buildingLength),
                createRectangleBuildingAtPoint(thirdBuildingCorner, buildingLength, buildingLength),
                createRectangleBuildingAtPoint(fourthBuildgCorner, buildingLength, buildingLength),
            };
        }

        private static Vector2 createVectorRelativeToOrgin(Vector2 origin, float xOffset, float yOffset)
        {
            return new Vector2(origin.X + xOffset, origin.Y + yOffset);
        }

        public static Vector2 convertPointToRectangleCenter(Vector2 point, float width, float height)
        {
            return new Vector2(point.X + width / 2, point.Y + height / 2);
        }

        private RectangleBody createRectangleBuildingAtPoint(Vector2 point, float width, float height)
        {
            var origin = convertPointToRectangleCenter(point, width, height);

            return new RectangleBody(width, height, origin, _world, _manager, _spriteBatch);
        }

    }

    public class City
    {
        // Future: If different type of Body is made create an abstraction on that concept
        


    }


    //TODO: Move this to it's own class
    public class SquareBuilding
    {
        //private RectangleBody createRectangleBuildingAtPoint(Vector2 point, float width, float height)
        //{
        //    var origin = GeneratedCity.convertPointToRectangleCenter(point, width, height);

        //    return new RectangleBody(width, height, origin, _world, _manager, _spriteBatch);
        //}
    }
}
