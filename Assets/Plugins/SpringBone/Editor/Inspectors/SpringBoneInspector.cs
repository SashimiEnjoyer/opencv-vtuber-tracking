using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FUnit
{
    using Inspector;
    using SpringBoneButton = SpringManagerInspector.InspectorButton<SpringBone>;

    // https://docs.unity3d.com/ScriptReference/Editor.html

    [CustomEditor(typeof(SpringBone))]
    [CanEditMultipleObjects]
    public class SpringBoneInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var bone = (SpringBone)target;

            ShowActionButtons(bone);

            GUILayout.Space(16f);
            var newEnabled = EditorGUILayout.Toggle("Enable", bone.enabled);
            if (newEnabled != bone.enabled)
            {
                var targetBones = from targetObject in serializedObject.targetObjects
                                  where targetObject is SpringBone
                                  select (SpringBone)targetObject;

                if (targetBones.Any())
                {
                    Undo.RecordObjects(targetBones.ToArray(), "Change Active State of SpringBone");
                    foreach (var targetBone in targetBones)
                    {
                        targetBone.enabled = newEnabled;
                    }
                }
            }

            var setCount = propertySets.Length;
            for (int setIndex = 0; setIndex < setCount; setIndex++)
            {
                propertySets[setIndex].Show();
            }
            GUILayout.Space(16f);

            serializedObject.ApplyModifiedProperties();

            showOriginalInspector = EditorGUILayout.Toggle("Show Original Inspector", showOriginalInspector);
            if (showOriginalInspector)
            {
                base.OnInspectorGUI();
            }
        }

        // private

        private const int ButtonHeight = 30;

        private SpringBoneButton[] actionButtons;
        private PropertySet[] propertySets;
        private bool showOriginalInspector = false;

        private class PropertySet
        {
            public PropertySet(string newTitle, PropertyInfo[] newProperties)
            {
                title = newTitle;
                properties = newProperties;
            }

            public void Initialize(SerializedObject serializedObject)
            {
                var propertyCount = properties.Length;
                for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    properties[propertyIndex].Initialize(serializedObject);
                }
            }

            public void Show()
            {
                const float Spacing = 16f;

                GUILayout.Space(Spacing);
                GUILayout.Label(title, GUILayout.Height(ButtonHeight));
                var propertyCount = properties.Length;
                for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
                {
                    properties[propertyIndex].Show();
                }
            }

            private string title;
            private PropertyInfo[] properties;
        }

        private void InitializeActionButtons()
        {
            if (actionButtons == null)
            {
                actionButtons = new SpringBoneButton[] {
                    new SpringBoneButton("Select Spring Manager", SelectSpringManager),
                    new SpringBoneButton("Select Pivot Node", SelectPivotNode)
                };
            }
        }

        private void ShowActionButtons(SpringBone bone)
        {
            InitializeActionButtons();
            var buttonCount = actionButtons.Length;
            var buttonHeight = GUILayout.Height(ButtonHeight);
            for (int buttonIndex = 0; buttonIndex < buttonCount; buttonIndex++)
            {
                actionButtons[buttonIndex].Show(bone, buttonHeight);
            }
        }

        private void OnEnable()
        {
            InitializeActionButtons();

            var forceProperties = new PropertyInfo[] {
                new PropertyInfo("stiffnessForce", "Stiffness Force"),
                new PropertyInfo("dragForce", "Drag Force"),
                new PropertyInfo("springForce", "Spring Force"),
                new PropertyInfo("windInfluence", "Wind Influence")
            };

            var angleLimitProperties = new PropertyInfo[] {
                new PropertyInfo("pivotNode", "Pivot Node"),
                new PropertyInfo("angularStiffness", "Angular Stiffness"),
                new AngleLimitPropertyInfo("yAngleLimits", "Y Angle Limits"),
                new AngleLimitPropertyInfo("zAngleLimits", "Z Angle Limits")
            };

            var lengthLimitProperties = new PropertyInfo[] {
                new PropertyInfo("lengthLimitTargets", "Length Limit Targets")
            };

            var collisionProperties = new PropertyInfo[] {
                new PropertyInfo("radius", "Radius"),
                new PropertyInfo("sphereColliders", "Sphere Colliders"),
                new PropertyInfo("capsuleColliders", "Capsule Colliders"),
                new PropertyInfo("panelColliders", "Plane Colliders")
            };

            propertySets = new PropertySet[] {
                new PropertySet("Force Properties", forceProperties), 
                new PropertySet("Angle Limit Properties", angleLimitProperties),
                new PropertySet("Length Limit Properties", lengthLimitProperties),
                new PropertySet("Collision Properties", collisionProperties),
            };

            foreach (var set in propertySets)
            {
                set.Initialize(serializedObject);
            }
        }

        private static void SelectSpringManager(SpringBone bone)
        {
            var manager = bone.gameObject.GetComponentInParent<SpringManager>();
            if (manager != null)
            {
                Selection.objects = new Object[] { manager.gameObject };
            }
        }

        private static void SelectPivotNode(SpringBone bone)
        {
            var pivotObjects = new List<GameObject>();
            foreach (var gameObject in Selection.gameObjects)
            {
                var springBone = gameObject.GetComponent<SpringBone>();
                if (springBone != null
                    && springBone.pivotNode != null)
                {
                    pivotObjects.Add(springBone.pivotNode.gameObject);
                }
            }
            Selection.objects = pivotObjects.ToArray();
        }
    }
}