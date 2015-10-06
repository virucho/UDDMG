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
 * Authors:  Luis Rojas (luis-alejandro.rojas-vargas@tu-ilmenau.de) 
 *           Julian Castro (julian.castro-bosiso@tu-ilmenau.de)
 *           Ricardo Rieckhof (ricardo.rieckhof@tu-ilmenau.de)
 *************************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/***** Namespaces to use GoblinsXNA *****/
using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Physics;
using GoblinXNA.Physics.Newton1;
using GoblinXNA.Shaders;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.UI;

namespace CarGameAR
{
    public enum BulletType_Enum
    {
        CUBE = 0,
        BALL,
        MODEL
    };
    
    class Graphics3D
    {
        #region Member Fields

        private MarkerNode groundMarkerNode;                //Principal marker for AR ""Ground"
        TransformNode parentTNodeGrd;                       //Parent for all Scene Objects

        private IShadowMap GShadowMap;                      //Shadow Map

        private bool LoadLevel;                             //Flag for load the all Level
        private bool LoadCars;                              //Flag for load the all Cars
        
        /******** Bullets *********/
        private int BulletID = 0;                           //Count for Bullets in the Scene
        public Material BulletrMat;                        //Material For Bullets
        public PrimitiveModel BulletModel;                 //Bullet Model
        /******** Lapida **********/
        //public GeometryNode LapidaNode;

        /******* Obstacles *******/
        private static string[] ObsModName = new string[] { "muro1",
                                                            "muro2",
                                                            "Muro",
                                                            "Maso",
                                                            "Tank",
                                                            "muro3",
                                                            "muro4",
                                                            "muro5"};
        /******* Weapons *******/
        private static string WeaponsModName = "torreta_";

        #endregion

        #region Constructors

        public Graphics3D(MarkerNode MarkerNode, IShadowMap ShadowMap)
        {
            //Asignation Ground Node
            this.groundMarkerNode = MarkerNode;
            //Asignation Shadow Map
            this.GShadowMap = ShadowMap;

            //Add Big Parent for All Scenary Elements
            parentTNodeGrd = new TransformNode();
            parentTNodeGrd.Name = "Level";
            groundMarkerNode.AddChild(parentTNodeGrd);

            // Create a material for the model
            BulletrMat = new Material();
            BulletrMat.Diffuse = Color.Blue.ToVector4();
            BulletrMat.Specular = Color.White.ToVector4();
            BulletrMat.SpecularPower = 10;

            //create Bullet Model
            BulletModel = new TexturedBox(2.0f);

            LoadLevel = false;
            LoadCars = false;
        }

        #endregion

        #region Public Methods

        /********************* Basic Elements ****************/

        public GeometryNode CreateCube(MarkerNode Marker, Vector4 CubeColor)
        {
            GeometryNode boxNode;
            // Create a geometry node with a model of a box
            boxNode = new GeometryNode("Box");
            boxNode.Model = new TexturedBox(32.4f);

            // Add this box model to the physics engine for collision detection
            boxNode.AddToPhysicsEngine = true;
            boxNode.Physics.Shape = ShapeType.Box;
            // Make this box model cast and receive shadows
            boxNode.Model.ShadowAttribute = ShadowAttribute.ReceiveCast;
            // Assign a shadow shader for this model that uses the IShadowMap we assigned to the scene
            boxNode.Model.Shader = new SimpleShadowShader(GShadowMap);

            // Create a material to apply to the box model
            Material boxMaterial = new Material();
            boxMaterial.Diffuse = CubeColor;
            boxMaterial.Specular = Color.White.ToVector4();
            boxMaterial.SpecularPower = 10;

            boxNode.Material = boxMaterial;

            // Add this box model node to the ground marker node
            Marker.AddChild(boxNode);

            return boxNode;

            // Create a collision pair and add a collision callback function that will be
            // called when the pair collides
            //NewtonPhysics.CollisionPair pair = new NewtonPhysics.CollisionPair(boxNode.Physics, sphereNode.Physics);
            //((NewtonPhysics)scene.PhysicsEngine).AddCollisionCallback(pair, BoxSphereCollision);
        }

        public static void CreateBall(MarkerNode Marker)
        {
            // Create a geometry node for Sphere
            PrimitiveModel PsphereModel = new Sphere(15f, 20, 20);

            // Create a material to apply to the sphere model
            Material sphereMaterial = new Material();
            sphereMaterial.Diffuse = new Vector4(0, 0.5f, 0, 1);
            sphereMaterial.Specular = Color.White.ToVector4();
            sphereMaterial.SpecularPower = 10;

            GeometryNode sphereNode = new GeometryNode("Sphere");
            //sphereNode.Model = new TexturedSphere(16, 20, 20);
            sphereNode.Model = PsphereModel;
            sphereNode.Material = sphereMaterial;

            // Add this sphere model to the physics engine for collision detection
            sphereNode.Physics.Interactable = true;
            sphereNode.Physics.Collidable = true;
            sphereNode.Physics.Shape = ShapeType.Sphere;
            sphereNode.Physics.Mass = 30;

            //sphereNode.Physics.ApplyGravity = false;
            sphereNode.Physics.InitialLinearVelocity = new Vector3(30, 0, 0);
            //sphereNode.Physics.MaterialName = "Sphere";
            //sphereNode.Physics.ApplyGravity = true;
            sphereNode.AddToPhysicsEngine = true;

            // Make this sphere model cast and receive shadows
            //sphereNode.Model.ShadowAttribute = ShadowAttribute.ReceiveCast;
            // Assign a shadow shader for this model that uses the IShadowMap we assigned to the scene
            //sphereNode.Model.Shader = new SimpleShadowShader(scene.ShadowMap);

            TransformNode sphereTransNode = new TransformNode();
            sphereTransNode.Translation = new Vector3(50, 0, 50);

            // Now add the above nodes to the scene graph in the appropriate order.
            // Note that only the nodes added below the marker node are affected by 
            // the marker transformation.
            Marker.AddChild(sphereTransNode);
            sphereTransNode.AddChild(sphereNode);
        }

        public void CreateShootBullet(Vector3 InitPos, Vector3 DirBullet, Color BulletColor)
        {
            if (BulletID == 20)
                return;
            
            GeometryNode ShootBullet = new GeometryNode("ShootBullet" + BulletID++);

            ShootBullet.Model = BulletModel;

            BulletrMat.Diffuse = BulletColor.ToVector4();

            ShootBullet.Material = BulletrMat;
            ShootBullet.Physics.Interactable = true;
            ShootBullet.Physics.Collidable = true;
            ShootBullet.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            ShootBullet.Physics.Mass = 60f;
            ShootBullet.Physics.MaterialName = "Bullet";
            ShootBullet.AddToPhysicsEngine = true;

            // Assign the initial velocity to this shooting box
            ShootBullet.Physics.InitialLinearVelocity = new Vector3(DirBullet.X * 80, DirBullet.Y * 80, DirBullet.Z * 50);

            TransformNode BulletTrans = new TransformNode();
            BulletTrans.Translation = InitPos;

            groundMarkerNode.AddChild(BulletTrans);
            BulletTrans.AddChild(ShootBullet);
        }

        /********************* Scene Elements ****************/

        public static LightNode CreateAtmLight()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSource = lightSource;

            // Set this light node to cast shadows (by just setting this to true will not cast any shadows,
            // scene.ShadowMap needs to be set to a valid IShadowMap and Model.Shader needs to be set to
            // a proper IShadowShader implementation
            lightNode.CastShadows = true;

            // You should also set the light projection when casting shadow from this light
            lightNode.LightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                1, 1f, 500);

            return lightNode;
            //scene.RootNode.AddChild(lightNode);
        }

        public void CreateGround(bool Occluder)
        {
            GeometryNode groundNode = new GeometryNode("Ground");

            //Cordenates X, y, Z
            groundNode.Model = new TexturedBox(324, 180, 0.1f);

            // Set this model Occluded ist or not
            groundNode.IsOccluder = Occluder;

            //Setup Physics Aspecte
            groundNode.Physics.Collidable = true;
            groundNode.Physics.Shape = GoblinXNA.Physics.ShapeType.Box;
            groundNode.Physics.MaterialName = "Ground";
            groundNode.AddToPhysicsEngine = true;

            // Make the ground model to receive shadow casted by other objects
            groundNode.Model.ShadowAttribute = ShadowAttribute.ReceiveOnly;
            // Assign a shadow shader
            groundNode.Model.Shader = new SimpleShadowShader(GShadowMap);

            //Material
            Material groundMaterial = new Material();
            groundMaterial.Diffuse = Color.Gray.ToVector4();
            groundMaterial.Specular = Color.White.ToVector4();
            groundMaterial.SpecularPower = 20;

            //Assig Material
            groundNode.Material = groundMaterial;

            //Add Model in the Scene
            parentTNodeGrd.AddChild(groundNode);
        }

        public void CreateGroundLevel(string Folder, String Model, Vector3 ObjPos, Vector3 Scala)
        {
            GeometryNode groundNode;

            groundNode = LoadModel("Models/" + Folder, Model, true);

            //Setup Physics Aspecte
            groundNode.Physics.Collidable = true;

            // Make the ground model to receive shadow casted by other objects
            //groundNode.Model.ShadowAttribute = ShadowAttribute.ReceiveOnly;
            // Assign a shadow shader
            //groundNode.Model.Shader = new SimpleShadowShader(scene.ShadowMap);

            TransformNode parentTransNode = new TransformNode("ground");
            parentTransNode.Scale = Scala;
            parentTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.ToRadians(90));
            //Add Rotation because Blender Axis
            parentTransNode.Translation = ObjPos;
            parentTransNode.AddChild(groundNode);

            //Add Model in the Scene
            parentTNodeGrd.AddChild(parentTransNode);
        }

        public void CreateSceneObj(string Folder, string ObjName, Vector3 ObjPos, Vector3 ObjScale)
        {
            GeometryNode ObjNode;

            ObjNode = LoadModel("Models/" + Folder, ObjName, true);

            //Setup Physics Aspecte
            ObjNode.Physics.Collidable = false;
            ObjNode.AddToPhysicsEngine = false;

            TransformNode parentTransNode = new TransformNode(ObjName);
            parentTransNode.Scale = ObjScale;
            parentTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.ToRadians(90));
            parentTransNode.Translation = ObjPos;
            parentTransNode.AddChild(ObjNode);

            //Add Model in the Scene
            parentTNodeGrd.AddChild(parentTransNode);
        }

        public void AddLapida(Vector3 PosCar, int LevelHeight, float Scala)
        {
            GeometryNode LapidaNode = LoadModel("Models", "lapida", true);
            LapidaNode.Physics.Collidable = true;
            
            TransformNode parentTransNode = new TransformNode("Lapida");
            parentTransNode.Scale = new Vector3(Scala, Scala + 2, Scala);
            parentTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.ToRadians(90));
            parentTransNode.Translation = new Vector3(PosCar.X, PosCar.Y, LevelHeight + 10);
            //Add Rotation because Blender Axis
            parentTransNode.AddChild(LapidaNode);

            //Add Model in the Scene
            groundMarkerNode.AddChild(parentTransNode);
        }

        public static GeometryNode AddModel(MarkerNode Marker, String Folder, String Model, bool IntMaterial, float Scala)
        {
            GeometryNode ObstNode; 

            if (Model == "null")
                return ObstNode = new GeometryNode();

            ObstNode = Graphics3D.LoadModel("Models/" + Folder, Model, IntMaterial);

            //define the Physic Material name
            ObstNode.Physics.MaterialName = "Obstacle";

            TransformNode parentTransNode = new TransformNode();
            parentTransNode.Name = Model;
            parentTransNode.Scale = new Vector3(Scala, Scala + 2, Scala);
            parentTransNode.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.ToRadians(90));
            //Add Rotation because Blender Axis
            parentTransNode.AddChild(ObstNode);

            // Add this box model node to the ground marker node
            if(Marker != null)
                Marker.AddChild(parentTransNode);

            return ObstNode;
        }

        /********************* Levels ****************/

        public void CreateLevel1()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreateLevel1\n");

            if (LoadLevel == false)
            {
                //Create the base Ground
                CreateGroundLevel("Scene1", "escenario1_piso", new Vector3(0, -10, 0), new Vector3(50, 20, 50));

                CreateSceneObj("Scene1", "escenario1_edif", new Vector3(0, 0, 0), new Vector3(20, 20, 20));

                LoadLevel = true;
            }
        }

        public void CreateLevel2()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreateLevel2\n"); 
            
            if (LoadLevel == false)
            {
                //Create the base Ground
                CreateGroundLevel("Scene2", "escenario2_piso1", new Vector3(0, -60, 0), new Vector3(10, 10, 10));
                CreateGroundLevel("Scene2", "escenario2_piso2", new Vector3(0, -60, 0), new Vector3(10, 10, 10));
                CreateGroundLevel("Scene2", "escenario2_piso3", new Vector3(0, -60, 0), new Vector3(10, 10, 10));
                CreateGroundLevel("Scene2", "escenario2_piso4", new Vector3(0, -60, 0), new Vector3(10, 10, 10));

                CreateSceneObj("Scene2", "escenario2_obj", new Vector3(0, -60, 0), new Vector3(10, 10, 10));

                LoadLevel = true;
            }
        }

        public void CreateLevel3()
        {
            Console.WriteLine(DateTime.Now.ToString() + " CreateLevel3\n"); 
            
            if (LoadLevel == false)
            {
                //Create the base Ground
                CreateGroundLevel("Scene3", "escenario3_floor1", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor2", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor3", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor4", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor5", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor6", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_floor7", new Vector3(0, 0, 0), new Vector3(20, 20, 20));

                CreateGroundLevel("Scene3", "escenario3_wall1", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_wall2", new Vector3(0, 0, 0), new Vector3(20, 20, 20));
                CreateGroundLevel("Scene3", "escenario3_wall3", new Vector3(0, 0, 0), new Vector3(20, 20, 20));

                //CreateSceneObj("Scene3", "escenario3_walls", new Vector3(0, 0, 0), new Vector3(20, 20, 20));

                LoadLevel = true;
            }
        }

        public void DestroyLevel()
        {
            Console.WriteLine(DateTime.Now.ToString() + " DestroyLevel\n"); 
            
            parentTNodeGrd.RemoveChildren();
            parentTNodeGrd.Children.Clear();

            LoadLevel = false;
        }

        /********************* Helpers **********************/

        public static GeometryNode LoadModel(string DirModel, string Modelname, bool InternalMaterial)
        {
            GeometryNode MyNode = new GeometryNode(Modelname);
            //Intento cargar un modelo
            ModelLoader loader = new ModelLoader();

            MyNode.Physics.Mass = 20;
            MyNode.Physics.Shape = ShapeType.ConvexHull;
            MyNode.Physics.MaterialName = Modelname;
            MyNode.Physics.ApplyGravity = true;
            //Debo probar mas esta propiedad
            MyNode.Physics.NeverDeactivate = true;
            MyNode.AddToPhysicsEngine = true;

            //Load Model
            MyNode.Model = (Model)loader.Load(DirModel, Modelname);

            //Define Material
            if (InternalMaterial)
            {
                ((Model)MyNode.Model).UseInternalMaterials = true;
            }
            else
            {
                // Create a material for the Model
                Material ModelMat = new Material();
                ModelMat.Diffuse = Color.Blue.ToVector4();
                ModelMat.Specular = Color.White.ToVector4();
                ModelMat.SpecularPower = 20;
                //Add Material
                MyNode.Material = ModelMat;
            }

            return MyNode;
        }

        public string GetName(int Idx)
        {
            return ObsModName[Idx];
        }

        #endregion

        #region Properties

        public string[] ObstNames
        {
            get { return ObsModName; }
        }

        public string WeaponNames
        {
            get { return WeaponsModName; }
        }

        public bool IsLoadLevel
        {
            get { return LoadLevel; }
        }

        public bool IsLoadCars
        {
            get { return LoadCars; }
            set { LoadCars = value; }
        }

        #endregion
    }
}
