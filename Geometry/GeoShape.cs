// #region copyright
// // Copyright (C) Daniel Fink 2019 - All Rights Reserved.
// // Unauthorized copying of this file (via any medium) is strictly prohibited.
// // Proprietary and confidential.
// // Written by Daniel Fink <daniel@placeful.io>, 2018.
// #endregion copyright
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// using Rhino;
// using Rhino.Geometry;
// using Rhino.DocObjects;
// using Rhino.Collections;
//
// using Grasshopper;
// using Grasshopper.Kernel.Types;
//
// using GeoAPI;
// using GeoAPI.Geometries;
// using ProjNet.CoordinateSystems;
//
// using NetTopologySuite.Geometries;
//
//
// namespace Antelope
// {
//     /// <summary>
//     /// A Shape with geospatial information; maps to a GIS Polygon
//     /// </summary>
//     [Serializable]
//     public class GeoShape : Brep, IGeospatialGeom
//     {
//         public ProjectionSystem ProjectionSystem { get; private set; }
//         //public GeometryBase Geometry
//         //{
//         //    get
//         //    {
//         //        return this.GetGeometry();
//         //    }
//         //    private set
//         //    {
//
//         //    }
//         //}
//
//         public GeoLineString Boundary
//         {
//             get
//             {
//                 return this.GetGeoBoundary();
//             }
//             private set
//             {
//
//             }
//         }
//
//         // public MultiGeoLineString Holes
//         // {
//         //     get
//         //     {
//         //         return this.GetGeoHoles();
//         //     }
//         //     private set
//         //     {
//         //
//         //     }
//         // }
//
//         //private GeometryBase GetGeometry()
//         //{
//         //    return (Brep)this.Value;
//         //}
//
//         private GeoLineString GetGeoBoundary()
//         {
//             return new GeoLineString(this.ExtractBoundaryPolyline(), this.ProjectionSystem);
//         }
//
//         // private MultiGeoLineString GetGeoHoles()
//         // {
//         //     return new MultiGeoLineString(this.ExtractHolePolylines(), this.ProjectionSystem);
//         // }
//
//         public GeoShape(Brep brep, ProjectionSystem sourceProjection) : base(brep)
//         {
//             //this.Boundary = this.GetBoundary();
//             //this.Holes = this.GetHoles();
//             this.ProjectionSystem = sourceProjection;
//
//         }
//
//         public GeoShape(NetTopologySuite.Geometries.Polygon geoPolygon, Plane sourceBasis) : base(new PlanarBrep(geoPolygon.ToBrep()))
//         {
//             //this.Value = geoPolygon.ToBrep();
//             //this.Boundary = this.GetBoundary();
//             //this.Holes = this.GetHoles();
//             this.ProjectionSystem = new ProjectionSystem(geoPolygon.SRID, sourceBasis);
//         }
//
//         public GeoShape(LinearRing linearRing, Plane sourceBasis) : base(new PlanarBrep(linearRing.ToBrep()))
//         {
//             //this.Value = linearRing.ToBrep();
//             //this.Boundary = this.GetBoundary();
//             //this.Holes = this.GetHoles();
//             this.ProjectionSystem = new ProjectionSystem(linearRing.SRID, sourceBasis);
//         }
//
//         private Transform TargetTransform()
//         {
//             return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
//         }
//
//         /// <summary>
//         /// Transforms an GeoShape from its source basis to Rhino's origin basis (target).
//         /// </summary>
//         public void ToTargetBasis()
//         {
//             this.Transform(this.TargetTransform());
//         }
//
//         /// <summary>
//         /// Transforms an GeoShape from its target basis to its source basis.
//         /// </summary>
//         public void ToSourceBasis()
//         {
//             Transform inverseTransform;
//             this.TargetTransform().TryGetInverse(out inverseTransform);
//             this.Transform(inverseTransform);
//         }
//     }
//
//     // [Serializable]
//     // public class MultiGeoShape : GH_GeometryGroup, IGeospatialGeom
//     // {
//     //     public ProjectionSystem ProjectionSystem { get; private set; }
//     //     public GeometryBase[] Geometries
//     //     {
//     //         get
//     //         {
//     //             return this.GetGeometries();
//     //         }
//     //         private set
//     //         {
//     //
//     //         }
//     //     }
//     //
//     //     private GeometryBase[] GetGeometries()
//     //     {
//     //         var geoms = this.Objects.Select(geom => (GeoShape)geom);
//     //         return geoms.Select(geom => (GeometryBase)geom).ToArray();
//     //     }
//     //
//     //     public void Add(GeoShape geoShape)
//     //     {
//     //         this.Objects.Add(geoShape);
//     //     }
//     //
//     //     public MultiGeoShape(IEnumerable<GeoShape> geoShapes)
//     //     {
//     //         foreach (var shape in geoShapes)
//     //         {
//     //             this.Objects.Add(shape);
//     //         }
//     //         var projSystems = geoShapes.Select(p => p.ProjectionSystem);
//     //         this.ProjectionSystem = projSystems.Condense<ProjectionSystem>();
//     //         this.Geometries = this.GetGeometries();
//     //     }
//     //
//     //     private Transform TargetTransform()
//     //     {
//     //         return Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, this.ProjectionSystem.SourceBasis);
//     //     }
//     //
//     //     public void ToTargetBasis()
//     //     {
//     //         this.Objects.Select(ls => ls.Transform(this.TargetTransform()));
//     //     }
//     //
//     //     public void ToSourceBasis()
//     //     {
//     //         Transform inverseTransform;
//     //         this.TargetTransform().TryGetInverse(out inverseTransform);
//     //         this.Objects.Select(ls => ls.Transform(inverseTransform));
//     //     }
//     // }
// }
