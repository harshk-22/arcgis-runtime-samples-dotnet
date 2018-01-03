// Copyright 2018 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific 
// language governing permissions and limitations under the License.

using Android.App;
using Android.OS;
using Android.Widget;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Drawing;

namespace ArcGISRuntimeXamarin.Samples.FeatureLayerExtrusion
{
    [Activity]
    public class FeatureLayerExtrusion : Activity
    {
        // Create and hold reference to the used MapView
        private SceneView _mySceneView = new SceneView();

        private Button _button1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Feature layer extrusion";

            // Create the UI, setup the control references 
            CreateLayout();

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                // Define the Uri for the service feature table (US state polygons)
                var myServiceFeatureTable_Uri = new Uri("https://sampleserver6.arcgisonline.com/arcgis/rest/services/Census/MapServer/3");

                // Create a new service feature table from the Uri
                ServiceFeatureTable myServiceFeatureTable = new ServiceFeatureTable(myServiceFeatureTable_Uri);

                // Create a new feature layer from the service feature table
                FeatureLayer myFeatureLayer = new FeatureLayer(myServiceFeatureTable);

                // Set the rendering mode of the feature layer to be dynamic (needed for extrusion to work)
                myFeatureLayer.RenderingMode = FeatureRenderingMode.Dynamic;

                // Create a new simple line symbol for the feature layer
                SimpleLineSymbol mySimpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.Black, 1);

                // Create a new simple fill symbol for the feature layer 
                SimpleFillSymbol mysimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, Color.Blue, mySimpleLineSymbol);

                // Create a new simple renderer for the feature layer
                SimpleRenderer mySimpleRenderer = new SimpleRenderer(mysimpleFillSymbol);

                // Get the scene properties from the simple renderer
                RendererSceneProperties myRendererSceneProperties = mySimpleRenderer.SceneProperties;

                // Set the extrusion mode for the scene properties to be base height
                myRendererSceneProperties.ExtrusionMode = ExtrusionMode.BaseHeight;

                // Set the initial extrusion expression
                myRendererSceneProperties.ExtrusionExpression = "[POP2007] / 10";

                // Set the feature layer's renderer to the define simple renderer
                myFeatureLayer.Renderer = mySimpleRenderer;

                // Create a new scene with the topographic backdrop 
                Scene myScene = new Scene(BasemapType.Topographic);

                // Set the scene view's scene to the newly create one
                _mySceneView.Scene = myScene;

                // Add the feature layer to the scene's operational layer collection
                myScene.OperationalLayers.Add(myFeatureLayer);

                // Create a new map point to define where to look on the scene view
                MapPoint myMapPoint = new MapPoint(-10974490, 4814376, 0, SpatialReferences.WebMercator);

                // Create a new orbit location camera controller using the map point and defined distance
                OrbitLocationCameraController myOrbitLocationCameraController = new OrbitLocationCameraController(myMapPoint, 20000000);

                // Set the scene view's camera controller to the orbit location camera controller
                _mySceneView.CameraController = myOrbitLocationCameraController;
            }
            catch (Exception ex)
            {
                // Something went wrong, display the error
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error");
                alert.SetMessage(ex.Message);
                alert.Show();
            }
        }

        private void ChangeExtrusionExpression()
        {
            // Get the first layer from the scene view's operation layers, it should be a feature layer
            FeatureLayer myFeatureLayer = (FeatureLayer)_mySceneView.Scene.OperationalLayers[0];

            // Get the renderer from the feature layer
            Renderer myRenderer = myFeatureLayer.Renderer;

            // Get the scene properties from the feature layer's renderer
            RendererSceneProperties myRendererSceneProperties = myRenderer.SceneProperties;

            // Toggle the feature layer's scene properties renderer extrusion expression and change the button text
            if (_button1.Text == "Population Density")
            {
                myRendererSceneProperties.ExtrusionExpression = "[POP07_SQMI] * 5000";
                _button1.Text = "Total Population";
            }
            else if (_button1.Text == "Total Population")
            {
                myRendererSceneProperties.ExtrusionExpression = "[POP2007] / 10";
                _button1.Text = "Population Density";
            }
        }

        private void Button1_Clicked(object sender, EventArgs e)
        {

            // Call the function to change the feature layer's renderer scene properties extrusion expression
            ChangeExtrusionExpression();
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app
            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

            // Create Button
            _button1 = new Button(this);
            _button1.Text = "Population Density";
            _button1.Click += Button1_Clicked;

            // Add Button to the layout  
            layout.AddView(_button1);

            // Add the scene view to the layout
            layout.AddView(_mySceneView);

            // Show the layout in the app
            SetContentView(layout);
        }
    }
}