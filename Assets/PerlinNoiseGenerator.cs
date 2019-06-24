using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//生成一个方块
public class PerlinNoiseGenerator
{
    public static int BlockFaces = 0;
    public static Texture2D noiseHeightMap;
    //长宽的地图
    int texWidth = 200, texHeight = 200;

    //噪声缩放值 值越大越密集
    float scale1 = 1f;
    float scale2 = 10f;
    float scale3 = 20f;

    //随机采样偏移
    float offectX;
    float offectY;

    public PerlinNoiseGenerator()
    {
        offectX = Random.Range(0, 99999);
        offectY = Random.Range(0, 99999);
    }

    /// <summary>
    /// 根据长短创建200X200的每一个点
    /// </summary>
    /// <returns></returns>
    public Texture2D GenerateHeightMap()
    {
        Texture2D heightMap = new Texture2D(texWidth, texHeight);

        for (int i = 0; i < texWidth; i++)
        {
            for (int j = 0; j < texHeight; j++)
            {
                Color color = CakculateColor(i, j);
                heightMap.SetPixel(i, j, color);
            }
        }

        heightMap.Apply();
        return heightMap;
    }

    /// <summary>
    /// 用unity自带的2维柏林噪声计算每个点的偏移值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    Color CakculateColor(int x, int y)
    {
        //根据我们的偏移值 计算出类似于波形的图 就是黑白黑白间隔的图
        float xCoord1 = (float)x / texWidth * scale1 + offectX;
        float yCoord1 = (float)y / texHeight * scale1 + offectY;
        float xCoord2 = (float)x / texWidth * scale2 + offectX;
        float yCoord2 = (float)y / texHeight * scale2 + offectY;
        float xCoord3 = (float)x / texWidth * scale3 + offectX;
        float yCoord3 = (float)y / texHeight * scale3 + offectY;

        //返回值为0.0 ~ 1.0之间的小数 可能会略大于一
        float sample1 = Mathf.PerlinNoise(xCoord1, yCoord1) / 15;
        float sample2 = Mathf.PerlinNoise(xCoord2, yCoord2) / 15;
        float sample3 = Mathf.PerlinNoise(xCoord3, yCoord3) / 15;

        return new Color(sample1 + sample2 + sample3, sample1 + sample2 + sample3, sample1 + sample2 + sample3);
    }
}
