/*Author: Tobias Zimmerlin
 * 30.01.2015
 * V1
 * 
 */


using UnityEngine;
using System.Collections;
using System;

namespace SimpleLibrary
{
	//Monobehaviour Singleton for Unity use
    public class Singleton<ChildType> : MonoBehaviour where ChildType : MonoBehaviour
    {
        //Never use this directly, thats why its private NOT protected
        private static ChildType instance = null;

		public static bool HasInstance
		{
			get
			{
				return instance != null;
			}
		}

        //Use this to get the current single instance of this type
        public static ChildType Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<ChildType>();
                if (instance == null)
                {
                    Debug.Log(string.Format("There was no object of type <{0}>", typeof(ChildType)));
                }
                return (ChildType)instance;
            }
            set
            {
                instance = value;
            }
        }
        protected virtual void Awake()
        {
            Instance = this as ChildType;
        }
    }
}