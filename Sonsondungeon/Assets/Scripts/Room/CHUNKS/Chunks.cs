using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunks {

	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;
	public List <Vector3> Positions { get; set; }
	void BuildChunk()
	{
		chunkData = new Block[SsimpleVisualizer.chunkSize+1,3,SsimpleVisualizer.chunkSizez+1];

		for(int z = 0; z < SsimpleVisualizer.chunkSizez+1; z++)
			for(int y = 0; y < 3; y++)
				for(int x = 0; x < SsimpleVisualizer.chunkSize+1; x++)
				{
					Vector3 pos = new Vector3(x,y,z);
						chunkData[x,y,z] = new Block(Block.BlockType.DIRT, pos, 
						                chunk.gameObject, this);
				}
	}

	public void DrawChunk()
	{
		for(int z = 0; z < SsimpleVisualizer.chunkSizez+1; z++)
			for(int y = 0; y < 3; y++)
				for(int x = 0; x < SsimpleVisualizer.chunkSize+1; x++)
				{
					chunkData[x,y,z].Draw();	
				}
		CombineQuads();
	}

	// Use this for initialization
	public Chunks (Vector3 position, Material c) {
		Positions = new List <Vector3>();
		chunk = new GameObject(SsimpleVisualizer.BuildChunkName(position));
		chunk.transform.position = position;
	    Positions.Add(position);
		cubeMaterial = c;
		BuildChunk();
	}
	
	void CombineQuads()
	{
		//1. Combine all children meshes
		MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //2. Create a new mesh on the parent object
        MeshFilter mf = (MeshFilter) chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        //3. Add combined meshes on children as the parent's mesh
        mf.mesh.CombineMeshes(combine);

        //4. Create a renderer for the parent
		MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = cubeMaterial;

		//5. Delete all uncombined children
		foreach (Transform quad in chunk.transform) {
     		GameObject.Destroy(quad.gameObject);
 		}

	}

}
