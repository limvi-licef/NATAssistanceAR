/// ---------------------------------------------------------------------
/// Author: Anthony Melin
/// Date: 2019 August 14
/// ---------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Urho;
using Urho.Gui;
using Urho.Shapes;
using Urho.SharpReality;

namespace Annotations
{
    //############################################################################################################
    //#                                            ColorPalette                                                  #
    //#                                                                                                          #
    //#  public static Color FromString(string color)                                                            #
    //#                                                                                                          #
    //############################################################################################################
    /// <summary>
    /// Static class for convert string to UrhoSharp color.
    /// </summary>
    public static class ColorPalette
    {
        //########################################################################################################
        /// <summary>
        /// Get the coresponding color.
        /// </summary>
        /// <param name="color"> color to get </param>
        /// <returns> coresponding UrhoSharp color </returns>
        public static Color FromString(string color)
        {
            if (color == "red") return Color.Red;
            else if (color == "green") return Color.Green;
            else if (color == "blue") return Color.Cyan;
            else if (color == "yellow") return Color.Yellow;
            else if (color == "gray") return Color.Gray;
            else if (color == "alpha") return Color.Transparent;
            else return Color.White;
        }
    }


    //############################################################################################################
    //#                                              Annotation                                                  #
    //#                                                                                                          #
    //#  public static Text3D Text(StereoApplication app, string text, Vector3 pos)                              #
    //#                                                                                                          #
    //############################################################################################################
    /// <summary>
    /// Static class for build displayable UrhoSharp shapes in immersive view on Hololens.
    /// It use UrhoSharp Text3D component.
    /// </summary>
    public static class Annotation
    {
        //########################################################################################################
        /// <summary>
        /// Method to call for build a new Text3D
        /// </summary>
        /// <param name="app"> the current running application that display the scene </param>
        /// <param name="text"> text to display </param>
        /// <param name="pos"> position x y z in the world scene (not relative to user position). The unit is the meter </param>
        /// <returns> UrhoSharp Text3D component </returns>
        public static Text3D Text(StereoApplication app, string text, Vector3 pos)
        {
            // create a new node in the scene and bind a new component to it
            var textNode = app.Scene.CreateChild();
            var textComponent = textNode.CreateComponent<Text3D>();

            textComponent.Node.SetScale(0.1f); // reduce the font size as its too large by default
            textComponent.Node.Position = pos;

            // text alignment
            textComponent.HorizontalAlignment = HorizontalAlignment.Center;
            textComponent.VerticalAlignment = VerticalAlignment.Top;

            textComponent.ViewMask = 0x80000000; //hide from raycasts

            textComponent.SetFont(CoreAssets.Fonts.AnonymousPro, 26); // set font style
            textComponent.Text = text;

            return textComponent; // component
        }


        //########################################################################################################
        /// <summary>
        /// Method to call for build a new disc
        /// </summary>
        /// <param name="app"> the current running application that display the scene </param>
        /// <param name="color"> Urhosharp color </param>
        /// <param name="pos"> position x y z in the world scene (not relative to user position). The unit is the meter </param>
        /// <param name="r"> radius of the cylinder </param>
        /// <returns> UrhoSharp Text3D component </returns>
        public static Cylinder Disc(StereoApplication app, Color color, Vector3 pos, float r)
        {
            // create a new node in the scene and bind a new component to it
            var Node = app.Scene.CreateChild();
            var Component = Node.CreateComponent<Cylinder>();

            // visual settings
            Component.Node.Scale = new Vector3(r, 0.02f, r);
            Component.Node.Position = pos;
            Component.Color = color;

            return Component;
        }

    }



    //############################################################################################################
    //#                                  Text3DAnnotationList : List<Text3D>                                     #
    //#                                                                                                          #
    //#  public Text3DAnnotationList(StereoApplication app)                                                      #
    //#  public void Update(Quaternion cameraRotation)                                                           #
    //#  public void Remove()                                                                                    #
    //#                                                                                                          #
    //############################################################################################################
    /// <summary>
    /// A list for UrhoSharp Text3D component.
    /// Inherited from generic list. Allow to storage generated text and get access to it for update or delete.
    /// </summary>
    public partial class Text3DAnnotationList : List<Text3D>
    {
        /// <summary>
        /// Attribute for access the current running application.
        /// </summary>
        StereoApplication _app;

        //########################################################################################################
        /// <summary>
        /// Constructor of the generic list and set as attribute the current scene.
        /// </summary>
        /// <param name="app"> the current running application that display the scene </param>
        public Text3DAnnotationList(StereoApplication app)
        {
            _app = app;
        }


        //########################################################################################################
        /// <summary>
        /// Method that update the text orientation for facing the user. May be called during frame update.
        /// </summary>
        /// <param name="cameraRotation"> user camera rotation that each text annotation in the list will use for update </param>
        public void Update(Quaternion cameraRotation)
        {
            // loop over annotations
            foreach (var annotation in this)
            {
                annotation.Node.Rotation = cameraRotation; // text orientation is set using its scene child node
            }
        }


        //########################################################################################################
        /// <summary>
        /// Remove all stored text component by calling remove method of each annotation and then clear the list.
        /// </summary>
        public void Remove()
        {
            // loop over annotations
            foreach (var annotation in this)
            {
                annotation.Remove(); // remove annotation from the scene
            }
            Clear(); // clear the list
        }
    }

}