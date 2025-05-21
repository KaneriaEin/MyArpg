using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MeshGenerator
{
    public static Mesh GenerateFanMesh(float insideRadius, float radius, float height, float angle)
    {
        Mesh fanMesh = new Mesh();
        Vector3 centerPos = Vector3.zero;
        Vector3 direction = Vector3.forward;
        Vector3 rightDir = Quaternion.AngleAxis(angle / 2, Vector3.up) * direction;
        float deltaAngle = 2.5f;
        int rect = (int)(angle / deltaAngle);       // 面的数量
        int lines = rect + 1;                // 两边线的数量
        Vector3[] vertexs = new Vector3[2 * lines *2];
        int[] triangles = new int[6 * rect * 4 + 12];

        // 底面  三角形顶点顺序 逆时针(从下网上看是顺时针)
        for(int i = 0; i < lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * i, Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * radius;
            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            // 处理三角形，因为lines比rect多1
            // 1 2 0 \ 1 3 2
            if(i < lines - 1)
            {
                triangles[i * 6 + 0] = i * 2 + 1;
                triangles[i * 6 + 1] = i * 2 + 2;
                triangles[i * 6 + 2] = i * 2 + 0;
                triangles[i * 6 + 3] = i * 2 + 1;
                triangles[i * 6 + 4] = i * 2 + 3;
                triangles[i * 6 + 5] = i * 2 + 2;
            }
            
        }

        // 顶面  三角形顶点顺序 顺时针(从上往下看是顺时针)
        for(int i = lines; i < 2 * lines; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(-deltaAngle * (i - lines), Vector3.up) * rightDir;
            Vector3 minPos = centerPos + dir * insideRadius;
            Vector3 maxPos = centerPos + dir * radius;

            minPos.y += height;
            maxPos.y += height;
            vertexs[i * 2] = minPos;
            vertexs[i * 2 + 1] = maxPos;

            // 处理三角形，因为lines比rect多1 
            // 1 2 0 \ 1 3 2
            if(i < 2 * lines - 1)
            {
                triangles[i * 6 + 0] = i * 2 + 2;
                triangles[i * 6 + 1] = i * 2 + 1;
                triangles[i * 6 + 2] = i * 2 + 0;
                triangles[i * 6 + 3] = i * 2 + 3;
                triangles[i * 6 + 4] = i * 2 + 1;
                triangles[i * 6 + 5] = i * 2 + 2;
            }
        }

        // 右面
        int start = 2 * lines - 1;
        triangles[start * 6 + 0] = 0;
        triangles[start * 6 + 1] = 2 * lines;
        triangles[start * 6 + 2] = 1;
        triangles[start * 6 + 3] = 2 * lines;
        triangles[start * 6 + 4] = 2 * lines + 1;
        triangles[start * 6 + 5] = 1;

        // 左面
        int end = lines - 1;
        triangles[end * 6 + 0] = 2 * lines - 2;
        triangles[end * 6 + 1] = 2 * lines - 1;
        triangles[end * 6 + 2] = 4 * lines - 2;
        triangles[end * 6 + 3] = 4 * lines - 2;
        triangles[end * 6 + 4] = 2 * lines - 1;
        triangles[end * 6 + 5] = 4 * lines - 1;

        // 后面(靠近玩家)
        start = 2 * lines;
        for (int i = 0; i < rect; i++, start++)
        {
            triangles[start * 6 + 0] = 2 * i + 2 * lines + 2;
            triangles[start * 6 + 1] = 2 * i + 2 * lines;
            triangles[start * 6 + 2] = 2 * i + 2;
            triangles[start * 6 + 3] = 2 * i + 2;
            triangles[start * 6 + 4] = 2 * i + 2 * lines;
            triangles[start * 6 + 5] = 2 * i;
        }

        // 前面(远离玩家)
        for (int i = 0; i < rect; i++, start++)
        {
            triangles[start * 6 + 0] = 2 * i + 2 * lines + 1;
            triangles[start * 6 + 1] = 2 * i + 3;
            triangles[start * 6 + 2] = 2 * i + 1;
            triangles[start * 6 + 3] = 2 * i + 3;
            triangles[start * 6 + 4] = 2 * i + 2 * lines + 1;
            triangles[start * 6 + 5] = 2 * i + 2 * lines + 3;
        }

        fanMesh.vertices = vertexs;
        fanMesh.triangles = triangles;
        fanMesh.RecalculateNormals();

        return fanMesh;
    }
}
