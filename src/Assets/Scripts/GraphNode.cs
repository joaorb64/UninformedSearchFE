using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode {
	
	public GraphNode up;
	public GraphNode down;
	public GraphNode left;
	public GraphNode right;

	public int cost;

	public GraphNode(GraphNode up, GraphNode down, GraphNode left, GraphNode right, int cost){
		this.up = up;
		this.down = down;
		this.left = left;
		this.right = right;
		this.cost = cost;
	}
}
