// Copyright information can be found in the file named COPYING
// located in the root directory of this distribution.

// An implementation of "Practical Morphological Anti-Aliasing" from 
// GPU Pro 2 by Jorge Jimenez, Belen Masia, Jose I. Echevarria, 
// Fernando Navarro, and Diego Gutierrez.
//
// http://www.iryoku.com/mlaa/


uniform sampler2D edgesMap  : register(S0);
uniform sampler2D edgesMapL : register(S1);
uniform sampler2D areaMap   : register(S2);

#include "./functions.hlsl"


float4 main(float2 texcoord : TEXCOORD0) : COLOR0 
{
   float4 areas = 0.0;

   float2 e = tex2D(edgesMap, texcoord).rg;	

   [branch]
   if (e.g) // Edge at north
   { 
      // Search distances to the left and to the right:
      float2 d = float2(SearchXLeft(texcoord), SearchXRight(texcoord));

      // Now fetch the crossing edges. Instead of sampling between edgels, we
      // sample at -0.25, to be able to discern what value has each edgel:
      float4 coords = mad(float4(d.x, -0.25, d.y + 1.0, -0.25),
                                 PIXEL_SIZE.xyxy, texcoord.xyxy);
      float e1 = tex2Dlevel0(edgesMapL, coords.xy).r;
      float e2 = tex2Dlevel0(edgesMapL, coords.zw).r;

      // Ok, we know how this pattern looks like, now it is time for getting
      // the actual area:
      areas.rg = Area(abs(d), e1, e2);
   }

   [branch]
   if (e.r) // Edge at west
   {
      // Search distances to the top and to the bottom:
      float2 d = float2(SearchYUp(texcoord), SearchYDown(texcoord));

      // Now fetch the crossing edges (yet again):
      float4 coords = mad(float4(-0.25, d.x, -0.25, d.y + 1.0),
                                 PIXEL_SIZE.xyxy, texcoord.xyxy);
      float e1 = tex2Dlevel0(edgesMapL, coords.xy).g;
      float e2 = tex2Dlevel0(edgesMapL, coords.zw).g;

      // Get the area for this direction:
      areas.ba = Area(abs(d), e1, e2);
   }

   return areas;
}