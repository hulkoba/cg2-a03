/*
  *
 * Module main: CG2 Aufgabe 3
 * (C)opyright Hartmut Schirmacher, hschirmacher.beuth-hochschule.de 
 *
 */


/* 
 *  RequireJS alias/path configuration (http://requirejs.org/)
 */

requirejs.config({
    paths: {
    
        // jquery library
        "jquery": [
            // try content delivery network location first
            'http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min',
            //If the load via CDN fails, load locally
            '../lib/jquery-1.7.2.min'],
            
        // gl-matrix library
        "gl-matrix": "../lib/gl-matrix-1.3.7"

    }
});


/*
 * The function defined below is the "main" module,
 * it will be called once all prerequisites listed in the
 * define() statement are loaded.
 *
 */

/* requireJS module definition */
define(["jquery", "gl-matrix", "util", "scene", "animation", "html_controller", "scene_explorer"], 
       (function($, glmatrix, util, Scene, Animation, HtmlController, SceneExplorer ) {

    "use strict";
    
    /*
     * create an animation that rotates the scene around 
     * the Y axis over time. 
     */
    var makeAnimation = function(scene) {
    
        // create animation to rotate the scene
        var animation = new Animation( (function(t, deltaT) {

            // rotation a bit around Y axis, depending on animation time
            var angle = deltaT/1000 * animation.customSpeed / 180*Math.PI; // 10 deg/sec, in radians
            mat4.rotate(scene.worldTransformation, angle, [0,1,0]);
            
            // (re-) draw the scene
            scene.draw();
            
        } )); // end animation callback

        // set an additional attribute that can be controlled from the outside
        animation.customSpeed = 20; 

        return animation;
    
    };

    $(document).ready( (function() {
    
        // create WebGL context object for the named canvas object
        var gl = util.makeWebGLContext("drawing_area");
                                        
        // create scene, create animation, and draw once
        var scene = new Scene(gl);
        scene.draw();        

        // animate rotation of sun around the earth ;-)
        var animation = new Animation( (function(t,deltaT) {
        
            this.customSpeed = this.customSpeed || 10;
            var angle = deltaT/1000*this.customSpeed * Math.PI / 180;
            
            // rotate the sunlight around the Y axis
            mat4.rotate(scene.sunNode.transformation, angle, [0,1,0] );
        
            // redraw
            scene.draw();
            
        }));

        // create HtmlController that takes care of all interaction
        // of HTML elements with the scene and the animation
        var controller = new HtmlController(scene,animation); 

        // create scene explorer handling all mouse events for the canvas
        var canvas = $("#drawing_area").get(0);
        var explorer = new SceneExplorer(canvas, true, scene);
        
    })); // $(document).ready()


    
    
})); // define module
        

