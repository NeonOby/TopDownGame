using UnityEngine;
using System.Collections;

namespace SimpleLibrary
{
	public class Cube : MonoBehaviour
	{
		public bool Spawned = false;
		public float Speed = 2f;
		public float MaxXPosition = 20f;


		void Update()
		{
			if (!Spawned)
				return;
			transform.position += Vector3.right * Time.deltaTime * Speed;
			if (transform.position.x > MaxXPosition)
			{
				SimpleLibrary.SimplePoolManager.Despawn(gameObject);
			}
		}

		void OnSpawn()
		{
			Spawned = true;
		}
		void OnDespawn()
		{
			Spawned = false;
		}
	}
}