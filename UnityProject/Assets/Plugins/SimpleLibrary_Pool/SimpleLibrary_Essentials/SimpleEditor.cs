/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SimpleLibrary
{
    public class SimpleEditor
    {

        public static float EditorLineHeight
        {
            get
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }

		public static void SplitBox(float height = 10f, float boxHeightDiff = 4f)
		{
			Rect boxRect = GUILayoutUtility.GetRect(0, height);
			boxRect.y += 2;
			boxRect.height -= boxHeightDiff;
			GUI.Box(boxRect, "");
		}

    }
}
#endif