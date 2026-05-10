using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScanlinesEffect : MonoBehaviour
{
    [Header("Apparence")]
    public Color lineColor = new Color(0f, 0f, 0f, 0.08f);
    public int   lineEvery = 4;

    void Start()
    {
        GetComponent<RawImage>().color = Color.white;
        int w = Screen.width;
        int h = Screen.height;

        Texture2D tex    = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode   = FilterMode.Point;
        Color transparent = new Color(0f, 0f, 0f, 0f);

        for (int y = 0; y < h; y++)
        {
            Color c = (y % lineEvery == 0) ? lineColor : transparent;
            for (int x = 0; x < w; x++)
                tex.SetPixel(x, y, c);
        }

        tex.Apply();
        GetComponent<RawImage>().texture = tex;
    }
}