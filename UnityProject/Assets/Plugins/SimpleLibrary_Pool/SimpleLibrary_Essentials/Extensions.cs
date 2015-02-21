/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V2
 * 
 */

namespace SimpleLibrary
{
    public static class Extensions
    {
        #region RandomEnumerableEntry
        //(C) Nevermind @ http://programmers.stackexchange.com/a/150642
        //Usage MyGenericList.RandomEntry(t => t.WeightValue)
        public static T RandomEntry<T>(this System.Collections.Generic.IEnumerable<T> enumerable, System.Func<T, float> weightFunc)
        {
            float totalWeight = 0; // this stores sum of weights of all elements before current
            T selected = default(T); // currently selected element
            foreach (var data in enumerable)
            {
                float weight = weightFunc(data); // weight of current element
                float r = UnityEngine.Random.Range(0f, totalWeight + weight); // random value
                if (r >= totalWeight) // probability of this is weight/(totalWeight+weight)
                    selected = data; // it is the probability of discarding last selected element and selecting current one instead
                totalWeight += weight; // increase weight sum
            }

            return selected; // when iterations end, selected is some element of sequence. 
        }
        #endregion

        #region Vector
        public static UnityEngine.Vector4 Round(this UnityEngine.Vector4 vector, int decimalCount = 0)
        {
            vector.x = (float)System.Math.Round(vector.x, decimalCount);
            vector.y = (float)System.Math.Round(vector.y, decimalCount);
            vector.z = (float)System.Math.Round(vector.z, decimalCount);
            vector.w = (float)System.Math.Round(vector.w, decimalCount);
            return vector;
        }
        public static UnityEngine.Vector3 Round(this UnityEngine.Vector3 vector, int decimalCount = 0)
        {
            vector.x = (float)System.Math.Round(vector.x, decimalCount);
            vector.y = (float)System.Math.Round(vector.y, decimalCount);
            vector.z = (float)System.Math.Round(vector.z, decimalCount);
            return vector;
        }
        public static UnityEngine.Vector2 Round(this UnityEngine.Vector2 vector, int decimalCount = 0)
        {
            vector.x = (float)System.Math.Round(vector.x, decimalCount);
            vector.y = (float)System.Math.Round(vector.y, decimalCount);
            return vector;
        }
        #endregion
    }
}


