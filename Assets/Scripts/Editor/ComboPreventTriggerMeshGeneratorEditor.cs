using GoedWareGameJam3.MonoBehaviours.Combines;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GoedWareGameJam3.EditorBehaviour
{
    [CustomEditor(typeof(ComboPreventTriggerMeshGenerator))]
    public class ComboPreventTriggerMeshGeneratorEditor : Editor
    {
        private SerializedProperty _height;
        private SerializedProperty _radius;
        private SerializedProperty _standHeight;
        private SerializedProperty _standWidth;

        private float _materialSize = 1f;

        private int _childsCount = 0;
        private Transform[] _childObjects;

        private ComboPreventTriggerMeshGenerator _targetComponent;

        private void OnEnable()
        {
            _targetComponent = (ComboPreventTriggerMeshGenerator)target;

            _height = serializedObject.FindProperty("_height");
            _radius = serializedObject.FindProperty("_radius");
            _standHeight = serializedObject.FindProperty("_standHeight");
            _standWidth = serializedObject.FindProperty("_standWidth");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _childObjects = _targetComponent.GetPoints();

            _height.floatValue = EditorGUILayout.FloatField("Height: ", _height.floatValue);
            if (_height.floatValue <= 0f)
            {
                _height.floatValue = 0.5f;
            }

            _radius.floatValue = EditorGUILayout.FloatField("Radius: ", _radius.floatValue);
            if (_radius.floatValue <= 0.09f)
            {
                _radius.floatValue = 0.1f;
            }

            GUILayout.Label($"Children amount: {_childObjects.Length}");

            if (GUILayout.Button("Generate mesh"))
            {
                GenerateForceFieldMesh(_childObjects);
                GenerateStandMesh(_childObjects);
            }

            if (GUILayout.Button("Generate colliders"))
            {
                GenerateColliders(_childObjects);
            }


            if (GUI.changed)
            {
                if (target != null)
                    EditorUtility.SetDirty(target);
                _height.serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnSceneGUI()
        {
            DrawLines(_childObjects);
        }

        private void DrawLines(Transform[] points)
        {
            Handles.color = Color.green;

            for (int i = 0; i < points.Length; i++)
            {
                if (i + 1 >= points.Length)
                {
                    break;
                }

                Handles.DrawLine(points[i].position, points[i + 1].position);
                Handles.DrawLine(points[i].position + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue), points[i + 1].position + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue));
            }
        }

        private void GenerateColliders(Transform[] childObjects)
        {
            if (childObjects.Length <= 1)
            {
                Debug.LogError($"There is less than 2 children objects on {_targetComponent.name} object to generate colliders. Needs more.");
                return;
            }

            for (int i = 0; i < childObjects.Length - 1; i++)
            {
                childObjects[i].LookAt(childObjects[i + 1]);

                float magnitude = (childObjects[i + 1].position - childObjects[i].position).magnitude;

                if (!childObjects[i].gameObject.TryGetComponent(out BoxCollider collider))
                {
                    collider = childObjects[i].gameObject.AddComponent<BoxCollider>();
                }
                collider.center = new Vector3(0f, _height.floatValue / 2f + _standHeight.floatValue, magnitude / 2f);
                collider.size = new Vector3(_radius.floatValue * 2f, _height.floatValue + _standHeight.floatValue * 2f, magnitude);
                collider.isTrigger = true;
            }
        }
        private void GenerateForceFieldMesh(Transform[] childObjects)
        {
            Mesh mesh = new Mesh();

            int childObjectCount = childObjects.Length;

            if (childObjectCount <= 1)
            {
                Debug.LogError($"There is less than 2 children objects on {_targetComponent.name} object to generate mesh. Needs more.");
                return;
            }

            Debug.Log($"Generating mesh for {childObjects.Length - 1} objects");

            Vector3[] vertices = new Vector3[2 * 4 + (childObjectCount - 1) * 12]; // One vertex for each face of a mesh except bottom, we don't need bottom face
            Vector2[] uvs = new Vector2[vertices.Length];
            Vector3[] normals = new Vector3[vertices.Length];

            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[((childObjectCount - 1) * 6 * 3) + 2 * 3 + 2 * 3]; // 2 at the start, 2 at the end and 6 for each point except last.

            int currentIndex = 0;
            Vector3 perpendicular = Vector3.Cross((childObjects[currentIndex + 1].position - childObjects[currentIndex].position).normalized, childObjects[currentIndex].up);

            //Debug.Log($"{vertices.Length} vertices, {triangles.Length} triangles");
            //Debug.Log($"perpendicular = {perpendicular}");

            Vector3 localPosition = childObjects[currentIndex].localPosition;
            Vector3[] previousMainPoints = new Vector3[4];
            Vector3[] currentMainPoints = new Vector3[4];
            Vector3 normalDirection;

            float height = 0f;
            float length = 0f;
            float heightScale = 0f;
            float uvScale = 0f;

            int vertexIndex = 0;
            int triangleIndex = 0;



            vertices[vertexIndex] = localPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 1] = vertices[vertexIndex] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 2] = localPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 3] = vertices[vertexIndex + 2] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);

            height = (vertices[vertexIndex] - vertices[vertexIndex + 1]).magnitude;
            length = (vertices[vertexIndex + 1] - vertices[vertexIndex + 3]).magnitude;
            uvScale = length / height;
            heightScale = height / _height.floatValue;

            uvs[vertexIndex]     = new Vector2(uvScale, 0);
            uvs[vertexIndex + 1] = new Vector2(uvScale, heightScale);
            uvs[vertexIndex + 3] = new Vector2(0, heightScale);
            uvs[vertexIndex + 2] = new Vector2(0, 0);

            previousMainPoints[0] = vertices[vertexIndex];
            previousMainPoints[1] = vertices[vertexIndex + 1];
            previousMainPoints[2] = vertices[vertexIndex + 2];
            previousMainPoints[3] = vertices[vertexIndex + 3];

            triangles[triangleIndex++] = vertexIndex;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 2;

            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 3;
            triangles[triangleIndex++] = vertexIndex + 2;

            vertexIndex += 4;

            for (int childObjectIndex = 1; childObjectIndex < childObjects.Length; childObjectIndex++)
            {
                Vector3 childObjectLocalPosition = childObjects[childObjectIndex].localPosition;
                if (childObjectIndex + 1 < childObjects.Length)
                {
                    Vector3 prevPerpendicular = Vector3.Cross((childObjects[childObjectIndex - 1].position - childObjects[childObjectIndex].position).normalized, childObjects[childObjectIndex].up);
                    Vector3 nextPerpendicular = Vector3.Cross((childObjects[childObjectIndex + 1].position - childObjects[childObjectIndex].position).normalized, childObjects[childObjectIndex].up);

                    perpendicular = (nextPerpendicular - prevPerpendicular).normalized;
                }

                if (childObjectIndex == childObjects.Length - 1)
                {
                    perpendicular = Vector3.Cross((childObjects[childObjectIndex - 1].position - childObjects[childObjectIndex].position).normalized, childObjects[childObjectIndex].up);
                    perpendicular *= -1;
                    Debug.Log($"perpendicular on last is: {perpendicular}");
                }

                currentMainPoints[0] = childObjectLocalPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                currentMainPoints[1] = currentMainPoints[0] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);
                currentMainPoints[2] = childObjectLocalPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                currentMainPoints[3] = currentMainPoints[2] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);

                // Left
                vertices[vertexIndex] = currentMainPoints[2];
                vertices[vertexIndex + 1] = currentMainPoints[3];
                vertices[vertexIndex + 2] = previousMainPoints[3];
                vertices[vertexIndex + 3] = previousMainPoints[2];

                height = (vertices[vertexIndex] - vertices[vertexIndex + 1]).magnitude;
                length = (vertices[vertexIndex + 1] - vertices[vertexIndex + 3]).magnitude;
                uvScale = length / height;
                heightScale = height / _height.floatValue;

                uvs[vertexIndex]     = new Vector2(uvScale, 0);
                uvs[vertexIndex + 1] = new Vector2(uvScale, heightScale);
                uvs[vertexIndex + 2] = new Vector2(0, heightScale);
                uvs[vertexIndex + 3] = new Vector2(0, 0);

                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + 3;

                // Top
                vertices[vertexIndex + 4] = currentMainPoints[3];
                vertices[vertexIndex + 5] = currentMainPoints[1];
                vertices[vertexIndex + 6] = previousMainPoints[1];
                vertices[vertexIndex + 7] = previousMainPoints[3];

                height = (vertices[vertexIndex + 4] - vertices[vertexIndex + 5]).magnitude;
                length = (vertices[vertexIndex + 5] - vertices[vertexIndex + 7]).magnitude;
                uvScale = length / height;
                heightScale = height / _height.floatValue;

                uvs[vertexIndex + 4] = new Vector2(uvScale, 0);
                uvs[vertexIndex + 5] = new Vector2(uvScale, heightScale);
                uvs[vertexIndex + 6] = new Vector2(0, heightScale);
                uvs[vertexIndex + 7] = new Vector2(0, 0);

                triangles[triangleIndex++] = vertexIndex + 5;
                triangles[triangleIndex++] = vertexIndex + 4;
                triangles[triangleIndex++] = vertexIndex + 7;
                triangles[triangleIndex++] = vertexIndex + 7;
                triangles[triangleIndex++] = vertexIndex + 6;
                triangles[triangleIndex++] = vertexIndex + 5;

                // Right
                vertices[vertexIndex + 8] = previousMainPoints[0];
                vertices[vertexIndex + 9] = previousMainPoints[1];
                vertices[vertexIndex + 10] = currentMainPoints[1];
                vertices[vertexIndex + 11] = currentMainPoints[0];

                height = (vertices[vertexIndex + 8] - vertices[vertexIndex + 9]).magnitude;
                length = (vertices[vertexIndex + 9] - vertices[vertexIndex + 11]).magnitude;
                uvScale = length / height;
                heightScale = height / _height.floatValue;

                uvs[vertexIndex + 8]  = new Vector2(uvScale, 0);
                uvs[vertexIndex + 9]  = new Vector2(uvScale, heightScale);
                uvs[vertexIndex + 10] = new Vector2(0, heightScale);
                uvs[vertexIndex + 11] = new Vector2(0, 0);

                triangles[triangleIndex++] = vertexIndex + 10;
                triangles[triangleIndex++] = vertexIndex + 9;
                triangles[triangleIndex++] = vertexIndex + 8;
                triangles[triangleIndex++] = vertexIndex + 8;
                triangles[triangleIndex++] = vertexIndex + 11;
                triangles[triangleIndex++] = vertexIndex + 10;

                vertexIndex += 12;

                previousMainPoints[0] = currentMainPoints[0];
                previousMainPoints[1] = currentMainPoints[1];
                previousMainPoints[2] = currentMainPoints[2];
                previousMainPoints[3] = currentMainPoints[3];
            }

            Vector3 lastChildObjectLocalPosition = childObjects[childObjects.Length - 1].localPosition;

            currentMainPoints[0] = lastChildObjectLocalPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            currentMainPoints[1] = currentMainPoints[0] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);
            currentMainPoints[2] = lastChildObjectLocalPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            currentMainPoints[3] = currentMainPoints[2] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);

            vertices[vertexIndex] = currentMainPoints[0];
            vertices[vertexIndex + 1] = currentMainPoints[1];
            vertices[vertexIndex + 2] = currentMainPoints[2];
            vertices[vertexIndex + 3] = currentMainPoints[3];

            height = (vertices[vertexIndex] - vertices[vertexIndex + 1]).magnitude;
            length = (vertices[vertexIndex + 1] - vertices[vertexIndex + 3]).magnitude;
            uvScale = length / height;
            heightScale = height / _height.floatValue;

            uvs[vertexIndex]     = new Vector2(uvScale, 0);
            uvs[vertexIndex + 1] = new Vector2(uvScale, heightScale);
            uvs[vertexIndex + 3] = new Vector2(0, heightScale);
            uvs[vertexIndex + 2] = new Vector2(0, 0);

            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 3;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            foreach(Vector2 uvElement in uvs)
            {
                Debug.Log($"{uvElement}");
            }
            //Unwrapping.GenerateSecondaryUVSet(mesh);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            Debug.Log($"{mesh.normals.Length} normals..");

            mesh.uv = uvs;

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!AssetDatabase.IsValidFolder($"Assets/Scenes/{sceneName}"))
            {
                AssetDatabase.CreateFolder("Assets/Scenes", $"{sceneName}");
            }
            if (!AssetDatabase.IsValidFolder($"Assets/Scenes/{sceneName}/Meshes"))
            {
                AssetDatabase.CreateFolder($"Assets/Scenes/{sceneName}", "Meshes");
            }
            string path = $"Assets/Scenes/{sceneName}/Meshes/{sceneName}_{_targetComponent.name}.asset";
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();

            Mesh loadedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
            loadedMesh.RecalculateNormals();

            Debug.Log($"loadedMesh.normals = {loadedMesh.normals.Length}");
            _targetComponent.SetNewMesh(loadedMesh);
        }

        private void GenerateStandMesh(Transform[] childObjects)
        {
            //Debug.Log($"GenerateStandMesh is called with {vertices.Count} vertices");
            Mesh standMesh = new Mesh();

            Vector3[] vertices = new Vector3[childObjects.Length * 4];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[((childObjects.Length - 1) * 6 * 3) + 2 * 3 + 2 * 3];

            int vertexIndex = 0;
            int triangleIndex = 0;

            Vector3 perpendicular = Vector3.Cross((childObjects[1].position - childObjects[0].position).normalized, childObjects[0].up);
            Vector3 localPosition = childObjects[0].localPosition;

            vertices[vertexIndex] = localPosition - (perpendicular * _radius.floatValue) - (perpendicular * _standWidth.floatValue);
            vertices[vertexIndex + 1] = localPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 2] = localPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 3] = localPosition + (perpendicular * _radius.floatValue) + (perpendicular * _standWidth.floatValue);

            triangles[triangleIndex++] = vertexIndex + 3;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex;
            triangles[triangleIndex++] = vertexIndex + 3;
            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 1;

            vertexIndex += 4;

            for (int i = 1; i < childObjects.Length; i++)
            {
                Vector3 childObjectLocalPosition = childObjects[i].localPosition;
                if (i + 1 < childObjects.Length)
                {
                    Vector3 prevPerpendicular = Vector3.Cross((childObjects[i - 1].position - childObjects[i].position).normalized, childObjects[i].up);
                    Vector3 nextPerpendicular = Vector3.Cross((childObjects[i + 1].position - childObjects[i].position).normalized, childObjects[i].up);

                    perpendicular = (nextPerpendicular - prevPerpendicular).normalized;
                }

                if (i == childObjects.Length - 1)
                {
                    perpendicular = Vector3.Cross((childObjects[i - 1].position - childObjects[i].position).normalized, childObjects[i].up);
                    perpendicular *= -1;
                }

                vertices[vertexIndex] = childObjectLocalPosition - (perpendicular * _radius.floatValue) - (perpendicular * _standWidth.floatValue);
                vertices[vertexIndex + 1] = childObjectLocalPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                vertices[vertexIndex + 2] = childObjectLocalPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                vertices[vertexIndex + 3] = childObjectLocalPosition + (perpendicular * _radius.floatValue) + (perpendicular * _standWidth.floatValue);

                int previousZeroIndex = vertexIndex - 4;

                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = previousZeroIndex;
                triangles[triangleIndex++] = previousZeroIndex + 1;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = previousZeroIndex;

                triangles[triangleIndex++] = previousZeroIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 3;
                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = previousZeroIndex + 3;
                triangles[triangleIndex++] = vertexIndex + 3;
                triangles[triangleIndex++] = previousZeroIndex + 2;

                triangles[triangleIndex++] = previousZeroIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = previousZeroIndex + 1;
                triangles[triangleIndex++] = previousZeroIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 1;

                vertexIndex += 4;
            }

            vertexIndex -= 4;

            triangles[triangleIndex++] = vertexIndex;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 3;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 3;

            standMesh.vertices = vertices;
            standMesh.triangles = triangles;
            Unwrapping.GenerateSecondaryUVSet(standMesh);

            standMesh.RecalculateBounds();
            standMesh.RecalculateNormals();
            standMesh.RecalculateTangents(); 
            
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!AssetDatabase.IsValidFolder($"Assets/Scenes/{sceneName}"))
            {
                AssetDatabase.CreateFolder("Assets/Scenes", $"{sceneName}");
            }
            if (!AssetDatabase.IsValidFolder($"Assets/Scenes/{sceneName}/Meshes"))
            {
                AssetDatabase.CreateFolder($"Assets/Scenes/{sceneName}", "Meshes");
            }
            string path = $"Assets/Scenes/{sceneName}/Meshes/{sceneName}_{_targetComponent.name}_stand.asset";
            AssetDatabase.CreateAsset(standMesh, path);
            AssetDatabase.SaveAssets();

            Mesh loadedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
            loadedMesh.RecalculateNormals();

            _targetComponent.SetNewStandMesh(loadedMesh);
        }

        private float GetUvScale(float length)
        {
            return 0.3f;

            Debug.Log($"GetUvScale {length} / {_materialSize}");

            float scale = length / _materialSize;
            //scale = Mathf.Clamp01(scale);

            return scale;
        }
    }
}