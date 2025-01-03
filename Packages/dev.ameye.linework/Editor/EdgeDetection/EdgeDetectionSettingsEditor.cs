using Linework.EdgeDetection;
using Linework.Editor.Common.Utils;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Resolution = Linework.EdgeDetection.Resolution;

namespace Linework.Editor.EdgeDetection
{
    [CustomEditor(typeof(EdgeDetectionSettings))]
    public class EdgeDetectionSettingsEditor : UnityEditor.Editor
    {
        private SerializedProperty injectionPoint;
        private SerializedProperty showInSceneView;
        private SerializedProperty debugView;
        private SerializedProperty debugSectionsRaw;

        // Discontinuity.
        private SerializedProperty discontinuityInput;
        private SerializedProperty depthSensitivity;
        private SerializedProperty depthDistanceModulation;
        private SerializedProperty grazingAngleMaskPower;
        private SerializedProperty grazingAngleMaskHardness;
        private SerializedProperty normalSensitivity;
        private SerializedProperty luminanceSensitivity;
        private SerializedProperty sectionRenderingLayer;
        private SerializedProperty objectId;
        private SerializedProperty particles;
        private SerializedProperty sectionsMask;
        private SerializedProperty depthMask;
        private SerializedProperty normalsMask;
        private SerializedProperty luminanceMask;
        private SerializedProperty sectionMapInput;
        private SerializedProperty sectionTexture;
        private SerializedProperty sectionTextureUvSet;
        private SerializedProperty vertexColorChannel;

        // Outline.
        private SerializedProperty kernel;
        private SerializedProperty outlineThickness;
        private SerializedProperty scaleWithResolution;
        private SerializedProperty referenceResolution;
        private SerializedProperty customReferenceResolution;
        private SerializedProperty backgroundColor;
        private SerializedProperty outlineColor;
        private SerializedProperty overrideColorInShadow;
        private SerializedProperty outlineColorShadow;
        private SerializedProperty fillColor;
        private SerializedProperty fadeInDistance;
        private SerializedProperty fadeStart;
        private SerializedProperty fadeDistance;
        private SerializedProperty fadeColor;
        private SerializedProperty blendMode;

        private SerializedProperty showDiscontinuitySection, showOutlineSection;

        private void OnEnable()
        {
            showDiscontinuitySection = serializedObject.FindProperty(nameof(EdgeDetectionSettings.showDiscontinuitySection));
            showOutlineSection = serializedObject.FindProperty(nameof(EdgeDetectionSettings.showOutlineSection));
      
            injectionPoint = serializedObject.FindProperty("injectionPoint");
            showInSceneView = serializedObject.FindProperty("showInSceneView");
            debugView = serializedObject.FindProperty("debugView");
            debugSectionsRaw = serializedObject.FindProperty(nameof(EdgeDetectionSettings.debugSectionsRaw));

            // Discontinuity.
            discontinuityInput = serializedObject.FindProperty(nameof(EdgeDetectionSettings.discontinuityInput));
            depthSensitivity = serializedObject.FindProperty(nameof(EdgeDetectionSettings.depthSensitivity));
            depthDistanceModulation = serializedObject.FindProperty(nameof(EdgeDetectionSettings.depthDistanceModulation));
            grazingAngleMaskPower = serializedObject.FindProperty(nameof(EdgeDetectionSettings.grazingAngleMaskPower));
            grazingAngleMaskHardness = serializedObject.FindProperty(nameof(EdgeDetectionSettings.grazingAngleMaskHardness));
            normalSensitivity = serializedObject.FindProperty(nameof(EdgeDetectionSettings.normalSensitivity));
            luminanceSensitivity = serializedObject.FindProperty(nameof(EdgeDetectionSettings.luminanceSensitivity));
            sectionRenderingLayer = serializedObject.FindProperty(nameof(EdgeDetectionSettings.SectionRenderingLayer));
            objectId = serializedObject.FindProperty(nameof(EdgeDetectionSettings.objectId));
            particles = serializedObject.FindProperty(nameof(EdgeDetectionSettings.particles));
            sectionsMask = serializedObject.FindProperty(nameof(EdgeDetectionSettings.sectionsMask));
            depthMask = serializedObject.FindProperty(nameof(EdgeDetectionSettings.depthMask));
            normalsMask = serializedObject.FindProperty(nameof(EdgeDetectionSettings.normalsMask));
            luminanceMask = serializedObject.FindProperty(nameof(EdgeDetectionSettings.luminanceMask));
            sectionMapInput = serializedObject.FindProperty(nameof(EdgeDetectionSettings.sectionMapInput));
            sectionTexture = serializedObject.FindProperty(nameof(EdgeDetectionSettings.sectionTexture));
            sectionTextureUvSet = serializedObject.FindProperty(nameof(EdgeDetectionSettings.sectionTextureUvSet));
            serializedObject.FindProperty(nameof(EdgeDetectionSettings.sectionTextureChannel));
            vertexColorChannel = serializedObject.FindProperty(nameof(EdgeDetectionSettings.vertexColorChannel));

            // Outline.
            kernel = serializedObject.FindProperty(nameof(EdgeDetectionSettings.kernel));
            outlineThickness = serializedObject.FindProperty(nameof(EdgeDetectionSettings.outlineThickness));
            scaleWithResolution = serializedObject.FindProperty(nameof(EdgeDetectionSettings.scaleWithResolution));
            referenceResolution = serializedObject.FindProperty(nameof(EdgeDetectionSettings.referenceResolution));
            customReferenceResolution = serializedObject.FindProperty(nameof(EdgeDetectionSettings.customResolution));
            backgroundColor = serializedObject.FindProperty(nameof(EdgeDetectionSettings.backgroundColor));
            outlineColor = serializedObject.FindProperty(nameof(EdgeDetectionSettings.outlineColor));
            overrideColorInShadow = serializedObject.FindProperty(nameof(EdgeDetectionSettings.overrideColorInShadow));
            outlineColorShadow = serializedObject.FindProperty(nameof(EdgeDetectionSettings.outlineColorShadow));
            fillColor = serializedObject.FindProperty(nameof(EdgeDetectionSettings.fillColor));
            fadeInDistance = serializedObject.FindProperty(nameof(EdgeDetectionSettings.fadeInDistance));
            fadeStart = serializedObject.FindProperty(nameof(EdgeDetectionSettings.fadeStart));
            fadeDistance = serializedObject.FindProperty(nameof(EdgeDetectionSettings.fadeDistance));
            fadeColor = serializedObject.FindProperty(nameof(EdgeDetectionSettings.fadeColor));
            blendMode = serializedObject.FindProperty(nameof(EdgeDetectionSettings.blendMode));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Edge Detection", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(injectionPoint, EditorUtils.CommonStyles.InjectionPoint);
            EditorGUILayout.PropertyField(showInSceneView, EditorUtils.CommonStyles.ShowInSceneView);
            EditorGUILayout.PropertyField(debugView, EditorUtils.CommonStyles.DebugStage);
            switch ((DebugView) debugView.intValue)
            {
                case DebugView.None:
                    break;
                case DebugView.Depth:
                    if (!((DiscontinuityInput) discontinuityInput.intValue).HasFlag(DiscontinuityInput.Depth))
                    {
                        EditorGUILayout.HelpBox("Depth is not configured as a source. No edges will be detected based on scene depth.", MessageType.Warning);
                    }
                    break;
                case DebugView.Normals:
                    if (!((DiscontinuityInput) discontinuityInput.intValue).HasFlag(DiscontinuityInput.Normals))
                    {
                        EditorGUILayout.HelpBox("Normals is not configured as a source. No edges will be detected based on scene normals.", MessageType.Warning);
                    }
                    break;
                case DebugView.Luminance:
                    if (!((DiscontinuityInput) discontinuityInput.intValue).HasFlag(DiscontinuityInput.Luminance))
                    {
                        EditorGUILayout.HelpBox("Luminance is not configured as a source. No edges will be detected based on scene luminance.", MessageType.Warning);
                    }
                    break;
                case DebugView.Sections:
                    if (!((DiscontinuityInput) discontinuityInput.intValue).HasFlag(DiscontinuityInput.Sections))
                    {
                        EditorGUILayout.HelpBox("Sections is not configured as a source. No edges will be detected based on section map.", MessageType.Warning);
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(debugSectionsRaw, EditorUtils.CommonStyles.SectionsRawValues);
                        EditorGUILayout.HelpBox("White = mask", MessageType.Info);
                        EditorGUI.indentLevel--;
                    }
                    break;
            }
            EditorGUILayout.Space();
            CoreEditorUtils.DrawSplitter();
            serializedObject.ApplyModifiedProperties();
            
            EditorUtils.SectionGUI("Discontinuity", showDiscontinuitySection, () =>
            {
                var discontinuityInputValue = (DiscontinuityInput) discontinuityInput.intValue;
                discontinuityInputValue = (DiscontinuityInput) EditorGUILayout.EnumFlagsField(EditorUtils.CommonStyles.DiscontinuityInput, discontinuityInputValue);
                discontinuityInput.intValue = (int) discontinuityInputValue;
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("Section Map", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(sectionRenderingLayer, EditorUtils.CommonStyles.SectionLayer);
                EditorGUILayout.PropertyField(sectionMapInput, EditorUtils.CommonStyles.SectionMapInput);
                EditorGUI.indentLevel++;

                if ((SectionMapInput) sectionMapInput.intValue == SectionMapInput.VertexColors)
                {
                    EditorGUILayout.PropertyField(vertexColorChannel, EditorUtils.CommonStyles.VertexColorChannel);
                }

                if ((SectionMapInput) sectionMapInput.intValue == SectionMapInput.SectionTexture)
                {
                    EditorGUILayout.PropertyField(sectionTexture, EditorUtils.CommonStyles.SectionTexture);
                    EditorGUILayout.PropertyField(sectionTextureUvSet, EditorUtils.CommonStyles.SectionTextureUVSet);
                    EditorGUILayout.PropertyField(vertexColorChannel, EditorUtils.CommonStyles.SectionTextureChannel);
                }
                EditorGUI.indentLevel--;
                
                using (new EditorGUI.DisabledScope((SectionMapInput) sectionMapInput.intValue == SectionMapInput.Custom))
                {
                    EditorGUILayout.PropertyField(objectId, EditorUtils.CommonStyles.ObjectId);
                    EditorGUILayout.PropertyField(particles, EditorUtils.CommonStyles.Particles);
                }
                EditorGUILayout.Space();
                if ((SectionMapInput) sectionMapInput.intValue == SectionMapInput.Custom)
                {
                    const string keywordMessage = "Custom Section Map: Use the _SECTION_PASS keyword to render directly to the section map.";
                    EditorGUILayout.HelpBox(keywordMessage, MessageType.Info);
                }
                EditorGUILayout.Space();
                
                using (new EditorGUI.DisabledScope(!discontinuityInputValue.HasFlag(DiscontinuityInput.Sections)))
                {
                    EditorGUILayout.LabelField("Sections", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(sectionsMask, EditorUtils.CommonStyles.SectionMask);
                }
                EditorGUILayout.Space();
                
                using (new EditorGUI.DisabledScope(!discontinuityInputValue.HasFlag(DiscontinuityInput.Depth)))
                {
                    EditorGUILayout.LabelField("Depth", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(depthSensitivity, EditorUtils.CommonStyles.Sensitivity);
                    EditorGUILayout.PropertyField(depthDistanceModulation, EditorUtils.CommonStyles.DepthDistanceModulation);
                    EditorGUILayout.PropertyField(grazingAngleMaskPower, EditorUtils.CommonStyles.GrazingAngleMaskPower);
                    EditorGUILayout.PropertyField(grazingAngleMaskHardness, EditorUtils.CommonStyles.GrazingAngleMaskHardness);
                    EditorGUILayout.PropertyField(depthMask, EditorUtils.CommonStyles.SectionMask);
                }
                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(!discontinuityInputValue.HasFlag(DiscontinuityInput.Normals)))
                {
                    EditorGUILayout.LabelField("Normals", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(normalSensitivity, EditorUtils.CommonStyles.Sensitivity);
                    EditorGUILayout.PropertyField(normalsMask, EditorUtils.CommonStyles.SectionMask);
                }
                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(!discontinuityInputValue.HasFlag(DiscontinuityInput.Luminance)))
                {
                    EditorGUILayout.LabelField("Luminance", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(luminanceSensitivity, EditorUtils.CommonStyles.Sensitivity);
                    EditorGUILayout.PropertyField(luminanceMask, EditorUtils.CommonStyles.SectionMask);
                }
                EditorGUILayout.Space();
            }, serializedObject);
            
            EditorUtils.SectionGUI("Outline", showOutlineSection, () =>
            {
                EditorGUILayout.LabelField("Sampling", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(kernel, EditorUtils.CommonStyles.Kernel);
                EditorGUILayout.PropertyField(outlineThickness, EditorUtils.CommonStyles.OutlineThickness);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(scaleWithResolution, EditorUtils.CommonStyles.ScaleWithResolution);
                if (scaleWithResolution.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(referenceResolution, GUIContent.none);
                    if ((Resolution) referenceResolution.intValue == Resolution.Custom) EditorGUILayout.PropertyField(customReferenceResolution, GUIContent.none, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(outlineColor, EditorUtils.CommonStyles.EdgeColor);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(overrideColorInShadow, EditorUtils.CommonStyles.OverrideShadow);
                if (overrideColorInShadow.boolValue) EditorGUILayout.PropertyField(outlineColorShadow, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(backgroundColor, EditorUtils.CommonStyles.BackgroundColor);
                EditorGUILayout.PropertyField(fillColor, EditorUtils.CommonStyles.OutlineFillColor);
                EditorGUILayout.PropertyField(fadeInDistance, EditorUtils.CommonStyles.FadeInDistance);
                if (fadeInDistance.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(fadeStart, EditorUtils.CommonStyles.FadeStart);
                    EditorGUILayout.PropertyField(fadeDistance, EditorUtils.CommonStyles.FadeDistance);
                    EditorGUILayout.PropertyField(fadeColor, EditorUtils.CommonStyles.FadeColor);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(blendMode, EditorUtils.CommonStyles.OutlineBlendMode);
                EditorGUILayout.Space();
            }, serializedObject);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}