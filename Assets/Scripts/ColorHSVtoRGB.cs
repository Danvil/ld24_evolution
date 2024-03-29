/***
 * http://stackoverflow.com/questions/2942/hsl-in-net
 */

using UnityEngine;

public class ColorHSVtoRGB
{
  // Given H,S,L in range of 0-1
  // Returns a Color (RGB struct) in range of 0-255
  public static Color FromHSL(float H, float S, float L)
  {
    return FromHSLA(H, S, L, 1.0f);
  }

  // Given H,S,L,A in range of 0-1
  // Returns a Color (RGB struct) in range of 0-255
  public static Color FromHSLA(float H, float S, float L, float A)
  {
    float v;
    float r, g, b;
    if (A > 1.0f)
      A = 1.0f;

    r = L;   // default to gray
    g = L;
    b = L;
    v = (L <= 0.5f) ? (L * (1.0f + S)) : (L + S - L * S);
    if (v > 0)
    {
      float m;
      float sv;
      int sextant;
      float fract, vsf, mid1, mid2;

      m = L + L - v;
      sv = (v - m) / v;
      H *= 6.0f;
      sextant = (int)H;
      fract = H - sextant;
      vsf = v * sv * fract;
      mid1 = m + vsf;
      mid2 = v - vsf;
      switch (sextant)
      {
        case 0:
          r = v;
          g = mid1;
          b = m;
          break;
        case 1:
          r = mid2;
          g = v;
          b = m;
          break;
        case 2:
          r = m;
          g = v;
          b = mid1;
          break;
        case 3:
          r = m;
          g = mid2;
          b = v;
          break;
        case 4:
          r = mid1;
          g = m;
          b = v;
          break;
        case 5:
          r = v;
          g = m;
          b = mid2;
          break;
      }
    }
    return new Color(r,g,b,A);
  }
}
