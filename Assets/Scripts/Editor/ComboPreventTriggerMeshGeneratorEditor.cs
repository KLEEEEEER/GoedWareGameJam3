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

        private int _childsCount = 0;
        private Transform[] _childObjects;

        private ComboPreventTriggerMeshGenerator _targetComponent;

        private void OnEnable()
        {
            _targetComponent = (ComboPreventTriggerMeshGenerator)target;

            _height = serializedObject.FindProperty("_height");
            _radius = serializedObject.FindProperty("_radius");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            List<Transform> childObjectsList = new List<Transform>(_targetComponent.GetComponentsInChildren<Transform>());
            childObjectsList.Remove(_targetComponent.transform);
            _childObjects = childObjectsList.ToArray();

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

            GUILayout.Label($"Childen amount: {_childObjects.Length}");

            if (GUILayout.Button("Generate mesh"))
            {
                Mesh mesh = new Mesh();

                int childObjectCount = _childObjects.Length;

                if (childObjectCount <= 1) 
                {
                    Debug.LogError($"There is less than 2 children objects on {_targetComponent.name} object to generate mesh. Needs more.");
                    return;
                }

                Debug.Log($"Generating mesh for {_childObjects.Length - 1} objects");

                Vector3[] vertices = new Vector3[childObjectCount * 4];
                Vector2[] uv = new Vector2[vertices.Length];
                int[] triangles = new int[((childObjectCount - 1) * 6 * 3) + 2 * 3 + 2 * 3]; // 2 at the start, 2 at the end and 6 for each point except last.

                int currentIndex = 0;
                Vector3 perpendicular = Vector3.Cross((_childObjects[currentIndex + 1].position - _childObjects[currentIndex].position).normalized, _childObjects[currentIndex].up);

                //Debug.Log($"perpendicular is {perpendicular}");

                Vector3 localPosition = _childObjects[currentIndex].localPosition;

                int vertexIndex = 0;
                int triangleIndex = 0;

                vertices[vertexIndex] = localPosition + perpendicular * _radius.floatValue;
                vertices[vertexIndex + 1] = vertices[vertexIndex] + Vector3.up * _height.floatValue;
                vertices[vertexIndex + 2] = localPosition - perpendicular * _radius.floatValue;
                vertices[vertexIndex + 3] = vertices[vertexIndex + 2] + Vector3.up * _height.floatValue;

                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex + 2;
                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = vertexIndex + 3;
                triangles[triangleIndex++] = vertexIndex + 2;

                vertexIndex += 4;

                for (int childObjectIndex = 1; childObjectIndex < _childObjects.Length; childObjectIndex++)
                {
                    Vector3 childObjectLocalPosition = _childObjects[childObjectIndex].localPosition;
                    if (childObjectIndex + 1 < _childObjects.Length)
                    {
                        Vector3 prevPerpendicular = Vector3.Cross((_childObjects[childObjectIndex - 1].position - _childObjects[childObjectIndex].position).normalized, _childObjects[childObjectIndex].up);
                        Vector3 nextPerpendicular = Vector3.Cross((_childObjects[childObjectIndex + 1].position - _childObjects[childObjectIndex].position).normalized, _childObjects[childObjectIndex].up);

                        perpendicular = (nextPerpendicular - prevPerpendicular).normalized;
                    }

                    if (childObjectIndex == _childObjects.Length - 1)
                    {
                        perpendicular = Vector3.Cross((_childObjects[childObjectIndex - 1].position - _childObjects[childObjectIndex].position).normalized, _childObjects[childObjectIndex].up);
                        perpendicular *= -1;
                        Debug.Log($"perpendicular on last is: {perpendicular}");
                    }

                    vertices[vertexIndex] = childObjectLocalPosition + perpendicular * _radius.floatValue;
                    vertices[vertexIndex + 1] = vertices[vertexIndex] + Vector3.up * _height.floatValue;
                    vertices[vertexIndex + 2] = childObjectLocalPosition - perpendicular * _radius.floatValue;
                    vertices[vertexIndex + 3] = vertices[vertexIndex + 2] + Vector3.up * _height.floatValue;

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

            if (GUILayout.Button("Generate colliders"))
            {
                if (_childObjects.Length <= 1)
                {
                    Debug.LogError($"There is less than 2 children objects on {_targetComponent.name} object to generate colliders. Needs more.");
                    return;
                }

                for (int i = 0; i < _childObjects.Length - 1; i++)
                {
                    _childObjects[i].LookAt(_childObjects[i + 1]);

                    float magnitude = (_childObjects[i + 1].position - _childObjects[i].position).magnitude;

                    if (!_childObjects[i].gameObject.TryGetComponent(out BoxCollider collider))
                    {
                        collider = _childObjects[i].gameObject.AddComponent<BoxCollider>();
                    }
                    collider.center = new Vector3(0f, _height.floatValue / 2f, magnitude / 2f);
                    collider.size = new Vector3(_radius.floatValue * 2f, _height.floatValue, magnitude);
                    collider.isTrigger = true;
                }
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
                Handles.DrawLine(points[i].position + Vector3.up * _height.floatValue, points[i + 1].position + Vector3.up * _height.floatValue);
            }
        }


        private Vector2[] CreateUV(ref Mesh mesh)
        {

            int i = 0;
            Vector3 p = Vector3.up;
            Vector3 u = Vector3.Cross(p, Vector3.forward);
            if (Vector3.Dot(u, u) < 0.001f)
            {
                u = Vector3.right;
            }
            else
            {
                u = Vector3.Normalize(u);
            }

            Vector3 v = Vector3.Normalize(Vector3.Cross(p, u));
            Vector3[] vertexs = mesh.vertices;
            int[] tris = mesh.triangles;
            Vector2[] uvs = new Vector2[vertexs.Length];

            for (i = 0; i < tris.Length; i += 3)
            {

                Vector3 a = vertexs[tris[i]];
                Vector3 b = vertexs[tris[i + 1]];
                Vector3 c = vertexs[tris[i + 2]];
                Vector3 side1 = b - a;
                Vector3 side2 = c - a;
                Vector3 N = Vector3.Cross(side1, side2);

                N = new Vector3(Mathf.Abs(N.normalized.x), Mathf.Abs(N.normalized.y), Mathf.Abs(N.normalized.z));



                if (N.x > N.y && N.x > N.z)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].z, vertexs[tris[i]].y);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].z, vertexs[tris[i + 1]].y);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].z, vertexs[tris[i + 2]].y);
                }
                else if (N.y > N.x && N.y > N.z)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].x, vertexs[tris[i]].z);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].x, vertexs[tris[i + 1]].z);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].x, vertexs[tris[i + 2]].z);
                }
                else if (N.z > N.x && N.z > N.y)
                {
                    uvs[tris[i]] = new Vector2(vertexs[tris[i]].x, vertexs[tris[i]].y);
                    uvs[tris[i + 1]] = new Vector2(vertexs[tris[i + 1]].x, vertexs[tris[i + 1]].y);
                    uvs[tris[i + 2]] = new Vector2(vertexs[tris[i + 2]].x, vertexs[tris[i + 2]].y);
                }

            }

            mesh.uv = uvs;
            return uvs;
        }
    }
}