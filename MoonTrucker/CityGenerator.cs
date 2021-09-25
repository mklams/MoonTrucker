using System;
using System.Collections.Generic;
using System.Linq;
using Genbox.VelcroPhysics.Dynamics;
using Genbox.VelcroPhysics.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonTrucker
{
    // TODO: This class is no longer needed
    public class GeneratedCity
    {
        private VehicleWithPhysics _mainVehicle;
        private PropFactory _bodyFactory;

        public GeneratedCity(PropFactory bodyFactory, VehicleWithPhysics mainVehicle)
        {
            _bodyFactory = bodyFactory;
            _mainVehicle = mainVehicle;
        }

        public List<RectangleProp> GenerateCity()
        {
            var tileWidth = _mainVehicle.Height * 1.5f;
            var map = new GameMap(tileWidth, _bodyFactory, new Vector2(0, 0));
            return map.ParseMap();
        }

        public List<IDrawable> GenerateSquareCity()
        {
            var buildingLength = _mainVehicle.Height * 3f;
            var roadLaneWidth = _mainVehicle.Height * 1.5f;

            var city = createSquareCityBlock(new Vector2(0f,0f), buildingLength, roadLaneWidth);
            var nextPos = city.Last().TopRightCorner;
            city.AddRange(createHiddenPassageBuilding(nextPos, buildingLength, buildingLength * 3, roadLaneWidth));


            return city;
        }


        /// <summary>
        /// Creates a city block with four square buildings
        /// </summary>
        /// <param name="blockCorner"></param>
        /// <returns></returns>
        private List<IDrawable> createSquareCityBlock(Vector2 blockCorner, float buildingLength, float roadLaneWidth)
        {
            
            var topLeftBuilding = new SquareBuilding(buildingLength, roadLaneWidth, blockCorner, _bodyFactory);
            var topRightBuilding = new SquareBuilding(buildingLength, roadLaneWidth, topLeftBuilding.TopRightCorner, _bodyFactory);
            var bottomLeftBuilding = new SquareBuilding(buildingLength, roadLaneWidth, topLeftBuilding.BottomLeftCorner, _bodyFactory);
            var bottomRightBuilding = new SquareBuilding(buildingLength, roadLaneWidth, topLeftBuilding.BottomRightCorner, _bodyFactory);

            return new List<IDrawable>
            {  
                topLeftBuilding,
                bottomLeftBuilding,
                bottomRightBuilding,
                topRightBuilding, // BAD ASSUMPTION: last building has top left corner of block
            };
        }

        private List<IDrawable> createHiddenPassageBuilding(Vector2 topLeftCorner, float buildingWidth, float buildingHeight, float roadLaneWidth)
        {
            var buildingLeftCorner = Vector2.Add(topLeftCorner, new Vector2(roadLaneWidth, roadLaneWidth));
            var topSection = new RectangleBuilding(buildingWidth, buildingHeight/3, 0, buildingLeftCorner, _bodyFactory);
            var middleHiddenSection = new RectangleBuilding(buildingWidth, buildingHeight/3, 0, topSection.BottomLeftCorner, _bodyFactory, true);
            var bottomSection = new RectangleBuilding(buildingWidth, buildingHeight / 3, 0, middleHiddenSection.BottomLeftCorner, _bodyFactory);

            return new List<IDrawable>
            {
                topSection,
                middleHiddenSection,
                bottomSection
            };

        }

    }

    public interface IDrawable
    {
        public void Draw();

        public Vector2 TopRightCorner { get; }
        public Vector2 BottomRightCorner { get; }
        public Vector2 BottomLeftCorner { get; }

    }

    //TODO: Move this to it's own class
    public class SquareBuilding : IDrawable
    {
        private RectangleProp _buildingBody;
        public Vector2 TopRightCorner { get; }
        public Vector2 BottomRightCorner { get; }
        public Vector2 BottomLeftCorner { get; }

        public SquareBuilding(float length, float streeWidth, Vector2 leftCorner, PropFactory bodyFactory)
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

    //TODO: Move this to it's own class
    public class RectangleBuilding : IDrawable
    {
        private RectangleProp _buildingBody;
        public Vector2 TopRightCorner { get; }
        public Vector2 BottomRightCorner { get; }
        public Vector2 BottomLeftCorner { get; }

        public RectangleBuilding(float width, float height, float streeWidth, Vector2 leftCorner, PropFactory bodyFactory, bool isSensor = false)
        {
            float widthOffsetToOrigin = streeWidth + width / 2; // add street width since streets surround the entire building 
            float heightOffsetToOrigin = streeWidth + height / 2; // add street width since streets surround the entire building 

            Vector2 buildingOrigin = Vector2.Add(leftCorner, new Vector2(widthOffsetToOrigin, heightOffsetToOrigin));

            _buildingBody = isSensor ? bodyFactory.CreateRectangleSensor(width, height, buildingOrigin) : bodyFactory.CreateRectangleBody(width, height, buildingOrigin);

            float widthOffSetToCorner = streeWidth * 2 + width; // add 2 street width since there is a street on each side of building
            float heightOffSetToCorner = streeWidth * 2 + height;
            TopRightCorner = Vector2.Add(leftCorner, new Vector2(widthOffSetToCorner, 0));
            BottomRightCorner = Vector2.Add(leftCorner, new Vector2(widthOffSetToCorner, heightOffSetToCorner));
            BottomLeftCorner = Vector2.Add(leftCorner, new Vector2(0, heightOffSetToCorner));

        }

        public void Draw()
        {
            _buildingBody.Draw();
        }
    }
}
