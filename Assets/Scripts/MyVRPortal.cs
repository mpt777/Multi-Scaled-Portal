using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Assertions;


enum MyTriggerAxis
{
    X,
    Y,
    Z
};

public class MyVRPortal : MonoBehaviour
{
	
	[Tooltip("Increase this value to increase quality. Lower this value to increase performance. Default is 1.")]
	public float renderQuality = 1f;

	[Tooltip("The maximum distance at which the portal begins to deform away from the main camera to avoid clipping.")]
	public float maximumDeformRange = 1f;

	[Tooltip("The power with which the portal deforms away from the camera. If you see a flicker as you pass through, increase this variable.")]
	public float deformPower = .5f;

	[Tooltip("The portal deforms away from rigidbodies, but if you give them a tag and set it here, it will ignore them. This is really good for the Vive Controllers.")]
	public string ignoreRigidbodyTag;

	[Tooltip("This mask defines the parts of the scene that are visible in all dimensions. Always set the layers individually, don't use Everything.")]
	public LayerMask alwaysVisibleMask;

	[Tooltip("Oblique Projection clips the camera exactly to the portal rectangle. This is really good if you've got nearby objects. Unfortunately, it also screws with the skybox on Gear VR.")]
	public bool enableObliqueProjection = false;

	// Test against this bool to see if you are currently inside of the starting dimension, or the ending dimension for the portal.
	public bool dimensionSwitched { get; private set; }

	[TextArea]
	[Tooltip(" ")]
	public string Notes = "Hover over each variable to get tooltips with more information on what they do. Quick Tip: " +
		"Don't set the visible mask to Everything. Select each option you want to be always visible.";


	public Camera mainCamera;
	public Camera teleportCamera;

	public GameObject otherSideCenterPivot;
	public GameObject thisSideCenterPivot;

	private Renderer meshRenderer;
	private MeshFilter meshFilter;
	private MeshDeformer meshDeformer;

	private bool triggerZDirection;

	private List<PortalTransitionObject> transitionObjects = new List<PortalTransitionObject>();

	// Rendering & VR Support
	private Camera renderCam;
	private Skybox camSkybox;
	private RenderTexture leftTexture;
	private RenderTexture rightTexture;

	private float portalSwitchDistance = 0.03f;

    private void OnEnable()
    {
		meshRenderer = GetComponent<Renderer>();
		meshFilter = GetComponent<MeshFilter>();
		meshDeformer = GetComponent<MeshDeformer>();

		mainCamera = Camera.main;
		teleportCamera = GameObject.FindWithTag("PortalCamera").GetComponent<Camera>();

		Vector3 convertedPoint = transform.InverseTransformPoint(mainCamera.transform.position);
		triggerZDirection = (convertedPoint.z > 0);
	}

    Vector3 getRelativePosition(Transform cam, Transform pivot)
	{
		
		return cam.position - pivot.position;
	}

	Vector3 getMainCameraRotation()
	{
		return mainCamera.transform.rotation.eulerAngles;
	}
	

	// Use this for initialization
	void Start()
	{
		meshRenderer = GetComponent<Renderer>();
		meshFilter = GetComponent<MeshFilter>();
		meshDeformer = GetComponent<MeshDeformer>();

		this.mainCamera = Camera.main;

		Assert.IsNotNull(this.mainCamera, "Portal could not find a main camera in your scene.");

		Vector3 convertedPoint = transform.InverseTransformPoint(mainCamera.transform.position);
		triggerZDirection = (convertedPoint.z > 0);
	}

	private void OnDestroy()
	{
		if (teleportCamera != null)
		{
			Destroy(teleportCamera.gameObject);
			teleportCamera = null;
		}

		if (leftTexture != null)
		{
			Destroy(leftTexture);
		}
		if (rightTexture != null)
		{
			Destroy(rightTexture);
		}
	}

	// ---------------------------------
	// Rendering and Display
	// ---------------------------------

	void OnWillRenderObject()
	{
		// Create the textures and camera if they don't exist.
		if (!leftTexture)
		{
			Vector2 texSize = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);

			leftTexture = new RenderTexture((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);
			rightTexture = new RenderTexture((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);

			renderCam = teleportCamera; 


			if (renderCam.GetComponent<Skybox>())
			{
				camSkybox = renderCam.GetComponent<Skybox>();
			}
			else
			{
				renderCam.gameObject.AddComponent<Skybox>();
				camSkybox = renderCam.GetComponent<Skybox>();
			}

			CameraExtensions.ClearCameraComponents(renderCam.GetComponent<Camera>());
		}

		meshRenderer.material.SetFloat("_RecursiveRender", (gameObject.layer != Camera.current.gameObject.layer) ? 1 : 0);
		RenderPortal(Camera.current);
	}

	private void RenderPortal(Camera camera)
	{
		
		if (camera.stereoEnabled)
		{  
			this.RenderSteamVR(camera);
		}
	}

	private void RenderSteamVR(Camera camera)
	{
		teleportCamera.ResetWorldToCameraMatrix();
		Matrix4x4 camera_to_world = mainCamera.transform.parent.transform.localToWorldMatrix;
		Matrix4x4 world_to_pivot = thisSideCenterPivot.transform.worldToLocalMatrix;


        var scale = Matrix4x4.Scale(new Vector3(1, 1, 1));

        if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Left)
        {
			Vector3 eyePos = camera.transform.TransformPoint(SteamVR.instance.eyes[0].pos); 
			Quaternion eyeRot = camera.transform.rotation * SteamVR.instance.eyes[0].rot;

			Matrix4x4 camera_local = Matrix4x4.TRS(eyePos, eyeRot, new Vector3(1,1,1));
			Matrix4x4 camera_to_parent = mainCamera.transform.parent.transform.worldToLocalMatrix * camera_local;
			Matrix4x4 camera_to_pivot = world_to_pivot * camera_local * scale;

			teleportCamera.transform.localPosition = camera_to_pivot.GetPosition();
			teleportCamera.transform.localRotation = camera_to_pivot.GetRotation();

			Matrix4x4 projectionMatrix = GetSteamVRProjectionMatrix(camera, Valve.VR.EVREye.Eye_Left);
            RenderTexture target = leftTexture;

            RenderPlane(renderCam, target, eyePos, eyeRot, projectionMatrix, mainCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
            meshRenderer.material.SetTexture("_LeftTex", target);
		} 
		
		if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Right)
        {
		
			Vector3 eyePos = camera.transform.TransformPoint(SteamVR.instance.eyes[1].pos);
			Quaternion eyeRot = camera.transform.rotation * SteamVR.instance.eyes[1].rot;
			Matrix4x4 camera_local = Matrix4x4.TRS(eyePos, eyeRot, new Vector3(1, 1, 1));
			Matrix4x4 camera_to_parent = mainCamera.transform.parent.transform.worldToLocalMatrix * camera_local;
			Matrix4x4 camera_to_pivot = world_to_pivot * camera_local * scale;
			teleportCamera.transform.localPosition = camera_to_pivot.GetPosition();
			teleportCamera.transform.localRotation = camera_to_pivot.GetRotation();


			Matrix4x4 projectionMatrix = GetSteamVRProjectionMatrix(camera, Valve.VR.EVREye.Eye_Right);
            RenderTexture target = rightTexture;

            RenderPlane(renderCam, target, eyePos, eyeRot, projectionMatrix, mainCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right));
            meshRenderer.material.SetTexture("_RightTex", target);
		}
    }

	protected void RenderPlane(Camera portalCamera, RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix, Matrix4x4 pm)
	{
		portalCamera.targetTexture = targetTexture;
		portalCamera.ResetWorldToCameraMatrix();

        portalCamera.projectionMatrix = camProjectionMatrix;

        var scale = Matrix4x4.Scale(new Vector3(1, 1, 1));
        //var scale = Matrix4x4.Scale(new Vector3(1, 1, 1));

		portalCamera.projectionMatrix = portalCamera.projectionMatrix * scale;

        // Hide the other dimensions
        portalCamera.enabled = false;
        portalCamera.cullingMask = 0;

        CameraExtensions.LayerCullingShowMask(portalCamera, alwaysVisibleMask);

        portalCamera.orthographic = mainCamera.orthographic;
        portalCamera.orthographicSize = mainCamera.orthographicSize;
		portalCamera.fieldOfView = mainCamera.fieldOfView;
        portalCamera.aspect = mainCamera.aspect;

		portalCamera.Render();

		portalCamera.targetTexture = null;

    }

    // Creates a clip plane for the projection matrix that clips to the portal.
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));

		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * portalSwitchDistance * (triggerZDirection ? -1 : 1);
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}

	public static Matrix4x4 GetSteamVRProjectionMatrix(Camera cam, Valve.VR.EVREye eye)
	{
		Valve.VR.HmdMatrix44_t proj = SteamVR.instance.hmd.GetProjectionMatrix(eye, cam.nearClipPlane, cam.farClipPlane);
		
		Matrix4x4 m = new Matrix4x4();
		m.m00 = proj.m0;
		m.m01 = proj.m1;
		m.m02 = proj.m2;
		m.m03 = proj.m3;
		m.m10 = proj.m4;
		m.m11 = proj.m5;
		m.m12 = proj.m6;
		m.m13 = proj.m7;
		m.m20 = proj.m8;
		m.m21 = proj.m9;
		m.m22 = proj.m10;
		m.m23 = proj.m11;
		m.m30 = proj.m12;
		m.m31 = proj.m13;
		m.m32 = proj.m14;
		m.m33 = proj.m15;
		return m;
	}

    private Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
		// from unity docs. if I can scale left right bottom top then it should be okay
        var x = (2.0f * near) / (right - left);
        var y = (2.0f * near) / (top - bottom);
        var a = (right + left) / (right - left);
        var b = (top + bottom) / (top - bottom);
        var c = -(far + near) / (far - near);
        var d = -(2.0f * far * near) / (far - near);
        var e = -1.0f;

        var m = new Matrix4x4();
        m[0, 0] = x;
		m[0, 1] = 0.0f; 
		m[0, 2] = a; 
		m[0, 3] = 0.0f;
        m[1, 0] = 0.0f;
		m[1, 1] = y; 
		m[1, 2] = b; 
		m[1, 3] = 0.0f;
        m[2, 0] = 0.0f; 
		m[2, 1] = 0.0f;
		m[2, 2] = c; 
		m[2, 3] = d;
        m[3, 0] = 0.0f;
		m[3, 1] = 0.0f; 
		m[3, 2] = e; 
		m[3, 3] = 0.0f;
        return m;
    }


}
