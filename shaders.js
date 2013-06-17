/*
 * WebGL core teaching framework 
 * (C)opyright Hartmut Schirmacher, hschirmacher.beuth-hochschule.de 
 *
 * Module: Shaders
 *
 * This module loads required shaders using the require.js text plugin, 
 * see https://github.com/requirejs/text 
 *
 */


/* requireJS module definition */
define(["text!shaders/vertex_color.vs", "text!shaders/vertex_color.fs",
        "text!shaders/phong.vs",        "text!shaders/phong.fs",
        "text!shaders/planet.vs",        "text!shaders/planet.fs"
       ], 
       (function( vertex_color_vs, vertex_color_fs,
                  phong_vs,        phong_fs,
                  planet_vs,       planet_fs
                 ) {

    "use strict";
    
    // return source code of a vertex shader
    var shaders = function(name) {
        var shader = eval(name);
        if(!shader) {
            throw "module shaders: shader " + name + " undefined.";
        }
        return shader;
    };
    
    // module returns the function shaders
    return shaders;    
    
})); // define module
        

