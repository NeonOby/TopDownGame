using UnityEngine;
using System.Collections;

namespace SimpleLibrary
{
	public class RandomPositionSpawner : MonoBehaviour
	{
		public PoolInfo CubePool;
		public SimpleLibrary.Timer SpawnTimer;
		public SimpleLibrary.Timer SpawnTimer2;
		public SimpleLibrary.Timer SpawnTimer3;

		[Range(0f, 1f)]
		public float LerpValue = 0f;

		public Vector3 MinPosition, MaxPosition;

		void Update()
		{
			LerpValue = Mathf.Sin(Time.time);

			if (SpawnTimer.UpdateAutoReset(LerpValue))
			{
				Vector3 position;
				position.x = Random.Range(MinPosition.x, MaxPosition.x);
				position.y = Random.Range(MinPosition.y, MaxPosition.y);
				position.z = Random.Range(MinPosition.z, MaxPosition.z);
				SimpleLibrary.SimplePoolManager.Spawn(CubePool, position);
			} 
			if (SpawnTimer2.UpdateAutoReset(LerpValue))
			{
				Vector3 position;
				position.x = Random.Range(MinPosition.x, MaxPosition.x);
				position.y = Random.Range(MinPosition.y, MaxPosition.y);
				position.z = Random.Range(MinPosition.z, MaxPosition.z);
				SimpleLibrary.SimplePoolManager.Spawn(CubePool, position);
			}
			if (SpawnTimer3.UpdateAutoReset(LerpValue))
			{
				Vector3 position;
				position.x = Random.Range(MinPosition.x, MaxPosition.x);
				position.y = Random.Range(MinPosition.y, MaxPosition.y);
				position.z = Random.Range(MinPosition.z, MaxPosition.z);
				SimpleLibrary.SimplePoolManager.Spawn(CubePool, position);
			}
		}
	}
}

