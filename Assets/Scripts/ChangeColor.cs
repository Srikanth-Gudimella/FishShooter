using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FishShooting
{

    public class ChangeColor : MonoBehaviour
    {
        public MeshRenderer objectRenderer;
        public Material material;
        // Start is called before the first frame update
        void Start()
        {
            objectRenderer = GetComponent<MeshRenderer>();
            material = objectRenderer.material;
            //material = new Material(objectRenderer.material);
            //objectRenderer.material = material;

            //if (objectRenderer == null)
            //{
            //    Debug.LogError("Object does not have a Renderer component.");
            //}
        }

        public void ChangeMaterialColor(Color newColor)
        {
            if (objectRenderer != null)
            {
                //objectRenderer.material = material;
               Debug.Log("------------- Color");
               // objectRenderer.material.color = newColor;
                material.color = newColor;
            }
        }
    }
}
