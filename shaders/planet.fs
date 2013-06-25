/*
 * WebGL core teaching framwork 
 * (C)opyright Hartmut Schirmacher, hschirmacher.beuth-hochschule.de 
 *
 * Fragment Shader: planet
 *
 * expects position and normal vectors in eye coordinates per vertex;
 * expects uniforms for ambient light, directional light, and phong material.
 * 
 *
 */

precision mediump float;

// position and normal in eye coordinates
varying vec4  ecPosition;
varying vec3  ecNormal;


varying vec2 vertexTexCoords_fs;


// transformation matrices
uniform mat4  modelViewMatrix;
uniform mat4  projectionMatrix;

// Ambient Light
uniform vec3 ambientLight;

// Material Type
struct PhongMaterial {
    vec3  ambient;
    vec3  diffuse;
    vec3  specular;
    float shininess;
};
// uniform variable for the currently active PhongMaterial
uniform PhongMaterial material;

// Light Source Data for a directional light (not point light)
struct LightSource {

    int  type;
    vec3 direction;
    vec3 color;
    bool on;
    
} ;
uniform LightSource light;


uniform bool debug;
uniform bool worldTexture;
uniform sampler2D daylightTexture;
uniform sampler2D nightlightTexture;

/*

 Calculate surface color based on Phong illumination model.
 - pos:  position of point on surface, in eye coordinates
 - n:    surface normal at pos
 - v:    direction pointing towards the viewer, in eye coordinates
 + assuming directional light
 
 */

vec3 phong(vec3 pos, vec3 n, vec3 v, LightSource light, PhongMaterial material) {

    // vector from light to current point
    vec3 l = normalize(light.direction);
    
    // cosine of angle between light and surface normal. 
    float ndotl = dot(n,-l);

    // ambient part, this is a constant term shown on the
    // all sides of the object
    vec3 ambient = material.ambient * ambientLight;

   

    // is the current fragment's normal pointing away from the light?
    // then we are on the "back" side of the object, as seen from the light
     
    
    if(!debug){
        ambient = material.ambient * ambientLight;
    }
   

    // diffuse contribution
    vec3 dayTex = texture2D(daylightTexture, vertexTexCoords_fs).rgb;
    vec3 nightTex = texture2D(nightlightTexture, vertexTexCoords_fs).rgb;

    vec3 diffuseCoeff = material.diffuse;

    vec3 diffuse ;
  

    //day or red
    if(worldTexture){
        diffuse = dayTex * light.color * ndotl * 0.5;        
        ambient = nightTex * ambientLight;
    } else {
        diffuse = diffuseCoeff * light.color * ndotl;
        ambient = material.ambient * ambientLight;
    }

    //draw the green border between 0 and 3°    
    if(debug && ndotl >= 0.0 && ndotl < 0.03){
        ambient = vec3(0.0, 1.0, 0.0);
    }
    //draw texture stripes 
    if(debug){
        if(mod(vertexTexCoords_fs.s , 0.05) >= 0.025){
            ambient = ambient * 0.8 + diffuse * 0.8;
        }
    }
  
    if(ndotl < 0.0 )
        return ambient;

    

     // reflected light direction = perfect reflection direction
    vec3 r = reflect(l,n);
    
    // cosine of angle between reflection dir and viewing dir
    float rdotv = max( dot(r,v), 0.0);
    
    // specular contribution
    vec3 specularCoeff = material.specular;
    float shininess = material.shininess;
    vec3 specular = specularCoeff * light.color * pow(rdotv, shininess);

   
 
    // return sum of all contributions
    return ambient + diffuse + specular;
    
}

void main() {
    
    // normalize normal after projection
    vec3 normalEC = normalize(ecNormal);
    
    // do we use a perspective or an orthogonal projection matrix?
    bool usePerspective = projectionMatrix[2][3] != 0.0;
    
    // for perspective mode, the viewing direction (in eye coords) points
    // from the vertex to the origin (0,0,0) --> use -ecPosition as direction.
    // for orthogonal mode, the viewing direction is simply (0,0,1)
    vec3 viewdirEC = usePerspective? normalize(-ecPosition.xyz) : vec3(0,0,1);
    
    // calculate color using phong illumination
    vec3 color = phong( ecPosition.xyz, normalEC, viewdirEC,
                        light, material );
    
    // set fragment color
    gl_FragColor = vec4(color, 1.0);
    
}
