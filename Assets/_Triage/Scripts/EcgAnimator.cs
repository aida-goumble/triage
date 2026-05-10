using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class EcgAnimator : MonoBehaviour
{
    [Header("Dimensions")]
    public int texWidth  = 512;
    public int texHeight = 60;

    [Header("Apparence")]
    public Color lineColor = new Color(0.55f, 0.77f, 0.55f, 0.15f);
    public Color bgColor   = new Color(0f, 0f, 0f, 0f);

    [Header("Vitesse")]
    public float speed = 80f;

    private Texture2D tex;
    private Color[]   pixels;
    private int[]     ecgPattern;
    private float     offset;

    void Start()
    {
        GetComponent<RawImage>().color = Color.white;
        tex            = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Repeat;
        pixels         = new Color[texWidth * texHeight];
        ecgPattern     = BuildPattern();
        GetComponent<RawImage>().texture = tex;
    }

    void Update()
    {
        offset = (offset + speed * Time.deltaTime) % texWidth;
        DrawEcg();
    }

    void DrawEcg()
    {
        // Efface
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = bgColor;

        int mid = texHeight / 2;

        for (int x = 0; x < texWidth; x++)
        {
            int patternX = (int)(x + offset) % ecgPattern.Length;
            int yCur     = mid + ecgPattern[patternX];
            int yPrev    = mid + ecgPattern[((int)(x + offset - 1) + ecgPattern.Length) % ecgPattern.Length];

            int yMin = Mathf.Clamp(Mathf.Min(yCur, yPrev), 0, texHeight - 1);
            int yMax = Mathf.Clamp(Mathf.Max(yCur, yPrev), 0, texHeight - 1);

            for (int y = yMin; y <= yMax; y++)
                pixels[y * texWidth + x] = lineColor;
        }

        tex.SetPixels(pixels);
        tex.Apply();
    }

    int[] BuildPattern()
    {
        int[] p = new int[120];
        // Ligne de base
        for (int i = 0; i < 120; i++) p[i] = 0;
        // P wave
        p[20] = -2; p[21] = -4; p[22] = -2;
        // Complexe QRS
        p[35] = 3;
        p[36] = -20;
        p[37] = 18;
        p[38] = 2;
        // T wave
        p[48] = -3; p[49] = -6; p[50] = -7;
        p[51] = -6; p[52] = -3;
        return p;
    }

    void OnDestroy()
    {
        if (tex) Destroy(tex);
    }
}