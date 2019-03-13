// Mesh Merger Script
// Copyright 2009, Russ Menapace
// http://humanpoweredgames.com
 
// Summary:
//  This script allows you to draw a large number of meshes with a single 
//  draw call.  This is particularly useful for iPhone games.
 
// License:
//  Free to use as you see fit, but I would appreciate one of the following:
//  * A credit for Human Powered Games, or even a link to humanpoweredgames.com
//    in whatever you make with this
//  * Hire me to make games or simulations
//  * A donation to the PayPal account at russ@databar.com.  I'm very poor, so 
//    even a small donation would be greatly appreciated!
//  * A thank you note to russ@databar.com
//  * Suggestions on how the script could be improved mailed to russ@databar.com
 
// Warranty:
//  This software carries no warranty, and I don't guarantee anything about it.
//  If it burns down your house or gets your cat pregnant, don't look at me. 
 
// Acnowledgements: 
//  This was pieced together out of code I found onthe UnifyCommunity wiki, and 
//  the Unity forum.  I did not keep track of names, but I do recall gaining
//  a lot of insight from the posts of PirateNinjaAlliance.  
//  Thanks to anybody that may have been involved.
 
// Requirements:  
//  All the meshes you want to use must use the same material.  
//  This material may be a texture atlas and the meshes UV to portions of the atlas.
//  The texture atlas technique works particularly well for GUI stuff.
 
// Usage:
//  There are two ways to use this script:
 
//  Implicit:  
//    Simply drop the script into a GameObject that has a number of
//    child objects containing mesh filters.
 
//  Explicit:
//    Populate the meshFilter array with the meshes you want merged
//    Optionally, set the material to be used.  If no material is selected,
//    The script will apply the first material it encounters to all subsequent
//    meshes
 
// To see if it's working:
//  Move the camera so you can see several of your objects in the Game pane
//  Note the number of draw calls
//  Hit play. You should see the number of draw calls for those meshes reduced to one
 
using UnityEngine;
using System;
 
//==============================================================================
public class MeshMerger : MonoBehaviour 
{ 
  public MeshFilter[] meshFilters;
  public Material material;
 
  //----------------------------------------------------------------------------
  void Start () 
  { 
    // if not specified, go find meshes
    if(meshFilters.Length == 0)
    {
      // find all the mesh filters
      Component[] comps = GetComponentsInChildren(typeof(MeshFilter));
      meshFilters = new MeshFilter[comps.Length];
 
      int mfi = 0;
      foreach(Component comp in comps)
        meshFilters[mfi++] = (MeshFilter) comp;
    }
 
    // figure out array sizes
    int vertCount = 0;
    int normCount = 0;
    int triCount = 0;
    int uvCount = 0;
 
    foreach(MeshFilter mf in meshFilters)
    {
      vertCount += mf.mesh.vertices.Length; 
      normCount += mf.mesh.normals.Length;
      triCount += mf.mesh.triangles.Length; 
      uvCount += mf.mesh.uv.Length;
      if(material == null)
        material = mf.gameObject.GetComponent<Renderer>().material;       
    }
 
    // allocate arrays
    Vector3[] verts = new Vector3[vertCount];
    Vector3[] norms = new Vector3[normCount];
    Transform[] aBones = new Transform[meshFilters.Length];
    Matrix4x4[] bindPoses = new Matrix4x4[meshFilters.Length];
    BoneWeight[] weights = new BoneWeight[vertCount];
    int[] tris  = new int[triCount];
    Vector2[] uvs = new Vector2[uvCount];
 
    int vertOffset = 0;
    int normOffset = 0;
    int triOffset = 0;
    int uvOffset = 0;
    int meshOffset = 0;
 
    // merge the meshes and set up bones
    foreach(MeshFilter mf in meshFilters)
    {     
      foreach(int i in mf.mesh.triangles)
        tris[triOffset++] = i + vertOffset;
 
      aBones[meshOffset] = mf.transform;
      bindPoses[meshOffset] = Matrix4x4.identity;
 
      foreach(Vector3 v in mf.mesh.vertices)
      {
        weights[vertOffset].weight0 = 1.0f;
        weights[vertOffset].boneIndex0 = meshOffset;
        verts[vertOffset++] = v;
      }
 
      foreach(Vector3 n in mf.mesh.normals)
        norms[normOffset++] = n;
 
      foreach(Vector2 uv in mf.mesh.uv)
        uvs[uvOffset++] = uv;
 
      meshOffset++;
 
      MeshRenderer mr = 
        mf.gameObject.GetComponent(typeof(MeshRenderer)) 
        as MeshRenderer;
 
      if(mr)
        mr.enabled = false;
    }
 
    // hook up the mesh
    Mesh me = new Mesh();       
    me.name = gameObject.name;
    me.vertices = verts;
    me.normals = norms;
    me.boneWeights = weights;
    me.uv = uvs;
    me.triangles = tris;
    me.bindposes = bindPoses;
 
    // hook up the mesh renderer        
    SkinnedMeshRenderer smr = 
      gameObject.AddComponent(typeof(SkinnedMeshRenderer)) 
      as SkinnedMeshRenderer;
 
    smr.sharedMesh = me;
    smr.bones = aBones;
    this.GetComponent<Renderer>().material = material;
 
  }
}