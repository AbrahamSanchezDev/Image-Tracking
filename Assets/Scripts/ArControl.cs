using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Worlds
{
    [System.Serializable]
    public class ArImageObj
    {
        public string Name;
        public Texture2D Image;
        public GameObject Prefab;
        [System.NonSerialized] public GameObject RunTimeObj;
    }

    public class ArControl : MonoBehaviour
    {
        public List<ArImageObj> RunTimeObjects = new List<ArImageObj>();
        private XRReferenceImageLibrary _imageLibrary;
        private ARTrackedImageManager _trackedImageManager;
        private ARSessionOrigin _origin;

        public Text DebugText;

        private GameObject _empty;

        protected void Awake()
        {
            _origin = FindObjectOfType<ARSessionOrigin>();
            _imageLibrary = Resources.Load<XRReferenceImageLibrary>("Ar/ReferenceImageLibrary");
            _trackedImageManager = _origin.gameObject.GetComponent<ARTrackedImageManager>();
            _empty = new GameObject();
            _trackedImageManager.trackedImagePrefab = _empty;
            SpawnObjs();
        }

        private void SpawnObjs()
        {
            var targetObj = Camera.main;
            for (var i = 0; i < RunTimeObjects.Count; i++)
            {
                var obj = Instantiate(RunTimeObjects[i].Prefab);
                RunTimeObjects[i].RunTimeObj = obj;
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.SetActive(false);
                //StartCoroutine("AddImageCo", RunTimeObjects[i]);
                var textObj = obj.transform.GetComponentInChildren<TextMeshPro>();
                if (textObj != null)
                {
                    var look = textObj.gameObject.AddComponent<WorldSpaceUI>();
                    look.LookOnMyY = false;
                    look.SetUpCameraTransform(targetObj.transform);
                    look.SetLookAtCam(true);
                }
            }
        }

        protected void OnEnable()
        {
            _trackedImageManager.trackedImagesChanged += OnImageTracked;
        }

        protected void OnDisable()
        {
            _trackedImageManager.trackedImagesChanged -= OnImageTracked;
        }

        private string toShow = "";

        private void OnImageTracked(ARTrackedImagesChangedEventArgs data)
        {
            for (var i = 0; i < data.added.Count; i++)
            {
                toShow += "Added " + data.added[i].referenceImage.name;
                UpdateImage(data.added[i]);
            }

            for (var i = 0; i < data.updated.Count; i++)
            {
                toShow += "updated " + data.updated[i].referenceImage.name;
                UpdateImage(data.updated[i]);
            }
            for (var i = 0; i < data.removed.Count; i++)
            {
                toShow += "removed " + data.removed[i].referenceImage.name;
                DeSpawn(data.removed[i].referenceImage.name);
            }
            if (toShow.Length > 150)
                toShow = toShow.Substring(0, 150);
            ShowText(toShow);
        }

        private void UpdateImage(ARTrackedImage data)
        {
            var theName = data.referenceImage.name;
            var position = data.transform.position;
            var go = ShowObj(theName, true);
            if (go != null)
            {
                go.transform.position = position;
                //go.transform.rotation = data.transform.rotation;
                //_trackedImageManager.enabled = false;
                //_trackedImageManager.trackedImagePrefab = go;
                //_trackedImageManager.enabled = true;
                ShowText("tracking " + theName);
            }

            else
            {
                ShowText("null Obj");
            }
        }

        private void DeSpawn(string theName)
        {
            ShowObj(theName, false);
        }

        private GameObject ShowObj(string theName, bool show)
        {
            for (var i = 0; i < RunTimeObjects.Count; i++)
                if (RunTimeObjects[i].Name == theName)
                {
                    RunTimeObjects[i].RunTimeObj.SetActive(show);
                    return RunTimeObjects[i].RunTimeObj;
                }
            return null;
        }

        //protected IEnumerator AddImageCo(ArImageObj data)
        //{
        //    yield return null;
        //    var fist = new SerializableGuid(0, 0);
        //    var second = new SerializableGuid(0, 0);
        //    var newImage = new XRReferenceImage(fist, second, new Vector2(0.1f, 0.1f), data.Name, data.Image);
        //    if (!_trackedImageManager.subsystem.SubsystemDescriptor.supportsMutableLibrary)
        //    {
        //        ShowText("Does not have support");
        //    }
        //    MutableRuntimeReferenceImageLibrary library =
        //        _trackedImageManager.referenceLibrary as MutableRuntimeReferenceImageLibrary;
        //    var handle = library.ScheduleAddImageJob(data.Image, data.Name, 0.1f);
        //    while (!handle.IsCompleted)
        //    {
        //        yield return null;
        //    }
        //    ShowText("Finish Loading");
        //    Debug.Log("Finish Loading");
        //}

        private void ShowText(string theText)
        {
            if (DebugText)
                DebugText.text = theText;
        }
    }
}