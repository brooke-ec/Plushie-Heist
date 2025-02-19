using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EdgeRenderer : Graphic
{
    public List<Vector2> points;
    public float thickness = 10f;

    public void AddNewPoints(Vector2 parentPos, Vector2 childPos, Color32 lineColour)
    {
        points = new List<Vector2> { parentPos, childPos };
        color = lineColour; // Set to desired color
        SetVerticesDirty(); // Ensures proper rendering
    }

    public void ChangeColourOfEdgeRenderer(Color32 colour)
    {
        color = colour;
        SetVerticesDirty();
    }


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if(points.Count<2)
        {
            return;
        }

        float angle = 0;

        //plot two vertices at each point
        for(int i=0; i<points.Count -1; i++)
        {
            if (i<points.Count - 1)
            {
                angle = GetAngle(points[i], points[i + 1]) +90f;
            }

            DrawVerticesForPoint(points[i], points[i + 1], vh, angle, color);
        }

        //draw triangles of all vertices
        //plotting only two at a time so we ignore the last vertex
        for(int i=0; i<points.Count-1; i++)
        {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }

    }

    private float GetAngle(Vector2 source, Vector2 target)
    {
        //gets angle between two points
        return (float)(Mathf.Atan2(9f * (target.y - source.y), 16f * (target.x - source.x)) * (180 / Mathf.PI));
    }

    private void DrawVerticesForPoint(Vector2 point, Vector2 point2, VertexHelper vh, float angle, Color32 colorPassed)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = colorPassed;

        //this is to rotate vertices at the origin and offset it by the position
        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(point2.x, point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(point2.x, point2.y);
        vh.AddVert(vertex);
    }
}
