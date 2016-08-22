# GenericThreading

C# Generic Class for easily setting up multithreading in Unity Engine.

# How to Use

Just copy the entire script into your project, than create as many instances of ThreadedWorker as you need. To start an asyncronous job in a new thread just call RequestWork passing in you custom input data and the function that sould manipulate them, toghether with a callback to manipulate results from the main thread (as Unity prevents you from doing thing like accessing a Transform or instantiating things from different threads) ; at the end the result will be put on a queue toghether with the callback: just call ClearThreadInfoQueue to run them all.

# Sample Code

The sample code below uses a separate thread for generating a heightmap, a colormap and a mesh; than it stores them in a struct and pass it to the callback which instantiate the mesh and the texture (you can do this kind of things only from Unity's main thread).

```C#
  ThreadedWorker<NoiseInput, MeshInfo> worker = new ThreadedWorker<NoiseInput, MeshInfo>();

  UnityAction<MeshInfo> callback = new UnityAction<MeshInfo>((MeshInfo meshInfo) =>
    {
      targetRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromColors32(meshInfo.Colors, width, height);
      target.GetComponent<MeshFilter>().sharedMesh = meshInfo.GetMesh();     
    });

  Func<NoiseInput, MeshInfo> work = new Func<NoiseInput, MeshInfo>((NoiseInput input) =>
    {
      float[,] noise = NoiseGenerator.GenerateNoise(input);
      Color32[] colors = ColorsFromRegions(noise);
      MeshData meshData = MeshGenerator.MeshFromHeightmap(noise);
      return new MeshInfo(meshData, colors);
    });

  worker.RequestWork(noiseInput, work, callback);
```
