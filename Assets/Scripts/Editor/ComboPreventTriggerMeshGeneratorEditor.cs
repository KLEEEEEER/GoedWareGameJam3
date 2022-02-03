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

            Vector3[] vertices = new Vector3[childObjectCount * 4];

            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[((childObjectCount - 1) * 6 * 3) + 2 * 3 + 2 * 3]; // 2 at the start, 2 at the end and 6 for each point except last.

            int currentIndex = 0;
            Vector3 perpendicular = Vector3.Cross((childObjects[currentIndex + 1].position - childObjects[currentIndex].position).normalized, childObjects[currentIndex].up);

            //Debug.Log($"perpendicular is {perpendicular}");

            Vector3 localPosition = childObjects[currentIndex].localPosition;

            int vertexIndex = 0;
            int triangleIndex = 0;

            vertices[vertexIndex] = localPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 1] = vertices[vertexIndex] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 2] = localPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
            vertices[vertexIndex + 3] = vertices[vertexIndex + 2] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);

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

                vertices[vertexIndex] = childObjectLocalPosition + perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                vertices[vertexIndex + 1] = vertices[vertexIndex] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);
                vertices[vertexIndex + 2] = childObjectLocalPosition - perpendicular * _radius.floatValue + (Vector3.up * _standHeight.floatValue);
                vertices[vertexIndex + 3] = vertices[vertexIndex + 2] + Vector3.up * _height.floatValue + (Vector3.up * _standHeight.floatValue);

                triangles[triangleIndex++] = vertexIndex - 4;
                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex - 4;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex - 4 + 1;

                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex - 4 + 2;
                triangles[triangleIndex++] = vertexIndex - 4 + 3;
                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex - 4 + 3;
                triangles[triangleIndex++] = vertexIndex + 3;

                triangles[triangleIndex++] = vertexIndex - 4 + 1;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex + 3;
                triangles[triangleIndex++] = vertexIndex + 3;
                triangles[triangleIndex++] = vertexIndex - 4 + 3;
                triangles[triangleIndex++] = vertexIndex - 4 + 1;

                vertexIndex += 4;
            }

            vertexIndex -= 4;
            triangles[triangleIndex++] = vertexIndex;
            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 1;
            triangles[triangleIndex++] = vertexIndex + 2;
            triangles[triangleIndex++] = vertexIndex + 3;

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            Unwrapping.GenerateSecondaryUVSet(mesh);

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

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
    }
}