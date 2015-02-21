/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SimpleLibrary
{
    [CustomPropertyDrawer(typeof(Timer))]
    public class Timer_Editor : PropertyDrawer
    {
        //The height of this property is based on how many child-properties are gonna be drawn
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int lineCount = 1;

            SerializedProperty Foldout = property.FindPropertyRelative("Foldout");
            if (Foldout.boolValue)
            {
                lineCount += 1;

                SerializedProperty MyType = property.FindPropertyRelative("MyType");

                switch ((Timer.TimerType)MyType.enumValueIndex)
                {
                    case Timer.TimerType.CONST:
                        lineCount += 1;
                        break;
                    case Timer.TimerType.LERP_TWO_CONSTANTS:
                        lineCount += 4;
                        break;
                    case Timer.TimerType.RANDOM_TWO_CONSTANTS:
                        lineCount += 3;
                        break;
                    case Timer.TimerType.LERP_RANDOM_FOUR_CONSTANTS:
                        lineCount += 6;
                        break;
                    case Timer.TimerType.LERP_CURVE:
                        lineCount += 4;
                        break;
                    case Timer.TimerType.RANDOM_CURVE:
                        lineCount += 3;
                        break;
                    case Timer.TimerType.RANDOM_TWO_CURVES:
                        lineCount += 4;
                        break;
                    case Timer.TimerType.LERP_RANDOM_TWO_CURVES:
                        lineCount += 5;
                        break;
                    default:
                        break;
                }
            }

            return SimpleEditor.EditorLineHeight * lineCount;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            
            SerializedProperty Foldout = property.FindPropertyRelative("Foldout");
            SerializedProperty LerpValue = property.FindPropertyRelative("LerpValue");

            SerializedProperty MyType = property.FindPropertyRelative("MyType");

            SerializedProperty Time1 = property.FindPropertyRelative("Time1");
            SerializedProperty Time2 = property.FindPropertyRelative("Time2");
            SerializedProperty Time3 = property.FindPropertyRelative("Time3");
            SerializedProperty Time4 = property.FindPropertyRelative("Time4");

            SerializedProperty Curve1 = property.FindPropertyRelative("Curve1");
            SerializedProperty Curve2 = property.FindPropertyRelative("Curve2");

            SerializedProperty ValueMultiplier = property.FindPropertyRelative("ValueMultiplier");

            SerializedProperty timer = property.FindPropertyRelative("timer");
            SerializedProperty CurrentTimeValue = property.FindPropertyRelative("CurrentTimeValue");

            float procentage = 0f;
            if (CurrentTimeValue.floatValue > 0f)
                procentage = Mathf.Clamp01(timer.floatValue / CurrentTimeValue.floatValue);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            string foldOutLabel = string.Format("{0}", label.text);

            //float foldOutNameWidth= foldOutLabel.Length * 7f;
            float progressWidth = 100f;

            Rect rectFoldout = new Rect(rect.x, rect.y, rect.width - progressWidth, SimpleEditor.EditorLineHeight);
            Rect rectProgressbar = new Rect(rect.x + rectFoldout.width, rect.y, progressWidth, SimpleEditor.EditorLineHeight);

            Foldout.boolValue = EditorGUI.Foldout(rectFoldout, Foldout.boolValue, new GUIContent(foldOutLabel, label.image, label.tooltip));
            EditorGUI.ProgressBar(rectProgressbar, procentage, string.Format("{0:0.0} => {1:0.0}", timer.floatValue, CurrentTimeValue.floatValue));

            if (Foldout.boolValue)
            {
                Rect rect1 = new Rect(rect.x, rect.y + SimpleEditor.EditorLineHeight, rect.width, SimpleEditor.EditorLineHeight);
                Rect rect2 = rect1;
                rect2.y += SimpleEditor.EditorLineHeight;
                Rect rect3 = rect2;
                rect3.y += SimpleEditor.EditorLineHeight;
                Rect rect4 = rect3;
                rect4.y += SimpleEditor.EditorLineHeight;
                Rect rect5 = rect4;
                rect5.y += SimpleEditor.EditorLineHeight;
                Rect rect6 = rect5;
                rect6.y += SimpleEditor.EditorLineHeight;
                Rect rect7 = rect6;
                rect7.y += SimpleEditor.EditorLineHeight;

                EditorGUI.PropertyField(rect1, MyType);

                AnimationCurve curve1, curve2;
                float value1 = 0f;
                float value2 = 0f;
                float value3 = 0f;
                float value4 = 0f;

                switch ((Timer.TimerType)MyType.enumValueIndex)
                {
                    case Timer.TimerType.CONST:
                        EditorGUI.PropertyField(rect2, Time1);
                        break;
                    case Timer.TimerType.LERP_TWO_CONSTANTS:
                        EditorGUI.PropertyField(rect2, Time1, new GUIContent("MinValue"));
                        EditorGUI.PropertyField(rect3, Time2, new GUIContent("MaxValue"));
                        EditorGUI.Slider(rect4, LerpValue, 0f, 1f, new GUIContent("Test LerpValue"));
                        EditorGUI.LabelField(rect5, new GUIContent(string.Format("Expected Timer Value: {0:0.###}", Mathf.Lerp(Time1.floatValue, Time2.floatValue, LerpValue.floatValue))));
                        break;
                    case Timer.TimerType.RANDOM_TWO_CONSTANTS:
                        EditorGUI.PropertyField(rect2, Time1);
                        EditorGUI.PropertyField(rect3, Time2);
                        EditorGUI.LabelField(rect4, new GUIContent(string.Format("Expected Timer Value Between: {0:0.###} => {1:0.###}", Time1.floatValue, Time2.floatValue)));
                        break;
                    case Timer.TimerType.LERP_RANDOM_FOUR_CONSTANTS:
                        EditorGUI.PropertyField(rect2, Time1, new GUIContent("MinValue1"));
                        EditorGUI.PropertyField(rect3, Time2, new GUIContent("MinValue2"));
                        EditorGUI.PropertyField(rect4, Time3, new GUIContent("MaxValue1"));
                        EditorGUI.PropertyField(rect5, Time4, new GUIContent("MaxValue2"));
                        EditorGUI.Slider(rect6, LerpValue, 0f, 1f, new GUIContent("Test LerpValue"));
                        EditorGUI.LabelField(rect7, new GUIContent(string.Format("Expected Timer Value Between: {0:0.###} => {1:0.###}", Mathf.Lerp(Time1.floatValue, Time2.floatValue, LerpValue.floatValue), Mathf.Lerp(Time3.floatValue, Time4.floatValue, LerpValue.floatValue))));
                        break;
                    case Timer.TimerType.LERP_CURVE:
                        EditorGUI.PropertyField(rect2, Curve1);
                        EditorGUI.PropertyField(rect3, ValueMultiplier);
                        EditorGUI.Slider(rect4, LerpValue, 0f, 1f, new GUIContent("Test LerpValue"));
                        curve1 = Curve1.animationCurveValue;
                        if (curve1.keys.Length > 0)
                        {
                            value1 = curve1.Evaluate(Mathf.Lerp(curve1.keys[0].time, curve1.keys[curve1.keys.Length - 1].time, LerpValue.floatValue));

                            value1 *= ValueMultiplier.floatValue;
                        }
                        EditorGUI.LabelField(rect5, new GUIContent(string.Format("Expected Timer Value: {0:0.###}", value1)));
                        break;
                    case Timer.TimerType.RANDOM_CURVE:
                        EditorGUI.PropertyField(rect2, Curve1);
                        EditorGUI.PropertyField(rect3, ValueMultiplier);
                        curve1 = Curve1.animationCurveValue;
                        if (curve1.keys.Length > 0)
                        {
                            value1 = curve1.Evaluate(curve1.keys[0].time);
                            value2 = curve1.Evaluate(curve1.keys[curve1.keys.Length - 1].time);

                            value1 *= ValueMultiplier.floatValue;
                            value2 *= ValueMultiplier.floatValue;
                        }
                        EditorGUI.LabelField(rect4, new GUIContent(string.Format("Expected Timer Value Between: {0:0.###} => {1:0.###}", value1, value2)));
                        break;
                    case Timer.TimerType.LERP_RANDOM_TWO_CURVES:
                        EditorGUI.PropertyField(rect2, Curve1);
                        EditorGUI.PropertyField(rect3, Curve2);
                        EditorGUI.PropertyField(rect4, ValueMultiplier);
                        EditorGUI.Slider(rect5, LerpValue, 0f, 1f, new GUIContent("Test LerpValue"));
                        curve1 = Curve1.animationCurveValue;
                        curve2 = Curve2.animationCurveValue;
                        if (curve1.keys.Length > 0 && curve2.keys.Length > 0)
                        {
                            //First get min (value1) and max (value2) time
                            value1 = curve1.keys[0].time;
                            if (curve2.keys[0].time < value1)
                                value1 = curve2.keys[0].time;

                            value2 = curve1.keys[curve1.keys.Length - 1].time;
                            if (curve2.keys[curve2.keys.Length - 1].time > value2)
                                value2 = curve2.keys[curve2.keys.Length - 1].time;

                            //find time value (lerp with lerpValue)
                            value3 = Mathf.Lerp(value1, value2, LerpValue.floatValue);

                            //Now Evaluate with both curves and find smallest and highest value
                            value1 = curve1.Evaluate(value3);
                            if (curve2.Evaluate(value3) < value1)
                                value1 = curve2.Evaluate(value3);

                            value2 = curve1.Evaluate(value3);
                            if (curve2.Evaluate(value3) > value2)
                                value2 = curve2.Evaluate(value3);

                            value1 *= ValueMultiplier.floatValue;
                            value2 *= ValueMultiplier.floatValue;
                        }
                        EditorGUI.LabelField(rect6, new GUIContent(string.Format("Expected Timer Value Between: {0:0.###} => {1:0.###}", value1, value2)));
                        break;
                    case Timer.TimerType.RANDOM_TWO_CURVES:
                        EditorGUI.PropertyField(rect2, Curve1);
                        EditorGUI.PropertyField(rect3, Curve2);
                        EditorGUI.PropertyField(rect4, ValueMultiplier);
                        curve1 = Curve1.animationCurveValue;
                        curve2 = Curve2.animationCurveValue;
                        if (curve1.keys.Length > 0 && curve2.keys.Length > 0)
                        {
                            //First get min (value1) and max (value2) time
                            value1 = curve1.keys[0].time;
                            if (curve2.keys[0].time < value1)
                                value1 = curve2.keys[0].time;

                            value2 = curve1.keys[curve1.keys.Length - 1].time;
                            if (curve2.keys[curve2.keys.Length - 1].time > value2)
                                value2 = curve2.keys[curve2.keys.Length - 1].time;

                            //save min and max time values
                            value3 = value1;
                            value4 = value2;

                            //Now Evaluate with both curves and find smallest and highest value
                            value1 = curve1.Evaluate(value3);
                            if (curve2.Evaluate(value3) < value1)
                                value1 = curve2.Evaluate(value3);

                            value2 = curve1.Evaluate(value4);
                            if (curve2.Evaluate(value4) > value2)
                                value2 = curve2.Evaluate(value4);

                            value1 *= ValueMultiplier.floatValue;
                            value2 *= ValueMultiplier.floatValue;
                        }
                        EditorGUI.LabelField(rect5, new GUIContent(string.Format("Expected Timer Value Between: {0:0.###} => {1:0.###}", value1, value2)));
                        break;
                    default:
                        break;
                }
            }

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
#endif