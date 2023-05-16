# UnityAnimeDash

Litte Dash effect.
You first have to attach the deformation.cs script to your object, then attach the LerpVertex compute shader and the meshFilter to the deformation script.
Then put the material on your object.

You then have 4 parameters : 
  - Lerp = length of the trail, the lower the slower it will get back to it initial state.
  - Noise Spread = add noise to the direction of the trails.
  - Length randomness = add randomness to the length of you trail.
  - Modulo = the lower, the more trail you will have. Use power of 2 for this parameters if you want constant result.
  

https://github.com/PierrePALMER/UnityAnimeDash/assets/72413533/69bb9227-8dd4-4e00-a5e2-36f18ec47587

