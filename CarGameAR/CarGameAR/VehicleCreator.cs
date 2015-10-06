/************************************************************************************ 
 * Copyright (c) 2014, TU Ilmenau
 * 
 * Build with GoblinsXna framework und the Help from tutorial 8/10/12
 * GoblinsXna use:
 *                  XNA to create the 3D Scenes
 *                  ALVAR Framework to Augmented Reallity
 *                  Newton Physic Engine
 *                  OpenCV to Capture Images
 *                  DirectShow to Capture Images
 * Viel Dank guys
 * ===================================================================================
 * Author: Ohan Oda (ohan@cs.columbia.edu)
 * 
 * modified: Luis Rojas (luis-alejandro.rojas-vargas@tu-ilmenau.de) 
 *           Julian Castro (julian.castro-bosiso@tu-ilmenau.de)
 *           Ricardo Rieckhof (ricardo.rieckhof@tu-ilmenau.de)
 *************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GoblinXNA;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.ParticleEffects;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Physics;
using GoblinXNA.Physics.Newton1;
using GoblinXNA.Helpers;

using NewtonDynamics;

namespace Space_VehicleCreator
{
    public enum CarType_Enum
    {
        Basic = 0,
        Ferrary,
        Tank
    };
    
    /// <summary>
    /// A helper class for creating newton vehicles. This class mimics the tutorial 9 of
    /// the original Newton tutorials written in C++.
    /// </summary>
    public class VehicleCreator
    {
        public static RaceCar AddRaceCarOld(Scene scene, TransformNode parentTrans, Vector3 CarPos, Vector4 CarColor, CarType_Enum CType)
        {
            TransformNode transNode = new TransformNode();
            //transNode.Scale = new Vector3(2.8f, 2.8f, 2.8f);
            transNode.Translation = CarPos;

            Material carMat = new Material();
            carMat.Diffuse = CarColor;
            carMat.Specular = Color.White.ToVector4();
            carMat.SpecularPower = 10;

            GeometryNode carNode = new GeometryNode("Race Car");
            //Intento cargar un modelo
            ModelLoader loader = new ModelLoader();

            //Cargo el modelo Dependiendo del tipo
            switch (CType)
            {
                case CarType_Enum.Basic:
                    carNode.Model = new Box(3, 1.0f, 2);
                    carNode.Material = carMat;
                    break;
                case CarType_Enum.Ferrary:
                    carNode.Model = (Model)loader.Load("Models", "ferrari");
                    ((Model)carNode.Model).UseInternalMaterials = true;
                    break;
                case CarType_Enum.Tank:
                    carNode.Model = (Model)loader.Load("Models", "Osirian_Battle_Tank");
                    ((Model)carNode.Model).UseInternalMaterials = true;
                    break;
                default:
                    carNode.Model = new Box(3, 1.0f, 2);
                    carNode.Material = carMat;
                    break;
            }
            
            carNode.Model.ShadowAttribute = ShadowAttribute.ReceiveCast;

            NewtonPhysics physicsEngine = (NewtonPhysics)scene.PhysicsEngine;

            RaceCar car = new RaceCar(carNode, physicsEngine);
            for (int i = 0; i < 4; i++)
                car.Tires[i] = CreateTire((TireID)Enum.ToObject(typeof(TireID), i), 
                    car.TireTransformNode[i], carNode, scene.PhysicsEngine.Gravity);

            car.Collidable = true;
            car.Interactable = true;

            car.StartPos = CarPos;

            carNode.Physics = car;
            carNode.Physics.NeverDeactivate = true;
            carNode.AddToPhysicsEngine = true;

            parentTrans.AddChild(transNode);
            transNode.AddChild(carNode);

            Newton.NewtonSetBodyLeaveWorldEvent(physicsEngine.NewtonWorld,
                car.LeaveWorldCallback);

            return car;
        }

        public static RaceCar AddRaceCar(Scene scene, TransformNode Marke, Vector3 CarPos, GeometryNode CModel, int IdPlayer)
        {
            TransformNode transNode = new TransformNode();
            transNode.Name = "Tcar:" + IdPlayer;
            transNode.Translation = CarPos;

            CModel.Model.ShadowAttribute = ShadowAttribute.ReceiveCast;

            NewtonPhysics physicsEngine = (NewtonPhysics)scene.PhysicsEngine;

            RaceCar car = new RaceCar(CModel, physicsEngine);
            for (int i = 0; i < 4; i++)
                car.Tires[i] = CreateTire((TireID)Enum.ToObject(typeof(TireID), i),
                    car.TireTransformNode[i], CModel, scene.PhysicsEngine.Gravity);

            car.Collidable = true;
            car.Interactable = true;
            car.StartPos = CarPos;
            //Physic material Name
            car.MaterialName = "Car" + IdPlayer;

            CModel.Physics = car;
            CModel.Physics.NeverDeactivate = true;
            CModel.AddToPhysicsEngine = true;

            //Add To node
            transNode.AddChild(CModel);
            Marke.AddChild(transNode);

            Newton.NewtonSetBodyLeaveWorldEvent(physicsEngine.NewtonWorld,
                car.LeaveWorldCallback);

            return car;
        }

        private static NewtonTire CreateTire(TireID tireID, TransformNode tireTrans,
            GeometryNode carNode, float gravity)
        {
            NewtonTire tire = new NewtonTire();

            Material tireMat = new Material();
            tireMat.Diffuse = Color.Orange.ToVector4();
            tireMat.Specular = Color.White.ToVector4();
            tireMat.SpecularPower = 10;

            float tireRadius = 1.4f;
            GeometryNode tireNode = new GeometryNode("Race Car " + tireID + " Tire");
            tireNode.Model = new Cylinder(tireRadius, tireRadius, 0.9f, 20);
            tireNode.Material = tireMat;

            carNode.AddChild(tireTrans);
            tireTrans.AddChild(tireNode);

            tire.Mass = 5.0f;
            tire.Width = 0.3f * 1.25f;
            tire.Radius = tireRadius;

            switch (tireID)
            {
                case TireID.FrontLeft:
                    tire.TireOffsetMatrix = Matrix.CreateTranslation(new Vector3(-5.9f, 0, -3.0f));
                    break;
                case TireID.FrontRight:
                    tire.TireOffsetMatrix = Matrix.CreateTranslation(new Vector3(-5.9f, 0, 3.0f));
                    break;
                case TireID.RearLeft:
                    tire.TireOffsetMatrix = Matrix.CreateTranslation(new Vector3(5.0f, 0, -3.0f));
                    break;
                case TireID.RearRight:
                    tire.TireOffsetMatrix = Matrix.CreateTranslation(new Vector3(5.0f, 0, 3.0f));
                    break;
            }

            // the tire will spin around the lateral axis of the same tire space
            tire.Pin = Vector3.UnitZ;

            tire.SuspensionLength = RaceCar.SUSPENSION_LENGTH;

            float x = RaceCar.SUSPENSION_LENGTH;
            tire.SuspensionSpring = (200.0f * (float)Math.Abs(gravity)) / x;
            //tireSuspesionSpring = (100.0f * dAbs (GRAVITY)) / x;

            float w = (float)Math.Sqrt(tire.SuspensionSpring);

            // a critically damped suspension act too jumpy for a race car
            tire.SuspensionShock = 1.0f * w;

            // make it a little super critical y damped
            //tireSuspesionShock = 2.0f * w;

            tire.CollisionID = 0x100;

            return tire;
        }
    }
}
