using System;
using System.Collections.Generic;
using Genbox.VelcroPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonTrucker
{
    public class GeneratedCity
    {
        private SpriteBatch _spriteBatch;
        private float _screenWidth;
        private float _screenHeight;
        private World _world;
        private VehicleWithPhysics _mainVehicle;
        private TextureManager _manager;

        public GeneratedCity(SpriteBatch spriteBatch, float screenWidth, float screenHeight, World world, VehicleWithPhysics mainVehicle, TextureManager manager)
        {
            _spriteBatch = spriteBatch;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _world = world;
            _mainVehicle = mainVehicle;
            _manager = manager;
        }

        public List<RectangleBody> GenerateSquareCity()
        {
            var city = createBoundryWalls();
            var screenCenter = new Vector2(_screenWidth / 2f, _screenHeight / 2f);
            city.AddRange(createSquareCityBlock(new Vector2(0,0)));

            return city;
        }

        private List<RectangleBody> createBoundryWalls()
        {
            return new List<RectangleBody>
            {
                RectangleBody.CreateRectangleBodyFromDisplayUnits(5f, _screenHeight, new Vector2(2.5f, _screenHeight / 2f), _world, _manager, _spriteBatch),
                RectangleBody.CreateRectangleBodyFromDisplayUnits(5f, _screenHeight, new Vector2(_screenWidth - 2.5f, _screenHeight / 2f), _world, _manager, _spriteBatch),
                RectangleBody.CreateRectangleBodyFromDisplayUnits(_screenWidth, 5f, new Vector2(_screenWidth / 2, 2.5f), _world, _manager, _spriteBatch),
                RectangleBody.CreateRectangleBodyFromDisplayUnits(_screenWidth, 5f, new Vector2(_screenWidth / 2, _screenHeight - 5f), _world, _manager, _spriteBatch),
            };
        }

        private RectangleBody createSquare(float sideLength, Vector2 position)
        {
            return RectangleBody.CreateRectangleBodyFromDisplayUnits(sideLength, sideLength, position, _world, _manager, _spriteBatch);
        }

        /// <summary>
        /// Creates a city block with four square buildings
        /// </summary>
        /// <param name="blockCorner"></param>
        /// <returns></returns>
        private List<RectangleBody> createSquareCityBlock(Vector2 blockCorner)
        {
            float vehicleHeight = _mainVehicle.GetWidth();
            var buildingLength = _mainVehicle.GetWidth() * 3f;
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

        private static Vector2 convertPointToRectangleCenter(Vector2 point, float width, float height)
        {
            return new Vector2(point.X + width / 2, point.Y + height / 2);
        }

        private RectangleBody createRectangleBuildingAtPoint(Vector2 point, float width, float height)
        {
            var origin = convertPointToRectangleCenter(point, width, height);

            return RectangleBody.CreateRectangleBodyFromDisplayUnits(width, height, origin, _world, _manager, _spriteBatch);
        }

    }

    public class City
    {
        // Future: If different type of Body is made create an abstraction on that concept
        private SquareBody[] _cityBody;


    }


    //TODO: Move this to it's own class
    public class SquareBody
    {

    }
}
