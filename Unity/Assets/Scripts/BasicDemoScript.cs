using Microsoft.Azure.SpatialAnchors;
using Microsoft.Azure.SpatialAnchors.Unity;
using Microsoft.Azure.SpatialAnchors.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BasicDemoScript : InputInteractionBase
{

    #region Member Variables

    private string currentAnchorId = "";
    private CloudSpatialAnchor currentCloudAnchor;
    private CloudSpatialAnchorWatcher currentWatcher;
    private GameObject spawnedObject = null;

    #endregion

    #region Unity Inspector Variables

    [SerializeField]
    [Tooltip("The prefab used to represent an anchored object.")]
    private GameObject anchoredObjectPrefab = null;

    [SerializeField]
    [Tooltip("SpatialAnchorManager instance to use for this demo. This is required.")]
    private SpatialAnchorManager cloudManager = null;

    [SerializeField]
    private Text logText;

    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the prefab used to represent an anchored object.
    /// </summary>
    public GameObject AnchoredObjectPrefab { get { return anchoredObjectPrefab; } }

    /// <summary>
    /// Gets the <see cref="SpatialAnchorManager"/> instance used by this demo.
    /// </summary>
    public SpatialAnchorManager CloudManager { get { return cloudManager; } }
    #endregion // Public Properties

    #region Unity Methods

    public override void Start()
    {
        StartASA();
        base.Start();
    }

    public override void OnDestroy()
    {
        if (CloudManager != null)
        {
            CloudManager.StopSession();
        }

        if (currentWatcher != null)
        {
            currentWatcher.Stop();
            currentWatcher = null;
        }

        base.OnDestroy();
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when a touch object interaction occurs.
    /// </summary>
    /// <param name="hitPoint">The position.</param>
    /// <param name="target">The target.</param>
    protected override void OnSelectObjectInteraction(Vector3 hitPoint, object target)
    {
        Quaternion rotation = Quaternion.AngleAxis(0, Vector3.up);

        SpawnOrMoveCurrentAnchoredObject(hitPoint, rotation);
    }

    /// <summary>
    /// Called when a touch interaction occurs.
    /// </summary>
    /// <param name="touch">The touch.</param>
    protected override void OnTouchInteraction(Touch touch)
    {
        base.OnTouchInteraction(touch);
    }

    #endregion

    #region Events

    private void CloudManager_SessionStarted(object sender, EventArgs e)
    {
        Log("event: CloudManager_SessionStarted");
    }

    private void CloudManager_LocateAnchorsCompleted(object sender, LocateAnchorsCompletedEventArgs args)
    {
        // TODO: Expand
        //OnCloudLocateAnchorsCompleted(args);

        Log("event: CloudManager_LocateAnchorsCompleted");
    }

    private void CloudManager_Error(object sender, SessionErrorEventArgs args)
    {
        // TODO: Expand
        //isErrorActive = true;
        Log("event: CloudManager_Error: " + args.ErrorMessage);

        //UnityDispatcher.InvokeOnAppThread(() => this.feedbackBox.text = string.Format("Error: {0}", args.ErrorMessage));
    }

    private void CloudManager_LogDebug(object sender, OnLogDebugEventArgs args)
    {
        Log("event: CloudManager_LogDebug: " + args.Message);
    }

    private void CloudManager_AnchorLocated(object sender, AnchorLocatedEventArgs args)
    {
        // TODO: Expand
        Log("event: CloudManager_AnchorLocated");
        Debug.LogFormat("Anchor recognized as a possible anchor {0} {1}", args.Identifier, args.Status);
        if (args.Status == LocateAnchorStatus.Located)
        {
            //OnCloudAnchorLocated(args);
        }
    }

    private void CloudManager_SessionUpdated(object sender, SessionUpdatedEventArgs args)
    {
        //Debug.Log("CloudManager_SessionUpdated");
        Log("event: CloudManager_SessionUpdated");
        //OnCloudSessionUpdated();
    }

    #endregion

    #region Methods

    public void TryStartASA()
    {
        
    }

    private async Task StartASA()
    {
        Log("\nStarting...");

        await Task.Delay(3000);

        currentAnchorId = "";
        currentCloudAnchor = null;

        CloudManager.SessionStarted += CloudManager_SessionStarted;
        CloudManager.SessionUpdated += CloudManager_SessionUpdated;
        CloudManager.AnchorLocated += CloudManager_AnchorLocated;
        CloudManager.LocateAnchorsCompleted += CloudManager_LocateAnchorsCompleted;
        CloudManager.LogDebug += CloudManager_LogDebug;
        CloudManager.Error += CloudManager_Error;

        try
        {
            Log("Creating session...");

            await CloudManager.CreateSessionAsync();

            await Task.Delay(3000);

            Log("Starting session...");

            await CloudManager.StartSessionAsync();

            Log("Tap anywhere to place object");
        }
        catch (Exception e)
        {
            Log("Error: " + e.Message + "\n" + e.InnerException + "\n" + e.GetType().Name + "\n" + e.HelpLink);
            Debug.Log(e.Message);
            Debug.Log(e.InnerException);
        }
    }

    /// <summary>
    /// Saves the current object anchor to the cloud.
    /// </summary>
    protected async Task SaveCurrentObjectAnchorToCloudAsync()
    {
        /*// Get the cloud-native anchor behavior
        CloudNativeAnchor cna = spawnedObject.GetComponent<CloudNativeAnchor>();

        // If the cloud portion of the anchor hasn't been created yet, create it
        if (cna.CloudAnchor == null) { cna.NativeToCloud(); }

        // Get the cloud portion of the anchor
        CloudSpatialAnchor cloudAnchor = cna.CloudAnchor;

        // In this sample app we delete the cloud anchor explicitly, but here we show how to set an anchor to expire automatically
        cloudAnchor.Expiration = DateTimeOffset.Now.AddDays(7);

        while (!CloudManager.IsReadyForCreate)
        {
            await Task.Delay(330);
            float createProgress = CloudManager.SessionStatus.RecommendedForCreateProgress;
            feedbackBox.text = $"Move your device to capture more environment data: {createProgress:0%}";
        }

        bool success = false;

        feedbackBox.text = "Saving...";

        try
        {
            // Actually save
            await CloudManager.CreateAnchorAsync(cloudAnchor);

            // Store
            currentCloudAnchor = cloudAnchor;

            // Success?
            success = currentCloudAnchor != null;

            if (success && !isErrorActive)
            {
                // Await override, which may perform additional tasks
                // such as storing the key in the AnchorExchanger
                await OnSaveCloudAnchorSuccessfulAsync();
            }
            else
            {
                OnSaveCloudAnchorFailed(new Exception("Failed to save, but no exception was thrown."));
            }
        }
        catch (Exception ex)
        {
            OnSaveCloudAnchorFailed(ex);
        }*/
    }

    /// <summary>
    /// Spawns a new anchored object.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
    /// <param name="worldRot">The world rotation.</param>
    /// <returns><see cref="GameObject"/>.</returns>
    private GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot)
    {
        // Create the prefab
        GameObject newGameObject = GameObject.Instantiate(AnchoredObjectPrefab, worldPos, worldRot);

        // Attach a cloud-native anchor behavior to help keep cloud
        // and native anchors in sync.
        newGameObject.AddComponent<CloudNativeAnchor>();

        // Set the color
        //newGameObject.GetComponent<MeshRenderer>().material.color = GetStepColor();

        // Return created object
        return newGameObject;
    }

    /// <summary>
    /// Spawns a new object.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
    /// <param name="worldRot">The world rotation.</param>
    /// <param name="cloudSpatialAnchor">The cloud spatial anchor.</param>
    /// <returns><see cref="GameObject"/>.</returns>
    private GameObject SpawnNewAnchoredObject(Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor)
    {
        // Create the object like usual
        GameObject newGameObject = SpawnNewAnchoredObject(worldPos, worldRot);

        // If a cloud anchor is passed, apply it to the native anchor
        if (cloudSpatialAnchor != null)
        {
            CloudNativeAnchor cloudNativeAnchor = newGameObject.GetComponent<CloudNativeAnchor>();
            cloudNativeAnchor.CloudToNative(cloudSpatialAnchor);
        }

        // Set color
        //newGameObject.GetComponent<MeshRenderer>().material.color = GetStepColor();

        // Return newly created object
        return newGameObject;
    }

    /// <summary>
    /// Spawns a new anchored object and makes it the current object or moves the
    /// current anchored object if one exists.
    /// </summary>
    /// <param name="worldPos">The world position.</param>
    /// <param name="worldRot">The world rotation.</param>
    private void SpawnOrMoveCurrentAnchoredObject(Vector3 worldPos, Quaternion worldRot)
    {
        // Create the object if we need to, and attach the platform appropriate
        // Anchor behavior to the spawned object
        if (spawnedObject == null)
        {
            // Use factory method to create
            spawnedObject = SpawnNewAnchoredObject(worldPos, worldRot, currentCloudAnchor);

            // Update color
            //spawnedObjectMat = spawnedObject.GetComponent<MeshRenderer>().material;
        }
        else
        {
            // Use factory method to move
            MoveAnchoredObject(spawnedObject, worldPos, worldRot, currentCloudAnchor);
        }
    }

    /// <summary>
    /// Moves the specified anchored object.
    /// </summary>
    /// <param name="objectToMove">The anchored object to move.</param>
    /// <param name="worldPos">The world position.</param>
    /// <param name="worldRot">The world rotation.</param>
    /// <param name="cloudSpatialAnchor">The cloud spatial anchor.</param>
    protected virtual void MoveAnchoredObject(GameObject objectToMove, Vector3 worldPos, Quaternion worldRot, CloudSpatialAnchor cloudSpatialAnchor = null)
    {
        // Get the cloud-native anchor behavior
        CloudNativeAnchor cna = objectToMove.GetComponent<CloudNativeAnchor>();

        // Warn and exit if the behavior is missing
        if (cna == null)
        {
            Debug.LogWarning($"The object {objectToMove.name} is missing the {nameof(CloudNativeAnchor)} behavior.");
            return;
        }

        // Is there a cloud anchor to apply
        if (cloudSpatialAnchor != null)
        {
            // Yes. Apply the cloud anchor, which also sets the pose.
            cna.CloudToNative(cloudSpatialAnchor);
        }
        else
        {
            // No. Just set the pose.
            cna.SetPose(worldPos, worldRot);
        }
    }

    private void Log(string message)
    {
        var msg = message + "\n" + logText.text;
        logText.text = msg;
        Debug.Log(message);
    }

    protected virtual void CleanupSpawnedObjects()
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
            spawnedObject = null;
        }

        //if (spawnedObjectMat != null)
        //{
        //    Destroy(spawnedObjectMat);
        //    spawnedObjectMat = null;
        //}
    }

    #endregion


}
