using System;
//using System.Data;
using System.Collections;
using System.Collections.Generic;
using Rhino;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

//using DotSpatial.Data;
using NetTopologySuite;
using NetTopologySuite.IO;

using GH_IO;

namespace Grasshopper
{    
    public class ScriptableComponent
    {
        public GH_Component Component { get; set; }      
        
        public ScriptableComponent(GH_Component component)
        {
            Component = component;
            Component.Message = "Load Shapefile";
        }
        
        public void RunScript (List<object> input, List<object> output)
        {
            // Input Casting:
            var file = (string) input[0];
            
            
            // Logic:
            
            
            //var geomFactory = new NetTopologySuite.Geometries.GeometryFactory().PrecisionModel;
            //Console.Write(geomFactory.ToString());

            //TODO: current NTS is netstandard2.0; need to change to net45
            
            //try
            //{
            //var foo = Shapefile.CreateDataTable(file, "foo", NetTopologySuite.Geometries.GeometryFactory.Default);
            //}
            //catch (Exception e)
            //{
                
            //}


                // ! Output
            output.Add("foo");
        }  
    }    
}