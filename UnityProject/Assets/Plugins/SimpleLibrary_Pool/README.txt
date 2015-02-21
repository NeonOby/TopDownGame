This is SimpleLibrary Pool

It contains a simple pooling System for GameObjects
Set up Pools using the SimplePoolManager 
(Found in GameObject => SimpleLibary => SimplePoolManager)
Add Pools and then set them up.


It contains easy startup but complex setup IF you want.

Easy:
1. GameObject >= SimpleLibrary => SimplePoolManager => SimplePool
2. Automatically creates manager and connections for you
3. Set up pool name
4. Set up wanted prefab and drag into prefab field
5. Add "PoolInfo" variable to your spawner
6. Use SimplePool.Spawn(PoolInfo) to spawn one object
You can also give it position and rotation afterwards. Spawn(PoolInfo, position, rotation)
7. Select wanted pool in inspector of spawner

