using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// wasd 이동, F1로 시점 변환


[ExecuteInEditMode]
public class CarMaker : EditorWindow
{
    // Car Object to make
    private GameObject carObject;

    // Car settings of the car
    private CarSettings carSettings;

    // Gameobject that contains the body mesh of the car
    private GameObject carBodyMesh;

    // Front left Wheel mesh gameobject
    private GameObject wheelFrontLeft;

    // Front right Wheel mesh gameobject
    private GameObject wheelFrontRight;

    // back left Wheel mesh gameobject
    private GameObject wheelBackLeft;

    // back right Wheel mesh gameobject
    private GameObject wheelBackRight;


    // show advanced settings of the menu
    private bool showAdvancedSettings = false;

    // Create a camera as well if true
    private bool makeCamera = true;

    // Base rotation of car body mesh to match the default wheel forward direction
    public float meshRotationY = 0;

    // how slippery the car should be, higher value = more slippery
    public float slipperiness = 1;

    // Currently disabled because it throws errors
    Editor gameObjectEditor;
    GameObject lastGameObject;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Car Maker/Open")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(CarMaker));
    }

    void OnGUI()
    {
        ///if no car object selected
        if (carObject == null) {

            ///init car settings
            carSettings = new CarSettings();

            GUILayout.Label("Please Select Car GameObject!", EditorStyles.boldLabel);
        }

        carObject = (GameObject)EditorGUILayout.ObjectField(carObject, typeof(GameObject), true);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        /*
        if (carObject != null)
        {
            if (gameObjectEditor == null) {
                gameObjectEditor = Editor.CreateEditor(carObject);
                lastGameObject = carObject;

                carSettings = new CarSettings();
            }

            if (lastGameObject != carObject) {
                gameObjectEditor = Editor.CreateEditor(carObject);
                lastGameObject = carObject;

                carSettings = new CarSettings();
            }


            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(200, 200), GUIStyle.none);
        }

        */

        //display car object preview texture
        if (carObject != null) {
            Texture2D tex2d = AssetPreview.GetAssetPreview(carObject);

            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                   GUILayout.Label(tex2d);
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        //display settings and buttons
        if (carObject != null)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Car Body Mesh");
            carBodyMesh = EditorGUILayout.ObjectField(carBodyMesh, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Auto Detect Wheels"))
            {
                AutoDetectWheels();
            }

            EditorGUILayout.Space();


            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Front Left");
            wheelFrontLeft = EditorGUILayout.ObjectField(wheelFrontLeft, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Front Right");
            wheelFrontRight = EditorGUILayout.ObjectField(wheelFrontRight, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Back Left");
            wheelBackLeft = EditorGUILayout.ObjectField(wheelBackLeft, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Back Right");
            wheelBackRight = EditorGUILayout.ObjectField(wheelBackRight, typeof(Object), true) as GameObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            meshRotationY = EditorGUILayout.FloatField("MeshRotation", meshRotationY);
            makeCamera = EditorGUILayout.Toggle("Make Camera", makeCamera);

            EditorGUILayout.Space();
            EditorGUILayout.Space(); 
            EditorGUILayout.Space();

            showAdvancedSettings = EditorGUILayout.Toggle("Advanced Settings", showAdvancedSettings);
            if (showAdvancedSettings) {
                EditorGUILayout.Space();

                EditorGUI.indentLevel++;

                carSettings.mass = EditorGUILayout.FloatField("mass", carSettings.mass);
                carSettings.drag = EditorGUILayout.FloatField("drag", carSettings.drag);
                carSettings.centerOfMass = EditorGUILayout.Vector3Field("centerOfMass", carSettings.centerOfMass);
                carSettings.motorTorque = EditorGUILayout.FloatField("motorTorque", carSettings.motorTorque);
                carSettings.steeringAngle = EditorGUILayout.FloatField("steeringAngle", carSettings.steeringAngle);

                slipperiness = EditorGUILayout.FloatField("slipperiness", slipperiness);
            }


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            string carMeshBodyError = "ok";

            if (carBodyMesh == null)
                carMeshBodyError = "Please Select Car body Mesh!";

            else if (carBodyMesh.GetComponent<MeshRenderer>() == null)
                carMeshBodyError = "Wrong Car Body Mesh : Please Select the gameobject with car body Meshrenderer!";


            if(carMeshBodyError != "ok")
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.red;
                style.fontSize = 15;
                style.fontStyle = FontStyle.Bold;
                EditorGUILayout.LabelField(carMeshBodyError, style);
            }
            else if (GUILayout.Button("Make Car"))
            {
                GameObject car = MakeCar();

                if (car != null)
                    if (makeCamera)
                        MakeCamera(car);
            }

        }


    }

    // Create car prefab with the current car maker settings
    /// <returns></returns>
    private GameObject MakeCar()
    {
        ///get save path
        var path = EditorUtility.SaveFilePanelInProject(
           "Select new car location",
           "New Car",
           "prefab",
           "message");

        GameObject newCar = null;


        //if car is null
        if (carObject == null) {
            Debug.LogError("Please Assign Car GameObject!");
        }
        else {
            //init car maker Prefab GameObject
            newCar = new GameObject();
            newCar.transform.position = Vector3.zero;

            //add rigidbody to root
            Rigidbody rb = newCar.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
           
            //calculate body mesh rotation
            Quaternion meshRot = Quaternion.Euler(Vector3.zero);
            meshRot = Quaternion.Euler(0, meshRotationY, 0);


            //add car container
            GameObject carContainer = GameObject.Instantiate(carObject, Vector3.zero, meshRot, newCar.transform) as GameObject;
           
            //get car body mesh
            GameObject carMesh = GetChildByName(carContainer, carBodyMesh.name);

            //add collider to car body mesh
            Collider collider = carMesh.GetComponent<Collider>();
            if (collider !=null)
            {
                MeshCollider meshCollider = carMesh.GetComponent<MeshCollider>();
                if (meshCollider != null)
                    meshCollider.convex = true;

            }
            else
                carMesh.AddComponent<MeshCollider>().convex = true;



            //create container for wheels
            GameObject wheelsContainer = new GameObject();
            wheelsContainer.name = "wheelsContainer";
            wheelsContainer.transform.parent = newCar.transform;
            wheelsContainer.transform.localPosition = Vector3.zero;
            wheelsContainer.transform.localRotation = meshRot;

            //create wheels 
            WheelCollider wheelColliderFrontLeft = AddWheelCollider(wheelsContainer, wheelFrontLeft, "Front Left", true);
            WheelCollider wheelColliderFrontRight = AddWheelCollider(wheelsContainer, wheelFrontRight, "Front Right", true);
            WheelCollider wheelColliderBackLeft = AddWheelCollider(wheelsContainer, wheelBackLeft, "Back Left", false);
            WheelCollider wheelColliderBackRight = AddWheelCollider(wheelsContainer, wheelBackRight, "Back Right", false);


            //Create the new car Prefab.
            GameObject newPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newCar, path, InteractionMode.UserAction);
            
            //set car root name
            newCar.name = newPrefab.name;

            //add car CarControler
            CarControler carController = newCar.AddComponent<CarControler>();
            
            //asign car settings
            carController.carSettings = carSettings;

            //init axleInfos List
            carController.wheelAxleList = new List<WheelAxle>();

            //create Front Wheels
            WheelAxle axelInfoFront = new WheelAxle();
            axelInfoFront.wheelColliderRight = wheelColliderFrontRight;
            axelInfoFront.wheelColliderLeft = wheelColliderFrontLeft;
            axelInfoFront.wheelMeshLeft = GetChildByName(carContainer,  wheelFrontLeft.name);
            axelInfoFront.wheelMeshRight = GetChildByName(carContainer, wheelFrontRight.name);
            axelInfoFront.motor = true;
            axelInfoFront.steering = true;
            carController.wheelAxleList.Add(axelInfoFront);

            //create Back Wheels
            WheelAxle axelInfoBack = new WheelAxle();
            axelInfoBack.wheelColliderRight = wheelColliderBackRight;
            axelInfoBack.wheelColliderLeft = wheelColliderBackLeft;
            axelInfoBack.wheelMeshLeft = GetChildByName(carContainer, wheelBackLeft.name);
            axelInfoBack.wheelMeshRight = GetChildByName(carContainer, wheelBackRight.name);
            axelInfoBack.motor = false;
            axelInfoBack.steering = false;
            carController.wheelAxleList.Add(axelInfoBack);

        }


        return newCar;
    }

    /// Create Camera
    /// <param name="car"></param>
    private void MakeCamera(GameObject car)
    {
        // Remove Existing cameras
        if (Camera.allCameras.Length > 0)
        {
            for (int i = 0; i < Camera.allCameras.Length; i++)
            {
                DestroyImmediate(Camera.allCameras[i].gameObject);
            }
        }

        //create camera GameObject
        GameObject newCamera = new GameObject();
        newCamera.name = "CarMakerCamera";

        //add scripts to GameObject
        newCamera.AddComponent<Camera>();
        newCamera.AddComponent<AudioListener>();
        
        //add CarCamera script
        CarCamera carCamera = newCamera.AddComponent<CarCamera>();

        //asign default camera settings
        carCamera.carCameraSettingsList = new List<CarCameraSettings>();
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings0());
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings1());
        carCamera.carCameraSettingsList.Add(CarCameraSettings.GetDefaultSettings2());

        //set camera target to follow
        carCamera.target = car.transform;

    }

    // Automaticly try to detect wheels of the car GameObject
    private void AutoDetectWheels() {
        //get all childtren transforms
        Transform[] allChildren = carObject.GetComponentsInChildren<Transform>(true);

        //iterate all children
        for (int i = 0; i < allChildren.Length; i++)
        {
            //ignore parent object
            if (allChildren[i] == carObject.transform)
                continue;

            //front left
            if (FindChildrenWithMatch(allChildren[i], searchWheelFrontLeft))
                wheelFrontLeft = allChildren[i].gameObject;

            //front right
            if (FindChildrenWithMatch(allChildren[i], searchWheelFrontRight))
                wheelFrontRight = allChildren[i].gameObject;

            //back left
            if (FindChildrenWithMatch(allChildren[i], searchWheelBackLeft))
                wheelBackLeft = allChildren[i].gameObject;

            //back right
            if (FindChildrenWithMatch(allChildren[i], searchWheelBackRight))
                wheelBackRight = allChildren[i].gameObject;
        }

    }

    private static string[][] searchWheelFrontLeft = new string[][]{
        new string[] { "front", "left"},
        new string[] { "fl"}
    };

    private static string[][] searchWheelFrontRight = new string[][]{
        new string[] { "front", "right"},
        new string[] { "fr"}
    };

    private static string[][] searchWheelBackLeft = new string[][]{
        new string[] { "back", "left"},
        new string[] { "rear", "left"},
        new string[] { "bl"}
    };

    private static string[][] searchWheelBackRight = new string[][]{
        new string[] { "back", "right"},
        new string[] { "rear", "right"},
        new string[] { "br"}
    };


    // Try to match transform name string with multiple strings
    /// <param name="childTransform"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    private bool FindChildrenWithMatch(Transform childTransform, string[][] criteria)
    {
        bool b = false;

        //iterate all possible string matches
        for (int i = 0; i < criteria.Length; i++)
        {
            //create bool array for the matches
            bool[] matchAaray = new bool[criteria[i].Length];

            //iterate all strings
            for (int i1 = 0; i1 < criteria[i].Length; i1++)
            {
                //set match array to true if there is string match
                if (childTransform.name.ToLower().Contains(criteria[i][i1]) == true)
                    matchAaray[i1] = true;

               
            }

            bool allMatch = true;

            //check if all matches are true
            foreach (bool match in matchAaray)
            {
                if (match == false)
                    allMatch = false;
            }

            if (allMatch == true)
                b = true;
        }

        return b;
    }


    // Create and add WheelCollider to given parent GameObject
    /// <param name="parent"></param>
    /// <param name="wheelMeshGO"></param>
    /// <param name="name"></param>
    /// <param name="isFrontWheel"></param>
    /// <returns></returns>
    private WheelCollider AddWheelCollider(GameObject parent, GameObject wheelMeshGO, string name, bool isFrontWheel)
    {
        //create wheel GameObject and set position and rotation
        GameObject wheel = new GameObject();
        wheel.transform.parent = parent.transform;
        wheel.transform.localPosition = wheelMeshGO.transform.localPosition;
        wheel.transform.localRotation = Quaternion.identity;
        wheel.name = name;

        //add wheel collider GameObject
        WheelCollider wheelCollider = wheel.AddComponent<WheelCollider>();
        wheelCollider.mass = 40.0f;

        //adjust wheel collider forward friction settings
        WheelFrictionCurve wfcForward = wheelCollider.forwardFriction;
        wfcForward.extremumSlip = 0.05f;
        wfcForward.extremumValue = 1.0f;
        wfcForward.asymptoteSlip = 2.0f;
        wfcForward.asymptoteValue = 2.0f;
        wheelCollider.forwardFriction = wfcForward;

        //adjust wheel collider sideways friction settings
        WheelFrictionCurve wfcSideways = wheelCollider.forwardFriction;
        wfcSideways.extremumSlip = 0.05f;
        wfcSideways.extremumValue = 1.0f;

        //if this is back wheel
        if (isFrontWheel == false)
        {    
            wfcSideways.extremumSlip = 0.1f;
            wfcSideways.extremumValue = 1.2f / slipperiness;
        }


        wheelCollider.sidewaysFriction = wfcSideways;

        //try to get mesh render of the wheel
        Renderer wheelRenderer = wheelMeshGO.GetComponent<Renderer>();

        if (wheelRenderer) {
            ///calculate size of the wheelcollider based on the renderer size
            wheelCollider.radius = wheelRenderer.bounds.size.y / 2;
        }


        return wheelCollider;
    }

    // returns child GameObject if there is a name match
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private GameObject GetChildByName(GameObject parent, string name)
    {
        GameObject childGO = null;
        Transform[] transformArray = parent.GetComponentsInChildren<Transform>();

        //iterate all child transform
        foreach (Transform t in transformArray)
        {
            //fix name if it was a copy
            t.name = t.name.Replace("(Clone)", "");

            //check for match
            if (t.name == name)
                childGO = t.gameObject;
        }

        return childGO;
    }
}
